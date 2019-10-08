using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public class SingMaxHot_108
    {
      public long  LongPeriod_001{set;get;}
      public int   PositionType_002 {set;get;}
      public int   PositionValue_003 {set;get;}
      public long  DateNumber_004 {set;get;}
      public int   ShortPeriod_005 {set;get;}
      public int   HotType_006 {set;get;}
      public int   IsComplete_007{set;get;}
      public long    CompletePeriod_008 {set;get;} //统计到那一期了
      public string   StrPreAppearSpan_009 {set;get;}
      public string   StrLaterAppearSpan_010 {set;get;}
      public long    LastAppearPeriod_011 {set;get;} //最后一次出现的期号
      public long    StartAppearPeriod_012 {set;get;} //最开始时出现的期号
      public int  LaterCount_013 {set;get;}
      public int   PreCount_014 {set;get;}
      public int   OrderAll_59_015 {set;get;}
      public int   OrderAll_80_016 {set;get;}
      public int   OrderSing_15_017 {set;get;}
      public int   OrderSing_30_018 {set;get;}
      public int   OrderSing_59_019 {set;get;}
      public int OrderSing_80_020 { set; get; }
    }
}
