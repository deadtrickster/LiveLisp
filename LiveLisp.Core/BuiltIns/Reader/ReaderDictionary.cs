using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Types;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types.Streams;
using System.IO;
using LiveLisp.Core.Reader;

namespace LiveLisp.Core.BuiltIns.Reader
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class ReaderDictionary
    {
        private static Readtable GetReadtableFromDesignator(string function_name, string paramter_name, object readtabledes)
        {
            Readtable readtable = readtabledes == DefinedSymbols.NIL ? DefinedSymbols.StandardReadtable.ValueAsReadtable : readtabledes as Readtable;

            if (readtable == null)
            {

                throw new SimpleTypeException(function_name + ": " + paramter_name + " value " + readtabledes + " is invalid readtable designator");
            }
            return readtable;
        }

        #region copy-readtable
        [Builtin("copy-readtable")]
        public static Readtable CopyReadtable([Optional("$common-lisp:*readtable*")] object from_readtable, [Optional] object to_readtable)
        {
            Readtable from = GetReadtableFromDesignator("copy-readtable", "from-readtable", from_readtable);

            Readtable to = to_readtable == DefinedSymbols.NIL ? new Readtable() : to_readtable as Readtable;

            if (to == null)
            {
                throw new SimpleTypeException("copy-readtable: to-readtable value {0} is invalid readtable designator", to_readtable);
            }

            to.FillFrom(from);

            return to;
        }
        #endregion

        #region make-dispatch-macro-character
        [Builtin("make-dispatch-macro-character")]
        public static Symbol make_dispatch_macro_character(object _character, [Optional("$nil")] object _nonTerminatingp, [Optional("$common-lisp:*readtable*")] object _readtable)
        {
            char character;

            try
            {
                character = (char)_character;
            }
            catch (InvalidCastException)
            {
                throw new SimpleTypeException("make-dispatch-macro-character: parameter 'char' is not a character (got {0}).", _character);
            }

            bool non_terminating_p = _nonTerminatingp != DefinedSymbols.NIL;

            Readtable readtable = GetReadtableFromDesignator("make-dispatch-macro-character", "readtable", _readtable);

            readtable.MakeDispatchMacroCharacter(character);

            if (!non_terminating_p)
            {
                readtable.MakeTerminal(character);
            }

            return DefinedSymbols.T;
        }
        #endregion

        #region read
       // [Builtin]
        public static object Read([Optional] object input_stream, [Optional] object eof_error_p, [Optional] object eof_value, [Optional] object recursive_p)
        {
            if (input_stream == DefinedSymbols.NIL)
            {
                input_stream = DefinedSymbols._Standard_Input_.Value;
            }
            else if (input_stream == DefinedSymbols.T)
            {
                input_stream = DefinedSymbols.Termainal_IO.Value;
            }

            ICharacterInputStream stream;
            if (input_stream is string)
                stream = new CharacterInputStream(new StringReader(input_stream as string));
            else
                stream = input_stream as ICharacterInputStream;// Contract.NotNull<TextReader>(protoStream, "Stream");

            if (stream == null)
                throw new SimpleTypeException("READ: input_stream is not a valid stream");

            return _Read(stream, eof_error_p != DefinedSymbols.NIL, eof_value, recursive_p != DefinedSymbols.NIL);
        }

        private static object _Read(ICharacterInputStream stream, bool eof_error_p, object eof_value, bool recursive_p)
        {
            int startindex = -1;
            int startline = -1;
            int startcolumn = -1;
            ContainerType? ctype = null ;
            try
            {
            Step1:
                int peeked = stream.Read();

                if (peeked == -1)
                {
                    return EndOfFile(eof_error_p, eof_value, recursive_p);
                }

                bool hasescape = false;
                object ret = UnboundValue.Unbound;
                while (peeked != -1)
                {
                    char x = (char)peeked;

                    if (Readtable.Current.IsWhiteSpace(x))
                    {
                        peeked = stream.Read();
                        continue;
                    }

                    if (Settings.ReadWithInfo)
                    {
                        startindex = stream.RawIndex;
                        startline = stream.CurrentLine;
                        startcolumn = stream.CurrentColumn;
                    }

                    if (Readtable.Current.IsInvalidChar(x))
                        throw new ReadErrorException("read: invalid character " + x);


                    if (Readtable.Current.IsReaderMacro(x))
                    {
                        var macro = Readtable.Current.GetReaderMacro(x);

                        var ret1 = macro.ValuesInvoke(stream, x);
                        var values = ret1 as MultipleValuesContainer;
                        if (values == null)
                        {
                            
                            ret = ret1;
                            goto Return;
                        }
                        else
                        {

                            if (values.Count == 0)
                            {
                                if (recursive_p)
                                {
                                    ret = DefinedSymbols.NIL;
                                    ctype = ContainerType.Comment;
                                    goto Return;
                                }
                                goto Step1;
                            }
                            else
                            {
                                ret = values.First;
                            }
                        }
                    }

                    StringBuilder token = new StringBuilder();

                    if (Readtable.Current.IsSingleEscape(x))
                    {
                        peeked = stream.Read();

                        if (peeked == -1)
                            throw new ReaderErrorException("Unexpected eof after the single escape character");

                        x = (char)peeked;
                        token.Append(x);
                        hasescape = true;
                        goto Step8;
                    }

                    if (Readtable.Current.IsMultipleEscape(x))
                    {
                        hasescape = true;
                        goto Step9;
                    }
                    x = RespectCase(x);
                    token.Append(x);
                Step8:

                    peeked = stream.Read();
                    if (peeked == -1)
                        goto Step10;

                    x = (char)peeked;

                    if (Readtable.Current.IsTerminal(x))
                    {
                        stream.UnreadChar(x);
                        goto Step10;
                    }

                    if (Readtable.Current.IsSingleEscape(x))
                    {
                        ProbeForEOF(stream, ref peeked, ref x);
                        token.Append(x);

                        goto Step8;
                    }

                    if (Readtable.Current.IsMultipleEscape(x))
                    {
                        goto Step9;
                    }

                    if (Readtable.Current.IsInvalidChar(x))
                        throw new ReadErrorException("read: invalid character " + x);

                    if (Readtable.Current.IsWhiteSpace(x))
                    {
                        goto Step10;
                    }

                    // my god after all that bullshit

                    x = RespectCase(x);

                    token.Append(x);
                    goto Step8;
                Step9:
                    ProbeForEOF(stream, ref peeked, ref x);

                    if (Readtable.Current.IsSingleEscape(x))
                    {
                        ProbeForEOF(stream, ref peeked, ref x);
                        token.Append(x);

                        goto Step9;
                    }

                    if (Readtable.Current.IsMultipleEscape(x))
                        goto Step8;

                    if (Readtable.Current.IsInvalidChar(x))
                        throw new ReadErrorException("read: invalid character " + x);

                    token.Append(x);
                    goto Step9;
                Step10:
                    if (hasescape)
                    {
                        ret = ParseTokenWithoutNumbers(token.ToString());
                        goto Return;
                    }
                    else
                    {
                        ret = ParseToken(token);
                        goto Return;
                    }
                }

            Return:

                if (ret == UnboundValue.Unbound)
                {
                    if (!recursive_p)
                        return DefinedSymbols.NIL;
                    throw new SimpleErrorException("read totally failed");
                }

                if (Settings.ReadWithInfo)
                {
                    int endindex = stream.RawIndex;
                    int endline = stream.CurrentLine;
                    int endcolumn = stream.CurrentColumn;
                    ReadSession.Add(new FormContainer(startindex, startline, startcolumn, endindex, endline, endcolumn, ret, ctype.HasValue?ctype.Value:ContainerType.Object));
                }

                return ret;
            }
            catch(ReaderErrorException ex)
            {
                if (Settings.ReadWithInfo)
                {
                    int endindex = stream.RawIndex;
                    int endline = stream.CurrentLine;
                    int endcolumn = stream.CurrentColumn;
                    ReadSession.Add(new SyntaxErrorContainer(startindex, startline, startcolumn, endindex, endline, endcolumn, ex, ex.Recoverable, stream.DocId));

                    if (!ex.Recoverable)
                        throw;
                    else
                    {
                        return ex;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        private static char RespectCase(char x)
        {
            switch (Readtable.Current.ReadtableCase)
            {
                case ReadtableCases.Upcase:
                    x = char.ToUpper(x);
                    break;
                case ReadtableCases.Downcase:
                    x = char.ToLower(x);
                    break;
                case ReadtableCases.Invert:
                    if (char.IsLower(x))
                        x = char.ToUpper(x);
                    else
                        x = char.ToLower(x);
                    break;
            }
            return x;
        }

        private static void ProbeForEOF(ICharacterInputStream stream, ref int peeked, ref char x)
        {
            peeked = stream.Read();

            if (peeked == -1)
                throw new ReaderErrorException("Unexpected eof after the single escape character");

            x = (char)peeked;
        }

        private static object ParseToken(StringBuilder token)
        {
            object ret;
            string literal = token.ToString();
            if (NumberParser.TryParse(literal, out ret))
            {
                return ret;
            }
            return ParseTokenWithoutNumbers(literal);
        }

        private static object ParseTokenWithoutNumbers(string literal)
        {
            string literalToUpper = literal.ToUpper();
            if (literalToUpper == Boolean.TrueString.ToUpper())
            {
                return Boxing.True;
            }
            else if (literalToUpper == Boolean.FalseString.ToUpper())
            {
                return Boxing.False;
            }
            else if (literalToUpper == "NIL")
                return DefinedSymbols.NIL;
            else if (literalToUpper == "T")
                return DefinedSymbols.T;
            else if (literalToUpper == "CLRNULL")
                return null;
            else
                return ParseSymbol(literal/*.ToLower()*/);
        }

        internal static Symbol ParseSymbol(string long_name)
        {
            StringBuilder packagename = new StringBuilder();
            StringBuilder visibility_str = new StringBuilder();
            StringBuilder symbol_name = new StringBuilder();

            foreach (var item in long_name)
            {
                switch (item)
                {
                    case ':':
                        if (visibility_str.Length == 2)
                            goto default;
                        else
                            visibility_str.Append(':');
                        break;
                    default:
                        if (visibility_str.Length > 0)
                            symbol_name.Append(item);
                        else
                            packagename.Append(item);
                        break;
                }
            }

            if (visibility_str.Length != 0)
            {
                if (packagename.Length == 0)
                {
                    return KeywordPackage.Instance.Intern(symbol_name.ToString());
                }
                else
                {
                    string pname = packagename.ToString();
                    if (!PackagesCollection.Instance.Contains(pname))
                    {
                        throw new ReaderErrorException(true, "Package {0} not found", pname);
                    }
                    Package package = PackagesCollection.Instance[pname];
                    string sname = symbol_name.ToString();
                    Symbol symbol;

                    if (visibility_str.Length == 1)
                    {
                        if (package.IsExternal(sname, out symbol))
                        {
                            return symbol;
                        }
                        else
                        {
                            throw new ReaderErrorException(true, "package " + pname + " has no external symbols with name " + sname);
                        }
                    }
                    else
                    {
                        if (package.IsInternal(sname, out symbol))
                        {
                            return symbol;
                        }
                        if (package.IsExternal(sname, out symbol))
                        {
                            return symbol;
                        }

                        return CreateNewSymbol(sname, package);
                    }
                }
            }
            else
            {
                symbol_name = packagename;
                Symbol sym = Package.LookupInPackages(symbol_name.ToString());
                if (sym == null)
                {
                    return CreateNewSymbol(symbol_name.ToString(), Package.Current);
                }
                else
                    return sym;
            }


        }

        private static Symbol CreateNewSymbol(string symbol_name, Package package)
        {
            if (Settings.NotInternSymbols)
            {
                Symbol s = new Symbol(symbol_name, -667);
                s.Package = package;
                return s;
            }
            else
            {
                return package.Intern(symbol_name.ToString());
            }
        }

        private static object EndOfFile(bool eof_error_p, object eof_value, bool recursive_p)
        {
            if (recursive_p)
            {
                throw new ReaderErrorException("read: EOF while recursive call");
            }

            if (eof_error_p)
            {
                throw new ReaderErrorException("read: EOF");
            }
            else
                return eof_value;
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
                return ParseSymbol(literal/*.ToLower()*/);
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

        [Builtin("read-preserving-whitespace")]
        public static object ReadPreservingWhitespace([Optional] object input_stream, [Optional] object eof_error_p, [Optional(1)] object eof_value, [Optional("$nil$")] object recursive_p)
        {
            throw new NotImplementedException();
        }

        #region READ-DELIMITED-LIST
        [Builtin("read-delimited-list")]
        public static object ReadDelimitedList(object ch, [Optional] object input_stream, [Optional] object recursive_p)
        {
            char character;
            try
            {
                character = (char)ch;
            }
            catch (InvalidCastException)
            {
                throw new SimpleTypeException("read-delimited-list: parameter 'char' is not a character (got {0}).", ch);
            }

            if (input_stream == DefinedSymbols.NIL)
            {
                input_stream = DefinedSymbols._Standard_Input_.Value;
            }
            else if (input_stream == DefinedSymbols.T)
            {
                input_stream = DefinedSymbols.Termainal_IO.Value;
            }

            ICharacterInputStream stream;
            if (input_stream is string)
                stream = new CharacterInputStream(new StringReader(input_stream as string));
            else
                stream = input_stream as ICharacterInputStream;// Contract.NotNull<TextReader>(protoStream, "Stream");

            if (stream == null)
                throw new SimpleTypeException("READ: input_stream is not a valid stream");

            return ReadDelimitedList(character, stream, recursive_p != DefinedSymbols.NIL);
        }


        // TODO: rewrite
        public static object ReadDelimitedList(char wanted, ICharacterInputStream stream, bool recursive_p)
        {
            int peeked = stream.Read();

            if (peeked == -1)
                throw new ReaderErrorException("read-delimited-list: unexpected eof");

            char ch = (char)peeked;
            if (ch == '.')
                new ReaderErrorException("Token . not allowed here");

            while (Readtable.Current.IsWhiteSpace(ch))
            {
                peeked = stream.Read();

                if (peeked == -1)
                    throw new ReaderErrorException("read-delimited-list: unexpected eof");

                ch = (char)peeked;
            }

            Cons list = new Cons(DefinedSymbols.NIL);
            bool dot = false;
            while (wanted != ch)
            {
                stream.UnreadChar(ch);
                object readed = _Read(stream, true, DefinedSymbols.NIL, recursive_p);

                if (readed != MultipleValuesContainer.Void) // e.g. comment
                {
                    object toadd;
                    var values = readed as MultipleValuesContainer;
                    if (values != null)
                    {
                        toadd = values.First;
                    }
                    else
                    {
                        toadd = readed;
                    }
                    if (dot)
                        list.Last.Cdr = toadd;
                    else
                        list.Append(toadd);
                }

                peeked = stream.Read();

                if (peeked == -1)
                    throw new ReaderErrorException("read-delimited-list: unexpected eof");

                ch = (char)peeked;

                while (Readtable.Current.IsWhiteSpace(ch))
                {
                    peeked = stream.Read();

                    if (peeked == -1)
                        throw new ReaderErrorException("read-delimited-list: unexpected eof");

                    ch = (char)peeked;
                }

                if (dot)
                    if (ch != wanted)
                        throw new ReaderErrorException("Illegal end of dotted list");
                    else
                    {
                        dot = false;
                        break;
                    }

                if (ch == '.')
                {
                    dot = true;
                    peeked = stream.Read();

                    if (peeked == -1)
                        throw new ReaderErrorException("read-delimited-list: unexpected eof after dot");
                    ch = (char)peeked;

                    while (Readtable.Current.IsWhiteSpace(ch))
                    {
                        peeked = stream.Read();

                        if (peeked == -1)
                            throw new ReaderErrorException("read-delimited-list: unexpected eof");

                        ch = (char)peeked;
                    }
                }
            }
            if (dot)
            {
                throw new ReaderErrorException("Illegal end of dotted list");
            }

            if (list.IsProperList && list.Count == 1)
                return DefinedSymbols.NIL;

            return list.Cdr;
        }
        #endregion

        //read-from-string string &optional eof-error-p eof-value &key start end preserve-whitespace
        [Builtin("read-from-string", ValuesReturnPolitics=ValuesReturnPolitics.AlwaysNonZero)]
        public static object ReadFromString(object str, [Optional] object eof_error_p, [Optional] object eof_value, [Key] object start, [Key] object end, [Key] object preserve_whitespace)
        {
            throw new NotImplementedException();
        }

        [Builtin("readtable-case")]
        public static object ReadtableCase(object readtable)
        {
            throw new NotImplementedException();
        }

        [Builtin("SYSTEM::set_ReadtabeCase", OverridePackage = true)]
        public static object set_ReadtableCase(object readtable, object mode)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object Readtablep(object obj)
        {
            return obj is Readtable ? DefinedSymbols.T : DefinedSymbols.NIL;
        }

        [Builtin("get-dispatch-macro-character")]
        public static LispFunction GetDispatchMacroCharacter(object disp_char, object sub_char, [Optional] object readtable)
        {
            throw new NotImplementedException();
        }

        [Builtin("set-dipatch-macro-character", VoidReturn = "t")]
        public static void SetDipatchMacroCharacter(object disp_char, object sub_char, object new_function, [Optional] object readtable)
        {
            throw new NotImplementedException();
        }

        [Builtin("get-macro-character", ValuesReturnPolitics=ValuesReturnPolitics.AlwaysNonZero)]
        public static LispFunction GetMacroCharacter(object ch, [Optional] object readtable)
        {
            throw new NotImplementedException();
        }

        [Builtin("set-macro-character", VoidReturn = "t")]
        public static void SetMacroCharacter(object ch,  object new_function, [Optional] object non_terminating_p, [Optional] object readtable)
        {
            throw new NotImplementedException();
        }

        [Builtin("set-syntax-from-char", VoidReturn = "t")]
        public static void SetDipatchMacroCharacter(object to_char, object from_char, object new_function, [Optional] object to_readtable, [Optional] object from_readtable)
        {
            throw new NotImplementedException();
        }

    }

    public enum ContainerType
    {
        Empty,
        Object,
        Error,
        ObjectReadedByMecro,
        ObjectReadedByMacroDispatch,
        Comment,
        FeatureCondition,
        CompilationCondition,
        Root
    }

    public class FormContainer
    {
        public int StartIndex;
        public int StartLine;
        public int StartColumn;

        public int EndIndex;
        public int EndLine;
        public int EndColumn;

        public object Object;

        public ContainerType Type;

        public FormContainer Parent;

        public FormContainer(int StartIndex, int StartLine, int StartColumn, int EndIndex, int EndLine, int EndColumn, object Object, ContainerType type)
        {
            this.StartIndex = StartIndex;
            this.StartLine = StartLine;
            this.StartColumn = StartColumn;
            this.EndIndex = EndIndex;
            this.EndLine = EndLine;
            this.EndColumn = EndColumn;
            this.Object = Object;
            this.Type = type;
        }

        public List<FormContainer> Childs = new List<FormContainer>();

        public override string ToString()
        {
            return Object.ToString();
        }

        public FormContainer FindChild(object child)
        {
            for (int i = 0; i < Childs.Count; i++)
            {
                if (Childs[i].Object == child)
                    return Childs[i];
            }

            throw new InvalidOperationException();
        }
    }

    public class SyntaxErrorContainer : FormContainer
    {
        public bool Restartable;
        public string BufferName;

        public SyntaxErrorContainer(int StartIndex, int StartLine, int StartColumn, int EndIndex, int EndLine, int EndColumn, object error, bool restartable, string bufferName)
            :base (StartIndex, StartLine, StartColumn, EndIndex, EndLine, EndColumn, error, ContainerType.Error)
        {
            Restartable = restartable;
            BufferName = bufferName;
        }

        public override string ToString()
        {
            return "ERROR: " + (Object as Exception).Message;
        }
    }


    public class EmptySyntaxContainer : FormContainer
    {
        public EmptySyntaxContainer()
            :base(0,0,0,0,0,0,null, ContainerType.Empty){}
    }


    /*
     * form := [sf1, sf2, ... , sfn]
     * form := atom
     * 
     * ---------------------------------------------------------------------------------------------
     * | sf1 | sf2 | ............... || ssf(n-1)1 | ... ssf(n-1)(p-1) | ssf(n-1)p | sf(n-1) || sfn |
     * ---------------------------------------------------------------------------------------------
     *           i2                        <                          i(n-1)p     >     i(n-1) >  i
     */
    public static class ReadSession
    {
        [ThreadStatic]
        private static List<FormContainer> Containers;

        public static FormContainer ConvertToTree()
        {
            if (Containers == null)
                throw new FormattedException("Not read session exists");

            if (Containers.Count == 0)
            {
                return new EmptySyntaxContainer();
            }

            FormContainer root = Containers[Containers.Count - 1];
            int index = Containers.Count - 2;


            for (; index >= 0; index--)
            {
                _Walk(root, ref index);
            }

            root.Childs.Reverse();
            StartNewSession();
            return root;
        }


        private static void _Walk(FormContainer root, ref int index)
        {
            while(index >= 0)
            {
                FormContainer new_root = Containers[index];
                if (new_root.StartIndex > root.StartIndex)
                {
                    root.Childs.Add(new_root);
                    new_root.Parent = root;
                    index--;
                    _Walk(new_root, ref index);
                }
                else
                {
                    root.Childs.Reverse();
                    return;
                }
            }
        }

        public static void StartNewSession()
        {
            if (Containers != null)
            {
                Containers.Clear();
            }
            else
            {
                Containers = new List<FormContainer>();
            }
        }

        public static void Add(FormContainer container)
        {
            Containers.Add(container);
        }

        public static bool Empty
        {
            get { return Containers == null || Containers.Count == 0; }
        }
    }

}
namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol _Read_Base_;
        public static Symbol _Read_Default_Float_Format_;
        public static Symbol _Read_Eval_;
        public static Symbol _Read_Supress_;
        public static Symbol _Readtable_;
    }
}