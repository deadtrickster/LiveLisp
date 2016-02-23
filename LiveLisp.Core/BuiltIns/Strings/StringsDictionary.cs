using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.BuiltIns.Conditions;
using LiveLisp.Core.BuiltIns.Numbers;
using LiveLisp.Core.BuiltIns.Characters;
using LiveLisp.Core.Types;
using System.Collections;
using LiveLisp.Core.CLOS.Standard_classes;

namespace LiveLisp.Core.BuiltIns.Strings
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class StringsDictionary
    {
        [Builtin("simple-string-p", Predicate=true)]
        public static object SimpleStringP(object obj)
        {
            if (!CLOS_String.Instance.Is(obj))
                return DefinedSymbols.NIL;

            return DefinedSymbols.T;
        }

        [Builtin]
        public static char Char(object str, object index)
        {
            if(!CLOS_String.Instance.Is(str))
                ConditionsDictionary.TypeError("CHAR: argument 1 is not a string (" + str + ")");

            dynamic instance = CLOS_String.Instance.The(str);

            if(!(index is int))
                ConditionsDictionary.TypeError("CHAR: argument 2 is not a integer (" + index + ")");

            int i = (int)index;

            if (i >= instance.Count)
                ConditionsDictionary.TypeError("CHAR: index " + i + " should be less than the length of the string");

            return instance[i];
        }
        
        [Builtin("SYSTEM::set-char", OverridePackage = true)]
        public unsafe static object set_Char(object str, object index, object new_character)
        {
            string s= str as string;

            if (s == null)
                ConditionsDictionary.TypeError("(SETF CHAR): argument 1 is a not a string (" + str + ")");


            if (!(index is int))
                ConditionsDictionary.TypeError("(SETF CHAR): argument 2 is not a integer (" + index + ")");

            int i = (int)index;

            if (i >= s.Length)
                ConditionsDictionary.TypeError("(SETF CHAR): index " + i + " should be less than the length of the string");


            if (!(new_character is char))
                ConditionsDictionary.TypeError("(SETF CHAR): argument 3 is not a character (" + new_character + ")");

            char ch = (char)new_character;

            fixed (char* p = s)
            {
                p[i] = ch;
            }

            return new_character;
        }

        [Builtin]
        public static object SChar(object str, object index)
        {
            string s = str as string;

            if (s == null)
                ConditionsDictionary.TypeError("CHAR: argument 1 is not a string (" + str + ")");

            int i;

            if (!(index is int))
                ConditionsDictionary.TypeError("CHAR: argument 2 is not a integer (" + index + ")");
            i = (int)index;

            if (i >= s.Length)
                ConditionsDictionary.TypeError("CHAR: index " + i + " should be less than the length of the string");

            return s[i];
        }

        [Builtin("SYSTEM::set-schar", OverridePackage = true)]
        public unsafe static object set_SChar(object str, object index, object new_character)
        {
            string s = str as string;

            if (s == null)
                ConditionsDictionary.TypeError("(SETF SCHAR): argument 1 is not a string (" + str + ")");

            int i;

            if (!(index is int))
                ConditionsDictionary.TypeError("(SETF SCHAR): argument 2 is not a integer (" + index + ")");
            i = (int)index;

            if (i >= s.Length)
                ConditionsDictionary.TypeError("(SETF SCHAR): index " + i + " should be less than the length of the string");


            if (!(new_character is char))
                ConditionsDictionary.TypeError("(SETF SCHAR): argument 3 is not a character (" + new_character + ")");

            char ch = (char)new_character;

            fixed (char* p = s)
            {
                p[i] = ch;
            }

            return new_character;
        }

        [Builtin]
        public static string String(object obj)
        {
            string str = obj as string;

            if (str != null)
                return str;

            Symbol sym = obj as Symbol;

            if (sym != null)
                return sym.Name;

            if (obj is char)
                return new string(new char[] { (char)obj });

            List<char> list_ch = obj as List<char>;

            if (list_ch != null)
                return new string(list_ch.ToArray());

            return obj.ToString();

        }

        /*
        [Builtin("string-upcase")]
        public static object StringUpcase(object str, [Key(DefaultValue = "0")] object start, [Key] object end)
        {
            string s = String(str);

            int starti = start as int;

            if (starti == null)
                ConditionsDictionary.TypeError("STRING-UPCASE: argument 2 is not a integer (" + start + ")");

            int _endi;
            if (end == DefinedSymbols.NIL)
                _endi = s.Length;
            else
            {
                int endi = end as int;
                if (endi == null)
                    ConditionsDictionary.TypeError("STRING-UPCASE: argument 3 is not a integer (" + end + ")");

                _endi = endi.Value;
            }

            var ret = new string(s.Length);
            _Upcase(s, ret, starti.Value, _endi);

            return ret;
        }

        private static void _Upcase(string from, string to, int start, int end)
        {
            int i = 0;
            for (; i < start; i++)
            {
                to[i] = from[i];
            }

            for (; i < end; i++)
            {
                to[i] = char.ToUpper(from[i]);
            }

            for (; i < from.Count; i++)
            {
                to[i] = from[i];
            }
        }

        [Builtin("string-downcase")]
        public static object StringDowncase(object str, [Key] object start, [Key] object end)
        {
            string s = String(str);

            int starti = start as int;

            if (starti == null)
                ConditionsDictionary.TypeError("STRING-DOWNCASE: argument 2 is not a integer (" + start + ")");

            int _endi;
            if (end == DefinedSymbols.NIL)
                _endi = s.Length;
            else
            {
                int endi = end as int;
                if (endi == null)
                    ConditionsDictionary.TypeError("STRING-DOWNCASE: argument 3 is not a integer (" + end + ")");

                _endi = endi.Value;
            }

            var ret = new string(s.Length);
            _Downcase(s, ret , starti.Value, _endi);

            return ret;
        }

        private static void _Downcase(string from, string to, int start, int end)
        {
            int i = 0;
            for (; i < start; i++)
            {
                to[i] = from[i];
            }

            for (; i < end; i++)
            {
                to[i] = char.ToLower(from[i]);
            }

            for (; i < from.Count; i++)
            {
                to[i] = from[i];
            }
        }

        [Builtin("string-capitalize")]
        public static object StringCapitalize(object str, [Key(DefaultValue="0")] object start, [Key] object end)
        {
            string s = String(str);

            int starti = start as int;

            if (starti == null)
                ConditionsDictionary.TypeError("STRING-CAPITALIZE: argument 2 is not a integer (" + start + ")");

            int _endi;
            if (end == DefinedSymbols.NIL)
                _endi = s.Length;
            else
            {
                int endi = end as int;
                if (endi == null)
                    ConditionsDictionary.TypeError("STRING-CAPITALIZE: argument 3 is not a integer (" + end + ")");

                _endi = endi.Value;
            }

            string ret = new string(s.Length);

            _Capitalize(s, ret, starti.Value, _endi);

            return ret;
        }

        private static void _Capitalize(string from, string to, int start, int end)
        {
            int i = 0;
            for (; i < start; i++)
            {
                to[i] = from[i];
            }

            bool capitalize = true; // for wirst 'word'

            for (; i < end; i++)
            {
                char ch = from[i];

                if (char.IsLetterOrDigit(ch))
                {
                    if (capitalize)
                    {
                        to[i] = char.ToUpper(ch);
                        capitalize = false;
                    }
                    else
                        to[i] = ch;
                }
                else
                {
                    capitalize = true;
                }
            }

            for (; i < from.Count; i++)
            {
                to[i] = from[i];
            }
        }

        [Builtin("nstring-upcase")]
        public static object NStringUpcase(object str, [Key(DefaultValue="0")] object start, [Key] object end)
        {
            string s = str as string;

            if (s == null)
                ConditionsDictionary.TypeError("NSTRING-UPCASE: argument 1 is not a string (" + str + ")");

            int starti = start as int;

            if (starti == null)
                ConditionsDictionary.TypeError("NSTRING-UPCASE: argument 2 is not a integer (" + start + ")");

            int _endi;
            if (end == DefinedSymbols.NIL)
                _endi = s.Length - 1;
            else
            {
                int endi = end as int;
                if (endi == null)
                    ConditionsDictionary.TypeError("NSTRING-UPCASE: argument 3 is not a integer (" + end + ")");

                _endi = endi.Value;
            }

            for (int i = starti.Value; i < _endi; i++)
            {
                s[i] = char.ToUpper(s[i]);
            }

            return s;
        }

        [Builtin("nstring-downcase")]
        public static object NStringDowncase(object str, [Key(DefaultValue="0")] object start, [Key] object end)
        {
            string s = str as string;

            if (s == null)
                ConditionsDictionary.TypeError("NSTRING-DOWNCASE: argument 1 is not a string (" + str + ")");

            int starti = start as int;

            if (starti == null)
                ConditionsDictionary.TypeError("NSTRING-DOWNCASE: argument 2 is not a integer (" + start + ")");

            int _endi;
            if (end == DefinedSymbols.NIL)
                _endi = s.Length;
            else
            {
                int endi = end as int;
                if (endi == null)
                    ConditionsDictionary.TypeError("NSTRING-DOWNCASE: argument 3 is not a integer (" + end + ")");

                _endi = endi.Value;
            }

            for (int i = starti.Value; i < _endi; i++)
            {
                s[i] = char.ToLower(s[i]);
            }

            return s;
        }

        [Builtin("nstring-capitalize")]
        public static object NStringCapitalize(object str, [Key] object start, [Key] object end)
        {
            string s = str as string;

            if (s == null)
                ConditionsDictionary.TypeError("NSTRING-CAPITALIZE: argument 1 is not a string (" + str + ")");

            int starti = start as int;

            if (starti == null)
                ConditionsDictionary.TypeError("NSTRING-CAPITALIZE: argument 2 is not a integer (" + start + ")");

            int _endi;
            if (end == DefinedSymbols.NIL)
                _endi = s.Length - 1;
            else
            {
                int endi = end as int;
                if (endi == null)
                    ConditionsDictionary.TypeError("NSTRING-CAPITALIZE: argument 3 is not a integer (" + end + ")");

                _endi = endi.Value;
            }

            bool capitalize = true; // for first 'word'

            for (int i = starti.Value; i < _endi; i++)
            {
                char ch = s[i];

                if (char.IsLetterOrDigit(ch))
                {
                    if (capitalize)
                    {
                        s[i] = char.ToUpper(ch);
                        capitalize = false;
                    }
                }
                else
                {
                    capitalize = true;
                }
            }

            return s;
        }

        [Builtin("string-trim")]
        public static object StringTrim(object character_bag, object str)
        {
            throw new NotImplementedException();
        }

        [Builtin("string-left-trim")]
        public static object StringLeftTrim(object character_bag, object str)
        {
            throw new NotImplementedException();
        }

        [Builtin("string-right-trim")]
        public static object StringRightTrim(object character_bag, object str)
        {
            throw new NotImplementedException();
        }

        [Builtin("string=", Predicate = true)]
        public static object StringEq(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string/=", Predicate = true)]
        public static object StringNEq(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string<", Predicate = true)]
        public static object StringL(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string>", Predicate = true)]
        public static object StringG(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string<=", Predicate = true)]
        public static object StringLE(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string>=", Predicate = true)]
        public static object StringGE(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string-equal", Predicate = true)]
        public static object StringEqual(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string-not-equal", Predicate = true)]
        public static object StringNotEqaul(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string-lessp", Predicate = true)]
        public static object StringLessp(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string-greaterp", Predicate = true)]
        public static object StringGreaterp(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string-not-greaterp", Predicate = true)]
        public static object StringNotGrearep(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin("string-not-lessp", Predicate = true)]
        public static object StringNotLessp(object str1, object str2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate=true)]
        public static object Stringp(object obj)
        {
            return obj is string ? DefinedSymbols.T : DefinedSymbols.NIL;
        }

        [Builtin("make-string")]
        public static object MakeString(object size, [Key("#\0")] object initital_element, [Key("'character")] object element_type)
        {
            int s = size as int;

            if (s == null)
                ConditionsDictionary.TypeError("MAKE-STRING: argument 1 is not a integer (" + size + ")");

            char ch = initital_element as char;

            if (ch == null)
                ConditionsDictionary.TypeError("MAKE-STRING: argument 2 is not a character (" + initital_element + ")");

            if (element_type != DefinedSymbols.Character)
                ConditionsDictionary.Error("MAKE-STRING: :ELEMENT-TYPE argument must be a subtype of CHARACTER, not " + element_type);

            char[] chars = new char[s.Value];

            if (ch.Value != 0)
            {
                for (int i = 0; i < s.Value; i++)
                {
                    chars[i] = ch.Value;
                }
            }

            return new string(chars);
        }*/
    }
}
