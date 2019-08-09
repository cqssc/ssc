//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    28973a41-625f-4657-8771-1b51e6eb4791
//        CLR Version:              4.0.30319.42000
//        Name:                     IModuleView
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai
//        File Name:                IModuleView
//
//        Created by Charley at 2019/8/9 10:20:14
//        http://www.netinfo.com 
//
//======================================================================

namespace ShiShiCai
{
    public interface IModuleView
    {
        MainWindow PageParent { get; set; }
        void Reload();
    }
}
