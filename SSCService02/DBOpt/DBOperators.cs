using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CI.CIUtility.Utility;

namespace SSCService02
{
    public class DBOperators
    {
        public static DataSet GetNewDataIn101(long strLastPeriod,string strConnString)
        {
            DataSet ds = null;
            try
            {
                string strSql = string.Empty;
                if (string.IsNullOrEmpty(strLastPeriod.ToString()))
                {
                    strSql = @"SELECT top 1 * FROM [Pocker-V3].[dbo].[T_101_18] order by C001 desc";
                }
                else
                {
                    strSql = string.Format("SELECT * FROM [Pocker-V3].[dbo].[T_101_18]  where C001>{0}", strLastPeriod);
                }
                ds = DBHelp.DbHelperSQL.GetDataSet(strConnString, strSql);
            }
            catch (Exception ex)
            {
                Logger.Error("Get new data in 101 error:" + ex.ToString());
            }
            return ds;
        }

    }
}
