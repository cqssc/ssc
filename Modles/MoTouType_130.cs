using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    public class MoTouType_130
    {
        public int MoTouTypeID_001 { get; set; }
        public String StrMoTouDesc_002 { get; set; }
        public int BuyType_003 { get; set; } //1复位表示要买多个数字
        public decimal Per_004 { get; set; }
        public int StopTime_005 { get; set; }
        public int IsFanBei_006 { get; set; }
        public int IsRateAndAbs_007 { get; set; }
        public decimal RateAndAbs_008 { get; set; }
        public decimal StartMoney_009 { get; set; }
        public decimal TotalMoney_010 { get; set; }

        public long RunOverPeriod_011 { get; set; } //当前服务跑到那一期了
        public long FirstTimeStartPeriod_012 { get; set; } //休息多少期
        public int TimeAftretStartPeriod_013 { get; set; }  //20期内的第几次,超过20期后制0   

        public decimal LastLostMoney_014 { get; set; }  //上一期亏多少钱
        public long LastStartRulePeriod_015 { get; set; } //上一次规律的启动期号 
    }
}
