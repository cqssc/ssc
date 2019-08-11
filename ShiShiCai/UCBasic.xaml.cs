﻿//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    a7119008-3db0-4286-8d6d-45106eff0d12
//        CLR Version:              4.0.30319.42000
//        Name:                     UCBasic
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai
//        File Name:                UCBasic
//
//        Created by Charley at 2019/8/9 10:07:21
//        http://www.netinfo.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShiShiCai
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

        private bool mIsInited;

        private readonly ObservableCollection<LotteryGroupItem> mListGroupItems =
            new ObservableCollection<LotteryGroupItem>();

        public UCBasic()
        {
            InitializeComponent();

            Loaded += UCBasic_Loaded;
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
            ListBoxLotteryGroup.ItemsSource = mListGroupItems;

            InitLotteryItems();
            InitLotteryPath();
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
