using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public class HotStatistics_104
    {
        /// <summary>
        /// 期数
        /// </summary>
        public long LongPeriod_001
        {
            get;
            set;
        }


        //日期的前6位
        public Int64 DateNumber_002 { get; set; }


        //数字期
        public int ShortPeriod_003 { set; get; }

        public int PositionType_004 { set; get; }

        public int PositionVale_005 { get; set; }

        public int SpliteType_006 { get; set; }


        //该段内出现个数       
        public int NumberCount_007 { get; set; }
        //除出来的热度值
        public decimal HotValue_008 { get; set; }

        //该段内总的个数
        public int NumberAllCount_009 { get; set; }
    }
}
