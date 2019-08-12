//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    9c7f11a9-151b-4789-8d07-c773daa37342
//        CLR Version:              4.0.30319.42000
//        Name:                     PopupWindow
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai
//        File Name:                PopupWindow
//
//        Created by Charley at 2019/8/12 14:40:18
//        http://www.netinfo.com 
//
//======================================================================

using System.Windows;

namespace ShiShiCai
{
    /// <summary>
    /// PopupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PopupWindow
    {
        private bool mIsInited;

        public PopupWindow()
        {
            InitializeComponent();

            Loaded += PopupWindow_Loaded;
        }

        void PopupWindow_Loaded(object sender, RoutedEventArgs e)
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
    }
}
