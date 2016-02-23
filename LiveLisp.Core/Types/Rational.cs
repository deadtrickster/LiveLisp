using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using LiveLisp.Core.Utils;

namespace LiveLisp.Core.Types
{
    public struct Ratio : ILispNumber
    {
        public Int64 Numerator
        {
            get;
            private set;
        }
        public Int64 Denominator
        {
            get;
            private set;
        }

        public Ratio(Int64 numerator)
            : this(numerator, 1)
        {

        }

        public Ratio(Int64 numerator, Int64 denominator)
            :this()
        {
            Denominator = denominator;

            Numerator = numerator;

            if (Denominator == 0)
            {
                throw new System.DivideByZeroException("Attempt to set a denominator to zero.");
            }

            Normalize();
        }

        private void Normalize()
        {
            long t = GCD(Numerator, Denominator);

            if (t != 1)
            {

                Numerator /= t;

                Denominator /= t;

            }
        }

        private long GCD(long a, long b)
        {
            if (a == 0)
            {
                return 1;
            }
            if (a < b) return GCD(b, a);

            long c = 1;

            while (c != 0)
            {

                c = a % b;

                a = b;

                b = c;

            }

            return a;
        }

        #region overrides

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool alwaysPrintRatio)
        {
            if (Denominator == 1 && !alwaysPrintRatio)
            {
                return Numerator.ToString("g", NumberFormatInfo.InvariantInfo);
            }
            else
            {
                return Numerator.ToString("g", NumberFormatInfo.InvariantInfo) + "/" + Denominator.ToString("g", NumberFormatInfo.InvariantInfo);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Ratio)
            {
                return this == ((Ratio)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region opeartors

        public static Ratio operator *(Ratio r1, Ratio r2)
        {

            return new Ratio(r1.Numerator * r2.Numerator, r1.Denominator * r2.Denominator);

        }



        public static Ratio operator +(Ratio r1, Ratio r2)
        {

            return new Ratio(r1.Numerator * r2.Denominator +

                r2.Numerator * r1.Denominator,

                r1.Denominator * r2.Denominator);

        }



        public static Ratio operator -(Ratio r1, Ratio r2)
        {

            return new Ratio(r1.Numerator * r2.Denominator -

                r2.Numerator * r1.Denominator,

                r1.Denominator * r2.Denominator);

        }



        public static bool operator ==(Ratio f, Ratio s)
        {
            if (f.Numerator == s.Numerator)
            {
                if(f.Denominator == s.Denominator)
                return true;
            }

                return false;
        }

        public static bool operator !=(Ratio f, Ratio s)
        {
            return !(f == s);
        }

        #endregion

        #region conversion operators
        /*public static explicit operator double(Rational r)
        {
            Int64 ret = r.Numerator / r.Denominator;
            return Int64.ToDouble(ret);
        }*/
        public static implicit operator double(Ratio r)
        {
            double ret = (double)r.Numerator / (double)r.Denominator;
            return ret;
        }

        public static explicit operator Int32(Ratio r)
        {
            Int64 ret = r.Numerator / r.Denominator;
            return (Int32)ret;
        }

        public static implicit operator Ratio(Int32 i)
        {
            Ratio r = new Ratio(i);
            return r;
        }

        public static implicit operator Int64(Ratio r)
        {
            Int64 ret = r.Numerator / r.Denominator;
            return ret;
        }

        #endregion

        public static Ratio Parse(string str, int fromBase)
        {
            string[] nums = str.Split('/');
            if (nums.Length > 2)
            {
                throw new FormatException("Too many '/' in token");
            }

            String numeratorString = RadixConverter.Convert(fromBase, 10, nums[0]);
            Ratio r = new Ratio(Int64.Parse(numeratorString, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowCurrencySymbol, NumberFormatInfo.InvariantInfo));

            if (nums.Length == 2)
            {
                string denominatorString = RadixConverter.Convert(fromBase, 10, nums[1]);
                r.Denominator = Int64.Parse(denominatorString, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowCurrencySymbol, NumberFormatInfo.InvariantInfo);
            }

            return r;
        }

        internal static bool TryParse(string str, int Base, out Ratio r)
        {
            r = new Ratio();

            try
            {
                r = Parse(str, Base);
                return true;
            }
            catch
            {
                return false;
            }

        }

        #region ICloneable Members

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            Ratio r = ((Ratio)this) - ((Ratio)obj);

            return (int)(r.Numerator / r.Denominator);
        }

        #endregion

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (Int32)this;
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Numerator + "/" + Denominator;
        }

        #endregion

        #region ILispNumber Members

        public ILispNumber Add(ILispNumber other)
        {
            throw new NotImplementedException();
        }

        public ILispNumber Mul(ILispNumber other)
        {
            throw new NotImplementedException();
        }

        public ILispNumber Div(ILispNumber other)
        {
            throw new NotImplementedException();
        }

        public ILispNumber Sub(ILispNumber other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ILispNumber Members


        public NumberType NumberType
        {
            get { return NumberType.Ratio; }
        }

        #endregion

        #region ILispNumber Members


        public byte Byte
        {
            get { throw new NotImplementedException(); }
        }

        public int Int32
        {
            get { throw new NotImplementedException(); }
        }

        public BigInteger BigInteger
        {
            get { throw new NotImplementedException(); }
        }

        public long Int64
        {
            get { throw new NotImplementedException(); }
        }

        Ratio ILispNumber.Ratio
        {
            get { throw new NotImplementedException(); }
        }

        public float Single
        {
            get { throw new NotImplementedException(); }
        }

        public double Double
        {
            get { throw new NotImplementedException(); }
        }

        public Complex Complex
        {
            get { throw new NotImplementedException(); }
        }

        public decimal Decimal
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ILispObject Members

        public LiveLisp.Core.CLOS.LispObjectTypeCode LispTypeCode
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ILispNumber Members


        public bool Zerop
        {
            get { return Numerator == 0; }
        }

        #endregion

        internal object Sin()
        {
            throw new NotImplementedException();
        }

        internal object Cos()
        {
            throw new NotImplementedException();
        }

        internal object Tan()
        {
            throw new NotImplementedException();
        }

        internal object Atan()
        {
            throw new NotImplementedException();
        }

        internal object Asin()
        {
            throw new NotImplementedException();
        }

        internal object Acos()
        {
            throw new NotImplementedException();
        }

        internal object Sinh()
        {
            throw new NotImplementedException();
        }

        internal object Cosh()
        {
            throw new NotImplementedException();
        }

        internal object Tanh()
        {
            throw new NotImplementedException();
        }

        internal object Abs()
        {
            throw new NotImplementedException();
        }
    }
}
