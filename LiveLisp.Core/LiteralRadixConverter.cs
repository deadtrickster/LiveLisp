using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.Utils
{
        //UNDONE: Otimize it
        //UGLY: RadixConverter
        public class RadixConverter
        {

            /// <summary>
            /// Lookup tables for strings
            /// </summary>
            static string digit = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            /// <summary>
            /// Error Messages
            /// </summary>
            static string ErrRadixTooLarge1 = "RadixError: radix larger than 36.";
            static string ErrRadixTooLarge2 = "RadixError: radix larger than 1000000.";
            static string ErrRadixTooSmall = "RadixError: radix smaller than 2.";
            static string ErrRadixFormat = "RadixError: number not in radix format.";
            static string ErrRadixDecode = "RadixError: generic decode error.";
            static string ErrRadixNoSymbolFormat = "RadixError: number not in symbolic format.";

            public static string Spaces(string val, int nr)
            {
                return Spaces(val, nr, ' ');
            }

            public static string Spaces(string val, int nr, char sep)
            {
                string rv = "";
                int j = 0;
                for (int i = val.Length - 1; i >= 0; i--)
                {
                    j++;
                    rv = val[i] + rv;
                    if (j % nr == 0) rv = sep + rv;
                }
                if (rv[0] == sep) rv = rv.Substring(1);
                return rv;
            }
            /// <summary>
            /// CheckArg checks the arguments for the encoder and decoder calls
            /// </summary>
            private static void CheckArg(long radix, bool sym)
            {
                if ((radix > 36) && (sym == false))
                {
                    throw new Exception(ErrRadixTooLarge1);
                }
                if (radix > 1000000)
                {
                    throw new Exception(ErrRadixTooLarge2);
                }
                if (radix < 2)
                {
                    throw new Exception(ErrRadixTooSmall);
                }
            }

            //////////////////////////////////////////////////////////////
            /// LONG CODE
            //////////////////////////////////////////////////////////////

            public static string Encode(long x, long radix)
            {
                return Encode(x, radix, false);
            }
            /// <summary>
            /// Encoder for a long to a string with the base [radix]. if sym is true
            /// the number will be converted to a generic symbolic notation.
            /// </summary>
            public static string Encode(long x, long radix, bool sym)
            {
                // check parameters
                CheckArg(radix, sym);

                // work in positive domain
                long t = Math.Abs(x);

                // return value
                string rv = "";

                if (sym)
                {
                    if (t == 0)
                    {
                        rv = ",0";
                    }
                    while (t > 0)
                    {
                        // split of one digit
                        long r = t % radix;
                        // convert it and add it to the return string
                        rv = "," + r.ToString() + rv;
                        t = (t - r) / radix;
                    }
                    rv = rv.Substring(1);			// strip one ','
                    // add sign
                    if (x < 0)
                    {
                        rv = "-," + rv;
                    }
                    if (x == 0)
                    {
                        rv = "0";
                    }
                    rv = "[(" + radix + ")," + rv + "]";
                }
                else
                {
                    if (t == 0)
                    {
                        rv = "0";
                    }
                    while (t > 0)
                    {
                        // split of one digit
                        long r = t % radix;
                        // convert it and add it to the return string
                        rv = digit[(int)r] + rv;
                        t = (t - r) / radix;
                    }
                    if (x < 0)
                    {
                        // add sign
                        rv = "-" + rv;
                    }
                    if (x == 0)
                    {
                        rv = "0";
                    }
                }
                return rv;
            }

            public static void Decode(string val, long radix, out long rv)
            {
                Decode(val, radix, out rv, false);
            }
            /// <summary>
            /// Decoder for a string to a long with the base [radix]. if sym is true
            /// the number will be converted from a generic symbolic notation.
            /// </summary>
            public static void Decode(string val, long radix, out long rv, bool sym)
            {
                CheckArg(radix, sym);
                rv = 0;
                try
                {
                    if (sym)
                    {
                        string ws = val.Trim();
                        if (ws[0] != '[')
                        {
                            throw new Exception(ErrRadixNoSymbolFormat);
                        }
                        // strip [(
                        ws = ws.Substring(2);
                        // get radix
                        int pos = ws.IndexOf(')');
                        long tr = Int64.Parse(ws.Substring(0, pos));
                        // strip it
                        ws = ws.Substring(pos + 2);
                        ws = ws.Remove(ws.Length - 1, 1);		// strip ]

                        char sign = ws[0];
                        int si = 1;
                        if ((sign == '-') || (sign == '+'))
                        {
                            if (sign == '-') si = -1;
                            ws = ws.Substring(2);				// skip sign and ,
                        }

                        string[] t = ws.Split(',');
                        for (int i = 0; i < t.Length; i++)
                        {
                            rv *= radix;
                            long l = long.Parse(t[i]);
                            if (l >= radix) throw new Exception(ErrRadixFormat);
                            rv += l;
                        }
                        // add sign
                        rv *= si;
                    }
                    else
                    {
                        string ws = val.Trim();
                        char sign = ws[0];
                        int si = 1;
                        if ((sign == '-') || (sign == '+'))
                        {
                            if (sign == '-') si = -1;
                            ws = ws.Substring(1);
                        }

                        for (int i = 0; i < ws.Length; i++)
                        {
                            rv *= radix;
                            char c = ws[i];
                            long l = digit.IndexOf(c);
                            if (l >= radix) throw new Exception(ErrRadixFormat);
                            rv += l;
                        }
                        // add sign
                        rv *= si;
                    }
                }
                catch
                {
                    throw new Exception(ErrRadixDecode);
                }

                return;
            }

            //////////////////////////////////////////////////////////////
            /// FLOATING POINT CODE
            //////////////////////////////////////////////////////////////

            public static string Encode(double x, long radix)
            {
                return Encode(x, radix, false);
            }

            public static string Encode(double x, long radix, bool sym)
            {
                // Check parameters
                CheckArg(radix, sym);

                double t = Math.Abs(x);

                // first part before decimal point
                long t1 = (long)t;

                // t2 holds part after decimal point
                double t2 = t - t1;

                // return value;
                string rv = "";

                if (sym)
                {
                    if (x == 0.0)
                    {
                        rv = ",0";
                    }
                    // process part before decimal point
                    while (t1 > 0)
                    {
                        long r = t1 % radix;
                        rv = "," + r.ToString() + rv;
                        t1 = (t1 - r) / radix;
                    }
                    rv = rv.Substring(1);	// strip one ','

                    // after the decimal point
                    if (t2 > 0.0)
                    {
                        rv += ",.,";
                    }
                    int maxdigit = 50; // to prevent endless loop
                    while (t2 > 0)
                    {
                        long r = (long)(t2 * radix);
                        rv += r.ToString() + ",";
                        t2 = (t2 * radix) - r;

                        // forced break after maxdigits
                        maxdigit--;
                        if (maxdigit == 0) break;
                    }
                    rv = rv.Substring(0, rv.Length - 1);	// strip one ','
                    if (x < 0)
                    {
                        rv = "-," + rv;
                    }
                    rv = "[(" + radix + ")," + rv + "]";
                }
                else
                {
                    if (x == 0.0)
                    {
                        rv = "0";
                    }
                    // process part before decimal point
                    while (t1 > 0)
                    {
                        long r = t1 % radix;
                        rv = digit[(int)r] + rv;
                        t1 = (t1 - r) / radix;
                    }

                    // after the decimal point
                    if (t2 > 0.0)
                    {
                        rv += ".";
                    }
                    int maxdigit = 50; // to prevent endless loop
                    while (t2 > 0)
                    {
                        long r = (long)(t2 * radix);
                        rv += digit[(int)r];
                        t2 = (t2 * radix) - r;

                        // forced break after 10 digits
                        maxdigit--;
                        if (maxdigit == 0) break;
                    }
                    if (x < 0)
                    {
                        rv = "-" + rv;
                    }
                }
                return rv;
            }

            public static void Decode(string val, long radix, out double rv)
            {
                Decode(val, radix, out rv, false);
            }
            public static void Decode(string val, long radix, out double rv, bool sym)
            {
                CheckArg(radix, sym);
                rv = 0;

                try
                {
                    double tradix = 1;
                    if (sym)
                    {
                        string ws = val.Trim();
                        // strip [(
                        ws = ws.Substring(2);
                        // get radix
                        int pos = ws.IndexOf(')');
                        long tr = Int64.Parse(ws.Substring(0, pos));
                        // strip it
                        ws = ws.Substring(pos + 2);
                        ws = ws.Remove(ws.Length - 1, 1);		// strip ]

                        char sign = ws[0];
                        int si = 1;
                        if ((sign == '-') || (sign == '+'))
                        {
                            if (sign == '-') si = -1;
                            ws = ws.Substring(2);					// skip sign and ,
                        }

                        string[] t = ws.Split(',');
                        bool before = true;
                        for (int i = 0; i < t.Length; i++)
                        {
                            if (t[i] == ".")
                            {
                                before = false;
                                continue;
                            }
                            // next 'digit'
                            long l = long.Parse(t[i]);
                            if (l >= radix) throw new Exception(ErrRadixFormat);

                            if (before)
                            {
                                // process before dec. point
                                rv *= radix;
                                rv += l;
                            }
                            else
                            {
                                // process after decimal point
                                tradix *= radix;
                                rv += l / tradix;
                            }
                        }

                        // add sign
                        rv *= si;
                    }
                    else
                    {
                        string ws = val.Trim();
                        char sign = ws[0];
                        int si = 1;
                        if ((sign == '-') || (sign == '+'))
                        {
                            if (sign == '-') si = -1;
                            ws = ws.Substring(1);
                        }

                        bool before = true;
                        for (int i = 0; i < ws.Length; i++)
                        {
                            if (ws[i] == '.')
                            {
                                before = false;
                                continue;
                            }
                            // next 'digit'
                            long l = digit.IndexOf(ws[i]);
                            if (l >= radix) throw new Exception(ErrRadixFormat);

                            if (before)
                            {
                                // process before dec. point
                                rv *= radix;
                                rv += l;
                            }
                            else
                            {
                                // process after decimal point
                                tradix *= radix;
                                rv += digit.IndexOf(ws[i]) / tradix;
                            }
                        }
                        // add sign
                        rv *= si;
                    }
                }
                catch
                {
                    throw new Exception(ErrRadixDecode);
                }
                return;
            }

            public static String Convert(int from, int to, String s)
            {
                //Return error if input is empty
                if (String.IsNullOrEmpty(s))
                {
                    return ("Error: Nothing in Input String");
                }
                //only allow uppercase input characters in string
                s = s.ToUpper();
                //only do base 2 to base 36 (digit represented by charecaters 0-Z)"
                if (from < 2 || from > 36 || to < 2 || to > 36) { return ("Base requested outside range"); }
                //convert string to an array of integer digits representing number in base:from
                int il = s.Length;
                int[] fs = new int[il];
                int k = 0;
                char? Sign = null;
                for (int i = s.Length - 1; i >= 0; i--)
                {
                    if (s[i] == '-' || s[i] == '+')
                    {
                        if (i == 0)
                        {
                            Sign = s[i];
                        }
                        else
                        {
                            return "";
                        }
                    }
                    else if (s[i] >= '0' && s[i] <= '9') { fs[k++] = (int)(s[i] - '0'); }
                    else
                    {
                        if (s[i] >= 'A' && s[i] <= 'Z') { fs[k++] = 10 + (int)(s[i] - 'A'); }
                        else
                        { return ("Error: Input string must only contain any of 0-9 or A-Z"); } //only allow 0-9 A-Z characters
                    }
                }
                //check the input for digits that exceed the allowable for base:from
                foreach (int i in fs)
                {
                    if (i >= from) { return ("Error: Not a valid number for this input base"); }
                }
                //find how many digits the output needs
                int ol = il * (from / to + 1);
                int[] ts = new int[ol + 10]; //assign accumulation array
                int[] cums = new int[ol + 10]; //assign the result array
                ts[0] = 1; //initialise array with number 1 
                //evaluate the output
                for (int i = 0; i < il; i++) //for each input digit
                {
                    for (int j = 0; j < ol; j++) //add the input digit times (base:to from^i) to the output cumulator
                    {
                        cums[j] += ts[j] * fs[i];
                        int temp = cums[j];
                        int rem = 0;
                        int ip = j;
                        do // fix up any remainders in base:to
                        {
                            rem = temp / to;
                            cums[ip] = temp - rem * to; ip++;
                            cums[ip] += rem;
                            temp = cums[ip];
                        }
                        while (temp >= to);
                    }
                    //calculate the next power from^i) in base:to format
                    for (int j = 0; j < ol; j++)
                    {
                        ts[j] = ts[j] * from;
                    }
                    for (int j = 0; j < ol; j++) //check for any remainders
                    {
                        int temp = ts[j];
                        int rem = 0;
                        int ip = j;
                        do  //fix up any remainders
                        {
                            rem = temp / to;
                            ts[ip] = temp - rem * to; ip++;
                            ts[ip] += rem;
                            temp = ts[ip];
                        }
                        while (temp >= to);
                    }
                }
                //convert the output to string format (digits 0,to-1 converted to 0-Z characters) 
                String sout = String.Empty; //initialise output string
                if (Sign.HasValue)
                    sout += Sign.Value;
                bool first = false; //leading zero flag
                for (int i = ol; i >= 0; i--)
                {
                    if (cums[i] != 0) { first = true; }
                    if (!first) { continue; }
                    if (cums[i] < 10) { sout += (char)(cums[i] + '0'); }
                    else { sout += (char)(cums[i] + 'A' - 10); }
                }
                if (String.IsNullOrEmpty(sout)) { return "0"; } //input was zero, return 0
                //return the converted string
                return sout;
            }

            public static int IsFigure(char ch, int numBase)
            {
                return digit.Substring(0, numBase).IndexOf(ch);
            }

            internal static char GetCharByWeight(int weight)
            {
                return digit[weight];
            }
        }
}
