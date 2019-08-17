//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    ada087cb-6c89-4360-a3cc-51cd7cf56c56
//        CLR Version:              4.0.30319.42000
//        Name:                     SscDefines
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai
//        File Name:                SscDefines
//
//        Created by Charley at 2019/8/3 11:17:40
//        http://www.netinfo.com 
//
//======================================================================

namespace ShiShiCai
{
    public class SscDefines
    {

        #region 模块定义

        public const int MODULE_BASIC = 1;
        public const int MODULE_LARGE_SMALL = 2;
        public const int MODULE_HOT = 3;
        public const int MODULE_TREND = 4;

        public const string MODULE_NAME_BASIC = "开奖号码";
        public const string MODULE_NAME_LARGE_SMALL = "大小单双";
        public const string MODULE_NAME_HOT = "热度展示";
        public const string MODULE_NAME_TREND = "趋势展示";

        #endregion

        #region 统计

        public const int CALC_MODE_LAST_LOTTERY = 1;
        public const int CALC_MODE_DATE = 2;

        #endregion


        #region 分段

        public const int SECTION_10 = 1;
        public const int SECTION_15 = 2;
        public const int SECTION_20 = 3;
        public const int SECTION_30 = 4;
        public const int SECTION_DAY = 5;

        public const string SECTION_NAME_10 = "10期";
        public const string SECTION_NAME_15 = "15期";
        public const string SECTION_NAME_20 = "20期";
        public const string SECTION_NAME_30 = "30期";
        public const string SECTION_NAME_DAY = "全天";

        #endregion


    }
}
