using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public  class AllWeiStatistics_120
    {
        /// <summary>
        /// 期数
        /// </summary>
        public long LongPeriod_001 { get; set; }
        //日期的前6位
        public Int64 DateNumber_002 { get; set; }
        //数字期
        public int ShortPeriod_003 { set; get; }

        public String StrAllHotNuberRate_5_004 { set; get; }
        public String StrAllHotNuberRate_10_005 { set; get; }
        public String StrAllHotNuberRate_15_006 { set; get; }
        public String StrAllHotNuberRate_30_007 { set; get; }
        public String StrAllHotNuberRate_80_008 { set; get; }
        public String StrAllHotNuberRate_Back_009 { set; get; }
        public String StrAllHotNuberRate_Back_010 { set; get; }

        public int EO_Order_1_011 { set; get; }  //个
        public int EO_Order_2_012 { set; get; }  
        public int EO_Order_3_013 { set; get; }  
        public int EO_Order_4_014 { set; get; }  
        public int EO_Order_5_015 { set; get; }  //万

        public int BS_Order_1_016 { set; get; }  //个
        public int BS_Order_2_017 { set; get; }
        public int BS_Order_3_018 { set; get; }
        public int BS_Order_4_019 { set; get; }
        public int BS_Order_5_020 { set; get; }  //万


        public int Appear20LostCount_021 { set; get; }
        public int Appear20LostSpan_022 { set; get; }
        public int Appear20MLostCount_023 { set; get; }
        public int Appear20MLostSpan_024 { set; get; }
        public int Remain20LostCount_025 { set; get; }
        public int Remain20MLostCount_026 { set; get; }
        public int Remain20DistinctNumber_027 { set; get; }
        public int Remain20PositionNum_028 { set; get; }
        public int AllWei5LostCount_029 { set; get; }
        public int AllWei5LostMax_030 { set; get; }
        public int AllRepickCount_031 { set; get; }
        public int AllRepickSpan_032 { set; get; }
        public int AllSpanCount_033 { set; get; }
        public int AllSpanSpan_034 { set; get; }
        public int AllAbsSpanCount_035 { set; get; }
        public int AllAbsSpanSpan_036 { set; get; }
        public int IsThreeSame_037 { set; get; }
        public int ThreeSameSpan_038 { set; get; }

        public int TrendRepick_039 { set; get; }
        public int TrendRepickContinue_040 { set; get; }
        public int TrendAdd_041 { set; get; }
        public int TrendAddContinue_042 { set; get; }
        public int TrendSub_043 { set; get; }
        public int TrendSubContinue_044 { set; get; }
        public int TrendSwing_045 { set; get; }
        public int TrendSwingContinue_046 { set; get; }

        public int ContinueSingle4Count_047 { set; get; }
        public int ContinueSingle6Count_048 { set; get; }
        public int ContinueSingle9Count_049 { set; get; }
        public int ContinueSingleMaxHot_050  { set; get; }


        public int MaxBigContinue_051 { set; get; }
        public int MaxSmallContinue_052 { set; get; }
        public int MaxEvenContinue_053 { set; get; }
        public int MaxOddContinue_054 { set; get; }
        public int AllBigContinue_055 { set; get; }
        public int AllSmallContinue_056 { set; get; }


      
    }
}
