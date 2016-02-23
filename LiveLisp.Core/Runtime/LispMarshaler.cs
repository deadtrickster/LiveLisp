using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.Runtime
{
    public static class LispMarshaler
    {
        public static object ToClr(object obj)
        {
            if (obj == DefinedSymbols.T)
                return true;
            else if (obj == DefinedSymbols.NIL)
                return false;

            else return obj;
        }

        public static object[] ToClr(object[] obj)
        {
            object[] ret = new object[obj.Length];

            for (int i = 0; i < obj.Length; i++)
            {
                ret[i] = ToClr(obj[i]);
            }

            return ret;
        }
    }
}
