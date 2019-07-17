using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public  class LostCross_104
    {
        /// <summary>
        /// 期数
        /// </summary>
        public long LongPeriod_001 { get; set; }
        //日期的前6位
        public Int64 DateNumber_002 { get; set; }
        //短期数
        public int ShortPeriod_003 { set; get; }

        public int PositionType_004 { get; set; }

        public int CrossType_005 { get; set; }


        public int IsAppear_006 { get; set; }

        public int LostValue_007 { get; set; }

        public int ContinueValue_008 { get; set; }

    }
}
