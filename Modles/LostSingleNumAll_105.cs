using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
     public class LostSingleNumAll_105
    {
        /// <summary>
        /// 期数
        /// </summary>
        public long LongPeriod_001 { get; set; }
        //日期的前6位
        public Int64 DateNumber_002 { get; set; }
        //短期数
        public int ShortPeriod_003 { set; get; }

         //单个数字0~9
        public int SingleNum_004{ set; get; }

        public long NotAppearPeriodStart_005 { set; get; }

        public int  LostSpan_006 { set; get; }

        public int AppearNumCount_007 { set; get; }  //用于判定是否出现

        public int IsComplete_008 { set; get; }

        public long Later1Period_009 { set; get; }

        public int LaterAppearNum_010 { set; get; }

        public long Later2Period_011 { set; get; }
        public int Later2AppearNum_012 { set; get; }

        public long Later3Period_013 { set; get; }
        public int Later3AppearNum_014 { set; get; }

        public long Later4Period_015 { set; get; }
        public int Later4AppearNum_016 { set; get; }


        public long Later5Period_017 { set; get; }
        public int Later5AppearNum_018 { set; get; }


    }
}
