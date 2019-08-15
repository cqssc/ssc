﻿using System;
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
        CancellationTokenSource _taskCancel = new CancellationTokenSource();
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

        private static List<SpecialFuture_110> IListSpecialFuture_108_NotComplete;



        //存储更新字段的SQL语句，在程序最后一次性执行。
        private static List<String> IListStringSQL = new List<string>();
        #endregion

        public StatisticsData()
        {
            ISqlConnect = ConfigurationManager.AppSettings["SqlServerConnect"] != null ? ConfigurationManager.AppSettings["SqlServerConnect"] : "Data Source=127.0.0.1,1433;Initial Catalog=Pocker;User Id=sa;Password=net,123";

            ICurrentPeriod_101 = new PeriodDetail_101();
            ICurrentLostAll_102 = new LostAll_102();
            IPreLostAll_102 = new LostAll_102();

            IListSpecialFuture_108_NotComplete = new List<SpecialFuture_110>();

            IListStringSQL.Clear();
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
                         //当前要统计期
                         ICurrentPeriod_101 = listPeriodTemp_101[0];
                         //取上一次统计到的那期遗失数据
                         IPreLostAll_102 = GetDataLostAll_102(ConstDefine.Const_GetCurrent, IStrLastPeriod);
                         if (IPreLostAll_102 == null)
                         {
                             //置0
                            listPeriodTemp_101.Clear();
                            listPeriodTemp_101 = GetListPeriodDetail_101(ConstDefine.Const_GetPreSpan, ICurrentPeriod_101.LongPeriod_001, 3);

                            Task t1 = Task.Factory.StartNew(delegate { DoLostAll_102(ConstDefine.Const_SetZero, listPeriodTemp_101, ref  ICurrentLostAll_102); });
                            Task t2 = Task.Factory.StartNew(delegate { DoLostTrend_103(ConstDefine.Const_SetZero, listPeriodTemp_101); });
                            Task t3 = Task.Factory.StartNew(delegate { DoContinueBigSmall_111(ConstDefine.Const_SetZero, listPeriodTemp_101); });
                            Task t4 = Task.Factory.StartNew(delegate { DoLostCross_104(ConstDefine.Const_SetZero, listPeriodTemp_101); });
                            Task t5 = Task.Factory.StartNew(delegate { DoLostSingleNumAll_105(ConstDefine.Const_SetZero, listPeriodTemp_101); });
                            Task t6 = Task.Factory.StartNew(delegate { DoHotSingleNum_107(ConstDefine.Const_SetZero, listPeriodTemp_101); });
                            Task.WaitAll(t1, t2, t3, t4, t5, t6);

                            Task t7 = Task.Factory.StartNew(delegate { DoSingleAnalysis_106(ConstDefine.Const_SetZero, listPeriodTemp_101); });

                            t7.Wait();

                            //DoLostAll_102(ConstDefine.Const_SetZero, listPeriodTemp_101, ref  ICurrentLostAll_102);

                            //DoLostTrend_103(ConstDefine.Const_SetZero, listPeriodTemp_101);

                            //DoContinueBigSmall_111(ConstDefine.Const_SetZero, listPeriodTemp_101);

                            //DoLostCross_104(ConstDefine.Const_SetZero, listPeriodTemp_101);

                            //DoLostSingleNumAll_105(ConstDefine.Const_SetZero, listPeriodTemp_101);

                            //DoSingleAnalysis_106(ConstDefine.Const_SetZero, listPeriodTemp_101);

                            //DoHotSingleNum_107(ConstDefine.Const_SetZero, listPeriodTemp_101);


                             //108


                             //109


                             //120

                         }
                         else
                         {
                             //取前面6天的数据倒序
                             listPeriodTemp_101.Clear();
                             listPeriodTemp_101 = GetListPeriodDetail_101(ConstDefine.Const_GetPreSpan, ICurrentPeriod_101.LongPeriod_001, 59 * 6);

                             Task t1 = Task.Factory.StartNew(delegate { DoLostAll_102(ConstDefine.Const_SetNormal, listPeriodTemp_101, ref  ICurrentLostAll_102); });
                             Task t2 = Task.Factory.StartNew(delegate { DoLostTrend_103(ConstDefine.Const_SetNormal, listPeriodTemp_101); });
                             Task t3 = Task.Factory.StartNew(delegate { DoContinueBigSmall_111(ConstDefine.Const_SetNormal, listPeriodTemp_101); });
                             Task t4 = Task.Factory.StartNew(delegate { DoLostCross_104(ConstDefine.Const_SetNormal, listPeriodTemp_101); });
                             Task t5 = Task.Factory.StartNew(delegate { DoLostSingleNumAll_105(ConstDefine.Const_SetNormal, listPeriodTemp_101); });
                             Task t6 = Task.Factory.StartNew(delegate { DoHotSingleNum_107(ConstDefine.Const_SetNormal, listPeriodTemp_101); });
                             Task.WaitAll(t1, t2, t3, t4, t5, t6);

                             Task t7 = Task.Factory.StartNew(delegate { DoSingleAnalysis_106(ConstDefine.Const_SetNormal, listPeriodTemp_101); });

                             t7.Wait();

                             //DoLostAll_102(ConstDefine.Const_SetNormal, listPeriodTemp_101, ref  ICurrentLostAll_102);

                             //DoLostTrend_103(ConstDefine.Const_SetNormal, listPeriodTemp_101);

                             //DoContinueBigSmall_111(ConstDefine.Const_SetNormal, listPeriodTemp_101);

                             //DoLostCross_104(ConstDefine.Const_SetNormal, listPeriodTemp_101);

                             //DoLostSingleNumAll_105(ConstDefine.Const_SetNormal, listPeriodTemp_101);

                             //DoSingleAnalysis_106(ConstDefine.Const_SetNormal, listPeriodTemp_101);

                             //DoHotSingleNum_107(ConstDefine.Const_SetNormal, listPeriodTemp_101);

                         }

                         ExecuteListSQL(IListStringSQL);
                         IListStringSQL.Clear();

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

                    pp.DayInWeek_006 = int.Parse(drNewRow["C006"].ToString());
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
        public static bool DoLostAll_102(int AType, List<PeriodDetail_101> AListPeriod_101, ref LostAll_102 ALostAll_102)
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

            if (AType == ConstDefine.Const_SetZero)
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

        #region  DoLostTrend_103 遗失趋势表

        public static bool DoLostTrend_103(int Atype, List<PeriodDetail_101> AListPeriod_101) 
        {
            bool flag = true;
            List<ContinueTrend_103> listLostTrend_103Temp = new List<ContinueTrend_103>();
            List<ContinueTrend_103> listLostTrend_103Pre =null;
            ContinueTrend_103 lostTrend_103Pre = null;
            if (Atype == ConstDefine.Const_SetNormal) 
            {
                listLostTrend_103Pre = GetDataLostTrend_103(1, AListPeriod_101[0].LongPeriod_001);
            }

            if(AListPeriod_101 !=null && AListPeriod_101.Count>0)
            {
                List<int> listSingleNumValue = new List<int>();
                for (int i = 1; i <= 5; i++) //1位到5位
                {
                    #region
                    int periodCount = 0;
                    listSingleNumValue.Clear();
                    foreach (PeriodDetail_101 p in AListPeriod_101)
                    {
                        periodCount++;
                        listSingleNumValue.Add(int.Parse(GetObjectPropertyValue(p, String.Format("Wei{0}_0{0}0", i))));
                        if (periodCount >= 25)
                        {
                            break;
                        }
                    }
                    ContinueTrend_103 lostTrend_103 = new ContinueTrend_103();
                    lostTrend_103.LongPeriod_001 = AListPeriod_101[0].LongPeriod_001;
                    lostTrend_103.DateNumber_002 = AListPeriod_101[0].DateNumber_004;
                    lostTrend_103.ShortPeriod_003 = AListPeriod_101[0].ShortPeriod_005;
                    lostTrend_103.PositionType_004 = i;

                    if (Atype == ConstDefine.Const_SetZero)
                    {                       
                        lostTrend_103.Repick_005 = 1;
                        lostTrend_103.Swing_006 = 1;
                        lostTrend_103.AddOrSub_007 = 1;
                        lostTrend_103.Other_008 = 0;
                        lostTrend_103.ContinueValue_009 = 1;
                        lostTrend_103.SwingValue_010 = 0;
                    }
                    else if (Atype == ConstDefine.Const_SetNormal)
                    {
                        if (listLostTrend_103Pre != null && listLostTrend_103Pre.Count > 0 && listLostTrend_103Pre.Where(p => p.PositionType_004 == i).Count()>0)
                        {
                            //当前位前一期的数据
                            lostTrend_103Pre = listLostTrend_103Pre.Where(p => p.PositionType_004 == i).First();
                            lostTrend_103.Repick_005 = lostTrend_103Pre.Repick_005+1;
                            lostTrend_103.Swing_006 = lostTrend_103Pre.Swing_006+1;
                            lostTrend_103.AddOrSub_007 = lostTrend_103Pre.AddOrSub_007+1;
                            lostTrend_103.Other_008 = lostTrend_103Pre.Other_008+1;
                            lostTrend_103.ContinueValue_009 = 0;
                            lostTrend_103.SwingValue_010 = 0;
                            if (listSingleNumValue.Count <= 2)
                            {
                                if (listSingleNumValue[0] == listSingleNumValue[1])//重复
                                {
                                    lostTrend_103.Repick_005 = 0;
                                    lostTrend_103.ContinueValue_009 = 1;
                                }
                                else //其它
                                {
                                    lostTrend_103.Other_008 = 0;
                                    lostTrend_103.ContinueValue_009 = 1;
                                    lostTrend_103.SwingValue_010 = Math.Abs(listSingleNumValue[0] - listSingleNumValue[1]);
                                }
                            }
                            else 
                            {
                                int aIntTemp = listSingleNumValue[0] - listSingleNumValue[1];
                                int bIntTemp=(listSingleNumValue[0] - listSingleNumValue[1]) * (listSingleNumValue[1]-listSingleNumValue[2]);
                                int continueCount = 0;

                                if (aIntTemp==0) //重复
                                {

                                    lostTrend_103.Repick_005 = 0;
                                    continueCount = 0;
                                    for (int k = 0; k < listSingleNumValue.Count; k++) 
                                    {
                                        if ((k + 1) < listSingleNumValue.Count && (listSingleNumValue[k] == listSingleNumValue[k + 1]))
                                        {
                                            continueCount++;
                                        }
                                        else 
                                        {
                                            break;
                                        }
                                    }
                                    lostTrend_103.ContinueValue_009 = continueCount;
                                    lostTrend_103.SwingValue_010 = 0;
                                }
                                else
                                {
                                    if (bIntTemp < 0)  //振荡
                                    {
                                        lostTrend_103.Swing_006 = 0;
                                        continueCount = 0;
                                        for (int k = 0; k < listSingleNumValue.Count; k++)
                                        {
                                            if ((k + 2) < listSingleNumValue.Count && (listSingleNumValue[k] - listSingleNumValue[k + 1]) * (listSingleNumValue[k+1] - listSingleNumValue[k + 2]) < 0)
                                            {
                                                continueCount++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        lostTrend_103.ContinueValue_009 = continueCount;
                                        lostTrend_103.SwingValue_010 = Math.Abs(aIntTemp);
                                    }
                                    else if (bIntTemp > 0) //递增减
                                    {
                                        lostTrend_103.AddOrSub_007 = 0;

                                        continueCount = 0;
                                        for (int k = 0; k < listSingleNumValue.Count; k++)
                                        {
                                            if ((k + 2) < listSingleNumValue.Count && (listSingleNumValue[k] - listSingleNumValue[k + 1]) * (listSingleNumValue[k + 1] - listSingleNumValue[k + 2]) > 0)
                                            {
                                                continueCount++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        lostTrend_103.ContinueValue_009 = continueCount;
                                        lostTrend_103.SwingValue_010 = Math.Abs(aIntTemp);
                                    }
                                    else //其它
                                    {
                                        lostTrend_103.Other_008 = 0;
                                        lostTrend_103.ContinueValue_009 = 1;
                                        lostTrend_103.SwingValue_010 = Math.Abs(aIntTemp);
                                    }


                                }
                            }
                        }
                        else //如果找不到全置0
                        {
                            lostTrend_103.Repick_005 = 1;
                            lostTrend_103.Swing_006 = 1;
                            lostTrend_103.AddOrSub_007 = 1;
                            lostTrend_103.Other_008 = 0;
                            lostTrend_103.ContinueValue_009 = 1;
                            lostTrend_103.SwingValue_010 = 0;
                        }    
                    }
                    listLostTrend_103Temp.Add(lostTrend_103);
                    #endregion
                }

                if (UpdataOrAddLostTreand_103(listLostTrend_103Temp))
                {
                    flag = true;
                    FileLog.WriteInfo("DoLostTrend_103()", "succ");
                }
                else 
                {
                    flag = false;
                    FileLog.WriteInfo("DoLostTrend_103()", "fail");
                }
            }
            return flag;
        }

        public static bool UpdataOrAddLostTreand_103(List<ContinueTrend_103> AListLostTrend_103) 
        {
            bool flag = true;
            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_103_{1} where C001={0}", ICurrentPeriod_101.LongPeriod_001, IStrYY);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                foreach (ContinueTrend_103 ss in AListLostTrend_103)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1} ", ss.LongPeriod_001, ss.PositionType_004)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1} ", ss.LongPeriod_001, ss.PositionType_004)).First() : null;

                    if (drCurrent != null) //更新
                    {
                        drCurrent.BeginEdit();
                        drCurrent["C001"] = ss.LongPeriod_001.ToString();
                        drCurrent["C002"] = ss.DateNumber_002.ToString();
                        drCurrent["C003"] = ss.ShortPeriod_003;
                        drCurrent["C004"] = ss.PositionType_004;
                        drCurrent["C005"] = ss.Repick_005;
                        drCurrent["C006"] = ss.Swing_006;
                        drCurrent["C007"] = ss.AddOrSub_007;
                        drCurrent["C008"] = ss.Other_008;
                        drCurrent["C009"] = ss.ContinueValue_009;
                        drCurrent["C010"] = ss.SwingValue_010;
                        drCurrent.EndEdit();
                    }
                    else //添加新行
                    {
                        DataRow drNewRow = objDataSet.Tables[0].NewRow();
                        drNewRow["C001"] = ss.LongPeriod_001.ToString();
                        drNewRow["C002"] = ss.DateNumber_002.ToString();
                        drNewRow["C003"] = ss.ShortPeriod_003;
                        drNewRow["C004"] = ss.PositionType_004;
                        drNewRow["C005"] = ss.Repick_005;
                        drNewRow["C006"] = ss.Swing_006;
                        drNewRow["C007"] = ss.AddOrSub_007;
                        drNewRow["C008"] = ss.Other_008;
                        drNewRow["C009"] = ss.ContinueValue_009;
                        drNewRow["C010"] = ss.SwingValue_010;

                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }
                }
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
                FileLog.WriteInfo("UpdataOrAddLostTreand_103() ", "Success :" + ICurrentPeriod_101.LongPeriod_001);

            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdataOrAddLostTreand_103()  Err ", e.Message);
                return flag = false;
            }
            #endregion
            return flag;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="AType">0:当前期  1：当前期的前一期</param>
        /// <param name="LongPeriodNumber"></param>
        /// <returns></returns>
        public static List< ContinueTrend_103> GetDataLostTrend_103(int AType ,long LongPeriodNumber) 
        {
            List<ContinueTrend_103> ListLostTrend_103 = null;
            String StrSQL = string.Empty;
            switch (AType)
            {
                case 0:  //得到当前期
                    StrSQL = string.Format("select top 5 * from T_103_{0} where C001={1} order by  C001 desc", IStrYY, LongPeriodNumber);
                    break;
                case 1://得到前一期
                    StrSQL = string.Format("select top 5 * from T_103_{0} where C001<{1} order by  C001 desc", IStrYY, LongPeriodNumber);
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                ListLostTrend_103 = new List<ContinueTrend_103>();
                foreach (DataRow drNewRow in ds.Tables[0].Rows)
                {
                    ContinueTrend_103 lostTrend_103 = new ContinueTrend_103();
                    lostTrend_103.LongPeriod_001 = long.Parse(drNewRow["C001"].ToString());
                    lostTrend_103.DateNumber_002 = long.Parse(drNewRow["C002"].ToString());
                    lostTrend_103.ShortPeriod_003 = int.Parse(drNewRow["C003"].ToString());
                    lostTrend_103.PositionType_004 = int.Parse(drNewRow["C004"].ToString());
                    lostTrend_103.Repick_005 = int.Parse(drNewRow["C005"].ToString());
                    lostTrend_103.Swing_006 = int.Parse(drNewRow["C006"].ToString());
                    lostTrend_103.AddOrSub_007 = int.Parse(drNewRow["C007"].ToString());
                    lostTrend_103.Other_008 = int.Parse(drNewRow["C008"].ToString());
                    lostTrend_103.ContinueValue_009 = int.Parse(drNewRow["C009"].ToString());
                    lostTrend_103.SwingValue_010 = int.Parse(drNewRow["C010"].ToString());
                    ListLostTrend_103.Add(lostTrend_103);
                }
            }
            return ListLostTrend_103;
        }

        #endregion

        #region  DoContinueBigSmall_111大小单双持续表

        public static bool DoContinueBigSmall_111(int AType, List<PeriodDetail_101> AListPeriod_101) 
        {
            bool flag = true;

            List<ContinueBigSmall_111> listContinueBigSmall_111Temp = new List<ContinueBigSmall_111>();
            List<ContinueBigSmall_111> listContinueBigSmall_111Pre = null;
            ContinueBigSmall_111 continueBigSmall_111Pre = null;
            if(AType==ConstDefine.Const_SetNormal)
            {
                listContinueBigSmall_111Pre = GetDataContinueBigSmall_111(1, ICurrentPeriod_101.LongPeriod_001);
            }
            if(AListPeriod_101 !=null && AListPeriod_101.Count>0)
            {
                int countbig = 0;
                int countsmall = 0;
                int counteven = 0;
                int countodd = 0;
                for (int i = 1; i <= 6;i++ ) 
                {
                    ContinueBigSmall_111 cbs1 = new ContinueBigSmall_111(); 
                    listContinueBigSmall_111Temp.Add(cbs1);
                    InitContinueBigSmall_111(i,ref cbs1);
                    #region
                    int tempValue = 0;
                    if (i != 6)
                    {
                        tempValue = int.Parse(GetObjectPropertyValue(ICurrentPeriod_101, String.Format("Wei{0}_0{0}0", i)));   
                    }
                    else 
                    {
                        tempValue = ICurrentPeriod_101.AllSub_009;
                    }                                    

                    if (AType == ConstDefine.Const_SetNormal)
                    {
                        #region
                        #endregion
                        if (listContinueBigSmall_111Pre != null && listContinueBigSmall_111Pre.Count > 0 && listContinueBigSmall_111Pre.Where(p => p.PositionType_004 == i).Count() > 0)
                        {
                            continueBigSmall_111Pre = listContinueBigSmall_111Pre.Where(p => p.PositionType_004 == i).First();
                            if (i != 6)
                            {
                                if (tempValue >= 5)
                                {
                                    cbs1.Big_005 = 1; //大出现
                                    countbig++;
                                }
                                else
                                {
                                    cbs1.Small_006 = 1; //小出现
                                    countsmall++;
                                }
                                if (tempValue % 2 == 1)
                                {
                                    cbs1.Even_007 = 1; //单出现
                                    counteven++;
                                }
                                else
                                {
                                    cbs1.Odd_008 = 1; //双出现
                                    countodd++;
                                }

                            }
                            else 
                            {
                                if (tempValue >= 23)
                                {
                                    cbs1.Big_005 = 1; //大出现
                                }
                                else
                                {
                                    cbs1.Small_006 = 1; //小出现
                                }
                                if (tempValue % 2 == 1)
                                {
                                    cbs1.Even_007 = 1; //单出现
                                }
                                else
                                {
                                    cbs1.Odd_008 = 1; //双出现
                                }
                                cbs1.ComposeBig_011 = countbig;
                                cbs1.ComposeSmall_012 = countsmall;
                                cbs1.ComposeEven_013 = counteven;
                                cbs1.ComposeOdd_014 = countodd;
                            }                            


                            if (continueBigSmall_111Pre.Big_005 == cbs1.Big_005)
                            {
                                cbs1.BigSmallContinue_009 = continueBigSmall_111Pre.BigSmallContinue_009 + 1;
                            }
                            else 
                            {
                                cbs1.BigSmallContinue_009 = 1;
                            }

                            if (continueBigSmall_111Pre.Even_007 == cbs1.Even_007)
                            {
                                cbs1.EvenOddContinue_010 = continueBigSmall_111Pre.EvenOddContinue_010 + 1;
                            }
                            else 
                            {
                                cbs1.EvenOddContinue_010 = 1;
                            }


                        }
                        else
                        {
                            #region
                            if (i != 6)
                            {
                                if (tempValue >= 5)
                                {
                                    cbs1.Big_005 = 1; //大出现
                                    countbig++;
                                }
                                else
                                {
                                    cbs1.Small_006 = 1; //小出现
                                    countsmall++;
                                }
                                if (tempValue % 2 == 1)
                                {
                                    cbs1.Even_007 = 1; //单出现
                                    counteven++;
                                }
                                else
                                {
                                    cbs1.Odd_008 = 1; //双出现
                                    countodd++;
                                }
                            }
                            else
                            {
                                if (tempValue >= 23)
                                {
                                    cbs1.Big_005 = 1; //大出现
                                }
                                else
                                {
                                    cbs1.Small_006 = 1; //小出现
                                }
                                if (tempValue % 2 == 1)
                                {
                                    cbs1.Even_007 = 1; //单出现
                                }
                                else
                                {
                                    cbs1.Odd_008 = 1; //双出现
                                }
                                cbs1.ComposeBig_011 = countbig;
                                cbs1.ComposeSmall_012 = countsmall;
                                cbs1.ComposeEven_013 = counteven;
                                cbs1.ComposeOdd_014 = countodd;
                            }

                            
                            #endregion
                          
                        }
                    }
                    else
                    {
                        #region
                        if (i != 6)
                        {
                            if (tempValue >= 5)
                            {
                                cbs1.Big_005 = 1; //大出现
                                countbig++;
                            }
                            else
                            {
                                cbs1.Small_006 = 1; //小出现
                                countsmall++;
                            }
                            if (tempValue % 2 == 1)
                            {
                                cbs1.Even_007 = 1; //单出现
                                counteven++;
                            }
                            else
                            {
                                cbs1.Odd_008 = 1; //双出现
                                countodd++;
                            }
                        }
                        else
                        {
                            if (tempValue >= 23)
                            {
                                cbs1.Big_005 = 1; //大出现
                            }
                            else
                            {
                                cbs1.Small_006 = 1; //小出现
                            }

                            if (tempValue % 2 == 1)
                            {
                                cbs1.Even_007 = 1; //单出现
                            }
                            else
                            {
                                cbs1.Odd_008 = 1; //双出现
                            }
                            cbs1.ComposeBig_011 = countbig;
                            cbs1.ComposeSmall_012 = countsmall;
                            cbs1.ComposeEven_013 = counteven;
                            cbs1.ComposeOdd_014 = countodd;
                        }

                        
                        #endregion
                    }
                    #endregion
                }


            }
           

            if (UpdataOrAddContinueBigSmall_111( listContinueBigSmall_111Temp))
            {
                flag = true;
                FileLog.WriteInfo("DoContinueBigSmall_111()", "succ");
            }
            else
            {
                flag = false;
                FileLog.WriteInfo("DoContinueBigSmall_111()", "fail");
            }         

            return flag;
        }

        public static void InitContinueBigSmall_111(int AWeiType,ref ContinueBigSmall_111 AcontinueBigSmall_111)
        {
            AcontinueBigSmall_111.LongPeriod_001 = ICurrentPeriod_101.LongPeriod_001;
            AcontinueBigSmall_111.DateNumber_002 = ICurrentPeriod_101.DateNumber_004;
            AcontinueBigSmall_111.ShortPeriod_003 = ICurrentPeriod_101.ShortPeriod_005;
            AcontinueBigSmall_111.PositionType_004 = AWeiType;
            AcontinueBigSmall_111.Big_005 = 0;
            AcontinueBigSmall_111.Small_006 = 0;
            AcontinueBigSmall_111.Even_007 = 0;
            AcontinueBigSmall_111.Odd_008 = 0;
            AcontinueBigSmall_111.BigSmallContinue_009 = 1;
            AcontinueBigSmall_111.EvenOddContinue_010 = 1;
            AcontinueBigSmall_111.ComposeBig_011 = 0;
            AcontinueBigSmall_111.ComposeSmall_012 = 0;
            AcontinueBigSmall_111.ComposeEven_013 = 0;
            AcontinueBigSmall_111.ComposeOdd_014 = 0;

        }

        public static bool UpdataOrAddContinueBigSmall_111(List<ContinueBigSmall_111> AListLostTrend_103)
        {
            bool flag = true;
            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_111_{1} where C001={0}", ICurrentPeriod_101.LongPeriod_001, IStrYY);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                foreach (ContinueBigSmall_111 ss in AListLostTrend_103)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1} ", ss.LongPeriod_001, ss.PositionType_004)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  ", ss.LongPeriod_001, ss.PositionType_004)).First() : null;

                    if (drCurrent != null) //更新
                    {
                        drCurrent.BeginEdit();
                        drCurrent["C001"] = ss.LongPeriod_001.ToString();
                        drCurrent["C002"] = ss.DateNumber_002.ToString();
                        drCurrent["C003"] = ss.ShortPeriod_003;
                        drCurrent["C004"] = ss.PositionType_004;
                        drCurrent["C005"] = ss.Big_005;
                        drCurrent["C006"] = ss.Small_006;
                        drCurrent["C007"] = ss.Even_007;
                        drCurrent["C008"] = ss.Odd_008;
                        drCurrent["C009"] = ss.BigSmallContinue_009;
                        drCurrent["C010"] = ss.EvenOddContinue_010;
                        drCurrent["C011"] = ss.ComposeBig_011;
                        drCurrent["C012"] = ss.ComposeSmall_012;
                        drCurrent["C013"] = ss.ComposeEven_013;
                        drCurrent["C014"] = ss.ComposeOdd_014;
                        drCurrent.EndEdit();
                    }
                    else //添加新行
                    {
                        DataRow drNewRow = objDataSet.Tables[0].NewRow();
                        drNewRow["C001"] = ss.LongPeriod_001.ToString();
                        drNewRow["C002"] = ss.DateNumber_002.ToString();
                        drNewRow["C003"] = ss.ShortPeriod_003;
                        drNewRow["C004"] = ss.PositionType_004;
                        drNewRow["C005"] = ss.Big_005;
                        drNewRow["C006"] = ss.Small_006;
                        drNewRow["C007"] = ss.Even_007;
                        drNewRow["C008"] = ss.Odd_008;
                        drNewRow["C009"] = ss.BigSmallContinue_009;
                        drNewRow["C010"] = ss.EvenOddContinue_010;
                        drNewRow["C011"] = ss.ComposeBig_011;
                        drNewRow["C012"] = ss.ComposeSmall_012;
                        drNewRow["C013"] = ss.ComposeEven_013;
                        drNewRow["C014"] = ss.ComposeOdd_014;
                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }
                }
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
                FileLog.WriteInfo("UpdataOrAddContinueBigSmall_111() ", "Success :" + ICurrentPeriod_101.LongPeriod_001);

            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdataOrAddContinueBigSmall_111()  Err ", e.Message);
                return flag = false;
            }
            #endregion
            return flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AType">0:当前期  1：当前期的前一期</param>
        /// <param name="LongPeriodNumber"></param>
        /// <returns></returns>
        public static List<ContinueBigSmall_111> GetDataContinueBigSmall_111(int AType, long LongPeriodNumber)
        {
            List<ContinueBigSmall_111> ListLostTrend_103 = null;
            String StrSQL = string.Empty;
            switch (AType)
            {
                case 0:  //得到当前期
                    StrSQL = string.Format("select top 6 * from T_111_{0} where C001={1} order by  C001 desc", IStrYY, LongPeriodNumber);
                    break;
                case 1://得到前一期
                    StrSQL = string.Format("select top 6 * from T_111_{0} where C001<{1} order by  C001 desc", IStrYY, LongPeriodNumber);
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                ListLostTrend_103 = new List<ContinueBigSmall_111>();
                foreach (DataRow drNewRow in ds.Tables[0].Rows)
                {
                    ContinueBigSmall_111 lostTrend_103 = new ContinueBigSmall_111();
                    lostTrend_103.LongPeriod_001 = long.Parse(drNewRow["C001"].ToString());
                    lostTrend_103.DateNumber_002 = long.Parse(drNewRow["C002"].ToString());
                    lostTrend_103.ShortPeriod_003 = int.Parse(drNewRow["C003"].ToString());
                    lostTrend_103.PositionType_004 = int.Parse(drNewRow["C004"].ToString());
                    lostTrend_103.Big_005 = int.Parse(drNewRow["C005"].ToString());
                    lostTrend_103.Small_006 = int.Parse(drNewRow["C006"].ToString());
                    lostTrend_103.Even_007 = int.Parse(drNewRow["C007"].ToString());
                    lostTrend_103.Odd_008 = int.Parse(drNewRow["C008"].ToString());
                    lostTrend_103.BigSmallContinue_009 = int.Parse(drNewRow["C009"].ToString());
                    lostTrend_103.EvenOddContinue_010 = int.Parse(drNewRow["C010"].ToString());
                    lostTrend_103.ComposeBig_011 = int.Parse(drNewRow["C011"].ToString());
                    lostTrend_103.ComposeSmall_012= int.Parse(drNewRow["C012"].ToString());
                    lostTrend_103.ComposeEven_013= int.Parse(drNewRow["C013"].ToString());
                    lostTrend_103.ComposeOdd_014= int.Parse(drNewRow["C014"].ToString());
                    ListLostTrend_103.Add(lostTrend_103);
                }
            }
            return ListLostTrend_103;
        }
        #endregion

        #region DoLostCross_104 横向遗失表
        public static bool DoLostCross_104(int AType,List<PeriodDetail_101> AListPeriodDetail_101) 
        {
            bool flag = true;
            List<LostCross_104> listLostCrossTemp = new List<LostCross_104>();
            List<LostCross_104> listLostCrossPre = null;
            LostCross_104 lostCross_104Pre = null;
            if(AType== ConstDefine.Const_SetNormal)
            {
                listLostCrossPre = GetDataLostCross_104(1, ICurrentPeriod_101.LongPeriod_001);
            }

            int allRepickNum = 0;
            int allSpanOneNum = 0;
            for (int i = 1; i <= 6;i++ ) //位
            {
                LostCross_104 lostCross1 = new LostCross_104();
                InitLostCross_104(i, 1, ref  lostCross1);
                LostCross_104 lostCross2 = new LostCross_104();
                InitLostCross_104(i, 2, ref  lostCross2);
                listLostCrossTemp.Add(lostCross1);
                listLostCrossTemp.Add(lostCross2);

                if (AType == ConstDefine.Const_SetNormal && AListPeriodDetail_101.Count>0)
                {                    
                        #region

                        if (listLostCrossPre != null && listLostCrossPre.Count > 0)
                        {
                            PeriodDetail_101 p1 = AListPeriodDetail_101[0];
                            PeriodDetail_101 p2 = AListPeriodDetail_101[1];
                            if (AListPeriodDetail_101.Count <= 2) //只有重复可以判断
                            {
                                if (i != 6)
                                {
                                    #region
                                    if (int.Parse(GetObjectPropertyValue(p1, String.Format("Wei{0}_0{0}0", i))) == int.Parse(GetObjectPropertyValue(p2, String.Format("Wei{0}_0{0}0", i))))
                                    {
                                        lostCross1.IsAppear_006 = 1;
                                        lostCross1.LostValue_007 = 0;
                                        lostCross1.PreLostValue_008 = 1;
                                        lostCross1.AppearNumber_009 = 1;

                                        allRepickNum++;
                                    }
                                    else
                                    {
                                        lostCross1.IsAppear_006 = 0;
                                        lostCross1.LostValue_007 = 2;

                                        lostCross2.IsAppear_006 = 0;
                                        lostCross2.LostValue_007 = 2;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region
                                    if (allRepickNum > 0)
                                    {
                                        //更新原表
                                        lostCross1.IsAppear_006 = 1;
                                        lostCross1.LostValue_007 = 0;
                                        lostCross1.PreLostValue_008 = 1;
                                        lostCross1.AppearNumber_009 = allRepickNum;

                                        string ss = string.Format("Update T_101_{0} set C100={1}  where  C001={2} ",IStrYY,allRepickNum,ICurrentLostAll_102.LongPeriod_001);
                                        IListStringSQL.Add(ss);
                                    }
                                    else 
                                    {
                                        lostCross1.IsAppear_006 = 0;
                                        lostCross1.LostValue_007 = 2;
                                        lostCross2.IsAppear_006 = 0;
                                        lostCross2.LostValue_007 = 2;
                                    }

                                    #endregion
                                }   
                            }
                            else //重复和间隔都可以
                            {
                                PeriodDetail_101 p3 = AListPeriodDetail_101[2];
                                if (listLostCrossPre != null && listLostCrossPre.Count > 0 && listLostCrossPre.Where(p => p.PositionType_004 == i).Count() > 0)
                                {
                                    if (i != 6)
                                    {
                                        #region
                                        lostCross_104Pre = listLostCrossPre.Where(p => p.PositionType_004 == i && p.CrossType_005 == 1).First();
                                        if (int.Parse(GetObjectPropertyValue(p1, String.Format("Wei{0}_0{0}0", i))) == int.Parse(GetObjectPropertyValue(p2, String.Format("Wei{0}_0{0}0", i)))) //重复
                                        {
                                            lostCross1.PreLostValue_008 = lostCross_104Pre.LostValue_007;
                                            lostCross1.IsAppear_006 = 1;
                                            lostCross1.LostValue_007 = 0;
                                            lostCross1.AppearNumber_009 = 1;
                                            allRepickNum++;
                                        }
                                        else
                                        {
                                            lostCross1.IsAppear_006 = 0;
                                            lostCross1.LostValue_007 = lostCross_104Pre.LostValue_007 + 1;
                                        }

                                        lostCross_104Pre = listLostCrossPre.Where(p => p.PositionType_004 == i && p.CrossType_005 == 2).First();
                                        if (int.Parse(GetObjectPropertyValue(p1, String.Format("Wei{0}_0{0}0", i))) == int.Parse(GetObjectPropertyValue(p3, String.Format("Wei{0}_0{0}0", i))))//间隔
                                        {
                                            lostCross2.PreLostValue_008 = lostCross_104Pre.LostValue_007;
                                            lostCross2.IsAppear_006 = 1;
                                            lostCross2.LostValue_007 = 0;
                                            lostCross2.AppearNumber_009 = 1;
                                            allSpanOneNum++;
                                        }
                                        else
                                        {
                                            lostCross2.IsAppear_006 = 0;
                                            lostCross2.LostValue_007 = lostCross_104Pre.LostValue_007 + 1;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region
                                        if (allRepickNum > 0 && listLostCrossPre.Where(p => p.PositionType_004 == i && p.CrossType_005 == 1).Count()>0)
                                        {
                                             lostCross_104Pre = listLostCrossPre.Where(p => p.PositionType_004 == i && p.CrossType_005 == 1).First();
                                            //更新原表
                                            lostCross1.PreLostValue_008 = lostCross_104Pre.LostValue_007;
                                            lostCross1.IsAppear_006 = 1;
                                            lostCross1.LostValue_007 = 0;
                                            lostCross1.AppearNumber_009 = allRepickNum;
                                            string ss = string.Format("Update T_101_{0} set C100={1}  where  C001={2} ", IStrYY, allRepickNum, ICurrentLostAll_102.LongPeriod_001);
                                            IListStringSQL.Add(ss);
                                        }
                                        else
                                        {
                                            lostCross1.IsAppear_006 = 0;
                                            lostCross1.LostValue_007 = lostCross_104Pre.LostValue_007+1;
                                        }

                                        if (allSpanOneNum > 0 && listLostCrossPre.Where(p => p.PositionType_004 == i && p.CrossType_005 == 2).Count()>0)
                                        {
                                            lostCross_104Pre = listLostCrossPre.Where(p => p.PositionType_004 == i && p.CrossType_005 == 2).First();
                                            lostCross2.PreLostValue_008 = lostCross_104Pre.LostValue_007;
                                            lostCross2.IsAppear_006 = 1;
                                            lostCross2.LostValue_007 = 0;
                                            lostCross2.AppearNumber_009 = allSpanOneNum;
                                            string ss = string.Format("Update T_101_{0} set C101={1}  where  C001={2} ", IStrYY, allSpanOneNum, ICurrentLostAll_102.LongPeriod_001);
                                            IListStringSQL.Add(ss);
                                        }
                                        else 
                                        {
                                            lostCross2.IsAppear_006 = 0;
                                            lostCross2.LostValue_007 = lostCross_104Pre.LostValue_007 + 1;
                                        }
                                        #endregion
                                    }       
                                }
                            }
                        }

                        #endregion
                   
                }
            }

            if (UpdataOrAddLostCross_104(listLostCrossTemp))
            {
                flag = true;
                FileLog.WriteInfo("DoLostCross_104()", "succ");
            }
            else
            {
                flag = false;
                FileLog.WriteInfo("DoLostCross_104()", "fail");
            }
            return flag;
        }

        public static void InitLostCross_104(int PositionType_004, int CrossType_005, ref LostCross_104 ALostCross) 
        {
            ALostCross.LongPeriod_001 = ICurrentPeriod_101.LongPeriod_001;
            ALostCross.DateNumber_002 = ICurrentPeriod_101.DateNumber_004;
            ALostCross.ShortPeriod_003 = ICurrentPeriod_101.ShortPeriod_005;
            ALostCross.PositionType_004 = PositionType_004;
            ALostCross.CrossType_005 = CrossType_005;
            ALostCross.IsAppear_006 = 0;
            ALostCross.LostValue_007 = 1;
            ALostCross.PreLostValue_008 = -1;
            ALostCross.AppearNumber_009 = 0;
        }

        public static List<LostCross_104> GetDataLostCross_104(int AType, long LongPeriodNumber) 
        {
            List<LostCross_104> ListLostTrend_103 = null;
            String StrSQL = string.Empty;
            switch (AType)
            {
                case 0:  //得到当前期
                    StrSQL = string.Format("select top 12 * from T_104_{0} where C001={1} order by  C001 desc", IStrYY, LongPeriodNumber);
                    break;
                case 1://得到前一期
                    StrSQL = string.Format("select top 12 * from T_104_{0} where C001<{1} order by  C001 desc", IStrYY, LongPeriodNumber);
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                ListLostTrend_103 = new List<LostCross_104>();
                foreach (DataRow drNewRow in ds.Tables[0].Rows)
                {
                    LostCross_104 lostTrend_103 = new LostCross_104();
                    lostTrend_103.LongPeriod_001 = long.Parse(drNewRow["C001"].ToString());
                    lostTrend_103.DateNumber_002 = long.Parse(drNewRow["C002"].ToString());
                    lostTrend_103.ShortPeriod_003 = int.Parse(drNewRow["C003"].ToString());
                    lostTrend_103.PositionType_004 = int.Parse(drNewRow["C004"].ToString());
                    lostTrend_103.CrossType_005 = int.Parse(drNewRow["C005"].ToString());
                    lostTrend_103.IsAppear_006 = int.Parse(drNewRow["C006"].ToString());
                    lostTrend_103.LostValue_007 = int.Parse(drNewRow["C007"].ToString());
                    lostTrend_103.PreLostValue_008 = int.Parse(drNewRow["C008"].ToString());
                    lostTrend_103.AppearNumber_009 = int.Parse(drNewRow["C009"].ToString());
                    ListLostTrend_103.Add(lostTrend_103);
                }
            }
            return ListLostTrend_103;
        }

        public static bool UpdataOrAddLostCross_104(List<LostCross_104> AListLostTrend_103) 
        {
            bool flag = true;

            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_104_{1} where C001={0}", ICurrentPeriod_101.LongPeriod_001, IStrYY);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                foreach (LostCross_104 ss in AListLostTrend_103)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1} AND C005={2} ", ss.LongPeriod_001, ss.PositionType_004, ss.CrossType_005)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  AND C005={2} ", ss.LongPeriod_001, ss.PositionType_004, ss.CrossType_005)).First() : null;

                    if (drCurrent != null) //更新
                    {
                        drCurrent.BeginEdit();
                        drCurrent["C001"] = ss.LongPeriod_001.ToString();
                        drCurrent["C002"] = ss.DateNumber_002.ToString();
                        drCurrent["C003"] = ss.ShortPeriod_003;
                        drCurrent["C004"] = ss.PositionType_004;
                        drCurrent["C005"] = ss.CrossType_005;
                        drCurrent["C006"] = ss.IsAppear_006;
                        drCurrent["C007"] = ss.LostValue_007;
                        drCurrent["C008"] = ss.PreLostValue_008;
                        drCurrent["C009"] = ss.AppearNumber_009;
                        drCurrent.EndEdit();
                    }
                    else //添加新行
                    {
                        DataRow drNewRow = objDataSet.Tables[0].NewRow();
                        drNewRow["C001"] = ss.LongPeriod_001.ToString();
                        drNewRow["C002"] = ss.DateNumber_002.ToString();
                        drNewRow["C003"] = ss.ShortPeriod_003;
                        drNewRow["C004"] = ss.PositionType_004;
                        drNewRow["C005"] = ss.CrossType_005;
                        drNewRow["C006"] = ss.IsAppear_006;
                        drNewRow["C007"] = ss.LostValue_007;
                        drNewRow["C008"] = ss.PreLostValue_008;
                        drNewRow["C009"] = ss.AppearNumber_009;
                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }
                }
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
                FileLog.WriteInfo("UpdataOrAddLostCross_104() ", "Success :" + ICurrentPeriod_101.LongPeriod_001);

            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdataOrAddLostCross_104()  Err ", e.Message);
                return flag = false;
            }
            #endregion
            return flag;
        }

        #endregion


        #region  DoLostSingleNumAll_105 遗失数字全部
        public static bool DoLostSingleNumAll_105(int AType, List<PeriodDetail_101> AListPeriod_101) 
        {
            bool flag = true;
            List<LostSingleNumAll_105> listLostSingleNumAll_105Temp = new List<LostSingleNumAll_105>();
            List<LostSingleNumAll_105> listLostSingleNumAll_105Pre = null;
            LostSingleNumAll_105 lostSingleNumPre = null;
            if (AType == ConstDefine.Const_SetNormal)
            {
                listLostSingleNumAll_105Pre = GetDataLostSingleNumAll_105(1, ICurrentPeriod_101.LongPeriod_001);
            }
            for (int i = 0; i <= 9;i++ )
            {
                LostSingleNumAll_105 lostSingleNumAll_105 = new LostSingleNumAll_105();
                InitLostSingleNumAll_105(i,ref lostSingleNumAll_105);
                listLostSingleNumAll_105Temp.Add(lostSingleNumAll_105);
                int count = 0;
                count = GetMatchCount(AListPeriod_101[0].AwardNumber_002, i.ToString());
                if (count > 0)
                {
                    lostSingleNumAll_105.IsAppear_005 = 1;
                    lostSingleNumAll_105.AppearCount_007 = count;
                    lostSingleNumAll_105.LostValue_006 = 0;
                    if (AType == ConstDefine.Const_SetNormal)
                    {
                        if (listLostSingleNumAll_105Pre != null && listLostSingleNumAll_105Pre.Count > 0 && listLostSingleNumAll_105Pre.Where(p => p.SingleNum_004 == i).Count() > 0)
                        {
                            lostSingleNumPre = listLostSingleNumAll_105Pre.Where(p => p.SingleNum_004 == i).First();
                            lostSingleNumAll_105.PreLostValue_008 = lostSingleNumPre.LostValue_006;
                        }
                        else
                        {
                            lostSingleNumAll_105.PreLostValue_008 = 1;
                        }
                    }
                    else
                    {
                        lostSingleNumAll_105.PreLostValue_008 = 1;
                    }
                }
                else 
                {
                    lostSingleNumAll_105.IsAppear_005 = 0;
                    lostSingleNumAll_105.AppearCount_007 = 0;
                    if (AType == ConstDefine.Const_SetNormal)
                    {
                        if (listLostSingleNumAll_105Pre != null && listLostSingleNumAll_105Pre.Count > 0 && listLostSingleNumAll_105Pre.Where(p => p.SingleNum_004 == i).Count() > 0)
                        {
                            lostSingleNumPre = listLostSingleNumAll_105Pre.Where(p => p.SingleNum_004 == i).First();
                            lostSingleNumAll_105.LostValue_006 = lostSingleNumPre.LostValue_006 + 1;
                        }
                        else
                        {
                            lostSingleNumAll_105.LostValue_006 = 1;
                        }
                    }

                }
               
            }

            if (UpdateOrAddLostSingleNumAll_105(listLostSingleNumAll_105Temp))
            {
                flag = true;
            }
            else 
            {
                flag = false;
            }

            return flag;
        }

        public static void InitLostSingleNumAll_105(int SingleNum_004, ref  LostSingleNumAll_105 lostSingleNumAll_105) 
        {
            lostSingleNumAll_105.LongPeriod_001 = ICurrentPeriod_101.LongPeriod_001;
            lostSingleNumAll_105.DateNumber_002 = ICurrentPeriod_101.DateNumber_004;
            lostSingleNumAll_105.ShortPeriod_003 = ICurrentPeriod_101.ShortPeriod_005;
            lostSingleNumAll_105.SingleNum_004 = SingleNum_004;
            lostSingleNumAll_105.IsAppear_005 = 0;
            lostSingleNumAll_105.LostValue_006 = 1;
            lostSingleNumAll_105.AppearCount_007 = 0;
            lostSingleNumAll_105.PreLostValue_008 = -1;
        }

        public static bool UpdateOrAddLostSingleNumAll_105(List<LostSingleNumAll_105> listLostSingleNumAll_105)
        {
            bool flag = true;

            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_105_{1} where C001={0}", ICurrentPeriod_101.LongPeriod_001, IStrYY);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                foreach (LostSingleNumAll_105 ss in listLostSingleNumAll_105)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  ", ss.LongPeriod_001, ss.SingleNum_004)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  ", ss.LongPeriod_001, ss.SingleNum_004)).First() : null;

                    if (drCurrent != null) //更新
                    {
                        drCurrent.BeginEdit();
                        drCurrent["C001"] = ss.LongPeriod_001.ToString();
                        drCurrent["C002"] = ss.DateNumber_002.ToString();
                        drCurrent["C003"] = ss.ShortPeriod_003;
                        drCurrent["C004"] = ss.SingleNum_004;
                        drCurrent["C005"] = ss.IsAppear_005;
                        drCurrent["C006"] = ss.LostValue_006;
                        drCurrent["C007"] = ss.AppearCount_007;
                        drCurrent["C008"] = ss.PreLostValue_008;
                        drCurrent.EndEdit();
                    }
                    else //添加新行
                    {
                        DataRow drNewRow = objDataSet.Tables[0].NewRow();
                        drNewRow["C001"] = ss.LongPeriod_001.ToString();
                        drNewRow["C002"] = ss.DateNumber_002.ToString();
                        drNewRow["C003"] = ss.ShortPeriod_003;
                        drNewRow["C004"] = ss.SingleNum_004;
                        drNewRow["C005"] = ss.IsAppear_005;
                        drNewRow["C006"] = ss.LostValue_006;
                        drNewRow["C007"] = ss.AppearCount_007;
                        drNewRow["C008"] = ss.PreLostValue_008;
                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }
                }
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
                FileLog.WriteInfo("UpdateOrAddLostSingleNumAll_105() ", "Success :" + ICurrentPeriod_101.LongPeriod_001);

            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdateOrAddLostSingleNumAll_105()  Err ", e.Message);
                return flag = false;
            }
            #endregion
            return flag;
        
        }

        public static List<LostSingleNumAll_105> GetDataLostSingleNumAll_105(int AType, long LongPeriodNumber)
        {
            List<LostSingleNumAll_105> ListLostTrend_103 = null;
            String StrSQL = string.Empty;
            switch (AType)
            {
                case 0:  //得到当前期
                    StrSQL = string.Format("select top 10 * from T_111_{0} where C001={1} order by  C001 desc", IStrYY, LongPeriodNumber);
                    break;
                case 1://得到前一期
                    StrSQL = string.Format("select top 10 * from T_111_{0} where C001<{1} order by  C001 desc", IStrYY, LongPeriodNumber);
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                ListLostTrend_103 = new List<LostSingleNumAll_105>();
                foreach (DataRow drNewRow in ds.Tables[0].Rows)
                {
                    LostSingleNumAll_105 lostTrend_103 = new LostSingleNumAll_105();
                    lostTrend_103.LongPeriod_001 = long.Parse(drNewRow["C001"].ToString());
                    lostTrend_103.DateNumber_002 = long.Parse(drNewRow["C002"].ToString());
                    lostTrend_103.ShortPeriod_003 = int.Parse(drNewRow["C003"].ToString());
                    lostTrend_103.SingleNum_004 = int.Parse(drNewRow["C004"].ToString());
                    lostTrend_103.IsAppear_005 = int.Parse(drNewRow["C005"].ToString());
                    lostTrend_103.LostValue_006 = int.Parse(drNewRow["C006"].ToString());
                    lostTrend_103.AppearCount_007 = int.Parse(drNewRow["C007"].ToString());
                    lostTrend_103.PreLostValue_008 = int.Parse(drNewRow["C008"].ToString());
                    ListLostTrend_103.Add(lostTrend_103);
                }
            }
            return ListLostTrend_103;
        }
        #endregion

        #region  DoSingleAnalysis_106  单位出现数字分析
        /// <summary>
        /// ConstDefine.Const_SetZero 
        /// ConstDefine.Const_SetNormal
        /// </summary>
        /// <param name="AType"></param>
        /// <param name="AListPeriod_101"></param>
        /// <returns></returns>
        public static bool DoSingleAnalysis_106(int AType, List<PeriodDetail_101> AListPeriod_101) 
        {
            bool flag = true;

            //保存前一期所有遗失次数 用于比较全位排序
            List<int> listLostValueAllTemp = new List<int>();

            //保存本位单双上的遗失次数 //用于比较本位排序
            List<int> listLostValueBenTemp = new List<int>();
                                  
            List<SingleAnalysis_106> ListSingleAnalysis_106Temp = new List<SingleAnalysis_106>();

            //保存0~9的数量
            List<int> listAllNumCount = new List<int>();
            //保存临时的PeriodDetail_101
            List<PeriodDetail_101> listPeriodDetailTemp = new List<PeriodDetail_101>();

            if (AType !=ConstDefine. Const_SetZero) 
            {
                for (int i = 1; i <= 5; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        listLostValueAllTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}{1}", i, j))));
                    }
                }
            }


            for (int i = 1; i <= 5;i++ )
            {
                listLostValueBenTemp = new List<int>();
                SingleAnalysis_106 singTemp = new SingleAnalysis_106();
                ListSingleAnalysis_106Temp.Add(singTemp);
                singTemp.LongPeriod_001 = ICurrentPeriod_101.LongPeriod_001;
                singTemp.DateNumber_002 = ICurrentPeriod_101.DateNumber_004;
                singTemp.ShortPeriod_003 = ICurrentPeriod_101.ShortPeriod_005;
                singTemp.PositionType_004 = i;
                singTemp.PositionVale_005 = int.Parse(GetObjectPropertyValue(ICurrentPeriod_101, String.Format("Wei{0}_0{0}0", i)));
                if (singTemp.PositionVale_005 >= 5)
                {
                    singTemp.BigOrSmall_006 = 1;
                }
                else
                {
                    singTemp.BigOrSmall_006 = 2;
                }

                int danshuangAdd = 0;
                if (singTemp.PositionVale_005 % 2 == 1)
                {
                    danshuangAdd = 0;
                    singTemp.EvenODD_007 = 1;
                    if (IPreLostAll_102 != null)
                    {
                        listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}1", i))));
                        listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}3", i))));
                        listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}5", i))));
                        listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}7", i))));
                        listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}9", i))));
                    }

                }
                else
                {
                    danshuangAdd = 10;
                    singTemp.EvenODD_007 = 2;
                    if (IPreLostAll_102 != null)
                    {
                        listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}0", i))));
                        listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}2", i))));
                        listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}4", i))));
                        listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}6", i))));
                        listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}8", i))));
                    }
                }


                singTemp.LostValue_008 = int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("PreLost_10{0}", i)));
                singTemp.LostEvenODDOrderNum_009 = 0;
                singTemp.LostAllOrderNum_010 = 0;

                switch (AType)
                {
                    case -1: //Const_SetZero
                        {
                            singTemp.LostEvenODDOrderNum_009 = 1 + danshuangAdd;
                            singTemp.LostAllOrderNum_010 = 1;
                        }
                        break;
                    case 1: //Const_SetNormal
                        {
                            singTemp.LostEvenODDOrderNum_009 = GetOrder(listLostValueBenTemp, singTemp.LostValue_008) + danshuangAdd;

                            singTemp.LostAllOrderNum_010 = GetOrder(listLostValueAllTemp, singTemp.LostValue_008);
                        }
                        break;
                    default:
                        break;
                }
                ////拼接更新101表的单位单双排序
                string ss = string.Format("Update T_101_{0} set C0{1}1={2}  where  C001={3} ;", IStrYY,i, singTemp.LostEvenODDOrderNum_009, ICurrentLostAll_102.LongPeriod_001);
                IListStringSQL.Add(ss);                

            }

            if (UpdateOrAddSingleAnalysis_106(ListSingleAnalysis_106Temp))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            return flag;
        }

        //得到某位上某数在0-9的数量上的排序
        public static int GetSingValueOrder(int WeiType, List<PeriodDetail_101> AlistPeriodDetail, int APositionValue)
        {
            int order = 0;
            List<int> listAllNumCount = new List<int>();
            int count = 0;
            switch (WeiType)
            {
                case 1:
                    {
                        for (int i = 0; i <= 9; i++)
                        {
                            listAllNumCount.Add(AlistPeriodDetail.Where(p => p.Wei1_010 == i).Count());

                        }
                        count = AlistPeriodDetail.Where(p => p.Wei1_010 == APositionValue).Count();
                        order = GetOrder(listAllNumCount, count);
                    }
                    break;
                case 2:
                    {
                        for (int i = 0; i <= 9; i++)
                        {
                            listAllNumCount.Add(AlistPeriodDetail.Where(p => p.Wei2_020 == i).Count());

                        }
                        count = AlistPeriodDetail.Where(p => p.Wei2_020 == APositionValue).Count();
                        order = GetOrder(listAllNumCount, count);
                    }
                    break;
                case 3:
                    {
                        for (int i = 0; i <= 9; i++)
                        {
                            listAllNumCount.Add(AlistPeriodDetail.Where(p => p.Wei3_030 == i).Count());

                        }
                        count = AlistPeriodDetail.Where(p => p.Wei3_030 == APositionValue).Count();
                        order = GetOrder(listAllNumCount, count);
                    }
                    break;
                case 4:
                    {
                        for (int i = 0; i <= 9; i++)
                        {
                            listAllNumCount.Add(AlistPeriodDetail.Where(p => p.Wei4_040 == i).Count());

                        }
                        count = AlistPeriodDetail.Where(p => p.Wei4_040 == APositionValue).Count();
                        order = GetOrder(listAllNumCount, count);
                    }
                    break;
                case 5:
                    {
                        for (int i = 0; i <= 9; i++)
                        {
                            listAllNumCount.Add(AlistPeriodDetail.Where(p => p.Wei5_050 == i).Count());

                        }
                        count = AlistPeriodDetail.Where(p => p.Wei5_050 == APositionValue).Count();
                        order = GetOrder(listAllNumCount, count);
                    }
                    break;
                default:
                    break;
            }
            return order;
        }

        public static void InitSingleAnalysis_106() { }

        public static bool UpdateOrAddSingleAnalysis_106(List<SingleAnalysis_106> listLostSingleAnalysis_106) 
        {
            bool flag = true;

            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_106_{1} where C001={0}", ICurrentPeriod_101.LongPeriod_001, IStrYY);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                foreach (SingleAnalysis_106 ss in listLostSingleAnalysis_106)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1} AND C005={2}  ", ss.LongPeriod_001, ss.PositionType_004, ss.PositionVale_005)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  AND C005={2} ", ss.LongPeriod_001, ss.PositionType_004, ss.PositionVale_005)).First() : null;

                    if (drCurrent != null) //更新
                    {
                        drCurrent.BeginEdit();
                        drCurrent["C001"] = ss.LongPeriod_001.ToString();
                        drCurrent["C002"] = ss.DateNumber_002.ToString();
                        drCurrent["C003"] = ss.ShortPeriod_003;
                        drCurrent["C004"] = ss.PositionType_004;
                        drCurrent["C005"] = ss.PositionVale_005;
                        drCurrent["C006"] = ss.BigOrSmall_006;
                        drCurrent["C007"] = ss.EvenODD_007;
                        drCurrent["C008"] = ss.LostValue_008;
                        drCurrent["C009"] = ss.LostEvenODDOrderNum_009;
                        drCurrent["C010"] = ss.LostAllOrderNum_010;
                        drCurrent.EndEdit();
                    }
                    else //添加新行
                    {
                        DataRow drNewRow = objDataSet.Tables[0].NewRow();
                        drNewRow["C001"] = ss.LongPeriod_001.ToString();
                        drNewRow["C002"] = ss.DateNumber_002.ToString();
                        drNewRow["C003"] = ss.ShortPeriod_003;
                        drNewRow["C004"] = ss.PositionType_004;
                        drNewRow["C005"] = ss.PositionVale_005;
                        drNewRow["C006"] = ss.BigOrSmall_006;
                        drNewRow["C007"] = ss.EvenODD_007;
                        drNewRow["C008"] = ss.LostValue_008;
                        drNewRow["C009"] = ss.LostEvenODDOrderNum_009;
                        drNewRow["C010"] = ss.LostAllOrderNum_010;
                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }
                }
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
                FileLog.WriteInfo("UpdateOrAddSingleAnalysis_106() ", "Success :" + ICurrentPeriod_101.LongPeriod_001);

            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdateOrAddSingleAnalysis_106()  Err ", e.Message +" 期数:" +ICurrentPeriod_101.LongPeriod_001);
                return flag = false;
            }
            #endregion
            return flag;
        
        
        }

        public static List<SingleAnalysis_106> GetDataSingleAnalysis_106(int ACount, long APeriod)
        {
            List<SingleAnalysis_106> listSingleAnalysis = new List<SingleAnalysis_106>();
            string strSql = string.Empty;
            strSql = String.Format("select top {0} C001,C002,C003,C004,C005,C008 from T_103_{2} where C001<= {1}  order by C001 desc ", ACount * 5, APeriod, IStrYY);

            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, strSql);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SingleAnalysis_106 ll = new SingleAnalysis_106();
                    ll.LongPeriod_001 = long.Parse(dr["C001"].ToString());
                    ll.DateNumber_002 = long.Parse(dr["C002"].ToString());
                    ll.ShortPeriod_003 = int.Parse(dr["C003"].ToString());
                    ll.PositionType_004 = int.Parse(dr["C004"].ToString());
                    ll.PositionVale_005 = int.Parse(dr["C005"].ToString());
                    ll.LostValue_008 = int.Parse(dr["C008"].ToString());
                    listSingleAnalysis.Add(ll);
                }
            }

            return listSingleAnalysis;
        }
        #endregion

        #region  DoHotSingleNum_107
        public static bool DoHotSingleNum_107(int AType, List<PeriodDetail_101> AListPeriodDetail) 
        {
            bool flag = true;
            List<HotSingleNum_107> listHotStatistics = new List<HotSingleNum_107>();
            List<PeriodDetail_101> listPeriodDetailTemp = new List<PeriodDetail_101>();

            if (AListPeriodDetail.Count >= 0)
            {
                listPeriodDetailTemp = AListPeriodDetail.Take(5 ).ToList();
                DoHotStatistics1(5, listPeriodDetailTemp, ref listHotStatistics);

                listPeriodDetailTemp = AListPeriodDetail.Take(10).ToList();
                DoHotStatistics1(10, listPeriodDetailTemp, ref listHotStatistics);

                listPeriodDetailTemp = AListPeriodDetail.Take(15).ToList();
                DoHotStatistics1(15, listPeriodDetailTemp, ref listHotStatistics);

                listPeriodDetailTemp = AListPeriodDetail.Take(20).ToList();
                DoHotStatistics1(20, listPeriodDetailTemp, ref listHotStatistics);

            }

            //写数据
            if (listHotStatistics.Count > 0)
            {
                UpdateOrAddHotSingleNum_107(listHotStatistics);
            }

            return flag;
        }


        public static void DoHotStatistics1(int ASpliteType, List< PeriodDetail_101> AlistPeriodDetail, ref List<HotSingleNum_107> AlistHotStatistics_107)
        {
            if (AlistPeriodDetail.Count > 0)
            {
                int coutNum = 0;
                Type t = typeof(HotSingleNum_107);
                for (int i = 1; i <= 6; i++) //位
                {                    
                    HotSingleNum_107 hs = new HotSingleNum_107();
                    hs.LongPeriod_001 = ICurrentPeriod_101.LongPeriod_001;
                    hs.DateNumber_002 = ICurrentPeriod_101.DateNumber_004;
                    hs.ShortPeriod_003 = ICurrentPeriod_101.ShortPeriod_005;
                    hs.PositionType_004 = i;
                    hs.SliptType_005 = ASpliteType;
                    AlistHotStatistics_107.Add(hs);
                    for (int j = 0; j <= 9; j++) //0~9的值
                    {
                        switch (i)
                        {
                            case 1:
                                {
                                    coutNum = AlistPeriodDetail.Where(p => p.Wei1_010 == j).Count();      
                                    t.GetProperty(String.Format("AppearCount_01{0}", j)).SetValue(hs, coutNum, null);
                                }
                                break;
                            case 2:
                                {
                                    coutNum = AlistPeriodDetail.Where(p => p.Wei2_020 == j).Count();
                                    t.GetProperty(String.Format("AppearCount_01{0}", j)).SetValue(hs, coutNum, null);
                                }
                                break;
                            case 3:
                                {
                                    coutNum = AlistPeriodDetail.Where(p => p.Wei3_030 == j).Count();
                                    t.GetProperty(String.Format("AppearCount_01{0}", j)).SetValue(hs, coutNum, null);
                                }
                                break;
                            case 4:
                                {
                                    coutNum = AlistPeriodDetail.Where(p => p.Wei4_040 == j).Count();
                                    t.GetProperty(String.Format("AppearCount_01{0}", j)).SetValue(hs, coutNum, null);
                                }
                                break;
                            case 5:
                                {
                                    coutNum = AlistPeriodDetail.Where(p => p.Wei5_050 == j).Count();
                                    t.GetProperty(String.Format("AppearCount_01{0}", j)).SetValue(hs, coutNum, null);
                                }
                                break;
                            case 6:
                                {
                                    coutNum = AlistPeriodDetail.Where(p => p.Wei5_050 == j).Count() + AlistPeriodDetail.Where(p => p.Wei4_040 == j).Count() + AlistPeriodDetail.Where(p => p.Wei3_030 == j).Count() + AlistPeriodDetail.Where(p => p.Wei2_020 == j).Count() + AlistPeriodDetail.Where(p => p.Wei1_010 == j).Count();
                                    t.GetProperty(String.Format("AppearCount_01{0}", j)).SetValue(hs, coutNum, null);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        //public static bool DoHotSingleNum_107(int AType, List<PeriodDetail_101> AListPeriodDetail)
        //{
        //    bool flag = true;

        //    List<HotSingleNum_107> listHotStatistics = new List<HotSingleNum_107>();
        //    List<SingleAnalysis_106> ListSingleAnalysis_106 = new List<SingleAnalysis_106>();
        //    ListSingleAnalysis_106 = GetDataSingleAnalysis_106(240, ICurrentPeriod_101.LongPeriod_001);
        //    List<SingleAnalysis_106> listSingle106Temp = new List<SingleAnalysis_106>();

        //    if (ListSingleAnalysis_106.Count >= 5)
        //    {
        //        listSingle106Temp = ListSingleAnalysis_106.Take(5 * 5).ToList();
        //        DoHotStatistics1(5, AListPeriodDetail, ref listHotStatistics);

        //        listSingle106Temp = ListSingleAnalysis_106.Take(5 * 10).ToList();
        //        DoHotStatistics1(10, AListPeriodDetail, ref listHotStatistics);

        //        listSingle106Temp = ListSingleAnalysis_106.Take(5 * 15).ToList();
        //        DoHotStatistics1(15, AListPeriodDetail, ref listHotStatistics);

        //        listSingle106Temp = ListSingleAnalysis_106.Take(5 * 20).ToList();
        //        DoHotStatistics1(20, AListPeriodDetail, ref listHotStatistics);

        //    }

        //    //写数据
        //    if (listHotStatistics.Count > 0)
        //    {
        //        UpdateOrAddHotSingleNum_107(listHotStatistics);
        //    }

        //    return flag;
        //}
        //public static void DoHotStatistics(int ASpliteType, List<SingleAnalysis_106> AlistSingle106, PeriodDetail_101 APeriodDetail, ref List<HotSingleNum_107> AlistHotStatistics_107)
        //{
        //    if (AlistSingle106.Count > 0)
        //    {
        //        decimal pecentage = 0;
        //        int coutNum = 0;
        //        for (int i = 1; i <= 6; i++) //位
        //        {
        //            for (int j = 0; j <= 9; j++) //0~9的值
        //            {
        //                coutNum = 0;
        //                pecentage = 0;
        //                HotSingleNum_107 hs = new HotSingleNum_107();
        //                hs.LongPeriod_001 = APeriodDetail.LongPeriod_001;
        //                hs.DateNumber_002 = APeriodDetail.DateNumber_004;
        //                hs.ShortPeriod_003 = APeriodDetail.ShortPeriod_005;

        //                hs.PositionType_004 = i;
        //                hs.PositionVale_005 = j;
        //                hs.SplitValueType_006 = ASpliteType;
        //                if (i == 6)
        //                {
        //                    coutNum = AlistSingle106.Where(p => p.PositionVale_005 == j).Count();
        //                    hs.NumberAllCount_009 = AlistSingle106.Count;
        //                    pecentage = Math.Round((decimal)coutNum / AlistSingle106.Count, 2);

        //                }
        //                else
        //                {
        //                    coutNum = AlistSingle106.Where(p => p.PositionVale_005 == j && p.PositionType_004 == i).Count();
        //                    hs.NumberAllCount_009 = AlistSingle106.Count / 5;
        //                    pecentage = Math.Round((decimal)coutNum / AlistSingle106.Count, 2);

        //                }

        //                hs.AppearCount_007 = coutNum;
        //                hs.HotValue_008 = pecentage;

        //                AlistHotStatistics_107.Add(hs);
        //            }
        //        }
        //    }
        //}

        public static bool UpdateOrAddHotSingleNum_107(List<HotSingleNum_107> AListHotStatistics_104) 
        {
            bool flag = true;
            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_107_{1} where C001={0}", ICurrentPeriod_101.LongPeriod_001, IStrYY);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                foreach (HotSingleNum_107 ss in AListHotStatistics_104)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  AND C005={2} ", ss.LongPeriod_001, ss.PositionType_004, ss.SliptType_005)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  AND C005={2} ", ss.LongPeriod_001, ss.PositionType_004, ss.SliptType_005)).First() : null;

                    if (drCurrent != null) //更新
                    {
                        drCurrent.BeginEdit();
                        drCurrent["C001"] = ss.LongPeriod_001.ToString();
                        drCurrent["C002"] = ss.DateNumber_002.ToString();
                        drCurrent["C003"] = ss.ShortPeriod_003;
                        drCurrent["C004"] = ss.PositionType_004;
                        drCurrent["C005"] = ss.SliptType_005;
                        drCurrent["C010"] = ss.AppearCount_010;
                        drCurrent["C011"] = ss.AppearCount_011;
                        drCurrent["C012"] = ss.AppearCount_012;
                        drCurrent["C013"] = ss.AppearCount_013;
                        drCurrent["C014"] = ss.AppearCount_014;
                        drCurrent["C015"] = ss.AppearCount_015;
                        drCurrent["C016"] = ss.AppearCount_016;
                        drCurrent["C017"] = ss.AppearCount_017;
                        drCurrent["C018"] = ss.AppearCount_018;
                        drCurrent["C019"] = ss.AppearCount_019;
                        drCurrent.EndEdit();
                    }
                    else //添加新行
                    {
                        DataRow drNewRow = objDataSet.Tables[0].NewRow();
                        drNewRow["C001"] = ss.LongPeriod_001.ToString();
                        drNewRow["C002"] = ss.DateNumber_002.ToString();
                        drNewRow["C003"] = ss.ShortPeriod_003;
                        drNewRow["C004"] = ss.PositionType_004;
                        drNewRow["C005"] = ss.SliptType_005;
                        drNewRow["C010"] = ss.AppearCount_010;
                        drNewRow["C011"] = ss.AppearCount_011;
                        drNewRow["C012"] = ss.AppearCount_012;
                        drNewRow["C013"] = ss.AppearCount_013;
                        drNewRow["C014"] = ss.AppearCount_014;
                        drNewRow["C015"] = ss.AppearCount_015;
                        drNewRow["C016"] = ss.AppearCount_016;
                        drNewRow["C017"] = ss.AppearCount_017;
                        drNewRow["C018"] = ss.AppearCount_018;
                        drNewRow["C019"] = ss.AppearCount_019;
                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }                   
                }
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
                FileLog.WriteInfo("UpdateOrAddHotSingleNum_107() ", "Success :" + ICurrentPeriod_101.LongPeriod_001);

            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdateOrAddHotSingleNum_107() ", e.Message);
                return flag = false;
            }
            #endregion
            return flag;
        }

        public static void GetDataHotSingleNum_107() { }
        #endregion

        #region  DoHotTrend_108
        public static void DoHotTrend_108() { }

        public static void InitHotTrend_108() { }

        public static void UpdateOrAddHotTrend_108() { }

        public static void GetDataHotTrend_108() { }
        #endregion

        #region  DoHotCross_109
        public static void DoHotCross_109() { }

        public static void InitHotCross_109() { }

        public static void UpdateOrAddHotCross_109() { }

        public static void GetDataHotCross_109() { }
        #endregion


        #region  DoSpecialFuture_110
        public static void DoSpecialFuture_110() { }

        //初始化108
        public static SpecialFuture_110 InitDetailSpecialFutrue_108(SingleAnalysis_106 ASingleAnalysis_103, EnumFutureType AFutrue)
        {
            SpecialFuture_110 specialFuture_108 = new SpecialFuture_110();
            specialFuture_108.LongPeriod_001 = ASingleAnalysis_103.LongPeriod_001;
            specialFuture_108.PositionType_002 = ASingleAnalysis_103.PositionType_004;
            specialFuture_108.PositionVale_003 = ASingleAnalysis_103.PositionVale_005;
            specialFuture_108.IsComplete_005 = 0;
            switch (AFutrue)
            {
                case EnumFutureType.F_Xian:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFutureType.F_Xian;
                    }
                    break;
                case EnumFutureType.F_FanXian:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFutureType.F_FanXian;
                    }
                    break;
                case EnumFutureType.F_AddOneOriginal:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFutureType.F_AddOneOriginal;
                    }
                    break;
                case EnumFutureType.F_SubOneOriginal:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFutureType.F_SubOneOriginal;
                    }
                    break;
                case EnumFutureType.F_32283:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFutureType.F_32283;
                    }
                    break;
                case EnumFutureType.F_Three20InSingle:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFutureType.F_Three20InSingle;
                    }
                    break;
                case EnumFutureType.F_ThreeConRepick:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFutureType.F_ThreeConRepick;
                    }
                    break;
                default:
                    break;
            }
            specialFuture_108.IsScore_006 = 0;
            specialFuture_108.ScoreSpan_007 = 0;
            specialFuture_108.ScoreLongPeriod_008 = 0;
            specialFuture_108.ScoreBack_009 = "";
            specialFuture_108.ScoreBack_010 = "";
            specialFuture_108.ScoreBack_011 = 0;
            specialFuture_108.ScoreBack_012 = 0;
            specialFuture_108.ScoreBack_013 = 0;
            specialFuture_108.ScoreBack_014 = 0;
            specialFuture_108.ScoreBack_015 = 0;


            return specialFuture_108;
        }

        public static void UpdateOrAddSpecialFuture_110() { }

        public static void GetDataSpecialFuture_110() { }
        #endregion


        #region  横向遗漏表

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AType">-1为置0，1</param>
        /// <param name="AListPeriod_101"></param>
        /// <returns></returns>
        public static bool DoLostCross_109(int AType, List<PeriodDetail_101> AListPeriod_101) 
        {
            bool flag = true;


            return flag;
        }



        #endregion


        #region  公用方法
        static bool ExecuteListSQL(List<String> AListStringSQL) 
        {
            bool flag = true;
            StringBuilder sb = new StringBuilder();
            if(AListStringSQL!=null && AListStringSQL.Count>0)
            {
                foreach(String ss in AListStringSQL)
                {
                    if ((sb.Length +ss.Length)< 8000)
                    {
                        sb.Append(ss);
                    }
                    else 
                    {
                        try
                        {
                            DbHelperSQL.ExcuteSql(ISqlConnect, sb.ToString());
                            sb = sb.Clear();
                            sb.Append(ss);
                        }
                        catch (Exception e)
                        {
                            FileLog.WriteInfo("ExecuteListSQL():Err", e.Message.ToString() + "---SQL:" + sb.ToString());
                            flag = false;
                            break;
                        }
                    }
                }

                try
                {
                    DbHelperSQL.ExcuteSql(ISqlConnect, sb.ToString());
                }
                catch (Exception e)
                {
                    FileLog.WriteInfo("ExecuteListSQL():Err", e.Message.ToString() + "---SQL:" + sb.ToString());
                    flag = false;
                }

            }
            return flag;
        }

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
                _taskCancel.Cancel();
            }
        }
    }
}
