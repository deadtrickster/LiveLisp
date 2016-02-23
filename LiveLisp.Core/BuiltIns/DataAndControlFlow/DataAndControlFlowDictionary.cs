using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Types;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.BuiltIns.DataAndControlFlow
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class DataAndControlFlowDictionary
    {
        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.Sometimes)]
        // TODO: rewrite with lisp code or with excplicit LispFunction override (because may return multiple values)
        public static object Apply(object function, params object[] args)
        {
            LispFunction method = function as LispFunction;
            if (method != null)
            {
                return method.ValuesInvoke(args);
            }

            FunctionName name;

            var result = FunctionName.Create(function, out name);

            if (result != FunctionNameParseResult.OK)
            {
                FunctionName.ReportError(DefinedSymbols.Apply, result, function);
            }
            args = SpreadArgs(args);
            return name.Function.ValuesInvoke(args);
        }

        private static object[] SpreadArgs(object[] args)
        {
            List<object> ret = new List<object>(args.Length);
            for (int i = 0; i < args.Length-1; i++)
            {
                ret.Add(args[i]);
            }

            Cons c = args[args.Length - 1] as Cons;

            if (c == null)
                ConditionsDictionary.TypeError("The last element of spreadable list is not a list");

            if(!c.IsProperList)
                ConditionsDictionary.TypeError("The last element of spreadable list is not a proper list");

            while (c != null)
            {
                ret.Add(c.Car);
                c = c.Child;
            }

            return ret.ToArray();
        }

        [Builtin]
        public static object Fdefinition(object function_name)
        {
            FunctionName name;

            var result = FunctionName.Create(function_name, out name);

            if (result != FunctionNameParseResult.OK)
            {
                FunctionName.ReportError(DefinedSymbols.Apply, result, function_name);
            }

            return name.Function;
        }

        [Builtin("SYSTEM::set_Fdefinition", OverridePackage = true)]
        public static object set_Fdefinition(object function_name, object function)
        {
            FunctionName name;

            var result = FunctionName.Create(function_name, out name);

            if (result != FunctionNameParseResult.OK)
            {
                FunctionName.ReportError(DefinedSymbols.Apply, result, function_name);
            }

            LispFunction func = function as LispFunction;
            if (func == null)
                ConditionsDictionary.TypeError("(SETF FDEFINITION): second argument is not a function");

            name.Function = func;
            return func;
        }

        [Builtin(Predicate = true)]
        public static object Fboundp(object function_name)
        {
            FunctionName name;

            var result = FunctionName.Create(function_name, out name);

            if (result != FunctionNameParseResult.OK)
            {
                FunctionName.ReportError(DefinedSymbols.FBoundp, result, function_name);
            }

            return name.Bound ? DefinedSymbols.T : DefinedSymbols.NIL;
        }

        [Builtin]
        public static object Fmakeunbound(object function_name)
        {
            FunctionName name;

            var result = FunctionName.Create(function_name, out name);

            if (result != FunctionNameParseResult.OK)
            {
                FunctionName.ReportError(DefinedSymbols.Fmakeunbound, result, function_name);
            }

            name.Bound = false;

            return function_name;
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.Sometimes)]
        // TODO: rewrite with lisp code or with excplicit LispFunction override (because may return multiple values)
        public static object Funcall(object function, params object[] args)
        {
            LispFunction method = function as LispFunction;
            if (method != null)
            {
                return method.ValuesInvoke(args);
            }

            FunctionName name;

            var result = FunctionName.Create(function, out name);

            if (result != FunctionNameParseResult.OK)
            {
                FunctionName.ReportError(DefinedSymbols.Funcall, result, function);
            }

            return name.Function.ValuesInvoke(args);
        }

        [Builtin("function-lambda-expression")]
        // TODO: rewrite with lisp code or with excplicit LispFunction override (because always return multiple values)
        public static object FunctionLambdaExpression(object function)
        {
            LispFunction method = function as LispFunction;
            if (method != null)
            {
                return DefinedSymbols.Values.Function.ValuesInvoke(method.LambdaExpression, method.IsClosure ? DefinedSymbols.T : DefinedSymbols.NIL, method.Name);
            }

            return ConditionsDictionary.TypeError("FUNCTION-LABMDA-EXPRESSION: first arg is not a function");
        }

        [Builtin(Predicate = true)]
        public static object Functionp(object obj)
        {
            return obj is LispFunction ? DefinedSymbols.T : DefinedSymbols.NIL;
        }

        [Builtin("compiled-function-p", Predicate = true)]
        public static object CompiledFunctionFunctionp(object obj)
        {
            LispFunction method = obj as LispFunction;
            if (method != null)
            {
                return method.IsCompiled ? DefinedSymbols.T : DefinedSymbols.NIL;
            }

            return ConditionsDictionary.TypeError("COMPILED-FUNCTION-P: first arg is not a function");
        }

        [Builtin(Predicate = true)]
        public static object Not(object obj)
        {
            return obj == DefinedSymbols.NIL ? obj : DefinedSymbols.T;
        }

        [Builtin]
        public static object Eq(object obj1, object obj2)
        {
            if (object.ReferenceEquals(obj1, obj2))
                return DefinedSymbols.T;

            return DefinedSymbols.NIL;
        }

        [Builtin]
        public static object Eql(object obj1, object obj2)
        {
            if (object.ReferenceEquals(obj1, obj2))
                return DefinedSymbols.T;

            TypeCode tcode1 = Type.GetTypeCode(obj1.GetType());

            if (obj1.GetType() == obj2.GetType())
            {
                switch (tcode1)
                {
                    case TypeCode.Boolean:
                        if ((bool)obj1 == (bool)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.Byte:
                        if ((Byte)obj1 == (Byte)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.Char:
                        if ((Char)obj1 == (Char)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.DBNull:
                        if ((DBNull)obj1 == (DBNull)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.DateTime:
                        if ((DateTime)obj1 == (DateTime)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.Decimal:
                        if ((Decimal)obj1 == (Decimal)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.Double:
                        if ((Double)obj1 == (Double)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.Empty:
                        return DefinedSymbols.T;
                    case TypeCode.Int16:
                        if ((Int16)obj1 == (Int16)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.Int32:
                        if ((Int32)obj1 == (Int32)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.Int64:
                        if ((Int64)obj1 == (Int64)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.Object:
                        return DefinedSymbols.NIL;
                    case TypeCode.SByte:
                        if ((SByte)obj1 == (SByte)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.Single:
                        if ((Single)obj1 == (Single)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.String:
                        return DefinedSymbols.NIL;
                    case TypeCode.UInt16:
                        if ((UInt16)obj1 == (UInt16)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.UInt32:
                        if ((UInt32)obj1 == (UInt32)obj2)
                            return DefinedSymbols.T;
                        break;
                    case TypeCode.UInt64:
                        if ((UInt64)obj1 == (UInt64)obj2)
                            return DefinedSymbols.T;
                        break;
                }

                var lnum = obj1 as ILispNumber;
                if (lnum != null)
                {
                    if (lnum.CompareTo(obj2) == 0)
                    {
                        return DefinedSymbols.T;
                    }
                }
            }

            return DefinedSymbols.NIL;
        }

        [Builtin]
        public static object Equal(object obj1, object obj2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Equalp(object obj1, object obj2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Identity(object obj1)
        {
            return obj1;
        }

        [Builtin]
        public static LispFunction Constantly(object value)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Every(object predicate, [Rest] object sequences)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Some(object predicate, [Rest] object sequences)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Notevery(object predicate, [Rest] object sequences)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Notany(object predicate, [Rest] object sequences)
        {
            throw new NotImplementedException();
        }

        /*[Builtin(ValuesReturnPolitics = ValuesReturnPolitics.Always)]
        public static MultipleValuesContainer set_Values(params object[] args)
        {
            return MultipleValues.FromArray(args);
        }
         Сеттер не имплементируется как отдельная функция, просто делается диспатчинг в setf
         */

        [Builtin("GET-SETF-EXPANSION", ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer GetSetfExpansion(object place, [Optional] object environment)
        {
            throw new NotImplementedException();
        }
    }

    public class ValuesFunction : FullLispFunction
    {
        public ValuesFunction()
            : base("COMMON-LISP:VALUES")
        {
            MinArgsCount = 0;
            MaxArgsCount = Int32.MaxValue;
        }

        #region single return

        public override object Invoke()
        {
            return DefinedSymbols.NIL;
        }

        public override object Invoke(object arg1)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            return arg1;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            return arg1;
        }

        public override object Invoke(object[] args)
        {
            if (args.Length == 0)
                return DefinedSymbols.NIL;
            else
                return args[0];
        }
        #endregion

        #region values return

        public override object ValuesInvoke()
        {
            return MultipleValuesContainer.Void;
        }

        public override object ValuesInvoke(object arg1)
        {
            return new MultipleValuesContainer(arg1);
        }

        public override object ValuesInvoke(object arg1, object arg2)
        {
            var ret = new MultipleValuesContainer(arg1, 2);
            ret.Add(arg2);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3)
        {
            var ret = new MultipleValuesContainer(arg1, 3);
            ret.Add(arg2);
            ret.Add(arg3);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            var ret = new MultipleValuesContainer(arg1, 4);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            var ret = new MultipleValuesContainer(arg1, 5);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            ret.Add(arg5);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            var ret = new MultipleValuesContainer(arg1, 6);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            ret.Add(arg5);
            ret.Add(arg6);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            var ret = new MultipleValuesContainer(arg1, 7);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            ret.Add(arg5);
            ret.Add(arg6);
            ret.Add(arg7);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            var ret = new MultipleValuesContainer(arg1, 8);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            ret.Add(arg5);
            ret.Add(arg6);
            ret.Add(arg7);
            ret.Add(arg8);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            var ret = new MultipleValuesContainer(arg1, 9);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            ret.Add(arg5);
            ret.Add(arg6);
            ret.Add(arg7);
            ret.Add(arg8);
            ret.Add(arg9);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            var ret = new MultipleValuesContainer(arg1, 10);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            ret.Add(arg5);
            ret.Add(arg6);
            ret.Add(arg7);
            ret.Add(arg8);
            ret.Add(arg9);
            ret.Add(arg10);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            var ret = new MultipleValuesContainer(arg1, 11);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            ret.Add(arg5);
            ret.Add(arg6);
            ret.Add(arg7);
            ret.Add(arg8);
            ret.Add(arg9);
            ret.Add(arg10);
            ret.Add(arg11);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            var ret = new MultipleValuesContainer(arg1, 12);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            ret.Add(arg5);
            ret.Add(arg6);
            ret.Add(arg7);
            ret.Add(arg8);
            ret.Add(arg9);
            ret.Add(arg10);
            ret.Add(arg11);
            ret.Add(arg12);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            var ret = new MultipleValuesContainer(arg1, 13);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            ret.Add(arg5);
            ret.Add(arg6);
            ret.Add(arg7);
            ret.Add(arg8);
            ret.Add(arg9);
            ret.Add(arg10);
            ret.Add(arg11);
            ret.Add(arg12);
            ret.Add(arg13);
            return ret;
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            var ret = new MultipleValuesContainer(arg1, 14);
            ret.Add(arg2);
            ret.Add(arg3);
            ret.Add(arg4);
            ret.Add(arg5);
            ret.Add(arg6);
            ret.Add(arg7);
            ret.Add(arg8);
            ret.Add(arg9);
            ret.Add(arg10);
            ret.Add(arg11);
            ret.Add(arg12);
            ret.Add(arg13);
            ret.Add(arg14);
            return ret;
        }

        public override object ValuesInvoke(object[] args)
        {
            return new MultipleValuesContainer(args);
        }
        #endregion

        #region void return
        public override void VoidInvoke()
        { }

        public override void VoidInvoke(object arg1)
        { }

        public override void VoidInvoke(object arg1, object arg2)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        { }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        { }

        public override void VoidInvoke(object[] args)
        { }
        #endregion
    }

    public class ValuesListFunction : FullLispFunction
    {
        public ValuesListFunction()
            : base("COMMON-LISP:VALUES-LIST")
        {
            MinArgsCount = MaxArgsCount = 1;
        }

        public override object Invoke(object arg1)
        {
            if (DefinedSymbols.NIL == arg1)
                return DefinedSymbols.NIL;

            Cons list = RuntimeHelpers.Coerce<Cons>(arg1, "VALUES-LIST", "list");

            if (!list.IsProperList)
            {
                ConditionsDictionary.TypeError("VALUES-LIST: list " + list + " is not a proper list");
            }

            return list.Car;
        }

        public override object ValuesInvoke(object arg1)
        {
            if (DefinedSymbols.NIL == arg1)
                return MultipleValuesContainer.Void;

            Cons list = RuntimeHelpers.Coerce<Cons>(arg1, "VALUES-LIST", "list");

            if (!list.IsProperList)
            {
                ConditionsDictionary.TypeError("VALUES-LIST: list " + list + " is not a proper list");
            }

            MultipleValuesContainer ret = new MultipleValuesContainer(list.Car);

            while (list.Child != null)
            {
                list = list.Child;

                ret.Add(list.Car);
            }

            return ret;
        }

        //TODO: deside should the type checking be there
        public override void VoidInvoke(object arg1)
        {
            if (DefinedSymbols.NIL == arg1)
                return;
            Cons list = RuntimeHelpers.Coerce<Cons>(arg1, "VALUES-LIST", "list");

            if (!list.IsProperList)
            {
                ConditionsDictionary.TypeError("VALUES-LIST: list " + list + " is not a proper list");
            }
        }
    }
}

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol Apply;
        public static Symbol FBoundp;
        public static Symbol Fmakeunbound;
        public static Symbol Funcall;
        public static Symbol _CallArgumentsLimit_;
        public static Symbol _LambdaListKeywords_;
        public static Symbol _LambdaParametersLimit_;
        public static Symbol _MultipleValuesLimit_;

        public static Symbol Eq;
        public static Symbol Eql;
        public static Symbol Equal;
        public static Symbol Equalp;

        public static Symbol Values;
        public static Symbol ValuesList;

        internal static void InitializeDataAndCotnrolFlowSymbols()
        {
            Apply = cl.Intern("APPLY", true);

            _CallArgumentsLimit_ = cl.Intern("*CALL-ARGUMENTS-LIMIT*");
            _CallArgumentsLimit_.Value = Int32.MaxValue - 1;
            _CallArgumentsLimit_.IsConstant = true;

            _LambdaListKeywords_ = cl.Intern("*LAMBDA-LIST-KEYWORDS*");
            _LambdaListKeywords_.Value = Types.Cons.FromCollection(new object[] { LambdaAllowOtherKeys, LambdaAux, LambdaBody, 
                LambdaEnvironment, LambdaKey, LambdaOptional, LambdaRest, LambdaWhole, LambdaParams });
            _LambdaListKeywords_.IsConstant = true;

            _LambdaParametersLimit_ = cl.Intern("*LAMBDA-PARAMETERS-LIMIT*");
            _LambdaParametersLimit_.Value = Int32.MaxValue - 1;
            _LambdaParametersLimit_.IsConstant = true;

            _MultipleValuesLimit_ = cl.Intern("*MULTIPLE-VALUES-LIMIT*");
            _MultipleValuesLimit_.Value = Int32.MaxValue - 1;
            _MultipleValuesLimit_.IsConstant = true; 

            Eq = cl.Intern("EQ", true);
            Eql = cl.Intern("EQL", true);
            Equal = cl.Intern("EQUAL", true);
            Equalp = cl.Intern("EQUALP", true);

            Values = cl.Intern("VALUES", true);
            ValuesList = cl.Intern("VALUES-LIST", true);
        }
    }
}