using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.BuiltIns.Printer
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class PrinterDictionary
    {
        [Builtin("copy-pprint-dispatch")]
        public static object CopyPPrintDispatch([Optional] object table)
        {
            throw new NotImplementedException();
        }

        [Builtin("pprint-dispatch", ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer PPrintDispatch(object obj, [Optional] object table)
        {
            throw new NotImplementedException();
        }

        [Builtin("pprint-fill")]
        public static void PPrintFill(object stream, object obj, [Optional] object colon_p, [Optional] object at_sign_p)
        {
            throw new NotImplementedException();
        }

        [Builtin("pprint-linear")]
        public static void PPrintLinear(object stream, object obj, [Optional] object colon_p, [Optional] object at_sign_p)
        {
            throw new NotImplementedException();
        }

        [Builtin("pprint-tabular")]
        public static void PPrintTabular(object stream, object obj, [Optional] object colon_p, [Optional] object at_sign_p, [Optional] object tabsize)
        {
            throw new NotImplementedException();
        }

        [Builtin("pprint-ident")]
        public static void PPrintIdent(object realtive_to, object n, [Optional] object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("pprint-newline")]
        public static void PPrintNewline(object kind, [Optional] object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("pprint-tab")]
        public static void PPrintTab(object kind, object column, object colinc, [Optional] object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("set-pprint-dispatch")]
        public static void SetPPrintDispatch(object type_specifier, object function, [Optional] object priority, [Optional] object table)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Write(object obj, [Key] object array, [Key] object _base, [Key] object _case, [Key] object circle, [Key] object escape, 
            [Key] object gensym, [Key] object length, [Key] object level, [Key] object lines, [Key] object miser_width, [Key] object pprint_dispatch, 
            [Key] object pretty, [Key] object radix, [Key] object readably, [Key] object right_margin, [Key] object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Prin1(object obj, [Optional] object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Princ(object obj, [Optional] object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Print(object obj, [Optional] object output_stream)
        {
            Console.WriteLine(obj);
            return obj;

        }

        [Builtin]
        public static void PPrint(object obj, [Optional] object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("write-to-string")]
        public static object WriteToString(object obj, [Key] object array, [Key] object _base, [Key] object _case, [Key] object circle, [Key] object escape,
            [Key] object gensym, [Key] object length, [Key] object level, [Key] object lines, [Key] object miser_width, [Key] object pprint_dispatch,
            [Key] object pretty, [Key] object radix, [Key] object readably, [Key] object right_margin)
        {
            throw new NotImplementedException();
        }

        [Builtin("prin1-to-string")]
        public static object Prin1ToString(object obj)
        {
            throw new NotImplementedException();
        }

        [Builtin("princ-to-string")]
        public static object PrincToString(object obj)
        {
            throw new NotImplementedException();
        }

        [Builtin("print-not-readable-object")]
        public static object PrintNotReadableObject(object condition)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Format(object destination, object control_string, [Rest] object args)
        {
            throw new NotImplementedException();
        }
    }
}

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol _Print_Array_;
        public static Symbol _Print_Base_;
        public static Symbol _Print_Radix_;
        public static Symbol _Print_Case_;
        public static Symbol _Print_Circle_;
        public static Symbol _Print_Escape_;
        public static Symbol _Print_Gensym_;
        public static Symbol _Print_Level_;
        public static Symbol _Print_Length_;
        public static Symbol _Print_Lines_;
        public static Symbol _Print_Miser_Width_;
        public static Symbol _Print_PPrint_Dispatch_;
        public static Symbol _Print_Pretty_;
        public static Symbol _Print_Readably_;
        public static Symbol _Print_Right_Margin_;
    }
}