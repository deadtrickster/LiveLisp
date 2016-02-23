using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
//using LiveLisp.Core.BuiltIns.Strings;
using LiveLisp.Core.BuiltIns.DataAndControlFlow;
using LiveLisp.Core.BuiltIns.Conditions;
using LiveLisp.Core.BuiltIns.Numbers;
using LiveLisp.Core.Utils;

namespace LiveLisp.Core.BuiltIns.Characters
{

    [BuiltinsContainer("COMMON-LISP")]
    public static class CharactersDictionary
    {
        [Builtin("char=", Predicate = true)]
        public static object CharE(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char/=", Predicate = true)]
        public static object CharNE(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char<", Predicate = true)]
        public static object CharL(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char>", Predicate = true)]
        public static object CharG(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char<=", Predicate = true)]
        public static object CharLE(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char>=", Predicate = true)]
        public static object CharGE(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char-equal", Predicate = true)]
        public static object CharEqual(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char-not-equal", Predicate = true)]
        public static object CharNotequal(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char-lessp", Predicate = true)]
        public static object CharLessp(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char-greaterp", Predicate = true)]
        public static object Chargreaterp(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char-not-greaterp", Predicate = true)]
        public static object CharNotGreaterP(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin("char-not-lessp", Predicate = true)]
        public static object CharNotLessp(params object[] characters)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Character(object character_designator)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Characterp(object obj)
        {
            return (obj is char) ? DefinedSymbols.T : DefinedSymbols.NIL;
        }
        /*
                [Builtin("alpha-char-p", Predicate = true)]
                public static object AlphaCharP(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("ALPHA-CHAR-P: argument 1 is not a character (" + character + ")");

                    return ch.IsAlpha;
                }

                [Builtin(Predicate = true)]
                public static object Alphanumericp(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("ALPHANUMERIC: argument 1 is not a character (" + character + ")");

                    return ch.IsAlphanumeric;
                }

                [Builtin("digit-char")]
                public static object DigitChar(object weight, [Optional(10)] object radix)
                {
                    LispInteger w = weight as LispInteger;

                    if (w == null)
                        ConditionsDictionary.TypeError("DIGIT-CHAR: argument 1 is not a character (" + weight + ")");

                    LispInteger rad = radix as LispInteger;
                    if (rad == null)
                        ConditionsDictionary.TypeError("DIGIT-CHAR: argument 2 is not a integer (" + radix + ")");

                    if (rad.Value < 2 || rad.Value > 36)
                        ConditionsDictionary.TypeError("DIGIT-CHAR: the radix must be an integer between 2 and 36, not " + radix);

                    return char.FromWeight(w, rad);
                }

                [Builtin("digit-char-p")]
                public static object DigitCharP(object character, [Optional(10)] object radix)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("DIGIT-CHAR-P: argument 1 is not a character (" + character + ")");

                    LispInteger rad = radix as LispInteger;
                    if (rad == null)
                        ConditionsDictionary.TypeError("DIGIT-CHAR-P: argument 2 is not a integer (" + radix + ")");

                    if (rad.Value < 2 || rad.Value > 36)
                        ConditionsDictionary.TypeError("DIGIT-CHAR-P: the radix must be an integer between 2 and 36, not " + radix);

                    return ch.ToUpper.GetWeight(rad);
                }

                [Builtin("graphic-char-p", Predicate = true)]
                public static object GraphicCharP(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("GRAPHIC-CHAR-P: argument 1 is not a character (" + character + ")");

                    return ch.IsGraphic;
                }

                [Builtin("standard-char-p", Predicate = true)]
                public static object StandardCharP(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("STANDARD-CHAR-P: argument 1 is not a character (" + character + ")");

                    return ch.IsStandard;
                }

                [Builtin("char-upcase")]
                public static object CharUpcase(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("CHAR-UPCASE: argument 1 is not a character (" + character + ")");

                    return ch.ToUpper();
                }

                [Builtin("char-downcase")]
                public static object CharDowncase(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("CHAR-DOWNCASE: argument 1 is not a character (" + character + ")");

                    return ch.ToLower;
                }

                [Builtin("upper-case-p", Predicate = true)]
                public static object UpperCaseP(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("UPPER-CASE-P: argument 1 is not a character (" + character + ")");

                    return ch.IsUpCase;
                }

                [Builtin("lower-case-p", Predicate = true)]
                public static object LowerCaseP(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("LOWER-CASE-P: argument 1 is not a character (" + character + ")");

                    return ch.IsLowerCase;
                }

                [Builtin("both-case-p", Predicate = true)]
                public static object BothCaseP(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("BOTH-CASE-P: argument 1 is not a character (" + character + ")");

                    return ch.IsBothCase;
                }

                [Builtin("char-code")]
                public static object CharCode(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("CHAR-CODE: argument 1 is not a character (" + character + ")");

                    return ch.Code;
                }

                [Builtin("char-int")]
                public static object CharInt(object character)
                {
                    char ch = character as char;

                    if (ch == null)
                        ConditionsDictionary.TypeError("CHAR-INT: argument 1 is not a character (" + character + ")");

                    return ch.Code;
                }

                [Builtin("code-char")]
                public static object CodeChar(object code)
                {
                    int integer;

                    if (code is int)
                        integer = (int)code;
                    else
                        return DefinedSymbols.NIL;

                    if (integer< 0 || integer > char.MaxValue)
                    {
                        return DefinedSymbols.NIL;
                    }

                    return (char)integer;
                }

                [Builtin("char-name")]
                public static object CharName(object character)
                {
                    throw new NotImplementedException();
                }

                [Builtin("SYSTEM::set-char-name")]
                public static object set_CharName(object character, object name)
                {
                    throw new NotImplementedException();
                }

                [Builtin("name-char")]
                public static object NameChar(object name)
                {
                    throw new NotImplementedException();
                }
                delegate object OneArg(object arg);
                static Dictionary<Type, OneArg> characterConstructorsCache;
                static CharactersDictionary()
                {
                    characterConstructorsCache = new Dictionary<Type, OneArg>(5);
                    characterConstructorsCache.Add(typeof(char), Fromcharacter);
                    characterConstructorsCache.Add(typeof(string), Fromstring);
                    characterConstructorsCache.Add(typeof(Symbol), FromSymbol);
                    characterConstructorsCache.Add(typeof(Char), FromBoxedChar);
                    characterConstructorsCache.Add(typeof(String), FromClrString);
                }

                #region Character constructors
                static object Fromcharacter(object character)
                {
                    return character;
                }

                static object Fromstring(object _lstring)
                {

                    throw new NotImplementedException();
                }
                static object FromSymbol(object symbol)
                {
                    return Fromstring((symbol as Symbol).Name);
                }

                static object FromBoxedChar(object ch)
                {
                    return (char)((char)ch);
                }

                static object FromClrString(object _str)
                {
                    throw new NotImplementedException();
                }
                #endregion
            }
             * 
             * */
    }
}

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol Char_Code_Limit;
        public static Symbol Character;

        static void InitCharactersDictionarySymbols()
        {
            Char_Code_Limit = cl.Intern("CHAR-CODE-LIMIT", true);
            Char_Code_Limit.IsConstant = true;
            Char_Code_Limit.Value = char.MaxValue;

            Character = cl.Intern("CHARACTER", true);
        }
    }
}