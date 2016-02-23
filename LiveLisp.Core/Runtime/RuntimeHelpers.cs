using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LiveLisp.Core.Types;
using LiveLisp.Core.BuiltIns.Conditions;
using System.Collections;
using LiveLisp.Core.AST;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.BuiltIns.Printer;

namespace LiveLisp.Core.Runtime
{
    public static class RuntimeHelpers
    {
        public static StaticMethodResolver ReferenceEqualsMethod = typeof(object).GetMethod("ReferenceEquals", BindingFlags.Public | BindingFlags.Static);
        public static StaticMethodResolver ArrayCopyMethod = typeof(Array).GetMethod("Copy", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(Array), typeof(int), typeof(Array), typeof(int), typeof(int) }, null);
        public static StaticMethodResolver GuidEqualsMethod = typeof(Guid).GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Guid) }, null);
        public static StaticMethodResolver ProgvBindingHelperMethod = typeof(RuntimeHelpers).GetMethod("ProgvBindingHelper", BindingFlags.Public | BindingFlags.Static);
        public static StaticMethodResolver RestoreProgvHelperMethod = typeof(RuntimeHelpers).GetMethod("RestoreProgvHelper", BindingFlags.Public | BindingFlags.Static);
        public static StaticMethodResolver SimplifyDoubleMethod = typeof(RuntimeHelpers).GetMethod("SimplifyDouble", BindingFlags.Public | BindingFlags.Static);

        public static StaticMethodResolver Int32AddMethod = typeof(RuntimeHelpers).GetMethod("Int32Add", BindingFlags.Public | BindingFlags.Static);
        public static StaticMethodResolver Int32MulMethod = typeof(RuntimeHelpers).GetMethod("Int32Mul", BindingFlags.Public | BindingFlags.Static);
        public static StaticMethodResolver Int32SubMethod = typeof(RuntimeHelpers).GetMethod("Int32Sub", BindingFlags.Public | BindingFlags.Static);
        public static StaticMethodResolver Int32DivMethod = typeof(RuntimeHelpers).GetMethod("Int32Div", BindingFlags.Public | BindingFlags.Static);

        public static StaticMethodResolver Int64AddMethod = typeof(RuntimeHelpers).GetMethod("Int64Add", BindingFlags.Public | BindingFlags.Static);
        public static StaticMethodResolver Int64MulMethod = typeof(RuntimeHelpers).GetMethod("Int64Mul", BindingFlags.Public | BindingFlags.Static);
        public static StaticMethodResolver Int64SubMethod = typeof(RuntimeHelpers).GetMethod("Int64Sub", BindingFlags.Public | BindingFlags.Static);
        public static StaticMethodResolver Int64DivMethod = typeof(RuntimeHelpers).GetMethod("Int64Div", BindingFlags.Public | BindingFlags.Static);
        
        /// <summary>
        /// Used with br.false so actual return doesn't matter
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool LispIf(object obj)
        {
            if (obj == DefinedSymbols.NIL)
                return false;

            return true;
        }

        public static readonly MethodInfo LispIfMethod;

        static RuntimeHelpers()
        {
            var selfType = typeof(RuntimeHelpers);
            var flags = BindingFlags.Static | BindingFlags.Public;
            LispIfMethod = selfType.GetMethod("LispIf", flags);
        }

        public static T Coerce<T>(object obj, string function_name, string arg_name) where T : class
        {
            T ret = obj as T;
            if (ret == null)
                throw new TypeErrorException(function_name + ": " + arg_name + " is not a " + typeof(T).Name + " got " + obj);

            return ret;
        }

        //TODO: replace Cons keyslist with List<Symbol> keyslist
        public static Dictionary<int,object> ValidateListForKeyParameters(Symbol name, object list, Cons keyslist, bool allowOtherKeysByDefault)
        {
            if (list == DefinedSymbols.NIL)
                return new Dictionary<int, object>();

            Cons cons = list as Cons;

            if ((cons.Count % 2) != 0)
            {
                ConditionsDictionary.Error(name.ToString() + ": keyword arguments in " + list + " should occur pairwise");
            }

            // check for allow-other-keys override

            object overr = cons[DefinedSymbols.KAllowOtherKeys];
            if (overr != UnboundValue.Unbound)
            {
                if (overr == DefinedSymbols.NIL)
                    allowOtherKeysByDefault = false;
                else
                    allowOtherKeysByDefault = true;
            }

            //

            Dictionary<int, object> pairs = new Dictionary<int, object>();
 
            IEnumerator enumerator = cons.GetEnumerator();
            object key;
            object value;
            while (enumerator.MoveNext())
            {
                key = enumerator.Current;
                enumerator.MoveNext();
                value = enumerator.Current;
                if (keyslist.Contains(key))
                {
                    if (!pairs.ContainsKey((key as Symbol).Id))
                    {
                        pairs.Add((key as Symbol).Id, value);
                    }
                }
                else
                {
                    if (!allowOtherKeysByDefault)
                        ConditionsDictionary.Error(name.ToString() + ": illegal keyword/value pair " + key + ", " + value + " in argument list. The allowed keywords are " + keyslist);
                }
            }

            return pairs;
        }

        public static LispFunction CoerceToFunction(object obj, string Caller_name, string arg_name)
        {
            Symbol symbol = obj as Symbol;
            if (symbol != null)
            {
                return symbol.Function;
            }

            LispFunction ret = obj as LispFunction;
            if (ret == null)
                throw new TypeErrorException(Caller_name + ": " + arg_name + " is not a function designator");

            return ret;
        }

        public static void ProgvBindingHelper(object _sym_list, object _vals_list, ref List<object> backup)
        {
            Cons sym_list = _sym_list as Cons;
            if (sym_list == null)
                throw new TypeErrorException("PROGV: symbols list is not a list - " + _sym_list);

            if (!sym_list.IsProperList)
                throw new TypeErrorException("PROGV: symbols list is not a proper list - " + sym_list);

            Cons vals_list = _vals_list as Cons;
            if (vals_list == null)
                throw new TypeErrorException("PROGV: values list is not a list - " + _vals_list);

            if (!vals_list.IsProperList)
                throw new TypeErrorException("PROGV: values list is not a proper list - " + _vals_list);

            backup = new List<object>(sym_list.Count);

            while (sym_list != null)
            {
                Symbol var = sym_list.Car as Symbol;
                if(var == null)
                    throw new TypeErrorException("PROGV: variable is not a symbol - " + sym_list.Car);

                backup.Add(var.RawValue);
                if(vals_list!=null)
                    var.RawValue = vals_list.Car;
                else
                    var.RawValue = UnboundValue.Unbound;

                sym_list = sym_list.Child;

                if(vals_list != null)
                    vals_list = vals_list.Child;
            }
        }

        public static void RestoreProgvHelper(object _sym_list, List<object> backup)
        {
            Cons sym_list = _sym_list as Cons;
            if (sym_list == null)
                throw new TypeErrorException("PROGV: symbols list is not a list - " + _sym_list);

            if (!sym_list.IsProperList)
                throw new TypeErrorException("PROGV: symbols list is not a proper list - " + sym_list);

            /*if(sym_list.Count != backup.Count)
                throw new */

            int index = 0;
            while (sym_list != null)
            {
                (sym_list.Car as Symbol).RawValue = backup[index];

                sym_list = sym_list.Child;

                index++;
            }
        }

        public static StaticMethodResolver CoerceToFunctionMethod = typeof(RuntimeHelpers).GetMethod("CoerceToFunction");
        

        internal static void PrintTraceEnter(SymbolFunctionDesignator symbolFunctionDesignator, int callLevel)
        {
            StringBuilder sb = new StringBuilder();

            ComputeTabs(callLevel, sb);
            sb.Append("ENTER ");
            sb.Append(symbolFunctionDesignator.Name);
            sb.Append(" ");
            PrinterDictionary.Print(sb.ToString(), DefinedSymbols._Standard_Output_.Value);
        }

        internal static void PrintTraceExit(SymbolFunctionDesignator symbolFunctionDesignator, object[] args, object ret, int callLevel)
        {
            StringBuilder sb = new StringBuilder();
            ComputeTabs(callLevel, sb);
            sb.Append("EXIT (");
            sb.Append(symbolFunctionDesignator.Name);
            ComputeArgs(args, sb);
            sb.Append(") => ");
            PrintReturn(ret, sb);
            PrinterDictionary.Print(sb.ToString(), DefinedSymbols._Standard_Output_.Value);
        }

        private static void ComputeTabs(int callLevel, StringBuilder sb)
        {
            for (int i = 1; i < callLevel; i++)
            {
                sb.Append("\t");
            }
        }

        private static void ComputeArgs(object[] _args, StringBuilder sb)
        {
            for (int i = 0; i < _args.Length; i++)
            {
                sb.Append(" " + _args[i]);
            }
        }

        private static void PrintReturn(object ret, StringBuilder sb)
        {
            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                PrintValues(values, sb);
            else sb.Append(ret.ToString());
        }

        private static void PrintValues(MultipleValuesContainer values, StringBuilder sb)
        {
            if (values.Count == 0)
            {
                sb.Append("No values");
                return;
            }

            for (int i = 0; i < values.Count; i++)
            {
                sb.Append(values[i] + ", ");
            }

            sb.Remove(sb.Length - 3, 2);
        }

        public static object SimplifyDouble(double d)
        {
            if (Int32.MinValue <= d && d <= Int32.MaxValue)
                return (Int32)d;
            if (Int32.MaxValue <= d && d <= Int32.MaxValue)
                return (Int32)d;

            return d;
        }

        public static object Int32Add(Int32 arg1, Int32 arg2)
        {
            long l = (Int64)arg1 + (Int64)arg2;
            if (Int32.MinValue <= l && l <= Int32.MaxValue)
                return (Int32)l;
            else
                return l;
        }

        public static object Int32Mul(Int32 arg1, Int32 arg2)
        {
            long l = Math.BigMul(arg1, arg2);
            if (Int32.MinValue <= l && l <= Int32.MaxValue)
                return (Int32)l;
            else
                return l;
        }

        public static object Int32Sub(Int32 arg1, Int32 arg2)
        {
            long l = (Int64)arg1 - (Int64)arg2;
            if (Int32.MinValue <= l && l <= Int32.MaxValue)
                return (Int32)l;
            else
                return l;
        }

        public static object Int32Div(Int32 arg1, Int32 arg2)
        {
            return SimplifyDouble((double)arg1 / arg2);
        }

        public static object Int64Add(Int64 arg1, Int64 arg2)
        {
            BigInteger l = (BigInteger)arg1 + (BigInteger)arg2;
            long lo;
            if (l.AsInt64(out lo))
                return lo;
            else
                return l;
        }

        public static object Int64Mul(Int64 arg1, Int64 arg2)
        {
            BigInteger l = (BigInteger)arg1 * (BigInteger)arg2;
            long lo;
            if (l.AsInt64(out lo))
                return lo;
            else
                return l;
        }

        public static object Int64Sub(Int64 arg1, Int64 arg2)
        {
            BigInteger l = (BigInteger)arg1 - (BigInteger)arg2;
            long lo;
            if (l.AsInt64(out lo))
                return lo;
            else
                return l;
        }

        public static object Int64Div(Int64 arg1, Int64 arg2)
        {
            return SimplifyDouble((double)arg1 / arg2);
        }
    }
}
