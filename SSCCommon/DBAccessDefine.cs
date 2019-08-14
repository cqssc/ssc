//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    2e0600f5-f759-4493-8409-501113639a36
//        CLR Version:              4.0.30319.42000
//        Name:                     DBAccessDefine
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Common
//        File Name:                DBAccessDefine
//
//        Created by Charley at 2019/8/8 12:15:11
//        http://www.netinfo.com 
//
//======================================================================

namespace ShiShiCai.Common
{
    /// <summary>
    /// 相关常量定义
    /// </summary>
    public class DBAccessDefine
    {

        #region 错误号定义

        /// <summary>
        /// 表不存在
        /// </summary>
        public const int ERR_TABLE_NOT_EXIST = 1001;

        #endregion


        #region MySQL错误号

        internal const int MYSQL_ERR_OBJECT_NOT_EXIST = 208;

        #endregion


        #region MSSQL 错误号

        internal const int MSSQL_ERR_OBJECT_NOT_EXIST = 208;  //表不存在

        #endregion


        #region ORCL 错误号

        internal const int ORCL_ERR_OBJECT_NOT_EXIST = 942;     //表不存在

        #endregion
    }
}
