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

namespace SSCService02
{
    public partial class SSCService02 : ServiceBase
    {


        bool IBoolShiShiCai01ThreadWorking = false;
        ShiShiCaiData IShiShiCaiDataGet = new ShiShiCaiData();

        public SSCService02(string[] args)
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
                Thread shishiCai01Thread = new Thread(ShiShiCai01ThreadWorking);
                IBoolShiShiCai01ThreadWorking = true;
                shishiCai01Thread.Start(this);
                FileLog.WriteInfo("SSCService02", "SSCService02Thread Start()");
            }
            catch (Exception ex)
            {

                FileLog.WriteError("SSCService02 OnStart() ", ex.Message);
            } 
        }

        protected override void OnStop()
        {
            try
            {
                IShiShiCaiDataGet.ShiShiCaiDataStop();
            }
            catch (Exception ex)
            {
                FileLog.WriteError("SSCService02 OnStop() ", ex.Message);
            }
        }

        public static void ShiShiCai01ThreadWorking(object o)
        {
            try
            {
                SSCService02 shishicai01 = o as SSCService02;
                if (shishicai01.IBoolShiShiCai01ThreadWorking)
                    shishicai01.IShiShiCaiDataGet.ShiShiCaiDataStartup();
                FileLog.WriteInfo("SSCService02ThreadWorking() ", "SSCService02 Startup()");
            }
            catch (Exception ex)
            {

                FileLog.WriteError("SSCService02ThreadWorking() ", ex.Message);
            }

        }
    }
}
