using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    //走势遗漏表
     public   class ContinueTrend_103
    {
        /// <summary>
        /// 期数
        /// </summary>
        public long LongPeriod_001 { get; set; }
        //日期的前6位
        public Int64 DateNumber_002 { get; set; }
        //数字期
        public int ShortPeriod_003 { set; get; }

        public int PositionType_004 { get; set; }

        public int Repick_005 { get; set; }

        public int Swing_006 { get; set; }

        public int AddOrSub_007 { get; set; }

        public int Other_008 { get; set; }

        public int ContinueValue_009 { get; set; }

        public int SwingValue_010 { get; set; }

        //public int TrendType_005 { get; set; }

        //public int IsAppear_006 { get; set; }

        //public int LostValue_007 { get; set; }

        //public int ContinueValue_008 { get; set; }

        //public int SwingValue_009 { get; set; }

        //public decimal AvgContinue_010 { get; set; }

        //public decimal LiSanContinue_011 { get; set; }


    }
}
