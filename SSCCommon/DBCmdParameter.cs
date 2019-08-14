//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    e6947067-1de6-4512-82e3-ce623f77af33
//        CLR Version:              4.0.30319.42000
//        Name:                     DBCmdParameter
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Common
//        File Name:                DBCmdParameter
//
//        Created by Charley at 2019/8/8 12:16:09
//        http://www.netinfo.com 
//
//======================================================================

namespace ShiShiCai.Common
{
    public class DBCmdParameter
    {
        public string Name { get; set; }
        public int DataType { get; set; }
        public int Direction { get; set; }
        public int Length { get; set; }
        public string ParamValue { get; set; }
    }
}
