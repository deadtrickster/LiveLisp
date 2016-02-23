using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Utils;
using LiveLisp.Core.Reader;
using LiveLisp.Core.BuiltIns.Characters;
using LiveLisp.Core.Types.Streams;
using System.Collections;
using LiveLisp.Core.BuiltIns.Numbers;

namespace LiveLisp.Core.Types
{
    public enum MarkerPosition
    {
        Start,
        End,
        Any
    }


    public partial class Readtable
    {
        public static Readtable Current
        {
            get { return (Readtable)DefinedSymbols._Readtable_.Value; }
            set { DefinedSymbols._Readtable_.Value = value; }
        }

        public Readtable()
        {
            WhiteSpaces = new List<char>();
            MacroTable = new Dictionary<char, LispFunction>();
            Termainals = new List<char>();
            InvalidChars = new List<char>();
            ReadtableCase = ReadtableCases.Upcase;
            CharNicknames = new LiveLisp.Core.Reader.Mirror<string, char>();
            DefinedSymbols._Read_Base_.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_ReadBase__PropertyChanged);
            
        }

        /// <summary>
        /// Lookup the character in whitespaces list
        /// </summary>
        /// <param name="ch">character for lookup</param>
        /// <returns></returns>
        public bool IsWhiteSpace(char ch)
        {
            return WhiteSpaces.Contains(ch);
        }

        /// <summary>
        /// Makes character ch a whitespace character.
        /// That is, removes any existed associations with ch in current readtable and add adds to white space list
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="leave_if_is_macro">if this parameter is true then if character ch is macro or macrodispatch
        /// character then method do nothing</param>
        public void MakeWhiteSpace(char ch, bool leave_if_is_macro)
        {
            if (WhiteSpaces.Contains(ch))
                return;

            if (leave_if_is_macro)
            {
                if (MacroTable.ContainsKey(ch))
                    return;
            }

            Termainals.Remove(ch);
            InvalidChars.Remove(ch);

            if (!leave_if_is_macro)
            {
                if (MacroTable.ContainsKey(ch))
                {
                    MacroTable.Remove(ch);
                    return;
                }
                else
                {
                }
            }
        }

        public void RemoveWhiteSpace(char ch)
        {
            WhiteSpaces.Remove(ch);
        }

        public bool IsTerminal(char ch)
        {
            return Termainals.Contains(ch);
        }

        public void MakeTerminal(char ch)
        {
            if (!Termainals.Contains(ch))
            {
                Termainals.Add(ch);
            }
        }

        public void RemoveTerminal(char ch)
        {
            Termainals.Remove(ch);
        }

        public bool IsInvalidChar(char ch)
        {
            return InvalidChars.Contains(ch);
        }

        public void MakeInvalidChar(char ch)
        {
            if (!InvalidChars.Contains(ch))
                InvalidChars.Add(ch);
        }

        public void RemoveInvalidChar(char ch)
        {
            InvalidChars.Remove(ch);
        }

        public void MakeDispatchMacroCharacter(char character)
        {
            if (MacroTable.ContainsKey(character))
                MacroTable[character] = new MacroDispatchLambda(character);
            else
                MacroTable.Add(character, new MacroDispatchLambda(character));
        }

        public bool IsReaderMacro(char ch)
        {
            return MacroTable.ContainsKey(ch);
        }

        public LispFunction GetReaderMacro(char ch)
        {
            return MacroTable[ch];
        }

        static Readtable _default;

        static public Readtable CreateDefault()
        {
            if (_default != null)
                return _default;

            Readtable readtable = new Readtable();

            readtable.NumBase = 10;
            readtable.RationalSplit = "/";

            readtable.SignleEscape = '\\';
            readtable.MultipleEscape = '|';


            /*LambdaMethod LeftParen = new LambdaMethod();
            LeftParen.Add(typeof(ReaderMacro).GetMethod("LeftParen"), 2);

            readtable.ReaderMacroChars.Add('(', LeftParen);

            MethodGroup DblQuote = new MethodGroup();
            DblQuote.Add(typeof(ReaderMacro).GetMethod("DblQuote"), 2);

            readtable.ReaderMacroChars.Add('"', DblQuote);

            MethodGroup BackQuote = new MethodGroup();
            BackQuote.Add(typeof(ReaderMacro).GetMethod("Backquote"), 2);

            readtable.ReaderMacroChars.Add('`', BackQuote);

            MethodGroup Comma = new MethodGroup();
            Comma.Add(typeof(ReaderMacro).GetMethod("Comma"), 2);

            readtable.ReaderMacroChars.Add(',', Comma);

            MethodGroup Quote = new MethodGroup();
            Quote.Add(typeof(ReaderMacro).GetMethod("Quote"), 2);

            readtable.ReaderMacroChars.Add('\'', Quote);

            MethodGroup SemiColon = new MethodGroup();
            SemiColon.Add(typeof(ReaderMacro).GetMethod("Semicolon"), 2);

            readtable.ReaderMacroChars.Add(';', SemiColon);*/

            readtable.WhiteSpaces = StandardWhitespace();
            readtable.Termainals = StandardTerminals();
            readtable.InvalidChars = StandardInvalids();

            readtable.CharNicknames.Add("space", ' ');
            readtable.CharNicknames.Add("return", '\r');
            readtable.CharNicknames.Add("newline", '\n');

            return _default = readtable;
        }

        /// <summary>
        /// Return standart whitespace chars list.
        /// 9 10 11 12 13 32 133 160
        /// 5760 6158 8192 8193 8194
        /// 8195 8196 8197 8198 8199
        /// 8200 8201 8202 8232 8233
        /// 8239 8287 12288
        /// </summary>
        /// <returns></returns>
        static private List<char> StandardWhitespace()
        {
            List<char> ws = new List<char>();
            ws.Add((char)9);
            ws.Add((char)10);
            ws.Add((char)11);
            ws.Add((char)12);
            ws.Add((char)13);
            ws.Add((char)32);
            ws.Add((char)133);
            ws.Add((char)160);
            ws.Add((char)5760);
            ws.Add((char)6158);
            ws.Add((char)8192);
            ws.Add((char)8193);
            ws.Add((char)8194);
            ws.Add((char)8195);
            ws.Add((char)8196);
            ws.Add((char)8197);
            ws.Add((char)8198);
            ws.Add((char)8199);
            ws.Add((char)8200);
            ws.Add((char)8201);
            ws.Add((char)8202);
            ws.Add((char)8232);
            ws.Add((char)8233);
            ws.Add((char)8239);
            ws.Add((char)8287);
            ws.Add((char)12288);

            return ws;
        }

        static private List<char> StandardTerminals()
        {
            List<char> terms = new List<char>();
            terms.Add('(');
            terms.Add(')');
            terms.Add('{');
            terms.Add('}');
            terms.Add('"');
           // terms.Add('[');
            //terms.Add(']');

            terms.AddRange(StandardWhitespace());
            return terms;
        }



        private static List<char> StandardInvalids()
        {
            return new List<char>(new char[] { (char)8 }); // backspace
        }

        internal static Readtable CreateDefault(Dictionary<char, LispFunction> macroTable)
        {
            CreateDefault().MacroTable = macroTable;

            return CreateDefault();
        }

        internal void FillFrom(Readtable from)
        {
            this.CharNicknames = new Mirror<string, char>(from.CharNicknames);

            this.WhiteSpaces = new List<char>(from.WhiteSpaces);

            this.MacroTable = new Dictionary<char, LispFunction>(from.MacroTable);

            var macrodeispts =  new Dictionary<char, Dictionary<char, LispFunction>>();

            this.Termainals = new List<char>(from.Termainals);

            this.InvalidChars = new List<char>(from.InvalidChars);

            this.ReadtableCase = from.ReadtableCase;
        }

        internal char GetCharFromNickName(string nickname)
        {
            if (CharNicknames.FirstExists(nickname))
            {
                return CharNicknames[nickname];
            }

            throw new SimpleErrorException("Unkonown character nickname: " + nickname);
        }

        public bool IsSingleEscape(char ch)
        {
            return SignleEscape == ch;
        }

        public bool IsMultipleEscape(char ch)
        {
            return MultipleEscape == ch;
        }

        public string GetTerminalsAsString()
        {
            return new string(Termainals.ToArray());
        }
    }

    public class DefalutMacroDispatchFunction : DefaultBehavior
    {
        public DefalutMacroDispatchFunction(char character)
            : base(character.ToString())
        {

        }

        protected override object DoDefault()
        {
            throw new ReadErrorException("with " + _symbol_name + " macro character is associated default macro function");
        } 
    }

    public class MacroDispatchLambda : FullLispFunction
    {
        Dictionary<char, LispFunction> dispatchTable;
        char ch;
        public MacroDispatchLambda(char ch)
            : base("DispatchLambda$" + ch)
        {
            this.ch = ch;
            dispatchTable = new Dictionary<char, LispFunction>();
        }

        public void AddOrReplaceSubcharacter(char ch, LispFunction lambda)
        {
            if (dispatchTable.ContainsKey(ch))
                dispatchTable[ch] = lambda;
            else
                dispatchTable.Add(ch, lambda);
        }

        public bool TryGetSubcharacter(char ch, out LispFunction lambda)
        {
            if (dispatchTable.ContainsKey(ch))
            {
                lambda = dispatchTable[ch];
                return true;
            }

            lambda = null;
            return false;
        }

        public override object ValuesInvoke(object _stream, object ch)
        {
            ICharacterInputStream stream = _stream as ICharacterInputStream;
            if (stream == null)
                throw new SimpleTypeException("READ: input_stream is not a valid stream");


            bool number_supplied = false;
            int number = 0;

        Next:
            int peeked = stream.Peek();
            if (peeked == -1)
                throw new ReaderErrorException("Unexpected eof after the dispatch macro character");
            stream.Read();

            int digit = peeked - 48;

            if (0 <= digit && digit <= 9)
            {
                number_supplied = true;
                number = number * 10 + digit;
            }
            else
            {
                char _ch = (char)(char)peeked;
                if (!dispatchTable.ContainsKey(_ch))
                    throw new ReaderErrorException(String.Format("After #\\{0} is #\\{1} an undefined dispatch macro character", ch, _ch));

                object n;
                if (number_supplied)
                    n = number;
                else
                    n = DefinedSymbols.NIL;
                return dispatchTable[_ch].ValuesInvoke(stream, _ch, n);
            }
            goto Next;
        }
    }
}
