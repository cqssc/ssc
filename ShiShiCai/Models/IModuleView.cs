//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    a6bc9934-ddf2-40f6-8d24-0465445e0aab
//        CLR Version:              4.0.30319.42000
//        Name:                     IModuleView
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Models
//        File Name:                IModuleView
//
//        Created by Charley at 2019/8/11 11:44:39
//        http://www.netinfo.com 
//
//======================================================================

namespace ShiShiCai.Models
{
    public interface IModuleView
    {
        MainWindow PageParent { get; set; }
        void Reload();
    }
}
