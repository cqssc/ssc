using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public class ContinueBigSmall_111
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

        public int BigSmallOrEvenOddType_005 { get; set; }  // 1(大 小) 2(单 双)

        public int BigOrEven_006 { get; set; } //当C005为1 时C006为大   为2时 C006为单

        public int SmallOrOdd_007 { get; set; }  //当C005为 时C006为小   为2时 C006为 双

        public int ContinueValue_008 { get; set; }

    }
}
