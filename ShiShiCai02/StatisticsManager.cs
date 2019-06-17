using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.IO.Compression;
using DBHelp;
using System.Reflection;

namespace ShiShiCai02
{
    public   class StatisticsManager
    {
        #region  统计线程
        private bool IFlagThread002;
        private Thread IThreadLostData002;
        private bool IFlagThread007;
        private Thread IThread007;
        #endregion


        #region  静态字段
        private static string ISqlConnect = string.Empty;
        private static int ILostMarkSet = 0;
        //上次统计到多少期
        public static long IPeriodNumber = 0;
        #endregion



        #region  线程启停
        public StatisticsManager() 
        {
            IFlagThread002 = true;
            IFlagThread007 = true;
            ISqlConnect = ConfigurationManager.AppSettings["SqlServerConnect"] != null ? ConfigurationManager.AppSettings["SqlServerConnect"] : "Data Source=127.0.0.1,1433;Initial Catalog=Pocker;User Id=sa;Password=daifeng";
            ILostMarkSet = ConfigurationManager.AppSettings["IntLostAllWeiSet"] != null ? int.Parse( ConfigurationManager.AppSettings["IntLostAllWeiSet"].ToString() ): 5;
            IPeriodNumber = ConfigurationManager.AppSettings["PeriodNumber"] != null ? long.Parse(ConfigurationManager.AppSettings["PeriodNumber"].ToString()) : 20170101001;
        }

        public void StatisticsStartup()
        {
            IFlagThread002 = false;
            if (IThreadLostData002 !=null)
            {
                IThreadLostData002.Abort();
            }

            IFlagThread007 = false;
            if (IThread007 !=null)
            {
                IThread007.Abort();
            }

            IThreadLostData002 = new Thread(new ParameterizedThreadStart(StatisticsManager.AutoDoData002));
            IFlagThread002 = true;
            IThreadLostData002.Start(this);
            FileLog.WriteInfo("StatisticsManager ", "AutoDoData002  Start");


            IThread007 = new Thread(new ParameterizedThreadStart(StatisticsManager.AutoDoData007));
            IFlagThread007 = true;
            IThread007.Start(this);
            FileLog.WriteInfo("StatisticsManager ", " AutoDoData007  Start");
        }

        public void StatisticsStop() 
        {
            IFlagThread002=false;
            if (IThreadLostData002 != null)
            {
                IThreadLostData002.Abort();
                IThreadLostData002 = null;
            }

            IFlagThread007 = false;
            if (IThread007 !=null) 
            {
                IThread007.Abort();
                IThread007 = null;
            }

        }
        #endregion


        private static void AutoDoData002(object o)
        {
            StatisticsManager staticsmanager = o as StatisticsManager;
            OptLostData002 optLostData002 = new OptLostData002();
            while (staticsmanager.IFlagThread002)
            {
                optLostData002.IStrDatabaseProfile = ISqlConnect;
            }
        }

        private static void AutoDoData007(object o)
        {

            StatisticsManager staticsmanager = o as StatisticsManager;

            while (staticsmanager.IFlagThread007)
            {

            }
        }



        #region  公用方法

        //得到5个数中的最大值
        public static int GetMax(int one, int two, int three, int four, int five)
        {
            int Imax = 0;
            List<int> listInt = new List<int>();
            listInt.Add(one);
            listInt.Add(two);
            listInt.Add(three);
            listInt.Add(four);
            listInt.Add(five);
            Imax = listInt.Max();

            return Imax;
        }

        /// <summary>
        /// 先判断Alist有没有这个数，没有这个数则先加入后再求序
        /// </summary>
        /// <param name="AlistInt"></param>
        /// <param name="Current"></param>
        /// <returns></returns>
        public static int QiuXu1(List<int> AlistInt, int Current)
        {
            int index = 0;
            if (!AlistInt.Contains(Current))
            {
                AlistInt.Add(Current);
            }
            AlistInt = AlistInt.OrderByDescending(s => s).ToList();
            index = AlistInt.FindIndex(s => s == Current);
            return index + 1;
        }


        public static string GetObjectPropertyValue<T>(T t, string propertyname)
        {
            Type type = typeof(T);

            PropertyInfo property = type.GetProperty(propertyname);

            if (property == null) return string.Empty;

            object o = property.GetValue(t, null);

            if (o == null) return string.Empty;

            return o.ToString();
        }


        /// <summary>
        /// 将字符串转日期
        /// </summary>
        /// <param name="source">日期字符串</param>
        /// <returns></returns>
        public static DateTime StringToDateTime(string source)
        {
            if (source.Length < 8)
            {
                return DateTime.Parse("2100-1-1 00:00:00");
            }
            DateTime dt;
            string strTime = source.Substring(0, 4) + "-";
            strTime += source.Substring(4, 2) + "-";
            strTime += source.Substring(6, 2) + " ";
            strTime += "00:00:00";

            dt = DateTime.Parse(DateTime.Parse(strTime).ToString("yyyy-MM-dd HH:mm:ss"));
            return dt;
        }


        /// <summary>
        /// 得到N 期以前的期数
        /// </summary>
        /// <param name="CurrentValue"></param>
        /// <param name="ASpanValue"></param>
        /// <returns></returns>
        public static long GetStopPeriodValue(long CurrentValue, int ASpanValue)
        {
            long Stop = 0;
            int i = 0;
            int k = 0; //取商
            int m = 0; //取模

            int j = int.Parse(CurrentValue.ToString().Substring(8));

            if ((ASpanValue - j) >= 0)
            {
                i = ASpanValue - j;
                k = i / 120;
                m = i % 120;

                DateTime lastDay = StringToDateTime(CurrentValue.ToString().Substring(0, 8)).AddDays(-(k + 1));
                Stop = long.Parse(lastDay.ToString("yyyyMMdd") + "120");
                CurrentValue = Stop - m;

            }
            else
            {
                CurrentValue = CurrentValue - ASpanValue;
            }
            return CurrentValue;
        }


        public static long GetSpanOfTwoPeriod(long CurrentValue, long OldValue)
        {
            long Span = 0;
            string strCurr = CurrentValue.ToString().Substring(0, 8) + "000";
            string strOld = OldValue.ToString().Substring(0, 8) + "000";
            string strOldStop = OldValue.ToString().Substring(0, 8) + "120";
            if (strCurr.Equals(strOld))
            {
                Span = CurrentValue - OldValue;
            }
            else
            {
                DateTime start = StringToDateTime(strCurr);
                DateTime stop = StringToDateTime(strOld);
                TimeSpan sp = start.Subtract(stop);
                Span = (sp.Days - 1) * 120 + (long.Parse(strOldStop) - OldValue) + (CurrentValue - long.Parse(strCurr));
            }

            return Span;
        }



        /// <summary>
        ///  //根据时间得到期数
        /// </summary>
        /// <param name="ANow"></param>
        /// <param name="i">1为0点到2点，10～22点,22点到24点</param>
        /// <returns></returns>
        public static long Time2PeriodNumber(DateTime ANow)
        {
            int i = 0;
            DateTime StartTime;
            long currentPeriod = 0;
            if (ANow < ANow.Date.AddHours(2))
            {
                StartTime = ANow.Date;
                TimeSpan ts1 = ANow.Subtract(StartTime);
                i = (ts1.Hours * 60 + ts1.Minutes) / 5;
            }
            else if (ANow >= ANow.Date.AddHours(10) && ANow <= ANow.Date.AddHours(22))
            {
                StartTime = ANow.Date.AddHours(10);
                TimeSpan ts1 = ANow.Subtract(StartTime);
                i = (ts1.Hours * 60 + ts1.Minutes) / 10 + 24;
            }
            else if (ANow >= ANow.Date.AddHours(22))
            {
                StartTime = ANow.Date.AddHours(22);
                TimeSpan ts1 = ANow.Subtract(StartTime);
                i = (ts1.Hours * 60 + ts1.Minutes) / 5 + 96;
            }
            currentPeriod = long.Parse(ANow.ToString("yyyyMMdd") + i.ToString().PadLeft(3, '0'));

            return currentPeriod;
        }


        /// <summary>
        /// 将字符串转日期或者期数
        /// </summary>
        /// <param name="source">日期字符串</param>
        /// <returns></returns>
        public static DateTime PeriodNumber2DateTime(string source)
        {
            if (source.Length < 8)
            {
                return DateTime.Parse("2100-1-1 00:00:00");
            }
            DateTime dt;
            string strTime = source.Substring(0, 4) + "-";
            strTime += source.Substring(4, 2) + "-";
            strTime += source.Substring(6, 2) + " ";
            strTime += "00:00:00";

            dt = DateTime.Parse(DateTime.Parse(strTime).ToString("yyyy-MM-dd HH:mm:ss"));
            return dt;
        }

        #endregion


    }
}
