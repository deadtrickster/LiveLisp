using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using LiveLisp.Core.BuiltIns.Numbers;
using System.Threading;
using LiveLisp.Core.BuiltIns.Strings;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.BuiltIns.Environment
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class EnvironmentDictionary
    {
        [Builtin("decode-universal-time", ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer DecodeUniversalTime(object unversal_time, [Optional] object time_zone)
        {
            throw new NotImplementedException();
        }

        [Builtin("encode-universal-time")]
        public static object EncodeUniversalTime(object second, object minute, object hour, object date, object month, object year, [Optional] object time_zone)
        {
            throw new NotImplementedException();
        }

        [Builtin("get-universal-time")]
        public static object GetUniversalTime()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// second, minute, hour, date, month, year, day, daylight-p, zone
        /// </summary>
        /// <returns></returns>
        [Builtin("get-decoded-time", ValuesReturnPolitics=ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer GetDecodedTime()
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static void Sleep(object _seconds)
        {
            int seconds;

            if(!(_seconds is int))
                throw new TypeErrorException("SLEEP: seconds is not an integer");

            seconds = (int)_seconds;

            Thread.Sleep(seconds * 1000);
        }

        [Builtin]
        public static void mSleep(object _seconds)
        {
            int seconds;

            if (!(_seconds is int))
                throw new TypeErrorException("MSSLEEP: seconds is not an integer");

            seconds = (int)_seconds;

            Thread.Sleep(seconds);
        }

        [Builtin]
        public static void Apropos(object name_part, [Optional] object _package)
        {
            List<Symbol> symbols = _apropos("APROPOS", name_part, _package);

            for (int i = 0; i < symbols.Count; i++)
            {
                DefinedSymbols.Print.VoidInvoke(PrintSymbol(symbols[i]));
            }
        }

        private static List<Symbol> _apropos(string name, object name_part, object _package)
        {
            string str = null;
            if (name_part is string)
                str = name_part.ToString();
           // else if (name_part is LispString)
            //    str = (name_part as LispString).ToBaseString();

            else ConditionsDictionary.Error(name + ": first argument {0} is not a STRING", name_part);

            Package package = null;
            List<Symbol> symbols;
            if (_package != DefinedSymbols.NIL)
            {
                package = _package as Package;

                if (package == null)
                {
                    Symbol pk_name = _package as Symbol;
                    if (pk_name == null)
                    {
                        ConditionsDictionary.Error(name + ": second argument {0} is not a PACKAGE designator", _package);
                    }

                    package = PackagesCollection.FindByNameOrNickname(pk_name.Name);
                    if (package == null)
                    {
                        ConditionsDictionary.Error(name + ": unable to find package by designator argument {0} ", _package);
                    }
                }

                symbols = package.GetAllAcessibleSymbols(s => s.Name.ToUpper().Contains(str.ToUpper()));
            }
            else
            {
                symbols = new List<Symbol>();
                for (int i = 0; i < PackagesCollection.PackagesCount; i++)
                {
                    symbols.AddRange(PackagesCollection.GetAtIndex(i).GetAllAcessibleSymbols(s => s.Name.ToUpper().Contains(str.ToUpper())));
                }
            }
            return symbols;
        }

        private static string PrintSymbol(Symbol symbol)
        {
            return symbol.ToString(true) + " CONSTANT: " + symbol.IsConstant + " DYNAMIC: " + symbol.IsDynamic
                                + " FBOUND: " + symbol.FBound;
        }

        [Builtin("apropos-list")]
        public static object AproposList(object name, [Optional] object package)
        {
            var symbols = _apropos("APROPOS-LIST", name, package);
            if (symbols.Count == 0)
                return DefinedSymbols.NIL;
            else
            {
                return Cons.FromCollection(symbols);
            }
        }

        [Builtin]
        public static void Describe(object obj, [Optional] object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("SYSTEM::%TRACE", OverridePackage=true)]
        public static object Trace(params object[] _symbols)
        {
            Symbol[] symbols = new Symbol[_symbols.Length];

            for (int i = 0; i < _symbols.Length; i++)
            {
                symbols[i] = _symbols[i] as Symbol;

                if (symbols[i] == null)
                    throw new TypeErrorException("TRACE: " + _symbols[i] + " is not a symbol");
            }

            Interpreter.Interpreter.AddFunctionsToTrace(symbols);

            return symbols;
        }

        [Builtin("SYSTEM::%UNTRACE", OverridePackage = true)]
        public static object UnTrace(params object[] _symbols)
        {
            Symbol[] symbols = new Symbol[_symbols.Length];

            for (int i = 0; i < _symbols.Length; i++)
            {
                symbols[i] = _symbols[i] as Symbol;

                if (symbols[i] == null)
                    throw new TypeErrorException("UNTRACE: " + _symbols[i] + " is not a symbol");
            }

            Interpreter.Interpreter.Untrace(symbols);

            return symbols;
        }

        [Builtin("get-internal-real-time")]
        public static object GetInternalReadTime()
        {
            throw new NotImplementedException();
        }

        [Builtin("get-internal-run-time")]
        public static object GetInternalRunTime()
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static void Dissasemble(object fn)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static void Room([Optional(NextParamIsPresentedIndicator=true)] object x, object suppliedp)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static void Ed([Optional] object x)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static void Inspect(object x)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static void Dribble([Optional] object pathname)
        {
            throw new NotImplementedException();
        }

        [Builtin("lisp-implementation-type")]
        public static string LispImplementationType()
        {
            throw new NotImplementedException();
        }

        [Builtin("lisp-implementation-version")]
        public static string LispImplementationVersion()
        {
            throw new NotImplementedException();
        }

        [Builtin("short-site-name")]
        public static string ShortSiteName()
        {
            throw new NotImplementedException();
        }

        [Builtin("long-site-name")]
        public static string LongSiteName()
        {
            throw new NotImplementedException();
        }

        [Builtin("machine-instance")]
        public static string MachineInstance()
        {
            throw new NotImplementedException();
        }

        [Builtin("machine-type")]
        public static string MachineType()
        {
            throw new NotImplementedException();
        }

        [Builtin("machine-version")]
        public static object MachineVersion()
        {
            throw new NotImplementedException();
        }

        [Builtin("software-type")]
        public static object SoftwareType()
        {
            throw new NotImplementedException();
        }

        [Builtin("software-version")]
        public static object SoftwareVersion()
        {
            throw new NotImplementedException();
        }

        [Builtin("user-homedit-pathname")]
        public static object UserHomedirPathname([Optional] object host)
        {
            throw new NotImplementedException();
        }
    }
}

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        /// <summary>
        /// 10000000
        /// </summary>
        public static Symbol INTERNAL_TIME_UNITS_PER_SECOND;

        /// <summary>
        /// -
        /// </summary>
        public static Symbol Minus;

        public static Symbol Plus;

        public static Symbol PlusPlus;

        public static Symbol PlusPlusPlus;

        public static Symbol Star;

        public static Symbol StarStar;

        public static Symbol StarStarStar;

        public static Symbol Slash;

        public static Symbol SlashSlash;

        public static Symbol SlashSlashSlash;

    }
}
