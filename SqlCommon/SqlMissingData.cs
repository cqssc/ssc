using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlCommon
{
    public class SqlMissingData
    {
        public static DataTable GetMissingData(string strConnString)
        {
            DataSet dts = SqlHelper.GetDataSet(strConnString, "select top 1800  * from T_004 order by C001 desc");
            if (dts.Tables.Count > 0)
            {
                return dts.Tables[0].Rows.Count > 0 ? dts.Tables[0] : null;
            }
            else
            {
                return null;
            }
        }
    }
}
