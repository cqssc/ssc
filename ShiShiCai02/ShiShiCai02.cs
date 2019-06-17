using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace ShiShiCai02
{
    public partial class ShiShiCai02 : ServiceBase
    {
        bool IFlagStaisticsThreadWorking = false;
        StatisticsManager statisticsManager = new StatisticsManager();

        public ShiShiCai02(string[] args)
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

        public static void StaisticsManagerThreadWorking(object o)
        {
            try
            {
                ShiShiCai02 shishicai02Statistics = o as ShiShiCai02;
                if (shishicai02Statistics.IFlagStaisticsThreadWorking)
                    shishicai02Statistics.statisticsManager.StatisticsStartup();
                FileLog.WriteInfo("ShiShiCai02 ", "StaisticsManagerThreadWorking Startup()");

            }
            catch (Exception ex)
            {
                FileLog.WriteError("ShiShiCai02--StaisticsManagerThreadWorking() ", ex.Message);
            }

        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Thread shishiCai02StatisticsThread = new Thread(StaisticsManagerThreadWorking);
                IFlagStaisticsThreadWorking = true;
                shishiCai02StatisticsThread.Start(this);
                FileLog.WriteInfo("ShiShiCai02", " Start()");

            }
            catch (Exception ex)
            {

                FileLog.WriteError("ShiShiCai02 OnStart() ", ex.Message);
            } 
        }

        protected override void OnStop()
        {
            try
            {
                statisticsManager.StatisticsStop();
            }
            catch (Exception ex)
            {
                FileLog.WriteError("ShiShiCai02 OnStop()", ex.Message);
            }
        }
    }
}
