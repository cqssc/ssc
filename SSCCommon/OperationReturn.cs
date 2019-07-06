//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    bd3960bb-6398-4362-9624-c5bd86b88d1d
//        CLR Version:              4.0.30319.42000
//        Name:                     OperationReturn
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai.Common
//        File Name:                OperationReturn
//
//        Created by Charley at 2019/7/6 11:10:44
//        http://www.netinfo.com 
//
//======================================================================

using System;


namespace ShiShiCai.Common
{
    /// <summary>
    /// 操作返回值
    /// </summary>
    public class OperationReturn
    {
        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 返回代码，参考Defines中的定义
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回值，整型
        /// </summary>
        public int IntValue { get; set; }
        /// <summary>
        /// 返回值，数值型
        /// </summary>
        public decimal NumericValue { get; set; }
        /// <summary>
        /// 返回值，文本型
        /// </summary>
        public string StringValue { get; set; }
        /// <summary>
        /// 返回值，使用的时候可通过 as 转换成对应的对象
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 操作异常
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 以字符串形式返回
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strReturn = string.Empty;
            if (Exception != null)
            {
                strReturn = Exception.Message;
            }
            if (string.IsNullOrEmpty(Message))
            {
                return string.Format("{0}-{1}", Code.ToString("0000"), strReturn);
            }
            return string.Format("{0}-{1}", Code.ToString("0000"), Message);
        }
    }
}
