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
        public int SingNum_004{ set; get; }

        public int IsAppear_005 { set; get; }

        public int LostValue_006 { set; get; }

         //出现个数
        public int AppearCount_007 { set; get; }
    }
}
