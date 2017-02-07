using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormDemo.CommUtil
{
    class BaseUtil
    {
        public static int ParseInt(string src)
        {
            int rst = 0;
            int.TryParse(src, out rst);
            return rst;
        }
    }
}
