using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.Reader
{
    [global::System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public sealed class ReaderMacroAttribute : Attribute
    {
        public char Char
        {
            get;
            set;
        }

        public char Dispatch
        {
            get;
            set;
        }

        public ReaderMacroAttribute(char macroChar)
        {
            Char = macroChar;
        }
    }

    
}
