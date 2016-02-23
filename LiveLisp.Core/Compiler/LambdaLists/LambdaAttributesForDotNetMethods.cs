using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST;
using LiveLisp.Core.Eval;

namespace LiveLisp.Core.Compiler
{
    [global::System.AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class LambdaParamAttribute : Attribute
    {

    }

    /// <summary>
    /// как params в c#. 
    /// file:///R:/Desktop/lisp/http___www.lisp.org_HyperSpec_FrontMatter_Chapter-Index.html/www.lisp.org/HyperSpec/Body/sec_3-4-1-3.html
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class RestAttribute : LambdaParamAttribute
    {
        public bool AtLeastOne
        {
            get;
            set;
        }

        public RestAttribute()
        {
        }
    }

    [global::System.AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class OptionalAttribute : NextParamIsPresentedIndicatorAttribute
    {
        public OptionalAttribute()
        {
        }

        public OptionalAttribute(object defValue)
        {
            
        }
    }

    [global::System.AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class KeyAttribute : NextParamIsPresentedIndicatorAttribute
    {
        public string KeySymbolString
        {
            get;
            private set;
        }

        // This is a positional argument
        public KeyAttribute(string keySymbolString)
        {
            KeySymbolString = keySymbolString;
        }

        public KeyAttribute()
        {

        }
    }

    /// <summary>
    /// Use it with standard .net attributes like OptionalAttribute & DefaultParameteValueAtribute
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class NextParamIsPresentedIndicatorAttribute : LambdaParamAttribute
    {
        public string DefaultValue
        {
            get;
            set;
        }

        public bool NextParamIsPresentedIndicator
        {
            get;
            set;
        }

        public NextParamIsPresentedIndicatorAttribute()
        {
        }

        public NextParamIsPresentedIndicatorAttribute(string defValue)
        {
            DefaultValue = defValue;
        }
    }

    [global::System.AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class AuxAttribute : LambdaParamAttribute
    {
        public string DefaultValue
        {
            get;
            set;
        }

        public AuxAttribute(string defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}
