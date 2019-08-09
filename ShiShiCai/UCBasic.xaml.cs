//======================================================================
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
                    groupItem.Children.Add(group.ToList()[i]);
                }
                mListGroupItems.Add(groupItem);
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
            private ObservableCollection<IssueItem> mChildren = new ObservableCollection<IssueItem>();

            public int Date
            {
                get { return mDate; }
                set { mDate = value; OnPropertyChanged("Date"); }
            }

            public ObservableCollection<IssueItem> Children
            {
                get { return mChildren; }
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
