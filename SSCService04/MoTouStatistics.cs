using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Collections.Concurrent;
using Modles;
using DBHelp;
using System.Reflection;

namespace SSCService04
{
    public    class MoTouStatistics
    {

        #region  统计线程
        private bool IBoolIsThreadMoTouWorking;
        private Thread IThreadDoMoTou;


        private static bool IBoolNotAllEqualsLastFetch;
        private static long ILastFetchPeriod;


        private static string ISqlConnect = string.Empty;
        private static string IStrYY = string.Empty;

        private static List<MoTouType_130> IListMoTouType_130 =null;
        private static  List<AllWeiStatistics_120> IListAllWeiStatistics_120 =null;
        private static List<PeriodDetail_101> IListPeriodDetail_101 = null;
        private static List<LostAll_102> IListLostAll_102 = null;
        #endregion

        public MoTouStatistics()
        {
            ISqlConnect = ConfigurationManager.AppSettings["SqlServerConnect"] != null ? ConfigurationManager.AppSettings["SqlServerConnect"] : "Data Source=127.0.0.1,1433;Initial Catalog=Pocker;User Id=sa;Password=net,123";
            IStrYY = ConfigurationManager.AppSettings["IStrYY"] != null ? ConfigurationManager.AppSettings["IStrYY"] : "19";
            IBoolNotAllEqualsLastFetch = false;
            ILastFetchPeriod = 0;
            IListMoTouType_130 = new List<MoTouType_130>();
            IListAllWeiStatistics_120 = new List<AllWeiStatistics_120>();
        }

        private static void AutoDoMoTou(object o) 
        {
            MoTouStatistics autoMouTou = o as MoTouStatistics;
            int i = 0;
            while (autoMouTou.IBoolIsThreadMoTouWorking)
            {                
                //取一下120表内最后一期数据的期号
                if (!IBoolNotAllEqualsLastFetch)
                {
                    IListAllWeiStatistics_120 = GetDataAllWeiStatistics_120(1);
                    IListMoTouType_130 = GetDataMoTouType_130();

                    if (ILastFetchPeriod < IListAllWeiStatistics_120[0].LongPeriod_001) 
                    {
                        ILastFetchPeriod = IListAllWeiStatistics_120[0].LongPeriod_001;                       
                    }
                    else 
                    {
                        Thread.Sleep(1000);
                    }                 
                }

                foreach (MoTouType_130 mt in IListMoTouType_130)
                {
                    if (mt.RunOverPeriod_011 < ILastFetchPeriod)
                    {
                        switch (mt.MoTouTypeID_001)
                        {
                            case 11: //20期遗失一天出现小于3期的。
                                Do_Type11_20Lost(mt);
                                break;
                            case 12://20期遗失短期内迅速涨4个或5个
                               
                                break;
                            case 13: //最热号买
                                break;
                            case 14://前面20期无长双长大长单长双

                                break;
                            case 15://前面出现长大长小 后面出长大长小怎么办？
                                break;
                            default:
                                break;
                        }
                        //更新T_130表数据
                        UpdateDataMoTouType_130(mt);
                    }
                    else if (mt.RunOverPeriod_011 == ILastFetchPeriod)
                    {
                        i++;
                    }
                }


                if (i == IListMoTouType_130.Count)
                {
                    IBoolNotAllEqualsLastFetch = false;
                    i = 0;
                }
                else 
                {
                    IBoolNotAllEqualsLastFetch = true;
                    i = 0;
                }
               
                Thread.Sleep(10);
            }
        }

        #region  Do --11-----20期遗失
        public static void Do_Type11_20Lost( MoTouType_130  mt) 
        { 
            //get 131表的内容当前未完成的记录   
            //得到132表内当前未完成的记录与下一期101表数据对比 ，判断正确与否
            //更新131，132表的数据
            List<MoTouServiceSingle_131> listMoTouServiceSingleNotComplete_131 = GetDataMoTouServiceSing_131(1,S04_ConstDefine.MT_Type_20LostSmall,mt.RunOverPeriod_011);

            if (listMoTouServiceSingleNotComplete_131.Count > 0)
            {
                //取101数据，判断是否命中
                List<PeriodDetail_101> listPeriodTemp = new List<PeriodDetail_101>();
                listPeriodTemp = GetListPeriodDetail_101(1, mt.RunOverPeriod_011);
                foreach(MoTouServiceSingle_131 ms in   listMoTouServiceSingleNotComplete_131)
                {
                     List<MoTouServiceDouble_132> listMoTouDouble_132 = new List<MoTouServiceDouble_132>();
                     listMoTouDouble_132 = GetDataMoTouServiceDouble_132(1, ms.MTKeyID_001);      
                }
            }
            else 
            {
                //启动新的判断                
                if (mt.RunOverPeriod_011 == 0) //第一次判断,取出小于3的期号
                {
                    #region
                    //从102取最早100条
                    IListLostAll_102 = GetDataLostAll_102(1,100);
                    if(IListLostAll_102.Count==100)
                    {                        
                        long firstStartPeriod = IListLostAll_102[IListLostAll_102.Count - 1].LongPeriod_001;
                        //得到120统计数据
                        IListAllWeiStatistics_120 = GetDataAllWeiStatistics_120(2, mt.RunOverPeriod_011, 20);
                        //找到最近的最小的20期遗失数
                        int min20lost = IListAllWeiStatistics_120.Min(p=>p.Remain20LostCount_025);                        
                        if(min20lost<=3)
                        {
                            long periodOfMin20Lost = IListAllWeiStatistics_120.Where(p => p.Remain20LostCount_025 == min20lost).First().LongPeriod_001;
                            mt.RunOverPeriod_011 = periodOfMin20Lost;     
                            
                        }
                    }
                    #endregion
                }
                else 
                {
                    IListAllWeiStatistics_120 = GetDataAllWeiStatistics_120(2,mt.RunOverPeriod_011,20);
                }       
            }    
        }
        #endregion

        #region 得到101表的新 的开奖数据
        /// <summary>
        /// 默认取当前期  0如有当前期，则取当前期，没有取最近一期,1为取这期的下一期
        /// </summary>
        /// <param name="PeriodNumber"></param>
        /// <param name="AType"></param>
        /// <returns></returns>
        public static List<PeriodDetail_101> GetListPeriodDetail_101(int AType, long PeriodNumber, long RecordNumber = -1)
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

        public static int GetCountFromTwoPeriod(int AType,long PeriodStart,long PeriodStop)
        {
            int countNum = 0;
            string strSQL = string.Empty;
            switch (AType)
            {
                case 1:
                    {
                        strSQL = String.Format("select count(*) as countnum  from T_101_{0} where C001>={1} AND  C001<={2} ",IStrYY,PeriodStart,PeriodStop);
                    }
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, strSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drNewRow in ds.Tables[0].Rows)
                {
                    countNum = IntParse(drNewRow["countnum"].ToString(),0);
                }
            }
            return countNum;
        }

        #endregion

        #region 得到 102表的数据
        public static List<LostAll_102> GetDataLostAll_102(int AType, long PeriodNumber)
        {
            List<LostAll_102> ListLostall_102 = new List<LostAll_102>(); ;
            String StrSQL = string.Empty;
            switch (AType)
            {
                case 0:  //得到当前期
                    StrSQL = string.Format("select top 1 * from T_102_{0} where C001={1} order by  C001 desc", IStrYY, PeriodNumber);
                    break;
                case 1: //得到102表最早的的100条数据
                    StrSQL = string.Format("select top {1} * from T_102_{0} order by C001  desc ", IStrYY, PeriodNumber); //这个实际为条数
                    break;
                case 2://得到前一期
                    StrSQL = string.Format("select top 1 * from T_102_{0} where C001<={1} order by  C001 desc", IStrYY, PeriodNumber);
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drNewRow in ds.Tables[0].Rows)
                {
                    LostAll_102 lostall_102 = new LostAll_102();
                    lostall_102.LongPeriod_001 = long.Parse(drNewRow["C001"].ToString());
                    lostall_102.DateNumber_002 = long.Parse(drNewRow["C002"].ToString());
                    lostall_102.ShortPeriod_003 = int.Parse(drNewRow["C003"].ToString());
                    lostall_102.Appear20Lost_004 = int.Parse(drNewRow["C004"].ToString());
                    lostall_102.Appear20MLost_005 = int.Parse(drNewRow["C005"].ToString());
                    lostall_102.Remain20Lost_006 = int.Parse(drNewRow["C006"].ToString());
                    lostall_102.Remain20MLost_007 = int.Parse(drNewRow["C007"].ToString());
                    lostall_102.Remain20DistinctNumber_008 = int.Parse(drNewRow["C008"].ToString());
                    lostall_102.Remain20PositionNum_009 = int.Parse(drNewRow["C009"].ToString());

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

                    lostall_102.IsThreeSame_060 = IntParse(drNewRow["C060"].ToString(), 0);
                    lostall_102.ThreeSameValue_061 = IntParse(drNewRow["C061"].ToString(), 0);
                    lostall_102.TS_Pre_Lost_062 = IntParse(drNewRow["C062"].ToString(), 0);
                    lostall_102.TS_Lost_063 = IntParse(drNewRow["C063"].ToString(), 0);

                    lostall_102.PreLost_101 = int.Parse(drNewRow["C101"].ToString());
                    lostall_102.PreLost_102 = int.Parse(drNewRow["C102"].ToString());
                    lostall_102.PreLost_103 = int.Parse(drNewRow["C103"].ToString());
                    lostall_102.PreLost_104 = int.Parse(drNewRow["C104"].ToString());
                    lostall_102.PreLost_105 = int.Parse(drNewRow["C105"].ToString());
                    lostall_102.Lost20_106 = int.Parse(drNewRow["C106"].ToString());
                    lostall_102.Lost20M_107 = int.Parse(drNewRow["C107"].ToString());
                    ListLostall_102.Add(lostall_102);
                }
            }
            return ListLostall_102;
        }
        #endregion

        #region  全位不分段 T_120_YY

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AType">1 得到最新一期的120表数据  2：得到小于等这期的特定期数据120表数据</param>
        /// <param name="ALongPeriod"></param>
        /// <param name="ACount"></param>
        /// <returns></returns>
        public static List<AllWeiStatistics_120> GetDataAllWeiStatistics_120(int AType, long ALongPeriod = -1, int ACount = -1)
        {
            List<AllWeiStatistics_120> listAllWeiStatistics = new List<AllWeiStatistics_120>();
            string StrSql = string.Empty;
            switch (AType)
            {
                case 1://得到最一新一期数据
                    StrSql = string.Format("select top 1 *  from T_120_{0}  order by C001 des ", IStrYY);
                    break;
                case 2:
                    StrSql = string.Format("select top {1} *  from T_120_{0} Where C001<={2} order by C001 des ", IStrYY, ALongPeriod, ACount);
                    break;
                default:
                    break;
            }
            try
            {
                #region

                DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSql);
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow drNewRow in ds.Tables[0].Rows)
                    {
                        AllWeiStatistics_120 AllWeiStatistics120 = new AllWeiStatistics_120();
                        AllWeiStatistics120.LongPeriod_001 = long.Parse(drNewRow["C001"].ToString());
                        AllWeiStatistics120.DateNumber_002 = long.Parse(drNewRow["C002"].ToString());
                        AllWeiStatistics120.ShortPeriod_003 = IntParse(drNewRow["C003"].ToString(), 0);
                        AllWeiStatistics120.StrAllHotNuberRate_5_004 = drNewRow["C004"].ToString();
                        AllWeiStatistics120.StrAllHotNuberRate_10_005 = drNewRow["C005"].ToString();
                        AllWeiStatistics120.StrAllHotNuberRate_15_006 = drNewRow["C006"].ToString();
                        AllWeiStatistics120.StrAllHotNuberRate_30_007 = drNewRow["C007"].ToString();
                        AllWeiStatistics120.StrAllHotNuberRate_80_008 = drNewRow["C008"].ToString();
                        AllWeiStatistics120.StrAllHotNuberRate_Back_009 = drNewRow["C009"].ToString();
                        AllWeiStatistics120.StrAllHotNuberRate_Back_010 = drNewRow["C010"].ToString();
                        AllWeiStatistics120.EO_Order_1_011 = IntParse(drNewRow["C011"].ToString(), 0);
                        AllWeiStatistics120.EO_Order_2_012 = IntParse(drNewRow["C012"].ToString(), 0);
                        AllWeiStatistics120.EO_Order_3_013 = IntParse(drNewRow["C013"].ToString(), 0);
                        AllWeiStatistics120.EO_Order_4_014 = IntParse(drNewRow["C014"].ToString(), 0);
                        AllWeiStatistics120.EO_Order_5_015 = IntParse(drNewRow["C015"].ToString(), 0);
                        AllWeiStatistics120.BS_Order_1_016 = IntParse(drNewRow["C016"].ToString(), 0);
                        AllWeiStatistics120.BS_Order_2_017 = IntParse(drNewRow["C017"].ToString(), 0);
                        AllWeiStatistics120.BS_Order_3_018 = IntParse(drNewRow["C018"].ToString(), 0);
                        AllWeiStatistics120.BS_Order_4_019 = IntParse(drNewRow["C019"].ToString(), 0);
                        AllWeiStatistics120.BS_Order_5_020 = IntParse(drNewRow["C020"].ToString(), 0);
                        AllWeiStatistics120.Appear20LostCount_021 = IntParse(drNewRow["C021"].ToString(), 0);
                        AllWeiStatistics120.Appear20LostSpan_022 = IntParse(drNewRow["C022"].ToString(), 0);
                        AllWeiStatistics120.Appear20MLostCount_023 = IntParse(drNewRow["C023"].ToString(), 0);
                        AllWeiStatistics120.Appear20MLostSpan_024 = IntParse(drNewRow["C024"].ToString(), 0);
                        AllWeiStatistics120.Remain20LostCount_025 = IntParse(drNewRow["C025"].ToString(), 0);
                        AllWeiStatistics120.Remain20MLostCount_026 = IntParse(drNewRow["C026"].ToString(), 0);
                        AllWeiStatistics120.Remain20DistinctNumber_027 = IntParse(drNewRow["C027"].ToString(), 0);
                        AllWeiStatistics120.Remain20PositionNum_028 = IntParse(drNewRow["C028"].ToString(), 0);
                        AllWeiStatistics120.AllWei5LostCount_029 = IntParse(drNewRow["C029"].ToString(), 0);
                        AllWeiStatistics120.AllWei5LostMax_030 = IntParse(drNewRow["C030"].ToString(), 0);
                        AllWeiStatistics120.AllRepickCount_031 = IntParse(drNewRow["C031"].ToString(), 0);
                        AllWeiStatistics120.AllRepickSpan_032 = IntParse(drNewRow["C032"].ToString(), 0);
                        AllWeiStatistics120.AllSpanCount_033 = IntParse(drNewRow["C033"].ToString(), 0);
                        AllWeiStatistics120.AllSpanSpan_034 = IntParse(drNewRow["C034"].ToString(), 0);
                        AllWeiStatistics120.AllAbsSpanCount_035 = IntParse(drNewRow["C035"].ToString(), 0);
                        AllWeiStatistics120.AllAbsSpanSpan_036 = IntParse(drNewRow["C036"].ToString(), 0);
                        AllWeiStatistics120.IsThreeSame_037 = IntParse(drNewRow["C037"].ToString(), 0);
                        AllWeiStatistics120.ThreeSameSpan_038 = IntParse(drNewRow["C038"].ToString(), 0);
                        AllWeiStatistics120.TrendRepick_039 = IntParse(drNewRow["C039"].ToString(), 0);
                        AllWeiStatistics120.TrendRepickContinue_040 = IntParse(drNewRow["C040"].ToString(), 0);
                        AllWeiStatistics120.TrendAdd_041 = IntParse(drNewRow["C041"].ToString(), 0);
                        AllWeiStatistics120.TrendAddContinue_042 = IntParse(drNewRow["C042"].ToString(), 0);
                        AllWeiStatistics120.TrendSub_043 = IntParse(drNewRow["C043"].ToString(), 0);
                        AllWeiStatistics120.TrendSubContinue_044 = IntParse(drNewRow["C044"].ToString(), 0);
                        AllWeiStatistics120.TrendSwing_045 = IntParse(drNewRow["C045"].ToString(), 0);
                        AllWeiStatistics120.TrendSwingContinue_046 = IntParse(drNewRow["C046"].ToString(), 0);
                        AllWeiStatistics120.ContinueSingle4Count_047 = IntParse(drNewRow["C047"].ToString(), 0);
                        AllWeiStatistics120.ContinueSingle6Count_048 = IntParse(drNewRow["C048"].ToString(), 0);
                        AllWeiStatistics120.ContinueSingle9Count_049 = IntParse(drNewRow["C049"].ToString(), 0);
                        AllWeiStatistics120.ContinueSingleMaxHot_050 = IntParse(drNewRow["C050"].ToString(), 0);
                        AllWeiStatistics120.MaxBigContinue_051 = IntParse(drNewRow["C051"].ToString(), 0);
                        AllWeiStatistics120.MaxSmallContinue_052 = IntParse(drNewRow["C052"].ToString(), 0);
                        AllWeiStatistics120.MaxEvenContinue_053 = IntParse(drNewRow["C053"].ToString(), 0);
                        AllWeiStatistics120.MaxOddContinue_054 = IntParse(drNewRow["C054"].ToString(), 0);
                        AllWeiStatistics120.AllBigContinue_055 = IntParse(drNewRow["C055"].ToString(), 0);
                        AllWeiStatistics120.AllSmallContinue_056 = IntParse(drNewRow["C056"].ToString(), 0);
                        listAllWeiStatistics.Add(AllWeiStatistics120);
                    }
                }
                #endregion
            }
            catch (Exception e)
            {

                FileLog.WriteInfo("GetDataAllWeiStatistics_120", " err :" + e.Message.ToString());
            }

            return listAllWeiStatistics;
        }
        #endregion

        #region  模型类型表T_130_19
        public static List<MoTouType_130> GetDataMoTouType_130()
        {
            List<MoTouType_130> listMoTouType = new List<MoTouType_130>();
            string StrSql = String.Format("select  *  from T_130_{0} ", IStrYY);
            try
            {
                DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSql);
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow drNewRow in ds.Tables[0].Rows)
                    {
                        MoTouType_130 mt = new MoTouType_130();
                        mt.MoTouTypeID_001 = IntParse(drNewRow["C001"].ToString(), 0);
                        mt.StrMoTouDesc_002 = drNewRow["C002"].ToString();
                        mt.BuyType_003 = IntParse(drNewRow["C003"].ToString(), 0);
                        mt.Per_004 = Decimal.Parse(drNewRow["C004"].ToString());
                        mt.StopTime_005 = IntParse(drNewRow["C005"].ToString(), 0);
                        mt.IsFanBei_006 = IntParse(drNewRow["C006"].ToString(), 0);
                        mt.IsRateAndAbs_007 = IntParse(drNewRow["C007"].ToString(), 0);
                        mt.RateAndAbs_008 = DecimalParse(drNewRow["C008"].ToString(), 0);
                        mt.StartMoney_009 = DecimalParse(drNewRow["C009"].ToString(), 0);
                        mt.TotalMoney_010 = DecimalParse(drNewRow["C010"].ToString(), 0);
                        mt.RunOverPeriod_011 = long.Parse(drNewRow["C011"].ToString());
                        mt.FirstTimeStartPeriod_012 = long.Parse(drNewRow["C012"].ToString());
                        mt.TimeAftretStartPeriod_013 = IntParse(drNewRow["C013"].ToString(), 0);
                        mt.LastLostMoney_014 = DecimalParse(drNewRow["C014"].ToString(), 0);
                        mt.LastStartRulePeriod_015= LongParse(drNewRow["C015"].ToString(),0);
                        listMoTouType.Add(mt);
                    }
                }
            }
            catch (Exception e)
            {

                FileLog.WriteInfo("GetDataMoTouType_130()", e.Message.ToString());
            }

            return listMoTouType;
        }

        public static bool UpdateDataMoTouType_130(MoTouType_130 AMoTou)
        {
            bool flag = true;
            string strSQL = string.Empty;
            try
            {
                strSQL = string.Format("select * from T_130_{0} where C001={1} ", AMoTou.MoTouTypeID_001);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSQL);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}", AMoTou.MoTouTypeID_001)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}", AMoTou.MoTouTypeID_001)).First() : null;

                if (drCurrent != null) //更新
                {
                    drCurrent.BeginEdit();

                    drCurrent["C001"] = AMoTou.MoTouTypeID_001;
                    drCurrent["C002"] = AMoTou.StrMoTouDesc_002;
                    drCurrent["C003"] = AMoTou.BuyType_003;
                    drCurrent["C004"] = AMoTou.Per_004;
                    drCurrent["C005"] = AMoTou.StopTime_005;
                    drCurrent["C006"] = AMoTou.IsFanBei_006;
                    drCurrent["C007"] = AMoTou.IsRateAndAbs_007;
                    drCurrent["C008"] = AMoTou.RateAndAbs_008;
                    drCurrent["C009"] = AMoTou.StartMoney_009;
                    drCurrent["C010"] = AMoTou.TotalMoney_010;
                    drCurrent["C011"] = AMoTou.RunOverPeriod_011;
                    drCurrent["C012"] = AMoTou.FirstTimeStartPeriod_012;
                    drCurrent["C013"] = AMoTou.TimeAftretStartPeriod_013;
                    drCurrent["C014"] = AMoTou.LastLostMoney_014;
                    drCurrent["C015"] = AMoTou.LastStartRulePeriod_015;

                    drCurrent.EndEdit();
                }
                else
                {
                    DataRow drNewRow = objDataSet.Tables[0].NewRow();
                    drNewRow["C001"] = AMoTou.MoTouTypeID_001;
                    drNewRow["C002"] = AMoTou.StrMoTouDesc_002;
                    drNewRow["C003"] = AMoTou.BuyType_003;
                    drNewRow["C004"] = AMoTou.Per_004;
                    drNewRow["C005"] = AMoTou.StopTime_005;
                    drNewRow["C006"] = AMoTou.IsFanBei_006;
                    drNewRow["C007"] = AMoTou.IsRateAndAbs_007;
                    drNewRow["C008"] = AMoTou.RateAndAbs_008;
                    drNewRow["C009"] = AMoTou.StartMoney_009;
                    drNewRow["C010"] = AMoTou.TotalMoney_010;
                    drNewRow["C011"] = AMoTou.RunOverPeriod_011;
                    drNewRow["C012"] = AMoTou.FirstTimeStartPeriod_012;
                    drNewRow["C013"] = AMoTou.TimeAftretStartPeriod_013;
                    drNewRow["C014"] = AMoTou.LastLostMoney_014;
                    drNewRow["C015"] = AMoTou.LastStartRulePeriod_015;

                    objDataSet.Tables[0].Rows.Add(drNewRow);
                }

                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
                FileLog.WriteInfo("UpdateDataMoTouType_130() ", "Success :" + AMoTou.MoTouTypeID_001);

            }
            catch (Exception e)
            {
                FileLog.WriteInfo("UpdateDataMoTouType_130()", "err :" + e.Message.ToString());
            }
            return flag;
        }
        #endregion

        #region T_131_19单位投注表
        public  static List<MoTouServiceSingle_131> GetDataMoTouServiceSing_131(int AType ,int ATypeID,long APeriod=-1) 
        {
            List<MoTouServiceSingle_131> listMoTou131=new List<MoTouServiceSingle_131>();
            String StrSQL = string.Empty;
            switch (AType)
            {
                case 1: //未完成的
                    {
                        StrSQL = string.Format("select *  from T_131_{0} where  C001<={1} AND C002={2} And C007=0 order by  C001 desc", IStrYY, APeriod,ATypeID);
                    }
                    break;
                case 2:
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drCurrent in ds.Tables[0].Rows)
                {
                    MoTouServiceSingle_131 motou131 = new MoTouServiceSingle_131();
                    motou131.MTKeyID_001 = LongParse(drCurrent["C001"].ToString(), 0);
                    motou131.TypeID_002 = IntParse(drCurrent["C002"].ToString(), 0);
                    motou131.Position_003 = IntParse(drCurrent["C003"].ToString(), 0);
                    motou131.PositionValue_004 = IntParse(drCurrent["C004"].ToString(), 0);
                    motou131.TouNum_005 = IntParse(drCurrent["C005"].ToString(), 0);
                    motou131.StartPeriod_006 = LongParse(drCurrent["C006"].ToString(), 0);
                    motou131.CurrentPeriod_007 = LongParse(drCurrent["C007"].ToString(), 0);
                    motou131.IsComplete_008 = IntParse(drCurrent["C008"].ToString(), 0);
                    motou131.TotalBuy_010 = DecimalParse(drCurrent["C010"].ToString(), 0);
                    motou131.TotalInCome_011 = DecimalParse(drCurrent["C011"].ToString(), 0);
                    motou131.TotalEarn_012 = DecimalParse(drCurrent["C012"].ToString(), 0);
                    motou131.Buy1_021 = DecimalParse(drCurrent["C021"].ToString(), 0);
                    motou131.Buy2_022 = DecimalParse(drCurrent["C022"].ToString(), 0);
                    motou131.Buy3_023 = DecimalParse(drCurrent["C023"].ToString(), 0);
                    motou131.Buy4_024 = DecimalParse(drCurrent["C024"].ToString(), 0);
                    motou131.Buy5_025 = DecimalParse(drCurrent["C025"].ToString(), 0);
                    motou131.Buy6_026 = DecimalParse(drCurrent["C026"].ToString(), 0);
                    motou131.Buy7_027 = DecimalParse(drCurrent["C027"].ToString(), 0);
                    motou131.Buy8_028 = DecimalParse(drCurrent["C028"].ToString(), 0);
                    motou131.Buy9_029 = DecimalParse(drCurrent["C029"].ToString(), 0);
                    motou131.Buy10_030 = DecimalParse(drCurrent["C030"].ToString(), 0);
                    listMoTou131.Add(motou131);
                }
            }

            return listMoTou131;
        }

        public bool UpdateMoTouServiceSingle_131(List<MoTouServiceSingle_131> AListMoTouServiceSingle131) 
        {
            bool flag = true;
            if (AListMoTouServiceSingle131.Count == 0) { return true; }
            StringBuilder sb = new StringBuilder();
            List<long> listlongPeriod = new List<long>();
            foreach (MoTouServiceSingle_131 cbs in AListMoTouServiceSingle131)
            {
                if (!listlongPeriod.Contains(cbs.MTKeyID_001))
                {
                    listlongPeriod.Add(cbs.MTKeyID_001);
                    sb.Append(cbs.MTKeyID_001.ToString()).Append(",");
                }

            }
            String periods = sb.ToString().TrimEnd(',').ToString();
            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_131_{0} where C001 in ({1})  ", IStrYY, periods);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                foreach (MoTouServiceSingle_131 ss in AListMoTouServiceSingle131)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}  ", ss.MTKeyID_001)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}  ", ss.MTKeyID_001)).First() : null;
                    if (drCurrent != null) //更新
                    {
                        drCurrent.BeginEdit();
                        drCurrent["C001"] = ss.MTKeyID_001;
                        drCurrent["C002"] = ss.TypeID_002;
                        drCurrent["C003"] = ss.Position_003;
                        drCurrent["C004"] = ss.PositionValue_004;
                        drCurrent["C005"] = ss.TouNum_005;
                        drCurrent["C006"] = ss.StartPeriod_006;
                        drCurrent["C007"] = ss.CurrentPeriod_007;
                        drCurrent["C008"] = ss.IsComplete_008;
                        drCurrent["C010"] = ss.TotalBuy_010;
                        drCurrent["C011"] = ss.TotalInCome_011;
                        drCurrent["C012"] = ss.TotalEarn_012;
                        drCurrent["C021"] = ss.Buy1_021;
                        drCurrent["C022"] = ss.Buy2_022;
                        drCurrent["C023"] = ss.Buy3_023;
                        drCurrent["C024"] = ss.Buy4_024;
                        drCurrent["C025"] = ss.Buy5_025;
                        drCurrent["C026"] = ss.Buy6_026;
                        drCurrent["C027"] = ss.Buy7_027;
                        drCurrent["C028"] = ss.Buy8_028;
                        drCurrent["C029"] = ss.Buy9_029;
                        drCurrent["C030"] = ss.Buy10_030;
                        drCurrent.EndEdit();
                    }
                    else //添加新行
                    {
                        DataRow drNewRow = objDataSet.Tables[0].NewRow();
                        drNewRow["C001"] = ss.MTKeyID_001;
                        drNewRow["C002"] = ss.TypeID_002;
                        drNewRow["C003"] = ss.Position_003;
                        drNewRow["C004"] = ss.PositionValue_004;
                        drNewRow["C005"] = ss.TouNum_005;
                        drNewRow["C006"] = ss.StartPeriod_006;
                        drNewRow["C007"] = ss.CurrentPeriod_007;
                        drNewRow["C008"] = ss.IsComplete_008;
                        drNewRow["C010"] = ss.TotalBuy_010;
                        drNewRow["C011"] = ss.TotalInCome_011;
                        drNewRow["C012"] = ss.TotalEarn_012;
                        drNewRow["C021"] = ss.Buy1_021;
                        drNewRow["C022"] = ss.Buy2_022;
                        drNewRow["C023"] = ss.Buy3_023;
                        drNewRow["C024"] = ss.Buy4_024;
                        drNewRow["C025"] = ss.Buy5_025;
                        drNewRow["C026"] = ss.Buy6_026;
                        drNewRow["C027"] = ss.Buy7_027;
                        drNewRow["C028"] = ss.Buy8_028;
                        drNewRow["C029"] = ss.Buy9_029;
                        drNewRow["C030"] = ss.Buy10_030;
                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }
                }
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();

                FileLog.WriteInfo("UpdateMoTouServiceSingle_131() ", "Success :");

            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdateMoTouServiceSingle_131()  Err ", e.Message );
                return flag = false;
            }
            #endregion
            return flag;
        }
        #endregion

        #region T_132_19 复位投注表
        public static   List<MoTouServiceDouble_132> GetDataMoTouServiceDouble_132(int AType, long MTKeyID_001 = -1) 
        {
            List<MoTouServiceDouble_132> listMoTouDouble_132 = new List<MoTouServiceDouble_132>();    
            String StrSQL = string.Empty;
            switch (AType)
            {
                case 1: //未完成的
                    {
                        StrSQL = string.Format("select *  from T_132_{0} where  C001={1} order by  C001 desc", IStrYY, MTKeyID_001);
                    }
                    break;
                case 2:
                    break;
                default:
                    break;
            }
            DataSet ds = DBHelp.DbHelperSQL.GetDataSet(ISqlConnect, StrSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drCurrent in ds.Tables[0].Rows)
                {
                    MoTouServiceDouble_132 motou132 = new MoTouServiceDouble_132();
                    motou132.MTKeyID_001 =LongParse( drCurrent["C001"].ToString(),0);
                    motou132.TouNum_002 =IntParse( drCurrent["C002"].ToString(),0);
                    motou132.LongPeriod_003 =LongParse( drCurrent["C003"].ToString(),0);
                    motou132.PeriodDate_004 =LongParse( drCurrent["C004"].ToString(),0);
                    motou132.ShortPeriod_005 = IntParse(drCurrent["C005"].ToString(),0);
                    motou132.StrTypeDesc_006 = drCurrent["C006"].ToString();
                    motou132.StrTouPosition_007 = drCurrent["C007"].ToString();
                    motou132.EveryBuyMoney_008 =DecimalParse( drCurrent["C008"].ToString(),0);
                    motou132.TotalBuyNumber_009 =IntParse( drCurrent["C009"].ToString(),0);
                    motou132.TotalInCome_010 =DecimalParse( drCurrent["C010"].ToString(),0);
                    motou132.StrGeBuyNum_011 = drCurrent["C011"].ToString();
                    motou132.StrGeGoalNum_012 = drCurrent["C012"].ToString();
                    motou132.StrShiBuyNum_021 = drCurrent["C021"].ToString();
                    motou132.StrShiGoalNum_022 = drCurrent["C022"].ToString();
                    motou132.StrBaiBuyNum_031 = drCurrent["C031"].ToString();
                    motou132.StrBaiGoalNum_032 = drCurrent["C032"].ToString();
                    motou132.StrQianBuyNum_041 = drCurrent["C041"].ToString();
                    motou132.StrQianGoalNum_042 = drCurrent["C042"].ToString();
                    motou132.StrWanBuyNum_051 = drCurrent["C051"].ToString();
                    motou132.StrWanGoalNum_052 = drCurrent["C052"].ToString();

                    listMoTouDouble_132.Add(motou132);
                }
            }

           return   listMoTouDouble_132;
        }

        public bool UpdateMoTouServiceDouble_132(List<MoTouServiceDouble_132> AListMoTouDouble_132) 
        {
            bool flag = true;
            if (AListMoTouDouble_132.Count == 0) { return true; }
            StringBuilder sb = new StringBuilder();
            List<long> listlongPeriod = new List<long>();
            foreach (MoTouServiceDouble_132 cbs in AListMoTouDouble_132)
            {
                if (!listlongPeriod.Contains(cbs.MTKeyID_001))
                {
                    listlongPeriod.Add(cbs.MTKeyID_001);
                    sb.Append(cbs.MTKeyID_001.ToString()).Append(",");
                }

            }
            String periods = sb.ToString().TrimEnd(',').ToString();
            #region
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;
                strSql = string.Format("Select * from T_132_{0} where C001 in ({1})  ", IStrYY, periods);

                objConn = DbHelperSQL.GetConnection(ISqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                foreach (MoTouServiceDouble_132 ss in AListMoTouDouble_132)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}  AND C002={1} ", ss.MTKeyID_001, ss.TouNum_002)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}  AND C002={1} ", ss.MTKeyID_001, ss.TouNum_002)).First() : null;
                    if (drCurrent != null) //更新
                    {
                        drCurrent.BeginEdit();
                        drCurrent["C001"] = ss.MTKeyID_001;
                        drCurrent["C002"] = ss.TouNum_002;
                        drCurrent["C003"] = ss.LongPeriod_003;
                        drCurrent["C004"] = ss.PeriodDate_004;
                        drCurrent["C005"] = ss.ShortPeriod_005;
                        drCurrent["C006"] = ss.StrTypeDesc_006;
                        drCurrent["C007"] = ss.StrTouPosition_007;
                        drCurrent["C008"] = ss.EveryBuyMoney_008;
                        drCurrent["C009"] = ss.TotalBuyNumber_009;
                        drCurrent["C010"] = ss.TotalInCome_010;
                        drCurrent["C011"] = ss.StrGeBuyNum_011;
                        drCurrent["C012"] = ss.StrGeGoalNum_012;
                        drCurrent["C021"] = ss.StrShiBuyNum_021;
                        drCurrent["C022"] = ss.StrShiGoalNum_022;
                        drCurrent["C031"] = ss.StrBaiBuyNum_031;
                        drCurrent["C032"] = ss.StrBaiGoalNum_032;
                        drCurrent["C041"] = ss.StrQianBuyNum_041;
                        drCurrent["C042"] = ss.StrQianGoalNum_042;
                        drCurrent["C051"] = ss.StrWanBuyNum_051;
                        drCurrent["C052"] = ss.StrWanGoalNum_052;
                        drCurrent.EndEdit();
                    }
                    else //添加新行
                    {
                        DataRow drNewRow = objDataSet.Tables[0].NewRow();
                        drNewRow["C001"] = ss.MTKeyID_001;
                        drNewRow["C002"] = ss.TouNum_002;
                        drNewRow["C003"] = ss.LongPeriod_003;
                        drNewRow["C004"] = ss.PeriodDate_004;
                        drNewRow["C005"] = ss.ShortPeriod_005;
                        drNewRow["C006"] = ss.StrTypeDesc_006;
                        drNewRow["C007"] = ss.StrTouPosition_007;
                        drNewRow["C008"] = ss.EveryBuyMoney_008;
                        drNewRow["C009"] = ss.TotalBuyNumber_009;
                        drNewRow["C010"] = ss.TotalInCome_010;
                        drNewRow["C011"] = ss.StrGeBuyNum_011;
                        drNewRow["C012"] = ss.StrGeGoalNum_012;
                        drNewRow["C021"] = ss.StrShiBuyNum_021;
                        drNewRow["C022"] = ss.StrShiGoalNum_022;
                        drNewRow["C031"] = ss.StrBaiBuyNum_031;
                        drNewRow["C032"] = ss.StrBaiGoalNum_032;
                        drNewRow["C041"] = ss.StrQianBuyNum_041;
                        drNewRow["C042"] = ss.StrQianGoalNum_042;
                        drNewRow["C051"] = ss.StrWanBuyNum_051;
                        drNewRow["C052"] = ss.StrWanGoalNum_052;
                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }
                }
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();

                FileLog.WriteInfo("UpdateMoTouServiceSingle_131() ", "Success :");

            }
            catch (Exception e)
            {
                FileLog.WriteError("UpdateMoTouServiceSingle_131()  Err ", e.Message);
                return flag = false;
            }
            #endregion
            return flag;
        }

        #endregion


        #region  公用生成主键
        public static long GeneralMainKey(string strPeriodNumber, string strMoTouType )
        {
            long mainKey = 0;
            try
            {
                DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@AInParam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInParam02",MssqlDataType.Varchar,2),
                            MssqlOperation.GetDbParameter("@AOutParam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutErrorNumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@AOutErrorString",MssqlDataType.NVarchar,4000)
                        };
                mssqlParameters[0].Value = strPeriodNumber;
                mssqlParameters[1].Value = strMoTouType;
                mssqlParameters[2].Value = "";
                mssqlParameters[3].Value = 0;
                mssqlParameters[4].Value = "";
                mssqlParameters[2].Direction = ParameterDirection.Output;
                mssqlParameters[3].Direction = ParameterDirection.Output;
                mssqlParameters[4].Direction = ParameterDirection.Output;
                OperationReturn optReturn = MssqlOperation.ExecuteStoredProcedure(ISqlConnect, "P_001",
                   mssqlParameters);
                if (mssqlParameters[3].Value.ToString().Equals("0"))
                {
                    string strKey = mssqlParameters[2].Value.ToString();
                    mainKey = long.Parse(strKey);
                }
                else 
                {
                    FileLog.WriteInfo("GeneralMainKey() err",mssqlParameters[4].Value.ToString());
                }               
            }
            catch (Exception  ex)
            {
                FileLog.WriteInfo("GeneralMainKey() err", ex.Message.ToString());
            }
            return mainKey;
        }
        #endregion

        #region  服务启停
        public void MoTouStaticsStartup()
        {
            FileLog.WriteInfo("LostDataStatistics", "LostDataStatisticsStartup()");
            IBoolIsThreadMoTouWorking = false;
            if (IThreadDoMoTou != null)
            {
                IThreadDoMoTou.Abort();
            }
            IThreadDoMoTou = new Thread(new ParameterizedThreadStart(MoTouStatistics.AutoDoMoTou));
            IBoolIsThreadMoTouWorking = true;
            IThreadDoMoTou.Start(this);
            FileLog.WriteInfo("LostDataStatistics()", "LostDataStatisticsStartup() Start");
        
        }
        public void MoTouStaticsStop() 
        {
            FileLog.WriteInfo("LostDataStatistics ", "LostDataStatisticsStop() start");
            IBoolIsThreadMoTouWorking = false;
            if (IThreadDoMoTou != null)
            {
                IThreadDoMoTou.Abort();
                IThreadDoMoTou = null;
            }

        }
        #endregion

        #region  公用方法
        static bool ExecuteListSQL(List<String> AListStringSQL)
        {
            bool flag = true;
            StringBuilder sb = new StringBuilder();
            if (AListStringSQL != null && AListStringSQL.Count > 0)
            {
                foreach (String ss in AListStringSQL)
                {
                    if ((sb.Length + ss.Length) < 8000)
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
            string strOldStop = OldValue.ToString().Substring(0, 8) + "059";
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

        public static int IntParse(string str, int defaultValue)
        {
            int outRet = defaultValue;
            if (!int.TryParse(str, out outRet))
            {
                outRet = defaultValue;
            }

            return outRet;
        }

        public static double DoubleParse(string str, double defaultValue)
        {
            double outRet = defaultValue;
            double.TryParse(str, out outRet);

            return outRet;
        }

        public static decimal DecimalParse(string str, decimal defaultValue) 
        {
            decimal outRet = defaultValue;
            decimal.TryParse(str, out outRet);

            return outRet;
        }

        public static long LongParse(string str, long defaultValue)
        {
            long outRet = defaultValue;
            long.TryParse(str, out outRet);

            return outRet;
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
    }
}
