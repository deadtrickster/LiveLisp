/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using System.Diagnostics;
using System.Text;
using System.Collections;

namespace LiveLisp.Core.Types
{
    /// <summary>
    /// Implementation of the complex number data type.
    /// </summary>
    //[CLSCompliant(false)]
    public struct Complex : ILispNumber
    {

        

        private readonly double real, imag;

        public static Complex MakeImaginary(double imag)
        {
            return new Complex(0.0, imag);
        }

        public static Complex MakeReal(double real)
        {
            return new Complex(real, 0.0);
        }

        public static Complex Make(double real, double imag)
        {
            return new Complex(real, imag);
        }

        public Complex(double real)
            : this(real, 0.0)
        {
        }

        public Complex(double real, double imag)
        {
            this.real = real;
            this.imag = imag;
        }

        public bool IsZero
        {
            get
            {
                return real == 0.0 && imag == 0.0;
            }
        }

        public double Real
        {
            get
            {
                return real;
            }
        }

        public double Imag
        {
            get
            {
                return imag;
            }
        }

        public Complex Conjugate()
        {
            return new Complex(real, -imag);
        }


        public override string ToString()
        {
            if (real == 0.0) return imag.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + "j";
            else if (imag < 0.0) return string.Format(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, "({0}{1}j)", real, imag);
            else return string.Format(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, "({0}+{1}j)", real, imag);
        }

        public static implicit operator Complex(int i)
        {
            return MakeReal(i);
        }

        public static implicit operator Complex(uint i)
        {
            return MakeReal(i);
        }

        public static implicit operator Complex(short i)
        {
            return MakeReal(i);
        }

        public static implicit operator Complex(ushort i)
        {
            return MakeReal(i);
        }

        public static implicit operator Complex(long l)
        {
            return MakeReal(l);
        }

        public static implicit operator Complex(ulong i)
        {
            return MakeReal(i);
        }

        public static implicit operator Complex(sbyte i)
        {
            return MakeReal(i);
        }

        public static implicit operator Complex(byte i)
        {
            return MakeReal(i);
        }

        public static implicit operator Complex(float f)
        {
            return MakeReal(f);
        }

        public static implicit operator Complex(double d)
        {
            return MakeReal(d);
        }

        /*  public static implicit operator Complex64(BigInteger i) {
              if (object.ReferenceEquals(i, null)) {
                  throw new ArgumentException(MathResources.InvalidArgument, "i");
              }

              // throws an overflow exception if we can't handle the value.
              return MakeReal(i.ToFloat64());
          }*/

        public static bool operator ==(Complex x, Complex y)
        {
            return x.real == y.real && x.imag == y.imag;
        }

        public static bool operator !=(Complex x, Complex y)
        {
            return x.real != y.real || x.imag != y.imag;
        }

        public static Complex Add(Complex x, Complex y)
        {
            return x + y;
        }

        public static Complex operator +(Complex x, Complex y)
        {
            return new Complex(x.real + y.real, x.imag + y.imag);
        }

        public static Complex Subtract(Complex x, Complex y)
        {
            return x - y;
        }

        public static Complex operator -(Complex x, Complex y)
        {
            return new Complex(x.real - y.real, x.imag - y.imag);
        }

        public static Complex Multiply(Complex x, Complex y)
        {
            return x * y;
        }

        public static Complex operator *(Complex x, Complex y)
        {
            return new Complex(x.real * y.real - x.imag * y.imag, x.real * y.imag + x.imag * y.real);
        }

        public static Complex Divide(Complex x, Complex y)
        {
            return x / y;
        }

        public static Complex operator /(Complex a, Complex b)
        {
            if (b.IsZero) throw new DivideByZeroException();

            double real, imag, den, r;

            if (System.Math.Abs(b.real) >= System.Math.Abs(b.imag))
            {
                r = b.imag / b.real;
                den = b.real + r * b.imag;
                real = (a.real + a.imag * r) / den;
                imag = (a.imag - a.real * r) / den;
            }
            else
            {
                r = b.real / b.imag;
                den = b.imag + r * b.real;
                real = (a.real * r + a.imag) / den;
                imag = (a.imag * r - a.real) / den;
            }

            return new Complex(real, imag);
        }

        public static Complex Mod(Complex x, Complex y)
        {
            return x % y;
        }

        public static Complex operator %(Complex x, Complex y)
        {
            if (object.ReferenceEquals(x, null))
            {
                throw new ArgumentException();
            }
            if (object.ReferenceEquals(y, null))
            {
                throw new ArgumentException();
            }

            if (y == 0) throw new DivideByZeroException();

            throw new NotImplementedException();
        }

        public static Complex Negate(Complex x)
        {
            return -x;
        }

        public static Complex operator -(Complex x)
        {
            return new Complex(-x.real, -x.imag);
        }

        public static Complex Plus(Complex x)
        {
            return +x;
        }

        public static Complex operator +(Complex x)
        {
            return x;
        }

        public static double Hypot(double x, double y)
        {
            //
            // sqrt(x*x + y*y) == sqrt(x*x * (1 + (y*y)/(x*x))) ==
            // sqrt(x*x) * sqrt(1 + (y/x)*(y/x)) ==
            // abs(x) * sqrt(1 + (y/x)*(y/x))
            //

            //  First, get abs
            if (x < 0.0) x = -x;
            if (y < 0.0) y = -y;

            // Obvious cases
            if (x == 0.0) return y;
            if (y == 0.0) return x;

            // Divide smaller number by bigger number to safeguard the (y/x)*(y/x)
            if (x < y) { double temp = y; y = x; x = temp; }

            y /= x;

            // calculate abs(x) * sqrt(1 + (y/x)*(y/x))
            return x * System.Math.Sqrt(1 + y * y);
        }

        public double Abs()
        {
            return Hypot(real, imag);
        }

        public Complex Power(Complex y)
        {
            double c = y.real;
            double d = y.imag;
            int power = (int)c;

            if (power == c && power >= 0 && d == .0)
            {
                Complex result = new Complex(1.0);
                if (power == 0) return result;
                Complex factor = this;
                while (power != 0)
                {
                    if ((power & 1) != 0)
                    {
                        result = result * factor;
                    }
                    factor = factor * factor;
                    power >>= 1;
                }
                return result;
            }
            else if (IsZero)
            {
                return y.IsZero ? Complex.MakeReal(1.0) : Complex.MakeReal(0.0);
            }
            else
            {
                double a = real;
                double b = imag;
                double powers = a * a + b * b;
                double arg = System.Math.Atan2(b, a);
                double mul = System.Math.Pow(powers, c / 2) * System.Math.Exp(-d * arg);
                double common = c * arg + .5 * d * System.Math.Log(powers);
                return new Complex(mul * System.Math.Cos(common), mul * System.Math.Sin(common));
            }
        }

        public override int GetHashCode()
        {
            return (int)real + (int)imag * 1000003;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Complex)) return false;
            return this == ((Complex)obj);
        }

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
            if (this.Imag == 0)
            {
                return (int)this.Real;
            }
            else
            {
                throw new InvalidCastException("Cannot convert complex with non zero Imag to int");
            }
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

        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
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
            get { return NumberType.Complex; }
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

        public Ratio Ratio
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

        Complex ILispNumber.Complex
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
            get { return real == 0.0 && imag == 0.0; }
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
    }
}