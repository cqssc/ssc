using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public class SingleAnalysis_103
    {
        /// <summary>
        /// 期数
        /// </summary>
        public long LongPeriod_001{  get; set;  }
        //日期的前6位
        public Int64 DateNumber_002 { get; set; }
        //短期数
        public int ShortPeriod_003 { set; get; }

        public int PositionType_004 { get; set; }

        public int PositionVale_005 { get; set; }

        public int BigOrSmall_006 { get; set; }

        public int EvenODD_007 { get; set; }

        public int LostValue_008 { get; set; }

        public int LostEvenODDOrderNum_009 { get; set; }

        public int LostAllOrderNum_010 { get; set; }

        public int OrderInTen_011 { get; set; }

        public int OrderInTwenty_012 { get; set; }

        public int OrderInFourty_013 { get; set; }

        public int OrderInOneDay_014 { get; set; }

        public int OrderInTwoDay_015 { get; set; }

        public int OrderInThreeDay_016 { get; set; }

        public int OrderInFourDay_017 { get; set; }

        public int Remain20Num_018 { set; get; }

        public int RemainEven20Num_019 { set; get; }

        public int RemainOdd20Num_020 { set; get; }

       
        public int IsAppear20_021 { set; get; }
        public int IsAppear20M_022 { set; get; }
        public int IsThree20InSingle_023 { set; get; }

        public int IsInTwoSame_024 { set; get; }
        public int IsInThreeSame_025 { set; get; }
        public int IsInFourSame_026 { set; get; }
        public int IsInFiveSame_027 { set; get; }
        public int IsBigCon_028 { set; get; }
        public int BigConDetail_029 { set; get; }
        public int IsSmallCon_030 { set; get; }
        public int SmallConDetail_031 { set; get; }
        public int IsEvenCon_032 { set; get; }
        public int EvenConDetail_033 { set; get; }
        public int IsOddCon_034 { set; get; }
        public int OddConDetail_035 { set; get; }
        public int IsRepick_036 { set; get; }
        public int RepickDetail_037 { set; get; }
        public int IsThreeRepick_038 { set; get; }
        public int IsFourRepick_039 { set; get; }
        public int IsTurn_040 { set; get; }
        public int IsSpanOne_041 { set; get; }
        public int IsTwoConRepick_042 { set; get; }
        public int IsThreeConRepick_043 { set; get; }
        public int IsFourPile_044 { set; get; }
        public int IsFivePile_045 { set; get; }
        public int IsTread_046 { set; get; }
        public int TreadDetail_047 { set; get; }
        public int IsTreadAddOne_048 { set; get; }
        public int TreadAddOneDetail_049 { set; get; }
        public int IsTreadAddTwo_050 { set; get; }

        //public int Is38223_051 { set; get; }
        //public int Is32283_052 { set; get; }
        //public int Is8288_053 { set; get; }      
        //public int Is8828_054 { set; get; }  
        //public int IsXian_055 { set; get; }       
        //public int IsFanXian_056 { set; get; }      
        //public int IsAddOneOriginal_057 { set; get; }      
        //public int IsSubOneOriginal_058 { set; get; }      
        //public int IsBigConOriginalBig_059 { set; get; }       
        //public int IsSmallConOriginalSmall_060 { set; get; }      
        //public int IsEvenConOriginalEven_061{ set; get; }  
        //public int IsOddConOriginalOdd_062 { set; get; }

        //public int IsThree20InSingleAppear_024 { set; get; }

        //public int Is38223Alarm_051 { set; get; }
        //public int Is32283Alarm_053 { set; get; }
        //public int Is8288Alarm_055 { set; get; }
        //public int Is8828Alarm_057 { set; get; }
        //public int IsXianAlarm_059 { set; get; }
        //public int IsFanXianAlarm_061 { set; get; }
        //public int IsAddOneOriginalAlarm_063 { set; get; }
        //public int IsSubOneOriginalAlarm_065 { set; get; } 
        //public int IsBigConOriginalBigAlarm_067 { set; get; }
        //public int IsSmallConOriginalSmallAlarm_069 { set; get; }
        //public int IsEvenConOriginalEvenAlarm_071 { set; get; }
        //public int IsOddConOriginalOddAlarm_073 { set; get; }
    }
}
