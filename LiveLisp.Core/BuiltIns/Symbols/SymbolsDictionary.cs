using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
//using LiveLisp.Core.BuiltIns.Strings;
using LiveLisp.Core.BuiltIns.Environment;
using LiveLisp.Core.BuiltIns.DataAndControlFlow;
using LiveLisp.Core.BuiltIns.Conditions;
using LiveLisp.Core.BuiltIns.Numbers;

namespace LiveLisp.Core.BuiltIns.Symbols
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class SymbolsDictionary
    {
        [Builtin(Predicate = true)]
        public static object Symbolp(object obj)
        {
            if (obj is Symbol)
            {
                return DefinedSymbols.T;
            }

            else return DefinedSymbols.NIL;
        }

        [Builtin(Predicate = true)]
        public static object Keywordp(object obj)
        {
            if (obj is KeywordSymbol)
            {
                return DefinedSymbols.T;
            }

            else return DefinedSymbols.NIL;
        }

        [Builtin("make-symbol")]
        public static Symbol MakeSymbol(object name)
        {

            /*
            string lstring = name as string;

            if (lstring == null)
            {
                string clrstring = name as string;
                if (clrstring == null)
                {
                    ConditionsDictionary.Error(new TypeError((string)"name", (string)"string")); 
                }
            }

            return new Symbol(lstring);
             * 
             */

            throw new NotImplementedException();
        }

        [Builtin("copy-symbol")]
        public static Symbol CopySymbol(object symbol, [Optional] object copy_properties)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("COPY-SYMBOL: argument 1 is not a symbol (" + symbol + ")");


            Symbol newSymbol = new Symbol(s.Name);

            if (copy_properties != DefinedSymbols.NIL)
            {
                newSymbol.Value = s.Value;
                newSymbol.Function = s.Function;
                newSymbol.PropertyList = s.PropertyList.MakeCopy();
            }

            return newSymbol;
        }

        [Builtin]
        public static Symbol Gensym([Optional] object x)
        {
            if (x == DefinedSymbols.NIL)
            {
                int counter = DefinedSymbols._Gensym_Counter_.ValueAsInteger;

                return new Symbol("G" + counter++);
            }

            if (!(x is int))
            {
                string s = x as string;
                if (s == null)
                {
                    ConditionsDictionary.TypeError("GENSYM: invalid argument " + x);
                }

                int counter = DefinedSymbols._Gensym_Counter_.ValueAsInteger;
                return new Symbol(s + counter);

            }
            else
            {
                return new Symbol("G" + (int)x);
            }
        }

        [Builtin]
        public static object Gentemp([Optional("\"T\"")] object prefix, [Optional("*package*")] object package)
        {
            throw new NotImplementedException();
        }

        [Builtin("symbol-function")]
        public static object SymbolFunction(object symbol)
        {
            throw new NotImplementedException();
        }

        [Builtin("SYSTEM::set-symbol-function", OverridePackage=true)]
        public static LispFunction set_SymbolFunction(object symbol, object content)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("(SETF SYMBOL-FUNCTION): argument 1 is not a symbol (" + symbol + ")");

            LispFunction _method = content as LispFunction;
            if (_method == null)
                ConditionsDictionary.TypeError("(SETF SYMBOL-FUNCTION): argument 2 is not a function (" + content + ")");

            s.Function = _method;

            return _method;
        }

        [Builtin("SYSTEM::set-symbol-macro", OverridePackage = true)]
        public static LispFunction set_SymbolMacro(object symbol, object content)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("(SETF SYMBOL-MACRO): argument 1 is not a symbol (" + symbol + ")");

            LispFunction _method = content as LispFunction;
            if (_method == null)
                ConditionsDictionary.TypeError("(SETF SYMBOL-MACRO): argument 2 is not a function (" + content + ")");

            s.Macro = _method;

            return _method;
        }

        [Builtin("symbol-name")]
        public static string SymbolName(object symbol)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("SYMBOL-NAME: argument 1 is not a symbol (" + symbol + ")");

            return s.Name;
        }

        [Builtin("symbol-package")]
        public static object SymbolPackage(object symbol)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("SYMBOL-PACKAGE: argument 1 is not a symbol (" + symbol + ")");

            if (s.Package == null)
                return DefinedSymbols.NIL;
            else
                return s.Package;
        }

        [Builtin("symbol-plist")]
        public static object SymbolPlist(object symbol)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("SYMBOL-PLIST: argument 1 is not a symbol (" + symbol + ")");

            if (s.PropertyList.Count == 0)
                return DefinedSymbols.NIL;
            else
                return s.PropertyList;
        }

        [Builtin("SYSTEM::set-symbol-plist", OverridePackage = true)]
        public static object set_SymbolPlist(object symbol, object plist)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("(SETF SYMBOL-PLIST): argument 1 is not a symbol (" + symbol + ")");

            Cons list = null;
            if (plist == DefinedSymbols.NIL)
            {
                s.PropertyList = new SurrogateNilCons();
                return plist;
            }
            else if ((list = plist as Cons) == null)
            {
                ConditionsDictionary.TypeError("(SETF SYMBOL-PLIST): argument 2 is not a list (" + plist + ")");
            }
            else if (!(list.IsProperList && (list.Count % 2) == 0))
            {
                ConditionsDictionary.TypeError("(SETF SYMBOL-PLIST): argument 2 is not a property list (" + plist + ")");
            }


            s.PropertyList = list;
            return plist;
        }

        [Builtin("symbol-value")]
        public static object SymbolValue(object symbol)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("SYMBOL-VALUE: argument 1 is not a symbol (" + symbol + ")");

            return s.Value;
        }

        [Builtin("SYSTEM::set-symbol-value", OverridePackage = true)]
        public static object set_SymbolValue(object symbol, object value)
        {
            throw new NotImplementedException();
        }

        public static object Get(object symbol, object indicator, [Optional] object def)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("GET: argument 1 is not a symbol (" + symbol + ")");

            object ret = s.PropertyList[indicator];

            if (ret == UnboundValue.Unbound)
                return def;
            else
                return ret;
        }

        [Builtin("SYSTEM::set-get", OverridePackage=true)]
        public static object set_Get(object symbol, object indicator, object value)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("(SETF GET): argument 1 is not a symbol (" + symbol + ")");

            s.PropertyList.AddKeyValue(indicator, value);

            return value;
        }

        [Builtin(Predicate = true)]
        public static object Remprop(object symbol, object indicator)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("REMPROP: argument 1 is not a symbol (" + symbol + ")");

            return s.PropertyList.RemoveKey(indicator) ? DefinedSymbols.T : DefinedSymbols.NIL;
        }

        [Builtin(Predicate = true)]
        public static object Boundp(object symbol)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("BOUNDP: argument 1 is not a symbol (" + symbol + ")");

            return s.Boundp ? DefinedSymbols.T : DefinedSymbols.NIL;
        }

        [Builtin]
        public static object Makeunbound(object symbol)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("MAKEUNBOUND: argument 1 is not a symbol (" + symbol + ")");

            s.Makunbound();

            return s;
        }

        [Builtin]
        public static object Set(object symbol, object value)
        {
            Symbol s = symbol as Symbol;

            if (s == null)
                ConditionsDictionary.TypeError("SET: argument 1 is not a symbol (" + symbol + ")");

            s.Value = value;

            return value;
        }
    }
}

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol _Gensym_Counter_;
        public static Symbol Symbol;
        public static Symbol SymbolFunction;


        // TODO: make private
        public static void InitializeSymbolsDictionarySymbols()
        {
            _Gensym_Counter_ = cl.Intern("*GENSYM-COUNTER*", true);
            _Gensym_Counter_.Value = 0;
        }
    }
}
