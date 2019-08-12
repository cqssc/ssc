//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    62dff62a-4db8-4f10-8259-647f82beaac5
//        CLR Version:              4.0.30319.42000
//        Name:                     UCTrend
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.UserControls
//        File Name:                UCTrend
//
//        Created by Charley at 2019/8/12 16:27:54
//        http://www.netinfo.com 
//
//======================================================================

using System.Windows;
using ShiShiCai.Models;

namespace ShiShiCai.UserControls
{
    /// <summary>
    /// UCTrend.xaml 的交互逻辑
    /// </summary>
    public partial class UCTrend : IModuleView
    {
        public static readonly DependencyProperty PageParentProperty =
            DependencyProperty.Register("PageParent", typeof(MainWindow), typeof(UCTrend), new PropertyMetadata(default(MainWindow)));

        public MainWindow PageParent
        {
            get { return (MainWindow)GetValue(PageParentProperty); }
            set { SetValue(PageParentProperty, value); }
        }

        private bool mIsInited;

        public UCTrend()
        {
            InitializeComponent();

            Loaded += UCTrend_Loaded;
        }

        void UCTrend_Loaded(object sender, RoutedEventArgs e)
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

        public void Reload()
        {

        }
    }
}
