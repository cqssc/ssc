using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    /// <summary>
    /// 作废--趋势热度
    /// </summary>
    public class HotTrend_108
    {
        /// <summary>
        /// 期数
        /// </summary>
        public long LongPeriod_001 { get; set; }

        //日期的前6位
        public Int64 DateNumber_002 { get; set; }

        //数字期
        public int ShortPeriod_003 { set; get; }

        public int PositionType_004 { set; get; }

        public int SplitValueType_005 { get; set; }

        public int FenMu_006 { get; set; }

        public int RepickNum_010 { get; set; }

        public decimal RepickRate_011 { get; set; }

        public int SwingNum_020 { get; set; }

        public decimal SwingRate_021 { get; set; }

        public int AddOrSubNum_030 { get; set; }

        public decimal AddOrSubRate_031 { get; set; }

        public int OtherNum_040 { get; set; }

        public decimal OtherRate_041 { get; set; }


    }
}
