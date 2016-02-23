using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using LiveLisp.Core.Types.Streams;
using System.IO;

namespace LiveLisp.Core.BuiltIns.SystemConstruction
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class SystemConstructionDictionary
    {
        [Builtin("compile-file", ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer CompileFile(object input_file, [Key] object output_file, [Key] object verbose, [Key] object print, [Key] object external_format)
        {
            throw new NotImplementedException();
        }

        [Builtin("compile-file-pathname", AllowOtherKeys = true)]
        public static object CompileFilePathname(object input_file, [Key] object output_file)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Load(object filespec, [Key] object verbose, [Key] object print, [Key] object if_does_not_exist, [Key] object external_format)
        {
            if (filespec is string)
            {
                CharacterInputStream stream = new CharacterInputStream(new StreamReader(filespec as string));

                while (stream.Peek() != -1)
                {
                    object form = DefinedSymbols.Read.Invoke(stream);
                    DefinedSymbols.Eval.VoidInvoke(form);
                }

                return DefinedSymbols.T;
            }
            
            throw new NotImplementedException();
        }

        /*
         * provide module-name => implementation-dependent

require module-name &optional pathname-list => implementation-dependent

         */

        [Builtin]
        public static void Provide(object module_name)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static void Require(object module_name, [Optional] object pathname_list)
        {
            throw new NotImplementedException();
        }
    }
}

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol _Features_;
        public static Symbol _Compile_File_Pathname_;
        public static Symbol _Compile_File_Truename_;
        public static Symbol _Load_Pathname_;
        public static Symbol _Load_Truename_;
        public static Symbol _Compile_Print_;
        public static Symbol _Compile_Verbose_;
        public static Symbol _Load_Print_;
        public static Symbol _Load_Verbose_;
        public static Symbol _Modules_;
    }
}