//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    7fb9d498-41b5-40a7-9e7b-5191dc3c6a3b
//        CLR Version:              4.0.30319.42000
//        Name:                     UCTendency
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.UserControls
//        File Name:                UCTendency
//
//        Created by Charley at 2019/8/23 9:12:22
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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ShiShiCai.Common;
using ShiShiCai.Models;

namespace ShiShiCai.UserControls
{
    /// <summary>
    /// UCTendency.xaml 的交互逻辑
    /// </summary>
    public partial class UCTendency : IModuleView
    {

        #region Property

        public static readonly DependencyProperty PageParentProperty =
            DependencyProperty.Register("PageParent", typeof(MainWindow), typeof(UCTendency), new PropertyMetadata(default(MainWindow)));

        public MainWindow PageParent
        {
            get { return (MainWindow)GetValue(PageParentProperty); }
            set { SetValue(PageParentProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(UCTendency), new PropertyMetadata(default(double)));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public ObservableCollection<DateItem> DateItems
        {
            get { return mListDateItems; }
        }

        public ObservableCollection<TendencyPositionItem> PositionItems
        {
            get { return mListPositionItems; }
        }

        public ObservableCollection<TendencyNumberItem> NumberItems
        {
            get { return mListNumberItems; }
        }

        public ObservableCollection<int> NumberYAxisLabels
        {
            get { return mListNumberYAxisLabels; }
        }

        #endregion


        #region Members

        private readonly List<TendencyItem> mListTendencyData = new List<TendencyItem>();
        private readonly ObservableCollection<TendencyPositionItem> mListPositionItems = new ObservableCollection<TendencyPositionItem>();
        private readonly ObservableCollection<DateItem> mListDateItems = new ObservableCollection<DateItem>();
        private readonly ObservableCollection<TendencyNumberItem> mListNumberItems = new ObservableCollection<TendencyNumberItem>();
        private readonly ObservableCollection<int> mListNumberYAxisLabels = new ObservableCollection<int>();

        private bool mIsInited;

        #endregion


        public UCTendency()
        {
            InitializeComponent();

            Loaded += UCTendency_Loaded;
            ComboDate.SelectionChanged += ComboDate_SelectionChanged;
            BorderChart.SizeChanged += BorderChart_SizeChanged;
            ListBoxPositions.AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(CheckBoxNumber_Checked));
            ListBoxPositions.AddHandler(ToggleButton.UncheckedEvent, new RoutedEventHandler(CheckBoxNumber_Unchecked));
        }

        void UCTendency_Loaded(object sender, RoutedEventArgs e)
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
            LoadTendencyData();
            RefreshTendencyNumberItem(issueItem);
            RefreshTendencyPosItem(issueItem);
            InitItemWidth();
            InitNumberHeights();
            InitNumberPaths();
            InitCaculateInfos();
        }

        private void Init()
        {
            InitPositionItems();
            InitDateItems();
            LoadTendencyData();

            ComboDate.SelectedItem = mListDateItems.FirstOrDefault();
        }

        private void InitPositionItems()
        {
            mListPositionItems.Clear();
            TendencyPositionItem item = new TendencyPositionItem();
            item.Pos = 5;
            item.Name = "万位";
            item.Visible = true;
            item.Color = Brushes.Red;
            mListPositionItems.Add(item);
            item = new TendencyPositionItem();
            item.Pos = 4;
            item.Name = "千位";
            item.Color = Brushes.Green;
            mListPositionItems.Add(item);
            item = new TendencyPositionItem();
            item.Pos = 3;
            item.Name = "百位";
            item.Color = Brushes.Blue;
            mListPositionItems.Add(item);
            item = new TendencyPositionItem();
            item.Pos = 2;
            item.Name = "十位";
            item.Color = Brushes.Orange;
            mListPositionItems.Add(item);
            item = new TendencyPositionItem();
            item.Pos = 1;
            item.Name = "个位";
            item.Color = Brushes.Fuchsia;
            mListPositionItems.Add(item);
        }

        private void InitItemWidth()
        {
            int count = 60;
            double width = BorderChart.ActualWidth - 30 - 80;
            double itemWidth = width / (count * 1.0);
            ItemWidth = itemWidth;
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

        private void LoadTendencyData()
        {
            mListTendencyData.Clear();
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
                "SELECT * FROM T_103_19 WHERE C001 >= {0} AND C001 <= {1} ORDER BY C001, C004", begin, end);
            OperationReturn optReturn = MssqlOperation.GetDataSet(strConn, strSql);
            if (!optReturn.Result)
            {
                ShowException(string.Format("load tendency data fail. [{0}]{1}", optReturn.Code, optReturn.Message));
                return;
            }
            DataSet objDataSet = optReturn.Data as DataSet;
            if (objDataSet == null) { return; }
            for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
            {
                DataRow dr = objDataSet.Tables[0].Rows[i];
                TendencyItem item = new TendencyItem();
                item.Serial = dr["C001"].ToString();
                item.Date = Convert.ToInt32(dr["C002"]);
                item.Number = Convert.ToInt32(dr["C003"]);
                item.Pos = Convert.ToInt32(dr["C004"]);
                item.Repeat = dr["C005"].ToString() == "1";
                item.Osillation = dr["C006"].ToString() == "1";
                item.Increase = dr["C007"].ToString() == "1";
                item.Other = dr["C008"].ToString() == "1";
                item.Times = Convert.ToInt32(dr["C009"]);
                item.Range = Convert.ToInt32(dr["C010"]);
                mListTendencyData.Add(item);
            }
        }

        private void InitTendencyNumberItems()
        {
            mListNumberItems.Clear();
            DateItem dateItem = ComboDate.SelectedItem as DateItem;
            if (dateItem == null) { return; }
            int date = dateItem.Date;
            var data = mListTendencyData.Where(t => t.Date == date);
            var dataGroups = data.GroupBy(t => t.Number);
            foreach (var dataGroup in dataGroups)
            {
                int number = dataGroup.Key;
                TendencyNumberItem item = new TendencyNumberItem();
                item.Date = date;
                item.Number = number;
                foreach (var dataItem in dataGroup)
                {
                    string serial = dataItem.Serial;
                    item.Serial = serial;
                    int pos = dataItem.Pos;
                    if (pos == 1)
                    {
                        item.D1Range = dataItem.Range;
                    }
                    if (pos == 2)
                    {
                        item.D2Range = dataItem.Range;
                    }
                    if (pos == 3)
                    {
                        item.D3Range = dataItem.Range;
                    }
                    if (pos == 4)
                    {
                        item.D4Range = dataItem.Range;
                    }
                    if (pos == 5)
                    {
                        item.D5Range = dataItem.Range;
                    }
                }
                mListNumberItems.Add(item);
            }
        }

        private void InitNumberHeights()
        {
            double height = BorderChart.ActualHeight - 20;
            int maxValue = 0;


            #region 找到最大振幅值

            for (int i = 0; i < mListNumberItems.Count; i++)
            {
                var item = mListNumberItems[i];
                maxValue = Math.Max(item.D1Range, maxValue);
                maxValue = Math.Max(item.D2Range, maxValue);
                maxValue = Math.Max(item.D3Range, maxValue);
                maxValue = Math.Max(item.D4Range, maxValue);
                maxValue = Math.Max(item.D5Range, maxValue);
            }

            #endregion


            #region 确定振幅的Y轴标签，一般分为5段，标签值取整

            mListNumberYAxisLabels.Clear();
            int mod = maxValue % 5;
            int div = maxValue / 5;
            if (mod > 0)
            {
                div = div + 1;
            }
            maxValue = div * 5;
            for (int i = 5; i >= 1; i--)
            {
                mListNumberYAxisLabels.Add(i * div);
            }

            #endregion


            for (int i = 0; i < mListNumberItems.Count; i++)
            {
                var item = mListNumberItems[i];

                #region 高度

                item.D1Height = item.D1Range / (maxValue * 1.0) * height;
                item.D2Height = item.D2Range / (maxValue * 1.0) * height;
                item.D3Height = item.D3Range / (maxValue * 1.0) * height;
                item.D4Height = item.D4Range / (maxValue * 1.0) * height;
                item.D5Height = item.D5Range / (maxValue * 1.0) * height;

                #endregion


                #region 颜色和可见性

                for (int k = 0; k < mListPositionItems.Count; k++)
                {
                    var posItem = mListPositionItems[k];
                    int pos = posItem.Pos;
                    bool isShow = posItem.Visible;
                    if (pos == 1)
                    {
                        item.D1Color = posItem.Color;
                        item.D1Visible = isShow;
                    }
                    if (pos == 2)
                    {
                        item.D2Color = posItem.Color;
                        item.D2Visible = isShow;
                    }
                    if (pos == 3)
                    {
                        item.D3Color = posItem.Color;
                        item.D3Visible = isShow;
                    }
                    if (pos == 4)
                    {
                        item.D4Color = posItem.Color;
                        item.D4Visible = isShow;
                    }
                    if (pos == 5)
                    {
                        item.D5Color = posItem.Color;
                        item.D5Visible = isShow;
                    }
                }

                #endregion

            }
        }

        private void InitNumberPaths()
        {
            double itemWidth = ItemWidth;
            double height = BorderChart.ActualHeight - 20;
            for (int i = 0; i < mListPositionItems.Count; i++)
            {
                var posItem = mListPositionItems[i];
                var pos = posItem.Pos;
                PathSegmentCollection segments = new PathSegmentCollection();
                var firstItem = posItem.NumberItems.FirstOrDefault();
                if (firstItem == null) { continue; }
                double itemHeight = 0;
                if (pos == 1)
                {
                    itemHeight = firstItem.D1Height;
                }
                if (pos == 2)
                {
                    itemHeight = firstItem.D2Height;
                }
                if (pos == 3)
                {
                    itemHeight = firstItem.D3Height;
                }
                if (pos == 4)
                {
                    itemHeight = firstItem.D4Height;
                }
                if (pos == 5)
                {
                    itemHeight = firstItem.D5Height;
                }
                double firstX = itemWidth / 2.0;
                double firstY = height - itemHeight + 4;
                Point firtPoint = new Point(firstX, firstY);
                for (int j = 0; j < posItem.NumberItems.Count; j++)
                {
                    var item = posItem.NumberItems[j];
                    itemHeight = 0;
                    if (pos == 1)
                    {
                        itemHeight = item.D1Height;
                    }
                    if (pos == 2)
                    {
                        itemHeight = item.D2Height;
                    }
                    if (pos == 3)
                    {
                        itemHeight = item.D3Height;
                    }
                    if (pos == 4)
                    {
                        itemHeight = item.D4Height;
                    }
                    if (pos == 5)
                    {
                        itemHeight = item.D5Height;
                    }
                    double x = itemWidth * j + itemWidth / 2.0;
                    double y = height - itemHeight + 4;
                    Point point = new Point(x, y);
                    segments.Add(new LineSegment { Point = point });
                }
                PathGeometry path = new PathGeometry { Figures = new PathFigureCollection { new PathFigure { StartPoint = firtPoint, Segments = segments } } };
                posItem.Path = path;
            }
        }

        private void InitTendencyPosItems()
        {
            DateItem dateItem = ComboDate.SelectedItem as DateItem;
            if (dateItem == null) { return; }
            int date = dateItem.Date;
            for (int i = 0; i < mListPositionItems.Count; i++)
            {
                var posItem = mListPositionItems[i];
                int pos = posItem.Pos;
                posItem.NumberItems.Clear();
                posItem.Items.Clear();
                for (int j = 0; j < mListNumberItems.Count; j++)
                {
                    posItem.NumberItems.Add(mListNumberItems[j]);
                }
                var data = mListTendencyData.Where(t => t.Date == date && t.Pos == pos).OrderBy(t => t.Number).ToList();
                for (int j = 0; j < data.Count; j++)
                {
                    var dataItem = data[j];
                    TendencyDetailItem item = new TendencyDetailItem();
                    item.Serial = dataItem.Serial;
                    item.Number = dataItem.Number;
                    item.Date = dataItem.Date;
                    item.Pos = pos;
                    if (dataItem.Repeat)
                    {
                        item.Category = 1;
                    }
                    if (dataItem.Osillation)
                    {
                        item.Category = 2;
                    }
                    if (dataItem.Increase)
                    {
                        item.Category = 3;
                    }
                    if (dataItem.Other)
                    {
                        item.Category = 4;
                    }
                    item.Times = dataItem.Times;
                    item.Range = dataItem.Range;
                    posItem.Items.Add(item);
                }
            }
        }

        private void InitCaculateInfos()
        {
            for (int i = 0; i < mListPositionItems.Count; i++)
            {
                var posItem = mListPositionItems[i];
                int sum1 = 0;
                int sum2 = 0;
                int sum3 = 0;
                int sum4 = 0;
                int max1 = 0;
                int max2 = 0;
                int max3 = 0;
                int max4 = 0;
                for (int j = 0; j < posItem.Items.Count; j++)
                {
                    var item = posItem.Items[j];
                    if (item.Category == 1)
                    {
                        sum1++;
                        max1 = Math.Max(max1, item.Times);
                    }
                    if (item.Category == 2)
                    {
                        sum2++;
                        max2 = Math.Max(max2, item.Times);
                    }
                    if (item.Category == 3)
                    {
                        sum3++;
                        max3 = Math.Max(max3, item.Times);
                    }
                    if (item.Category == 4)
                    {
                        sum4++;
                        max4 = Math.Max(max4, item.Times);
                    }
                }
                posItem.Sum1 = sum1;
                posItem.Sum2 = sum2;
                posItem.Sum3 = sum3;
                posItem.Sum4 = sum4;
                posItem.Max1 = max1;
                posItem.Max2 = max2;
                posItem.Max3 = max3;
                posItem.Max4 = max4;
            }
        }

        private void RefreshTendencyNumberItem(IssueItem issueItem)
        {
            DateItem dateItem = ComboDate.SelectedItem as DateItem;
            if (dateItem == null) { return; }
            int date = dateItem.Date;
            if (date != issueItem.Date) { return; }
            string serial = issueItem.Serial;
            var data = mListTendencyData.Where(t => t.Serial == serial).ToList();
            if (data.Count <= 0) { return; }
            TendencyNumberItem item = new TendencyNumberItem();
            item.Serial = serial;
            item.Date = date;
            for (int i = 0; i < data.Count; i++)
            {
                var dataItem = data[i];
                item.Number = dataItem.Number;
                int pos = dataItem.Pos;
                if (pos == 1)
                {
                    item.D1Range = dataItem.Range;
                }
                if (pos == 2)
                {
                    item.D2Range = dataItem.Range;
                }
                if (pos == 3)
                {
                    item.D3Range = dataItem.Range;
                }
                if (pos == 4)
                {
                    item.D4Range = dataItem.Range;
                }
                if (pos == 5)
                {
                    item.D5Range = dataItem.Range;
                }
            }
            mListNumberItems.Add(item);
        }

        private void RefreshTendencyPosItem(IssueItem issueItem)
        {
            DateItem dateItem = ComboDate.SelectedItem as DateItem;
            if (dateItem == null) { return; }
            int date = dateItem.Date;
            if (date != issueItem.Date) { return; }
            for (int i = 0; i < mListPositionItems.Count; i++)
            {
                var posItem = mListPositionItems[i];
                int pos = posItem.Pos;
                string serial = issueItem.Serial;
                var numberItem = mListNumberItems.FirstOrDefault(n => n.Serial == serial);
                if (numberItem != null)
                {
                    posItem.NumberItems.Add(numberItem);
                }
                var dataItem = mListTendencyData.FirstOrDefault(t => t.Serial == serial && t.Pos == pos);
                if (dataItem == null) { continue; }
                TendencyDetailItem item = new TendencyDetailItem();
                item.Serial = serial;
                item.Number = dataItem.Number;
                item.Date = date;
                item.Pos = pos;
                if (dataItem.Repeat)
                {
                    item.Category = 1;
                }
                if (dataItem.Osillation)
                {
                    item.Category = 2;
                }
                if (dataItem.Increase)
                {
                    item.Category = 3;
                }
                if (dataItem.Other)
                {
                    item.Category = 4;
                }
                item.Times = dataItem.Times;
                item.Range = dataItem.Range;
                posItem.Items.Add(item);
            }
        }

        #endregion


        #region 事件处理

        void ComboDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitTendencyNumberItems();
            InitTendencyPosItems();
            InitItemWidth();
            InitNumberHeights();
            InitNumberPaths();
            InitCaculateInfos();
        }

        void BorderChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitItemWidth();
            InitNumberHeights();
            InitNumberPaths();
        }

        void CheckBoxNumber_Checked(object sender, RoutedEventArgs e)
        {
            InitNumberHeights();
            InitNumberPaths();
        }

        void CheckBoxNumber_Unchecked(object sender, RoutedEventArgs e)
        {
            InitNumberHeights();
            InitNumberPaths();
        }

        #endregion


        #region Others



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
