//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    b1382833-a0b4-483c-a446-14bbae30fadd
//        CLR Version:              4.0.30319.42000
//        Name:                     UCLargeSmall
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.UserControls
//        File Name:                UCLargeSmall
//
//        Created by Charley at 2019/8/16 11:35:41
//        http://www.netinfo.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ShiShiCai.Common;
using ShiShiCai.Models;

namespace ShiShiCai.UserControls
{
    /// <summary>
    /// UCLargeSmall.xaml 的交互逻辑
    /// </summary>
    public partial class UCLargeSmall : IModuleView
    {

        #region Property

        public static readonly DependencyProperty PageParentProperty =
            DependencyProperty.Register("PageParent", typeof(MainWindow), typeof(UCLargeSmall), new PropertyMetadata(default(MainWindow)));

        public MainWindow PageParent
        {
            get { return (MainWindow)GetValue(PageParentProperty); }
            set { SetValue(PageParentProperty, value); }
        }

        public static readonly DependencyProperty SumValuePathProperty =
            DependencyProperty.Register("SumValuePath", typeof(PathGeometry), typeof(UCLargeSmall), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry SumValuePath
        {
            get { return (PathGeometry)GetValue(SumValuePathProperty); }
            set { SetValue(SumValuePathProperty, value); }
        }

        public ObservableCollection<DateItem> DateItems
        {
            get { return mListDateItems; }
        }

        public ObservableCollection<LargeSmallItem> SumValueItems
        {
            get { return mListSumValueItems; }
        }

        public ObservableCollection<PositionLargeSmallItem> PositionItems
        {
            get { return mListPositionItems; }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(UCLargeSmall), new PropertyMetadata(default(double)));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(UCLargeSmall), new PropertyMetadata(default(double)));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty LargeNumProperty =
            DependencyProperty.Register("LargeNum", typeof(int), typeof(UCLargeSmall), new PropertyMetadata(default(int)));

        public int LargeNum
        {
            get { return (int)GetValue(LargeNumProperty); }
            set { SetValue(LargeNumProperty, value); }
        }

        public static readonly DependencyProperty SmallNumProperty =
            DependencyProperty.Register("SmallNum", typeof(int), typeof(UCLargeSmall), new PropertyMetadata(default(int)));

        public int SmallNum
        {
            get { return (int)GetValue(SmallNumProperty); }
            set { SetValue(SmallNumProperty, value); }
        }

        public static readonly DependencyProperty SingleNumProperty =
            DependencyProperty.Register("SingleNum", typeof(int), typeof(UCLargeSmall), new PropertyMetadata(default(int)));

        public int SingleNum
        {
            get { return (int)GetValue(SingleNumProperty); }
            set { SetValue(SingleNumProperty, value); }
        }

        public static readonly DependencyProperty DoubleNumProperty =
            DependencyProperty.Register("DoubleNum", typeof(int), typeof(UCLargeSmall), new PropertyMetadata(default(int)));

        public int DoubleNum
        {
            get { return (int)GetValue(DoubleNumProperty); }
            set { SetValue(DoubleNumProperty, value); }
        }

        public static readonly DependencyProperty LargeMaxNumProperty =
            DependencyProperty.Register("LargeMaxNum", typeof(int), typeof(UCLargeSmall), new PropertyMetadata(default(int)));

        public int LargeMaxNum
        {
            get { return (int)GetValue(LargeMaxNumProperty); }
            set { SetValue(LargeMaxNumProperty, value); }
        }

        public static readonly DependencyProperty SmallMaxNumProperty =
            DependencyProperty.Register("SmallMaxNum", typeof(int), typeof(UCLargeSmall), new PropertyMetadata(default(int)));

        public int SmallMaxNum
        {
            get { return (int)GetValue(SmallMaxNumProperty); }
            set { SetValue(SmallMaxNumProperty, value); }
        }

        public static readonly DependencyProperty SingleMaxNumProperty =
            DependencyProperty.Register("SingleMaxNum", typeof(int), typeof(UCLargeSmall), new PropertyMetadata(default(int)));

        public int SingleMaxNum
        {
            get { return (int)GetValue(SingleMaxNumProperty); }
            set { SetValue(SingleMaxNumProperty, value); }
        }

        public static readonly DependencyProperty DoubleMaxNumProperty =
            DependencyProperty.Register("DoubleMaxNum", typeof(int), typeof(UCLargeSmall), new PropertyMetadata(default(int)));

        public int DoubleMaxNum
        {
            get { return (int)GetValue(DoubleMaxNumProperty); }
            set { SetValue(DoubleMaxNumProperty, value); }
        }

        #endregion


        #region Members

        private readonly List<LargeSmallItem> mListLargeSmallData = new List<LargeSmallItem>();
        private readonly ObservableCollection<DateItem> mListDateItems = new ObservableCollection<DateItem>();
        private readonly ObservableCollection<LargeSmallItem> mListSumValueItems = new ObservableCollection<LargeSmallItem>();
        private readonly ObservableCollection<PositionLargeSmallItem> mListPositionItems = new ObservableCollection<PositionLargeSmallItem>();

        private bool mIsInited;

        #endregion


        public UCLargeSmall()
        {
            InitializeComponent();

            Loaded += UCLargeSmall_Loaded;
            ListBoxSumView.SizeChanged += ListBoxSumView_SizeChanged;
            ComboDate.SelectionChanged += ComboDate_SelectionChanged;
        }

        void UCLargeSmall_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }


        #region Init and Load

        public void Reload()
        {
            Init();
        }

        public void Refresh(IssueItem issueItem)
        {
            LoadLargeSmallData();
            int date = issueItem.Date;
            var dateItem = ComboDate.SelectedItem as DateItem;
            if (dateItem == null) { return; }
            if (date != dateItem.Date) { return; }
            RefreshSumValueItem(issueItem);
            RefreshPositionItem(issueItem);
            InitDistributeView();
            InitSumValueTitle();
            InitSumViewSize();
            InitPositionItemValues();
        }

        private void Init()
        {
            LoadLargeSmallData();
            InitDateItems();
            ComboDate.SelectedItem = mListDateItems.FirstOrDefault();
        }

        private void LoadLargeSmallData()
        {
            mListLargeSmallData.Clear();
            if (PageParent == null) { return; }
            var issueItems = PageParent.ListIssueItems;
            if (issueItems == null) { return; }
            var systemConfig = PageParent.SystemConfig;
            if (systemConfig == null) { return; }
            var db = systemConfig.Database;
            if (db == null) { return; }
            string strConn = db.GetConnectionString();
            var first = issueItems.FirstOrDefault();
            if (first == null) { return; }
            long begin = long.Parse(first.Serial);
            long end = long.Parse(first.Serial);
            for (int i = 0; i < issueItems.Count; i++)
            {
                var item = issueItems[i];
                long serial = long.Parse(item.Serial);
                begin = Math.Min(serial, begin);
                end = Math.Max(serial, end);
            }
            string strSql = string.Format(
                "SELECT * FROM t_111_19 WHERE C001 >= {0} AND C001 <= {1} ORDER BY C001", begin, end);
            OperationReturn optReturn = MssqlOperation.GetDataSet(strConn, strSql);
            if (!optReturn.Result)
            {
                ShowException(string.Format("load large small data fail. [{0}]{1}", optReturn.Code, optReturn.Message));
                return;
            }
            DataSet objDataSet = optReturn.Data as DataSet;
            if (objDataSet == null) { return; }
            for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
            {
                DataRow dr = objDataSet.Tables[0].Rows[i];
                LargeSmallItem item = new LargeSmallItem();
                item.Serial = dr["C001"].ToString();
                item.Date = Convert.ToInt32(dr["C002"]);
                item.Number = Convert.ToInt32(dr["C003"]);
                item.Pos = Convert.ToInt32(dr["C004"]);
                item.Large = Convert.ToInt32(dr["C005"]) == 1;
                item.Small = Convert.ToInt32(dr["C006"]) == 1;
                item.Single = Convert.ToInt32(dr["C007"]) == 1;
                item.Double = Convert.ToInt32(dr["C008"]) == 1;
                item.LargeSmallNum = Convert.ToInt32(dr["C009"]);
                item.SingleDoubleNum = Convert.ToInt32(dr["C010"]);
                item.LargeNum = Convert.ToInt32(dr["C011"]);
                item.SmallNum = Convert.ToInt32(dr["C012"]);
                item.SingleNum = Convert.ToInt32(dr["C013"]);
                item.DoubleNum = Convert.ToInt32(dr["C014"]);
                mListLargeSmallData.Add(item);
            }
        }

        private void InitDateItems()
        {
            mListDateItems.Clear();
            var pageParent = PageParent;
            if (pageParent == null) { return; }
            var issueItems = pageParent.ListIssueItems;
            if (issueItems == null) { return; }
            var dateGroups = issueItems.GroupBy(g => g.Date);
            foreach (var dateGroup in dateGroups)
            {
                int date = dateGroup.Key;
                DateItem item = new DateItem();
                item.Date = date;
                mListDateItems.Add(item);
            }
        }

        private void InitSumValueItems()
        {
            mListSumValueItems.Clear();
            int pos = 6;
            var dateItem = ComboDate.SelectedItem as DateItem;
            if (dateItem == null) { return; }
            int date = dateItem.Date;
            if (PageParent == null) { return; }
            var issueItems = PageParent.ListIssueItems;
            if (issueItems == null) { return; }
            var data = mListLargeSmallData.Where(l => l.Date == date && l.Pos == pos).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                var dataItem = data[i];
                string serial = dataItem.Serial;
                var issueItem = issueItems.FirstOrDefault(s => s.Serial == serial);
                if (issueItem == null) { continue; }
                LargeSmallItem item = new LargeSmallItem();
                item.Serial = serial;
                item.Date = date;
                item.Number = issueItem.Number;
                item.Pos = dataItem.Pos;
                item.SumValue = issueItem.SumValue;
                item.Large = dataItem.Large;
                item.Small = dataItem.Small;
                item.Single = dataItem.Single;
                item.Double = dataItem.Double;
                item.LargeSmallNum = dataItem.LargeSmallNum;
                item.SingleDoubleNum = dataItem.SingleDoubleNum;
                item.LargeNum = dataItem.LargeNum;
                item.SmallNum = dataItem.SmallNum;
                item.SingleNum = dataItem.SingleNum;
                item.DoubleNum = dataItem.DoubleNum;
                mListSumValueItems.Add(item);
            }
        }

        private void InitSumValueTitle()
        {
            var first = mListSumValueItems.FirstOrDefault();
            var last = mListSumValueItems.LastOrDefault();
            if (first != null && last != null)
            {
                string strBegin = first.Serial;
                string strEnd = last.Serial;
                TxtSumValueTitle.Text = string.Format("和值趋势 ({0} ~ {1})", strBegin, strEnd);
            }
        }

        private void InitSumViewSize()
        {
            int count = 60;
            double width = ListBoxSumView.ActualWidth;
            double height = ListBoxSumView.ActualHeight;
            double itemWidth = width / (count * 1.0);
            ItemWidth = itemWidth;
            for (int i = 0; i < mListSumValueItems.Count; i++)
            {
                var item = mListSumValueItems[i];
                item.ItemWidth = itemWidth;
                item.ItemHeight = item.SumValue * (height / (45 * 1.0));
            }
            InitSumViewPath();
        }

        private void InitSumViewPath()
        {
            int count = 60;
            double width = ListBoxSumView.ActualWidth;
            double height = ListBoxSumView.ActualHeight;
            double ballSize = 4;
            var itemWidth = width / (count * 1.0);
            var firstItem = mListSumValueItems.FirstOrDefault();
            if (firstItem == null) { return; }
            PathSegmentCollection segments = new PathSegmentCollection();
            double firstX = itemWidth / 2.0;
            double firstY = height - (firstItem.SumValue / 45.0) * height;
            Point first = new Point(firstX, firstY);
            for (int i = 0; i < mListSumValueItems.Count; i++)
            {
                var item = mListSumValueItems[i];
                double x = itemWidth * i + itemWidth / 2.0;
                double y = height - (item.SumValue / 45.0) * height + ballSize;
                Point point = new Point(x, y);
                segments.Add(new LineSegment { Point = point });
            }
            PathGeometry path = new PathGeometry { Figures = new PathFigureCollection { new PathFigure { StartPoint = first, Segments = segments } } };
            SumValuePath = path;
        }

        private void InitDistributeView()
        {
            int largeNum = 0;
            int smallNum = 0;
            int singleNum = 0;
            int doubleNum = 0;
            int largeMaxNum = 0;
            int smallMaxNum = 0;
            int singleMaxNum = 0;
            int doubleMaxNum = 0;
            for (int i = 0; i < mListSumValueItems.Count; i++)
            {
                var item = mListSumValueItems[i];
                largeNum += item.LargeNum;
                smallNum += item.SmallNum;
                singleNum += item.SingleNum;
                doubleNum += item.DoubleNum;
                largeMaxNum = Math.Max(item.LargeNum, largeMaxNum);
                smallMaxNum = Math.Max(item.SmallNum, smallMaxNum);
                singleMaxNum = Math.Max(item.SingleNum, singleMaxNum);
                doubleMaxNum = Math.Max(item.DoubleNum, doubleMaxNum);
            }
            LargeNum = largeNum;
            SmallNum = smallNum;
            SingleNum = singleNum;
            DoubleNum = doubleNum;
            LargeMaxNum = largeMaxNum;
            SmallMaxNum = smallMaxNum;
            SingleMaxNum = singleMaxNum;
            DoubleMaxNum = doubleMaxNum;
        }

        private void InitPositionItems()
        {
            mListPositionItems.Clear();
            PositionLargeSmallItem item = new PositionLargeSmallItem();
            item.Pos = 6;
            item.Name = "全位";
            InitPositionItems(item);
            mListPositionItems.Add(item);
            item = new PositionLargeSmallItem();
            item.Pos = 5;
            item.Name = "万位";
            InitPositionItems(item);
            mListPositionItems.Add(item);
            item = new PositionLargeSmallItem();
            item.Pos = 4;
            item.Name = "千位";
            InitPositionItems(item);
            mListPositionItems.Add(item);
            item = new PositionLargeSmallItem();
            item.Pos = 3;
            item.Name = "百位";
            InitPositionItems(item);
            mListPositionItems.Add(item);
            item = new PositionLargeSmallItem();
            item.Pos = 2;
            item.Name = "十位";
            InitPositionItems(item);
            mListPositionItems.Add(item);
            item = new PositionLargeSmallItem();
            item.Pos = 1;
            item.Name = "个位";
            InitPositionItems(item);
            mListPositionItems.Add(item);
        }

        private void InitPositionItems(PositionLargeSmallItem posItem)
        {
            int pos = posItem.Pos;
            var dateItem = ComboDate.SelectedItem as DateItem;
            if (dateItem == null) { return; }
            int date = dateItem.Date;
            if (PageParent == null) { return; }
            var issueItems = PageParent.ListIssueItems;
            if (issueItems == null) { return; }
            var data = mListLargeSmallData.Where(l => l.Date == date && l.Pos == pos).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                var dataItem = data[i];
                string serial = dataItem.Serial;
                var issueItem = issueItems.FirstOrDefault(l => l.Serial == serial);
                if (issueItem == null) { continue; }
                LargeSmallItem item = new LargeSmallItem();
                item.Serial = serial;
                item.Date = date;
                item.Number = issueItem.Number;
                item.Pos = pos;
                item.Large = dataItem.Large;
                item.Small = dataItem.Small;
                item.Single = dataItem.Single;
                item.Double = dataItem.Double;
                item.LargeSmallNum = dataItem.LargeSmallNum;
                item.SingleDoubleNum = dataItem.SingleDoubleNum;
                item.LargeNum = dataItem.LargeNum;
                item.SmallNum = dataItem.SmallNum;
                item.SingleNum = dataItem.SingleNum;
                item.DoubleNum = dataItem.DoubleNum;
                posItem.Items.Add(item);
            }
        }

        private void InitPositionItemValues()
        {
            for (int i = 0; i < mListPositionItems.Count; i++)
            {
                var posItem = mListPositionItems[i];
                int largeNum = 0;
                int smallNum = 0;
                int singleNum = 0;
                int doubleNum = 0;
                int largeMaxNum = 0;
                int smallMaxNum = 0;
                int singleMaxNum = 0;
                int doubleMaxNum = 0;
                for (int j = 0; j < posItem.Items.Count; j++)
                {
                    var item = posItem.Items[i];
                    if (item.Large)
                    {
                        largeNum++;
                        largeMaxNum = Math.Max(item.LargeSmallNum, largeMaxNum);
                    }
                    else
                    {
                        smallNum++;
                        smallMaxNum = Math.Max(item.LargeSmallNum, smallMaxNum);
                    }
                    if (item.Single)
                    {
                        singleNum++;
                        singleMaxNum = Math.Max(item.SingleDoubleNum, singleMaxNum);
                    }
                    else
                    {
                        doubleNum++;
                        doubleMaxNum = Math.Max(item.SingleDoubleNum, doubleMaxNum);
                    }
                }
                posItem.LargeNum = largeNum;
                posItem.SmallNum = smallNum;
                posItem.SingleNum = singleNum;
                posItem.DoubleNum = doubleNum;
                posItem.LargeMaxNum = largeMaxNum;
                posItem.SmallMaxNum = smallMaxNum;
                posItem.SingleMaxNum = singleMaxNum;
                posItem.DoubleMaxNum = doubleMaxNum;
            }
        }

        private void RefreshSumValueItem(IssueItem issueItem)
        {
            int pos = 6;
            string serial = issueItem.Serial;
            var dataItem = mListLargeSmallData.FirstOrDefault(l => l.Serial == serial && l.Pos == pos);
            if (dataItem == null) { return; }
            LargeSmallItem item = new LargeSmallItem();
            item.Serial = serial;
            item.Date = issueItem.Date;
            item.Number = issueItem.Number;
            item.Pos = pos;
            item.SumValue = issueItem.SumValue;
            item.Large = dataItem.Large;
            item.Small = dataItem.Small;
            item.Single = dataItem.Single;
            item.Double = dataItem.Double;
            item.LargeSmallNum = dataItem.LargeSmallNum;
            item.SingleDoubleNum = dataItem.SingleDoubleNum;
            item.LargeNum = dataItem.LargeNum;
            item.SmallNum = dataItem.SmallNum;
            item.SingleNum = dataItem.SingleNum;
            item.DoubleNum = dataItem.DoubleNum;
            mListSumValueItems.Add(item);
        }

        private void RefreshPositionItem(IssueItem issueItem)
        {
            for (int i = 0; i < mListPositionItems.Count; i++)
            {
                var posItem = mListPositionItems[i];
                int pos = posItem.Pos;
                string serial = issueItem.Serial;
                var dataItem = mListLargeSmallData.FirstOrDefault(l => l.Serial == serial && l.Pos == pos);
                if (dataItem == null) { continue;}
                LargeSmallItem item = new LargeSmallItem();
                item.Serial = serial;
                item.Date = issueItem.Date;
                item.Number = issueItem.Number;
                item.Pos = pos;
                item.Large = dataItem.Large;
                item.Small = dataItem.Small;
                item.Single = dataItem.Single;
                item.Double = dataItem.Double;
                item.LargeSmallNum = dataItem.LargeSmallNum;
                item.SingleDoubleNum = dataItem.SingleDoubleNum;
                item.LargeNum = dataItem.LargeNum;
                item.SmallNum = dataItem.SmallNum;
                item.SingleNum = dataItem.SingleNum;
                item.DoubleNum = dataItem.DoubleNum;
                posItem.Items.Add(item);
            }
        }

        #endregion


        #region 事件处理

        void ComboDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitSumValueItems();
            InitPositionItems();
            InitSumValueTitle();
            InitDistributeView();
            InitSumViewSize();
            InitPositionItemValues();
        }

        void ListBoxSumView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitSumViewSize();
        }

        #endregion


        #region Basic

        private void ShowException(string msg)
        {
            ThreadPool.QueueUserWorkItem(a => MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error));

        }

        private void ShowInfomation(string msg)
        {
            ThreadPool.QueueUserWorkItem(a => MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information));
        }

        #endregion


    }
}
