using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SSCService03
{
    public partial class SSCService03 : ServiceBase
    {
        bool IBoolStatisticsThreadWorking = false;
        StatisticsData IStatisticsData = new StatisticsData();

        public SSCService03(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
            {
                Console.WriteLine(args[0]);
                if (args[0].Trim().ToLower() == "-start")
                {
                    OnStart(args);
                }
            }
        }

        public static void StatisticsThreadWorking(object o)
        {
            try
            {
                SSCService03 shishicai01 = o as SSCService03;
                if (shishicai01.IBoolStatisticsThreadWorking)
                    shishicai01.IStatisticsData.StatisticsStartup();
                FileLog.WriteInfo("SSCService03", "StatisticsThreadWorking Startup()");
            }
            catch (Exception ex)
            {
                FileLog.WriteError("StatisticsThreadWorking() ", ex.Message);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Thread shishiCai01Thread = new Thread(StatisticsThreadWorking);
                IBoolStatisticsThreadWorking = true;
                shishiCai01Thread.Start(this);
                FileLog.WriteInfo("SSCService03", "StatisticsThreadWorking Start()");
            }
            catch (Exception ex)
            {

                FileLog.WriteError("SSCService03 OnStart() ", ex.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                IStatisticsData.StatisticsStop();
            }
            catch (Exception ex)
            {

                FileLog.WriteError("ZQStatistics OnStop() ", ex.Message);
            }
        }
    }
}
