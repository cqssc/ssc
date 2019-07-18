using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using SqlCommon;
using System.Data;
using System.Data.Common;
using System.Collections.Concurrent;
using Modles;
using DBHelp;
using System.Reflection;

namespace SSCService03
{
    public  class StatisticsData
    {
        private bool IBoolIsThreadStaticsDataWorking;
        private Thread IThreadStatiticsData;

        #region 全局变量
        private static string ISqlConnect = string.Empty;
        /// <summary>
        /// 101表的最后一期期号
        /// </summary>
        private static long IStrLastPeriod = 0;

        /// <summary>
        /// 当前属于那一年
        /// </summary>
        private static string IStrYY = String.Empty;
        /// <summary>
        /// 统计到那一天
        /// </summary>
        private static DateTime ILastStatisticsDataTime;
        private static PeriodDetail_101 ICurrentPeriod_101;
        private static LostAll_102 ICurrentLostAll_102;
        private static LostAll_102 IPreLostAll_102;
        private static List<SingleAnalysis_106> IListSingleAnalysis_103;
        private static List<SpecialFuture_110> IListSpecialFuture_108_NotComplete;
        #endregion

        public StatisticsData()
        {
            ISqlConnect = ConfigurationManager.AppSettings["SqlServerConnect"] != null ? ConfigurationManager.AppSettings["SqlServerConnect"] : "Data Source=127.0.0.1,1433;Initial Catalog=Pocker;User Id=sa;Password=net,123";
            ICurrentLostAll_102 = new LostAll_102();
            IPreLostAll_102 = new LostAll_102();
            ICurrentPeriod_101 = new PeriodDetail_101();

            IListSingleAnalysis_103 = new List<SingleAnalysis_106>();

            IListSpecialFuture_108_NotComplete = new List<SpecialFuture_110>();
        }        

        private static void AutoDataDo(object o)
        {
            StatisticsData statisticsdata = o as StatisticsData;
             while (statisticsdata.IBoolIsThreadStaticsDataWorking)
             {
                 IStrLastPeriod = ConfigurationManager.AppSettings["LastPeriodforService02"] != null ? long.Parse(ConfigurationManager.AppSettings["LastPeriodforService02"].ToString()) : 0;
                 if (IStrLastPeriod != 0)
                 {
                     ILastStatisticsDataTime = StringToDateTime(IStrLastPeriod.ToString());
                     IStrYY = IStrLastPeriod.ToString().Substring(2, 2);
                     //取这期之后的第一条数据。
                     List<PeriodDetail_101> listPeriodTemp_101 = GetListPeriodDetail_101(ConstDefine.Const_GetNext, IStrLastPeriod);
                     if (listPeriodTemp_101 != null && listPeriodTemp_101.Count > 0)
                     {
                         //取前一期102   
                         ICurrentPeriod_101 = listPeriodTemp_101[0];

                         //取上一次统计到的那期遗失数据
                         IPreLostAll_102 = GetDataLostAll_102(ConstDefine.Const_GetCurrent, IStrLastPeriod);

                         if (IPreLostAll_102 == null)
                         {
                             //置0
                             listPeriodTemp_101.Clear();
                             listPeriodTemp_101 = GetListPeriodDetail_101(ConstDefine.Const_GetPreSpan, ICurrentPeriod_101.LongPeriod_001, 3);
                             InitLostAll_102(ConstDefine.Const_SetZero, listPeriodTemp_101, ref  ICurrentLostAll_102);
                            
                         }
                         else
                         {
                             //取前面6天的数据倒序
                             listPeriodTemp_101.Clear();
                             listPeriodTemp_101 = GetListPeriodDetail_101(ConstDefine.Const_GetPreSpan, ICurrentPeriod_101.LongPeriod_001, 59 * 6);
                             InitLostAll_102(ConstDefine.Const_SetNormal, listPeriodTemp_101, ref  ICurrentLostAll_102);                           
                         }

                         //更新的是已经统计的期号
                         AppConfigOperation.UpdateAppConfig("LastPeriodforService02", ICurrentPeriod_101.LongPeriod_001.ToString());
                         Thread.Sleep(100);

                     }
                     else
                     {
                         //休息
                         Thread.Sleep(1000 * 1);
                     }
                 }
             }
        }

        #region   DoPeriodDetail_101  数据原表

        /// <summary>
        /// 默认取当前期  0如有当前期，则取当前期，没有取最近一期,1为取这期的下一期
        /// </summary>
        /// <param name="PeriodNumber"></param>
        /// <param name="AType"></param>
        /// <returns></returns>
        public  static List<PeriodDetail_101> GetListPeriodDetail_101(int AType, long PeriodNumber, long RecordNumber = -1)
        {
            List<PeriodDetail_101> listPeriod101 = new List<PeriodDetail_101>();
            string StrSelect = string.Empty;

            switch (AType)
            {
                case 1://取这期之后的第一条
                    StrSelect = string.Format("select top 1 * from T_101_{0} where C001>{1}", IStrYY, PeriodNumber);
                    break;
                case 2://取包含这期以及这期之前的59*6条数据
                    StrSelect = string.Format("select top {2} * from T_101_{0} where C001<={1}  order by C001 desc", IStrYY, PeriodNumber, RecordNumber);
                    break;
                default:
                    break;
            }
            PeriodDetail_101 pp = null;

            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSelect);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drNewRow in ds.Tables[0].Rows)
                {
                    pp = new PeriodDetail_101();
                    pp.LongPeriod_001 = long.Parse(drNewRow["C001"].ToString());
                    pp.AwardNumber_002 = drNewRow["C002"].ToString();
                    pp.DateNumber_004 = long.Parse(drNewRow["C004"].ToString());
                    pp.ShortPeriod_005 = int.Parse(drNewRow["C005"].ToString());
                    pp.Wei5_050 = int.Parse(drNewRow["C050"].ToString());
                    pp.Wei4_040 = int.Parse(drNewRow["C040"].ToString());
                    pp.Wei3_030 = int.Parse(drNewRow["C030"].ToString());
                    pp.Wei2_020 = int.Parse(drNewRow["C020"].ToString());
                    pp.Wei1_010 = int.Parse(drNewRow["C010"].ToString());

                    pp.BigOrSmall_007 = int.Parse(drNewRow["C007"].ToString());
                    pp.EvenODD_008 = int.Parse(drNewRow["C008"].ToString());
                    pp.AllSub_009 = int.Parse(drNewRow["C009"].ToString());
                    listPeriod101.Add(pp);
                }
            }
            return listPeriod101;
        }
        #endregion

        #region  DoLostAll_102    遗失表
        /// <summary>
        /// AType -1 置0操作 其它非置0操作
        /// </summary>
        /// <param name="AType"></param>
        /// <param name="ACurrentPeriod_101"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static bool InitLostAll_102(int AType, List<PeriodDetail_101> AListPeriod_101, ref LostAll_102 ALostAll_102)
        {

            bool flag = true;
            PeriodDetail_101 ACurrentPeriod_101 = AListPeriod_101[0];
            ALostAll_102 = new LostAll_102();
            ALostAll_102.LongPeriod_001 = ACurrentPeriod_101.LongPeriod_001;
            ALostAll_102.DateNumber_002 = ACurrentPeriod_101.DateNumber_004;
            ALostAll_102.ShortPeriod_003 = ACurrentPeriod_101.ShortPeriod_005;


            ALostAll_102.PreLost_101 = -1;
            ALostAll_102.PreLost_102 = -1;
            ALostAll_102.PreLost_103 = -1;
            ALostAll_102.PreLost_104 = -1;
            ALostAll_102.PreLost_105 = -1;

            if (AType == -1)
            {
                #region
                ALostAll_102.Lost_010 = 1;
                ALostAll_102.Lost_011 = 1;
                ALostAll_102.Lost_012 = 1;
                ALostAll_102.Lost_013 = 1;
                ALostAll_102.Lost_014 = 1;
                ALostAll_102.Lost_015 = 1;
                ALostAll_102.Lost_016 = 1;
                ALostAll_102.Lost_017 = 1;
                ALostAll_102.Lost_018 = 1;
                ALostAll_102.Lost_019 = 1;
                ALostAll_102.Lost_020 = 1;
                ALostAll_102.Lost_021 = 1;
                ALostAll_102.Lost_022 = 1;
                ALostAll_102.Lost_023 = 1;
                ALostAll_102.Lost_024 = 1;
                ALostAll_102.Lost_025 = 1;
                ALostAll_102.Lost_026 = 1;
                ALostAll_102.Lost_027 = 1;
                ALostAll_102.Lost_028 = 1;
                ALostAll_102.Lost_029 = 1;
                ALostAll_102.Lost_030 = 1;
                ALostAll_102.Lost_031 = 1;
                ALostAll_102.Lost_032 = 1;
                ALostAll_102.Lost_033 = 1;
                ALostAll_102.Lost_034 = 1;
                ALostAll_102.Lost_035 = 1;
                ALostAll_102.Lost_036 = 1;
                ALostAll_102.Lost_037 = 1;
                ALostAll_102.Lost_038 = 1;
                ALostAll_102.Lost_039 = 1;
                ALostAll_102.Lost_040 = 1;
                ALostAll_102.Lost_041 = 1;
                ALostAll_102.Lost_042 = 1;
                ALostAll_102.Lost_043 = 1;
                ALostAll_102.Lost_044 = 1;
                ALostAll_102.Lost_045 = 1;
                ALostAll_102.Lost_046 = 1;
                ALostAll_102.Lost_047 = 1;
                ALostAll_102.Lost_048 = 1;
                ALostAll_102.Lost_049 = 1;
                ALostAll_102.Lost_050 = 1;
                ALostAll_102.Lost_051 = 1;
                ALostAll_102.Lost_052 = 1;
                ALostAll_102.Lost_053 = 1;
                ALostAll_102.Lost_054 = 1;
                ALostAll_102.Lost_055 = 1;
                ALostAll_102.Lost_056 = 1;
                ALostAll_102.Lost_057 = 1;
                ALostAll_102.Lost_058 = 1;
                ALostAll_102.Lost_059 = 1;         


                Type t = typeof(LostAll_102);
                t.GetProperty("Lost_05" + ACurrentPeriod_101.Wei5_050).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_04" + ACurrentPeriod_101.Wei4_040).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_03" + ACurrentPeriod_101.Wei3_030).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_02" + ACurrentPeriod_101.Wei2_020).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_01" + ACurrentPeriod_101.Wei1_010).SetValue(ALostAll_102, 0, null);

                ALostAll_102.PreLost_101 = 1;
                ALostAll_102.PreLost_102 = 1;
                ALostAll_102.PreLost_103 = 1;
                ALostAll_102.PreLost_104 = 1;
                ALostAll_102.PreLost_105 = 1;

                #endregion
            }
            else
            {
                #region  非置0
                ALostAll_102.Lost_010 = IPreLostAll_102.Lost_010 + 1;
                ALostAll_102.Lost_011 = IPreLostAll_102.Lost_011 + 1;
                ALostAll_102.Lost_012 = IPreLostAll_102.Lost_012 + 1;
                ALostAll_102.Lost_013 = IPreLostAll_102.Lost_013 + 1;
                ALostAll_102.Lost_014 = IPreLostAll_102.Lost_014 + 1;
                ALostAll_102.Lost_015 = IPreLostAll_102.Lost_015 + 1;
                ALostAll_102.Lost_016 = IPreLostAll_102.Lost_016 + 1;
                ALostAll_102.Lost_017 = IPreLostAll_102.Lost_017 + 1;
                ALostAll_102.Lost_018 = IPreLostAll_102.Lost_018 + 1;
                ALostAll_102.Lost_019 = IPreLostAll_102.Lost_019 + 1;
                ALostAll_102.Lost_020 = IPreLostAll_102.Lost_020 + 1;
                ALostAll_102.Lost_021 = IPreLostAll_102.Lost_021 + 1;
                ALostAll_102.Lost_022 = IPreLostAll_102.Lost_022 + 1;
                ALostAll_102.Lost_023 = IPreLostAll_102.Lost_023 + 1;
                ALostAll_102.Lost_024 = IPreLostAll_102.Lost_024 + 1;
                ALostAll_102.Lost_025 = IPreLostAll_102.Lost_025 + 1;
                ALostAll_102.Lost_026 = IPreLostAll_102.Lost_026 + 1;
                ALostAll_102.Lost_027 = IPreLostAll_102.Lost_027 + 1;
                ALostAll_102.Lost_028 = IPreLostAll_102.Lost_028 + 1;
                ALostAll_102.Lost_029 = IPreLostAll_102.Lost_029 + 1;
                ALostAll_102.Lost_030 = IPreLostAll_102.Lost_030 + 1;
                ALostAll_102.Lost_031 = IPreLostAll_102.Lost_031 + 1;
                ALostAll_102.Lost_032 = IPreLostAll_102.Lost_032 + 1;
                ALostAll_102.Lost_033 = IPreLostAll_102.Lost_033 + 1;
                ALostAll_102.Lost_034 = IPreLostAll_102.Lost_034 + 1;
                ALostAll_102.Lost_035 = IPreLostAll_102.Lost_035 + 1;
                ALostAll_102.Lost_036 = IPreLostAll_102.Lost_036 + 1;
                ALostAll_102.Lost_037 = IPreLostAll_102.Lost_037 + 1;
                ALostAll_102.Lost_038 = IPreLostAll_102.Lost_038 + 1;
                ALostAll_102.Lost_039 = IPreLostAll_102.Lost_039 + 1;
                ALostAll_102.Lost_040 = IPreLostAll_102.Lost_040 + 1;
                ALostAll_102.Lost_041 = IPreLostAll_102.Lost_041 + 1;
                ALostAll_102.Lost_042 = IPreLostAll_102.Lost_042 + 1;
                ALostAll_102.Lost_043 = IPreLostAll_102.Lost_043 + 1;
                ALostAll_102.Lost_044 = IPreLostAll_102.Lost_044 + 1;
                ALostAll_102.Lost_045 = IPreLostAll_102.Lost_045 + 1;
                ALostAll_102.Lost_046 = IPreLostAll_102.Lost_046 + 1;
                ALostAll_102.Lost_047 = IPreLostAll_102.Lost_047 + 1;
                ALostAll_102.Lost_048 = IPreLostAll_102.Lost_048 + 1;
                ALostAll_102.Lost_049 = IPreLostAll_102.Lost_049 + 1;
                ALostAll_102.Lost_050 = IPreLostAll_102.Lost_050 + 1;
                ALostAll_102.Lost_051 = IPreLostAll_102.Lost_051 + 1;
                ALostAll_102.Lost_052 = IPreLostAll_102.Lost_052 + 1;
                ALostAll_102.Lost_053 = IPreLostAll_102.Lost_053 + 1;
                ALostAll_102.Lost_054 = IPreLostAll_102.Lost_054 + 1;
                ALostAll_102.Lost_055 = IPreLostAll_102.Lost_055 + 1;
                ALostAll_102.Lost_056 = IPreLostAll_102.Lost_056 + 1;
                ALostAll_102.Lost_057 = IPreLostAll_102.Lost_057 + 1;
                ALostAll_102.Lost_058 = IPreLostAll_102.Lost_058 + 1;
                ALostAll_102.Lost_059 = IPreLostAll_102.Lost_059 + 1;
           

                Type t = typeof(LostAll_102);
                t.GetProperty("Lost_05" + ACurrentPeriod_101.Wei5_050).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_04" + ACurrentPeriod_101.Wei4_040).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_03" + ACurrentPeriod_101.Wei3_030).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_02" + ACurrentPeriod_101.Wei2_020).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_01" + ACurrentPeriod_101.Wei1_010).SetValue(ALostAll_102, 0, null);

                ALostAll_102.PreLost_101 = int.Parse(GetObjectPropertyValue(IPreLostAll_102, string.Format("Lost_01{0}", ACurrentPeriod_101.Wei1_010)));
                ALostAll_102.PreLost_102 = int.Parse(GetObjectPropertyValue(IPreLostAll_102, string.Format("Lost_02{0}", ACurrentPeriod_101.Wei2_020)));
                ALostAll_102.PreLost_103 = int.Parse(GetObjectPropertyValue(IPreLostAll_102, string.Format("Lost_03{0}", ACurrentPeriod_101.Wei3_030)));
                ALostAll_102.PreLost_104 = int.Parse(GetObjectPropertyValue(IPreLostAll_102, string.Format("Lost_04{0}", ACurrentPeriod_101.Wei4_040)));
                ALostAll_102.PreLost_105 = int.Parse(GetObjectPropertyValue(IPreLostAll_102, string.Format("Lost_05{0}", ACurrentPeriod_101.Wei5_050)));

                #endregion

            }

            if (UpdateOrADDLostAll_102(ALostAll_102))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        public static bool UpdateOrADDLostAll_102(LostAll_102 AlostAllWei)
        {
            bool flag = true;

            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_102_{1} where C001={0}", AlostAllWei.LongPeriod_001, IStrYY);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}", AlostAllWei.LongPeriod_001)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}", AlostAllWei.LongPeriod_001)).First() : null;

                if (drCurrent != null) //更新
                {
                    drCurrent.BeginEdit();
                    drCurrent["C001"] = AlostAllWei.LongPeriod_001.ToString();
                    drCurrent["C002"] = AlostAllWei.DateNumber_002.ToString();
                    drCurrent["C003"] = AlostAllWei.ShortPeriod_003;
                    drCurrent["C050"] = AlostAllWei.Lost_050;
                    drCurrent["C051"] = AlostAllWei.Lost_051;
                    drCurrent["C052"] = AlostAllWei.Lost_052;
                    drCurrent["C053"] = AlostAllWei.Lost_053;
                    drCurrent["C054"] = AlostAllWei.Lost_054;
                    drCurrent["C055"] = AlostAllWei.Lost_055;
                    drCurrent["C056"] = AlostAllWei.Lost_056;
                    drCurrent["C057"] = AlostAllWei.Lost_057;
                    drCurrent["C058"] = AlostAllWei.Lost_058;
                    drCurrent["C059"] = AlostAllWei.Lost_059;
                    drCurrent["C040"] = AlostAllWei.Lost_040;
                    drCurrent["C041"] = AlostAllWei.Lost_041;
                    drCurrent["C042"] = AlostAllWei.Lost_042;
                    drCurrent["C043"] = AlostAllWei.Lost_043;
                    drCurrent["C044"] = AlostAllWei.Lost_044;
                    drCurrent["C045"] = AlostAllWei.Lost_045;
                    drCurrent["C046"] = AlostAllWei.Lost_046;
                    drCurrent["C047"] = AlostAllWei.Lost_047;
                    drCurrent["C048"] = AlostAllWei.Lost_048;
                    drCurrent["C049"] = AlostAllWei.Lost_049;
                    drCurrent["C030"] = AlostAllWei.Lost_030;
                    drCurrent["C031"] = AlostAllWei.Lost_031;
                    drCurrent["C032"] = AlostAllWei.Lost_032;
                    drCurrent["C033"] = AlostAllWei.Lost_033;
                    drCurrent["C034"] = AlostAllWei.Lost_034;
                    drCurrent["C035"] = AlostAllWei.Lost_035;
                    drCurrent["C036"] = AlostAllWei.Lost_036;
                    drCurrent["C037"] = AlostAllWei.Lost_037;
                    drCurrent["C038"] = AlostAllWei.Lost_038;
                    drCurrent["C039"] = AlostAllWei.Lost_039;
                    drCurrent["C020"] = AlostAllWei.Lost_020;
                    drCurrent["C021"] = AlostAllWei.Lost_021;
                    drCurrent["C022"] = AlostAllWei.Lost_022;
                    drCurrent["C023"] = AlostAllWei.Lost_023;
                    drCurrent["C024"] = AlostAllWei.Lost_024;
                    drCurrent["C025"] = AlostAllWei.Lost_025;
                    drCurrent["C026"] = AlostAllWei.Lost_026;
                    drCurrent["C027"] = AlostAllWei.Lost_027;
                    drCurrent["C028"] = AlostAllWei.Lost_028;
                    drCurrent["C029"] = AlostAllWei.Lost_029;
                    drCurrent["C010"] = AlostAllWei.Lost_010;
                    drCurrent["C011"] = AlostAllWei.Lost_011;
                    drCurrent["C012"] = AlostAllWei.Lost_012;
                    drCurrent["C013"] = AlostAllWei.Lost_013;
                    drCurrent["C014"] = AlostAllWei.Lost_014;
                    drCurrent["C015"] = AlostAllWei.Lost_015;
                    drCurrent["C016"] = AlostAllWei.Lost_016;
                    drCurrent["C017"] = AlostAllWei.Lost_017;
                    drCurrent["C018"] = AlostAllWei.Lost_018;
                    drCurrent["C019"] = AlostAllWei.Lost_019;

                 
                    drCurrent["C101"] = AlostAllWei.PreLost_101;
                    drCurrent["C102"] = AlostAllWei.PreLost_102;
                    drCurrent["C103"] = AlostAllWei.PreLost_103;
                    drCurrent["C104"] = AlostAllWei.PreLost_104;
                    drCurrent["C105"] = AlostAllWei.PreLost_105;


                    drCurrent.EndEdit();
                }
                else //添加新行
                {
                    DataRow drNewRow = objDataSet.Tables[0].NewRow();
                    drNewRow["C001"] = AlostAllWei.LongPeriod_001.ToString();
                    drNewRow["C002"] = AlostAllWei.DateNumber_002.ToString();
                    drNewRow["C003"] = AlostAllWei.ShortPeriod_003;
                    drNewRow["C050"] = AlostAllWei.Lost_050;
                    drNewRow["C051"] = AlostAllWei.Lost_051;
                    drNewRow["C052"] = AlostAllWei.Lost_052;
                    drNewRow["C053"] = AlostAllWei.Lost_053;
                    drNewRow["C054"] = AlostAllWei.Lost_054;
                    drNewRow["C055"] = AlostAllWei.Lost_055;
                    drNewRow["C056"] = AlostAllWei.Lost_056;
                    drNewRow["C057"] = AlostAllWei.Lost_057;
                    drNewRow["C058"] = AlostAllWei.Lost_058;
                    drNewRow["C059"] = AlostAllWei.Lost_059;
                    drNewRow["C040"] = AlostAllWei.Lost_040;
                    drNewRow["C041"] = AlostAllWei.Lost_041;
                    drNewRow["C042"] = AlostAllWei.Lost_042;
                    drNewRow["C043"] = AlostAllWei.Lost_043;
                    drNewRow["C044"] = AlostAllWei.Lost_044;
                    drNewRow["C045"] = AlostAllWei.Lost_045;
                    drNewRow["C046"] = AlostAllWei.Lost_046;
                    drNewRow["C047"] = AlostAllWei.Lost_047;
                    drNewRow["C048"] = AlostAllWei.Lost_048;
                    drNewRow["C049"] = AlostAllWei.Lost_049;
                    drNewRow["C030"] = AlostAllWei.Lost_030;
                    drNewRow["C031"] = AlostAllWei.Lost_031;
                    drNewRow["C032"] = AlostAllWei.Lost_032;
                    drNewRow["C033"] = AlostAllWei.Lost_033;
                    drNewRow["C034"] = AlostAllWei.Lost_034;
                    drNewRow["C035"] = AlostAllWei.Lost_035;
                    drNewRow["C036"] = AlostAllWei.Lost_036;
                    drNewRow["C037"] = AlostAllWei.Lost_037;
                    drNewRow["C038"] = AlostAllWei.Lost_038;
                    drNewRow["C039"] = AlostAllWei.Lost_039;
                    drNewRow["C020"] = AlostAllWei.Lost_020;
                    drNewRow["C021"] = AlostAllWei.Lost_021;
                    drNewRow["C022"] = AlostAllWei.Lost_022;
                    drNewRow["C023"] = AlostAllWei.Lost_023;
                    drNewRow["C024"] = AlostAllWei.Lost_024;
                    drNewRow["C025"] = AlostAllWei.Lost_025;
                    drNewRow["C026"] = AlostAllWei.Lost_026;
                    drNewRow["C027"] = AlostAllWei.Lost_027;
                    drNewRow["C028"] = AlostAllWei.Lost_028;
                    drNewRow["C029"] = AlostAllWei.Lost_029;
                    drNewRow["C010"] = AlostAllWei.Lost_010;
                    drNewRow["C011"] = AlostAllWei.Lost_011;
                    drNewRow["C012"] = AlostAllWei.Lost_012;
                    drNewRow["C013"] = AlostAllWei.Lost_013;
                    drNewRow["C014"] = AlostAllWei.Lost_014;
                    drNewRow["C015"] = AlostAllWei.Lost_015;
                    drNewRow["C016"] = AlostAllWei.Lost_016;
                    drNewRow["C017"] = AlostAllWei.Lost_017;
                    drNewRow["C018"] = AlostAllWei.Lost_018;
                    drNewRow["C019"] = AlostAllWei.Lost_019;

                    drNewRow["C101"] = AlostAllWei.PreLost_101;
                    drNewRow["C102"] = AlostAllWei.PreLost_102;
                    drNewRow["C103"] = AlostAllWei.PreLost_103;
                    drNewRow["C104"] = AlostAllWei.PreLost_104;
                    drNewRow["C105"] = AlostAllWei.PreLost_105;

                    objDataSet.Tables[0].Rows.Add(drNewRow);
                }

                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
                FileLog.WriteInfo("UpdateOrADDLostAll_102() ", "Success :" + AlostAllWei.LongPeriod_001);
            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdateOrADDLostAll_102() ", e.Message);
                return flag = false;
            }
            #endregion
            return flag;
        }

        public static LostAll_102 GetDataLostAll_102(int AType, long PeriodNumber)
        {
            LostAll_102 lostall_102 = null;
            String StrSQL = string.Empty;
            switch (AType)
            {
                case 0:  //得到当前期
                    StrSQL = string.Format("select top 1 * from T_102_{0} where C001={1} order by  C001 desc", IStrYY, PeriodNumber);
                    break;
                case 1:
                    break;
                case 2://得到前一期
                    StrSQL = string.Format("select top 1 * from T_102_{0} where C001<{1} order by  C001 desc", IStrYY, PeriodNumber);
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drNewRow in ds.Tables[0].Rows)
                {
                    lostall_102 = new LostAll_102();
                    lostall_102.LongPeriod_001 = long.Parse(drNewRow["C001"].ToString());
                    lostall_102.DateNumber_002 = long.Parse(drNewRow["C002"].ToString());
                    lostall_102.ShortPeriod_003 = int.Parse(drNewRow["C003"].ToString());
                    lostall_102.Lost_050 = int.Parse(drNewRow["C050"].ToString());
                    lostall_102.Lost_051 = int.Parse(drNewRow["C051"].ToString());
                    lostall_102.Lost_052 = int.Parse(drNewRow["C052"].ToString());
                    lostall_102.Lost_053 = int.Parse(drNewRow["C053"].ToString());
                    lostall_102.Lost_054 = int.Parse(drNewRow["C054"].ToString());
                    lostall_102.Lost_055 = int.Parse(drNewRow["C055"].ToString());
                    lostall_102.Lost_056 = int.Parse(drNewRow["C056"].ToString());
                    lostall_102.Lost_057 = int.Parse(drNewRow["C057"].ToString());
                    lostall_102.Lost_058 = int.Parse(drNewRow["C058"].ToString());
                    lostall_102.Lost_059 = int.Parse(drNewRow["C059"].ToString());
                    lostall_102.Lost_040 = int.Parse(drNewRow["C040"].ToString());
                    lostall_102.Lost_041 = int.Parse(drNewRow["C041"].ToString());
                    lostall_102.Lost_042 = int.Parse(drNewRow["C042"].ToString());
                    lostall_102.Lost_043 = int.Parse(drNewRow["C043"].ToString());
                    lostall_102.Lost_044 = int.Parse(drNewRow["C044"].ToString());
                    lostall_102.Lost_045 = int.Parse(drNewRow["C045"].ToString());
                    lostall_102.Lost_046 = int.Parse(drNewRow["C046"].ToString());
                    lostall_102.Lost_047 = int.Parse(drNewRow["C047"].ToString());
                    lostall_102.Lost_048 = int.Parse(drNewRow["C048"].ToString());
                    lostall_102.Lost_049 = int.Parse(drNewRow["C049"].ToString());
                    lostall_102.Lost_030 = int.Parse(drNewRow["C030"].ToString());
                    lostall_102.Lost_031 = int.Parse(drNewRow["C031"].ToString());
                    lostall_102.Lost_032 = int.Parse(drNewRow["C032"].ToString());
                    lostall_102.Lost_033 = int.Parse(drNewRow["C033"].ToString());
                    lostall_102.Lost_034 = int.Parse(drNewRow["C034"].ToString());
                    lostall_102.Lost_035 = int.Parse(drNewRow["C035"].ToString());
                    lostall_102.Lost_036 = int.Parse(drNewRow["C036"].ToString());
                    lostall_102.Lost_037 = int.Parse(drNewRow["C037"].ToString());
                    lostall_102.Lost_038 = int.Parse(drNewRow["C038"].ToString());
                    lostall_102.Lost_039 = int.Parse(drNewRow["C039"].ToString());
                    lostall_102.Lost_020 = int.Parse(drNewRow["C020"].ToString());
                    lostall_102.Lost_021 = int.Parse(drNewRow["C021"].ToString());
                    lostall_102.Lost_022 = int.Parse(drNewRow["C022"].ToString());
                    lostall_102.Lost_023 = int.Parse(drNewRow["C023"].ToString());
                    lostall_102.Lost_024 = int.Parse(drNewRow["C024"].ToString());
                    lostall_102.Lost_025 = int.Parse(drNewRow["C025"].ToString());
                    lostall_102.Lost_026 = int.Parse(drNewRow["C026"].ToString());
                    lostall_102.Lost_027 = int.Parse(drNewRow["C027"].ToString());
                    lostall_102.Lost_028 = int.Parse(drNewRow["C028"].ToString());
                    lostall_102.Lost_029 = int.Parse(drNewRow["C029"].ToString());
                    lostall_102.Lost_010 = int.Parse(drNewRow["C010"].ToString());
                    lostall_102.Lost_011 = int.Parse(drNewRow["C011"].ToString());
                    lostall_102.Lost_012 = int.Parse(drNewRow["C012"].ToString());
                    lostall_102.Lost_013 = int.Parse(drNewRow["C013"].ToString());
                    lostall_102.Lost_014 = int.Parse(drNewRow["C014"].ToString());
                    lostall_102.Lost_015 = int.Parse(drNewRow["C015"].ToString());
                    lostall_102.Lost_016 = int.Parse(drNewRow["C016"].ToString());
                    lostall_102.Lost_017 = int.Parse(drNewRow["C017"].ToString());
                    lostall_102.Lost_018 = int.Parse(drNewRow["C018"].ToString());
                    lostall_102.Lost_019 = int.Parse(drNewRow["C019"].ToString());

                    lostall_102.PreLost_101 = int.Parse(drNewRow["C101"].ToString());
                    lostall_102.PreLost_102 = int.Parse(drNewRow["C102"].ToString());
                    lostall_102.PreLost_103 = int.Parse(drNewRow["C103"].ToString());
                    lostall_102.PreLost_104 = int.Parse(drNewRow["C104"].ToString());
                    lostall_102.PreLost_105 = int.Parse(drNewRow["C105"].ToString());
                }
            }
            return lostall_102;
        }

        #endregion
 

        #region  公用方法

        /// <summary>
        /// 求某数在AlistInt里面的顺序,最多的为1
        /// </summary>
        /// <param name="AListInt"></param>
        /// <param name="ACurrent"></param>
        /// <returns></returns>
        static int GetOrder(List<int> AListInt, int ACurrent)
        {
            int index = 0;
            AListInt = AListInt.OrderByDescending(s => s).ToList();
            index = AListInt.FindIndex(s => s == ACurrent);

            return index + 1;
        }


        static int GetMatchCount(string str, string constr)
        {
            return System.Text.RegularExpressions.Regex.Matches(str, constr).Count;
        }

        //得到5个数中的最大值
        public static int GetMax(int one, int two, int three, int four, int five)
        {
            int Imax = 0;
            List<int> listInt = new List<int>();
            listInt.Add(one);
            listInt.Add(two);
            listInt.Add(three);
            listInt.Add(four);
            listInt.Add(five);
            Imax = listInt.Max();

            return Imax;
        }


        //得到大于20期以上遗失的数量
        public static int GetBigger20Number(int one, int two, int three, int four, int five)
        {
            int count = 0;
            if (one >= 20) { count++; }
            if (two >= 20) { count++; }
            if (three >= 20) { count++; }
            if (four >= 20) { count++; }
            if (five >= 20) { count++; }
            return count;
        }

        //是否存在大于20期以上遗失
        public static bool GetBiggest20Flag(int one, int two, int three, int four, int five)
        {
            bool flag = false;
            if (one >= 20) { return flag = true; }
            if (two >= 20) { return flag = true; }
            if (three >= 20) { return flag = true; }
            if (four >= 20) { return flag = true; }
            if (five >= 20) { return flag = true; }
            return flag;
        }


        //求序,当前数在这5个中的排序
        public static int QiuXu(int one, int two, int three, int four, int five, int Current)
        {
            int index = 0;
            List<int> listInt = new List<int>();
            listInt.Add(one);
            listInt.Add(two);
            listInt.Add(three);
            listInt.Add(four);
            listInt.Add(five);


            listInt = listInt.OrderByDescending(s => s).ToList();
            index = listInt.FindIndex(s => s == Current);

            return index + 1;
        }


        /// <summary>
        /// 先判断Alist有没有这个数，没有这个数则先加入后再求序
        /// </summary>
        /// <param name="AlistInt"></param>
        /// <param name="Current"></param>
        /// <returns></returns>
        public static int QiuXu1(List<int> AlistInt, int Current)
        {
            int index = 0;
            if (!AlistInt.Contains(Current))
            {
                AlistInt.Add(Current);
            }
            AlistInt = AlistInt.OrderByDescending(s => s).ToList();
            index = AlistInt.FindIndex(s => s == Current);
            return index + 1;
        }


        public static string GetObjectPropertyValue<T>(T t, string propertyname)
        {
            Type type = typeof(T);

            PropertyInfo property = type.GetProperty(propertyname);

            if (property == null) return string.Empty;

            object o = property.GetValue(t, null);

            if (o == null) return string.Empty;

            return o.ToString();
        }

        public bool SetModelValue(string FieldName, string Value, object obj)
        {
            try
            {
                Type Ts = obj.GetType();
                object v = Convert.ChangeType(Value, Ts.GetProperty(FieldName).PropertyType);
                Ts.GetProperty(FieldName).SetValue(obj, v, null);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 将字符串转日期
        /// </summary>
        /// <param name="source">日期字符串</param>
        /// <returns></returns>
        public static DateTime StringToDateTime(string source)
        {
            if (source.Length < 8)
            {
                return DateTime.Parse("2100-1-1 00:00:00");
            }
            DateTime dt;
            string strTime = source.Substring(0, 4) + "-";
            strTime += source.Substring(4, 2) + "-";
            strTime += source.Substring(6, 2) + " ";
            strTime += "00:00:00";

            dt = DateTime.Parse(DateTime.Parse(strTime).ToString("yyyy-MM-dd HH:mm:ss"));
            return dt;
        }



        /// <summary>
        /// 得到N 期以前的期数
        /// </summary>
        /// <param name="CurrentValue"></param>
        /// <param name="ASpanValue"></param>
        /// <returns></returns>
        public static long GetBeforePeriodValue59(long CurrentValue, int ASpanValue)
        {
            long Stop = 0;
            int i = 0;
            int k = 0; //取商
            int m = 0; //取模

            int j = int.Parse(CurrentValue.ToString().Substring(8));

            if ((ASpanValue - j) >= 0)
            {
                i = ASpanValue - j;
                k = i / 59;
                m = i % 59;

                DateTime lastDay = StringToDateTime(CurrentValue.ToString().Substring(0, 8)).AddDays(-(k + 1));
                Stop = long.Parse(lastDay.ToString("yyyyMMdd") + "059");
                CurrentValue = Stop - m;

            }
            else
            {
                CurrentValue = CurrentValue - ASpanValue;
            }
            return CurrentValue;

        }



        /// <summary>
        ///得到 N期以后的期数
        /// </summary>
        /// <param name="CurrentValue"></param>
        /// <param name="ASpanValue"></param>
        /// <returns></returns>
        public static long GetLaterPeriodValue59(long CurrentValue, int ASpanValue)
        {
            long Start = 0;
            int i = 0;
            int k = 0; //取商
            int m = 0; //取模

            int j = int.Parse(CurrentValue.ToString().Substring(8));

            if ((ASpanValue + j) > 59)
            {
                i = ASpanValue + j;
                k = i / 59;
                m = i % 59;
                DateTime lastDay = StringToDateTime(CurrentValue.ToString().Substring(0, 8)).AddDays(k);
                Start = long.Parse(lastDay.ToString("yyyyMMdd") + "000");
                CurrentValue = Start + m;
            }
            else
            {
                CurrentValue = CurrentValue + ASpanValue;
            }
            return CurrentValue;
        }

        #region 注释掉的方法

        ///// <summary>
        ///// 得到N 期以前的期数
        ///// </summary>
        ///// <param name="CurrentValue"></param>
        ///// <param name="ASpanValue"></param>
        ///// <returns></returns>
        //public static long GetStartPeriodValue120(long CurrentValue, int ASpanValue)
        //{
        //    long Stop = 0;
        //    int i = 0;
        //    int k = 0; //取商
        //    int m = 0; //取模

        //    int j = int.Parse(CurrentValue.ToString().Substring(8));

        //    if ((ASpanValue - j) >= 0)
        //    {
        //        i = ASpanValue - j;
        //        k = i / 120;
        //        m = i % 120;

        //        DateTime lastDay = StringToDateTime(CurrentValue.ToString().Substring(0, 8)).AddDays(-(k + 1));
        //        Stop = long.Parse(lastDay.ToString("yyyyMMdd") + "120");
        //        CurrentValue = Stop - m;

        //    }
        //    else
        //    {
        //        CurrentValue = CurrentValue - ASpanValue;
        //    }
        //    return CurrentValue;
        //}

        ///// <summary>
        ///// 得到当前期 以后N期
        ///// </summary>
        ///// <param name="CurrentValue"></param>
        ///// <param name="ASpanValue"></param>
        ///// <returns></returns>
        //public static long GetStopPeriodValue120(long CurrentValue, int ASpanValue)
        //{
        //    long Start = 0;
        //    int i = 0;
        //    int k = 0; //取商
        //    int m = 0; //取模

        //    int j = int.Parse(CurrentValue.ToString().Substring(8));

        //    if ((ASpanValue + j) > 120)
        //    {
        //        i = ASpanValue + j;
        //        k = i / 120;
        //        m = i % 120;
        //        DateTime lastDay = StringToDateTime(CurrentValue.ToString().Substring(0, 8)).AddDays(k);
        //        Start = long.Parse(lastDay.ToString("yyyyMMdd") + "000");
        //        CurrentValue = Start + m;
        //    }
        //    else
        //    {
        //        CurrentValue = CurrentValue + ASpanValue;
        //    }
        //    return CurrentValue;
        //}

        ///// <summary>
        ///// 得到两期间期数
        ///// </summary>
        ///// <param name="CurrentValue"></param>
        ///// <param name="OldValue"></param>
        ///// <returns></returns>
        //public static long GetSpanOfTwoPeriod120(long CurrentValue, long OldValue)
        //{
        //    long Span = 0;
        //    string strCurr = CurrentValue.ToString().Substring(0, 8) + "000";
        //    string strOld = OldValue.ToString().Substring(0, 8) + "000";
        //    string strOldStop = OldValue.ToString().Substring(0, 8) + "120";
        //    if (strCurr.Equals(strOld))
        //    {
        //        Span = CurrentValue - OldValue;
        //    }
        //    else
        //    {
        //        DateTime start = StringToDateTime(strCurr);
        //        DateTime stop = StringToDateTime(strOld);
        //        TimeSpan sp = start.Subtract(stop);
        //        Span = (sp.Days - 1) * 120 + (long.Parse(strOldStop) - OldValue) + (CurrentValue - long.Parse(strCurr));
        //    }
        //    return Span;
        //}
        #endregion



        /// <summary>
        /// 得到两期间期数
        /// </summary>
        /// <param name="CurrentValue"></param>
        /// <param name="OldValue"></param>
        /// <returns></returns>
        public static long GetSpanOfTwoPeriod59(long CurrentValue, long OldValue)
        {
            long Span = 0;
            string strCurr = CurrentValue.ToString().Substring(0, 8) + "000";
            string strOld = OldValue.ToString().Substring(0, 8) + "000";
            string strOldStop = OldValue.ToString().Substring(0, 8) + "59";
            if (strCurr.Equals(strOld))
            {
                Span = CurrentValue - OldValue;
            }
            else
            {
                DateTime start = StringToDateTime(strCurr);
                DateTime stop = StringToDateTime(strOld);
                TimeSpan sp = start.Subtract(stop);
                Span = (sp.Days - 1) * 59 + (long.Parse(strOldStop) - OldValue) + (CurrentValue - long.Parse(strCurr));
            }
            return Span;
        }

        /// 
        /// </summary>
        /// <param name="ANow"></param>
        /// <returns></returns>
        public static int CauTimeSpan59(DateTime ANow)
        {
            int i = 0;
            DateTime StartTime;

            if (ANow >= ANow.Date.AddMinutes(30) && ANow <= ANow.Date.AddMinutes(3 * 60 + 30))
            {
                StartTime = ANow.Date;
                TimeSpan ts1 = ANow.Subtract(StartTime);
                i = (ts1.Hours * 60 + ts1.Minutes - 30) / 20 + 1;
            }
            else if (ANow >= ANow.Date.AddHours(7).AddMinutes(30))
            {
                StartTime = ANow.Date.AddHours(7);
                TimeSpan ts1 = ANow.Subtract(StartTime);
                i = (ts1.Hours * 60 + ts1.Minutes - 30) / 20 + 10;
            }
            return i;
        }

        /// <summary>
        ///  //根据时间得到期数
        /// </summary>
        /// <param name="ANow"></param>
        /// <param name="i">1为0点到2点，10～22点,22点到24点</param>
        /// <returns></returns>
        public static long Time2PeriodNumber(DateTime ANow)
        {
            int i = 0;
            DateTime StartTime;
            long currentPeriod = 0;
            if (ANow < ANow.Date.AddHours(2))
            {
                StartTime = ANow.Date;
                TimeSpan ts1 = ANow.Subtract(StartTime);
                i = (ts1.Hours * 60 + ts1.Minutes) / 5;
            }
            else if (ANow >= ANow.Date.AddHours(10) && ANow <= ANow.Date.AddHours(22))
            {
                StartTime = ANow.Date.AddHours(10);
                TimeSpan ts1 = ANow.Subtract(StartTime);
                i = (ts1.Hours * 60 + ts1.Minutes) / 10 + 24;
            }
            else if (ANow >= ANow.Date.AddHours(22))
            {
                StartTime = ANow.Date.AddHours(22);
                TimeSpan ts1 = ANow.Subtract(StartTime);
                i = (ts1.Hours * 60 + ts1.Minutes) / 5 + 96;
            }
            currentPeriod = long.Parse(ANow.ToString("yyyyMMdd") + i.ToString().PadLeft(3, '0'));

            return currentPeriod;
        }


        /// <summary>
        /// 将字符串转日期或者期数
        /// </summary>
        /// <param name="source">日期字符串</param>
        /// <returns></returns>
        public static DateTime PeriodNumber2DateTime(string source)
        {
            if (source.Length < 8)
            {
                return DateTime.Parse("2100-1-1 00:00:00");
            }
            DateTime dt;
            string strTime = source.Substring(0, 4) + "-";
            strTime += source.Substring(4, 2) + "-";
            strTime += source.Substring(6, 2) + " ";
            strTime += "00:00:00";

            dt = DateTime.Parse(DateTime.Parse(strTime).ToString("yyyy-MM-dd HH:mm:ss"));
            return dt;
        }

        #endregion

        public void StatisticsStartup()
        {
            IBoolIsThreadStaticsDataWorking = false;

            if (IThreadStatiticsData != null)
            {
                IThreadStatiticsData.Abort();
            }

            IThreadStatiticsData = new Thread(new ParameterizedThreadStart(StatisticsData.AutoDataDo));
            IBoolIsThreadStaticsDataWorking = true;
            IThreadStatiticsData.Start(this);
        }

        public void StatisticsStop()
        {
            IBoolIsThreadStaticsDataWorking = false;
            if (IThreadStatiticsData != null)
            {
                IThreadStatiticsData.Abort();
                IThreadStatiticsData = null;
            }
        }
    }
}
