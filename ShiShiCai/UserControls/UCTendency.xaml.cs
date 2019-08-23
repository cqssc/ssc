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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        public ObservableCollection<DateItem> DateItems
        {
            get { return mListDateItems; }
        }

        public ObservableCollection<TendencyTypeItem> TypeItems
        {
            get { return mListTypeItems; }
        }

        #endregion


        private readonly List<TendencyItem> mListTendencyData = new List<TendencyItem>(); 
        private readonly ObservableCollection<DateItem> mListDateItems = new ObservableCollection<DateItem>();
        private readonly ObservableCollection<TendencyTypeItem> mListTypeItems = new ObservableCollection<TendencyTypeItem>();

        private bool mIsInited;

        public UCTendency()
        {
            InitializeComponent();

            Loaded += UCTendency_Loaded;
            ComboDate.SelectionChanged += ComboDate_SelectionChanged;
        }

        void UCTendency_Loaded(object sender, RoutedEventArgs e)
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
            InitTypesItems();
            InitDateItems();

            ComboDate.SelectedItem = mListDateItems.FirstOrDefault();
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

        private void InitTypesItems()
        {
            mListTypeItems.Clear();
            TendencyTypeItem item=new TendencyTypeItem();
            item.Number = 1;
            item.Name = "重复";
            item.Color = Brushes.DarkRed;
            item.IsChecked = true;
            mListTypeItems.Add(item);
            item = new TendencyTypeItem();
            item.Number = 2;
            item.Name = "振荡";
            item.Color = Brushes.DarkGreen;
            mListTypeItems.Add(item);
            item = new TendencyTypeItem();
            item.Number = 3;
            item.Name = "递增减";
            item.Color = Brushes.DarkOrange;
            mListTypeItems.Add(item);
            item = new TendencyTypeItem();
            item.Number = 4;
            item.Name = "其他";
            item.Color = Brushes.DarkBlue;
            mListTypeItems.Add(item);
        }


        #region 事件处理

        void ComboDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

    }
}
