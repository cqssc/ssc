using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modles
{

    //横向遗漏表类型
    enum EnumCrossType_104
    {
        //  1大 2小 3单 4双  5 >20期 6 20期老1  7一对 8 豹子
        Big=1,
        Small=2,
        Even=3,
        Odd=4,
        Big20=5,
        Big20Max=6,
        Pair=7,
        ThreeSame=8

    }
}
