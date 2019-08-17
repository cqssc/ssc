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

        public ObservableCollection<SumValueItem> SumValueItems
        {
            get { return mListSumValueItems; }
        }

        #endregion


        private readonly List<LargeSmallItem> mListLargeSmallData = new List<LargeSmallItem>();
        private readonly ObservableCollection<SectionItem> mListSectionItems = new ObservableCollection<SectionItem>();
        private readonly ObservableCollection<SectionDataItem> mListSectionDataItems = new ObservableCollection<SectionDataItem>();
        private readonly ObservableCollection<SumValueItem> mListSumValueItems = new ObservableCollection<SumValueItem>();
        private readonly ObservableCollection<SumValueDateItem> mListSumValueDateItems = new ObservableCollection<SumValueDateItem>();
        private readonly ObservableCollection<LargeSmallItem> mListDistributeItems = new ObservableCollection<LargeSmallItem>();

        private bool mIsInited;

        public UCLargeSmall()
        {
            InitializeComponent();

            Loaded += UCLargeSmall_Loaded;
            ListBoxSumView.SizeChanged += ListBoxSumView_SizeChanged;
            ListBoxSection.SelectionChanged += ListBoxSection_SelectionChanged;
            ComboDate.SelectionChanged += ComboDate_SelectionChanged;

            ListBoxSection.ItemsSource = mListSectionItems;
            ComboDate.ItemsSource = mListSumValueDateItems;
            ListBoxSectionData.ItemsSource = mListSectionDataItems;
            ListBoxDistribute.ItemsSource = mListDistributeItems;
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

        private void Init()
        {
            LoadLargeSmallData();
            InitSectionItems();
            InitSumValueDateItems();
            ComboDate.SelectedItem = mListSumValueDateItems.FirstOrDefault();
        }

        private void InitSectionItems()
        {
            mListSectionItems.Clear();
            SectionItem item = new SectionItem();
            item.Number = SscDefines.SECTION_10;
            item.Name = SscDefines.SECTION_NAME_10;
            item.IsChecked = true;
            mListSectionItems.Add(item);
            item = new SectionItem();
            item.Number = SscDefines.SECTION_15;
            item.Name = SscDefines.SECTION_NAME_15;
            mListSectionItems.Add(item);
            item = new SectionItem();
            item.Number = SscDefines.SECTION_20;
            item.Name = SscDefines.SECTION_NAME_20;
            mListSectionItems.Add(item);
            item = new SectionItem();
            item.Number = SscDefines.SECTION_30;
            item.Name = SscDefines.SECTION_NAME_30;
            mListSectionItems.Add(item);
            item = new SectionItem();
            item.Number = SscDefines.SECTION_DAY;
            item.Name = SscDefines.SECTION_NAME_DAY;
            mListSectionItems.Add(item);
        }

        private void LoadLargeSmallData()
        {
            mListLargeSmallData.Clear();
            if (PageParent == null) { return; }
            var issueItems = PageParent.ListIssueItems;
            if (issueItems == null) { return;}
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
                "SELECT * FROM t_111_19 WHERE C001 >= {0} AND C001 <= {1} ORDER BY C001 DESC", begin, end);
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
                item.Large = Convert.ToInt32(dr["C005"]);
                item.Small = Convert.ToInt32(dr["C006"]);
                item.Single = Convert.ToInt32(dr["C007"]);
                item.Double = Convert.ToInt32(dr["C008"]);
                item.LargeSmallNum = Convert.ToInt32(dr["C009"]);
                item.SingleDoubleNum = Convert.ToInt32(dr["C010"]);
                item.LargeNum = Convert.ToInt32(dr["C011"]);
                item.SmallNum = Convert.ToInt32(dr["C012"]);
                item.SingleNum = Convert.ToInt32(dr["C013"]);
                item.DoubleNum = Convert.ToInt32(dr["C014"]);
                mListLargeSmallData.Add(item);
            }
        }

        private void InitSumValueDateItems()
        {
            mListSumValueDateItems.Clear();
            var pageParent = PageParent;
            if (pageParent == null) { return; }
            var issueItems = pageParent.ListIssueItems;
            if (issueItems == null) { return; }
            var dateGroups = issueItems.GroupBy(g => g.Date);
            foreach (var dateGroup in dateGroups)
            {
                int date = dateGroup.Key;
                SumValueDateItem dateItem = new SumValueDateItem();
                dateItem.Date = date;
                for (int i = dateGroup.Count() - 1; i >= 0; i--)
                {
                    var issueItem = dateGroup.ToList()[i];
                    SumValueItem item = new SumValueItem();
                    item.Serial = issueItem.Serial;
                    item.Number = issueItem.Number;
                    item.Date = issueItem.Date;
                    item.SumValue = issueItem.SumValue;
                    item.LargeValue = issueItem.LargeValue;
                    item.DoubleValue = issueItem.DoubleValue;
                    dateItem.Items.Add(item);
                }
                mListSumValueDateItems.Add(dateItem);
            }
        }

        private void InitSumValueItems()
        {
            mListSumValueItems.Clear();
            var dateItem = ComboDate.SelectedItem as SumValueDateItem;
            if (dateItem != null)
            {
                for (int i = 0; i < dateItem.Items.Count; i++)
                {
                    mListSumValueItems.Add(dateItem.Items[i]);
                }
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

        private void InitSectionDataItems()
        {
            mListSectionDataItems.Clear();

            #region 分段及每段数量

            var section = ListBoxSection.SelectedItem as SectionItem;
            if (section == null) { return; }
            int sectionNumber = section.Number;
            int sectionSize = mListSumValueItems.Count;
            if (sectionNumber == SscDefines.SECTION_10)
            {
                sectionSize = 10;
            }
            if (sectionNumber == SscDefines.SECTION_15)
            {
                sectionSize = 15;
            }
            if (sectionNumber == SscDefines.SECTION_20)
            {
                sectionSize = 20;
            }
            if (sectionNumber == SscDefines.SECTION_30)
            {
                sectionSize = 30;
            }

            #endregion


            #region 分段处理

            int k = 0, g = 0;
            SectionDataItem sectionData = new SectionDataItem();
            sectionData.Number = g;
            sectionData.Section = sectionNumber;
            mListSectionDataItems.Add(sectionData);
            for (int i = 0; i < mListSumValueItems.Count; i++)
            {
                var sumValueItem = mListSumValueItems[i];
                if (k == sectionSize)
                {
                    k = 0;
                    g++;
                    sectionData = new SectionDataItem();
                    sectionData.Number = g;
                    sectionData.Section = sectionNumber;
                    mListSectionDataItems.Add(sectionData);
                }
                SectionLargeSmallItem largeSmall = new SectionLargeSmallItem();
                largeSmall.Serial = sumValueItem.Serial;
                largeSmall.Number = sumValueItem.Number;
                largeSmall.Date = sumValueItem.Date;
                largeSmall.LargeValue = sumValueItem.LargeValue;
                var data = mListLargeSmallData.FirstOrDefault(l => l.Serial == largeSmall.Serial && l.Pos == 6);
                if (data != null)
                {
                    largeSmall.Times = data.LargeSmallNum;
                }
                sectionData.Items.Add(largeSmall);
                k++;
            }

            #endregion


            #region 每段处理

            for (int i = 0; i < mListSectionDataItems.Count; i++)
            {
                var item = mListSectionDataItems[i];
                int largeNum = 0;
                int smallNum = 0;
                int largeMaxNum = 0;
                int smallMaxNum = 0;
                for (int j = 0; j < item.Items.Count; j++)
                {
                    SectionLargeSmallItem largeSmall = item.Items[j];
                    if (largeSmall.LargeValue)
                    {
                        largeNum++;
                        largeMaxNum = Math.Max(largeSmall.Times, largeMaxNum);
                    }
                    else
                    {
                        smallNum++;
                        smallMaxNum = Math.Max(largeSmall.Times, smallMaxNum);
                    }
                }
                item.LargeNum = largeNum;
                item.SmallNum = smallNum;
                item.LargeMaxNum = largeMaxNum;
                item.SmallMaxNum = smallMaxNum;
                var first = item.Items.FirstOrDefault();
                var last = item.Items.LastOrDefault();
                if (first != null && last != null)
                {
                    var begin = first.Number;
                    var end = last.Number;
                    item.Name = string.Format("{0}~{1}", begin, end);
                }
            }

            #endregion

        }

        private void InitDistributeItems()
        {
            mListDistributeItems.Clear();
            var dateItem = ComboDate.SelectedItem as SumValueDateItem;
            if (dateItem != null)
            {
                for (int i = 0; i < dateItem.Items.Count; i++)
                {
                    var sumValueItem = dateItem.Items[i];
                    string serial = sumValueItem.Serial;
                    LargeSmallItem item = new LargeSmallItem();
                    item.Serial = serial;
                    item.Number = sumValueItem.Number;
                    item.Date = sumValueItem.Date;
                    int pos = 6;
                    item.Pos = pos;
                    var data = mListLargeSmallData.FirstOrDefault(l => l.Serial == serial && l.Pos == pos);
                    if (data != null)
                    {
                        item.LargeNum = data.LargeNum;
                        item.SmallNum = data.SmallNum;
                        item.SingleNum = data.SingleNum;
                        item.DoubleNum = data.DoubleNum;
                    }
                    mListDistributeItems.Add(item);
                }
            }
        }

        #endregion


        #region 事件处理

        void ComboDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitSumValueItems();
            InitSumValueTitle();
            InitSumViewSize();
            InitSectionDataItems();
            InitDistributeItems();
        }

        void ListBoxSumView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitSumViewSize();
        }

        void ListBoxSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitSectionDataItems();
        }

        #endregion


        #region Basic

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInfomation(string msg)
        {
            MessageBox.Show(msg, App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion


    }
}
