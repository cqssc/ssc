//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    8d911d46-843b-43e2-8e2d-d1b39e217bad
//        CLR Version:              4.0.30319.42000
//        Name:                     UCPerformance
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.UserControls
//        File Name:                UCPerformance
//
//        Created by Charley at 2019/8/11 12:10:46
//        http://www.netinfo.com 
//
//======================================================================

using System.Windows;
using ShiShiCai.Models;

namespace ShiShiCai.UserControls
{
    /// <summary>
    /// UCPerformance.xaml 的交互逻辑
    /// </summary>
    public partial class UCPerformance : IModuleView
    {
        public static readonly DependencyProperty PageParentProperty =
            DependencyProperty.Register("PageParent", typeof(MainWindow), typeof(UCPerformance), new PropertyMetadata(default(MainWindow)));

        public MainWindow PageParent
        {
            get { return (MainWindow)GetValue(PageParentProperty); }
            set { SetValue(PageParentProperty, value); }
        }

        private bool mIsInited;

        public UCPerformance()
        {
            InitializeComponent();

            Loaded += UCPerformance_Loaded;
        }

        void UCPerformance_Loaded(object sender, RoutedEventArgs e)
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

        }
    }
}
