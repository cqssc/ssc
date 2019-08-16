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

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
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


        private readonly ObservableCollection<SumValueItem> mListSumValueItems = new ObservableCollection<SumValueItem>();

        private bool mIsInited;


        public UCLargeSmall()
        {
            InitializeComponent();

            Loaded += UCLargeSmall_Loaded;
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
            InitSumValueItems();
            InitSumViewSize();
        }

        private void InitSumValueItems()
        {
            mListSumValueItems.Clear();
            var pageParent = PageParent;
            if (pageParent == null) { return; }
            var issueItems = pageParent.ListIssueItems;
            if (issueItems == null) { return; }
            var first = issueItems.FirstOrDefault();
            if (first == null) { return; }
            int date = first.Date;
            for (int i = issueItems.Count - 1; i >= 0; i--)
            {
                var issueItem = issueItems[i];
                if (date == issueItem.Date)
                {
                    SumValueItem item = new SumValueItem();
                    item.Serial = issueItem.Serial;
                    item.Number = issueItem.Number;
                    item.Date = issueItem.Date;
                    item.SumValue = issueItem.SumValue;
                    mListSumValueItems.Add(item);
                }
            }
        }

        private void InitSumViewSize()
        {
            int count = mListSumValueItems.Count;
            double width = ListBoxSumView.ActualWidth;
            double height = ListBoxSumView.ActualHeight;
            double itemWidth = width / (count * 1.0);
            for (int i = 0; i < count; i++)
            {
                var item = mListSumValueItems[i];
                item.ItemWidth = itemWidth;
                item.ItemHeight = item.SumValue * (height / (45 * 1.0));
            }
            InitSumViewPath();
        }

        private void InitSumViewPath()
        {
            int count = mListSumValueItems.Count;
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
            for (int i = 0; i < count; i++)
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

        #endregion
       

        #region 事件处理

        void ListBoxSumView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitSumViewSize();
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
