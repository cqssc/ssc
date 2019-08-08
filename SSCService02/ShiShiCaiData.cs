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
using Modles;

namespace SSCService02
{
    public class ShiShiCaiData
    {
        #region  统计线程
        private bool IBoolIsThreadShiShiCaiDataWorking;
        private Thread IThreadShiShiCaiData;
        #endregion
        #region 静态全局字符串
        private static string sqlConnect = string.Empty;
        private static string LastTimeSychnData = string.Empty;
        private static string LinkOf08 = string.Empty;

        private static string LinkOf08_bak1 = string.Empty;
        private static string LinkOf08_bak2 = string.Empty; 
        private static DateTime RunCurrentDate;
        private static int CurrentPeriod;

        private static string IsSetUpCook = "0";
        private static string strCookier;
        #endregion

        public ShiShiCaiData()
        {
            sqlConnect = ConfigurationManager.AppSettings["SqlServerConnect"] != null ? ConfigurationManager.AppSettings["SqlServerConnect"] : "Data Source=127.0.0.1,1433;Initial Catalog=Pocker;User Id=sa;Password=net,123";

            LinkOf08 = ConfigurationManager.AppSettings["Url08"] != null ? ConfigurationManager.AppSettings["Url08"].ToString() : "http://vip.08vip.co/result/ssc_noticle.php?sdate={0}&sdate1={1}";

            LinkOf08_bak1 = ConfigurationManager.AppSettings["Url08_bak1"] != null ? ConfigurationManager.AppSettings["Url08_bak1"].ToString() : "http://vip.08vip.co/result/ssc_noticle.php?sdate={0}&sdate1={1}";

            LinkOf08_bak2 = ConfigurationManager.AppSettings["Url08_bak2"] != null ? ConfigurationManager.AppSettings["Url08_bak2"].ToString() : "http://vip.08vip.co/result/ssc_noticle.php?sdate={0}&sdate1={1}";

            IsSetUpCook = ConfigurationManager.AppSettings["IsSetUpCook"] != null ? ConfigurationManager.AppSettings["IsSetUpCook"].ToString() : "0";
            strCookier = "";
        }

        private static void AutoShishiCaiDataGet(object o)
        {
            ShiShiCaiData autoDBInfo = o as ShiShiCaiData;
            List<PeriodDetail_101> listPeriodTemp = new List<PeriodDetail_101>(); ;
            while (autoDBInfo.IBoolIsThreadShiShiCaiDataWorking)
            {
                DateTime Nowwwww = DateTime.Now;
                DateTime NowDate = Nowwwww.Date;
                RunCurrentDate = ConfigurationManager.AppSettings["LastTimeSynchData"] != null ? DateTime.Parse(ConfigurationManager.AppSettings["LastTimeSynchData"].ToString()).Date : DateTime.Now.Date;
                string YY = RunCurrentDate.Year.ToString().Substring(2, 2);
                //1、读取保存的最后的日期与当前日期做比较
                bool flag = true;
                listPeriodTemp.Clear();
                if (RunCurrentDate < NowDate)  //做全天数据写入
                {
                    #region
                    flag = GetDataFromUrl(RunCurrentDate, ref listPeriodTemp);
                    if (flag && listPeriodTemp.Count > 0)
                    {
                        flag = UpateDataBase(listPeriodTemp, -1, YY);
                        if (flag)
                        {
                            RunCurrentDate = RunCurrentDate.AddDays(1);
                            AppConfigOperation.UpdateAppConfig("LastTimeSynchData", RunCurrentDate.ToString("yyyy-MM-dd 00:00:00"));
                            FileLog.WriteInfo("AutoShishiCaiDataGet()@ ", RunCurrentDate.ToString());
                        }
                    }
                    else
                    {
                        RunCurrentDate = RunCurrentDate.AddDays(1);
                        AppConfigOperation.UpdateAppConfig("LastTimeSynchData", RunCurrentDate.ToString("yyyy-MM-dd 00:00:00"));
                        FileLog.WriteInfo("AutoShishiCaiDataGet()@ ", RunCurrentDate.ToString());
                    }
                    #endregion
                }
                else if (RunCurrentDate == NowDate)
                {
                    listPeriodTemp.Clear();
                    CurrentPeriod = GetCurrentPeriod(Nowwwww);
                    int periodCaul = CauTimeSpan(Nowwwww);
                    flag = true;
                    if (periodCaul - CurrentPeriod > 1)
                    {
                        #region
                        flag = GetDataFromUrl(RunCurrentDate, ref listPeriodTemp);
                        FileLog.WriteInfo("AutoShishiCai ===>", DateTime.Now.ToString() + " 1periodCaul:" + periodCaul);
                        if (flag && listPeriodTemp.Count > 0)
                        {
                            UpateDataBase(listPeriodTemp, -1, YY);
                            Thread.Sleep(1000 * 5);
                        }
                        #endregion
                    }
                    else if (periodCaul - CurrentPeriod == 1)
                    {
                        #region
                        flag = true;
                        flag = GetDataFromUrl(RunCurrentDate, ref listPeriodTemp, periodCaul);
                        FileLog.WriteInfo("AutoShishiCai ===>", DateTime.Now.ToString() + " 2periodCaul:" + periodCaul);
                        if (flag && listPeriodTemp.Count() > 0)
                        {
                            if (UpateDataBase(listPeriodTemp, 1, YY))
                            {
                                Thread.Sleep(1000 * 1);
                                FileLog.WriteInfo("AutoShishiCaiDataGet()", "periodCaul" + "--" + periodCaul);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        //计算期==当前期
                        Thread.Sleep(1000 * 1);
                    }

                }
                else
                {
                    //配置里的时间大于机器时间了
                    Thread.Sleep(1000 * 1);
                }
            
            }
            
        }


        public static bool GetDataFromUrl(DateTime ADateTime, ref List<PeriodDetail_101> AlistPeriod001, int AType = -1)
        {
            bool flag = true;

            for (int k = 0; k < 30; k++)
            {
                if (AType == -1) //用于取多期
                {

                    flag = GetHtmlString08(LinkOf08, ADateTime, ref AlistPeriod001, AType);
                    if (flag && AlistPeriod001.Count > 0)
                    {
                        return flag;
                    }

                    flag = GetHtmlString08(LinkOf08_bak1, ADateTime, ref AlistPeriod001, AType);
                    if (flag && AlistPeriod001.Count > 0)
                    {
                        return flag;
                    }

                    flag = GetHtmlString08(LinkOf08_bak2, ADateTime, ref AlistPeriod001, AType);
                    if (flag && AlistPeriod001.Count > 0)
                    {
                        return flag;
                    }

                }
                else
                {
                    flag = GetHtmlString08(LinkOf08, ADateTime, ref AlistPeriod001, AType);
                    if (flag && AlistPeriod001.Count > 0)
                    {
                        return flag;
                    }

                    flag = GetHtmlString08(LinkOf08_bak1, ADateTime, ref AlistPeriod001, AType);
                    if (flag && AlistPeriod001.Count > 0)
                    {
                        return flag;
                    }

                    flag = GetHtmlString08(LinkOf08_bak2, ADateTime, ref AlistPeriod001, AType);
                    if (flag && AlistPeriod001.Count > 0)
                    {
                        return flag;
                    }
                }


                Thread.Sleep(500);
                FileLog.WriteInfo(" K=", k.ToString());
            }
            return flag;
        }

        public static int GetCurrentPeriod(DateTime ADateTime)
        {
            int i = 0;
            string strSQL = string.Format("select MAX(C005 ) as C005 from T_101_{1} where C004={0}", ADateTime.ToString("yyyyMMdd"), ADateTime.ToString("yyyyMMdd").Substring(2, 2));
            DataSet ds = DbHelperSQL.GetDataSet(sqlConnect, strSQL);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["C005"] != DBNull.Value)
                    {
                        i = int.Parse(dr["C005"].ToString(), 0);
                    }
                }
            }
            return i;
        }

        /// <summary>
        /// 更新数据库
        /// AType==-1 做全天数据更新
        /// 其它为单期数据更新
        /// </summary>
        public static bool UpateDataBase(List<PeriodDetail_101> AListPeriod, int AType = -1, string YY = "")
        {
            bool flag = true;
            FileLog.WriteInfo("UpateDataBase", "AType=" + AType);
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            try
            {
                string strSql = string.Empty;

                if (AType == -1)
                {
                    strSql = string.Format("SELECT * from T_101_{1} where C004={0}", AListPeriod.First().DateNumber_004, YY);
                    FileLog.WriteInfo("UpateDataBase()", strSql);

                }
                else
                {
                    strSql = string.Format("SELECT * from T_101_{1} where C001={0}", AListPeriod.Last().LongPeriod_001, YY);
                    FileLog.WriteInfo("UpateDataBase()", strSql);
                }

                objConn = DbHelperSQL.GetConnection(sqlConnect);
                objAdapter = DbHelperSQL.GetDataAdapter(objConn, strSql);
                objCmdBuilder = DbHelperSQL.GetCommandBuilder(objAdapter);
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);

                //1\写数据库
                foreach (PeriodDetail_101 pp in AListPeriod)
                {
                    DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001={0}", pp.LongPeriod_001)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001={0}", pp.LongPeriod_001)).First() : null;

                    if (drCurrent != null) //更新
                    {
                        drCurrent.BeginEdit();
                        drCurrent["C002"] = pp.AwardNumber_002;
                        drCurrent["C050"] = pp.Wei5_050;
                        drCurrent["C040"] = pp.Wei4_040;
                        drCurrent["C030"] = pp.Wei3_030;
                        drCurrent["C020"] = pp.Wei2_020;
                        drCurrent["C010"] = pp.Wei1_010;
                        drCurrent["C003"] = pp.DateTimeInsert_003.ToString("yyyy/MM/dd HH:mm:ss");
                        drCurrent["C004"] = pp.DateNumber_004;
                        drCurrent["C005"] = pp.ShortPeriod_005;
                        drCurrent["C006"] = pp.DayInWeek_006;
                        drCurrent["C007"] = pp.BigOrSmall_007;
                        drCurrent["C008"] = pp.EvenODD_008;
                        drCurrent["C009"] = pp.AllSub_009;
                        drCurrent.EndEdit();
                    }
                    else //添加新行
                    {
                        DataRow drNewRow = objDataSet.Tables[0].NewRow();
                        drNewRow["C001"] = pp.LongPeriod_001;
                        drNewRow["C002"] = pp.AwardNumber_002;
                        drNewRow["C050"] = pp.Wei5_050;
                        drNewRow["C040"] = pp.Wei4_040;
                        drNewRow["C030"] = pp.Wei3_030;
                        drNewRow["C020"] = pp.Wei2_020;
                        drNewRow["C010"] = pp.Wei1_010;
                        drNewRow["C003"] = pp.DateTimeInsert_003.ToString("yyyy/MM/dd HH:mm:ss");
                        drNewRow["C004"] = pp.DateNumber_004;
                        drNewRow["C005"] = pp.ShortPeriod_005;
                        drNewRow["C006"] = pp.DayInWeek_006;

                        drNewRow["C007"] = pp.BigOrSmall_007;
                        drNewRow["C008"] = pp.EvenODD_008;
                        drNewRow["C009"] = pp.AllSub_009;
                        objDataSet.Tables[0].Rows.Add(drNewRow);
                    }
                }

                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
            }
            catch (Exception e)
            {
                FileLog.WriteError("AutoShishiCaiDataGet() ", e.Message);
                Thread.Sleep(5 * 60 * 1);
                return flag = false;
            }

            return flag;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="listPeriod"></param>
        /// <param name="AIntType">0 360,1 163 ，3  08</param>
        public static void GetPriodDataOfHtmlString(ref  List<PeriodDetail_101> listPeriod, string AStrHtmlString, int AIntType, DateTime ADateTime, int APeriod = -1)
        {
            string regex163 = "<td class=\"start\" data-win-number=\'(\\d{1}\\s\\d{1}\\s\\d{1}\\s\\d{1}\\s\\d{1})\' data-period=\"\\d{9}\">(\\d{3})</td>";
            string regex360 = "<td class=\'gray\'>(\\d{3})</td><td class=\'red big\'>(\\d{5})</td>";
            string regex08 = string.Empty;

            Regex re = null;
            if (AIntType == 0)
            {
                re = new Regex(regex360);
            }
            else if (AIntType == 1)
            {
                re = new Regex(regex163);
            }
            else
            {
                re = new Regex(regex08);
            }
            MatchCollection matches = re.Matches(AStrHtmlString);
            System.Collections.IEnumerator enu = matches.GetEnumerator();
            string strPeriod = string.Empty;
            string strAllValue = string.Empty;
            while (enu.MoveNext() && enu.Current != null)
            {
                strPeriod = string.Empty;
                strAllValue = string.Empty;

                Match match = (Match)(enu.Current);
                if (AIntType == 0)
                {
                    strPeriod = match.Groups[1].Value;
                    strAllValue = match.Groups[2].Value;
                }
                else if (AIntType == 1)
                {
                    strPeriod = match.Groups[2].Value;
                    strAllValue = match.Groups[1].Value;
                }
                else
                {
                }
                int Xingqi = 0;

                switch (ADateTime.DayOfWeek)
                {
                    case DayOfWeek.Friday:
                        Xingqi = 5;
                        break;
                    case DayOfWeek.Monday:
                        Xingqi = 1;
                        break;
                    case DayOfWeek.Saturday:
                        Xingqi = 6;
                        break;
                    case DayOfWeek.Sunday:
                        Xingqi = 7;
                        break;
                    case DayOfWeek.Thursday:
                        Xingqi = 4;
                        break;
                    case DayOfWeek.Tuesday:
                        Xingqi = 2;
                        break;
                    case DayOfWeek.Wednesday:
                        Xingqi = 3;
                        break;
                    default:
                        Xingqi = 0;
                        break;
                }

                if (!string.IsNullOrWhiteSpace(strPeriod) && !string.IsNullOrWhiteSpace(strAllValue))
                {
                    PeriodDetail_101 period = new PeriodDetail_101();
                    string[] strArrayTemp = strAllValue.Split(' ');
                    if (strArrayTemp.Length == 5)
                    {
                        period.Wei5_050 = int.Parse(strArrayTemp[0]);
                        period.Wei4_040 = int.Parse(strArrayTemp[1]);
                        period.Wei3_030 = int.Parse(strArrayTemp[2]);
                        period.Wei2_020 = int.Parse(strArrayTemp[3]);
                        period.Wei1_010 = int.Parse(strArrayTemp[4]);
                        period.DateTimeInsert_003 = DateTime.Now;
                        period.DateNumber_004 = Int64.Parse(ADateTime.ToString("yyyyMMdd"));
                        period.ShortPeriod_005 = int.Parse(strPeriod);
                        period.LongPeriod_001 = Int64.Parse(period.DateNumber_004.ToString() + period.ShortPeriod_005.ToString().PadLeft(3, '0'));
                        period.AwardNumber_002 = string.Format("{0}{1}{2}{3}{4}", period.Wei5_050, period.Wei4_040, period.Wei3_030, period.Wei2_020, period.Wei1_010);
                        period.DayInWeek_006 = Xingqi;
                        listPeriod.Add(period);
                    }
                }
            }

            if (APeriod != -1)
            {
                if (listPeriod.Where(p => p.ShortPeriod_005 == APeriod).Count() > 0)
                {
                    PeriodDetail_101 pTemp = listPeriod.Where(p => p.ShortPeriod_005 == APeriod).First();
                    listPeriod.Clear();
                    listPeriod.Add(pTemp);
                }
                else
                {
                    listPeriod.Clear();
                }
            }

        }

        /// <summary>
        /// 得到当前期数据
        /// </summary>
        /// <param name="listPeriod"></param>
        /// <param name="AType"></param>
        /// <returns></returns>
        public static bool GetCurrentPeriodDataOfList(ref   List<PeriodDetail_101> listPeriod, int GolaPeriod)
        {
            bool flag = true;


            if (listPeriod.Where(p => p.ShortPeriod_005 == GolaPeriod).Count() > 0)
            {
                PeriodDetail_101 pTemp = listPeriod.Where(p => p.ShortPeriod_005 == GolaPeriod).First();
                FileLog.WriteInfo("GetHtmlString163--Get Period's LongPeriod_001", pTemp.LongPeriod_001.ToString());
                listPeriod.Clear();
                listPeriod.Add(pTemp);
            }
            else
            {
                listPeriod.Clear();
            }
            return flag;
        }


        #region


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ANow"></param>
        /// <returns></returns>
        public static int CauTimeSpan(DateTime ANow)
        {
            int i = 0;
            DateTime StartTime;

            if (ANow >= ANow.Date.AddMinutes(30) && ANow <= ANow.Date.AddMinutes(3 * 60 + 30))
            {
                StartTime = ANow.Date;
                TimeSpan ts1 = ANow.Subtract(StartTime);
                i = (ts1.Hours * 60 + ts1.Minutes - 30) / 20 + 1;
            }
            else if (ANow >= ANow.Date.AddHours(7).AddMinutes(30))
            {
                StartTime = ANow.Date.AddHours(7);
                TimeSpan ts1 = ANow.Subtract(StartTime);
                i = (ts1.Hours * 60 + ts1.Minutes - 30) / 20 + 10;
            }
            return i;
        }


        /// <summary>
        /// 获取源代码
        /// </summary>
        /// <param name=”url”></param>
        /// <returns></returns>
        public static string GetHtml(string url, Encoding encoding)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 20000;
                request.AllowAutoRedirect = false;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK && response.ContentLength < 1024 * 1024)
                {
                    if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                        reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress), encoding);
                    else
                        reader = new StreamReader(response.GetResponseStream(), encoding);
                    string html = reader.ReadToEnd();
                    return html;
                }
            }
            catch
            {
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (reader != null)
                    reader.Close();
                if (request != null)
                    request = null;
            }
            return string.Empty;
        }


        #endregion



        #region  从网络上取数据

        /// <summary>
        /// AType:-1取那天全部的数据，为其它时取特定某期数据
        /// </summary>
        /// <param name="ADateTime"></param>
        /// <param name="listPeriod"></param>
        /// <param name="AType"></param>
        /// <returns></returns>
        public static bool GetHtmlString08(string AUrl,DateTime ADateTime, ref List<PeriodDetail_101> listPeriod, int AType = -1)
        {
            listPeriod.Clear();
            string LinkOf08Parse = string.Empty;
            LinkOf08Parse = string.Format(LinkOf08, ADateTime.ToString("yyyy-MM-dd"), '&', ADateTime.ToString("yyyyMMdd"));
            FileLog.WriteInfo("url", LinkOf08Parse);
            string strResult = string.Empty;

            if (IsSetUpCook.Equals("1"))
            {
                if (String.IsNullOrWhiteSpace(strCookier))
                {
                    strResult = GetHtml(LinkOf08Parse, Encoding.UTF8, "", -1);
                    Regex rgx = new Regex(@"ddos=([^]]*)\;\s");
                    Match match11 = rgx.Match(strResult);
                    string classification = match11.Groups[1].Value;
                    Console.WriteLine("classification:" + classification);
                    strCookier = "ddos=" + classification;
                }
                strResult = GetHtml(LinkOf08Parse, Encoding.UTF8, strCookier, 1);
            }
            else
            {
                strResult = GetHtml(LinkOf08Parse, Encoding.UTF8);
            }


            try
            {
                string regex = @"<td>\d{11}</td>[\s\S]*?<td>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}</td>[\s\S]*?<td><span>\d{1}</span><span>\d{1}</span><span>\d{1}</span><span>\d{1}</span><span>\d{1}</span></td>";
                Regex re = new Regex(regex);
                MatchCollection matches = re.Matches(strResult);
                System.Collections.IEnumerator enu = matches.GetEnumerator();
                while (enu.MoveNext() && enu.Current != null)
                {
                    string strPeriod = string.Empty;
                    string strAllValue = string.Empty;
                    Match match = (Match)(enu.Current);
                    if (match.Groups.Count > 0)
                    {
                        String strTemp = match.Groups[0].Value;
                        regex = @"\d{11}";
                        Regex reTemp = new Regex(regex);
                        MatchCollection matchesTemp = reTemp.Matches(strTemp);
                        System.Collections.IEnumerator enuTemp = matchesTemp.GetEnumerator();
                        while (enuTemp.MoveNext() && enuTemp.Current != null)
                        {
                            match = (Match)(enuTemp.Current);
                            strPeriod = match.Groups[0].Value;
                        }

                        regex = @"<span>\d{1}</span>";
                        reTemp = new Regex(regex);
                        matchesTemp = reTemp.Matches(strTemp);
                        enuTemp = matchesTemp.GetEnumerator();
                        StringBuilder sb = new StringBuilder();
                        while (enuTemp.MoveNext() && enuTemp.Current != null)
                        {
                            match = (Match)(enuTemp.Current);
                            sb.Append(match.Groups[0].Value.Substring(6, 1));
                        }
                        strAllValue = sb.ToString();
                    }
                    int Xingqi = 0;
                    switch (ADateTime.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            Xingqi = 5;
                            break;
                        case DayOfWeek.Monday:
                            Xingqi = 1;
                            break;
                        case DayOfWeek.Saturday:
                            Xingqi = 6;
                            break;
                        case DayOfWeek.Sunday:
                            Xingqi = 7;
                            break;
                        case DayOfWeek.Thursday:
                            Xingqi = 4;
                            break;
                        case DayOfWeek.Tuesday:
                            Xingqi = 2;
                            break;
                        case DayOfWeek.Wednesday:
                            Xingqi = 3;
                            break;
                        default:
                            Xingqi = 0;
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(strPeriod) && !string.IsNullOrWhiteSpace(strAllValue))
                    {
                        PeriodDetail_101 period = new PeriodDetail_101();
                        char[] charTemp = strAllValue.ToCharArray();
                        if (charTemp.Length == 5)
                        {
                            period.Wei5_050 = int.Parse(charTemp[0].ToString());
                            period.Wei4_040 = int.Parse(charTemp[1].ToString());
                            period.Wei3_030 = int.Parse(charTemp[2].ToString());
                            period.Wei2_020 = int.Parse(charTemp[3].ToString());
                            period.Wei1_010 = int.Parse(charTemp[4].ToString());
                            int total = period.Wei5_050 + period.Wei4_040 + period.Wei3_030 + period.Wei2_020 + period.Wei1_010;
                            period.AllSub_009 = total;
                            if (total >= 23)
                            {
                                period.BigOrSmall_007 = 1;
                            }
                            else
                            {
                                period.BigOrSmall_007 = 2;
                            }

                            if (total % 2 == 1)
                            {
                                period.EvenODD_008 = 1;
                            }
                            else
                            {
                                period.EvenODD_008 = 2;
                            }
                            period.DateTimeInsert_003 = DateTime.Now;
                            period.DateNumber_004 = Int64.Parse(ADateTime.ToString("yyyyMMdd"));
                            period.ShortPeriod_005 = int.Parse(strPeriod.Substring(8));
                            period.LongPeriod_001 = Int64.Parse(strPeriod);
                            period.AwardNumber_002 = strAllValue;
                            period.DayInWeek_006 = Xingqi;
                            listPeriod.Add(period);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                listPeriod = listPeriod.OrderBy(p => p.LongPeriod_001).ToList();
                if (AType != -1)
                {
                    if (listPeriod.Where(p => p.ShortPeriod_005 == AType).Count() > 0)
                    {
                        PeriodDetail_101 pTemp = listPeriod.Where(p => p.ShortPeriod_005 == AType).First();
                        FileLog.WriteInfo("GetHtmlString08--Get Period's PeriodLong001", pTemp.LongPeriod_001.ToString());
                        listPeriod.Clear();
                        listPeriod.Add(pTemp);
                    }
                    else
                    {
                        listPeriod.Clear();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 获取源代码
        /// </summary>
        /// <param name=”url”></param>
        /// <returns></returns>
        public static string GetHtml(string url, Encoding encoding, string strCookie, int Atype = -1)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            string html = string.Empty;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 5000;
                //request.AllowAutoRedirect = false;
                //request.Headers.Set("Pragma", "no-cache");
                request.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Mobile Safari/537.36";

                if (Atype != -1)
                {
                    request.Headers.Add("Cookie", strCookie);
                }


                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK && response.ContentLength < 1024 * 1024)
                {
                    if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                        reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress), encoding);
                    else
                        reader = new StreamReader(response.GetResponseStream(), encoding);
                    html = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("请求中报错：", ":" + e.Message.ToString());
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                    response = null;
                }
                if (reader != null)
                    reader.Close();
                if (request != null)
                    request = null;
            }
            return html;
        }

 
         

        #endregion


        public void ShiShiCaiDataStartup()
        {
            FileLog.WriteInfo("ShiShiCaiData", "ShiShiCaiDataStartup()");
            IBoolIsThreadShiShiCaiDataWorking = false;
            if (IThreadShiShiCaiData != null)
            {
                IThreadShiShiCaiData.Abort();
            }
            IThreadShiShiCaiData = new Thread(new ParameterizedThreadStart(ShiShiCaiData.AutoShishiCaiDataGet));
            IBoolIsThreadShiShiCaiDataWorking = true;
            IThreadShiShiCaiData.Start(this);
            FileLog.WriteInfo("ShiShiCaiDataStartup()", "IThreadShiShiCaiData Start");
        }


        public void ShiShiCaiDataStop()
        {
            FileLog.WriteInfo("ShiShiCaiDataStop() ", "start");
            IBoolIsThreadShiShiCaiDataWorking = false;
            if (IThreadShiShiCaiData != null)
            {
                IThreadShiShiCaiData.Abort();
                IThreadShiShiCaiData = null;
            }
        }

    }
}
