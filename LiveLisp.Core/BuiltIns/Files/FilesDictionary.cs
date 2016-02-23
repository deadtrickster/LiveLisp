using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;

namespace LiveLisp.Core.BuiltIns.Files
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class FilesDictionary
    {
        [Builtin]
        public static object Directory(object pathspec, [Key] object recoursive)
        {
            throw new NotImplementedException();
        }

        [Builtin("probe-file", Predicate = true)]
        public static object ProbeFile(object pathspec)
        {
            throw new NotImplementedException();
        }

        [Builtin("ensure-directories-exist", ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static MultipleValuesContainer EnsureDirectoriesExist(object pathspec, [Key] object verbose)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Truename(object filespec)
        {
            throw new NotImplementedException();
        }

        [Builtin("file-author")]
        public static object FileAuthor(object pathspec)
        {
            throw new NotImplementedException();
        }

        [Builtin("file-write-date")]
        public static object FileWriteDate(object pathspec)
        {
            throw new NotImplementedException();
        }

        [Builtin("rename-file", ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static object RenameFile(object filespec, object new_name)
        {
            throw new NotImplementedException();
        }

        [Builtin("delete-file", VoidReturn = "t")]
        public static void DeleteFile(object filespec)
        {
            throw new NotImplementedException();
        }

        [Builtin("file-error-pathname")]
        public static object FileErrorPathname(object condition)
        {
            throw new NotImplementedException();
        }
    }
}
