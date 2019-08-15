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

        public int Big_005 { get; set; } 

        public int Small_006 { get; set; } 

        public int Even_007 { get; set; } 

        public int Odd_008 { get; set; }

        public int BigSmallContinue_009 { get; set; }

        public int EvenOddContinue_010 { get; set; }

        public int ComposeBig_011 { get; set; }
        public int ComposeSmall_012 { get; set; }
        public int ComposeEven_013 { get; set; }
        public int ComposeOdd_014 { get; set; }

    }
}
