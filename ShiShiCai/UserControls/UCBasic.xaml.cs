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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ShiShiCai.Models;

namespace ShiShiCai.UserControls
{
    /// <summary>
    /// UCBasic.xaml 的交互逻辑
    /// </summary>
    public partial class UCBasic : IModuleView
    {
        public static readonly DependencyProperty PageParentProperty =
            DependencyProperty.Register("PageParent", typeof(MainWindow), typeof(UCBasic), new PropertyMetadata(default(MainWindow)));

        public MainWindow PageParent
        {
            get { return (MainWindow)GetValue(PageParentProperty); }
            set { SetValue(PageParentProperty, value); }
        }

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

        private bool mIsInited;

        private readonly ObservableCollection<LotteryGroupItem> mListGroupItems =
            new ObservableCollection<LotteryGroupItem>();

        private readonly ObservableCollection<PositionItem> mListPositionItems =
            new ObservableCollection<PositionItem>();


        private static readonly RoutedUICommand mPositionClickCommand = new RoutedUICommand();

        public static RoutedUICommand PositionClickCommand
        {
            get { return mPositionClickCommand; }
        }


        public UCBasic()
        {
            InitializeComponent();

            Loaded += UCBasic_Loaded;

            CommandBindings.Add(new CommandBinding(PositionClickCommand,
                PositionClick_Executed, (s, ce) => ce.CanExecute = true));
        }

        void UCBasic_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        public void Reload()
        {
            Init();
        }

        private void Init()
        {
            ListBoxPosition.ItemsSource = mListPositionItems;
            ListBoxLotteryGroup.ItemsSource = mListGroupItems;

            InitPositionItems();
            InitLotteryItems();
            InitLotteryPath();
        }

        private void InitPositionItems()
        {
            mListPositionItems.Clear();
            PositionItem item = new PositionItem();
            item.Number = 1;
            item.Name = "个位";
            item.IsShow = Path1Visible;
            mListPositionItems.Add(item);
            item = new PositionItem();
            item.Number = 2;
            item.Name = "十位";
            item.IsShow = Path2Visible;
            mListPositionItems.Add(item);
            item = new PositionItem();
            item.Number = 3;
            item.Name = "百位";
            item.IsShow = Path3Visible;
            mListPositionItems.Add(item);
            item = new PositionItem();
            item.Number = 4;
            item.Name = "千位";
            item.IsShow = Path4Visible;
            mListPositionItems.Add(item);
            item = new PositionItem();
            item.Number = 5;
            item.Name = "万位";
            item.IsShow = Path5Visible;
            mListPositionItems.Add(item);
        }

        private void InitLotteryItems()
        {
            mListGroupItems.Clear();
            if (PageParent == null) { return; }
            var listIssuetems = PageParent.ListIssueItems;
            if (listIssuetems == null) { return; }
            var groups = listIssuetems.GroupBy(l => l.Date);
            foreach (var group in groups)
            {
                int date = group.Key;
                LotteryGroupItem groupItem = new LotteryGroupItem();
                groupItem.Date = date;
                for (int i = group.Count() - 1; i >= 0; i--)
                {
                    var issueItem = group.ToList()[i];
                    issueItem.D1Height = 120 * 1.0 / 10.0 * issueItem.D1 + 10;
                    issueItem.D2Height = 120 * 1.0 / 10.0 * issueItem.D2 + 10;
                    issueItem.D3Height = 120 * 1.0 / 10.0 * issueItem.D3 + 10;
                    issueItem.D4Height = 120 * 1.0 / 10.0 * issueItem.D4 + 10;
                    issueItem.D5Height = 120 * 1.0 / 10.0 * issueItem.D5 + 10;
                    groupItem.Children.Add(issueItem);

                    int number = 1;
                    var numberGroup = groupItem.NumberGroups.FirstOrDefault(n => n.Number == number);
                    if (numberGroup == null)
                    {
                        numberGroup = new NumberItemGroup();
                        numberGroup.Number = number;
                        groupItem.NumberGroups.Add(numberGroup);
                    }
                    NumberItem numberItem = new NumberItem();
                    numberItem.Number = number;
                    numberItem.Value = issueItem.D1;
                    numberGroup.Values.Add(numberItem);

                    number = 2;
                    numberGroup = groupItem.NumberGroups.FirstOrDefault(n => n.Number == number);
                    if (numberGroup == null)
                    {
                        numberGroup = new NumberItemGroup();
                        numberGroup.Number = number;
                        groupItem.NumberGroups.Add(numberGroup);
                    }
                    numberItem = new NumberItem();
                    numberItem.Number = number;
                    numberItem.Value = issueItem.D2;
                    numberGroup.Values.Add(numberItem);

                    number = 3;
                    numberGroup = groupItem.NumberGroups.FirstOrDefault(n => n.Number == number);
                    if (numberGroup == null)
                    {
                        numberGroup = new NumberItemGroup();
                        numberGroup.Number = number;
                        groupItem.NumberGroups.Add(numberGroup);
                    }
                    numberItem = new NumberItem();
                    numberItem.Number = number;
                    numberItem.Value = issueItem.D3;
                    numberGroup.Values.Add(numberItem);

                    number = 4;
                    numberGroup = groupItem.NumberGroups.FirstOrDefault(n => n.Number == number);
                    if (numberGroup == null)
                    {
                        numberGroup = new NumberItemGroup();
                        numberGroup.Number = number;
                        groupItem.NumberGroups.Add(numberGroup);
                    }
                    numberItem = new NumberItem();
                    numberItem.Number = number;
                    numberItem.Value = issueItem.D4;
                    numberGroup.Values.Add(numberItem);

                    number = 5;
                    numberGroup = groupItem.NumberGroups.FirstOrDefault(n => n.Number == number);
                    if (numberGroup == null)
                    {
                        numberGroup = new NumberItemGroup();
                        numberGroup.Number = number;
                        groupItem.NumberGroups.Add(numberGroup);
                    }
                    numberItem = new NumberItem();
                    numberItem.Number = number;
                    numberItem.Value = issueItem.D5;
                    numberGroup.Values.Add(numberItem);
                }
                mListGroupItems.Add(groupItem);
            }
        }

        private void InitLotteryPath()
        {
            for (int i = 0; i < mListGroupItems.Count; i++)
            {
                var groupItem = mListGroupItems[i];
                int number = 1;
                var numberGroup = groupItem.NumberGroups.FirstOrDefault(n => n.Number == number);
                if (numberGroup != null)
                {
                    PathGeometry path = OptPathGemotry(numberGroup);
                    numberGroup.Path1 = path;
                }
                number = 2;
                numberGroup = groupItem.NumberGroups.FirstOrDefault(n => n.Number == number);
                if (numberGroup != null)
                {
                    PathGeometry path = OptPathGemotry(numberGroup);
                    numberGroup.Path2 = path;
                }
                number = 3;
                numberGroup = groupItem.NumberGroups.FirstOrDefault(n => n.Number == number);
                if (numberGroup != null)
                {
                    PathGeometry path = OptPathGemotry(numberGroup);
                    numberGroup.Path3 = path;
                }
                number = 4;
                numberGroup = groupItem.NumberGroups.FirstOrDefault(n => n.Number == number);
                if (numberGroup != null)
                {
                    PathGeometry path = OptPathGemotry(numberGroup);
                    numberGroup.Path4 = path;
                }
                number = 5;
                numberGroup = groupItem.NumberGroups.FirstOrDefault(n => n.Number == number);
                if (numberGroup != null)
                {
                    PathGeometry path = OptPathGemotry(numberGroup);
                    numberGroup.Path5 = path;
                }
            }
        }

        private PathGeometry OptPathGemotry(NumberItemGroup numberGroup)
        {
            var itemWidth = 20;
            var itemHeight = 12;
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

        private void PositionClick_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var pItem = mListPositionItems.FirstOrDefault(p => p.Number == 1);
            if (pItem != null)
            {
                Path1Visible = pItem.IsShow;
            }
            pItem = mListPositionItems.FirstOrDefault(p => p.Number == 2);
            if (pItem != null)
            {
                Path2Visible = pItem.IsShow;
            }
            pItem = mListPositionItems.FirstOrDefault(p => p.Number == 3);
            if (pItem != null)
            {
                Path3Visible = pItem.IsShow;
            }
            pItem = mListPositionItems.FirstOrDefault(p => p.Number == 4);
            if (pItem != null)
            {
                Path4Visible = pItem.IsShow;
            }
            pItem = mListPositionItems.FirstOrDefault(p => p.Number == 5);
            if (pItem != null)
            {
                Path5Visible = pItem.IsShow;
            }
        }


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


        #region LotteryGroupItem

        private class LotteryGroupItem : INotifyPropertyChanged
        {
            private int mDate;

            private readonly ObservableCollection<IssueItem> mChildren = new ObservableCollection<IssueItem>();

            private readonly ObservableCollection<NumberItemGroup> mNumberGroups =
                new ObservableCollection<NumberItemGroup>();

            public int Date
            {
                get { return mDate; }
                set { mDate = value; OnPropertyChanged("Date"); }
            }

            public ObservableCollection<IssueItem> Children
            {
                get { return mChildren; }
            }

            public ObservableCollection<NumberItemGroup> NumberGroups
            {
                get { return mNumberGroups; }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string property)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        #endregion


        #region NumberItem

        private class NumberItem : INotifyPropertyChanged
        {
            private int mNumber;
            private int mValue;

            public int Number
            {
                get { return mNumber; }
                set { mNumber = value; OnPropertyChanged("Number"); }
            }

            public int Value
            {
                get { return mValue; }
                set { mValue = value; OnPropertyChanged("Value"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string property)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        #endregion


        #region NumberItemGroup

        private class NumberItemGroup : INotifyPropertyChanged
        {
            private int mNumber;
            private PathGeometry mPath1;
            private PathGeometry mPath2;
            private PathGeometry mPath3;
            private PathGeometry mPath4;
            private PathGeometry mPath5;

            private readonly ObservableCollection<NumberItem> mValues = new ObservableCollection<NumberItem>();

            public int Number
            {
                get { return mNumber; }
                set { mNumber = value; OnPropertyChanged("Number"); }
            }

            public PathGeometry Path1
            {
                get { return mPath1; }
                set { mPath1 = value; OnPropertyChanged("Path1"); }
            }

            public PathGeometry Path2
            {
                get { return mPath2; }
                set { mPath2 = value; OnPropertyChanged("Path2"); }
            }

            public PathGeometry Path3
            {
                get { return mPath3; }
                set { mPath3 = value; OnPropertyChanged("Path3"); }
            }

            public PathGeometry Path4
            {
                get { return mPath4; }
                set { mPath4 = value; OnPropertyChanged("Path4"); }
            }

            public PathGeometry Path5
            {
                get { return mPath5; }
                set { mPath5 = value; OnPropertyChanged("Path5"); }
            }

            public ObservableCollection<NumberItem> Values
            {
                get { return mValues; }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string property)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        #endregion

    }
}
