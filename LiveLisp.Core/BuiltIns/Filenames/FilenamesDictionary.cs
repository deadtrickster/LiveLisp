using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.BuiltIns.Filenames
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class FilenamesDictionary
    {
        [Builtin]
        public static object Pathname(object pathspec)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-pathname")]
        public static object MakePathname([Key] object host, [Key] object device, [Key] object directory, [Key] object name, [Key] object type, [Key] object version, [Key] object defaults, [Key] object _case)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Pathnamep(object obj)
        {
            throw new NotImplementedException();
        }

        [Builtin("pathname-host")]
        public static object PathnameHost(object pathname, [Key] object _case)
        {
            throw new NotImplementedException();
        }

        [Builtin("pathname-device")]
        public static object PathnameDevice(object pathname, [Key] object _case)
        {
            throw new NotImplementedException();
        }

        [Builtin("pathname-directory")]
        public static object PathnameDirectory(object pathname, [Key] object _case)
        {
            throw new NotImplementedException();
        }

        [Builtin("pathname-name")]
        public static object PathnameName(object pathname, [Key] object _case)
        {
            throw new NotImplementedException();
        }

        [Builtin("pathname-type")]
        public static object PathnameType(object pathname, [Key] object _case)
        {
            throw new NotImplementedException();
        }

        [Builtin("pathname-version")]
        public static object PathnameVersion(object pathname)
        {
            throw new NotImplementedException();
        }

        [Builtin("load-logical-pathname-translations", Predicate = true)]
        public static object LoadLogicalPathnameTranslations(object host)
        {
            throw new NotImplementedException();
        }

        [Builtin("logical-pathname-translations")]
        public static object LogicalPathnameTranslations(object host)
        {
            throw new NotImplementedException();
        }

        [Builtin("SYSTEM::set-logical-pathname-translations", OverridePackage=true)]
        public static object set_LogicalPathnameTranslations(object host, object new_translations)
        {
            throw new NotImplementedException();
        }

        [Builtin("logical-pathname")]
        public static object LogicalPathname(object pathspec)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Namestring(object pathname)
        {
            throw new NotImplementedException();
        }

        [Builtin("file-namestring")]
        public static object FileNamestring(object pathname)
        {
            throw new NotImplementedException();
        }

        [Builtin("directory-namestring")]
        public static object DirectoryNamestring(object pathname)
        {
            throw new NotImplementedException();
        }

        [Builtin("host-namestring")]
        public static object HostNamestring(object pathname)
        {
            throw new NotImplementedException();
        }

        [Builtin("enough-namestring")]
        public static object EnoughNamestring(object pathname, [Optional] object defaults)
        {
            throw new NotImplementedException();
        }

        [Builtin("parse-namestring", ValuesReturnPolitics=ValuesReturnPolitics.AlwaysNonZero)]
        public static object ParseNamestring(object thing, [Optional] object host, [Optional] object default_pathname, [Key(DefaultValue = "0")] object start, [Key] object end, [Key] object junk_allowed)
        {
            throw new NotImplementedException();
        }

        [Builtin("wild-pathname-p")]
        public static object WildPathnameP(object pathname, [Optional] object field_key)
        {
            throw new NotImplementedException();
        }

        [Builtin("pathname-match-p")]
        public static object PathnameMatchP(object pathname, object wildcard)
        {
            throw new NotImplementedException();
        }

        [Builtin("translate-logical-pathname")]
        public static object TranslateLogicalPathname(object pathname)
        {
            throw new NotImplementedException();
        }

        [Builtin("translate-pathname")]
        public static object TranslatePathname(object source, object from_wildcard, object to_wildcard)
        {
            throw new NotImplementedException();
        }

        [Builtin("merge-pathnames")]
        public static object MergePathnames(object pathname, [Optional] object default_pathname, [Optional] object default_version)
        {
            throw new NotImplementedException();
        }
    }
}

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol _Default_Pathname_Defaults_;
    }
}