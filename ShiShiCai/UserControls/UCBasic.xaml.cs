//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    a1c7f07b-d49a-4e15-9e44-a54ec8f132e3
//        CLR Version:              4.0.30319.42000
//        Name:                     UCBasic
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.UserControls
//        File Name:                UCBasic
//
//        Created by Charley at 2019/8/11 11:46:46
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
using System.Windows.Input;
using System.Windows.Media;
using ShiShiCai.Common;
using ShiShiCai.Models;

namespace ShiShiCai.UserControls
{
    /// <summary>
    /// UCBasic.xaml 的交互逻辑
    /// </summary>
    public partial class UCBasic : IModuleView
    {

        #region PageParent

        public static readonly DependencyProperty PageParentProperty =
          DependencyProperty.Register("PageParent", typeof(MainWindow), typeof(UCBasic), new PropertyMetadata(default(MainWindow)));

        public MainWindow PageParent
        {
            get { return (MainWindow)GetValue(PageParentProperty); }
            set { SetValue(PageParentProperty, value); }
        }

        #endregion


        #region 位置颜色和可见性

        public static readonly DependencyProperty Path1VisibleProperty =
           DependencyProperty.Register("Path1Visible", typeof(bool), typeof(UCBasic), new PropertyMetadata(default(bool)));

        public bool Path1Visible
        {
            get { return (bool)GetValue(Path1VisibleProperty); }
            set { SetValue(Path1VisibleProperty, value); }
        }

        public static readonly DependencyProperty Path2VisibleProperty =
            DependencyProperty.Register("Path2Visible", typeof(bool), typeof(UCBasic), new PropertyMetadata(default(bool)));

        public bool Path2Visible
        {
            get { return (bool)GetValue(Path2VisibleProperty); }
            set { SetValue(Path2VisibleProperty, value); }
        }

        public static readonly DependencyProperty Path3VisibleProperty =
            DependencyProperty.Register("Path3Visible", typeof(bool), typeof(UCBasic), new PropertyMetadata(default(bool)));

        public bool Path3Visible
        {
            get { return (bool)GetValue(Path3VisibleProperty); }
            set { SetValue(Path3VisibleProperty, value); }
        }

        public static readonly DependencyProperty Path4VisibleProperty =
            DependencyProperty.Register("Path4Visible", typeof(bool), typeof(UCBasic), new PropertyMetadata(default(bool)));

        public bool Path4Visible
        {
            get { return (bool)GetValue(Path4VisibleProperty); }
            set { SetValue(Path4VisibleProperty, value); }
        }

        public static readonly DependencyProperty Path5VisibleProperty =
            DependencyProperty.Register("Path5Visible", typeof(bool), typeof(UCBasic), new PropertyMetadata(true));

        public bool Path5Visible
        {
            get { return (bool)GetValue(Path5VisibleProperty); }
            set { SetValue(Path5VisibleProperty, value); }
        }

        public static readonly DependencyProperty Path1ColorProperty =
            DependencyProperty.Register("Path1Color", typeof(Brush), typeof(UCBasic), new PropertyMetadata(Brushes.Red));

        public Brush Path1Color
        {
            get { return (Brush)GetValue(Path1ColorProperty); }
            set { SetValue(Path1ColorProperty, value); }
        }

        public static readonly DependencyProperty Path2ColorProperty =
            DependencyProperty.Register("Path2Color", typeof(Brush), typeof(UCBasic), new PropertyMetadata(Brushes.Green));

        public Brush Path2Color
        {
            get { return (Brush)GetValue(Path2ColorProperty); }
            set { SetValue(Path2ColorProperty, value); }
        }

        public static readonly DependencyProperty Path3ColorProperty =
            DependencyProperty.Register("Path3Color", typeof(Brush), typeof(UCBasic), new PropertyMetadata(Brushes.Blue));

        public Brush Path3Color
        {
            get { return (Brush)GetValue(Path3ColorProperty); }
            set { SetValue(Path3ColorProperty, value); }
        }

        public static readonly DependencyProperty Path4ColorProperty =
            DependencyProperty.Register("Path4Color", typeof(Brush), typeof(UCBasic), new PropertyMetadata(Brushes.Orange));

        public Brush Path4Color
        {
            get { return (Brush)GetValue(Path4ColorProperty); }
            set { SetValue(Path4ColorProperty, value); }
        }

        public static readonly DependencyProperty Path5ColorProperty =
            DependencyProperty.Register("Path5Color", typeof(Brush), typeof(UCBasic), new PropertyMetadata(Brushes.Fuchsia));

        public Brush Path5Color
        {
            get { return (Brush)GetValue(Path5ColorProperty); }
            set { SetValue(Path5ColorProperty, value); }
        }

        #endregion


        #region 路径

        public static readonly DependencyProperty Path1Property =
           DependencyProperty.Register("Path1", typeof(PathGeometry), typeof(UCBasic), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry Path1
        {
            get { return (PathGeometry)GetValue(Path1Property); }
            set { SetValue(Path1Property, value); }
        }

        public static readonly DependencyProperty Path2Property =
            DependencyProperty.Register("Path2", typeof(PathGeometry), typeof(UCBasic), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry Path2
        {
            get { return (PathGeometry)GetValue(Path2Property); }
            set { SetValue(Path2Property, value); }
        }

        public static readonly DependencyProperty Path3Property =
            DependencyProperty.Register("Path3", typeof(PathGeometry), typeof(UCBasic), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry Path3
        {
            get { return (PathGeometry)GetValue(Path3Property); }
            set { SetValue(Path3Property, value); }
        }

        public static readonly DependencyProperty Path4Property =
            DependencyProperty.Register("Path4", typeof(PathGeometry), typeof(UCBasic), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry Path4
        {
            get { return (PathGeometry)GetValue(Path4Property); }
            set { SetValue(Path4Property, value); }
        }

        public static readonly DependencyProperty Path5Property =
            DependencyProperty.Register("Path5", typeof(PathGeometry), typeof(UCBasic), new PropertyMetadata(default(PathGeometry)));

        public PathGeometry Path5
        {
            get { return (PathGeometry)GetValue(Path5Property); }
            set { SetValue(Path5Property, value); }
        }

        #endregion


        #region 位置勾选命令

        private static readonly RoutedUICommand mPositionClickCommand = new RoutedUICommand();

        public static RoutedUICommand PositionClickCommand
        {
            get { return mPositionClickCommand; }
        }

        #endregion


        #region Members

        private readonly List<IssueItem> mListLostData = new List<IssueItem>();
        private readonly ObservableCollection<IssueDateItem> mListGroupItems = new ObservableCollection<IssueDateItem>();
        private readonly ObservableCollection<PositionItem> mListPositionItems = new ObservableCollection<PositionItem>();
        private readonly ObservableCollection<IssueItem> mListIssueItems = new ObservableCollection<IssueItem>();

        private bool mIsInited;

        #endregion



        public UCBasic()
        {
            InitializeComponent();

            Loaded += UCBasic_Loaded;
            TabControlView.SelectionChanged += TabControlView_SelectionChanged;

            CommandBindings.Add(new CommandBinding(PositionClickCommand,
                PositionClick_Executed, (s, ce) => ce.CanExecute = true));

            ListBoxIssueDate.ItemsSource = mListGroupItems;
            ListBoxIssueItem.ItemsSource = mListIssueItems;
        }

        void UCBasic_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }


        #region Init and load

        public void Reload()
        {
            Init();
        }

        private void Init()
        {
            LoadLostData();
            InitPositionItems();
            InitIssueItems();
            InitIssueDateItems();
            InitIssuePaths();
            InitPath();
        }

        private void LoadLostData()
        {
            mListLostData.Clear();
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
                "SELECT * FROM T_102_19 WHERE C001 >= {0} AND C001 <= {1} ORDER BY C001 DESC", begin, end);
            OperationReturn optReturn = MssqlOperation.GetDataSet(strConn, strSql);
            if (!optReturn.Result)
            {
                ShowException(string.Format("load lost data fail. [{0}]{1}", optReturn.Code, optReturn.Message));
                return;
            }
            DataSet objDataSet = optReturn.Data as DataSet;
            if (objDataSet == null) { return; }
            for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
            {
                DataRow dr = objDataSet.Tables[0].Rows[i];
                IssueItem item = new IssueItem();
                item.Serial = dr["C001"].ToString();
                item.Date = Convert.ToInt32(dr["C002"]);
                item.Number = Convert.ToInt32(dr["C003"]);

                item.D5Lost0 = Convert.ToInt32(dr["C050"]);
                item.D5Lost1 = Convert.ToInt32(dr["C051"]);
                item.D5Lost2 = Convert.ToInt32(dr["C052"]);
                item.D5Lost3 = Convert.ToInt32(dr["C053"]);
                item.D5Lost4 = Convert.ToInt32(dr["C054"]);
                item.D5Lost5 = Convert.ToInt32(dr["C055"]);
                item.D5Lost6 = Convert.ToInt32(dr["C056"]);
                item.D5Lost7 = Convert.ToInt32(dr["C057"]);
                item.D5Lost8 = Convert.ToInt32(dr["C058"]);
                item.D5Lost9 = Convert.ToInt32(dr["C059"]);

                item.D4Lost0 = Convert.ToInt32(dr["C040"]);
                item.D4Lost1 = Convert.ToInt32(dr["C041"]);
                item.D4Lost2 = Convert.ToInt32(dr["C042"]);
                item.D4Lost3 = Convert.ToInt32(dr["C043"]);
                item.D4Lost4 = Convert.ToInt32(dr["C044"]);
                item.D4Lost5 = Convert.ToInt32(dr["C045"]);
                item.D4Lost6 = Convert.ToInt32(dr["C046"]);
                item.D4Lost7 = Convert.ToInt32(dr["C047"]);
                item.D4Lost8 = Convert.ToInt32(dr["C048"]);
                item.D4Lost9 = Convert.ToInt32(dr["C049"]);

                item.D3Lost0 = Convert.ToInt32(dr["C030"]);
                item.D3Lost1 = Convert.ToInt32(dr["C031"]);
                item.D3Lost2 = Convert.ToInt32(dr["C032"]);
                item.D3Lost3 = Convert.ToInt32(dr["C033"]);
                item.D3Lost4 = Convert.ToInt32(dr["C034"]);
                item.D3Lost5 = Convert.ToInt32(dr["C035"]);
                item.D3Lost6 = Convert.ToInt32(dr["C036"]);
                item.D3Lost7 = Convert.ToInt32(dr["C037"]);
                item.D3Lost8 = Convert.ToInt32(dr["C038"]);
                item.D3Lost9 = Convert.ToInt32(dr["C039"]);

                item.D2Lost0 = Convert.ToInt32(dr["C020"]);
                item.D2Lost1 = Convert.ToInt32(dr["C021"]);
                item.D2Lost2 = Convert.ToInt32(dr["C022"]);
                item.D2Lost3 = Convert.ToInt32(dr["C023"]);
                item.D2Lost4 = Convert.ToInt32(dr["C024"]);
                item.D2Lost5 = Convert.ToInt32(dr["C025"]);
                item.D2Lost6 = Convert.ToInt32(dr["C026"]);
                item.D2Lost7 = Convert.ToInt32(dr["C027"]);
                item.D2Lost8 = Convert.ToInt32(dr["C028"]);
                item.D2Lost9 = Convert.ToInt32(dr["C029"]);

                item.D1Lost0 = Convert.ToInt32(dr["C010"]);
                item.D1Lost1 = Convert.ToInt32(dr["C011"]);
                item.D1Lost2 = Convert.ToInt32(dr["C012"]);
                item.D1Lost3 = Convert.ToInt32(dr["C013"]);
                item.D1Lost4 = Convert.ToInt32(dr["C014"]);
                item.D1Lost5 = Convert.ToInt32(dr["C015"]);
                item.D1Lost6 = Convert.ToInt32(dr["C016"]);
                item.D1Lost7 = Convert.ToInt32(dr["C017"]);
                item.D1Lost8 = Convert.ToInt32(dr["C018"]);
                item.D1Lost9 = Convert.ToInt32(dr["C019"]);

                mListLostData.Add(item);
            }
        }

        private void InitPositionItems()
        {
            mListPositionItems.Clear();
            PositionItem item = new PositionItem();
            item.Number = 1;
            item.Name = "个位";
            item.Brush = Brushes.Red;
            item.IsShow = Path1Visible;
            mListPositionItems.Add(item);
            item = new PositionItem();
            item.Number = 2;
            item.Name = "十位";
            item.Brush = Brushes.Green;
            item.IsShow = Path2Visible;
            mListPositionItems.Add(item);
            item = new PositionItem();
            item.Number = 3;
            item.Name = "百位";
            item.Brush = Brushes.Blue;
            item.IsShow = Path3Visible;
            mListPositionItems.Add(item);
            item = new PositionItem();
            item.Number = 4;
            item.Name = "千位";
            item.Brush = Brushes.Orange;
            item.IsShow = Path4Visible;
            mListPositionItems.Add(item);
            item = new PositionItem();
            item.Number = 5;
            item.Name = "万位";
            item.Brush = Brushes.Fuchsia;
            item.IsShow = Path5Visible;
            mListPositionItems.Add(item);
        }

        private void InitIssueItems()
        {
            mListIssueItems.Clear();
            if (PageParent == null) { return; }
            var items = PageParent.ListIssueItems;
            if (items == null) { return; }
            foreach (var item in items)
            {
                var data = mListLostData.FirstOrDefault(l => l.Serial == item.Serial);
                if (data != null)
                {
                    item.D1Lost0 = data.D1Lost0;
                    item.D1Lost1 = data.D1Lost1;
                    item.D1Lost2 = data.D1Lost2;
                    item.D1Lost3 = data.D1Lost3;
                    item.D1Lost4 = data.D1Lost4;
                    item.D1Lost5 = data.D1Lost5;
                    item.D1Lost6 = data.D1Lost6;
                    item.D1Lost7 = data.D1Lost7;
                    item.D1Lost8 = data.D1Lost8;
                    item.D1Lost9 = data.D1Lost9;

                    item.D2Lost0 = data.D2Lost0;
                    item.D2Lost1 = data.D2Lost1;
                    item.D2Lost2 = data.D2Lost2;
                    item.D2Lost3 = data.D2Lost3;
                    item.D2Lost4 = data.D2Lost4;
                    item.D2Lost5 = data.D2Lost5;
                    item.D2Lost6 = data.D2Lost6;
                    item.D2Lost7 = data.D2Lost7;
                    item.D2Lost8 = data.D2Lost8;
                    item.D2Lost9 = data.D2Lost9;

                    item.D3Lost0 = data.D3Lost0;
                    item.D3Lost1 = data.D3Lost1;
                    item.D3Lost2 = data.D3Lost2;
                    item.D3Lost3 = data.D3Lost3;
                    item.D3Lost4 = data.D3Lost4;
                    item.D3Lost5 = data.D3Lost5;
                    item.D3Lost6 = data.D3Lost6;
                    item.D3Lost7 = data.D3Lost7;
                    item.D3Lost8 = data.D3Lost8;
                    item.D3Lost9 = data.D3Lost9;

                    item.D4Lost0 = data.D4Lost0;
                    item.D4Lost1 = data.D4Lost1;
                    item.D4Lost2 = data.D4Lost2;
                    item.D4Lost3 = data.D4Lost3;
                    item.D4Lost4 = data.D4Lost4;
                    item.D4Lost5 = data.D4Lost5;
                    item.D4Lost6 = data.D4Lost6;
                    item.D4Lost7 = data.D4Lost7;
                    item.D4Lost8 = data.D4Lost8;
                    item.D4Lost9 = data.D4Lost9;

                    item.D5Lost0 = data.D5Lost0;
                    item.D5Lost1 = data.D5Lost1;
                    item.D5Lost2 = data.D5Lost2;
                    item.D5Lost3 = data.D5Lost3;
                    item.D5Lost4 = data.D5Lost4;
                    item.D5Lost5 = data.D5Lost5;
                    item.D5Lost6 = data.D5Lost6;
                    item.D5Lost7 = data.D5Lost7;
                    item.D5Lost8 = data.D5Lost8;
                    item.D5Lost9 = data.D5Lost9;
                }

                item.D1Width = item.D1 * 20.0 + 20;
                item.D2Width = item.D2 * 20.0 + 20;
                item.D3Width = item.D3 * 20.0 + 20;
                item.D4Width = item.D4 * 20.0 + 20;
                item.D5Width = item.D5 * 20.0 + 20;
                mListIssueItems.Add(item);
            }
        }

        private void InitIssueDateItems()
        {
            mListGroupItems.Clear();
            if (PageParent == null) { return; }
            var listIssuetems = PageParent.ListIssueItems;
            if (listIssuetems == null) { return; }
            var groups = listIssuetems.GroupBy(l => l.Date);
            foreach (var group in groups)
            {
                int date = group.Key;
                IssueDateItem groupItem = new IssueDateItem();
                groupItem.Date = date;
                for (int i = group.Count() - 1; i >= 0; i--)
                {
                    var issueItem = group.ToList()[i];
                    issueItem.D1Height = 120 * 1.0 / 10.0 * issueItem.D1 + 10;
                    issueItem.D2Height = 120 * 1.0 / 10.0 * issueItem.D2 + 10;
                    issueItem.D3Height = 120 * 1.0 / 10.0 * issueItem.D3 + 10;
                    issueItem.D4Height = 120 * 1.0 / 10.0 * issueItem.D4 + 10;
                    issueItem.D5Height = 120 * 1.0 / 10.0 * issueItem.D5 + 10;
                    groupItem.Issues.Add(issueItem);

                    int number = 1;
                    var numberGroup = groupItem.Groups.FirstOrDefault(n => n.Number == number);
                    if (numberGroup == null)
                    {
                        numberGroup = new IssueGroupItem();
                        numberGroup.Number = number;
                        groupItem.Groups.Add(numberGroup);
                    }
                    IssueNumberItem numberItem = new IssueNumberItem();
                    numberItem.Number = number;
                    numberItem.Value = issueItem.D1;
                    numberGroup.Values.Add(numberItem);

                    number = 2;
                    numberGroup = groupItem.Groups.FirstOrDefault(n => n.Number == number);
                    if (numberGroup == null)
                    {
                        numberGroup = new IssueGroupItem();
                        numberGroup.Number = number;
                        groupItem.Groups.Add(numberGroup);
                    }
                    numberItem = new IssueNumberItem();
                    numberItem.Number = number;
                    numberItem.Value = issueItem.D2;
                    numberGroup.Values.Add(numberItem);

                    number = 3;
                    numberGroup = groupItem.Groups.FirstOrDefault(n => n.Number == number);
                    if (numberGroup == null)
                    {
                        numberGroup = new IssueGroupItem();
                        numberGroup.Number = number;
                        groupItem.Groups.Add(numberGroup);
                    }
                    numberItem = new IssueNumberItem();
                    numberItem.Number = number;
                    numberItem.Value = issueItem.D3;
                    numberGroup.Values.Add(numberItem);

                    number = 4;
                    numberGroup = groupItem.Groups.FirstOrDefault(n => n.Number == number);
                    if (numberGroup == null)
                    {
                        numberGroup = new IssueGroupItem();
                        numberGroup.Number = number;
                        groupItem.Groups.Add(numberGroup);
                    }
                    numberItem = new IssueNumberItem();
                    numberItem.Number = number;
                    numberItem.Value = issueItem.D4;
                    numberGroup.Values.Add(numberItem);

                    number = 5;
                    numberGroup = groupItem.Groups.FirstOrDefault(n => n.Number == number);
                    if (numberGroup == null)
                    {
                        numberGroup = new IssueGroupItem();
                        numberGroup.Number = number;
                        groupItem.Groups.Add(numberGroup);
                    }
                    numberItem = new IssueNumberItem();
                    numberItem.Number = number;
                    numberItem.Value = issueItem.D5;
                    numberGroup.Values.Add(numberItem);
                }
                mListGroupItems.Add(groupItem);
            }
        }

        private void InitIssuePaths()
        {
            for (int i = 0; i < mListGroupItems.Count; i++)
            {
                var groupItem = mListGroupItems[i];
                int number = 1;
                var numberGroup = groupItem.Groups.FirstOrDefault(n => n.Number == number);
                if (numberGroup != null)
                {
                    PathGeometry path = OptPathGemotry(numberGroup);
                    numberGroup.Path1 = path;
                }
                number = 2;
                numberGroup = groupItem.Groups.FirstOrDefault(n => n.Number == number);
                if (numberGroup != null)
                {
                    PathGeometry path = OptPathGemotry(numberGroup);
                    numberGroup.Path2 = path;
                }
                number = 3;
                numberGroup = groupItem.Groups.FirstOrDefault(n => n.Number == number);
                if (numberGroup != null)
                {
                    PathGeometry path = OptPathGemotry(numberGroup);
                    numberGroup.Path3 = path;
                }
                number = 4;
                numberGroup = groupItem.Groups.FirstOrDefault(n => n.Number == number);
                if (numberGroup != null)
                {
                    PathGeometry path = OptPathGemotry(numberGroup);
                    numberGroup.Path4 = path;
                }
                number = 5;
                numberGroup = groupItem.Groups.FirstOrDefault(n => n.Number == number);
                if (numberGroup != null)
                {
                    PathGeometry path = OptPathGemotry(numberGroup);
                    numberGroup.Path5 = path;
                }
            }
        }

        private void InitPath()
        {
            double itemWidth = 20;
            double itemHeight = 18;
            var firstItem = mListIssueItems.FirstOrDefault();
            if (firstItem != null)
            {
                PathSegmentCollection segments = new PathSegmentCollection();
                double firstX = firstItem.D1 * itemWidth + itemWidth / 2.0;
                double firstY = itemHeight / 2.0;
                Point first = new Point(firstX, firstY);
                for (int i = 0; i < mListIssueItems.Count; i++)
                {
                    IssueItem item = mListIssueItems[i];
                    double x = item.D1 * itemWidth + itemWidth / 2.0;
                    double y = itemHeight * i + itemHeight / 2.0;
                    Point point = new Point(x, y);
                    segments.Add(new LineSegment { Point = point });
                }
                PathGeometry path = new PathGeometry { Figures = new PathFigureCollection { new PathFigure { StartPoint = first, Segments = segments } } };
                Path1 = path;
            }
            firstItem = mListIssueItems.FirstOrDefault();
            if (firstItem != null)
            {
                PathSegmentCollection segments = new PathSegmentCollection();
                double firstX = firstItem.D2 * itemWidth + itemWidth / 2.0;
                double firstY = itemHeight / 2.0;
                Point first = new Point(firstX, firstY);
                for (int i = 0; i < mListIssueItems.Count; i++)
                {
                    IssueItem item = mListIssueItems[i];
                    double x = item.D2 * itemWidth + itemWidth / 2.0;
                    double y = itemHeight * i + itemHeight / 2.0;
                    Point point = new Point(x, y);
                    segments.Add(new LineSegment { Point = point });
                }
                PathGeometry path = new PathGeometry { Figures = new PathFigureCollection { new PathFigure { StartPoint = first, Segments = segments } } };
                Path2 = path;
            }
            firstItem = mListIssueItems.FirstOrDefault();
            if (firstItem != null)
            {
                PathSegmentCollection segments = new PathSegmentCollection();
                double firstX = firstItem.D3 * itemWidth + itemWidth / 2.0;
                double firstY = itemHeight / 2.0;
                Point first = new Point(firstX, firstY);
                for (int i = 0; i < mListIssueItems.Count; i++)
                {
                    IssueItem item = mListIssueItems[i];
                    double x = item.D3 * itemWidth + itemWidth / 2.0;
                    double y = itemHeight * i + itemHeight / 2.0;
                    Point point = new Point(x, y);
                    segments.Add(new LineSegment { Point = point });
                }
                PathGeometry path = new PathGeometry { Figures = new PathFigureCollection { new PathFigure { StartPoint = first, Segments = segments } } };
                Path3 = path;
            }
            firstItem = mListIssueItems.FirstOrDefault();
            if (firstItem != null)
            {
                PathSegmentCollection segments = new PathSegmentCollection();
                double firstX = firstItem.D4 * itemWidth + itemWidth / 2.0;
                double firstY = itemHeight / 2.0;
                Point first = new Point(firstX, firstY);
                for (int i = 0; i < mListIssueItems.Count; i++)
                {
                    IssueItem item = mListIssueItems[i];
                    double x = item.D4 * itemWidth + itemWidth / 2.0;
                    double y = itemHeight * i + itemHeight / 2.0;
                    Point point = new Point(x, y);
                    segments.Add(new LineSegment { Point = point });
                }
                PathGeometry path = new PathGeometry { Figures = new PathFigureCollection { new PathFigure { StartPoint = first, Segments = segments } } };
                Path4 = path;
            }
            firstItem = mListIssueItems.FirstOrDefault();
            if (firstItem != null)
            {
                PathSegmentCollection segments = new PathSegmentCollection();
                double firstX = firstItem.D5 * itemWidth + itemWidth / 2.0;
                double firstY = itemHeight / 2.0;
                Point first = new Point(firstX, firstY);
                for (int i = 0; i < mListIssueItems.Count; i++)
                {
                    IssueItem item = mListIssueItems[i];
                    double x = item.D5 * itemWidth + itemWidth / 2.0;
                    double y = itemHeight * i + itemHeight / 2.0;
                    Point point = new Point(x, y);
                    segments.Add(new LineSegment { Point = point });
                }
                PathGeometry path = new PathGeometry { Figures = new PathFigureCollection { new PathFigure { StartPoint = first, Segments = segments } } };
                Path5 = path;
            }
        }

        #endregion


        #region Others

        private PathGeometry OptPathGemotry(IssueGroupItem numberGroup)
        {
            double itemWidth = 20;
            double itemHeight = 15;
            var firstItem = numberGroup.Values.FirstOrDefault();
            if (firstItem != null)
            {
                PathSegmentCollection segments = new PathSegmentCollection();
                double firstX = itemWidth / 2.0;
                double firstY = firstItem.Value * 12 + itemHeight / 2.0;
                Point first = new Point(firstX, firstY);
                for (int j = 1; j < numberGroup.Values.Count; j++)
                {
                    var numberItem = numberGroup.Values[j];
                    double x = itemWidth / 2.0 + itemWidth * j;
                    double y = numberItem.Value * 12 + itemHeight / 2.0;
                    Point point = new Point(x, y);
                    segments.Add(new LineSegment { Point = point });
                }
                PathGeometry path = new PathGeometry { Figures = new PathFigureCollection { new PathFigure { StartPoint = first, Segments = segments } } };
                return path;
            }
            return null;
        }

        #endregion


        #region 事件处理

        void TabControlView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PositionClick_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var pItem = mListPositionItems.FirstOrDefault(p => p.Number == 1);
            if (pItem != null)
            {
                Path1Visible = pItem.IsShow;
                Path1Color = pItem.Brush;
            }
            pItem = mListPositionItems.FirstOrDefault(p => p.Number == 2);
            if (pItem != null)
            {
                Path2Visible = pItem.IsShow;
                Path2Color = pItem.Brush;
            }
            pItem = mListPositionItems.FirstOrDefault(p => p.Number == 3);
            if (pItem != null)
            {
                Path3Visible = pItem.IsShow;
                Path3Color = pItem.Brush;
            }
            pItem = mListPositionItems.FirstOrDefault(p => p.Number == 4);
            if (pItem != null)
            {
                Path4Visible = pItem.IsShow;
                Path4Color = pItem.Brush;
            }
            pItem = mListPositionItems.FirstOrDefault(p => p.Number == 5);
            if (pItem != null)
            {
                Path5Visible = pItem.IsShow;
                Path5Color = pItem.Brush;
            }
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

    }
}
