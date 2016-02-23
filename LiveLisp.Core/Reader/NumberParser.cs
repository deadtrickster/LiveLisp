using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using LiveLisp.Core.Reader;
using LiveLisp.Core.Types;
using LiveLisp.Core.Utils;
using LiveLisp.Core.BuiltIns.Numbers;

namespace LiveLisp.Core.Reader
{
    public class NumberParser
    {
        private const string HexPref = "0x";
        public NumberParser()
        {
        }

        internal static bool TryParse(string literal, out object ret)
        {
            ret = null;
            if (literal.Contains(Readtable.Current.RationalSplit))
            {
                // возможно мы имеем дело с объектом Rational
                string dividend = literal.Substring(0, literal.IndexOf(Readtable.Current.RationalSplit));
                string divisor = literal.Substring(literal.IndexOf(Readtable.Current.RationalSplit) + 1);

                object dvdend = 0;
                object dvsor = 0;
                if (TryInt64(dividend, ref dvdend) && TryInt64(divisor, ref dvsor))
                {
                    ret = new Ratio((Int64)dvdend, (Int64)dvsor);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (literal.ToUpper().EndsWith("F"))
            {
                if (TrySingle(literal.Substring(0, literal.Length -1), ref ret))
                {
                    return true;
                }
                else
                {
                    return TryDouble(literal.Substring(0, literal.Length - 1), ref ret);
                }
            }

            if (literal.ToUpper().EndsWith("M"))
            {
                return TryDecimal(literal.Substring(0, literal.Length - 1), ref ret);
            }

            bool signed = false;
            if (literal[0] == '-' || literal[0] == '+')
            {
                // явно указан знак так что беззнаковые типы не рассматриваются
                signed = true;
            }

            string base10literal;

            int presentedBase;

            if (literal.Length > 3 && literal.Substring(1, 2) == HexPref)
            {
                // если в начале числа идЁт 0x значит текущее основание readtable игнорируется 
                // и число обрабатывается по основанию 16.
                presentedBase = 16;
            }
            else
            {
                presentedBase = Readtable.Current.NumBase;
            }

            if (presentedBase != 10)
            {
                /* if (!RadixConverter.Convert(literal, presentedBase, 10, out base10literal))
                 {
                     throw new FormattedException(Resources.InvalidBaseRNumberFormat, presentedBase);
                 }*/

                base10literal = RadixConverter.Convert(presentedBase, 10, literal);
            }
            else
            {
                base10literal = literal;
            }

            // сейчас мы удостоверились что число имеет правильный формат(тоесть что литерал это число)
            // строка base10literal содержит десятичное предстваление числа
            

            if (TryInt32(base10literal, ref ret))
            {
                return true;
            }

            if (!signed && TryUInt32(base10literal, ref ret))
            {
                return true;
            }

            if (TryInt64(base10literal, ref ret))
            {
                return true;
            }

            if (!signed && TryUInt64(base10literal, ref ret))
            {
                return true;
            }

            if (TryBigInteger(base10literal, ref ret))
            {
                return true;
            }

            if (TrySingle(base10literal, ref ret))
            {
                return true;
            }

            if (TryDouble(base10literal, ref ret))
            {
                return true;
            }

        /*    if (TryDecimal(base10literal, ref ret))
            {
                return true;
            }
            */
            return false;

        }

        
        #region Parse wrappers
        static int tint;
        private static bool TryInt32(string base10literal, ref object ret)
        {
            if (Int32.TryParse(base10literal, out tint))
            {
                ret = tint;
                return true;
            }
            else
            {
                return false;
            }
        }
        static uint tuint;
        private static bool TryUInt32(string base10literal, ref object ret)
        {
            if (UInt32.TryParse(base10literal, out tuint))
            {
                ret = tuint;
                return true;
            }
            else
            {
                return false;
            }
        }
        static Int64 tint64;
        private static bool TryInt64(string base10literal, ref object ret)
        {
            if (Int64.TryParse(base10literal, out tint64))
            {
                ret = tint64;
                return true;
            }
            else
            {
                return false;
            }
        }
        static UInt64 tuint64;
        private static bool TryUInt64(string base10literal, ref object ret)
        {
            if (UInt64.TryParse(base10literal, out tuint64))
            {
                ret = tuint64;
                return true;
            }
            else
            {
                return false;
            }
        }
        static BigInteger tbig;
        private static bool TryBigInteger(string base10literal, ref object ret)
        {
            if (BigInteger.TryParse(base10literal, out tbig))
            {
                ret = tbig;
                return true;
            }
            else
            {
                return false;
            }
        }

        static Single tsingle;
        private static bool TrySingle(string base10literal, ref object ret)
        {
            if (Single.TryParse(base10literal, out tsingle))
            {
                ret = tsingle;
                return true;
            }
            else
            {
                return false;
            }
        }
        static Double tdouble;
        private static bool TryDouble(string base10literal, ref object ret)
        {
            if (Double.TryParse(base10literal, out tdouble))
            {
                ret = tdouble;
                return true;
            }
            else
            {
                return false;
            }
        }
        static Decimal tdecimal;
        private static bool TryDecimal(string base10literal, ref object ret)
        {
            if (Decimal.TryParse(base10literal, out tdecimal))
            {
                ret = tdecimal;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
