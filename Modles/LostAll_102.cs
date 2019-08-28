using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{

    /// <summary>
    /// T_002
    /// </summary>
    public class LostAll_102
    {
        public LostAll_102()
        {
        }

        /// <summary>
        /// 期数
        /// </summary>
        public long LongPeriod_001 { get;  set;}
        //日期的前6位
        public Int64 DateNumber_002 { get; set; }
        //数字期
        public int ShortPeriod_003 { set; get; }



        //出现>20期遗失的数量  >20期遗失出现时的出现位置数量
        public int Appear20Lost_004 { set; get; }

        //出现>20M1的数量 
        public int Appear20MLost_005 { set; get; }



        //剩下>20期遗失的数量
        public int Remain20Lost_006 { set; get; }

        //剩下>20M1的数量
        public int Remain20MLost_007 { set; get; }

        //剩下>20集中的数字个数
        public int Remain20PositionValueNum_008 { set; get; }

        //>剩下20期遗失数字分布的个数数量
        public int Remain20PositionNum_009 { set; get; }


        


        public int Lost_010 { set; get; }

        public int Lost_011 { set; get; }

        public int Lost_012 { set; get; }

        public int Lost_013 { set; get; }

        public int Lost_014 { set; get; }

        public int Lost_015 { set; get; }

        public int Lost_016 { set; get; }

        public int Lost_017 { set; get; }

        public int Lost_018 { set; get; }

        public int Lost_019 { set; get; }

        public int Lost_020 { set; get; }

        public int Lost_021 { set; get; }

        public int Lost_022 { set; get; }

        public int Lost_023 { set; get; }

        public int Lost_024 { set; get; }

        public int Lost_025 { set; get; }

        public int Lost_026 { set; get; }

        public int Lost_027 { set; get; }

        public int Lost_028 { set; get; }

        public int Lost_029 { set; get; }

        public int Lost_030 { set; get; }

        public int Lost_031 { set; get; }

        public int Lost_032 { set; get; }

        public int Lost_033 { set; get; }

        public int Lost_034 { set; get; }

        public int Lost_035 { set; get; }

        public int Lost_036 { set; get; }

        public int Lost_037 { set; get; }

        public int Lost_038 { set; get; }

        public int Lost_039 { set; get; }

        public int Lost_040 { set; get; }

        public int Lost_041 { set; get; }

        public int Lost_042 { set; get; }

        public int Lost_043 { set; get; }

        public int Lost_044 { set; get; }

        public int Lost_045 { set; get; }

        public int Lost_046 { set; get; }

        public int Lost_047 { set; get; }

        public int Lost_048 { set; get; }

        public int Lost_049 { set; get; }

        public int Lost_050 { set; get; }

        public int Lost_051 { set; get; }

        public int Lost_052 { set; get; }

        public int Lost_053 { set; get; }

        public int Lost_054 { set; get; }

        public int Lost_055 { set; get; }

        public int Lost_056 { set; get; }

        public int Lost_057 { set; get; }
       
        public int Lost_058 { set; get; }

        public int Lost_059 { set; get; }


        //万....个各位出现时的遗失期
        public int PreLost_101 { set; get; }
        public int PreLost_102 { set; get; }
        public int PreLost_103 { set; get; }
        public int PreLost_104 { set; get; }
        public int PreLost_105 { set; get; }



    }
}
