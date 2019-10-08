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
        /// 置0  非置0
        /// </summary>
        public static int Const_Set_Zero = -1;        
        public static int Const_Set_Normal = 1;



        /// <summary>
        /// 某一期前面一期
        /// </summary>
        public static int Const_GetData_Pre = 2;
        /// <summary>
        /// 当前期
        /// </summary>
        public static int Const_GetData_Current = 0;
        /// <summary>
        /// 下一期
        /// </summary>
        public static int Const_GetData_Next = 1;
        /// <summary>
        /// 算上本期以及前面一段
        /// </summary>
        public static int Const_GetData_CurrentAndPre = 2;

        /// <summary>
        /// 未完成的
        /// </summary>
        public static int Const_GetData_NotComplete = 1;


        /// <summary>
        /// 103表要用的C005用的类型1重复 2 递增 （3期才算，要两个间隔）3递减（3期才算，要两个间隔）4 振荡 （3期才算，要两个间隔）
        /// </summary>
        public static int Const_103Type_Repick = 1;
        public static int Const_103Type_ContinueAdd = 2;
        public static int Const_103Type_ContinueSub = 3;
        public static int Const_103Type_Swing = 4;





        /// <summary>
        /// 104表要用的几个类型
        /// </summary>
        public static int Const_Cross_Type_Repick = 1;
        public static int Const_Cross_Type_Span = 2;
        public static int Const_Cross_Type_AbsSpan = 3;


        /// <summary>
        /// 规律详表110要用的几个规律类型
        /// </summary>
         public static int  Const_F_Xian=1;
         public static int  Const_F_FanXian=2;
         public static int  Const_F_AddOneOriginal=3;
         public static int  Const_F_SubOneOriginal=4;
         public static int  Const_F_32283=5;
         public static int  Const_F_Big20InSingle=6;
         public static int  Const_F_ThreeConRepick=7;
         public static int  Const_F_BigConOriginalBig=8;
         public static int  Const_F_SmallConOriginalSmall=9;
         public static int  Const_F_EvenConOriginalEven=10;
         public static int  Const_F_OddConOriginalOdd=11;
         public static int  Const_F_LostOver6=12;
         public static int  Const_F_RecpickOver6=13;
         public static int  Const_F_SpanOneOver6=14;
         public static int  Const_F_LostNumOver10 = 15;



        /// <summary>
        /// 108表用的热类型
        /// </summary>
         public static int Const_HotType_15MaxHot = 1;
         public static int Const_HotType_20MaxHot = 2;
         public static int Const_HotType_30MaxHot = 3;
         public static int Const_HotType_4S10 = 4;



    }
}
