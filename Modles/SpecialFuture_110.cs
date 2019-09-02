using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public class SpecialFuture_110
    {
        public long LongPeriod_001 { get; set; }


        public int PositionType_002 { get; set; }

        public int PositionVale_003 { get; set; }

        public int FutureType_004 { set; get; }

        public int IsComplete_005 { set; get; }

        //命中是否
        public int IsScore_006{ set;get;}

        //几期命中
        public int ScoreSpan_007 { set; get; }

  
        //命中时的期数
        public long ScoreLongPeriod_008 { set; get; }


        public string AlarmBeforeValues_009 { set; get; } //预警前数字
        public string FutureTypeNote_010 { set; get; } //特征类型解释


        //日期的前6位
        public Int64 DateNumber_011 { get; set; }

        //数字期
        public int ShortPeriod_012 { set; get; }

        public string  RealAppearValues_013 { set; get; } //实际出现数字
        public int ScoreBack_014 { set; get; }
        public int ScoreBack_015 { set; get; }


    }
}
