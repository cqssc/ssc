using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public class BigSmallHot_105
    {
        /// <summary>
        /// 期数
        /// </summary>
        public long LongPeriod_001 {get;set; }

        //日期的前6位
        public Int64 DateNumber_002 { get; set; }

        //数字期
        public int ShortPeriod_003 { set; get; }

        public int PositionType_004 { set; get; }

        public int SpliteType_005{ get; set; }

        public int BigSmallOddEvenType_006 { get; set; } // 1 大 2 小 3 单  4双
        //该段内出现个数       
        public int NumberCount_007 { get; set; }
        //除出来的热度值
        public decimal HotValue_008 { get; set; }
    }
}
