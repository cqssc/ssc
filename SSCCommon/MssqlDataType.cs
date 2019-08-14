//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    dcf485aa-9bd6-477a-94e1-1b034afb5d7d
//        CLR Version:              4.0.30319.42000
//        Name:                     MssqlDataType
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Common
//        File Name:                MssqlDataType
//
//        Created by Charley at 2019/8/8 12:17:09
//        http://www.netinfo.com 
//
//======================================================================

namespace ShiShiCai.Common
{
    /// <summary>
    /// Sql Server数据库的数据类型
    /// </summary>
    public enum MssqlDataType
    {
        /// <summary>
        /// 可变长字符型
        /// </summary>
        Varchar,
        /// <summary>
        /// 可变长字符型
        /// </summary>
        NVarchar,
        /// <summary>
        /// 固定大小字符型
        /// </summary>
        Char,
        /// <summary>
        /// 整型
        /// </summary>
        Int,
        /// <summary>
        /// 长整型
        /// </summary>
        Bigint
    }
}
