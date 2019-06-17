using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CI.CIUtility.Utility;

namespace SSCService01
{
    public class ServiceEntry:VLService
    {
        public override bool Stop()
        {
            Logger.Info(GetType(), "Service will Stop!");
            Main.Instance.Stop();
            return base.Stop();
        }

        public override bool Start()
        {
            Logger.Info(GetType(), "Service Start!");
            Main.Instance.Start();
            return base.Start();
        }

        public override void Run()
        {
            Logger.Info(GetType(), "Service Run");
        }
    }
}
