//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    0c1d8e66-7ee7-4a54-8fc1-de96d1edcbe6
//        CLR Version:              4.0.30319.42000
//        Name:                     UCNumberHot
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.UserControls
//        File Name:                UCNumberHot
//
//        Created by Charley at 2019/8/20 18:28:03
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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ShiShiCai.Common;
using ShiShiCai.Models;

namespace ShiShiCai.UserControls
{
    /// <summary>
    /// UCNumberHot.xaml 的交互逻辑
    /// </summary>
    public partial class UCNumberHot : IModuleView
    {

        #region Property

        public static readonly DependencyProperty PageParentProperty =
            DependencyProperty.Register("PageParent", typeof(MainWindow), typeof(UCNumberHot), new PropertyMetadata(default(MainWindow)));

        public MainWindow PageParent
        {
            get { return (MainWindow)GetValue(PageParentProperty); }
            set { SetValue(PageParentProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(UCNumberHot), new PropertyMetadata(default(double)));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty PosItemWidthProperty =
            DependencyProperty.Register("PosItemWidth", typeof(double), typeof(UCNumberHot), new PropertyMetadata(default(double)));

        public double PosItemWidth
        {
            get { return (double)GetValue(PosItemWidthProperty); }
            set { SetValue(PosItemWidthProperty, value); }
        }

        public ObservableCollection<NumberHotDateItem> DateItems
        {
            get { return mListDateItems; }
        }

        public ObservableCollection<NumberHotSectionItem> AllSectionItems
        {
            get { return mListAllSectionItems; }
        }

        public ObservableCollection<NumberHotSectionItem> PosSectionItems
        {
            get { return mListPosSectionItems; }
        }

        public ObservableCollection<NumberHotNumberItem> AllNumberItems
        {
            get { return mListAllNumberItems; }
        }

        public ObservableCollection<NumberHotNumberItem> PosNumberItems
        {
            get { return mListPosNumberItems; }
        }

        public ObservableCollection<int> AllYAxisLabels
        {
            get { return mListAllYAxisLabels; }
        }

        public ObservableCollection<NumberHotItem> AllNumberHotItems
        {
            get { return mListAllNumberHotItems; }
        }

        public ObservableCollection<PositionNumberHotItem> PosNumberHotItems
        {
            get { return mListPosNumberHotItems; }
        }

        #endregion


        #region Members

        private readonly List<NumberHotItem> mListNumberHotData = new List<NumberHotItem>();
        private readonly ObservableCollection<NumberHotDateItem> mListDateItems = new ObservableCollection<NumberHotDateItem>();
        private readonly ObservableCollection<NumberHotSectionItem> mListAllSectionItems = new ObservableCollection<NumberHotSectionItem>();
        private readonly ObservableCollection<NumberHotSectionItem> mListPosSectionItems = new ObservableCollection<NumberHotSectionItem>();
        private readonly ObservableCollection<NumberHotNumberItem> mListAllNumberItems = new ObservableCollection<NumberHotNumberItem>();
        private readonly ObservableCollection<NumberHotNumberItem> mListPosNumberItems = new ObservableCollection<NumberHotNumberItem>();
        private readonly ObservableCollection<int> mListAllYAxisLabels = new ObservableCollection<int>();
        private readonly ObservableCollection<NumberHotItem> mListAllNumberHotItems = new ObservableCollection<NumberHotItem>();
        private readonly ObservableCollection<PositionNumberHotItem> mListPosNumberHotItems = new ObservableCollection<PositionNumberHotItem>();

        private bool mIsInited;

        #endregion


        public UCNumberHot()
        {
            InitializeComponent();

            Loaded += UCNumberHot_Loaded;
            ComboDate.SelectionChanged += ComboDate_SelectionChanged;
            ListBoxAllSection.SelectionChanged += ListBoxAllSection_SelectionChanged;
            ListBoxPosSection.SelectionChanged += ListBoxPosSection_SelectionChanged;
            ListBoxAllNumber.AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(CheckBoxAllNumber_Checked));
            ListBoxAllNumber.AddHandler(ToggleButton.UncheckedEvent, new RoutedEventHandler(CheckBoxAllNumber_Unchecked));
            ListBoxPosNumber.AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(CheckBoxPosNumber_Checked));
            ListBoxPosNumber.AddHandler(ToggleButton.UncheckedEvent, new RoutedEventHandler(CheckBoxPosNumber_Unchecked));
            ListBoxAllNumberHot.SizeChanged += ListBoxAllNumberHot_SizeChanged;
            ListBoxPosNumberHot.SizeChanged += ListBoxPosNumberHot_SizeChanged;
        }

        void UCNumberHot_Loaded(object sender, RoutedEventArgs e)
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
            InitAllSectionItems();
            InitPosSectionItems();
            InitAllNumberItems();
            InitPosNumberItems();
            InitNumberHotData();
            InitDateItems();

            ComboDate.SelectedItem = mListDateItems.FirstOrDefault();
        }

        private void InitAllSectionItems()
        {
            mListAllSectionItems.Clear();
            NumberHotSectionItem item = new NumberHotSectionItem();
            item.Section = 5;
            item.Name = "5期";
            item.IsSelected = true;
            mListAllSectionItems.Add(item);
            item = new NumberHotSectionItem();
            item.Section = 10;
            item.Name = "10期";
            mListAllSectionItems.Add(item);
            item = new NumberHotSectionItem();
            item.Section = 15;
            item.Name = "15期";
            mListAllSectionItems.Add(item);
            item = new NumberHotSectionItem();
            item.Section = 20;
            item.Name = "20期";
            mListAllSectionItems.Add(item);
        }

        private void InitPosSectionItems()
        {
            mListPosSectionItems.Clear();
            NumberHotSectionItem item = new NumberHotSectionItem();
            item.Section = 15;
            item.Name = "15期";
            item.IsSelected = true;
            mListPosSectionItems.Add(item);
            item = new NumberHotSectionItem();
            item.Section = 20;
            item.Name = "20期";
            mListPosSectionItems.Add(item);
        }

        private void InitAllNumberItems()
        {
            mListAllNumberItems.Clear();
            NumberHotNumberItem item = new NumberHotNumberItem();
            item.Number = 0;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            item.IsSelected = true;
            mListAllNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 1;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            mListAllNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 2;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
            mListAllNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 3;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 128, 64, 64));
            mListAllNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 4;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 64, 128, 64));
            mListAllNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 5;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 64, 64, 128));
            mListAllNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 6;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 32, 128, 128));
            mListAllNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 7;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 128, 32, 128));
            mListAllNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 8;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 128, 128, 32));
            mListAllNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 9;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 255, 128, 128));
            mListAllNumberItems.Add(item);
        }

        private void InitPosNumberItems()
        {
            mListPosNumberItems.Clear();
            NumberHotNumberItem item = new NumberHotNumberItem();
            item.Number = 0;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            item.IsSelected = true;
            mListPosNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 1;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            mListPosNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 2;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
            mListPosNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 3;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 128, 64, 64));
            mListPosNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 4;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 64, 128, 64));
            mListPosNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 5;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 64, 64, 128));
            mListPosNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 6;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 32, 128, 128));
            mListPosNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 7;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 128, 32, 128));
            mListPosNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 8;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 128, 128, 32));
            mListPosNumberItems.Add(item);
            item = new NumberHotNumberItem();
            item.Number = 9;
            item.Color = new SolidColorBrush(Color.FromArgb(255, 255, 128, 128));
            mListPosNumberItems.Add(item);
        }

        private void InitNumberHotData()
        {
            mListNumberHotData.Clear();
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
                "SELECT * FROM T_107_19 WHERE C001 >= {0} AND C001 <= {1} ORDER BY C001, C004, C005", begin, end);
            OperationReturn optReturn = MssqlOperation.GetDataSet(strConn, strSql);
            if (!optReturn.Result)
            {
                ShowException(string.Format("load number hot data fail. [{0}]{1}", optReturn.Code, optReturn.Message));
                return;
            }
            DataSet objDataSet = optReturn.Data as DataSet;
            if (objDataSet == null) { return; }
            for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
            {
                DataRow dr = objDataSet.Tables[0].Rows[i];
                NumberHotItem item = new NumberHotItem();
                item.Serial = dr["C001"].ToString();
                item.Date = Convert.ToInt32(dr["C002"]);
                item.Number = Convert.ToInt32(dr["C003"]);
                item.Pos = Convert.ToInt32(dr["C004"]);
                item.Section = Convert.ToInt16(dr["C005"]);
                item.Num0 = Convert.ToInt32(dr["C010"]);
                item.Num1 = Convert.ToInt32(dr["C011"]);
                item.Num2 = Convert.ToInt32(dr["C012"]);
                item.Num3 = Convert.ToInt32(dr["C013"]);
                item.Num4 = Convert.ToInt32(dr["C014"]);
                item.Num5 = Convert.ToInt32(dr["C015"]);
                item.Num6 = Convert.ToInt32(dr["C016"]);
                item.Num7 = Convert.ToInt32(dr["C017"]);
                item.Num8 = Convert.ToInt32(dr["C018"]);
                item.Num9 = Convert.ToInt32(dr["C019"]);
                mListNumberHotData.Add(item);
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
                NumberHotDateItem item = new NumberHotDateItem();
                item.Date = date;
                mListDateItems.Add(item);
            }
        }

        private void InitAllNumberHotItems()
        {
            mListAllNumberHotItems.Clear();
            var dateItem = ComboDate.SelectedItem as NumberHotDateItem;
            if (dateItem == null) { return; }
            int date = dateItem.Date;
            var sectionItem = ListBoxAllSection.SelectedItem as NumberHotSectionItem;
            if (sectionItem == null) { return; }
            int section = sectionItem.Section;
            int pos = 6;
            var data = mListNumberHotData.Where(n => n.Date == date && n.Section == section && n.Pos == pos).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                var dataItem = data[i];
                NumberHotItem item = new NumberHotItem();
                item.Serial = dataItem.Serial;
                item.Number = dataItem.Number;
                item.Date = dateItem.Date;
                item.Pos = pos;
                item.Section = section;
                item.Num0 = dataItem.Num0;
                item.Num1 = dataItem.Num1;
                item.Num2 = dataItem.Num2;
                item.Num3 = dataItem.Num3;
                item.Num4 = dataItem.Num4;
                item.Num5 = dataItem.Num5;
                item.Num6 = dataItem.Num6;
                item.Num7 = dataItem.Num7;
                item.Num8 = dataItem.Num8;
                item.Num9 = dataItem.Num9;
                mListAllNumberHotItems.Add(item);
            }
        }

        private void InitAllItemWidth()
        {
            int count = 60;
            double width = ListBoxAllNumberHot.ActualWidth;
            double itemWidth = width / (count * 1.0);
            ItemWidth = itemWidth;
        }

        private void InitAllYAxisLabels()
        {
            mListAllYAxisLabels.Clear();
            var sectionItem = ListBoxAllSection.SelectedItem as NumberHotSectionItem;
            if (sectionItem == null) { return; }
            var section = sectionItem.Section;
            if (section == 5)
            {
                mListAllYAxisLabels.Add(10);
                mListAllYAxisLabels.Add(8);
                mListAllYAxisLabels.Add(6);
                mListAllYAxisLabels.Add(4);
                mListAllYAxisLabels.Add(2);
            }
            if (section == 10)
            {
                mListAllYAxisLabels.Add(20);
                mListAllYAxisLabels.Add(16);
                mListAllYAxisLabels.Add(12);
                mListAllYAxisLabels.Add(8);
                mListAllYAxisLabels.Add(4);
            }
            if (section == 15)
            {
                mListAllYAxisLabels.Add(30);
                mListAllYAxisLabels.Add(24);
                mListAllYAxisLabels.Add(18);
                mListAllYAxisLabels.Add(12);
                mListAllYAxisLabels.Add(6);
            }
            if (section == 20)
            {
                mListAllYAxisLabels.Add(40);
                mListAllYAxisLabels.Add(32);
                mListAllYAxisLabels.Add(24);
                mListAllYAxisLabels.Add(16);
                mListAllYAxisLabels.Add(8);
            }
        }

        private void InitAllNumberHeight()
        {
            var sectionItem = ListBoxAllSection.SelectedItem as NumberHotSectionItem;
            if (sectionItem == null) { return; }
            var section = sectionItem.Section;
            int maxNum = section * 2;
            double height = ListBoxAllNumberHot.ActualHeight;
            for (int i = 0; i < mListAllNumberHotItems.Count; i++)
            {
                var item = mListAllNumberHotItems[i];
                item.Num0Height = (item.Num0 / (maxNum * 1.0)) * height;
                item.Num1Height = (item.Num1 / (maxNum * 1.0)) * height;
                item.Num2Height = (item.Num2 / (maxNum * 1.0)) * height;
                item.Num3Height = (item.Num3 / (maxNum * 1.0)) * height;
                item.Num4Height = (item.Num4 / (maxNum * 1.0)) * height;
                item.Num5Height = (item.Num5 / (maxNum * 1.0)) * height;
                item.Num6Height = (item.Num6 / (maxNum * 1.0)) * height;
                item.Num7Height = (item.Num7 / (maxNum * 1.0)) * height;
                item.Num8Height = (item.Num8 / (maxNum * 1.0)) * height;
                item.Num9Height = (item.Num9 / (maxNum * 1.0)) * height;


                #region 颜色和可见性

                for (int k = 0; k < mListAllNumberItems.Count; k++)
                {
                    var numberItem = mListAllNumberItems[k];
                    int number = numberItem.Number;
                    if (number == 0)
                    {
                        item.Num0Color = numberItem.Color;
                        item.Num0Selected = numberItem.IsSelected;
                    }
                    if (number == 1)
                    {
                        item.Num1Color = numberItem.Color;
                        item.Num1Selected = numberItem.IsSelected;
                    }
                    if (number == 2)
                    {
                        item.Num2Color = numberItem.Color;
                        item.Num2Selected = numberItem.IsSelected;
                    }
                    if (number == 3)
                    {
                        item.Num3Color = numberItem.Color;
                        item.Num3Selected = numberItem.IsSelected;
                    }
                    if (number == 4)
                    {
                        item.Num4Color = numberItem.Color;
                        item.Num4Selected = numberItem.IsSelected;
                    }
                    if (number == 5)
                    {
                        item.Num5Color = numberItem.Color;
                        item.Num5Selected = numberItem.IsSelected;
                    }
                    if (number == 6)
                    {
                        item.Num6Color = numberItem.Color;
                        item.Num6Selected = numberItem.IsSelected;
                    }
                    if (number == 7)
                    {
                        item.Num7Color = numberItem.Color;
                        item.Num7Selected = numberItem.IsSelected;
                    }
                    if (number == 8)
                    {
                        item.Num8Color = numberItem.Color;
                        item.Num8Selected = numberItem.IsSelected;
                    }
                    if (number == 9)
                    {
                        item.Num9Color = numberItem.Color;
                        item.Num9Selected = numberItem.IsSelected;
                    }
                }

                #endregion

            }
        }

        private void InitAllNumberPaths()
        {
            int count = 60;
            int offset = 4;
            double width = ListBoxAllNumberHot.ActualWidth;
            double height = ListBoxAllNumberHot.ActualHeight;
            double itemWidth = width / (count * 1.0);
            var sectionItem = ListBoxAllSection.SelectedItem as NumberHotSectionItem;
            if (sectionItem == null) { return; }
            var section = sectionItem.Section;
            int maxNum = section * 2;
            for (int i = 0; i < mListAllNumberItems.Count; i++)
            {
                var numberItem = mListAllNumberItems[i];
                int number = numberItem.Number;
                var firstItem = mListAllNumberHotItems.FirstOrDefault();
                if (firstItem == null) { continue; }
                int numValue = 0;
                if (number == 0)
                {
                    numValue = firstItem.Num0;
                }
                if (number == 1)
                {
                    numValue = firstItem.Num1;
                }
                if (number == 2)
                {
                    numValue = firstItem.Num2;
                }
                if (number == 3)
                {
                    numValue = firstItem.Num3;
                }
                if (number == 4)
                {
                    numValue = firstItem.Num4;
                }
                if (number == 5)
                {
                    numValue = firstItem.Num5;
                }
                if (number == 6)
                {
                    numValue = firstItem.Num6;
                }
                if (number == 7)
                {
                    numValue = firstItem.Num7;
                }
                if (number == 8)
                {
                    numValue = firstItem.Num8;
                }
                if (number == 9)
                {
                    numValue = firstItem.Num9;
                }
                PathSegmentCollection segments = new PathSegmentCollection();
                double firstX = itemWidth / 2.0;
                double firstY = height - (numValue / (maxNum * 1.0)) * height + offset;
                Point firtPoint = new Point(firstX, firstY);
                for (int j = 0; j < mListAllNumberHotItems.Count; j++)
                {
                    var hotItem = mListAllNumberHotItems[j];
                    numValue = 0;
                    if (number == 0)
                    {
                        numValue = hotItem.Num0;
                    }
                    if (number == 1)
                    {
                        numValue = hotItem.Num1;
                    }
                    if (number == 2)
                    {
                        numValue = hotItem.Num2;
                    }
                    if (number == 3)
                    {
                        numValue = hotItem.Num3;
                    }
                    if (number == 4)
                    {
                        numValue = hotItem.Num4;
                    }
                    if (number == 5)
                    {
                        numValue = hotItem.Num5;
                    }
                    if (number == 6)
                    {
                        numValue = hotItem.Num6;
                    }
                    if (number == 7)
                    {
                        numValue = hotItem.Num7;
                    }
                    if (number == 8)
                    {
                        numValue = hotItem.Num8;
                    }
                    if (number == 9)
                    {
                        numValue = hotItem.Num9;
                    }
                    double x = itemWidth * j * 1.0 + itemWidth / 2.0;
                    double y = height - (numValue / (maxNum * 1.0)) * height + offset;
                    Point point = new Point(x, y);
                    segments.Add(new LineSegment { Point = point });
                }
                PathGeometry path = new PathGeometry { Figures = new PathFigureCollection { new PathFigure { StartPoint = firtPoint, Segments = segments } } };
                numberItem.Path = path;
            }
        }

        private void InitPosNumberHotItems()
        {
            mListPosNumberHotItems.Clear();
            PositionNumberHotItem item = new PositionNumberHotItem();
            item.Pos = 5;
            item.Name = "万位";
            mListPosNumberHotItems.Add(item);
            item = new PositionNumberHotItem();
            item.Pos = 4;
            item.Name = "千位";
            mListPosNumberHotItems.Add(item);
            item = new PositionNumberHotItem();
            item.Pos = 3;
            item.Name = "百位";
            mListPosNumberHotItems.Add(item);
            item = new PositionNumberHotItem();
            item.Pos = 2;
            item.Name = "十位";
            mListPosNumberHotItems.Add(item);
            item = new PositionNumberHotItem();
            item.Pos = 1;
            item.Name = "个位";
            mListPosNumberHotItems.Add(item);
        }

        private void InitPosNumberHotData()
        {
            for (int i = 0; i < mListPosNumberHotItems.Count; i++)
            {
                var posItem = mListPosNumberHotItems[i];
                int pos = posItem.Pos;
                posItem.Items.Clear();
                var dateItem = ComboDate.SelectedItem as NumberHotDateItem;
                if (dateItem == null) { return; }
                int date = dateItem.Date;
                var sectionItem = ListBoxPosSection.SelectedItem as NumberHotSectionItem;
                if (sectionItem == null) { return; }
                int section = sectionItem.Section;
                var data = mListNumberHotData.Where(n => n.Date == date && n.Section == section && n.Pos == pos).ToList();
                for (int j = 0; j < data.Count; j++)
                {
                    var dataItem = data[j];
                    NumberHotItem item = new NumberHotItem();
                    item.Serial = dataItem.Serial;
                    item.Number = dataItem.Number;
                    item.Date = dateItem.Date;
                    item.Pos = pos;
                    item.Section = section;
                    item.Num0 = dataItem.Num0;
                    item.Num1 = dataItem.Num1;
                    item.Num2 = dataItem.Num2;
                    item.Num3 = dataItem.Num3;
                    item.Num4 = dataItem.Num4;
                    item.Num5 = dataItem.Num5;
                    item.Num6 = dataItem.Num6;
                    item.Num7 = dataItem.Num7;
                    item.Num8 = dataItem.Num8;
                    item.Num9 = dataItem.Num9;
                    posItem.Items.Add(item);
                }
            }
        }

        private void InitPosItemWidth()
        {
            double width = ListBoxPosNumberHot.ActualWidth;
            width = width - 30;
            int count = 60;
            double itemWidth = width / (count * 1.0);
            PosItemWidth = itemWidth;
            for (int i = 0; i < mListPosNumberHotItems.Count; i++)
            {
                var posItem = mListPosNumberHotItems[i];
                posItem.ItemWidth = itemWidth;
            }
        }

        private void InitPosYAxisLabels()
        {
            for (int i = 0; i < mListPosNumberHotItems.Count; i++)
            {
                var posItem = mListPosNumberHotItems[i];
                posItem.YAxisLabels.Clear();
                var sectionItem = ListBoxPosSection.SelectedItem as NumberHotSectionItem;
                if (sectionItem == null) { continue; }
                var section = sectionItem.Section;
                if (section == 15)
                {
                    posItem.YAxisLabels.Add(15);
                    posItem.YAxisLabels.Add(12);
                    posItem.YAxisLabels.Add(9);
                    posItem.YAxisLabels.Add(6);
                    posItem.YAxisLabels.Add(3);
                }
                if (section == 20)
                {
                    posItem.YAxisLabels.Add(20);
                    posItem.YAxisLabels.Add(16);
                    posItem.YAxisLabels.Add(12);
                    posItem.YAxisLabels.Add(8);
                    posItem.YAxisLabels.Add(4);
                }
            }
        }

        private void InitPosNumberHeight()
        {
            var sectionItem = ListBoxPosSection.SelectedItem as NumberHotSectionItem;
            if (sectionItem == null) { return; }
            var section = sectionItem.Section;
            int maxNum = section;
            double height = 120 - 20;
            for (int i = 0; i < mListPosNumberHotItems.Count; i++)
            {
                var posItem = mListPosNumberHotItems[i];
                for (int j = 0; j < posItem.Items.Count; j++)
                {
                    var item = posItem.Items[j];
                    item.Num0Height = (item.Num0 / (maxNum * 1.0)) * height;
                    item.Num1Height = (item.Num1 / (maxNum * 1.0)) * height;
                    item.Num2Height = (item.Num2 / (maxNum * 1.0)) * height;
                    item.Num3Height = (item.Num3 / (maxNum * 1.0)) * height;
                    item.Num4Height = (item.Num4 / (maxNum * 1.0)) * height;
                    item.Num5Height = (item.Num5 / (maxNum * 1.0)) * height;
                    item.Num6Height = (item.Num6 / (maxNum * 1.0)) * height;
                    item.Num7Height = (item.Num7 / (maxNum * 1.0)) * height;
                    item.Num8Height = (item.Num8 / (maxNum * 1.0)) * height;
                    item.Num9Height = (item.Num9 / (maxNum * 1.0)) * height;

                    #region 颜色和可见性

                    for (int k = 0; k < mListPosNumberItems.Count; k++)
                    {
                        var numberItem = mListPosNumberItems[k];
                        int number = numberItem.Number;
                        if (number == 0)
                        {
                            item.Num0Color = numberItem.Color;
                            item.Num0Selected = numberItem.IsSelected;
                        }
                        if (number == 1)
                        {
                            item.Num1Color = numberItem.Color;
                            item.Num1Selected = numberItem.IsSelected;
                        }
                        if (number == 2)
                        {
                            item.Num2Color = numberItem.Color;
                            item.Num2Selected = numberItem.IsSelected;
                        }
                        if (number == 3)
                        {
                            item.Num3Color = numberItem.Color;
                            item.Num3Selected = numberItem.IsSelected;
                        }
                        if (number == 4)
                        {
                            item.Num4Color = numberItem.Color;
                            item.Num4Selected = numberItem.IsSelected;
                        }
                        if (number == 5)
                        {
                            item.Num5Color = numberItem.Color;
                            item.Num5Selected = numberItem.IsSelected;
                        }
                        if (number == 6)
                        {
                            item.Num6Color = numberItem.Color;
                            item.Num6Selected = numberItem.IsSelected;
                        }
                        if (number == 7)
                        {
                            item.Num7Color = numberItem.Color;
                            item.Num7Selected = numberItem.IsSelected;
                        }
                        if (number == 8)
                        {
                            item.Num8Color = numberItem.Color;
                            item.Num8Selected = numberItem.IsSelected;
                        }
                        if (number == 9)
                        {
                            item.Num9Color = numberItem.Color;
                            item.Num9Selected = numberItem.IsSelected;
                        }
                    }

                    #endregion

                }
            }
        }

        private void InitPosNumberPaths()
        {
            int count = 60;
            int offset = 4;
            double width = ListBoxPosNumberHot.ActualWidth;
            width = width - 30;
            double height = 120 - 20;
            double itemWidth = width / (count * 1.0);
            var sectionItem = ListBoxPosSection.SelectedItem as NumberHotSectionItem;
            if (sectionItem == null) { return; }
            var section = sectionItem.Section;
            int maxNum = section;
            for (int i = 0; i < mListPosNumberHotItems.Count; i++)
            {
                var posItem = mListPosNumberHotItems[i];
                posItem.NumberItems.Clear();
                for (int j = 0; j < mListPosNumberItems.Count; j++)
                {
                    var numberItem = mListPosNumberItems[j];
                    int number = numberItem.Number;
                    var firstItem = posItem.Items.FirstOrDefault();
                    if (firstItem == null) { continue; }
                    int numValue = 0;
                    if (number == 0)
                    {
                        numValue = firstItem.Num0;
                    }
                    if (number == 1)
                    {
                        numValue = firstItem.Num1;
                    }
                    if (number == 2)
                    {
                        numValue = firstItem.Num2;
                    }
                    if (number == 3)
                    {
                        numValue = firstItem.Num3;
                    }
                    if (number == 4)
                    {
                        numValue = firstItem.Num4;
                    }
                    if (number == 5)
                    {
                        numValue = firstItem.Num5;
                    }
                    if (number == 6)
                    {
                        numValue = firstItem.Num6;
                    }
                    if (number == 7)
                    {
                        numValue = firstItem.Num7;
                    }
                    if (number == 8)
                    {
                        numValue = firstItem.Num8;
                    }
                    if (number == 9)
                    {
                        numValue = firstItem.Num9;
                    }
                    PathSegmentCollection segments = new PathSegmentCollection();
                    double firstX = itemWidth / 2.0;
                    double firstY = height - (numValue / (maxNum * 1.0)) * height + offset;
                    Point firtPoint = new Point(firstX, firstY);
                    for (int k = 0; k < posItem.Items.Count; k++)
                    {
                        var hotItem = posItem.Items[k];
                        numValue = 0;
                        if (number == 0)
                        {
                            numValue = hotItem.Num0;
                        }
                        if (number == 1)
                        {
                            numValue = hotItem.Num1;
                        }
                        if (number == 2)
                        {
                            numValue = hotItem.Num2;
                        }
                        if (number == 3)
                        {
                            numValue = hotItem.Num3;
                        }
                        if (number == 4)
                        {
                            numValue = hotItem.Num4;
                        }
                        if (number == 5)
                        {
                            numValue = hotItem.Num5;
                        }
                        if (number == 6)
                        {
                            numValue = hotItem.Num6;
                        }
                        if (number == 7)
                        {
                            numValue = hotItem.Num7;
                        }
                        if (number == 8)
                        {
                            numValue = hotItem.Num8;
                        }
                        if (number == 9)
                        {
                            numValue = hotItem.Num9;
                        }
                        double x = itemWidth * k * 1.0 + itemWidth / 2.0;
                        double y = height - (numValue / (maxNum * 1.0)) * height + offset;
                        Point point = new Point(x, y);
                        segments.Add(new LineSegment { Point = point });
                    }
                    PathGeometry path = new PathGeometry { Figures = new PathFigureCollection { new PathFigure { StartPoint = firtPoint, Segments = segments } } };
                    NumberHotNumberItem newItem = new NumberHotNumberItem();
                    newItem.Number = numberItem.Number;
                    newItem.Color = numberItem.Color;
                    newItem.IsSelected = numberItem.IsSelected;
                    newItem.Path = path;
                    posItem.NumberItems.Add(newItem);
                }
            }
        }

        #endregion


        #region 事件处理

        void ComboDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitAllNumberHotItems();
            InitAllYAxisLabels();
            InitAllItemWidth();
            InitAllNumberHeight();
            InitAllNumberPaths();
            InitPosNumberHotItems();
            InitPosNumberHotData();
            InitPosItemWidth();
            InitPosYAxisLabels();
            InitPosNumberHeight();
        }

        void ListBoxAllSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitAllNumberHotItems();
            InitAllYAxisLabels();
            InitAllItemWidth();
            InitAllNumberHeight();
            InitAllNumberPaths();
        }

        void ListBoxPosSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitPosNumberHotItems();
            InitPosNumberHotData();
            InitPosItemWidth();
            InitPosYAxisLabels();
            InitPosNumberHeight();
        }

        void CheckBoxAllNumber_Checked(object sender, RoutedEventArgs e)
        {
            InitAllItemWidth();
            InitAllNumberHeight();
            InitAllNumberPaths();
        }

        void CheckBoxAllNumber_Unchecked(object sender, RoutedEventArgs e)
        {
            InitAllItemWidth();
            InitAllNumberHeight();
            InitAllNumberPaths();
        }

        void CheckBoxPosNumber_Checked(object sender, RoutedEventArgs e)
        {
            InitPosItemWidth();
            InitPosNumberHeight();
            InitPosNumberPaths();
        }

        void CheckBoxPosNumber_Unchecked(object sender, RoutedEventArgs e)
        {
            InitPosItemWidth();
            InitPosNumberHeight();
            InitPosNumberPaths();
        }

        void ListBoxAllNumberHot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitAllItemWidth();
            InitAllNumberHeight();
            InitAllNumberPaths();
        }

        void ListBoxPosNumberHot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitPosItemWidth();
            InitPosNumberHeight();
            InitPosNumberPaths();
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
