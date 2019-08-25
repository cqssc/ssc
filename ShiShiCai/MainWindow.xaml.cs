using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ShiShiCai.Common;
using ShiShiCai.Models;
using ShiShiCai.UserControls;
using Timer = System.Timers.Timer;

namespace ShiShiCai
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region Members

        private bool mIsInited;

        private readonly List<IssueItem> mListIssues = new List<IssueItem>();

        private readonly ObservableCollection<ModuleItem> mListModuleItems = new ObservableCollection<ModuleItem>();
        private readonly ObservableCollection<IssueItem> mListIssueItems = new ObservableCollection<IssueItem>();

        private SystemConfig mSystemConfig;
        private IssueItem mNewestIssueItem;
        private bool mLeftExpanded;
        private int mCalculateMode;
        private int mCalculateSize;
        private string mCalculateDate;
        private bool mLoading;
        private Timer mRefreshTimer;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnLeftExpand.Click += BtnLeftExpand_Click;
            BtnLeftCollaspe.Click += BtnLeftCollaspe_Click;
            BtnHistory.Click += BtnHistory_Click;
            BtnSetting.Click += BtnSetting_Click;
            ListBoxModules.SelectionChanged += ListBoxModules_SelectionChanged;
            PanelLeft.SizeChanged += PanelLeft_SizeChanged;
            SliderScale.ValueChanged += SliderScale_ValueChanged;
            BtnRefresh.Click += BtnRefresh_Click;

            DataContext = this;
            ListBoxModules.ItemsSource = mListModuleItems;
            ListBoxIssues.ItemsSource = mListIssueItems;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {

        }


        #region Init and load

        private void Init()
        {
            WindowState = WindowState.Maximized;

            InitModuleItems();
            LoadConfig();

            ListBoxModules.SelectedIndex = 0;//默认显示模块


            mLoading = true;
            mCalculateMode = SscDefines.CALC_MODE_LAST_LOTTERY;
            mCalculateSize = 300;
            LoadIssues();
            InitIssueItems();
            InitLottery();
            InitModule();
            InitCalculateRange();
            mLoading = false;

            mRefreshTimer = new Timer(3000);
            mRefreshTimer.Elapsed += RefreshTimer_Elapsed;
            mRefreshTimer.Start();
        }

        private void Reload()
        {
            mLoading = true;
            mCalculateMode = SscDefines.CALC_MODE_LAST_LOTTERY;
            mCalculateSize = 300;
            LoadIssues();
            InitIssueItems();
            InitLottery();
            InitModule();
            InitCalculateRange();
            mLoading = false;
        }

        private void Refresh()
        {
            mLoading = true;
            if (mCalculateMode == SscDefines.CALC_MODE_LAST_LOTTERY)
            {
                mListIssues.Insert(0, mNewestIssueItem);
                mListIssueItems.Insert(0, mNewestIssueItem);
                InitLottery();
                RefreshModule();
            }
            InitCalculateRange();
            mLoading = false;
        }

        private void InitModuleItems()
        {
            mListModuleItems.Clear();
            ModuleItem item = new ModuleItem();
            item.Number = SscDefines.MODULE_BASIC;
            item.Name = SscDefines.MODULE_NAME_BASIC;
            item.Title = SscDefines.MODULE_NAME_BASIC;
            item.Icon = "Themes/Default/Images/00009.png";
            mListModuleItems.Add(item);
            item = new ModuleItem();
            item.Number = SscDefines.MODULE_LARGE_SMALL;
            item.Name = SscDefines.MODULE_NAME_LARGE_SMALL;
            item.Title = SscDefines.MODULE_NAME_LARGE_SMALL;
            item.Icon = "Themes/Default/Images/00010.png";
            mListModuleItems.Add(item);
            item = new ModuleItem();
            item.Number = SscDefines.MODULE_HOT;
            item.Name = SscDefines.MODULE_NAME_HOT;
            item.Title = SscDefines.MODULE_NAME_HOT;
            item.Icon = "Themes/Default/Images/00003.png";
            mListModuleItems.Add(item);
            item = new ModuleItem();
            item.Number = SscDefines.MODULE_TREND;
            item.Name = SscDefines.MODULE_NAME_TREND;
            item.Title = SscDefines.MODULE_NAME_TREND;
            item.Icon = "Themes/Default/Images/00004.png";
            mListModuleItems.Add(item);
        }

        private void LoadConfig()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SystemConfig.FILE_NAME);
                OperationReturn optReturn = XMLHelper.DeserializeFile<SystemConfig>(path);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("load system config fail. [{0}]{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                SystemConfig config = optReturn.Data as SystemConfig;
                if (config == null)
                {
                    ShowException(string.Format("system config is null."));
                    return;
                }
                mSystemConfig = config;
            }
            catch (Exception ex)
            {
                ShowException(string.Format("Load system config fail. \r\n{0}", ex.Message));
            }
        }

        private void LoadIssues()
        {
            try
            {
                mListIssues.Clear();
                if (mSystemConfig == null) { return; }
                DatabaseConfig dbConfig = mSystemConfig.Database;
                if (dbConfig == null) { return; }
                string strConn = dbConfig.GetConnectionString();
                if (string.IsNullOrEmpty(strConn)) { return; }
                string strSql = string.Format("SELECT TOP {0} * FROM T_101_19 WHERE C099 = 1 ORDER BY C001 DESC", mCalculateSize);
                OperationReturn optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail. [{0}]{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                DataSet objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null) { return; }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    IssueItem item = new IssueItem();
                    item.Serial = dr["C001"].ToString();
                    item.Number = Convert.ToInt32(dr["C005"]);
                    item.Date = Convert.ToInt32(dr["C004"]);
                    item.WeekDay = Convert.ToInt32(dr["C006"]);

                    item.D1 = Convert.ToInt32(dr["C010"]);
                    item.D2 = Convert.ToInt32(dr["C020"]);
                    item.D3 = Convert.ToInt32(dr["C030"]);
                    item.D4 = Convert.ToInt32(dr["C040"]);
                    item.D5 = Convert.ToInt32(dr["C050"]);

                    item.FullValue = dr["C002"].ToString();
                    item.LargeValue = dr["C007"].ToString() == "1";
                    item.SingleValue = dr["C008"].ToString() == "1";
                    item.SumValue = Convert.ToInt32(dr["C009"]);
                    item.RepeatValue = dr["C100"].ToString() == "1";
                    item.IntervalValue = dr["C101"].ToString() == "1";
                    item.Larger20 = dr["C102"].ToString() == "1";
                    item.AllOne20 = dr["C103"].ToString() == "1";
                    item.PairsVaue = dr["C104"].ToString() == "1";
                    item.SameValue = dr["C105"].ToString() == "1";
                    mListIssues.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadIssuesByDate(string date)
        {
            try
            {
                mListIssues.Clear();
                if (mSystemConfig == null) { return; }
                DatabaseConfig dbConfig = mSystemConfig.Database;
                if (dbConfig == null) { return; }
                string strConn = dbConfig.GetConnectionString();
                if (string.IsNullOrEmpty(strConn)) { return; }
                string strSql = string.Format("SELECT * FROM T_101_19 WHERE C099 = 1 C004 = {0} ORDER BY C001 DESC", date);
                OperationReturn optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail. [{0}]{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                DataSet objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null) { return; }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    IssueItem item = new IssueItem();
                    item.Serial = dr["C001"].ToString();
                    item.Number = Convert.ToInt32(dr["C005"]);
                    item.Date = Convert.ToInt32(dr["C004"]);
                    item.WeekDay = Convert.ToInt32(dr["C006"]);

                    item.D1 = Convert.ToInt32(dr["C010"]);
                    item.D2 = Convert.ToInt32(dr["C020"]);
                    item.D3 = Convert.ToInt32(dr["C030"]);
                    item.D4 = Convert.ToInt32(dr["C040"]);
                    item.D5 = Convert.ToInt32(dr["C050"]);

                    item.FullValue = dr["C002"].ToString();
                    item.LargeValue = dr["C007"].ToString() == "2";
                    item.SingleValue = dr["C008"].ToString() == "2";
                    item.SumValue = Convert.ToInt32(dr["C009"]);
                    item.RepeatValue = dr["C100"].ToString() == "2";
                    item.IntervalValue = dr["C101"].ToString() == "2";
                    item.Larger20 = dr["C102"].ToString() == "2";
                    item.AllOne20 = dr["C103"].ToString() == "2";
                    item.PairsVaue = dr["C104"].ToString() == "2";
                    item.SameValue = dr["C105"].ToString() == "2";
                    mListIssues.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private int LoadLastIssue()
        {
            //判断刷新，返回0：没有刷新；1：全部重新加载；2：刷新一期新数据；-1：报错
            try
            {
                if (mSystemConfig == null) { return 0; }
                DatabaseConfig dbConfig = mSystemConfig.Database;
                if (dbConfig == null) { return 0; }
                string strConn = dbConfig.GetConnectionString();
                if (string.IsNullOrEmpty(strConn)) { return 0; }
                string strSql = string.Format("SELECT TOP 1 * FROM T_101_19 WHERE C099 = 1 ORDER BY C001 DESC");
                OperationReturn optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                if (!optReturn.Result)
                {
                    WriteLog("LoadLastIssue", string.Format("Fail. [{0}]{1}", optReturn.Code, optReturn.Message));
                    return -1;
                }
                DataSet objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null) { return 0; }
                if (objDataSet.Tables[0].Rows.Count <= 0) { return 0; }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                string serial = dr["C001"].ToString();
                int date = Convert.ToInt32(dr["C004"]);
                if (mNewestIssueItem == null) { return 0; }
                string lastSerial = mNewestIssueItem.Serial;
                int lastDate = mNewestIssueItem.Date;
                if (date > lastDate)
                {
                    return SscDefines.REFRESH_MODE_RELOAD;
                }
                if (date == lastDate && serial != lastSerial)
                {
                    IssueItem item = new IssueItem();
                    item.Serial = serial;
                    item.Number = Convert.ToInt32(dr["C005"]);
                    item.Date = Convert.ToInt32(dr["C004"]);
                    item.WeekDay = Convert.ToInt32(dr["C006"]);

                    item.D1 = Convert.ToInt32(dr["C010"]);
                    item.D2 = Convert.ToInt32(dr["C020"]);
                    item.D3 = Convert.ToInt32(dr["C030"]);
                    item.D4 = Convert.ToInt32(dr["C040"]);
                    item.D5 = Convert.ToInt32(dr["C050"]);

                    item.FullValue = dr["C002"].ToString();
                    item.LargeValue = dr["C007"].ToString() == "1";
                    item.SingleValue = dr["C008"].ToString() == "1";
                    item.SumValue = Convert.ToInt32(dr["C009"]);
                    item.RepeatValue = dr["C100"].ToString() == "1";
                    item.IntervalValue = dr["C101"].ToString() == "1";
                    item.Larger20 = dr["C102"].ToString() == "1";
                    item.AllOne20 = dr["C103"].ToString() == "1";
                    item.PairsVaue = dr["C104"].ToString() == "1";
                    item.SameValue = dr["C105"].ToString() == "1";

                    mNewestIssueItem = item;
                    return SscDefines.REFRESH_MODE_LOTTERY;
                }
                return -1;
            }
            catch (Exception ex)
            {
                WriteLog("LoadLastIssue", string.Format("Fail. {0}", ex.Message));
                return 0;
            }
        }

        private void InitIssueItems()
        {
            mListIssueItems.Clear();
            foreach (var item in mListIssues)
            {
                mListIssueItems.Add(item);
            }
        }

        private void InitLottery()
        {
            var newestIssue = mListIssues.FirstOrDefault();
            if (newestIssue == null) { return; }
            mNewestIssueItem = newestIssue;
            TxtLastLottery.Text = ParseLottery();
            TxtLastNumber.Text = ParseLotteryNumber();
        }

        private void InitModule()
        {
            var view = TabControlModule.SelectedContent as IModuleView;
            if (view == null) { return; }
            view.Reload();
        }

        private void InitCalculateRange()
        {
            if (mCalculateMode == SscDefines.CALC_MODE_LAST_LOTTERY)
            {
                TxtCalculateRange.Text = string.Format("显示最近 {0} 期", mCalculateSize);
            }
            if (mCalculateMode == SscDefines.CALC_MODE_DATE)
            {
                TxtCalculateRange.Text = string.Format("显示 {0} 共 {1} 期", ParseStandardDate(mCalculateDate),
                    mListIssues.Count);
            }
        }

        private void RefreshModule()
        {
            var view = TabControlModule.SelectedContent as IModuleView;
            if (view == null) { return; }
            view.Refresh(mNewestIssueItem);
        }

        #endregion


        #region Property

        public SystemConfig SystemConfig
        {
            get { return mSystemConfig; }
        }

        public ObservableCollection<IssueItem> ListIssueItems
        {
            get { return mListIssueItems; }
        }

        public static readonly DependencyProperty AutoRefreshProperty =
            DependencyProperty.Register("AutoRefresh", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

        public bool AutoRefresh
        {
            get { return (bool)GetValue(AutoRefreshProperty); }
            set { SetValue(AutoRefreshProperty, value); }
        }

        #endregion


        #region Event Handlers

        void PanelLeft_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (PanelLeft.ActualWidth <= 0)
            {
                PanelLeftExpander.Visibility = Visibility.Visible;
            }
            else
            {
                PanelLeftExpander.Visibility = Visibility.Collapsed;
            }
        }

        void BtnLeftCollaspe_Click(object sender, RoutedEventArgs e)
        {
            ColumnLeft.Width = new GridLength(0);
            PanelLeftExpander.Visibility = Visibility.Visible;
            mLeftExpanded = false;
        }

        void BtnLeftExpand_Click(object sender, RoutedEventArgs e)
        {
            ColumnLeft.Width = new GridLength(110);
            PanelLeftExpander.Visibility = Visibility.Collapsed;
            mLeftExpanded = true;
        }

        void BtnSetting_Click(object sender, RoutedEventArgs e)
        {

        }

        void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            UCHistoryQuery uc = new UCHistoryQuery();
            PopupWindow popup = new PopupWindow();
            popup.Title = "历史查询";
            popup.Content = uc;
            var result = popup.ShowDialog();
            if (result == true)
            {
                string issueDate = uc.IssueDate;
                mCalculateMode = SscDefines.CALC_MODE_DATE;
                mCalculateDate = issueDate;
                //ShowTipMessage("正在加载，请稍候...");
                //mLoadWorker = new BackgroundWorker();
                //mLoadWorker.DoWork += (s, de) => LoadIssuesByDate(issueDate);
                //mLoadWorker.RunWorkerCompleted += (s, re) =>
                //{
                //    mLoadWorker.Dispose();
                //    ShowTipMessage("就绪");

                //    InitIssueItems();
                //    InitModule();
                //    InitCalculateRange();

                //    if (!mLeftExpanded)
                //    {
                //        ColumnLeft.Width = new GridLength(110);
                //        PanelLeftExpander.Visibility = Visibility.Collapsed;
                //        mLeftExpanded = true;
                //    }
                //};
                //mLoadWorker.RunWorkerAsync();

                LoadIssuesByDate(issueDate);

                InitIssueItems();
                InitModule();
                InitCalculateRange();

                if (!mLeftExpanded)
                {
                    ColumnLeft.Width = new GridLength(110);
                    PanelLeftExpander.Visibility = Visibility.Collapsed;
                    mLeftExpanded = true;
                }
            }
        }

        void ListBoxModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitModule();
        }

        void SliderScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                double viewScale = 1.0;
                int value;
                if (int.TryParse(SliderScale.Value.ToString(), out value))
                {
                    switch (value)
                    {
                        case 10:
                            viewScale = 0.2;
                            break;
                        case 15:
                            viewScale = 0.3;
                            break;
                        case 20:
                            viewScale = 0.4;
                            break;
                        case 25:
                            viewScale = 0.5;
                            break;
                        case 30:
                            viewScale = 0.6;
                            break;
                        case 35:
                            viewScale = 0.7;
                            break;
                        case 40:
                            viewScale = 0.8;
                            break;
                        case 45:
                            viewScale = 0.9;
                            break;
                        case 50:
                            viewScale = 1.0;
                            break;
                        case 55:
                            viewScale = 1.5;
                            break;
                        case 60:
                            viewScale = 2.0;
                            break;
                        case 65:
                            viewScale = 2.5;
                            break;
                        case 70:
                            viewScale = 3.0;
                            break;
                        case 75:
                            viewScale = 3.5;
                            break;
                        case 80:
                            viewScale = 4.0;
                            break;
                        case 85:
                            viewScale = 4.5;
                            break;
                        case 90:
                            viewScale = 5.0;
                            break;
                    }
                }
                ScaleTransform tran = new ScaleTransform();
                tran.ScaleX = viewScale;
                tran.ScaleY = viewScale;
                BorderViewer.LayoutTransform = tran;
                SliderScale.Tag = viewScale;
                BindingExpression be = SliderScale.GetBindingExpression(ToolTipProperty);
                if (be != null)
                {
                    be.UpdateTarget();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (mLoading) { return; }
            int refresh = LoadLastIssue();
            if (refresh >= 0)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (!AutoRefresh) { return; }
                    if (refresh == SscDefines.REFRESH_MODE_RELOAD)
                    {
                        Reload();
                    }
                    else
                    {
                        Refresh();
                    }
                }));
            }
        }

        void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (mLoading) { return; }
            int refresh = LoadLastIssue();
            if (refresh >= 0)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (refresh == SscDefines.REFRESH_MODE_RELOAD)
                    {
                        Reload();
                    }
                    else
                    {
                        Refresh();
                    }
                }));
            }
        }

        #endregion


        #region Others

        private string ParseLottery()
        {
            if (mNewestIssueItem == null)
            {
                return "";
            }
            int intDate = mNewestIssueItem.Date;
            DateTime date = DateTime.ParseExact(intDate.ToString(), "yyyyMMdd", null);
            int number = mNewestIssueItem.Number;
            return string.Format("{0:yyyy-MM-dd} {1:000}期", date, number);
        }

        private string ParseLotteryNumber()
        {
            if (mNewestIssueItem == null)
            {
                return "";
            }
            int d1 = mNewestIssueItem.D1;
            int d2 = mNewestIssueItem.D2;
            int d3 = mNewestIssueItem.D3;
            int d4 = mNewestIssueItem.D4;
            int d5 = mNewestIssueItem.D5;
            return string.Format("{0} {1} {2} {3} {4}", d5, d4, d3, d2, d1);
        }

        private string ParseStandardDate(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "yyyyMMdd", null);
            return dt.ToString("yyyy-MM-dd");
        }

        #endregion


        #region basic

        private void ShowException(string msg)
        {
            ThreadPool.QueueUserWorkItem(a => MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error));

        }

        private void ShowInfomation(string msg)
        {
            ThreadPool.QueueUserWorkItem(a => MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information));
        }

        private void ShowTipMessage(string msg)
        {
            TxtMsg.Text = msg;
        }

        #endregion


        #region Log

        private void WriteLog(string category, string msg)
        {
            var app = App.Current as App;
            if (app != null)
            {
                app.WriteLog(category, msg);
            }
        }

        private void WriteLog(string msg)
        {
            WriteLog("SSC", msg);
        }

        #endregion

    }
}
