using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlarmService
{
    public class RunService
    {
        /// <summary>
        /// 用于判断处理线程是否退出
        /// </summary>
        private bool _isalarmworking;
        ///<summary>
        ///处理线程
        /// </summary>
        private Thread _threadalarm;
        private static string _connStr = ConfigurationManager.AppSettings["conn"];
        private static string _toUser = ConfigurationManager.AppSettings["touser"];
        private static string _sleepSecond = ConfigurationManager.AppSettings["sleepsecond"];

        public RunService()
        {
            _isalarmworking = true;    
        }

        /// <summary>
        /// 启动处理线程的方法
        /// </summary>
        public void Startup()
        {
            FileLog.WriteInfo("SyncService Startup", " begin");
            //同步中间表订单数据到CMS线程
            _threadalarm = new Thread(new ParameterizedThreadStart(RunService.DoAlarm));
            _threadalarm.Start(this);
            _isalarmworking = true;
            FileLog.WriteInfo("Start thread", "DoSyncOrder2CMS()");
        }

        /// <summary>
        /// 系统处理结束
        /// </summary>
        internal void Shutdown()
        {
            try
            {
                FileLog.WriteInfo("Shutdown", "OnStop DoAlarm START");
                _isalarmworking = false;
                System.Threading.Thread.Sleep(2000);
                _threadalarm.Abort();
                FileLog.WriteInfo("Shutdown", "OnStop DoAlarm END");
            }
            catch (Exception ex)
            {
                FileLog.WriteError("Shutdown", "Service Exception:\n" + ex.Message);
            }
        }

        private static void DoAlarm(object o)
        {
            try
            {
                RunService runService = o as RunService;
                DateTime dateTimeNow, dateTimeOld;
                dateTimeOld = Convert.ToDateTime("2000-1-1 00:00:00");
                FileLog.WriteInfo("thread begin", "DoAlarm()");
                while (runService._isalarmworking)
                {
                    FileLog.DateCountSaveLog = 30;
                    FileLog.DeleteOldLog();
                    try
                    {
                        dateTimeNow = DateTime.Now;
                        TimeSpan timeSpan = dateTimeNow - dateTimeOld;
                        double timeSpanTotal = 0;
                        timeSpanTotal = timeSpan.TotalSeconds;
                        if (timeSpanTotal >= Convert.ToDouble(_sleepSecond))
                        {
                            dateTimeOld = dateTimeNow;
                            //----------------------获取订单数据以及明细数据，通过webservice插入CMS系统----------------------
                            DoSendMsg();
                            //----------------------获取订单数据以及明细数据，通过webservice插入CMS系统----------------------
                        }
                    }
                    catch(Exception ex)
                    {
                        FileLog.WriteError("DoAlarm", ex.Message);
                    }
                    Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteError("DoAlarm", ex.Message);
            }
        }

        private static void DoSendMsg()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connStr))
                {
                    using (SqlCommand commandcopy = new SqlCommand(@"select * from T_121_19 where C004=0", connection))
                    {
                        commandcopy.CommandType = CommandType.Text;
                        connection.Open();
                        SqlDataReader sdrcopy = commandcopy.ExecuteReader();

                        List<string> ids = new List<string>();
                        List<string> sonids = new List<string>();

                        while (sdrcopy.Read())
                        {
                            ids.Add(sdrcopy["C001"].ToString());
                            string touser = touser = _toUser;
                            string flag = sdrcopy["C004"].ToString();
                            sonids.Add(sdrcopy["C002"].ToString());
                            if (flag != "1")
                            {
                                string content = sdrcopy["C003"].ToString();
                                QYWeixinHelper.SendText(touser, content + " ->" + DateTime.Now.ToString());
                            }
                        }
                        sdrcopy.Close();
                        for (int i = 0; i < ids.Count; i++) 
                        {
                            string sql = String.Format("Update T_121_19 set C004=1  where C001={0}  AND C002={1}", ids[i], sonids[i]);
                            SqlCommand delCommand = new SqlCommand(sql, connection);

                            delCommand.ExecuteNonQuery();
                        }
                        //for (int i = 0; i < ids.Count; i++)
                        //{
                        //    string id = ids[i];
                        //    string date = dates[i];
                        //    string cdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //    string sql = "INSERT INTO T_WX_Send SELECT '" + id + "','" + date + "','1','" + cdate + "' WHERE NOT EXISTS(SELECT * FROM T_WX_Send WHERE GameID='" + id + "' AND Datetime='" + date + "')";
                        //    SqlCommand delCommand = new SqlCommand(sql, connection);

                        //    delCommand.ExecuteNonQuery();
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteError("DoSendMsg", ex.Message);
            }
        }

        #region ///
        
        private static void SendMessage()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connStr))
                {
                    //依赖是基于某一张表的,而且查询语句只能是简单查询语句,不能带top或*,同时必须指定所有者,即类似[dbo].[]    
                    using (SqlCommand command = new SqlCommand(@"select ALARM_ID,LOGGER_SN,LOGGER_NAME,GROUP_ID,GROUP_NAME,ALARM_LEVEL,
ALARM_MSG,MOBILE_NO,ALARM_CREATE_TIME,LAST_SEND_TIME,ALARM_STATE,ALARM_ACTION From dbo.TO_ALARMS_SMS", connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        SqlDependency dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                        FileLog.WriteInfo("sdr.Read() start", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        SqlDataReader sdr = command.ExecuteReader();
                        Console.WriteLine();
                        while (sdr.Read())
                        {
                        }
                        sdr.Close();
                        FileLog.WriteInfo("sdr.Read() end", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteError("SendMessage", ex.Message);
            }            
        }
        private static void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Info == SqlNotificationInfo.Insert)
            {
                DoSendMsg();
            }
            SendMessage();
        }
        #endregion
    }
}
