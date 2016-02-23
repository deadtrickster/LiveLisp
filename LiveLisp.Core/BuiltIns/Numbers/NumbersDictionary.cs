using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Types;
using LiveLisp.Core.Compiler;
using System.Threading;
using LiveLisp.Core.Runtime.OperatorsCache;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.BuiltIns.Numbers
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class NumbersDictionary
    {
        static NumbersDictionary()
        {
            
        }

        [Builtin("=", Predicate = true)]
        public static object NumEq(params object[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }
            if (numbers.Length == 1)
                return numbers[0];

            if (numbers.Length == 2)
            {
                return EqaulityOp.GetAndInvokeTarget(numbers[0], numbers[1]);
            }

            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers.Count(el => el.Equals(numbers[i])) == 1)
                    return DefinedSymbols.NIL;
            }

            return DefinedSymbols.T;
        }

        [Builtin("/=", Predicate = true)]
        public static object NumNEq(params object[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }
            if (numbers.Length == 1)
                return numbers[0];

            if (numbers.Length == 2)
            {
                return InequalityOp.GetAndInvokeTarget(numbers[0], numbers[1]);
            }

            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers.Count(el => el.Equals(numbers[i])) > 1)
                    return DefinedSymbols.NIL;
            }

            return DefinedSymbols.T;
        }

        [Builtin("<", Predicate = true)]
        public static object NumLess(params object[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }
            if (numbers.Length == 1)
                return numbers[0];

            object current = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                object ret = LessThenOperator.GetAndInvokeTarget(current, numbers[i]);
                if (ret == DefinedSymbols.NIL)
                    return ret;
                current = numbers[i];
            }

            return DefinedSymbols.T;
        }

        [Builtin(">", Predicate = true)]
        public static object NumMore(params object[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }
            if (numbers.Length == 1)
                return numbers[0];

            object current = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                object ret = GreaterThenOperator.GetAndInvokeTarget(current, numbers[i]);
                if (ret == DefinedSymbols.NIL)
                    return ret;
                current = numbers[i];
            }

            return DefinedSymbols.T;
        }

        [Builtin("<=", Predicate = true)]
        public static object NumLE(params object[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }
            if (numbers.Length == 1)
                return numbers[0];

            object current = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                object ret = LessOrEqualOp.GetAndInvokeTarget(current, numbers[i]);
                if (ret == DefinedSymbols.NIL)
                    return ret;
                current = numbers[i];
            }

            return DefinedSymbols.T;
        }

        [Builtin(">=", Predicate = true)]
        public static object NumME(params object[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }
            if (numbers.Length == 1)
                return numbers[0];

            object current = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                object ret = GreaterOrEqualOp.GetAndInvokeTarget(current, numbers[i]);
                if (ret == DefinedSymbols.NIL)
                    return ret;
                current = numbers[i];
            }

            return DefinedSymbols.T;
        }

        [Builtin(MinArgs=1)]
        public static object Max(params dynamic[] reals)
        {
            if (reals.Length < 1)
            {
                ConditionsDictionary.Error("MAX: too few args");
            }

            dynamic ret = reals[0];
            
            for (int i = 1; i < reals.Length; i++)
			{
                if(ret < reals[i])
                {
                    ret = reals[i];
                }
			}

            return ret;
        }

        [Builtin]
        public static object Min(params dynamic[] reals)
        {
            if (reals.Length < 1)
            {
                ConditionsDictionary.Error("MIN: too few args");
            }

            dynamic ret = reals[0];
            
            for (int i = 1; i < reals.Length; i++)
			{
                if(ret > reals[i])
                {
                    ret = reals[i];
                }
			}

            return ret;
        }

        [Builtin(Predicate=true)]
        public static object Minusp(object real)
        {
            TypeCode code = Type.GetTypeCode(real.GetType());

            switch (code)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                    return DefinedSymbols.NIL;
                case TypeCode.Decimal:
                    return (Decimal)real < 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Double:
                    return (Double)real < 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Int16:
                    return (Int16)real < 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Int32:
                    return (Int32)real < 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Int64:
                    return (Int64)real < 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.SByte:
                    return (sbyte)real < 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Single:
                    return (Single)real < 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return DefinedSymbols.NIL;
                default:
                    if (real is Ratio)
                        return (Ratio)real < (double)0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                    if (real is BigInteger)
                        return (BigInteger)real < 0 ? DefinedSymbols.T : DefinedSymbols.NIL;

                    Conditions.ConditionsDictionary.Error("MINUSP: " + real + " is not a real number");
                    break;
            }

            throw new Exception("never reached");
        }

        [Builtin(Predicate = true)]
        public static object Plusp(object real)
        {
            TypeCode code = Type.GetTypeCode(real.GetType());

            switch (code)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                    return DefinedSymbols.T;
                case TypeCode.Decimal:
                    return (Decimal)real > 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Double:
                    return (Double)real > 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Int16:
                    return (Int16)real > 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Int32:
                    return (Int32)real > 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Int64:
                    return (Int64)real > 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.SByte:
                    return (sbyte)real > 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Single:
                    return (Single)real > 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return DefinedSymbols.T;
                default:
                    if (real is Ratio)
                        return (Ratio)real > (double)0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                    if (real is BigInteger)
                        return (BigInteger)real > 0 ? DefinedSymbols.T : DefinedSymbols.NIL;

                    Conditions.ConditionsDictionary.Error("MINUSP: " + real + " is not a real number");
                    break;
            }

            throw new Exception("never reached");
        }

        [Builtin(Predicate = true)]
        public static object Zerop(object number)
        {
            TypeCode code = Type.GetTypeCode(number.GetType());

            switch (code)
            {
                case TypeCode.Boolean:
                    return (Boolean)number ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.Byte:
                    return (byte)number == 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Char:
                    return (char)number == 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Decimal:
                    return (Decimal)number == (Decimal)0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Double:
                    return (Double)number == (Double)0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Int16:
                    return (Int16)number == 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Int32:
                    return (Int32)number == 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Int64:
                    return (Int64)number == 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.SByte:
                    return (sbyte)number == 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Single:
                    return (Single)number == (Single)0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.UInt16:
                    return (UInt16)number == 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.UInt32:
                    return (UInt32)number == 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.UInt64:
                    return (UInt64)number == 0 ? DefinedSymbols.T : DefinedSymbols.NIL;
                default:
                    if (number is Complex)
                        return ((Complex)number).Zerop ? DefinedSymbols.T : DefinedSymbols.NIL;
                    if (number is Ratio)
                        return ((Ratio)number).Zerop? DefinedSymbols.T : DefinedSymbols.NIL;
                    if (number is BigInteger)
                        return ((BigInteger)number).Zerop? DefinedSymbols.T : DefinedSymbols.NIL;

                    Conditions.ConditionsDictionary.Error("ZEROP: " + number + " is not a real number");
                    break;
            }

            throw new Exception("never reached");
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer Floor(object number, [Optional] object divisor)
        {
            throw new NotImplementedException();
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer FFloor(object number, [Optional] object divisor)
        {
            throw new NotImplementedException();
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer Ceiling(object number, [Optional] object divisor)
        {
            throw new NotImplementedException();
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer FCeiling(object number, [Optional] object divisor)
        {
            throw new NotImplementedException();
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer Truncate(object number, [Optional] object divisor)
        {
            throw new NotImplementedException();
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer FTruncate(object number, [Optional] object divisor)
        {
            throw new NotImplementedException();
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer Round(object number, [Optional] object divisor)
        {
            throw new NotImplementedException();
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer FRound(object number, [Optional] object divisor)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Sin(dynamic radians)
        {
            /*TypeCode code = Type.GetTypeCode(radians.GetType());
            double? sin = null;
            switch (code)
            {
                case TypeCode.Boolean:
                    sin = Math.Sin((Boolean)radians ? 1 : 0);
                    break;
                case TypeCode.Byte:
                    sin = Math.Sin((Byte)radians);
                    break;
                case TypeCode.Char:
                    sin = Math.Sin((Char)radians);
                    break;
                case TypeCode.Decimal:
                    sin = Math.Sin(Convert.ToDouble((Decimal)radians));
                    break;
                case TypeCode.Double:
                    sin = Math.Sin((Double)radians);
                    break;
                case TypeCode.Int16:
                    sin = Math.Sin((Int16)radians);
                    break;
                case TypeCode.Int32:
                    sin = Math.Sin((Int32)radians);
                    break;
                case TypeCode.Int64:
                    sin = Math.Sin((Int64)radians);
                    break;
                case TypeCode.SByte:
                    sin = Math.Sin((SByte)radians);
                    break;
                case TypeCode.Single:
                    sin = Math.Sin((Single)radians);
                    break;
                case TypeCode.UInt16:
                    sin = Math.Sin((UInt16)radians);
                    break;
                case TypeCode.UInt32:
                    sin = Math.Sin((Byte)radians);
                    break;
                case TypeCode.UInt64:
                    sin = Math.Sin((UInt64)radians);
                    break;
            }

            if (sin.HasValue)
            {
                if (sin.Value == Math.Floor(sin.Value))
                {
                    if (Int32.MinValue <= sin && sin <= Int32.MaxValue)
                        return (int)sin;
                    else if (Int64.MinValue <= sin && sin <= Int64.MaxValue)
                        return (Int64)sin;
                }
                else return sin.Value;
            }

            if (radians is Complex)
                return ((Complex)radians).Sin();
            if (radians is Ratio)
                return ((Ratio)radians).Sin();
            if (radians is BigInteger)
                Conditions.ConditionsDictionary.Error("SIN: " + radians + " is a big integer");

            Conditions.ConditionsDictionary.Error("SIN: " + radians + " is not a number");


            throw new Exception("Never reached");*/
            return Math.Sin(radians);
        }

        [Builtin]
        public static object Cos(dynamic radians)
        {
            /*
            TypeCode code = Type.GetTypeCode(radians.GetType());
            double? Cos = null;
            switch (code)
            {
                case TypeCode.Boolean:
                    Cos = Math.Cos((Boolean)radians ? 1 : 0);
                    break;
                case TypeCode.Byte:
                    Cos = Math.Cos((Byte)radians);
                    break;
                case TypeCode.Char:
                    Cos = Math.Cos((Char)radians);
                    break;
                case TypeCode.Decimal:
                    Cos = Math.Cos(Convert.ToDouble((Decimal)radians));
                    break;
                case TypeCode.Double:
                    Cos = Math.Cos((Double)radians);
                    break;
                case TypeCode.Int16:
                    Cos = Math.Cos((Int16)radians);
                    break;
                case TypeCode.Int32:
                    Cos = Math.Cos((Int32)radians);
                    break;
                case TypeCode.Int64:
                    Cos = Math.Cos((Int64)radians);
                    break;
                case TypeCode.SByte:
                    Cos = Math.Cos((SByte)radians);
                    break;
                case TypeCode.Single:
                    Cos = Math.Cos((Single)radians);
                    break;
                case TypeCode.UInt16:
                    Cos = Math.Cos((UInt16)radians);
                    break;
                case TypeCode.UInt32:
                    Cos = Math.Cos((UInt32)radians);
                    break;
                case TypeCode.UInt64:
                    Cos = Math.Cos((UInt64)radians);
                    break;
            }

            if (Cos.HasValue)
            {
                if (Cos.Value == Math.Floor(Cos.Value))
                {
                    if (Int32.MinValue <= Cos && Cos <= Int32.MaxValue)
                        return (int)Cos;
                    else if (Int64.MinValue <= Cos && Cos <= Int64.MaxValue)
                        return (Int64)Cos;
                }
                else return Cos.Value;
            }

            if (radians is Complex)
                return ((Complex)radians).Cos();
            if (radians is Ratio)
                return ((Ratio)radians).Cos();
            if (radians is BigInteger)
                Conditions.ConditionsDictionary.Error("COS: " + radians + " is a big integer");

            Conditions.ConditionsDictionary.Error("COS: " + radians + " is not a number");


            throw new Exception("Never reached");*/

            return Math.Cos(radians);
        }

        [Builtin]
        public static object Tan(dynamic radians)
        {
            /*
            TypeCode code = Type.GetTypeCode(radians.GetType());
            double? Tan = null;
            switch (code)
            {
                case TypeCode.Boolean:
                    Tan = Math.Tan((Boolean)radians ? 1 : 0);
                    break;
                case TypeCode.Byte:
                    Tan = Math.Tan((Byte)radians);
                    break;
                case TypeCode.Char:
                    Tan = Math.Tan((Char)radians);
                    break;
                case TypeCode.Decimal:
                    Tan = Math.Tan(Convert.ToDouble((Decimal)radians));
                    break;
                case TypeCode.Double:
                    Tan = Math.Tan((Double)radians);
                    break;
                case TypeCode.Int16:
                    Tan = Math.Tan((Int16)radians);
                    break;
                case TypeCode.Int32:
                    Tan = Math.Tan((Int32)radians);
                    break;
                case TypeCode.Int64:
                    Tan = Math.Tan((Int64)radians);
                    break;
                case TypeCode.SByte:
                    Tan = Math.Tan((SByte)radians);
                    break;
                case TypeCode.Single:
                    Tan = Math.Tan((Single)radians);
                    break;
                case TypeCode.UInt16:
                    Tan = Math.Tan((UInt16)radians);
                    break;
                case TypeCode.UInt32:
                    Tan = Math.Tan((UInt32)radians);
                    break;
                case TypeCode.UInt64:
                    Tan = Math.Tan((UInt64)radians);
                    break;
            }

            if (Tan.HasValue)
            {
                if (Tan.Value == Math.Floor(Tan.Value))
                {
                    if (Int32.MinValue <= Tan && Tan <= Int32.MaxValue)
                        return (int)Tan;
                    else if (Int64.MinValue <= Tan && Tan <= Int64.MaxValue)
                        return (Int64)Tan;
                    else return Tan.Value;
                }
            }
            
            if (radians is Complex)
                return ((Complex)radians).Tan();
            if (radians is Ratio)
                return ((Ratio)radians).Tan();
            if (radians is BigInteger)
                Conditions.ConditionsDictionary.Error("TAN: " + radians + " is a big integer");

            Conditions.ConditionsDictionary.Error("TAN: " + radians + " is not a number");


            throw new Exception("Never reached");*/

            return Math.Tan(radians);
        }

        [Builtin]
        public static object ASin(object radians)
        {
            TypeCode code = Type.GetTypeCode(radians.GetType());
            double? Asin = null;
            switch (code)
            {
                case TypeCode.Boolean:
                    Asin = Math.Asin((Boolean)radians ? 1 : 0);
                    break;
                case TypeCode.Byte:
                    Asin = Math.Asin((Byte)radians);
                    break;
                case TypeCode.Char:
                    Asin = Math.Asin((Char)radians);
                    break;
                case TypeCode.Decimal:
                    Asin = Math.Asin(Convert.ToDouble((Decimal)radians));
                    break;
                case TypeCode.Double:
                    Asin = Math.Asin((Double)radians);
                    break;
                case TypeCode.Int16:
                    Asin = Math.Asin((Int16)radians);
                    break;
                case TypeCode.Int32:
                    Asin = Math.Asin((Int32)radians);
                    break;
                case TypeCode.Int64:
                    Asin = Math.Asin((Int64)radians);
                    break;
                case TypeCode.SByte:
                    Asin = Math.Asin((SByte)radians);
                    break;
                case TypeCode.Single:
                    Asin = Math.Asin((Single)radians);
                    break;
                case TypeCode.UInt16:
                    Asin = Math.Asin((UInt16)radians);
                    break;
                case TypeCode.UInt32:
                    Asin = Math.Asin((UInt32)radians);
                    break;
                case TypeCode.UInt64:
                    Asin = Math.Asin((UInt64)radians);
                    break;
            }

            if (Asin.HasValue)
            {
                if (Asin.Value == Math.Floor(Asin.Value))
                {
                    if (Int32.MinValue <= Asin && Asin <= Int32.MaxValue)
                        return (int)Asin;
                    else if (Int64.MinValue <= Asin && Asin <= Int64.MaxValue)
                        return (Int64)Asin;
                    else return Asin.Value;
                }
            }

            if (radians is Complex)
                return ((Complex)radians).Asin();
            if (radians is Ratio)
                return ((Ratio)radians).Asin();
            if (radians is BigInteger)
                Conditions.ConditionsDictionary.Error("ASIN: " + radians + " is a big integer");

            Conditions.ConditionsDictionary.Error("ASIN: " + radians + " is not a number");


            throw new Exception("Never reached");
        }

        [Builtin]
        public static object ACos(object radians)
        {
            TypeCode code = Type.GetTypeCode(radians.GetType());
            double? Acos = null;
            switch (code)
            {
                case TypeCode.Boolean:
                    Acos = Math.Acos((Boolean)radians ? 1 : 0);
                    break;
                case TypeCode.Byte:
                    Acos = Math.Acos((Byte)radians);
                    break;
                case TypeCode.Char:
                    Acos = Math.Acos((Char)radians);
                    break;
                case TypeCode.Decimal:
                    Acos = Math.Acos(Convert.ToDouble((Decimal)radians));
                    break;
                case TypeCode.Double:
                    Acos = Math.Acos((Double)radians);
                    break;
                case TypeCode.Int16:
                    Acos = Math.Acos((Int16)radians);
                    break;
                case TypeCode.Int32:
                    Acos = Math.Acos((Int32)radians);
                    break;
                case TypeCode.Int64:
                    Acos = Math.Acos((Int64)radians);
                    break;
                case TypeCode.SByte:
                    Acos = Math.Acos((SByte)radians);
                    break;
                case TypeCode.Single:
                    Acos = Math.Acos((Single)radians);
                    break;
                case TypeCode.UInt16:
                    Acos = Math.Acos((UInt16)radians);
                    break;
                case TypeCode.UInt32:
                    Acos = Math.Acos((UInt32)radians);
                    break;
                case TypeCode.UInt64:
                    Acos = Math.Acos((UInt64)radians);
                    break;
            }

            if (Acos.HasValue)
            {
                if (Acos.Value == Math.Floor(Acos.Value))
                {
                    if (Int32.MinValue <= Acos && Acos <= Int32.MaxValue)
                        return (int)Acos;
                    else if (Int64.MinValue <= Acos && Acos <= Int64.MaxValue)
                        return (Int64)Acos;
                    else return Acos.Value;
                }
            }

            if (radians is Complex)
                return ((Complex)radians).Acos();
            if (radians is Ratio)
                return ((Ratio)radians).Acos();
            if (radians is BigInteger)
                Conditions.ConditionsDictionary.Error("Acos: " + radians + " is a big integer");

            Conditions.ConditionsDictionary.Error("Acos: " + radians + " is not a number");


            throw new Exception("Never reached");
        }

        [Builtin]
        // TODO: implement as class manually
        public static object ATan(object radians, [Optional] object number2)
        {
            /*TypeCode code = Type.GetTypeCode(radians.GetType());
            double? Atan = null;
            switch (code)
            {
                case TypeCode.Boolean:
                    Atan = Math.Atan((Boolean)radians ? 1 : 0);
                    break;
                case TypeCode.Byte:
                    Atan = Math.Atan((Byte)radians);
                    break;
                case TypeCode.Char:
                    Atan = Math.Atan((Char)radians);
                    break;
                case TypeCode.Decimal:
                    Atan = Math.Atan(Convert.ToDouble((Decimal)radians));
                    break;
                case TypeCode.Double:
                    Atan = Math.Atan((Double)radians);
                    break;
                case TypeCode.Int16:
                    Atan = Math.Atan((Int16)radians);
                    break;
                case TypeCode.Int32:
                    Atan = Math.Atan((Int32)radians);
                    break;
                case TypeCode.Int64:
                    Atan = Math.Atan((Int64)radians);
                    break;
                case TypeCode.SByte:
                    Atan = Math.Atan((SByte)radians);
                    break;
                case TypeCode.Single:
                    Atan = Math.Atan((Single)radians);
                    break;
                case TypeCode.UInt16:
                    Atan = Math.Atan((UInt16)radians);
                    break;
                case TypeCode.UInt32:
                    Atan = Math.Atan((UInt32)radians);
                    break;
                case TypeCode.UInt64:
                    Atan = Math.Atan((UInt64)radians);
                    break;
            }

            if (Atan.HasValue)
            {
                if (Atan.Value == Math.Floor(Atan.Value))
                {
                    if (Int32.MinValue <= Atan && Atan <= Int32.MaxValue)
                        return (int)Atan;
                    else if (Int64.MinValue <= Atan && Atan <= Int64.MaxValue)
                        return (Int64)Atan;
                    else return Atan.Value;
                }
            }

            if (radians is Complex)
                
            if(number2 == DefinedSymbols.NIL)
                return ((Complex)radians).Atan();
            else
                Conditions.ConditionsDictionary.Error("ATAN: when number2 has supplied number1 cannot be complex");
            if (radians is Ratio)
                return ((Ratio)radians).Atan();
            if (radians is BigInteger)
                Conditions.ConditionsDictionary.Error("ATAN: " + radians + " is a big integer");

            Conditions.ConditionsDictionary.Error("ATAN: " + radians + " is not a number");


            throw new Exception("Never reached");*/

            throw new NotImplementedException();
        }

        [Builtin]
        public static object Sinh(object radians)
        {
            TypeCode code = Type.GetTypeCode(radians.GetType());
            double? Sinh = null;
            switch (code)
            {
                case TypeCode.Boolean:
                    Sinh = Math.Sinh((Boolean)radians ? 1 : 0);
                    break;
                case TypeCode.Byte:
                    Sinh = Math.Sinh((Byte)radians);
                    break;
                case TypeCode.Char:
                    Sinh = Math.Sinh((Char)radians);
                    break;
                case TypeCode.Decimal:
                    Sinh = Math.Sinh(Convert.ToDouble((Decimal)radians));
                    break;
                case TypeCode.Double:
                    Sinh = Math.Sinh((Double)radians);
                    break;
                case TypeCode.Int16:
                    Sinh = Math.Sinh((Int16)radians);
                    break;
                case TypeCode.Int32:
                    Sinh = Math.Sinh((Int32)radians);
                    break;
                case TypeCode.Int64:
                    Sinh = Math.Sinh((Int64)radians);
                    break;
                case TypeCode.SByte:
                    Sinh = Math.Sinh((SByte)radians);
                    break;
                case TypeCode.Single:
                    Sinh = Math.Sinh((Single)radians);
                    break;
                case TypeCode.UInt16:
                    Sinh = Math.Sinh((UInt16)radians);
                    break;
                case TypeCode.UInt32:
                    Sinh = Math.Sinh((UInt32)radians);
                    break;
                case TypeCode.UInt64:
                    Sinh = Math.Sinh((UInt64)radians);
                    break;
            }

            if (Sinh.HasValue)
            {
                if (Sinh.Value == Math.Floor(Sinh.Value))
                {
                    if (Int32.MinValue <= Sinh && Sinh <= Int32.MaxValue)
                        return (int)Sinh;
                    else if (Int64.MinValue <= Sinh && Sinh <= Int64.MaxValue)
                        return (Int64)Sinh;
                    else return Sinh.Value;
                }
            }

            if (radians is Complex)
                return ((Complex)radians).Sinh();
            if (radians is Ratio)
                return ((Ratio)radians).Sinh();
            if (radians is BigInteger)
                Conditions.ConditionsDictionary.Error("SINH: " + radians + " is a big integer");

            Conditions.ConditionsDictionary.Error("SINH: " + radians + " is not a number");


            throw new Exception("Never reached");
        }

        [Builtin]
        public static object Cosh(object radians)
        {
            TypeCode code = Type.GetTypeCode(radians.GetType());
            double? Cosh = null;
            switch (code)
            {
                case TypeCode.Boolean:
                    Cosh = Math.Cosh((Boolean)radians ? 1 : 0);
                    break;
                case TypeCode.Byte:
                    Cosh = Math.Cosh((Byte)radians);
                    break;
                case TypeCode.Char:
                    Cosh = Math.Cosh((Char)radians);
                    break;
                case TypeCode.Decimal:
                    Cosh = Math.Cosh(Convert.ToDouble((Decimal)radians));
                    break;
                case TypeCode.Double:
                    Cosh = Math.Cosh((Double)radians);
                    break;
                case TypeCode.Int16:
                    Cosh = Math.Cosh((Int16)radians);
                    break;
                case TypeCode.Int32:
                    Cosh = Math.Cosh((Int32)radians);
                    break;
                case TypeCode.Int64:
                    Cosh = Math.Cosh((Int64)radians);
                    break;
                case TypeCode.SByte:
                    Cosh = Math.Cosh((SByte)radians);
                    break;
                case TypeCode.Single:
                    Cosh = Math.Cosh((Single)radians);
                    break;
                case TypeCode.UInt16:
                    Cosh = Math.Cosh((UInt16)radians);
                    break;
                case TypeCode.UInt32:
                    Cosh = Math.Cosh((UInt32)radians);
                    break;
                case TypeCode.UInt64:
                    Cosh = Math.Cosh((UInt64)radians);
                    break;
            }

            if (Cosh.HasValue)
            {
                if (Cosh.Value == Math.Floor(Cosh.Value))
                {
                    if (Int32.MinValue <= Cosh && Cosh <= Int32.MaxValue)
                        return (int)Cosh;
                    else if (Int64.MinValue <= Cosh && Cosh <= Int64.MaxValue)
                        return (Int64)Cosh;
                    else return Cosh.Value;
                }
            }

            if (radians is Complex)
                return ((Complex)radians).Cosh();
            if (radians is Ratio)
                return ((Ratio)radians).Cosh();
            if (radians is BigInteger)
                Conditions.ConditionsDictionary.Error("COSH: " + radians + " is a big integer");

            Conditions.ConditionsDictionary.Error("COSH: " + radians + " is not a number");


            throw new Exception("Never reached");
        }

        [Builtin]
        public static object Tanh(object radians)
        {
            TypeCode code = Type.GetTypeCode(radians.GetType());
            double? Tanh = null;
            switch (code)
            {
                case TypeCode.Boolean:
                    Tanh = Math.Tanh((Boolean)radians ? 1 : 0);
                    break;
                case TypeCode.Byte:
                    Tanh = Math.Tanh((Byte)radians);
                    break;
                case TypeCode.Char:
                    Tanh = Math.Tanh((Char)radians);
                    break;
                case TypeCode.Decimal:
                    Tanh = Math.Tanh(Convert.ToDouble((Decimal)radians));
                    break;
                case TypeCode.Double:
                    Tanh = Math.Tanh((Double)radians);
                    break;
                case TypeCode.Int16:
                    Tanh = Math.Tanh((Int16)radians);
                    break;
                case TypeCode.Int32:
                    Tanh = Math.Tanh((Int32)radians);
                    break;
                case TypeCode.Int64:
                    Tanh = Math.Tanh((Int64)radians);
                    break;
                case TypeCode.SByte:
                    Tanh = Math.Tanh((SByte)radians);
                    break;
                case TypeCode.Single:
                    Tanh = Math.Tanh((Single)radians);
                    break;
                case TypeCode.UInt16:
                    Tanh = Math.Tanh((UInt16)radians);
                    break;
                case TypeCode.UInt32:
                    Tanh = Math.Tanh((UInt32)radians);
                    break;
                case TypeCode.UInt64:
                    Tanh = Math.Tanh((UInt64)radians);
                    break;
            }

            if (Tanh.HasValue)
            {
                if (Tanh.Value == Math.Floor(Tanh.Value))
                {
                    if (Int32.MinValue <= Tanh && Tanh <= Int32.MaxValue)
                        return (int)Tanh;
                    else if (Int64.MinValue <= Tanh && Tanh <= Int64.MaxValue)
                        return (Int64)Tanh;
                    else return Tanh.Value;
                }
            }

            if (radians is Complex)
                return ((Complex)radians).Tanh();
            if (radians is Ratio)
                return ((Ratio)radians).Tanh();
            if (radians is BigInteger)
                Conditions.ConditionsDictionary.Error("TANH: " + radians + " is a big integer");

            Conditions.ConditionsDictionary.Error("TANH: " + radians + " is not a number");


            throw new Exception("Never reached");
        }

        [Builtin]
        public static object ASinh(dynamic radians)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object ACosh(dynamic radians)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object ATanh(dynamic radians)
        {
            throw new NotImplementedException();
        }

        [Builtin("*")]
        public static object Mult(params object[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }
            if (numbers.Length == 1)
                return numbers[0];

            object current = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                current = MulOperatorCache.GetAndInvokeTarget(current, numbers[i]);
            }

            return current;
        }

        [Builtin("+")]
        public static object Add(params object[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }
            if (numbers.Length == 1)
                return numbers[0];
           
            object current = numbers[0];

            try
            {

                for (int i = 1; i < numbers.Length; i++)
                {
                    current = AddOperatorCache.GetAndInvokeTarget(current, numbers[i]);
                }
            }
            catch (OperatorNotFoundException)
            {
                StringBuilder sb = new StringBuilder();
                bool string_found = false;
                for (int i = 0; i < numbers.Length; i++)
                {
                    sb.Append(numbers[i].ToString());
                    if (!string_found)
                    {
                        string_found = numbers[i] is string;
                    }
                }

                if (string_found)
                    return sb.ToString();

                else

                    throw;
            }

            return current;
        }
    

        [Builtin("-")]
        public static object Sub(params object[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }
            if (numbers.Length == 1)
                return numbers[0];

            object current = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                current = SubOperatorCache.GetAndInvokeTarget(current, numbers[i]);
            }

            return current;
        }

        [Builtin("/")]
        public static object Div(params object[] numbers)
        {
            if (numbers.Length == 0)
            {
                return 0;
            }
            if (numbers.Length == 1)
                return numbers[0];

            object current = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                current = DivOperatorCache.GetAndInvokeTarget(current, numbers[i]);
            }

            return current;
        }

        [Builtin("1+")]
        public static object PlusOne(object number)
        {
            return AddOperatorCache.GetAndInvokeTarget(1, number);
        }

        [Builtin("1-")]
        public static object MinusOne(object number)
        {
            return SubOperatorCache.GetAndInvokeTarget(number, 1);
        }

        [Builtin]
        public static object Abs(object number)
        {
            TypeCode code = Type.GetTypeCode(number.GetType());
            switch (code)
            {
                case TypeCode.Boolean:
                   return (Boolean)number ? 1 : 0;
                case TypeCode.Byte:
                    return number;
                case TypeCode.Char:
                    return number;
                case TypeCode.Decimal:
                    return Math.Abs((Decimal)number);
                case TypeCode.Double:
                    return Math.Abs((Double)number);
                case TypeCode.Int16:
                    return Math.Abs((Int16)number);
                case TypeCode.Int32:
                   return Math.Abs((Int32)number);
                case TypeCode.Int64:
                   return Math.Abs((Int64)number);
                case TypeCode.SByte:
                    return Math.Abs((SByte)number);
                case TypeCode.Single:
                   return Math.Abs((Single)number);
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return number;
            }

            if (number is Complex)
                return ((Complex)number).Abs();
            if (number is Ratio)
                return ((Ratio)number).Abs();
            if (number is BigInteger)
                return (number as BigInteger).Abs();

            Conditions.ConditionsDictionary.Error("ABS: " + number + " is not a number");


            throw new Exception("Never reached");
        }

        [Builtin(Predicate = true)]
        public static object Evenp(object number)
        {
            TypeCode code = Type.GetTypeCode(number.GetType());
            switch (code)
            {
                case TypeCode.Boolean:
                    return (Boolean)number ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.Byte:
                    return (Byte)number % 2 == 1 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.Char:
                    return (Char)number % 2 == 1 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.Int16:
                    return (Int16)number % 2 == 1 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.Int32:
                    return (Int32)number % 2 == 1 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.Int64:
                    return (Int64)number % 2 == 1 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.SByte:
                    return (SByte)number % 2 == 1 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.UInt16:
                    return (UInt16)number % 2 == 1 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.UInt32:
                    return (UInt32)number % 2 == 1 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.UInt64:
                    return (UInt64)number % 2 == 1 ? DefinedSymbols.NIL : DefinedSymbols.T;
            }

            BigInteger big = number as BigInteger;
            if (big != null)
                return big % 2 == 1 ? DefinedSymbols.NIL : DefinedSymbols.T;


            Conditions.ConditionsDictionary.Error("EVENP: " + number + " is not a integer");


            throw new Exception("Never reached");
        }

        [Builtin(Predicate = true)]
        public static object Oddp(object number)
        {
            TypeCode code = Type.GetTypeCode(number.GetType());
            switch (code)
            {
                case TypeCode.Boolean:
                    return (Boolean)number ? DefinedSymbols.T : DefinedSymbols.NIL;
                case TypeCode.Byte:
                    return (Byte)number % 2 == 01 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.Char:
                    return (Char)number % 2 == 0 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.Int16:
                    return (Int16)number % 2 == 0 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.Int32:
                    return (Int32)number % 2 == 0 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.Int64:
                    return (Int64)number % 2 == 0 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.SByte:
                    return (SByte)number % 2 == 0 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.UInt16:
                    return (UInt16)number % 2 == 0 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.UInt32:
                    return (UInt32)number % 2 == 0 ? DefinedSymbols.NIL : DefinedSymbols.T;
                case TypeCode.UInt64:
                    return (UInt64)number % 2 == 0 ? DefinedSymbols.NIL : DefinedSymbols.T;
            }

            BigInteger big = number as BigInteger;
            if (big != null)
                return big % 2 == 0 ? DefinedSymbols.NIL : DefinedSymbols.T;


            Conditions.ConditionsDictionary.Error("ODDP: " + number + " is not a integer");


            throw new Exception("Never reached");
        }

        [Builtin]
        public static object Exp(dynamic number)
        {
            return Math.Exp(number);
        }

        [Builtin]
        public static object Expt(object base_number, object power_number)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Gcd(params object[] numbers)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Lcm(params object[] numbers)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Log(object number,[Optional] object _base)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Mod(dynamic number, dynamic divisor)
        {
            return number % divisor;
        }

        [Builtin]
        public static object Rem(object number, object divisor)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Signum(dynamic number)
        {
            return Math.Sign(number);
        }

        [Builtin]
        public static object Sqrt(dynamic number)
        {
            return Math.Sqrt(number);
        }

        [Builtin]
        public static object ISqrt(object number)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-random-state")]
        public static object MakeRandomState([Optional] object state)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Random(object limit, [Optional] object state)
        {
            throw new NotImplementedException();
        }

        [Builtin("random-state-p", Predicate = true)]
        public static object RandomStateP(object obj)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Numberp(object obj)
        {
            TypeCode code = Type.GetTypeCode(obj.GetType());

            switch (code)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return DefinedSymbols.T;
                default:
                    if (obj is Complex)
                        return DefinedSymbols.T;
                    if (obj is Ratio)
                        return DefinedSymbols.T;
                    if (obj is BigInteger)
                        return DefinedSymbols.T;

                    return DefinedSymbols.NIL;
            }
        }

        [Builtin]
        public static object Cis(object real)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static Complex Complex(object real, [Optional] object imaginary)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Complexp(object obj)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object CONJUGATE(object number)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Phase(object number)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Realpart(object complex)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Imagpart(object complex)
        {
            throw new NotImplementedException();
        }

        [Builtin("UPGRADED-COMPLEX-PART-TYPE")]
        public static object UpgradedComplexPartType(object type_spec, [Optional] object env)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Realp(object number)
        {
            TypeCode code = Type.GetTypeCode(number.GetType());

            switch (code)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return DefinedSymbols.T;
                default:
                    if (number is Ratio)
                        return DefinedSymbols.T;
                    if (number is BigInteger)
                        return DefinedSymbols.T;

                    return DefinedSymbols.NIL;
            } 
        }

        [Builtin]
        public static object Numerator(object rational)
        {
            throw new NotImplementedException();
        }

        [Builtin("SYSTEM::set_Numerator", OverridePackage=true)]
        public static object set_Numerator(object rational)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Denominator(object rational)
        {
            throw new NotImplementedException();
        }

        [Builtin("SYSTEM::set_Denominator", OverridePackage = true)]
        public static object set_Denominator(object rational)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static Ratio Rational(object real)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static Ratio Rationalize(object real)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate=true)]
        public static object Rationalp(object obj)
        {
            return obj is Ratio ? DefinedSymbols.T : DefinedSymbols.NIL;
        }

        [Builtin]
        public static object Ash(dynamic integer, dynamic count)
        {
            int sign = Math.Sign(count);
            switch (sign)
            {
                case 0:
                    return integer;
                case 1:
                    return integer << count;
                case -1:
                    return integer >> count;
                default:
                    throw new Exception("unreachable");
            }
        }

        [Builtin("integer-length")]
        public static object IntegerLength(object integer)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Integerp(object obj)
        {
            if (obj is BigInteger)
                return DefinedSymbols.T;
            var typeCode = Type.GetTypeCode(obj.GetType());
            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DBNull:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Empty:
                    return DefinedSymbols.NIL;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return DefinedSymbols.T;
                case TypeCode.Object:
                    return DefinedSymbols.NIL;
                case TypeCode.SByte:
                    return DefinedSymbols.T;
                case TypeCode.Single:
                case TypeCode.String:
                    return DefinedSymbols.NIL;
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return DefinedSymbols.T;
                default:
                    throw new Exception("unreachable");
            }
        }

        //parse-integer string &key start end radix junk-allowed 
        [Builtin("parse-integer", ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer ParseInteger(object str, [Key] object start, [Key] object end, [Key] object radix, [Key] object junk_allowed)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Boole(object op, object number1, object number2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Logand(params object[] integers)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Logandc1(object integer1, object integer2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Logandc2(object integer1, object integer2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Logeqv(params object[] integers)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Logior(params object[] integers)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Logor(object integer1, object integer2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Lognot(object integer1)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Logorc1(object integer1, object integer2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Logorc2(object integer1, object integer2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Logxor(params object[] integers)
        {
            throw new NotImplementedException();
        }
    }
}

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol PI;

        public static Symbol _Random_State_;

        public static Symbol BOOLE_1;
        public static Symbol BOOLE_2;
        public static Symbol BOOLE_AND;
        public static Symbol BOOLE_ANDC1;
        public static Symbol BOOLE_ANDC2;
        public static Symbol BOOLE_C1;
        public static Symbol BOOLE_C2;
        public static Symbol BOOLE_CLR;
        public static Symbol BOOLE_EQV;
        public static Symbol BOOLE_IOR;
        public static Symbol BOOLE_NAND;
        public static Symbol BOOLE_NOR;
        public static Symbol BOOLE_ORC1;
        public static Symbol BOOLE_ORC2;
        public static Symbol BOOLE_SET;
        public static Symbol BOOLE_XOR;

        public static void InitializeNumbersDictionary()
        {
            PI = cl.Intern("PI", true);
            PI.Value = Math.PI;
            PI.IsConstant = true;
        }
    }
}