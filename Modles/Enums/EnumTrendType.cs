using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{
    //走势遗漏表Type
    enum EnumTrendType
    {
        //1重复 2振荡 3 递增减、4单独（两个重复间）

        Repick=1,
        Swing=2,
        AddOrSub=3,
        Other=4

    }
}
