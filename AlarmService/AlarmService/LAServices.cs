using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlarmService
{
    public partial class LAServices : ServiceBase
    {
        bool _isThreadWorking = false;
        RunService rc = new RunService();
        public LAServices(string[] args)
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

        protected override void OnStart(string[] args)
        {
            try
            {
                Thread rct = new Thread(new ParameterizedThreadStart(LAServices.ThreadWorking));

                _isThreadWorking = true;
                rct.Start(this);
                FileLog.WriteInfo("Start thread", "ThreadWorking()");
            }
            catch (Exception ex)
            {
                FileLog.WriteError("ThreadWorking.Startup", ex.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                rc.Shutdown();
            }
            catch (System.Exception e)
            {
                FileLog.WriteError("SyncService.Shutdown", e.Message);
            }
        }

        public static void ThreadWorking(object obj)
        {
            try
            {
                FileLog.WriteInfo("thread begin", "ThreadWorking()");
                LAServices HHSyncService = obj as LAServices;
                if (HHSyncService._isThreadWorking)
                    HHSyncService.rc.Startup();
                else
                    FileLog.WriteInfo("ThreadWorking", "_isThreadWorking is false.");
            }
            catch (Exception ex)
            {
                FileLog.WriteError("ThreadWorking", ex.Message);
            }
        }
    }
}
