using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    /// <summary>
    /// 开奖详情表
    /// </summary>
    public class PeriodDetail_101
    {
        public long LongPeriod_001 { get; set; }

        /// <summary>
        /// 5位开奖号码
        /// </summary>
        public string AwardNumber_002 { get; set; }

        public DateTime DateTimeInsert_003 { get; set; }

        public long DateNumber_004 { get; set; }

        public int ShortPeriod_005 { get; set; }

        public int DayInWeek_006 { get; set; }

        public int BigOrSmall_007 { set; get; }

        public int EvenODD_008 { get; set; }

        public int AllSub_009 { get; set; }

        public int Wei5_050 { get; set; }

        public int Wei4_040 { get; set; }

        public int Wei3_030 { get; set; }

        public int Wei2_020 { get; set; }

        public int Wei1_010 { get; set; }


        public int CountBig_106 { get; set; }
        public int CountSmall_107 { get; set; }
        public int CountEven_108 { get; set; }
        public int CountOdd_109 { get; set; }

    }
}
