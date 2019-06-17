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
        private static List<SingleAnalysis_103> IListSingleAnalysis_103;
        private static List<SpecialFuture_108> IListSpecialFuture_108_NotComplete;
        #endregion

        public StatisticsData()
        {
            ISqlConnect = ConfigurationManager.AppSettings["SqlServerConnect"] != null ? ConfigurationManager.AppSettings["SqlServerConnect"] : "Data Source=127.0.0.1,1433;Initial Catalog=Pocker;User Id=sa;Password=net,123";
            ICurrentLostAll_102 = new LostAll_102();
            IPreLostAll_102 = new LostAll_102();
            ICurrentPeriod_101 = new PeriodDetail_101();

            IListSingleAnalysis_103 = new List<SingleAnalysis_103>();

            IListSpecialFuture_108_NotComplete = new List<SpecialFuture_108>();
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

                             InitSingleAnalysis103(0, listPeriodTemp_101);
                         }
                         else
                         {
                             //取前面6天的数据倒序
                             listPeriodTemp_101.Clear();
                             listPeriodTemp_101 = GetListPeriodDetail_101(ConstDefine.Const_GetPreSpan, ICurrentPeriod_101.LongPeriod_001, 59 * 6);
                             InitLostAll_102(ConstDefine.Const_SetNormal, listPeriodTemp_101, ref  ICurrentLostAll_102);

                             IListSingleAnalysis_103.Clear();

                             //取前面100期的数据
                             IListSingleAnalysis_103 = GetDataSingleAnalysis_103(240, IStrLastPeriod);

                             InitSingleAnalysis103(1, listPeriodTemp_101);

                             InitHotStatistics_104(ICurrentPeriod_101);

                             IListSpecialFuture_108_NotComplete.Clear();
                             IListSpecialFuture_108_NotComplete = GetDataNotCompleteSpecialFuture_108(0, ICurrentPeriod_101.LongPeriod_001);

                             InitSpecialFuture_108(listPeriodTemp_101, IListSingleAnalysis_103);
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

            ALostAll_102.PreAllLost0_160 = -1;
            ALostAll_102.PreAllLost1_161 = -1;
            ALostAll_102.PreAllLost2_162 = -1;
            ALostAll_102.PreAllLost3_163 = -1;
            ALostAll_102.PreAllLost4_164 = -1;
            ALostAll_102.PreAllLost5_165 = -1;
            ALostAll_102.PreAllLost6_166 = -1;
            ALostAll_102.PreAllLost7_167 = -1;
            ALostAll_102.PreAllLost8_168 = -1;
            ALostAll_102.PreAllLost9_169 = -1;

            ALostAll_102.PreRepickLost0_170 = -1;
            ALostAll_102.PreRepickLost1_171 = -1;
            ALostAll_102.PreRepickLost2_172 = -1;
            ALostAll_102.PreRepickLost3_173 = -1;
            ALostAll_102.PreRepickLost4_174 = -1;
            ALostAll_102.PreRepickLost5_175 = -1;

            ALostAll_102.PreSpanOneLost0_180 = -1;
            ALostAll_102.PreSpanOneLost1_181 = -1;
            ALostAll_102.PreSpanOneLost2_182 = -1;
            ALostAll_102.PreSpanOneLost3_183 = -1;
            ALostAll_102.PreSpanOneLost4_184 = -1;
            ALostAll_102.PreSpanOneLost5_185 = -1;

            ALostAll_102.PreThreeSameLost_190 = -1;

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

                ALostAll_102.AllLost0_060 = 1;
                ALostAll_102.AllLost1_061 = 1;
                ALostAll_102.AllLost2_062 = 1;
                ALostAll_102.AllLost3_063 = 1;
                ALostAll_102.AllLost4_064 = 1;
                ALostAll_102.AllLost5_065 = 1;
                ALostAll_102.AllLost6_066 = 1;
                ALostAll_102.AllLost7_067 = 1;
                ALostAll_102.AllLost8_068 = 1;
                ALostAll_102.AllLost9_069 = 1;

                ALostAll_102.RepickLost0_070 = 1;
                ALostAll_102.RepickLost1_071 = 1;
                ALostAll_102.RepickLost2_072 = 1;
                ALostAll_102.RepickLost3_073 = 1;
                ALostAll_102.RepickLost4_074 = 1;
                ALostAll_102.RepickLost5_075 = 1;

                ALostAll_102.SpanOneLost0_080 = 1;
                ALostAll_102.SpanOneLost1_081 = 1;
                ALostAll_102.SpanOneLost2_082 = 1;
                ALostAll_102.SpanOneLost3_083 = 1;
                ALostAll_102.SpanOneLost4_084 = 1;
                ALostAll_102.SpanOneLost5_085 = 1;

                ALostAll_102.ThreeSameLost_090 = 1;



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

                for (int i = 0; i <= 9; i++)
                {
                    int count = 0;
                    count = GetMatchCount(ACurrentPeriod_101.AwardNumber_002, i.ToString());
                    if (count > 0)
                    {
                        t.GetProperty(string.Format("AllLost{0}_06{0}", i)).SetValue(ALostAll_102, 0, null);
                        t.GetProperty(string.Format("PreAllLost{0}_16{0}", i)).SetValue(ALostAll_102, 1, null);

                    }
                    if (count >= 3)
                    {
                        ALostAll_102.PreThreeSameLost_190 = 1;
                        ALostAll_102.ThreeSameLost_090 = 0;

                    }
                }

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


                ALostAll_102.AllLost0_060 = IPreLostAll_102.AllLost0_060 + 1;
                ALostAll_102.AllLost1_061 = IPreLostAll_102.AllLost1_061 + 1;
                ALostAll_102.AllLost2_062 = IPreLostAll_102.AllLost2_062 + 1;
                ALostAll_102.AllLost3_063 = IPreLostAll_102.AllLost3_063 + 1;
                ALostAll_102.AllLost4_064 = IPreLostAll_102.AllLost4_064 + 1;
                ALostAll_102.AllLost5_065 = IPreLostAll_102.AllLost5_065 + 1;
                ALostAll_102.AllLost6_066 = IPreLostAll_102.AllLost6_066 + 1;
                ALostAll_102.AllLost7_067 = IPreLostAll_102.AllLost7_067 + 1;
                ALostAll_102.AllLost8_068 = IPreLostAll_102.AllLost8_068 + 1;
                ALostAll_102.AllLost9_069 = IPreLostAll_102.AllLost9_069 + 1;


                ALostAll_102.RepickLost0_070 = IPreLostAll_102.RepickLost0_070 + 1;
                ALostAll_102.RepickLost1_071 = IPreLostAll_102.RepickLost1_071 + 1;
                ALostAll_102.RepickLost2_072 = IPreLostAll_102.RepickLost2_072 + 1;
                ALostAll_102.RepickLost3_073 = IPreLostAll_102.RepickLost3_073 + 1;
                ALostAll_102.RepickLost4_074 = IPreLostAll_102.RepickLost4_074 + 1;
                ALostAll_102.RepickLost5_075 = IPreLostAll_102.RepickLost5_075 + 1;


                ALostAll_102.SpanOneLost0_080 = IPreLostAll_102.SpanOneLost0_080 + 1;
                ALostAll_102.SpanOneLost1_081 = IPreLostAll_102.SpanOneLost1_081 + 1;
                ALostAll_102.SpanOneLost2_082 = IPreLostAll_102.SpanOneLost2_082 + 1;
                ALostAll_102.SpanOneLost3_083 = IPreLostAll_102.SpanOneLost3_083 + 1;
                ALostAll_102.SpanOneLost4_084 = IPreLostAll_102.SpanOneLost4_084 + 1;
                ALostAll_102.SpanOneLost5_085 = IPreLostAll_102.SpanOneLost5_085 + 1;

                ALostAll_102.ThreeSameLost_090 = IPreLostAll_102.ThreeSameLost_090 + 1;

                Type t = typeof(LostAll_102);
                t.GetProperty("Lost_05" + ACurrentPeriod_101.Wei5_050).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_04" + ACurrentPeriod_101.Wei4_040).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_03" + ACurrentPeriod_101.Wei3_030).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_02" + ACurrentPeriod_101.Wei2_020).SetValue(ALostAll_102, 0, null);
                t.GetProperty("Lost_01" + ACurrentPeriod_101.Wei1_010).SetValue(ALostAll_102, 0, null);


                if (AListPeriod_101.Count >= 2)
                {
                    PeriodDetail_101 Period1 = AListPeriod_101[1];
                    //重复
                    int rcount = 0;
                    if (ACurrentPeriod_101.Wei5_050 == Period1.Wei5_050)
                    {
                        rcount++;
                        ALostAll_102.RepickLost5_075 = 0;
                        ALostAll_102.PreRepickLost5_175 = IPreLostAll_102.RepickLost5_075;

                    }
                    if (ACurrentPeriod_101.Wei4_040 == Period1.Wei4_040)
                    {
                        rcount++;
                        ALostAll_102.RepickLost4_074 = 0;
                        ALostAll_102.PreRepickLost4_174 = IPreLostAll_102.RepickLost4_074;
                    }

                    if (ACurrentPeriod_101.Wei3_030 == Period1.Wei3_030)
                    {
                        rcount++;
                        ALostAll_102.RepickLost3_073 = 0;
                        ALostAll_102.PreRepickLost3_173 = IPreLostAll_102.RepickLost3_073;
                    }
                    if (ACurrentPeriod_101.Wei2_020 == Period1.Wei2_020)
                    {
                        rcount++;
                        ALostAll_102.RepickLost2_072 = 0;
                        ALostAll_102.PreRepickLost2_172 = IPreLostAll_102.RepickLost2_072;
                    }
                    if (ACurrentPeriod_101.Wei1_010 == Period1.Wei1_010)
                    {
                        rcount++;
                        ALostAll_102.RepickLost1_071 = 0;
                        ALostAll_102.PreRepickLost1_171 = IPreLostAll_102.RepickLost1_071;
                    }

                    if (rcount > 0)
                    {
                        ALostAll_102.RepickLost0_070 = 0;
                        ALostAll_102.PreRepickLost0_170 = IPreLostAll_102.RepickLost0_070;
                    }


                    if (AListPeriod_101.Count >= 3)
                    {   //间隔
                        PeriodDetail_101 Period2 = AListPeriod_101[2];
                        rcount = 0;
                        if (ACurrentPeriod_101.Wei5_050 == Period2.Wei5_050)
                        {
                            rcount++;
                            ALostAll_102.RepickLost5_075 = 0;
                            ALostAll_102.PreRepickLost5_175 = IPreLostAll_102.RepickLost5_075;

                        }
                        if (ACurrentPeriod_101.Wei4_040 == Period2.Wei4_040)
                        {
                            rcount++;
                            ALostAll_102.RepickLost4_074 = 0;
                            ALostAll_102.PreRepickLost4_174 = IPreLostAll_102.RepickLost4_074;
                        }

                        if (ACurrentPeriod_101.Wei3_030 == Period2.Wei3_030)
                        {
                            rcount++;
                            ALostAll_102.RepickLost3_073 = 0;
                            ALostAll_102.PreRepickLost3_173 = IPreLostAll_102.RepickLost3_073;
                        }
                        if (ACurrentPeriod_101.Wei2_020 == Period2.Wei2_020)
                        {
                            rcount++;
                            ALostAll_102.RepickLost2_072 = 0;
                            ALostAll_102.PreRepickLost2_172 = IPreLostAll_102.RepickLost2_072;
                        }
                        if (ACurrentPeriod_101.Wei1_010 == Period2.Wei1_010)
                        {
                            rcount++;
                            ALostAll_102.RepickLost1_071 = 0;
                            ALostAll_102.PreRepickLost1_171 = IPreLostAll_102.RepickLost1_071;
                        }
                        if (rcount > 0)
                        {
                            ALostAll_102.RepickLost0_070 = 0;
                            ALostAll_102.PreRepickLost0_170 = IPreLostAll_102.RepickLost0_070;
                        }

                    }
                }


                for (int i = 0; i <= 9; i++)
                {
                    int count = 0;
                    count = GetMatchCount(ACurrentPeriod_101.AwardNumber_002, i.ToString());
                    if (count > 0)
                    {
                        t.GetProperty(string.Format("AllLost{0}_06{0}", i)).SetValue(ALostAll_102, 0, null);
                        t.GetProperty(string.Format("PreAllLost{0}_16{0}", i)).SetValue(ALostAll_102,
                            int.Parse(GetObjectPropertyValue(IPreLostAll_102, string.Format("AllLost{0}_06{0}", i))),
                            null);
                        if (count >= 3)
                        {
                            ALostAll_102.PreThreeSameLost_190 = IPreLostAll_102.ThreeSameLost_090 - 1;
                            ALostAll_102.ThreeSameLost_090 = 0;
                        }
                    }
                }

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

                    drCurrent["C060"] = AlostAllWei.AllLost0_060;
                    drCurrent["C061"] = AlostAllWei.AllLost1_061;
                    drCurrent["C062"] = AlostAllWei.AllLost2_062;
                    drCurrent["C063"] = AlostAllWei.AllLost3_063;
                    drCurrent["C064"] = AlostAllWei.AllLost4_064;
                    drCurrent["C065"] = AlostAllWei.AllLost5_065;
                    drCurrent["C066"] = AlostAllWei.AllLost6_066;
                    drCurrent["C067"] = AlostAllWei.AllLost7_067;
                    drCurrent["C068"] = AlostAllWei.AllLost8_068;
                    drCurrent["C069"] = AlostAllWei.AllLost9_069;

                    drCurrent["C070"] = AlostAllWei.RepickLost0_070;
                    drCurrent["C071"] = AlostAllWei.RepickLost1_071;
                    drCurrent["C072"] = AlostAllWei.RepickLost2_072;
                    drCurrent["C073"] = AlostAllWei.RepickLost3_073;
                    drCurrent["C074"] = AlostAllWei.RepickLost4_074;
                    drCurrent["C075"] = AlostAllWei.RepickLost5_075;

                    drCurrent["C080"] = AlostAllWei.SpanOneLost0_080;
                    drCurrent["C081"] = AlostAllWei.SpanOneLost1_081;
                    drCurrent["C082"] = AlostAllWei.SpanOneLost2_082;
                    drCurrent["C083"] = AlostAllWei.SpanOneLost3_083;
                    drCurrent["C084"] = AlostAllWei.SpanOneLost4_084;
                    drCurrent["C085"] = AlostAllWei.SpanOneLost5_085;

                    drCurrent["C090"] = AlostAllWei.ThreeSameLost_090;

                    drCurrent["C101"] = AlostAllWei.PreLost_101;
                    drCurrent["C102"] = AlostAllWei.PreLost_102;
                    drCurrent["C103"] = AlostAllWei.PreLost_103;
                    drCurrent["C104"] = AlostAllWei.PreLost_104;
                    drCurrent["C105"] = AlostAllWei.PreLost_105;


                    drCurrent["C160"] = AlostAllWei.PreAllLost0_160;
                    drCurrent["C161"] = AlostAllWei.PreAllLost1_161;
                    drCurrent["C162"] = AlostAllWei.PreAllLost2_162;
                    drCurrent["C163"] = AlostAllWei.PreAllLost3_163;
                    drCurrent["C164"] = AlostAllWei.PreAllLost4_164;
                    drCurrent["C165"] = AlostAllWei.PreAllLost5_165;
                    drCurrent["C166"] = AlostAllWei.PreAllLost6_166;
                    drCurrent["C167"] = AlostAllWei.PreAllLost7_167;
                    drCurrent["C168"] = AlostAllWei.PreAllLost8_168;
                    drCurrent["C169"] = AlostAllWei.PreAllLost9_169;

                    drCurrent["C170"] = AlostAllWei.PreRepickLost0_170;
                    drCurrent["C171"] = AlostAllWei.PreRepickLost1_171;
                    drCurrent["C172"] = AlostAllWei.PreRepickLost2_172;
                    drCurrent["C173"] = AlostAllWei.PreRepickLost3_173;
                    drCurrent["C174"] = AlostAllWei.PreRepickLost4_174;
                    drCurrent["C175"] = AlostAllWei.PreRepickLost5_175;

                    drCurrent["C180"] = AlostAllWei.PreSpanOneLost0_180;
                    drCurrent["C181"] = AlostAllWei.PreSpanOneLost1_181;
                    drCurrent["C182"] = AlostAllWei.PreSpanOneLost2_182;
                    drCurrent["C183"] = AlostAllWei.PreSpanOneLost3_183;
                    drCurrent["C184"] = AlostAllWei.PreSpanOneLost4_184;
                    drCurrent["C185"] = AlostAllWei.PreSpanOneLost5_185;


                    drCurrent["C190"] = AlostAllWei.PreThreeSameLost_190;

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

                    drNewRow["C060"] = AlostAllWei.AllLost0_060;
                    drNewRow["C061"] = AlostAllWei.AllLost1_061;
                    drNewRow["C062"] = AlostAllWei.AllLost2_062;
                    drNewRow["C063"] = AlostAllWei.AllLost3_063;
                    drNewRow["C064"] = AlostAllWei.AllLost4_064;
                    drNewRow["C065"] = AlostAllWei.AllLost5_065;
                    drNewRow["C066"] = AlostAllWei.AllLost6_066;
                    drNewRow["C067"] = AlostAllWei.AllLost7_067;
                    drNewRow["C068"] = AlostAllWei.AllLost8_068;
                    drNewRow["C069"] = AlostAllWei.AllLost9_069;

                    drNewRow["C070"] = AlostAllWei.RepickLost0_070;
                    drNewRow["C071"] = AlostAllWei.RepickLost1_071;
                    drNewRow["C072"] = AlostAllWei.RepickLost2_072;
                    drNewRow["C073"] = AlostAllWei.RepickLost3_073;
                    drNewRow["C074"] = AlostAllWei.RepickLost4_074;
                    drNewRow["C075"] = AlostAllWei.RepickLost5_075;

                    drNewRow["C080"] = AlostAllWei.SpanOneLost0_080;
                    drNewRow["C081"] = AlostAllWei.SpanOneLost1_081;
                    drNewRow["C082"] = AlostAllWei.SpanOneLost2_082;
                    drNewRow["C083"] = AlostAllWei.SpanOneLost3_083;
                    drNewRow["C084"] = AlostAllWei.SpanOneLost4_084;
                    drNewRow["C085"] = AlostAllWei.SpanOneLost5_085;

                    drNewRow["C090"] = AlostAllWei.ThreeSameLost_090;

                    drNewRow["C101"] = AlostAllWei.PreLost_101;
                    drNewRow["C102"] = AlostAllWei.PreLost_102;
                    drNewRow["C103"] = AlostAllWei.PreLost_103;
                    drNewRow["C104"] = AlostAllWei.PreLost_104;
                    drNewRow["C105"] = AlostAllWei.PreLost_105;

                    drNewRow["C160"] = AlostAllWei.PreAllLost0_160;
                    drNewRow["C161"] = AlostAllWei.PreAllLost1_161;
                    drNewRow["C162"] = AlostAllWei.PreAllLost2_162;
                    drNewRow["C163"] = AlostAllWei.PreAllLost3_163;
                    drNewRow["C164"] = AlostAllWei.PreAllLost4_164;
                    drNewRow["C165"] = AlostAllWei.PreAllLost5_165;
                    drNewRow["C166"] = AlostAllWei.PreAllLost6_166;
                    drNewRow["C167"] = AlostAllWei.PreAllLost7_167;
                    drNewRow["C168"] = AlostAllWei.PreAllLost8_168;
                    drNewRow["C169"] = AlostAllWei.PreAllLost9_169;

                    drNewRow["C170"] = AlostAllWei.PreRepickLost0_170;
                    drNewRow["C171"] = AlostAllWei.PreRepickLost1_171;
                    drNewRow["C172"] = AlostAllWei.PreRepickLost2_172;
                    drNewRow["C173"] = AlostAllWei.PreRepickLost3_173;
                    drNewRow["C174"] = AlostAllWei.PreRepickLost4_174;
                    drNewRow["C175"] = AlostAllWei.PreRepickLost5_175;


                    drNewRow["C180"] = AlostAllWei.PreSpanOneLost0_180;
                    drNewRow["C181"] = AlostAllWei.PreSpanOneLost1_181;
                    drNewRow["C182"] = AlostAllWei.PreSpanOneLost2_182;
                    drNewRow["C183"] = AlostAllWei.PreSpanOneLost3_183;
                    drNewRow["C184"] = AlostAllWei.PreSpanOneLost4_184;
                    drNewRow["C185"] = AlostAllWei.PreSpanOneLost5_185;


                    drNewRow["C190"] = AlostAllWei.PreThreeSameLost_190;

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

                    lostall_102.AllLost0_060 = int.Parse(drNewRow["C060"].ToString());
                    lostall_102.AllLost1_061 = int.Parse(drNewRow["C061"].ToString());
                    lostall_102.AllLost2_062 = int.Parse(drNewRow["C062"].ToString());
                    lostall_102.AllLost3_063 = int.Parse(drNewRow["C063"].ToString());
                    lostall_102.AllLost4_064 = int.Parse(drNewRow["C064"].ToString());
                    lostall_102.AllLost5_065 = int.Parse(drNewRow["C065"].ToString());
                    lostall_102.AllLost6_066 = int.Parse(drNewRow["C066"].ToString());
                    lostall_102.AllLost7_067 = int.Parse(drNewRow["C067"].ToString());
                    lostall_102.AllLost8_068 = int.Parse(drNewRow["C068"].ToString());
                    lostall_102.AllLost9_069 = int.Parse(drNewRow["C069"].ToString());

                    lostall_102.RepickLost0_070 = int.Parse(drNewRow["C070"].ToString());
                    lostall_102.RepickLost1_071 = int.Parse(drNewRow["C071"].ToString());
                    lostall_102.RepickLost2_072 = int.Parse(drNewRow["C072"].ToString());
                    lostall_102.RepickLost3_073 = int.Parse(drNewRow["C073"].ToString());
                    lostall_102.RepickLost4_074 = int.Parse(drNewRow["C074"].ToString());
                    lostall_102.RepickLost5_075 = int.Parse(drNewRow["C075"].ToString());

                    lostall_102.SpanOneLost0_080 = int.Parse(drNewRow["C080"].ToString());
                    lostall_102.SpanOneLost1_081 = int.Parse(drNewRow["C081"].ToString());
                    lostall_102.SpanOneLost2_082 = int.Parse(drNewRow["C082"].ToString());
                    lostall_102.SpanOneLost3_083 = int.Parse(drNewRow["C083"].ToString());
                    lostall_102.SpanOneLost4_084 = int.Parse(drNewRow["C084"].ToString());
                    lostall_102.SpanOneLost5_085 = int.Parse(drNewRow["C085"].ToString());
                    lostall_102.ThreeSameLost_090 = int.Parse(drNewRow["C090"].ToString());

                    lostall_102.PreLost_101 = int.Parse(drNewRow["C101"].ToString());
                    lostall_102.PreLost_102 = int.Parse(drNewRow["C102"].ToString());
                    lostall_102.PreLost_103 = int.Parse(drNewRow["C103"].ToString());
                    lostall_102.PreLost_104 = int.Parse(drNewRow["C104"].ToString());
                    lostall_102.PreLost_105 = int.Parse(drNewRow["C105"].ToString());

                    lostall_102.PreAllLost0_160 = int.Parse(drNewRow["C160"].ToString());
                    lostall_102.PreAllLost1_161 = int.Parse(drNewRow["C161"].ToString());
                    lostall_102.PreAllLost2_162 = int.Parse(drNewRow["C162"].ToString());
                    lostall_102.PreAllLost3_163 = int.Parse(drNewRow["C163"].ToString());
                    lostall_102.PreAllLost4_164 = int.Parse(drNewRow["C164"].ToString());
                    lostall_102.PreAllLost5_165 = int.Parse(drNewRow["C165"].ToString());
                    lostall_102.PreAllLost6_166 = int.Parse(drNewRow["C166"].ToString());
                    lostall_102.PreAllLost7_167 = int.Parse(drNewRow["C167"].ToString());
                    lostall_102.PreAllLost8_168 = int.Parse(drNewRow["C168"].ToString());
                    lostall_102.PreAllLost9_169 = int.Parse(drNewRow["C169"].ToString());

                    lostall_102.PreRepickLost0_170 = int.Parse(drNewRow["C170"].ToString());
                    lostall_102.PreRepickLost1_171 = int.Parse(drNewRow["C171"].ToString());
                    lostall_102.PreRepickLost2_172 = int.Parse(drNewRow["C172"].ToString());
                    lostall_102.PreRepickLost3_173 = int.Parse(drNewRow["C173"].ToString());
                    lostall_102.PreRepickLost4_174 = int.Parse(drNewRow["C174"].ToString());
                    lostall_102.PreRepickLost5_175 = int.Parse(drNewRow["C175"].ToString());

                    lostall_102.PreSpanOneLost0_180 = int.Parse(drNewRow["C180"].ToString());
                    lostall_102.PreSpanOneLost1_181 = int.Parse(drNewRow["C181"].ToString());
                    lostall_102.PreSpanOneLost2_182 = int.Parse(drNewRow["C182"].ToString());
                    lostall_102.PreSpanOneLost3_183 = int.Parse(drNewRow["C183"].ToString());
                    lostall_102.PreSpanOneLost4_184 = int.Parse(drNewRow["C184"].ToString());
                    lostall_102.PreSpanOneLost5_185 = int.Parse(drNewRow["C185"].ToString());

                    lostall_102.PreThreeSameLost_190 = int.Parse(drNewRow["C190"].ToString());
                }
            }
            return lostall_102;
        }

        #endregion

        #region  DoSingle103  单位详情

        /// <summary>
        /// 单位分析详情
        /// </summary>
        /// <param name="AType">0为置0   1为正常统计</param>
        /// <param name="AListPeriod_101"></param>
        /// <param name="ASingleAnalysis_103"></param>
        /// <returns></returns>
        public static bool InitSingleAnalysis103(int AType, List<PeriodDetail_101> AListPeriod_101)
        {
            bool flag = true;

            Type t = typeof(SingleAnalysis_103);

            //保存前一期所有遗失次数
            List<int> listLostValueAllTemp = new List<int>();

            //保存本位单双上的遗失次数
            List<int> listLostValueBenTemp = new List<int>();

            //保存20期大于3个，且本期出现了
            List<SpecialFuture_108> ListSpecialFuture108Temp = new List<SpecialFuture_108>();
            List<SingleAnalysis_103> ListSingleAnalysis_103Temp = new List<SingleAnalysis_103>();

            //保存0~9的数量
            List<int> listAllNumCount = new List<int>();

            //保存临时的PeriodDetail_101
            List<PeriodDetail_101> listPeriodDetailTemp = new List<PeriodDetail_101>();

            if (AType != 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        listLostValueAllTemp.Add(int.Parse(GetObjectPropertyValue(IPreLostAll_102, String.Format("Lost_0{0}{1}", i, j))));
                    }
                }
            }

            for (int i = 1; i <= 5; i++) //1位到5位
            {
                listLostValueBenTemp = new List<int>();
                SingleAnalysis_103 singTemp = new SingleAnalysis_103();
                ListSingleAnalysis_103Temp.Add(singTemp);
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
                if (singTemp.PositionVale_005 % 2 == 1)
                {
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
                singTemp.OrderInTen_011 = 0;
                singTemp.OrderInTwenty_012 = 0;
                singTemp.OrderInFourty_013 = 0;
                singTemp.OrderInOneDay_014 = 0;
                singTemp.OrderInTwoDay_015 = 0;
                singTemp.OrderInThreeDay_016 = 0;
                singTemp.OrderInFourDay_017 = 0;
                singTemp.Remain20Num_018 = 0;
                singTemp.RemainEven20Num_019 = 0;
                singTemp.RemainOdd20Num_020 = 0;
                singTemp.IsAppear20_021 = 0;
                singTemp.IsAppear20M_022 = 0;
                singTemp.IsThree20InSingle_023 = 0;
                singTemp.IsInTwoSame_024 = 0;
                singTemp.IsInThreeSame_025 = 0;
                singTemp.IsInFourSame_026 = 0;
                singTemp.IsInFiveSame_027 = 0;
                singTemp.IsBigCon_028 = 0;
                singTemp.BigConDetail_029 = 0;
                singTemp.IsSmallCon_030 = 0;
                singTemp.SmallConDetail_031 = 0;
                singTemp.IsEvenCon_032 = 0;
                singTemp.EvenConDetail_033 = 0;
                singTemp.IsOddCon_034 = 0;
                singTemp.OddConDetail_035 = 0;
                singTemp.IsRepick_036 = 0;
                singTemp.RepickDetail_037 = 0;
                singTemp.IsThreeRepick_038 = 0;
                singTemp.IsFourRepick_039 = 0;
                singTemp.IsTurn_040 = 0;
                singTemp.IsSpanOne_041 = 0;
                singTemp.IsTwoConRepick_042 = 0;
                singTemp.IsThreeConRepick_043 = 0;
                singTemp.IsFourPile_044 = 0;
                singTemp.IsFivePile_045 = 0;
                singTemp.IsTread_046 = 0;
                singTemp.TreadDetail_047 = 0;
                singTemp.IsTreadAddOne_048 = 0;
                singTemp.TreadAddOneDetail_049 = 0;
                singTemp.IsTreadAddTwo_050 = 0;

                int count = 0;
                count = GetMatchCount(ICurrentPeriod_101.AwardNumber_002, singTemp.PositionVale_005.ToString());
                if (count >= 2)
                {
                    singTemp.IsInTwoSame_024 = 1;
                    if (count >= 3)
                    {
                        singTemp.IsInThreeSame_025 = 1;
                        if (count >= 4)
                        {
                            singTemp.IsInFourSame_026 = 1;
                            if (count == 5)
                            {
                                singTemp.IsInFiveSame_027 = 1;
                            }
                        }
                    }
                }

                switch (AType)
                {
                    case 0: //置0
                        {
                            singTemp.LostEvenODDOrderNum_009 = 1;
                            singTemp.LostAllOrderNum_010 = 1;
                        }
                        break;
                    case 1: //正常统计
                        {
                            singTemp.LostEvenODDOrderNum_009 = GetOrder(listLostValueBenTemp, singTemp.LostValue_008);
                            if (singTemp.LostValue_008 >= 20)
                            {
                                singTemp.IsAppear20_021 = 1;

                                if (singTemp.LostEvenODDOrderNum_009 == 1)
                                {
                                    singTemp.IsAppear20M_022 = 1;
                                }
                            }

                            singTemp.LostAllOrderNum_010 = GetOrder(listLostValueAllTemp, singTemp.LostValue_008);
                            if (AListPeriod_101.Count >= 5)
                            {
                                #region  //0~9这个位置上个数排序
                                if (AListPeriod_101 != null && AListPeriod_101.Count >= 10)
                                {

                                    listPeriodDetailTemp = AListPeriod_101.Take(10).ToList();
                                    singTemp.OrderInTen_011 = GetSingValueOrder(i, listPeriodDetailTemp, singTemp.PositionVale_005);

                                    if (AListPeriod_101.Count >= 20)
                                    {
                                        listPeriodDetailTemp.Clear();
                                        listPeriodDetailTemp = AListPeriod_101.Take(20).ToList();
                                        singTemp.OrderInTwenty_012 = GetSingValueOrder(i, listPeriodDetailTemp, singTemp.PositionVale_005);

                                        if (AListPeriod_101.Count >= 40)
                                        {
                                            listPeriodDetailTemp.Clear();
                                            listPeriodDetailTemp = AListPeriod_101.Take(40).ToList();
                                            singTemp.OrderInFourty_013 = GetSingValueOrder(i, listPeriodDetailTemp, singTemp.PositionVale_005);

                                            if (AListPeriod_101.Count >= 59)
                                            {
                                                listPeriodDetailTemp.Clear();
                                                listPeriodDetailTemp = AListPeriod_101.Take(59).ToList();
                                                singTemp.OrderInOneDay_014 = GetSingValueOrder(i, listPeriodDetailTemp, singTemp.PositionVale_005);


                                                if (AListPeriod_101.Count >= 59 * 2)
                                                {
                                                    listPeriodDetailTemp.Clear();
                                                    listPeriodDetailTemp = AListPeriod_101.Take(59 * 2).ToList();
                                                    singTemp.OrderInTwoDay_015 = GetSingValueOrder(i, listPeriodDetailTemp, singTemp.PositionVale_005);
                                                    if (AListPeriod_101.Count >= 59 * 3)
                                                    {
                                                        listPeriodDetailTemp.Clear();
                                                        listPeriodDetailTemp = AListPeriod_101.Take(59 * 3).ToList();
                                                        singTemp.OrderInThreeDay_016 = GetSingValueOrder(i, listPeriodDetailTemp, singTemp.PositionVale_005);
                                                        if (AListPeriod_101.Count >= 59 * 4)
                                                        {
                                                            listPeriodDetailTemp.Clear();
                                                            listPeriodDetailTemp = AListPeriod_101.Take(59 * 4).ToList();
                                                            singTemp.OrderInFourDay_017 = GetSingValueOrder(i, listPeriodDetailTemp, singTemp.PositionVale_005);

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region  //大于20期
                                listLostValueBenTemp.Clear();
                                if (singTemp.PositionVale_005 % 2 == 1)  //单
                                {
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}1", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}3", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}5", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}7", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}9", i))));

                                    int ctemp = 0;
                                    ctemp = listLostValueBenTemp.Count(s => s >= 20);
                                    singTemp.RemainEven20Num_019 = ctemp;
                                    if (ctemp >= 2 && singTemp.LostValue_008 >= 20)
                                    {
                                        SpecialFuture_108 specialFuture_108 = InitDetailSpecialFutrue_108(singTemp, EnumFuture.F_Three20InSingle);
                                        singTemp.IsThree20InSingle_023 = 1;
                                        StringBuilder sb = new StringBuilder();
                                        for (int k = 0; k < 5; k++)
                                        {
                                            if (listLostValueBenTemp[k] >= 20)
                                            {
                                                switch (k)
                                                {
                                                    case 0:
                                                        sb.Append("1").Append(",");
                                                        break;
                                                    case 1:
                                                        sb.Append("3").Append(",");
                                                        break;
                                                    case 2:
                                                        sb.Append("5").Append(",");
                                                        break;
                                                    case 3:
                                                        sb.Append("7").Append(",");
                                                        break;
                                                    case 4:
                                                        sb.Append("9").Append(",");
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                        sb.Append(specialFuture_108.PositionVale_003);
                                        specialFuture_108.ScoreBack_009 = sb.ToString().TrimEnd(',');
                                        ListSpecialFuture108Temp.Add(specialFuture_108);
                                    }
                                    listLostValueBenTemp.Clear();
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}0", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}2", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}4", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}6", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}8", i))));
                                    ctemp = listLostValueBenTemp.Count(s => s >= 20);
                                    singTemp.RemainOdd20Num_020 = ctemp;
                                    singTemp.Remain20Num_018 = singTemp.RemainEven20Num_019 + singTemp.RemainOdd20Num_020;

                                }
                                else  //双
                                {
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}0", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}2", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}4", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}6", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}8", i))));
                                    int ctemp = 0;
                                    ctemp = listLostValueBenTemp.Count(s => s >= 20);
                                    singTemp.RemainOdd20Num_020 = ctemp;

                                    if (ctemp >= 3 && singTemp.LostValue_008 >= 20)
                                    {
                                        SpecialFuture_108 specialFuture_108 = InitDetailSpecialFutrue_108(singTemp, EnumFuture.F_Three20InSingle);
                                        singTemp.IsThree20InSingle_023 = 1;
                                        StringBuilder sb = new StringBuilder();
                                        for (int k = 0; k < 5; k++)
                                        {
                                            if (listLostValueBenTemp[k] >= 20)
                                            {
                                                switch (k)
                                                {
                                                    case 0:
                                                        sb.Append("0").Append(",");
                                                        break;
                                                    case 1:
                                                        sb.Append("2").Append(",");
                                                        break;
                                                    case 2:
                                                        sb.Append("4").Append(",");
                                                        break;
                                                    case 3:
                                                        sb.Append("6").Append(",");
                                                        break;
                                                    case 4:
                                                        sb.Append("8").Append(",");
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                        sb.Append(specialFuture_108.PositionVale_003);
                                        specialFuture_108.ScoreBack_009 = sb.ToString().TrimEnd(',');
                                        ListSpecialFuture108Temp.Add(specialFuture_108);
                                    }
                                    listLostValueBenTemp.Clear();
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}1", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}3", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}5", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}7", i))));
                                    listLostValueBenTemp.Add(int.Parse(GetObjectPropertyValue(ICurrentLostAll_102, String.Format("Lost_0{0}9", i))));
                                    ctemp = listLostValueBenTemp.Count(s => s >= 20);
                                    singTemp.RemainEven20Num_019 = ctemp;
                                    singTemp.Remain20Num_018 = singTemp.RemainEven20Num_019 + singTemp.RemainOdd20Num_020;
                                }
                                #endregion

                                //存入最近25期该位上出现的值
                                int periodCount = 0;
                                listAllNumCount.Clear();
                                foreach (PeriodDetail_101 p in AListPeriod_101)
                                {
                                    periodCount++;
                                    listAllNumCount.Add(int.Parse(GetObjectPropertyValue(p, String.Format("Wei{0}_0{0}0", i))));
                                    if (periodCount >= 25)
                                    {
                                        break;
                                    }
                                }

                                #region  //是否大连 小连 双连 单连
                                int continueCount = 0;
                                if (singTemp.PositionVale_005 % 2 == 1) //单
                                {

                                    for (int k = 0; k < periodCount; k++)
                                    {
                                        if (listAllNumCount[k] % 2 == 1)
                                        {
                                            continueCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    if (continueCount >= 3)
                                    {
                                        singTemp.IsEvenCon_032 = 1;
                                        singTemp.EvenConDetail_033 = continueCount;
                                    }
                                }
                                else  //双
                                {
                                    for (int k = 0; k < periodCount; k++)
                                    {
                                        if (listAllNumCount[k] % 2 == 0)
                                        {
                                            continueCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    if (continueCount >= 3)
                                    {
                                        singTemp.IsOddCon_034 = 1;
                                        singTemp.OddConDetail_035 = continueCount;
                                    }
                                }

                                continueCount = 0;
                                if (singTemp.PositionVale_005 >= 5) //大
                                {

                                    for (int k = 0; k < periodCount; k++)
                                    {
                                        if (listAllNumCount[k] >= 5)
                                        {
                                            continueCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    if (continueCount >= 3)
                                    {
                                        singTemp.IsBigCon_028 = 1;
                                        singTemp.BigConDetail_029 = continueCount;
                                    }
                                }
                                else //小
                                {
                                    for (int k = 0; k <= AListPeriod_101.Count - 1; k++)
                                    {
                                        if (listAllNumCount[k] < 5)
                                        {
                                            continueCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    if (continueCount >= 3)
                                    {
                                        singTemp.IsSmallCon_030 = 1;
                                        singTemp.SmallConDetail_031 = continueCount;
                                    }
                                }

                                #endregion

                                #region 重复
                                {
                                    continueCount = 0;
                                    for (int k = 0; k < listAllNumCount.Count; k++)
                                    {
                                        if (
                                            ((k + 1) < (listAllNumCount.Count - 1))
                                            && listAllNumCount[k] == listAllNumCount[k + 1]
                                            )
                                        {
                                            continueCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    if (continueCount >= 1)
                                    {
                                        singTemp.IsRepick_036 = 1;
                                        singTemp.RepickDetail_037 = continueCount;
                                        if (continueCount >= 2)
                                        {
                                            singTemp.IsThreeRepick_038 = 1;
                                            if (continueCount >= 3)
                                            {
                                                singTemp.IsFourRepick_039 = 1;
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region 转折 间隔 2连对，3连对
                               
                                if (listAllNumCount.Count>=3)
                                {
                                    if((listAllNumCount[0]-listAllNumCount[1])>0 && (listAllNumCount[1]-listAllNumCount[2])<0)
                                    {
                                        singTemp.IsTurn_040 = 1;
                                    }
                                    else if ((listAllNumCount[0] - listAllNumCount[1]) < 0 && (listAllNumCount[1] - listAllNumCount[2]) > 0)
                                    {
                                        singTemp.IsTurn_040 = 1;
                                    }

                                    if (listAllNumCount[0] == listAllNumCount[2])
                                    {
                                        singTemp.IsSpanOne_041 = 1;
                                    }

                                    if(listAllNumCount.Count>=4)
                                    {
                                       if(listAllNumCount[0]== listAllNumCount[1] && listAllNumCount[2]==listAllNumCount[3])
                                       {
                                           singTemp.IsTwoConRepick_042 = 1;
                                       }

                                        if(listAllNumCount.Count>=6)
                                        {
                                            if (listAllNumCount[0] == listAllNumCount[1] && listAllNumCount[2] == listAllNumCount[3] && listAllNumCount[4]== listAllNumCount[5]) 
                                            {
                                                singTemp.IsThreeConRepick_043 = 1;
                                            }
                                        }

                                    }

                                }
                                #endregion

                                #region //4连堆

                                //4连堆  要取List103的数据
                                List<SingleAnalysis_103> listSingleAnalysis_103Temp = IListSingleAnalysis_103.Where(p => p.PositionVale_005 == singTemp.PositionVale_005 && p.PositionType_004 == i).Count() > 0 ? IListSingleAnalysis_103.Where(p => p.PositionVale_005 == singTemp.PositionVale_005 && p.PositionType_004 == i).ToList() : null;
                                if (listSingleAnalysis_103Temp != null && listSingleAnalysis_103Temp.Count >= 3)
                                {
                                    continueCount = 0;
                                    foreach (SingleAnalysis_103 ss in listSingleAnalysis_103Temp)
                                    {
                                        if (ss.LostValue_008 < 10)
                                        {
                                            continueCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    if (continueCount >= 3)
                                    {
                                        singTemp.IsFourPile_044 = 1;
                                        if (continueCount >= 4)
                                        {
                                            singTemp.IsFivePile_045 = 1;
                                        }
                                    }
                                }
                                #endregion

                                #region//趋势
                                continueCount = 0;
                                if ((listAllNumCount[0] - listAllNumCount[1]) > 0)
                                {
                                    for (int k = 0; k < listAllNumCount.Count; k++)
                                    {
                                        if ((k + 1) < listAllNumCount.Count && (listAllNumCount[k] - listAllNumCount[k + 1]) > 0)
                                        {
                                            continueCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else if ((listAllNumCount[0] - listAllNumCount[1]) < 0)
                                {
                                    for (int k = 0; k < listAllNumCount.Count; k++)
                                    {
                                        if ((k + 1) < listAllNumCount.Count && (listAllNumCount[k] - listAllNumCount[k + 1]) < 0)
                                        {
                                            continueCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int k = 0; k < listAllNumCount.Count; k++)
                                    {
                                        if ((k + 1) < listAllNumCount.Count && (listAllNumCount[k] - listAllNumCount[k + 1]) == 0)
                                        {
                                            continueCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (continueCount >= 2)
                                {
                                    singTemp.IsTread_046 = 1;
                                    singTemp.TreadDetail_047 = continueCount;
                                }

                                continueCount = 0;
                                if ((listAllNumCount[0] - listAllNumCount[1]) == 1)
                                {
                                    for (int k = 0; k < listAllNumCount.Count; k++)
                                    {
                                        if ((k + 1) < listAllNumCount.Count && (listAllNumCount[k] - listAllNumCount[k + 1]) == 1)
                                        {
                                            continueCount++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (continueCount >= 2)
                                {
                                    singTemp.IsTreadAddOne_048 = 1;
                                    singTemp.TreadAddOneDetail_049 = continueCount;
                                }

                                //continueCount = 0;
                                //if ((listAllNumCount[0] - listAllNumCount[1]) == 2)
                                //{
                                //    for (int k = 0; k < listAllNumCount.Count; k++)
                                //    {
                                //        if ((k + 1) < listAllNumCount.Count && (listAllNumCount[k] - listAllNumCount[k + 1]) == 2)
                                //        {
                                //            continueCount++;
                                //        }
                                //        else
                                //        {
                                //            break;
                                //        }
                                //    }
                                //}
                                //if (continueCount >= 2)
                                //{
                                //    singTemp.IsTreadAddTwo_049 = 1;
                                //}

                                #endregion
                            }
                        }
                        break;
                    default:
                        break;
                }

            }

            if (UpdateOrADDSingleAnalysis_103(ListSingleAnalysis_103Temp, AListPeriod_101[0].LongPeriod_001))
            {
                FileLog.WriteInfo("InitSingleAnalysis103() --UpdateOrADDSingleAnalysis_103()", "Succ ");
            }
            else
            {
                FileLog.WriteInfo("InitSingleAnalysis103() --UpdateOrADDSingleAnalysis_103()", "Fail ");
            }

            foreach (SingleAnalysis_103 ss in ListSingleAnalysis_103Temp)
            {
                IListSingleAnalysis_103.Add(ss);
            }

            IListSingleAnalysis_103 = IListSingleAnalysis_103.OrderByDescending(p => p.LongPeriod_001).ToList();

            if(ListSpecialFuture108Temp.Count > 0)
            {
                if (UpdateOrADDSpecialFuture_108(ListSpecialFuture108Temp, 1))
                {
                    FileLog.WriteInfo("InitSingleAnalysis103() --UpdateOrADDSpecialFuture_108()", "Succ ");
                }
                else
                {
                    FileLog.WriteInfo("InitSingleAnalysis103() --UpdateOrADDSpecialFuture_108()", "Fail ");
                }

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

        //取103表当前统计期前面的数据
        public static List<SingleAnalysis_103> GetDataSingleAnalysis_103(int ACount, long APeriod)
        {
            List<SingleAnalysis_103> listSingleAnalysis = new List<SingleAnalysis_103>();
            string strSql = string.Empty;
            strSql = String.Format("select top {0} C001,C002,C003,C004,C005,C008 from T_103_{2} where C001<= {1}  order by C001 desc ", ACount * 5, APeriod, IStrYY);

            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, strSql);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SingleAnalysis_103 ll = new SingleAnalysis_103();
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

        public static bool UpdateOrADDSingleAnalysis_103(List<SingleAnalysis_103> AListSingleAnalysis, long APeriod)
        {
            bool flag = true;

            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_103_{1} where C001={0}", APeriod, IStrYY);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                foreach (SingleAnalysis_103 ss in AListSingleAnalysis)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  AND C005={2} ", ss.LongPeriod_001, ss.PositionType_004, ss.PositionVale_005)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  AND C005={2}", ss.LongPeriod_001, ss.PositionType_004, ss.PositionVale_005)).First() : null;

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
                        drCurrent["C011"] = ss.OrderInTen_011;
                        drCurrent["C012"] = ss.OrderInTwenty_012;
                        drCurrent["C013"] = ss.OrderInFourty_013;
                        drCurrent["C014"] = ss.OrderInOneDay_014;
                        drCurrent["C015"] = ss.OrderInTwoDay_015;
                        drCurrent["C016"] = ss.OrderInThreeDay_016;
                        drCurrent["C017"] = ss.OrderInFourDay_017;
                        drCurrent["C018"] = ss.Remain20Num_018;
                        drCurrent["C019"] = ss.RemainEven20Num_019;
                        drCurrent["C020"] = ss.RemainOdd20Num_020;
                        drCurrent["C021"] = ss.IsAppear20_021;
                        drCurrent["C022"] = ss.IsAppear20M_022;
                        drCurrent["C023"] = ss.IsThree20InSingle_023;
                        drCurrent["C024"] = ss.IsInTwoSame_024;
                        drCurrent["C025"] = ss.IsInThreeSame_025;
                        drCurrent["C026"] = ss.IsInFourSame_026;
                        drCurrent["C027"] = ss.IsInFiveSame_027;
                        drCurrent["C028"] = ss.IsBigCon_028;
                        drCurrent["C029"] = ss.BigConDetail_029;
                        drCurrent["C030"] = ss.IsSmallCon_030;
                        drCurrent["C031"] = ss.SmallConDetail_031;
                        drCurrent["C032"] = ss.IsEvenCon_032;
                        drCurrent["C033"] = ss.EvenConDetail_033;
                        drCurrent["C034"] = ss.IsOddCon_034;
                        drCurrent["C035"] = ss.OddConDetail_035;
                        drCurrent["C036"] = ss.IsRepick_036;
                        drCurrent["C037"] = ss.RepickDetail_037;
                        drCurrent["C038"] = ss.IsThreeRepick_038;
                        drCurrent["C039"] = ss.IsFourRepick_039;
                        drCurrent["C040"] = ss.IsTurn_040;
                        drCurrent["C041"] = ss.IsSpanOne_041;
                        drCurrent["C042"] = ss.IsTwoConRepick_042;
                        drCurrent["C043"] = ss.IsThreeConRepick_043;
                        drCurrent["C044"] = ss.IsFourPile_044;
                        drCurrent["C045"] = ss.IsFivePile_045;
                        drCurrent["C046"] = ss.IsTread_046;
                        drCurrent["C047"] = ss.TreadDetail_047;
                        drCurrent["C048"] = ss.IsTreadAddOne_048;
                        drCurrent["C049"] = ss.TreadAddOneDetail_049;
                        drCurrent["C050"] = ss.IsTreadAddTwo_050;
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
                        drNewRow["C011"] = ss.OrderInTen_011;
                        drNewRow["C012"] = ss.OrderInTwenty_012;
                        drNewRow["C013"] = ss.OrderInFourty_013;
                        drNewRow["C014"] = ss.OrderInOneDay_014;
                        drNewRow["C015"] = ss.OrderInTwoDay_015;
                        drNewRow["C016"] = ss.OrderInThreeDay_016;
                        drNewRow["C017"] = ss.OrderInFourDay_017;
                        drNewRow["C018"] = ss.Remain20Num_018;
                        drNewRow["C019"] = ss.RemainEven20Num_019;
                        drNewRow["C020"] = ss.RemainOdd20Num_020;
                        drNewRow["C021"] = ss.IsAppear20_021;
                        drNewRow["C022"] = ss.IsAppear20M_022;
                        drNewRow["C023"] = ss.IsThree20InSingle_023;
                        drNewRow["C024"] = ss.IsInTwoSame_024;
                        drNewRow["C025"] = ss.IsInThreeSame_025;
                        drNewRow["C026"] = ss.IsInFourSame_026;
                        drNewRow["C027"] = ss.IsInFiveSame_027;
                        drNewRow["C028"] = ss.IsBigCon_028;
                        drNewRow["C029"] = ss.BigConDetail_029;
                        drNewRow["C030"] = ss.IsSmallCon_030;
                        drNewRow["C031"] = ss.SmallConDetail_031;
                        drNewRow["C032"] = ss.IsEvenCon_032;
                        drNewRow["C033"] = ss.EvenConDetail_033;
                        drNewRow["C034"] = ss.IsOddCon_034;
                        drNewRow["C035"] = ss.OddConDetail_035;
                        drNewRow["C036"] = ss.IsRepick_036;
                        drNewRow["C037"] = ss.RepickDetail_037;
                        drNewRow["C038"] = ss.IsThreeRepick_038;
                        drNewRow["C039"] = ss.IsFourRepick_039;
                        drCurrent["C040"] = ss.IsTurn_040;
                        drCurrent["C041"] = ss.IsSpanOne_041;
                        drCurrent["C042"] = ss.IsTwoConRepick_042;
                        drCurrent["C043"] = ss.IsThreeConRepick_043;
                        drCurrent["C044"] = ss.IsFourPile_044;
                        drCurrent["C045"] = ss.IsFivePile_045;
                        drCurrent["C046"] = ss.IsTread_046;
                        drCurrent["C047"] = ss.TreadDetail_047;
                        drCurrent["C048"] = ss.IsTreadAddOne_048;
                        drCurrent["C049"] = ss.TreadAddOneDetail_049;
                        drCurrent["C050"] = ss.IsTreadAddTwo_050;

                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }
                }


                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
                FileLog.WriteInfo("UpdateOrADDSingleAnalysis_103() ", "Success :" + APeriod);

            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdateOrADDSingleAnalysis_103()  fail ", e.Message);
                return flag = false;
            }
            #endregion
            return flag;
        }

        #endregion

        #region 104  热度表

        public static bool InitHotStatistics_104(PeriodDetail_101 APeriodDetail)
        {
            bool flag = true;

            List<HotStatistics_104> listHotStatistics = new List<HotStatistics_104>();

            List<SingleAnalysis_103> listSingle103Temp = new List<SingleAnalysis_103>();

            if (IListSingleAnalysis_103.Count >= 25)
            {
                listSingle103Temp = IListSingleAnalysis_103.Take(5 * 5).ToList();
                DoHotStatistics(5, IListSingleAnalysis_103, APeriodDetail, ref listHotStatistics);

                listSingle103Temp = IListSingleAnalysis_103.Take(5 * 10).ToList();
                DoHotStatistics(10, IListSingleAnalysis_103, APeriodDetail, ref listHotStatistics);

                listSingle103Temp = IListSingleAnalysis_103.Take(5 * 15).ToList();
                DoHotStatistics(15, IListSingleAnalysis_103, APeriodDetail, ref listHotStatistics);

                listSingle103Temp = IListSingleAnalysis_103.Take(5 * 20).ToList();
                DoHotStatistics(20, IListSingleAnalysis_103, APeriodDetail, ref listHotStatistics);

            }

            //写数据
            if (listHotStatistics.Count > 0)
            {
                UpdateOrAddHotstatistics_104(listHotStatistics, APeriodDetail.LongPeriod_001);
            }

            return flag;
        }

        public static void DoHotStatistics(int ASpliteType, List<SingleAnalysis_103> AlistSingle103, PeriodDetail_101 APeriodDetail, ref List<HotStatistics_104> AlistHotStatistics_104)
        {
            if (AlistSingle103.Count > 0)
            {
                decimal pecentage = 0;
                int coutNum = 0;
                for (int i = 0; i <= 5; i++) //位
                {
                    for (int j = 0; j <= 9; j++) //0~9的值
                    {
                        coutNum = 0;
                        pecentage = 0;
                        HotStatistics_104 hs = new HotStatistics_104();
                        hs.LongPeriod_001 = APeriodDetail.LongPeriod_001;
                        hs.DateNumber_002 = APeriodDetail.DateNumber_004;
                        hs.ShortPeriod_003 = APeriodDetail.ShortPeriod_005;

                        hs.PositionType_004 = i;
                        hs.PositionVale_005 = j;
                        hs.SpliteType_006 = ASpliteType;
                        if (i == 0)
                        {
                            coutNum = AlistSingle103.Where(p => p.PositionVale_005 == j).Count();
                            hs.NumberAllCount_009 = AlistSingle103.Count;
                            pecentage = Math.Round((decimal)coutNum / AlistSingle103.Count, 2);

                        }
                        else
                        {
                            coutNum = AlistSingle103.Where(p => p.PositionVale_005 == j && p.PositionType_004 == i).Count();
                            hs.NumberAllCount_009 = AlistSingle103.Count / 5;
                            pecentage = Math.Round((decimal)coutNum / AlistSingle103.Count, 2);

                        }

                        hs.NumberCount_007 = coutNum;
                        hs.HotValue_008 = pecentage;

                        AlistHotStatistics_104.Add(hs);
                    }
                }
            }
        }

        public static bool UpdateOrAddHotstatistics_104(List<HotStatistics_104> AListHotStatistics_104, long APeriod)
        {
            bool flag = true;

            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_104_{1} where C001={0}", APeriod, IStrYY);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                foreach (HotStatistics_104 ss in AListHotStatistics_104)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  AND C005={2} AND C006={3} ", ss.LongPeriod_001, ss.PositionType_004, ss.PositionVale_005, ss.SpliteType_006)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}  AND  C004={1}  AND C005={2} AND C006={3} ", ss.LongPeriod_001, ss.PositionType_004, ss.PositionVale_005, ss.SpliteType_006)).First() : null;

                    if (drCurrent != null) //更新
                    {
                        drCurrent.BeginEdit();
                        drCurrent["C001"] = ss.LongPeriod_001.ToString();
                        drCurrent["C002"] = ss.DateNumber_002.ToString();
                        drCurrent["C003"] = ss.ShortPeriod_003;
                        drCurrent["C004"] = ss.PositionType_004;
                        drCurrent["C005"] = ss.PositionVale_005;
                        drCurrent["C006"] = ss.SpliteType_006;
                        drCurrent["C007"] = ss.NumberCount_007;
                        drCurrent["C008"] = ss.HotValue_008;
                        drCurrent["C009"] = ss.NumberAllCount_009;
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
                        drNewRow["C006"] = ss.SpliteType_006;
                        drNewRow["C007"] = ss.NumberCount_007;
                        drNewRow["C008"] = ss.HotValue_008;
                        drNewRow["C009"] = ss.NumberAllCount_009;
                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                    FileLog.WriteInfo("UpdateOrADDSingleAnalysis_103() ", "Success :" + APeriod);
                }


            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdateOrADDSingleAnalysis_103() ", e.Message);
                return flag = false;
            }
            #endregion
            return flag;
        }

        #endregion

        #region 特征数据 SpecialFuture_108

        //补全更新数据108
        public static void AddDetailSpecialFuture_108(bool flag, long LongPeriod, SpecialFuture_108 sf)
        {
            if (flag)
            {
                sf.IsComplete_005 = 1;
                sf.IsScore_006 = 1;
                sf.ScoreSpan_007 = 1;
                sf.ScoreLongPeriod_008 = LongPeriod;
            }
            else
            {
                sf.IsComplete_005 = 1;
                sf.IsScore_006 = 0;
                sf.ScoreLongPeriod_008 = LongPeriod;
            }

        }

        //初始化108
        public static SpecialFuture_108 InitDetailSpecialFutrue_108(SingleAnalysis_103 ASingleAnalysis_103, EnumFuture AFutrue)
        {
            SpecialFuture_108 specialFuture_108 = new SpecialFuture_108();
            specialFuture_108.LongPeriod_001 = ASingleAnalysis_103.LongPeriod_001;
            specialFuture_108.PositionType_002 = ASingleAnalysis_103.PositionType_004;
            specialFuture_108.PositionVale_003 = ASingleAnalysis_103.PositionVale_005;
            specialFuture_108.IsComplete_005 = 0;
            switch (AFutrue)
            {
                case EnumFuture.F_Xian:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFuture.F_Xian;
                    }
                    break;
                case EnumFuture.F_FanXian:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFuture.F_FanXian;
                    }
                    break;
                case EnumFuture.F_AddOneOriginal:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFuture.F_AddOneOriginal;
                    }
                    break;
                case EnumFuture.F_SubOneOriginal:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFuture.F_SubOneOriginal;
                    }
                    break;
                case EnumFuture.F_32283:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFuture.F_32283;
                    }
                    break;
                case EnumFuture.F_Three20InSingle:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFuture.F_Three20InSingle;
                    }
                    break;
                case EnumFuture.F_ThreeConRepick:
                    {
                        specialFuture_108.FutureType_004 = (int)EnumFuture.F_ThreeConRepick;
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

        /// <summary>
        /// 根据101和103表得到108表数据
        /// </summary>
        public static void InitSpecialFuture_108(List<PeriodDetail_101> AListPeriod_101, List<SingleAnalysis_103> AListSingleAnalysis_103)
        {
            // 可用103和101的数据来处理得到这种结果
            List<int> listSingleNumValue = new List<int>();
            List<SpecialFuture_108> listSfTemp = new List<SpecialFuture_108>();
            List<SpecialFuture_108> listSpecialFuture_108new = new List<SpecialFuture_108>();
            for (int i = 1; i <= 5; i++) //1位到5位
            {
                listSfTemp.Clear();
                listSpecialFuture_108new.Clear();
                listSfTemp = IListSpecialFuture_108_NotComplete.Where(p => p.PositionType_002 == i).ToList();
                int periodCount = 0;
                listSingleNumValue.Clear();

                SingleAnalysis_103 singTemp = AListSingleAnalysis_103.Where(p => p.PositionType_004 == i && p.LongPeriod_001 == ICurrentPeriod_101.LongPeriod_001).Count() > 0 ? AListSingleAnalysis_103.Where(p => p.PositionType_004 == i && p.LongPeriod_001 == ICurrentPeriod_101.LongPeriod_001).First() : null;
                if (singTemp == null) return;

                foreach (PeriodDetail_101 p in AListPeriod_101)
                {
                    periodCount++;
                    listSingleNumValue.Add(int.Parse(GetObjectPropertyValue(p, String.Format("Wei{0}_0{0}0", i))));
                    if (periodCount >= 25)
                    {
                        break;
                    }
                }

                #region  //处理预警数据
                {
                    //F_Xian=1, 线性
                    if (GetXianXinAlarm(listSingleNumValue[0], listSingleNumValue[1], listSingleNumValue[2], listSingleNumValue[3]))
                    {
                        if (listSfTemp.Where(p => p.PositionType_002 == i && p.FutureType_004 == 1).Count() == 0)
                        {
                            SpecialFuture_108 specialFuture_108 = InitDetailSpecialFutrue_108(singTemp, EnumFuture.F_Xian);
                            listSpecialFuture_108new.Add(specialFuture_108);
                        }
                    }
                    //F_FanXian=2, 反线性
                    if (GetFanXianXinAlarm(listSingleNumValue[0], listSingleNumValue[1], listSingleNumValue[2], listSingleNumValue[3]))
                    {
                        if (listSfTemp.Where(p => p.PositionType_002 == i && p.FutureType_004 == 2).Count() == 0)
                        {
                            SpecialFuture_108 specialFuture_108 = InitDetailSpecialFutrue_108(singTemp, EnumFuture.F_FanXian);
                            listSpecialFuture_108new.Add(specialFuture_108);
                        }
                    }
                    //F_AddOneOriginal=3, 加1复位
                    if (GetAddOneOriginalAlarm(listSingleNumValue[0], listSingleNumValue[1], listSingleNumValue[2]))
                    {
                        SpecialFuture_108 specialFuture_108 = InitDetailSpecialFutrue_108(singTemp, EnumFuture.F_AddOneOriginal);
                        listSpecialFuture_108new.Add(specialFuture_108);
                    }

                    //F_SubOneOriginal=4,  减1复位
                    if (GetSubOneOriginalAlarm(listSingleNumValue[0], listSingleNumValue[1], listSingleNumValue[2]))
                    {
                        SpecialFuture_108 specialFuture_108 = InitDetailSpecialFutrue_108(singTemp, EnumFuture.F_SubOneOriginal);
                        listSpecialFuture_108new.Add(specialFuture_108);
                    }

                    //F_32283=5, 32283
                    if (Get32283Alarm(listSingleNumValue[0], listSingleNumValue[1], listSingleNumValue[2], listSingleNumValue[3]))
                    {
                        SpecialFuture_108 specialFuture_108 = InitDetailSpecialFutrue_108(singTemp, EnumFuture.F_32283);
                        listSpecialFuture_108new.Add(specialFuture_108);
                    }


                }
                #endregion

                #region    //没有完成的要处理  IListSpecialFuture_108_NotComplete
                foreach (SpecialFuture_108 sf in listSfTemp)
                {
                    #region
                    switch (sf.FutureType_004)
                    {
                        case 1: // F_Xian=1, 线性
                            {
                                if (listSingleNumValue.Count >= 5)
                                {
                                    bool flag = false;
                                    flag = GetXianXinJudge(listSingleNumValue[0], listSingleNumValue[1], listSingleNumValue[2], listSingleNumValue[3], listSingleNumValue[4]);
                                    AddDetailSpecialFuture_108(flag, AListPeriod_101[0].LongPeriod_001, sf);
                                }
                            }
                            break;
                        case 2:  //F_FanXian=2, 反线性
                            {
                                if (listSingleNumValue.Count >= 5)
                                {
                                    bool flag = false;
                                    flag = GetFanXianXinJudge(listSingleNumValue[0], listSingleNumValue[1], listSingleNumValue[2], listSingleNumValue[3], listSingleNumValue[4]);
                                    AddDetailSpecialFuture_108(flag, AListPeriod_101[0].LongPeriod_001, sf);
                                }
                            }
                            break;
                        case 3://F_AddOneOriginal=3, 加1复位
                            {
                                if (listSingleNumValue.Count >= 5)
                                {
                                    bool flag = false;
                                    flag = GetAddOneOriginalJudge(listSingleNumValue[0], listSingleNumValue[1], listSingleNumValue[2], listSingleNumValue[3]);
                                    AddDetailSpecialFuture_108(flag, AListPeriod_101[0].LongPeriod_001, sf);

                                }
                            }
                            break;
                        case 4://F_SubOneOriginal=4,  减1复位
                            {
                                bool flag = false;
                                flag = GetSubOneOriginalJudge(listSingleNumValue[0], listSingleNumValue[1], listSingleNumValue[2], listSingleNumValue[3]);
                                AddDetailSpecialFuture_108(flag, AListPeriod_101[0].LongPeriod_001, sf);
                            }
                            break;
                        case 5://F_32283=5, 32283
                            {
                                bool flag = false;
                                flag = Get32283Judge(listSingleNumValue[0], listSingleNumValue[3]);
                                AddDetailSpecialFuture_108(flag, AListPeriod_101[0].LongPeriod_001, sf);
                            }
                            break;
                        case 6://F_Three20InSingle, 单位单双遗失大于20以上3期内打出
                            {
                                #region
                                int span = AListPeriod_101.Where(p => p.LongPeriod_001 >= sf.LongPeriod_001 && p.LongPeriod_001 <= ICurrentPeriod_101.LongPeriod_001).Count() - 1;
                                //如果一直是双 ,期数上在3期内
                                if (sf.PositionVale_003 % 2 == 1)
                                {                                
                                    //单
                                    if (listSingleNumValue[0] % 2 == 1)
                                    {
                                        if (sf.ScoreBack_009.Contains(listSingleNumValue[0].ToString()))
                                        {
                                            sf.IsComplete_005 = 1;
                                            sf.IsScore_006 = 1;
                                            sf.ScoreSpan_007 = 1;
                                            sf.ScoreLongPeriod_008 = AListPeriod_101[0].LongPeriod_001;
                                        }
                                        else if (span >= 3)
                                        {
                                            sf.IsComplete_005 = 1;
                                            sf.IsScore_006 = 0;
                                            sf.ScoreLongPeriod_008 = AListPeriod_101[0].LongPeriod_001;
                                        }
                                    }
                                }
                                else
                                {
                                    if (listSingleNumValue[0] % 2 == 0)
                                    {
                                        if (sf.ScoreBack_009.Contains(listSingleNumValue[0].ToString()))
                                        {
                                            sf.IsComplete_005 = 1;
                                            sf.IsScore_006 = 1;
                                            sf.ScoreSpan_007 = 1;
                                            sf.ScoreLongPeriod_008 = AListPeriod_101[0].LongPeriod_001;
                                        }
                                        else if (span >= 3)
                                        {
                                            sf.IsComplete_005 = 1;
                                            sf.IsScore_006 = 0;
                                            sf.ScoreLongPeriod_008 = AListPeriod_101[0].LongPeriod_001;
                                        }
                                    }
                                }
                                #endregion
                            }
                            break;
                        case 7: //F_ThreeConRepick  连续3对
                            {
                            }
                            break;
                        case 8: //F_BigConOriginalBig 单位大长连后变小后复大
                            {
                            }
                            break;
                        case 9://F_SmallConOriginalSmall  单位小长连后变大后复小
                            {
                            }
                            break;
                        case 10: //F_EvenConOriginalEven 单位单长连后变双后复单
                            {
                            }
                            break;
                        case 11: //F_OddConOriginalOdd 单位双长连后变双后复双
                            {
                            }
                            break;
                        case 12: // F_LostOver6=14,>20期遗失大于6期未出出后后短期出
                            {
                            }
                            break;
                        case 13://F_RecpickOver6=15, 重复连续6期及上未出短期出
                            {
                            }
                            break;
                        case 14:// F_SpanOneOver6=16, 间隔连续6期及上未出出来后短期出
                            {
                            }
                            break;
                        case 15: // F_LostNumOver10=17 >20期遗失短期内冲到10个数以上出后狂出
                            {
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                }
                #endregion

                #region  //写数据
                {

                    if (listSpecialFuture_108new.Count > 0) 
                    {
                        if (UpdateOrADDSpecialFuture_108(listSpecialFuture_108new, 1))
                        {
                            FileLog.WriteInfo("UpdateOrADDSpecialFuture_108 1 ", "success");
                        }
                        else
                        {
                            FileLog.WriteInfo("UpdateOrADDSpecialFuture_108  1", "fail");
                        }
                    }                   

                    if (listSfTemp.Count > 0)
                    {
                        if (UpdateOrADDSpecialFuture_108(listSfTemp, 2))
                        {
                            FileLog.WriteInfo("UpdateOrADDSpecialFuture_108  2", "success");
                        }
                        else
                        {
                            FileLog.WriteInfo("UpdateOrADDSpecialFuture_108  2", "fail");
                        }
                    }
                }
                #endregion

            }
        }

        #region 特征判定

        //add one判定
        public static bool GetAddOneOriginalJudge(int one, int two, int three, int four)
        {
            bool flag = false;
            if (four == one)
            {
                return true;
            }
            else if (four > one)
            {
                if ((one + 2) >= four)
                {
                    flag = true;
                }
            }
            else if (four < one)
            {
                if ((four + 2) > one)
                {
                    flag = true;
                }
            }
            return flag;
        }

        //sub one判定
        public static bool GetSubOneOriginalJudge(int one, int two, int three, int four)
        {
            bool flag = false;
            if (four == one)
            {
                return true;
            }
            else if (four > one)
            {
                if ((one + 2) >= four)
                {
                    flag = true;
                }
            }
            else if (four < one)
            {
                if ((four + 2) > one)
                {
                    flag = true;
                }
            }

            return flag;
        }
        //32283判定
        public static bool Get32283Judge(int one, int four)
        {
            bool flag = false;
            if (one == four)
            {
                return true;
            }

            return flag;
        }
        //8288判定
        public static bool Get8288Judge(int one, int four)
        {
            bool flag = false;
            if (one == four)
            {
                return true;
            }
            return flag;
        }
        //8828判定
        public static bool Get8828Judge(int one, int four)
        {
            bool flag = false;
            if (one == four)
            {
                return true;
            }
            return flag;
        }

        public static bool GetXianXinJudge(int one, int two, int three, int four, int five)
        {
            bool flag = false;
            int span1 = one - two;
            int span2 = two - three;
            int span3 = three - four;
            int span4 = four - five;

            if (Math.Abs(span1) >= Math.Abs(span2) && Math.Abs(span2) >= Math.Abs(span3) && Math.Abs(span3) >= Math.Abs(span4)
                && (span1 * span2 < 0) && (span1 * span3 > 0) && (span2 * span4 > 0)
                )
            {
                flag = true;
            }


            return flag;
        }
        public static bool GetFanXianXinJudge(int one, int two, int three, int four, int five)
        {
            bool flag = false;
            int span1 = one - two;
            int span2 = two - three;
            int span3 = three - four;
            int span4 = four - five;

            if (Math.Abs(span1) <= Math.Abs(span2) && Math.Abs(span2) <= Math.Abs(span3) && Math.Abs(span3) <= Math.Abs(span4)
                && (span1 * span2 < 0) && (span1 * span3 > 0) && (span2 * span4 > 0)
                )
            {
                flag = true;
            }


            return flag;
        }

        #endregion

        #region  特征预警
        /// <summary>
        /// 
        /// </summary>
        /// <param name="one">为最开始时数据</param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <param name="four">为最后的数据</param>
        /// <returns></returns>
        public static bool GetXianXinAlarm(int one, int two, int three, int four)
        {
            bool flag = false;
            int span1 = one - two;
            int span2 = two - three;
            int span3 = three - four;
            if (Math.Abs(span1) >= Math.Abs(span2) && Math.Abs(span2) >= Math.Abs(span3) && (span1 * span2) < 0 && (span1 * span3) > 0)
            {
                flag = true;
            }

            return flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one">为最开始时数据</param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <param name="four">为最后的数据</param>
        /// <returns></returns>
        public static bool GetFanXianXinAlarm(int one, int two, int three, int four)
        {
            bool flag = false;
            int span1 = one - two;
            int span2 = two - three;
            int span3 = three - four;
            if (Math.Abs(span1) <= Math.Abs(span2) && Math.Abs(span2) <= Math.Abs(span3) && (span1 * span2) < 0 && (span1 * span3) > 0)
            {
                flag = true;
            }

            return flag;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one">最近的数字</param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <returns></returns>
        public static bool GetSubOneOriginalAlarm(int one, int two, int three)
        {
            bool flag = false;
            if ((one - two) == 1 && (three - two) >= 3)
            {
                return true;
            }
            else if ((two - one) == 1 && (two - three) >= 3)
            {
                return true;
            }
            return flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one">最近的数字</param>
        /// <param name="two"></param>
        /// <param name="three"></param>
        /// <returns></returns>
        public static bool GetAddOneOriginalAlarm(int one, int two, int three)
        {
            bool flag = false;
            if ((two - one) == 1 && (three - two) >= 3)
            {
                return true;
            }
            else if ((one - two) == 1 && (two - three) >= 3)
            {
                return true;
            }
            return flag;
        }

        public static bool Get32283Alarm(int one, int two, int three, int four)
        {
            bool flag = false;
            if ((two == three) && (four % 2 != three % 2) && (one % 2 == two % 2))
            {
                return true;
            }
            return flag;
        }

        public static bool Get8288Alarm(int one, int two, int three, int four)
        {
            bool flag = false;
            if ((one == three) && (one != two) && (four != three))
            {
                return true;
            }
            return flag;
        }

        public static bool Get8828Alarm(int one, int two, int three, int four)
        {
            bool flag = false;
            if ((one != two) && (two == three) && (four != three))
            {
                return true;
            }
            return flag;
        }


        #endregion


        //从特征表取没有完成的特征数据
        public static List<SpecialFuture_108> GetDataNotCompleteSpecialFuture_108(int AType, long APeriodNumber)
        {
            List<SpecialFuture_108> listSpecialFuture = new List<SpecialFuture_108>();

            String StrSQL = string.Empty;
            switch (AType)
            {
                case 0:  //得到当前期
                    StrSQL = string.Format("select  * from T_108_{0} where C005 = 0  AND C001< {1} order by  C001 desc", IStrYY, APeriodNumber);
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drNewRow in ds.Tables[0].Rows)
                {
                    SpecialFuture_108 sf = new SpecialFuture_108();
                    sf.LongPeriod_001 = long.Parse(drNewRow["C001"].ToString());
                    sf.PositionType_002 = int.Parse(drNewRow["C002"].ToString());
                    sf.PositionVale_003 = int.Parse(drNewRow["C003"].ToString());
                    sf.FutureType_004 = int.Parse(drNewRow["C004"].ToString());
                    sf.IsComplete_005 = int.Parse(drNewRow["C005"].ToString());
                    sf.IsScore_006 = int.Parse(drNewRow["C006"].ToString());
                    sf.ScoreSpan_007 = int.Parse(drNewRow["C007"].ToString());
                    sf.ScoreLongPeriod_008 = int.Parse(drNewRow["C008"].ToString());
                    sf.ScoreBack_009 = drNewRow["C009"] != DBNull.Value ? drNewRow["C009"].ToString() : "";
                    sf.ScoreBack_010 = drNewRow["C010"] != DBNull.Value ? drNewRow["C010"].ToString() : "";
                    sf.ScoreBack_011 = int.Parse(drNewRow["C011"].ToString());
                    sf.ScoreBack_012 = int.Parse(drNewRow["C012"].ToString());
                    sf.ScoreBack_013 = int.Parse(drNewRow["C013"].ToString());
                    sf.ScoreBack_014 = int.Parse(drNewRow["C014"].ToString());
                    sf.ScoreBack_015 = int.Parse(drNewRow["C015"].ToString());
                    listSpecialFuture.Add(sf);
                }
            }
            return listSpecialFuture;

        }


        //更新或添加特征数据到数据库
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AListSpecialFuture"></param>
        /// <param name="AType">1添加 2更新 </param>
        /// <returns></returns>
        public static bool UpdateOrADDSpecialFuture_108(List<SpecialFuture_108> AListSpecialFuture, int AType)
        {
            bool flag = true;
            string strSql = string.Empty;
            #region
            try
            {
                switch (AType)
                {
                    case 1: //添加
                        {
                            foreach (SpecialFuture_108 sf in AListSpecialFuture)
                            {
                                strSql = string.Format("insert into T_108_{0} (C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,C012,C013,C014,C015) values({1},{2},{3},{4},{5},{6},{7},{8},'{9}','{10}',{11},{12},{13},{14},{15}) ", IStrYY, sf.LongPeriod_001, sf.PositionType_002, sf.PositionVale_003, sf.FutureType_004, sf.IsComplete_005, sf.IsScore_006, sf.ScoreSpan_007, sf.ScoreLongPeriod_008, sf.ScoreBack_009, sf.ScoreBack_010, sf.ScoreBack_011, sf.ScoreBack_012, sf.ScoreBack_013, sf.ScoreBack_014, sf.ScoreBack_015);
                                try
                                {
                                    DbHelperSQL.ExcuteSql(ISqlConnect, strSql);
                                }
                                catch (Exception eccc)
                                {

                                    FileLog.WriteError("UpdateOrADDSpecialFuture_108() --insert", eccc.Message + strSql);
                                    return flag = false;
                                }

                            }
                        }
                        break;
                    case 2://更新
                        {
                            foreach (SpecialFuture_108 sf in AListSpecialFuture)
                            {
                                strSql = string.Format("update T_108_{0}  set C005={1},C006={2},C007={3},C008={4}  where C001={5} AND C002={6} AND C003={7} AND C004={8} ", IStrYY, sf.IsComplete_005, sf.IsScore_006, sf.ScoreSpan_007, sf.ScoreLongPeriod_008, sf.LongPeriod_001, sf.PositionType_002, sf.PositionVale_003, sf.FutureType_004);
                                try
                                {
                                    DbHelperSQL.ExcuteSql(ISqlConnect, strSql);
                                }
                                catch (Exception eccc)
                                {

                                    FileLog.WriteError("UpdateOrADDSpecialFuture_108() ---update", eccc.Message + strSql);
                                    return flag = false;
                                }

                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdateOrADDSpecialFuture_108() ", e.Message);
                return flag = false;
            }
            #endregion
            return flag;
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
