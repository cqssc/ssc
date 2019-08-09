//======================================================================
//
//        Copyright © 2016 - 2020 NetInfo Technologies Ltd.
//        All rights reserved
//        guid1:                    bf749bd9-5562-4c0d-a899-3daa175092e5
//        CLR Version:              4.0.30319.42000
//        Name:                     DatabaseConfig
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             NetInfo
//        Namespace:                ShiShiCai
//        File Name:                DatabaseConfig
//
//        Created by Charley at 2019/8/8 11:54:43
//        http://www.netinfo.com 
//
//======================================================================

using System.Xml.Serialization;


namespace ShiShiCai
{
    [XmlRoot(ElementName = "Database")]
    public class DatabaseConfig
    {
        [XmlAttribute]
        public int Type { get; set; }
        [XmlAttribute]
        public string Host { get; set; }
        [XmlAttribute]
        public int Port { get; set; }
        [XmlAttribute]
        public string DBName { get; set; }
        [XmlAttribute]
        public string LoginUser { get; set; }
        [XmlAttribute]
        public string LoginPassword { get; set; }

        public string GetConnectionString()
        {
            string strReturn = string.Empty;
            switch (Type)
            {
                case 1:
                    strReturn = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", Host, Port, DBName,
                        LoginUser, LoginPassword);
                    break;
                case 2:
                    strReturn = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", Host,
                        Port, DBName, LoginUser, LoginPassword);
                    break;
                case 3:
                    strReturn =
                        string.Format(
                            "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3}; Password={4}",
                            Host, Port, DBName, LoginUser, LoginPassword);
                    break;
            }
            return strReturn;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}:{2}-{3}-{4}", Type == 1 ? "MYSQL" : Type == 2 ? "MSSQL" : Type == 3 ? "ORCL" : Type.ToString(),
                Host,
                Port,
                DBName,
                LoginUser);
        }
    }
}
