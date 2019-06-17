using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modles;
using System.Data;

namespace SSCService01
{
    public class OptLostData102
    {
        public static string ISqlConnect
        {
            set;

            get;

        }

        public static long ICurrentPeriodNumber
        {
            set;
            get;

        }


        public static LostAll_102 CurrentLostAll_102;

        public static LostAll_102 PreLostAll_102;


        public OptLostData102()
        {
          
        }

        public  void  RunLostDataStatistics() 
        {



        }

        #region  数据库操作

        public static LostAll_102 GetLostAll_102(long PeriodNumber,int AType=-1)
        {
            LostAll_102 lostall_102 = new LostAll_102();
            String StrSQL = string.Empty;

            switch (AType)
            {
                case -1:
                    StrSQL = string.Format("select top 1 * from T_102 where C001<{0} order by  C001 desc", PeriodNumber);
                    break;
                case 0:
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSQL);



            return lostall_102;
        }



        #endregion



    }
}
