using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core
{
    public static class Extensions
    {
        public static string InvertCase(this string str)
        {
            char[] ar = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsLower(str[i]))
                    ar[i] = char.ToUpper(str[i]);
                else
                    ar[i] = char.ToLower(str[i]);
            }

            return new String(ar);
        }

        public static string SetCase(this string str)
        {
            return str;
        }

    }
}
