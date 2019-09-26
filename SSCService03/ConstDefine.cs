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



        public static int Const_Cross_Type_Repick = 1;
        public static int Const_Cross_Type_Span = 2;
        public static int Const_Cross_Type_AbsSpan = 3;


    }
}
