using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;

namespace LiveLisp.Core.Printer
{
    [BuiltinsContainer("COMMON-LISP")]
    public class PrinterDictionary
    {
        [Builtin]
        public static object Print(object obj)
        {
            Console.WriteLine(obj);
            return obj;
        }
    }
}
