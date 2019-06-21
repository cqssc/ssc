using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public class SwingDetail_107
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
        public int AddSubType_005 { get; set; }//1递增或递减 2递增减（排重复）3 平行  4振荡
        public int ContinueValue_006 { get; set; }
        public int SwingValue_007 { get; set; }
        public decimal SwingAVG_008 { get; set; }
        public decimal SwingLiSan_009 { get; set; }
    }
}
