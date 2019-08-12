//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    75eaffd2-d61a-4322-b796-7ef494299bb3
//        CLR Version:              4.0.30319.42000
//        Name:                     UCHistoryQuery
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.UserControls
//        File Name:                UCHistoryQuery
//
//        Created by Charley at 2019/8/12 14:45:06
//        http://www.netinfo.com 
//
//======================================================================

using System;
using System.Windows;

namespace ShiShiCai.UserControls
{
    /// <summary>
    /// UCHistoryQuery.xaml 的交互逻辑
    /// </summary>
    public partial class UCHistoryQuery
    {
        private bool mIsInited;
        public string IssueDate;

        public UCHistoryQuery()
        {
            InitializeComponent();

            Loaded += UCHistoryQuery_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCHistoryQuery_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        private void Init()
        {

        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupWindow;
            if (parent == null) { return; }
            parent.Close();
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupWindow;
            if (parent == null) { return; }
            var date = DatePickerDate.SelectedDate;
            if (date == null) { return; }
            IssueDate = ((DateTime)date).ToString("yyyyMMdd");
            parent.DialogResult = true;
            parent.Close();
        }
    }
}
