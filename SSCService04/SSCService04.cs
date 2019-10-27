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

namespace SSCService04
{
    public partial class SSCService04 : ServiceBase
    {

        bool IBoolMouTouThreadWorking = false;
        MoTouStatistics IMoTouStatistics = new MoTouStatistics();

        public SSCService04(string[] args)
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

        public static void MoTouStatisticsThreadWorking(object o)
        {
            try
            {
                SSCService04 sscService04 = o as SSCService04;
                if (sscService04.IBoolMouTouThreadWorking )
                {
                    sscService04.IMoTouStatistics.MoTouStaticsStartup();
                }
            }
            catch (Exception ex)
            {
                
                 FileLog.WriteError("MoTouStatisticsThreadWorking() ", ex.Message);
            }
              
         
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Thread MoTouThread = new Thread(MoTouStatisticsThreadWorking);
                IBoolMouTouThreadWorking = true;
                MoTouThread.Start(this);

            }
            catch (Exception ex)
            {

                FileLog.WriteError("OnStart() ", ex.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                IMoTouStatistics.MoTouStaticsStop();
            }
            catch (Exception ex)
            {

                FileLog.WriteError("OnStop() ", ex.Message);
            }
        }
    }
}
