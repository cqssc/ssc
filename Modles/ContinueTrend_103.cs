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

        public int TrendType_005 { get; set; }

        public int ContinueValue_006 { get; set; }

        public int IsComplete_007 { get; set; }

    }
}
