using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.BuiltIns.Numbers;
using LiveLisp.Core.Utils;

namespace LiveLisp.Core.BuiltIns.Characters
{
    public static class LispChar
    {
        public static int Code(this char ch)
        {
            return ch;
        }

        public static char ToUpper(this char ch)
        {
            return char.ToUpper(ch);
        }
            
    }
}
