using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ShiShiCai.Common;

namespace ShiShiCai
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private bool mIsInited;

        private readonly List<IssueItem> mListIssues = new List<IssueItem>();

        private readonly ObservableCollection<ModuleItem> mListModuleItems = new ObservableCollection<ModuleItem>();
        private readonly ObservableCollection<IssueItem> mListIssueItems = new ObservableCollection<IssueItem>();

        private SystemConfig mSystemConfig;
        private BackgroundWorker mLoadWorker;
        private IssueItem mCurrentIssueItem;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnLeftExpand.Click += BtnLeftExpand_Click;
            BtnLeftCollaspe.Click += BtnLeftCollaspe_Click;

            PanelLeft.SizeChanged += PanelLeft_SizeChanged;

            DataContext = this;
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
            ListBoxModules.ItemsSource = mListModuleItems;
            ListBoxIssues.ItemsSource = mListIssueItems;

            InitModuleItems();
            LoadConfig();

            mListIssueItems.Clear();
            mLoadWorker = new BackgroundWorker();
            mLoadWorker.DoWork += (s, de) => LoadIssues();
            mLoadWorker.RunWorkerCompleted += (s, re) =>
            {
                mLoadWorker.Dispose();

                InitIssueItems();
                InitLottery();
                InitModule();
            };
            mLoadWorker.RunWorkerAsync();
        }

        private void InitModuleItems()
        {
            mListModuleItems.Clear();
            ModuleItem item = new ModuleItem();
            item.Number = SscDefines.MODULE_BASIC;
            item.Name = SscDefines.MODULE_NAME_BASIC;
            item.Title = SscDefines.MODULE_NAME_BASIC;
            item.Icon = "Themes/Default/Images/00001.png";
            mListModuleItems.Add(item);
            item = new ModuleItem();
            item.Number = SscDefines.MODULE_LOST;
            item.Name = SscDefines.MODULE_NAME_LOST;
            item.Title = SscDefines.MODULE_NAME_LOST;
            item.Icon = "Themes/Default/Images/00002.png";
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
                string strSql = string.Format("SELECT TOP 300 * FROM T_101_19 ORDER BY C001 DESC");
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
                    item.DoubleValue = dr["C008"].ToString() == "2";
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
            var lotteryItem = mListIssueItems.FirstOrDefault();
            if (lotteryItem == null) { return; }
            mCurrentIssueItem = lotteryItem;
            TxtLastLottery.Text = ParseLottery();
            TxtLastNumber.Text = ParseLotteryNumber();
            ListBoxIssues.SelectedItem = mCurrentIssueItem;
        }

        private void InitModule()
        {
            var view = TabControlModule.SelectedContent as IModuleView;
            if (view == null) { return; }
            view.Reload();
        }

        #endregion


        #region Property

        public ObservableCollection<IssueItem> ListIssueItems
        {
            get { return mListIssueItems; }
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
        }

        void BtnLeftExpand_Click(object sender, RoutedEventArgs e)
        {
            ColumnLeft.Width = new GridLength(110);
            PanelLeftExpander.Visibility = Visibility.Collapsed;
        }

        #endregion


        #region Others

        private string ParseLottery()
        {
            if (mCurrentIssueItem == null)
            {
                return "";
            }
            int intDate = mCurrentIssueItem.Date;
            DateTime date = DateTime.ParseExact(intDate.ToString(), "yyyyMMdd", null);
            int number = mCurrentIssueItem.Number;
            return string.Format("{0:yyyy-MM-dd} {1:000}期", date, number);
        }

        private string ParseLotteryNumber()
        {
            if (mCurrentIssueItem == null)
            {
                return "";
            }
            int d1 = mCurrentIssueItem.D1;
            int d2 = mCurrentIssueItem.D2;
            int d3 = mCurrentIssueItem.D3;
            int d4 = mCurrentIssueItem.D4;
            int d5 = mCurrentIssueItem.D5;
            return string.Format("{0} {1} {2} {3} {4}", d5, d4, d3, d2, d1);
        }

        #endregion


        #region basic

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInfomation(string msg)
        {
            MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
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
