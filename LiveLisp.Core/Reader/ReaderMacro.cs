using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using LiveLisp.Core.Types;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Types.Streams;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.BuiltIns.Reader;
using LiveLisp.Core.BuiltIns.Characters;
using LiveLisp.Core.BuiltIns.Conses;

namespace LiveLisp.Core.Reader
{
    [ReaderMacro('%')]
    public static class ReaderMacro
    {
        [Obsolete("поменять тип выбрасываемого исключения")]
        [ReaderMacro('(')]
        public static object LeftParen(object protoStream, object ch)
        {
            return ReaderDictionary.ReadDelimitedList(')', protoStream, true);
        }

   //     [ReaderMacro('{')]
        public static object ClassMacro(object protoStream, object protoChar)
        {

            CharacterInputStream stream = protoStream as CharacterInputStream; // Contract.NotNull<TextReader>(protoStream, "Stream");

            //читать пока не встретим символ ( 
            //если достигли конца потока - ошибка

            List<object> listItems = new List<object>();

          //  listItems.Add(DefinedSymbols.Clr);

            int cpos = stream.Peek();

            int startline = stream.CurrentLine;
            int startcolumn = stream.CurrentColumn;


            while (cpos != -1)
            {
                if (char.IsWhiteSpace((char)cpos))
                {
                    if (char.IsWhiteSpace((char)stream.Peek()))
                        cpos = stream.Read();
                    else
                    {
                        cpos = stream.Peek();
                    }
                    continue;
                }

                if (cpos == '}') //')'
                {
                    stream.Read();
                    return new Cons(listItems);
                }

                object item = DefinedSymbols.Read.Invoke(stream);


                listItems.Add(item);

                cpos = stream.Peek();
            }


            throw new ReaderErrorException("LeftParenException" + " " + stream.CurrentLine + ":" + stream.CurrentColumn + " (started at " + startline + ":" + startcolumn + ")");
        }

        [ReaderMacro('"')]
        public static object DblQuote(object protoStream, object protoChar)
        {
            ICharacterInputStream stream = protoStream as ICharacterInputStream; // Contract.NotNull<TextReader>(protoStream, "Stream");

            // first accumulate literal
            StringBuilder acc = new StringBuilder();
            char ch;

           int startline = stream.CurrentLine;
            int startcolumn = stream.CurrentColumn;
            

            bool dbqfound = false;

            while (stream.Peek() != -1)
            {
                ch = (Char)stream.Read();

                if (ch == '"')
                {
                    dbqfound = true;
                    break;
                }

                if (ch == '\\')
                {
                    if ((char)stream.Peek() == '\\')
                    {
                        stream.Read();
                    }
                    else
                    {
                        if (stream.Peek() != -1)
                        {
                            char h = (char)stream.Peek();
                            if (h == 'n')
                            {
                                ch = '\n';
                                stream.Read();
                            }
                            else if (h == 'r')
                            {
                                ch = '\r';
                                stream.Read();
                            }
                            else if (h == '"')
                            {
                                ch = '"';
                                stream.Read();
                            }
                        }
                    }
                }

                acc.Append(ch);
            }

            if (!dbqfound)
                throw new ReaderErrorException("input stream ends within a string" + " " + stream.CurrentLine + ":" + stream.CurrentColumn + " (started at " + startline + ":" + startcolumn + ")");

      //      stream.Read();

            return acc.ToString();

        }

        [ReaderMacro(';')]
        public static object Semicolon(object protoStream, object protoChar)
        {
            ICharacterInputStream stream = protoStream as ICharacterInputStream; // Contract.NotNull<TextReader>(protoStream, "Stream");
            StringBuilder acc = new StringBuilder();
            char ch;
            int currentline = stream.CurrentLine;

            while (stream.Peek() != -1) 
            {
                ch = (Char)stream.Read();

                acc.Append(ch);

                if (System.Environment.NewLine[0] == ch)
                {
                    if (System.Environment.NewLine.Length == 1)
                    {
                        break;
                    }
                    else
                    {
                        if (System.Environment.NewLine[1] == (char)stream.Peek())
                        {
                            ch = (Char)stream.Read();

                            acc.Append(ch);

                            break;
                        }
                    }
                }
            }

            return MultipleValuesContainer.Void;
        }         
        
        [ReaderMacro('\'')]
        public static object Quote(object protoStream, object protoChar)
        {
            return new Cons(DefinedSymbols.Quote, new Cons(DefinedSymbols.Read.Invoke(protoStream)));
        }

        [ReaderMacro('`')]
        public static object _Backquote(object protoStream, object protoChar)
        {
            CharacterInputStream stream = protoStream as CharacterInputStream; // Contract.NotNull<TextReader>(protoStream, "Stream");
           // stream.Read(); // skip `

            return Bq_Completly_Process(DefinedSymbols.Read.Invoke(protoStream));
        }

        static Symbol Bq_quote = new Symbol("bq_quote");

       static Symbol comma_dot = new Symbol("splice");
       static Symbol comma_at = new Symbol("splice.");
       static Symbol comma = new Symbol("unquote");
       static Symbol Backquote = new Symbol("backquote");

       static Symbol bq_append = new Symbol("bq_append");
       static Symbol bq_list = new Symbol("bq_list");
       static Symbol bq_clobberable = new Symbol("bq_clobberable");
       static Symbol bq_nconc = new Symbol("bq_nconc");
       static Symbol bq_liststar = new Symbol("bq_list*");

        private static object Bq_Completly_Process(object p)
        {
            object raw_result = Bq_Process(p);
            return Bq_Remove_Tokens(raw_result);
        }

        private static object Bq_Process(object x)
        {
            Cons cons = x as Cons;
            if (cons == null)
              return  DefinedSymbols.List.Invoke(Bq_quote, x);

            if (cons.Car == Backquote)
            {
                return Bq_Process(Bq_Completly_Process(ConsesDictionary.Cadr(cons)));
            }

            if (cons.Car == comma)
                return ConsesDictionary.Cadr(cons);

            if (cons.Car == comma_at)
                throw new ReaderErrorException(",@{0} after `", ConsesDictionary.Cadr(cons));

            if (cons.Car == comma_dot)
                throw new ReaderErrorException(",.{0} after `", ConsesDictionary.Cadr(cons));


            object p = cons;
            object q = DefinedSymbols.NIL;

            while (p is Cons)
            {
                if (ConsesDictionary.Car(p) == comma)
                {
                    if ((ConsesDictionary.Cddr(p) != DefinedSymbols.NIL))
                    {
                        throw new ReaderErrorException("Malformerd ," + p);
                    }

                    return new Cons(bq_append, DefinedSymbols.Nreconc.Invoke(q, DefinedSymbols.List.Invoke(DefinedSymbols.Cadr.Invoke(p))));
                }

                if (ConsesDictionary.Car(p) == comma_at)
                {
                    throw new ReaderErrorException("Dotted ,@" + p);
                }

                if (ConsesDictionary.Car(p) == comma_dot)
                {
                    throw new ReaderErrorException("Dotted ,." + p);
                }
                object temp = p;
                p = ConsesDictionary.Cdr(p);
                q = new Cons(bracket(ConsesDictionary.Car(temp)), q);
            }
            return new Cons(bq_append, DefinedSymbols.Nreconc.Invoke(q, DefinedSymbols.List.Invoke(DefinedSymbols.List.Invoke(Bq_quote, p))));
           // return new Cons(bq_append, ConsesDictionary.NRECONC(q, new Cons(ConsesDictionary.LIST (new Cons(ConsesDictionary.LIST(new Cons(Backquote, new Cons(p))))))));
        }

        private static object bracket(object x)
        {
            Cons cons = x as Cons;

            if (cons == null)
                return DefinedSymbols.List.Invoke(bq_list, Bq_Process(x));

            if (cons.Car == comma)
                return DefinedSymbols.List.Invoke(bq_list, DefinedSymbols.Cadr.Invoke(x));

            if (cons.Car == comma_at)
                return DefinedSymbols.Cadr.Invoke(x);

            if (cons.Car == comma_dot)
                return DefinedSymbols.List.Invoke(bq_clobberable, DefinedSymbols.Cadr.Invoke(x));

            return DefinedSymbols.List.Invoke(bq_list, Bq_Process(x));
        }

        public static object Bq_Remove_Tokens(object x)
        {
            if (x == bq_list) return DefinedSymbols.List;
            if (x == bq_append) return DefinedSymbols.Append;
            if (x == bq_nconc) return DefinedSymbols.Nconc;
            if (x == bq_liststar) return DefinedSymbols.ListStar;
            if (x == Bq_quote) return DefinedSymbols.Quote;

            Cons cons = x as Cons;
            if (cons == null)
                return x;

            if (cons.Car == bq_clobberable)
                return Bq_Remove_Tokens(ConsesDictionary.Cadr(cons));

            if (cons.Car == bq_liststar && (ConsesDictionary.Cddr(cons) is Cons) && (ConsesDictionary.Cdddr(cons) == DefinedSymbols.NIL))
            {
                return new Cons(DefinedSymbols.Cons, map_bq_remove_tokens_to_tree(cons.Cdr));
            }

            return map_bq_remove_tokens_to_tree(x);
        }

        private static object map_bq_remove_tokens_to_tree(object x)
        {
            Cons cons = x as Cons;

            if (cons == null)
            {
                return Bq_Remove_Tokens(x);
            }
            else
            {
                object a = Bq_Remove_Tokens(cons.Car);
                object d = map_bq_remove_tokens_to_tree(cons.Cdr);

                if (a == cons.Car && d == cons.Cdr)
                    return cons;
                else
                    return new Cons(a, d);
            }
        }

        [ReaderMacro(',')]
        public static object Comma(object protoStream, object protoChar)
        {
            ICharacterInputStream stream = protoStream as ICharacterInputStream; // Contract.NotNull<TextReader>(protoStream, "Stream");

            if (stream.Peek() == -1)
            {
                throw new ReaderErrorException("unexpected end of stream");
            }

            if (stream.Peek() == '@')
            {
                stream.Read();
                return new Cons(comma_at, new Cons(DefinedSymbols.Read.Invoke(stream)));
            }

            if (stream.Peek() == '.')
            {
                stream.Read();
                return new Cons(comma_dot, new Cons(DefinedSymbols.Read.Invoke(stream)));
            }


            return new Cons(comma, new Cons(DefinedSymbols.Read.Invoke(stream)));

            //return Builtins.Read(stream);
        }

        [ReaderMacro('\'', Dispatch = '#')]
        public static object FucntionDispatch(object protoStream, object protoChar, object num)
        {
            return new Cons(DefinedSymbols.Function, new Cons(DefinedSymbols.Read.Invoke(protoStream)));
        }

        [ReaderMacro('\\', Dispatch = '#')]
        public static object CharDispatch(object protoStream, object protoChar, object num)
        {
            CharacterInputStream ss = protoStream as CharacterInputStream;

            string obj = ReadLiteral(ss);
            if (obj.Length == 1)
                return (char)obj[0];

            else
            {
                return Readtable.Current.GetCharFromNickName(obj.ToLower());
            }
        }

        private static string ReadLiteral(CharacterInputStream stream)
        {
            StringBuilder token = new StringBuilder();
            Next:
            int peeked = stream.Read();
            if (peeked == -1)
                throw new ReaderErrorException("Unexpected eof after the character dispatch");

            char x = (char)peeked;

            if (Readtable.Current.IsTerminal(x))
            {
                stream.UnreadChar(x);
                goto End;
            }

            if (Readtable.Current.IsSingleEscape(x))
            {
                throw new ReaderErrorException("char dispatch: escapes not allowed");
            }

            if (Readtable.Current.IsMultipleEscape(x))
            {
                throw new ReaderErrorException("char dispatch: escapes not allowed");
            }

            if (Readtable.Current.IsInvalidChar(x))
                throw new ReadErrorException("char dispatch: invalid character " + x);

            if (Readtable.Current.IsWhiteSpace(x))
            {
                goto End;
            }

            token.Append(x);
            goto Next;

        End:
            return token.ToString();

        }

       /* [ReaderMacro('.', Dispatch = '#')]
        public static object ReaderEval(object protoStream, object protoChar, object num)
        {
            if (DefinedSymbols._Read_Eval_ == DefinedSymbols.NIL)
                throw new ReaderErrorException("*READ-EVAL* = NIL does not allow the evaluation by reader");

            CharacterInputStream stream = protoStream as CharacterInputStream;


        }*/


        [ReaderMacro('|', Dispatch = '#')]
        public static object MultilineCommentDispatch(object protoStream, object protoChar, object num)
        {
            CharacterInputStream stream = protoStream as CharacterInputStream; // Contract.NotNull<TextReader>(protoStream, "Stream");
            StringBuilder acc = new StringBuilder();
            char ch;
            int startline = stream.CurrentLine;
            int startcolumn = stream.CurrentColumn;


            bool dbqfound = false;

            while (stream.Peek() != -1)
            {
                ch = (Char)stream.Read();

                

                if ('|' == ch)
                {
                   
                        if ('#' == (char)stream.Peek())
                        {
                            ch = (Char)stream.Read();

                            //acc.Append(ch);
                            dbqfound = true;
                            break;
                        }
                    
                }

                acc.Append(ch);
            }
            if (!dbqfound)
                throw new ReaderErrorException("input stream ends within a string" + " " + stream.CurrentLine + ":" + stream.CurrentColumn + " (started at " + startline + ":" + startcolumn + ")");
            CommentPlaceholder cp = new CommentPlaceholder(acc.ToString());
            return MultipleValuesContainer.Void;
        }

        [ReaderMacro('r', Dispatch = '#')]
        public static object RegexDispatch(object protoStream, object protoChar, object num)
        {
            CharacterInputStream stream = protoStream as CharacterInputStream;
            StringBuilder acc = new StringBuilder();
            char ch;

            int startline = stream.CurrentLine;
            int startcolumn = stream.CurrentColumn;

            bool dbqfound = false;

            while (stream.Peek() != -1)
            {
                ch = (Char)stream.Read();

                if (ch == '"')
                {
                    dbqfound = true;
                    break;
                }

                if (ch == '\\')
                {
                    if ((char)stream.Peek() == '"')
                    {
                        stream.Read();
                        ch = '"';
                    }
                }

                acc.Append(ch);
            }

            if (!dbqfound)
                throw new ReaderErrorException("input stream ends within a regular expression" + " " + stream.CurrentLine + ":" + stream.CurrentColumn + " (started at " + startline + ":" + startcolumn + ")");

            //      stream.Read();

            return new Regex(acc.ToString());
        }

        [ReaderMacro('$')]
        public static object DollarMacro(object protoStream, object ch)
        {
            return new Cons(DefinedSymbols.Read.Invoke(protoStream));
        }
        //    protected override object ExplicityTypedApply(TextReader reader, char ch)
        //    {
        //        if (reader.Peek() == '@')
        //        {
        //            reader.Read();
        //            return new Cons(Symbols.SPLICE, new Cons(Symbols.READ.Function.Apply(reader, reader.Read())));
        //        }
        //        return new Cons(Symbols.UNQUOTE, new Cons(Symbols.READ.Function.Apply(reader, reader.Read())));
        //    }
        //}
    }
}

