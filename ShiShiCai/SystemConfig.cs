//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    990b9657-b4cf-4271-ba55-11d032d1f3f8
//        CLR Version:              4.0.30319.42000
//        Name:                     SystemConfig
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai
//        File Name:                SystemConfig
//
//        Created by Charley at 2019/8/8 11:52:57
//        http://www.netinfo.com 
//
//======================================================================

using System.Xml.Serialization;


namespace ShiShiCai
{
    [XmlRoot(ElementName = "Config")]
    public class SystemConfig
    {
        public const string FILE_NAME = "config.xml";

        [XmlElement(ElementName = "Database")]
        public DatabaseConfig Database { get; set; }
    }
}
