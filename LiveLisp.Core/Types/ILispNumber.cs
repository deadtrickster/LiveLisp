using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.CLOS;

namespace LiveLisp.Core.Types
{
    public enum NumberType
    {
        Byte,
        Int,
        BigInt,
        Long,
        Ratio,
        Single,
        Double,
        Complex,
        Decimal
    }

    public interface ILispNumber : IConvertible, IComparable, IFormattable, ILispObject
    {
        ILispNumber Add(ILispNumber other);
        ILispNumber Mul(ILispNumber other);
        ILispNumber Div(ILispNumber other);
        ILispNumber Sub(ILispNumber other);

        bool Zerop
        {
            get;
        }

        NumberType NumberType
        {
            get;
        }

        Byte Byte
        {
            get;
        }

        Int32 Int32
        {
            get;
        }

        BigInteger BigInteger
        {
            get;
        }

        Int64 Int64
        {
            get;
        }

        Ratio Ratio
        {
            get;
        }

        Single Single
        {
            get;
        }

        Double Double
        {
            get;
        }

        Complex Complex
        {
            get;
        }

        Decimal Decimal
        {
            get;
        }
    }
}
