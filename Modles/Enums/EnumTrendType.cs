using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles.Enums
{
    //走势遗漏表Type
     public enum EnumTrendType
    {
        //1重复 2振荡 3 递增减、4单独（两个重复间）

        Repick=1,
        ConAdd=2,
        ConSub=3,
        Swing=4

    }
}
