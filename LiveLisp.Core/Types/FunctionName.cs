using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;

namespace LiveLisp.Core.Types
{
    public enum FunctionNameParseResult
    {
        OK,
        NotAName,
        MalformedSetfList,
        SetfNameNotASymbol
    }

    public abstract class FunctionName
    {
        public abstract LispFunction Function
        {
            get;
            set;
        }

        public abstract bool Bound
        {
            get;
            set;
        }

        public static FunctionNameParseResult Create(object obj, out FunctionName name)
        {
            
            Symbol s = obj as Symbol;
            if (s != null)
            {
                name = new SymbolFunctionName(s);
                return FunctionNameParseResult.OK;
            }

            Cons c = obj as Cons;
            if (c != null && c.Car == DefinedSymbols.SETF)
            {
                if (c.IsProperList && c.Count == 2)
                {
                    s = c.Cdr as Symbol;
                    if (s == null)
                    {
                        name = null;
                        return FunctionNameParseResult.SetfNameNotASymbol;
                    }
                    name = new SetfFunctionName(s);
                    return FunctionNameParseResult.OK;
                }
                else
                {
                    name = null;
                    return FunctionNameParseResult.MalformedSetfList;
                }
            }

            name = null;
            return FunctionNameParseResult.NotAName;
        }

        internal static void ReportError(Symbol name, FunctionNameParseResult result, object function)
        {
            throw new NotImplementedException();
        }
    }

    public class SymbolFunctionName : FunctionName
    {
        public Symbol Symbol
        {
            get;
            set;
        }

        public SymbolFunctionName(Symbol s)
        {
            Symbol = s;
        }

        public override LispFunction Function
        {
            get { return Symbol.Function; }
            set { Symbol.Function = value; }
        }

        public override bool Bound
        {
            get
            {
                return Symbol.FBound;
            }
            set{Symbol.FBound = false;}
        }
    }

    public class SetfFunctionName : FunctionName
    {
        public Symbol Symbol
        {
            get;
            set;
        }

        public SetfFunctionName(Symbol s)
        {
            Symbol = s;
        }

        public override LispFunction Function
        {
            get { return Symbol.SetfFunction; }
            set { Symbol.SetfFunction = value; }
        }

        public override bool Bound
        {
            get { return Symbol.SetfFBound; }
            set { Symbol.SetfFBound = false; }
        }
    }
}
