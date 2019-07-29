using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSCService03
{
    public static class ConstDefine
    {

        /// <summary>
        /// 置0
        /// </summary>
        public static int Const_SetZero = -1;
        
        /// <summary>
        /// 非置0
        /// </summary>
        public static int Const_SetNormal = 1;


        /// <summary>
        /// 当前期
        /// </summary>
        public static int Const_GetCurrent = 0;

        /// <summary>
        /// 下一期
        /// </summary>
        public static int Const_GetNext = 1;

        /// <summary>
        /// 算上本期以及前面一段
        /// </summary>
        public static int Const_GetPreSpan = 2;


        /// <summary>
        /// 某一期前面一期
        /// </summary>
        public static int Const_GetPre = 2;


        /// <summary>
        /// 取最新一期
        /// </summary>
        public static int Const_GetStartStop = 3;


        //public static int Con_Future_1_4LianDa = 1; //4连大取反
        //public static int Con_Future_2_4LianXiao = 2; //4连小取反
        //public static int Con_Future_3_4LianDan = 3; //4连单取反
        //public static int Con_Future_4_4LianShuang = 4; //4连双取反
        //public static int Con_Future_5_ZhenXianXing = 5; //正线性
        //public static int Con_Future_6_FanXianXing = 6;//反线性
        //public static int Con_Future_7_5Dui = 7;//堆（>5个）
        //public static int Con_Future_8_Jian = 8;//渐增1减1
        //public static int Con_Future_9_2LianDui = 9;//2连对
        //public static int Con_Future_10_3Tong = 10;//3同
        //public static int Con_Future_11_VFu = 11; //V重复
        //public static int Con_Future_12_FuV = 12; //重复后V
        //public static int Con_Future_13_32283 = 13;
        //public static int Con_Future_14_38223 = 14;
        //public static int Con_Future_15_4DanShuangShouWei = 15;   //长单双，首尾同  
        //public static int Con_Future_16_3Tong = 16;//3个数一样的，暂时先不做。

        //public static int Const_Future_0 = 20; ////重复特殊
        //public static int Const_Future_1 = 21; //3228#3   
        //public static int Const_Future_2 = 22; //3228#3
        //public static int Const_Future_3 = 23; //228#2
        //public static int Const_Future_4 = 24; //123#
        //public static int Const_Future_5 = 25; //长单双 首尾同
        //public static int Const_Future_6 = 26; //22448#
        //public static int Const_Future_7 = 27; //3338#3  
        //public static int Const_Future_8 = 28; //44#4
        //public static int Const_Future_9 = 29; //444#4
        //public static int Const_Future_10 = 30;//667#7 
        //public static int Const_Future_11 = 31;//66677#7 
        //public static int Const_Future_12 = 32; //282#2
    }
}
