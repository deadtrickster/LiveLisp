using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;

namespace LiveLisp.Core.BuiltIns.Structures
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class StructuresDictionary
    {
        [Builtin("copy-structure")]
        public static object CopyStructure(object structure)
        {
            throw new NotImplementedException();
        }
    }
}
