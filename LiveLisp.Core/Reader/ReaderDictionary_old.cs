using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using System.IO;
using LiveLisp.Core.Types;
using LiveLisp.Core.Types.Streams;

namespace LiveLisp.Core.Reader
{
    [Builtin("")]
    public static class ReaderDictionary_old
    {
        

        #region read
        [Builtin]
        public static object Read([Optional] object input_stream, [Optional] object eof_error_p, [Optional(1)] object eof_value, [Optional("$nil")] object recursive_p)
        {
            if (input_stream == DefinedSymbols.NIL)
            {
                input_stream = DefinedSymbols.Standard_Input.Value;
            }
            else if (input_stream == DefinedSymbols.T)
            {
                input_stream = DefinedSymbols.Termainal_IO.Value;
            }

            CharacterInputStream stream;
            if (input_stream is string)
                stream = new CharacterInputStream(new StringReader(input_stream as string));
            else
                stream = input_stream as CharacterInputStream;// Contract.NotNull<TextReader>(protoStream, "Stream");

            if (stream == null)
                throw new SimpleTypeException("READ: input_stream is not a valid stream");

            return Read(stream, eof_error_p != DefinedSymbols.NIL, eof_value, recursive_p != DefinedSymbols.NIL);
        }

        private static object Read(CharacterInputStream stream, bool eof_error_p, object eof_value, bool recursive_p)
        {
            // check for whitespace
            // and only call appropriate reader macro function

            object ret = null;

            int startline = stream.CurrentLine;
            int startcol = stream.CurrentColumn;
            int startindex = stream.RawIndex;

            while (stream.Peek() != -1)
            {
                char ch = (char)stream.Read();
                if (ch != '\\')
                {
                    if (Readtable.Current.IsWhiteSpace(ch))
                    {
                        startline = stream.CurrentLine;
                        startcol = stream.CurrentColumn;
                        startindex = stream.RawIndex;
                        continue;
                    }
                    else if (Readtable.Current.IsReaderMacro(ch))
                    {
                        // call reader macro char
                        var fun = Readtable.Current.GetReaderMacro(ch);
                        ret = fun.Invoke(stream, ch);
                        break;
                    }
                    else
                    {
                        ret = ReadLiteral(stream, ch);
                        break;
                    }
                }
                else
                {
                    if (stream.Peek() == -1)
                    {
                        ret = Package.getSymbol("");
                    }
                    else
                    {
                        ret = ReadLiteral(stream, (char)stream.Read());
                    }
                }
            }

            SourceSpan span = new SourceSpan(startindex, startline, startcol, stream.RawIndex, stream.CurrentLine, stream.CurrentColumn);

            SpanCollection.Add(stream.DocId, ret, span);

            if (ret is CommentPlaceholder)
            {
                ret = Read(stream, eof_error_p, eof_value, recursive_p);
            }

            return ret;
        }

        /// <summary>
        /// Reads the literal (symbol or number).
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="ch">The ch.</param>
        /// <returns></returns>
        private static object ReadLiteral(CharacterInputStream stream, char ch)
        {
            string literal = ReadLiteralString(stream, ch);

            // determine is number or not
            object ret;

            if (NumberParser.TryParse(literal, out ret))
            {
                return ret;
            }
            string literalToUpper = literal.ToUpper();
            if (literalToUpper == Boolean.TrueString.ToUpper())
            {
                return true;
            }
            else if (literalToUpper == Boolean.FalseString.ToUpper())
            {
                return false;
            }
            else if (literalToUpper == "NIL")
                return DefinedSymbols.NIL;
            else if (literalToUpper == "T")
                return DefinedSymbols.T;
            else if (literalToUpper == "CLRNULL")
                return null;
            else
                return Package.getSymbol(literal/*.ToLower()*/);
        }

        public static string ReadLiteralString(CharacterInputStream stream, char ch)
        {
            // first accumulate literal
            StringBuilder acc = new StringBuilder();
            acc.Append(ch);

            while (stream.Peek() != -1 && !Readtable.Current.IsTerminal((char)stream.Peek()))
            {
                ch = (Char)stream.Read();

                acc.Append(ch);
            }

            string literal = acc.ToString();
            return literal;
        }
        #endregion

        #region READ-DELIMITED-LIST
        public static object ReadDelimitedList(object _character, [Optional("$common-lisp:*standard-input*")] object input_stream, [Optional(1)] object _recursive_p)
        {
            char character;
            try
            {
                character = (char)_character;
            }
            catch (InvalidCastException)
            {
                throw new SimpleTypeException("READ-DELIMITED-LIST: parameter 'char' is not a character (got {0}).", _character);
            }

            if (input_stream == DefinedSymbols.NIL)
            {
                input_stream = DefinedSymbols.Standard_Input.Value;
            }
            else if (input_stream == DefinedSymbols.T)
            {
                input_stream = DefinedSymbols.Termainal_IO.Value;
            }

            CharacterInputStream stream;
            if (input_stream is string)
                stream = new CharacterInputStream(new StringReader(input_stream as string));
            else
                stream = input_stream as CharacterInputStream;// Contract.NotNull<TextReader>(protoStream, "Stream");

            if (stream == null)
                throw new SimpleTypeException("READ-DELIMITED-LIST: input_stream is not a valid stream");

            bool recursive_p = _recursive_p != DefinedSymbols.NIL;

            throw new NotImplementedException();
        }
        #endregion
    }
}
