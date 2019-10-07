using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public class SingleAnalysis_106
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



        public int IsTwoSameOne_006 { get; set; }
        public int IsThreeSameOne_007 { get; set; }

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

        public int IsInPair_024 { set; get; }
        public int IsInThreeSame_025 { set; get; }
        public int IsInFourSame_026 { set; get; }
        public int IsInFiveSame_027 { set; get; }

        public int IsFourPile_028 { set; get; }
        public int IsFivePile_029 { set; get; }
    }
}
