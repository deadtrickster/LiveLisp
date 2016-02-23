using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST.Expressions;
using LiveLisp.Core.Types;
using LiveLisp.Core.AST;
using System.Collections;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.Eval
{
    public partial class LispParser
    {
        private static void AssertArgsExistence(Cons rest, string opname)
        {
            if (rest.Count == 0)
            {
                throw new ReaderErrorException("{0}: too  few args for special operator " + opname + ": " + rest.Parent);
            }
        }

        private static void AssertArgsExistence(Cons rest, int count, string opname)
        {
            if (rest.Count < count)
            {
                throw new ReaderErrorException("too  few args for special operator " + opname + ": " + rest.Parent);
            }
        }

        private static void AssertMaxArgs(Cons rest, int maxcount, string opname)
        {
            if (rest.Count > maxcount)
                throw new ReaderErrorException("{0}: too many args {1}.", opname, rest);
        }

        private static void AssertnonDottedList(Cons rest, string opname)
        {
            if (!rest.IsProperList)
            {
                throw new SyntaxErrorException("dotted parameter list for special operator " + opname + ": " + rest.Parent);
            }
        }

        private static BlockExpression ParseBlock(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "block");

            AssertArgsExistence(rest, 1, "block");

            Symbol blockName = rest.Car as Symbol;
            if (blockName == null)
            {
                throw new SyntaxErrorException("block: block name not found. got " + rest.Car);
            }
            
            List<Expression> blockBody = BuildBody(rest.Cdr, context, "block");

            return new BlockExpression(blockName, blockBody, context);
        }

        private static CatchExpression ParseCatch(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "catch");

            AssertArgsExistence(rest, 1, "catch");

            Expression catchTag = ParseExpression(rest.Car, context);

            List<Expression> catchBody = BuildBody(rest.Cdr, context, "catch");

            return new CatchExpression(catchTag, catchBody, context);
        }

        private static EvalWhenExpression ParseEvalWhen(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "eval-whrn");

            AssertArgsExistence(rest, "eval-when");

            List<Symbol> conditions = new List<Symbol>();

            Cons condlist = rest.Car as Cons;

            if (condlist != null)
            {
                foreach (var item in condlist)
                {
                    Symbol condition = item as Symbol;
                    if (condition == null)
                    {
                        throw new SyntaxErrorException("eval-when: only symbols allowed in conditions list");
                    }
                    conditions.Add(condition);
                }
            }
            else
            {
                Symbol singleCondition = rest.Car as Symbol;

                if (singleCondition == null)
                {
                    throw new SyntaxErrorException("eval-when: nor condition list nor condition symbol not found");
                }

                conditions.Add(singleCondition);
            }

            return new EvalWhenExpression(conditions, BuildBody(rest.Cdr, context, "eval-when"), context);
        }

        private static FletExpression ParseFlet(Cons rest, ExpressionContext context)
        {
            Cons new_functions = rest.Car as Cons;

            if (new_functions == null)
            {
                throw new SyntaxErrorException("FLET: functions bindings is not a list: " + rest.Car);
            }

            List<LambdaFunctionDesignator> lambdas = new List<LambdaFunctionDesignator>();

            foreach (var func in new_functions)
            {
                Cons new_function = func as Cons;

                if (new_function == null)
                    throw new SyntaxErrorException("FLET: function declaration is not a list: " + func);

                Symbol func_name = new_function.Car as Symbol;

                if (func_name == null)
                    throw new SyntaxErrorException("FLET: function name is not a symbol: " + new_function.Car);

                if (new_function.Child == null)
                    throw new SyntaxErrorException("FLET: lambda declaration is not a list(or empty list): " + new_function.Cdr);

                LambdaFunctionDesignator lambda = ParseLambda(new_function.Child, context);

                lambda.Name = func_name;

                lambdas.Add(lambda);
            }

            List<Declaration> declarations = new List<Declaration>();
            List<Expression> forms = new List<Expression>();

            if (rest.Count > 1)
            {
                IEnumerator enumerator = rest.Child.GetEnumerator();

                List<Declaration> currentdecl;
                bool declexpected = true;
                while (enumerator.MoveNext())
                {
                    if (declexpected && TryParseDeclaration(enumerator.Current, out currentdecl))
                        declarations.AddRange(currentdecl);
                    else
                        forms.Add(LispParser.ParseExpression(enumerator.Current, context));
                }
            }

            return new FletExpression(lambdas, declarations, forms, context);
        }

        private static FunctionExpression ParseFunction(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "functional");
            AssertArgsExistence(rest, 1, "functional");
            LambdaCandidateRestart:
            Symbol functionSymbol = rest.Car as Symbol;

            FunctionNameDesignator funcname = null;

            if (functionSymbol != null && rest.Count == 1)
            {
                funcname = new SymbolFunctionDesignator(functionSymbol, context);
            }
            else
            {
                if (functionSymbol != null)
                    rest = rest.Child;

                Cons lambdaCandidate = rest.Car as Cons;
            
                if (lambdaCandidate == null)
                {
                    rest.Car = ConditionsDictionary.SyntaxError("function: " + rest.Car + " is invalid function name");
                    goto LambdaCandidateRestart;
                }
                if (lambdaCandidate.Car != DefinedSymbols.Lambda)
                {
                    rest.Car = ConditionsDictionary.SyntaxError("function: " + rest.Car + " is invalid function name");
                    goto LambdaCandidateRestart;
                }

                funcname = ParseLambda(lambdaCandidate.Child, context);

                if (functionSymbol != null)
                {
                    (funcname as LambdaFunctionDesignator).Name = functionSymbol;
                }
                else
                {
                    (funcname as LambdaFunctionDesignator).Name = new Symbol("UnnamedLambda");
                }
            }

            return new FunctionExpression(funcname, context);
        }

        private static GoExpression ParseGo(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "go");
            AssertArgsExistence(rest, "go");
            AssertMaxArgs(rest, 1, "go");

            Symbol tag = rest.Car as Symbol;

            if (tag != null)
            {
                return new GoExpression(tag, context);
            }
            int inttag;
            try
            {
                inttag = (int)rest.Car;
            }
            catch (InvalidCastException)
            {
                throw new ReaderErrorException("go: invalid tag {0}", rest.Car);
            }

            return new GoExpression(inttag, context);
        }

        private static IfExpression ParseIf(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "if");
            AssertArgsExistence(rest, 2, "if");
            AssertMaxArgs(rest, 3, "if");

            Expression testForm = LispParser.ParseExpression(rest.Car, context);
            Expression thenForm = LispParser.ParseExpression(rest.Child.Car, context);

            if (rest.Count == 3)
            {
                Expression elseForm = LispParser.ParseExpression(rest[2], context);
                return new IfExpression(testForm, thenForm, elseForm, context);
            }

            return new IfExpression(testForm, thenForm, context);
        }

        private static LabelsExpression ParseLabels(Cons rest, ExpressionContext context)
        {
            Cons new_functions = rest.Car as Cons;

            if (new_functions == null)
            {
                throw new SyntaxErrorException("LABELS: functions bindings is not a list: " + rest.Car);
            }

            List<LambdaFunctionDesignator> lambdas = new List<LambdaFunctionDesignator>();

            foreach (var func in new_functions)
            {
                Cons new_function = func as Cons;

                if (new_function == null)
                    throw new SyntaxErrorException("LABELS: function declaration is not a list: " + func);

                Symbol func_name = new_function.Car as Symbol;

                if (func_name == null)
                    throw new SyntaxErrorException("LABELS: function name is not a symbol: " + new_function.Car);

                if (new_function.Child == null)
                    throw new SyntaxErrorException("LABELS: lambda declaration is not a list(or empty list): " + new_function.Cdr);

                LambdaFunctionDesignator lambda = ParseLambda(new_function.Child, context);

                lambda.Name = func_name;

                lambdas.Add(lambda);
            }

            List<Declaration> declarations = new List<Declaration>();
            List<Expression> forms = new List<Expression>();

            if (rest.Count > 1)
            {
                IEnumerator enumerator = rest.Child.GetEnumerator();

                List<Declaration> currentdecl;
                bool declexpected = true;
                while (enumerator.MoveNext())
                {
                    if (declexpected && TryParseDeclaration(enumerator.Current, out currentdecl))
                        declarations.AddRange(currentdecl);
                    else
                        forms.Add(LispParser.ParseExpression(enumerator.Current, context));
                }
            }

            return new LabelsExpression(lambdas, declarations, forms, context);
        }

        private static LetLetStartBase ParseLet(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "let");
            AssertArgsExistence(rest, "let");

            List<SyntaxBinding> bindings;
            List<Declaration> declarations;
            List<Expression> forms;
            ParseLetLetStar(rest, context, out bindings, out declarations, out forms, "let");

            return new LetExpression(bindings, declarations, forms, context);
        }

        private static Expression ParseLetStar(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "let*");
            AssertArgsExistence(rest, "let*");

            List<SyntaxBinding> bindings;
            List<Declaration> declarations;
            List<Expression> forms;
            ParseLetLetStar(rest, context, out bindings, out declarations, out forms, "let*");
            return new LetStarExpression(bindings, declarations, forms, context);
        }

        private static LoadTimeValue ParseLoadTimeValue(Cons rest, ExpressionContext context)
        {
            if (rest.Count > 2)
                throw new SyntaxErrorException("LOAD-TIME-VALUE: too many args - " + rest);

            Expression form = ParseExpression(rest.Car, context);
            bool read_only = false;
            if (rest.Count == 2)
            {
                 if (rest.Child.Car != DefinedSymbols.NIL)
                    read_only = true;
            }

            return new LoadTimeValue(ParseExpression(rest.Car, context), read_only, context);
        }

        private static LocallyExpression ParseLocally(Cons rest, ExpressionContext context)
        {
            var declarations = new List<Declaration>();
            var forms = new List<Expression>();
            IEnumerator enumerator = rest.GetEnumerator();

            List<Declaration> currentdecl;
            bool declexpected = true;
            while (enumerator.MoveNext())
            {
                if (declexpected && TryParseDeclaration(enumerator.Current, out currentdecl))
                    declarations.AddRange(currentdecl);
                else
                {
                    forms.Add(LispParser.ParseExpression(enumerator.Current, context));
                    declexpected = false;
                }
            }

            return new LocallyExpression(declarations, forms, context);
        }

        private static MacroletExpression ParseMacrolet(Cons rest, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        private static MultipleValueCallExpression ParseMultipleValueCall(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "multiple-value-call");
            AssertArgsExistence(rest, 1, "multiple-value-call");

            Expression FunctionalForm = ParseExpression(rest.Car, context);

            List<Expression> forms = new List<Expression>();

            if (rest.Child != null)
            {
                foreach (var item in rest.Child)
                {
                    forms.Add(ParseExpression(item, context));
                }
            }

            return new MultipleValueCallExpression(FunctionalForm, forms, context);
        }

        private static MultipleValueProg1Expression ParseMultipleValueProg1(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "multiple-value-prog1");
            AssertArgsExistence(rest, 1, "multiple-value-prog1");

            Expression firstForm = ParseExpression(rest.Car, context);

            var forms = new List<Expression>();

            if (rest.Child != null)
            {
                foreach (var item in rest.Child)
                {
                    forms.Add(ParseExpression(item, context));
                }
            }

            return new MultipleValueProg1Expression(firstForm, forms, context);
        }

        private static PrognExpression ParseProgn(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "progn");

            var forms = new List<Expression>();
            if (rest.Count == 0)
                forms.Add(new ConstantExpression(DefinedSymbols.NIL, context));
            else
            {
                foreach (var item in rest)
                {
                    forms.Add(ParseExpression(item, context));
                }
            }

            return new PrognExpression(forms, context);
        }

        private static ProgvExpression ParseProgv(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "progv");
            AssertArgsExistence(rest, 2, "multiple-value-prog1");

            var varslist = ParseExpression(rest.Car, context);
            var valueslist = ParseExpression(rest.Child.Car, context);

            var forms = new List<Expression>();
            if (rest.Count == 2)
            {
                forms.Add(new ConstantExpression(DefinedSymbols.NIL, context));
            }
            else
            {
                foreach (var item in rest.Child.Child)
                {
                    forms.Add(ParseExpression(item, context));
                }
            }

            return new ProgvExpression(varslist, valueslist, forms, context);
        }

        private static ConstantExpression ParseQuote(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "quote");
            AssertArgsExistence(rest, 1, "quote");
            AssertMaxArgs(rest, 1, "quote");

            return new ConstantExpression(rest.Car, context);
        }

        private static ReturnFromExpression ParseReturnFrom(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "return-from");
            AssertArgsExistence(rest, 1, "return-from");
            AssertMaxArgs(rest, 2, "return-from");

            Symbol name = rest.Car as Symbol;

            if (name == null)
                throw new SyntaxErrorException("return-from: invlaid tag name " + rest.Car);

            Expression result;
            if (rest.Count == 2)
                result = ParseExpression(rest.Child.Car, context);
            else
                result = new ConstantExpression(DefinedSymbols.NIL, context);

            return new ReturnFromExpression(name, result, context);
        }

        private static SetqExpression ParseSetq(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "setq");

            if ((rest.Count % 2) != 0)
            {
                throw new SyntaxErrorException("SETQ: odd number of arguments: " + rest);
            }

            var assigns = new List<SyntaxBinding>();

            if (rest.Count == 0)
                return new SetqExpression(assigns, context);


            IEnumerator enumerator = rest.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol varname = enumerator.Current as Symbol;
                if (varname == null)
                    throw new SyntaxErrorException("SETQ: " + enumerator.Current + " is not a symbol");
                enumerator.MoveNext();
                Expression expression = ParseExpression(enumerator.Current, context);
                assigns.Add(new SyntaxBinding(varname, expression));
            }

            return new SetqExpression(assigns, context);
        }

        private static SymbolMacroletExpression ParseSymbolMacrolet(Cons rest, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        private static TagBodyExpression ParseTagbody(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "tagbody");
            List<Expression> nontaggedProlog = new List<Expression>();

            List<TaggedStatements> tagbodyforms = new List<TaggedStatements>();

            if (rest.Count == 0)
            {
                nontaggedProlog.Add(new ConstantExpression(DefinedSymbols.NIL, context));
                return new TagBodyExpression(nontaggedProlog, tagbodyforms, context);
            }

            IEnumerator enumerator = rest.GetEnumerator();
            object tag = null;

            // fill prolog
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                if (IsGoTag(current))
                {
                    tag = current;
                    break;
                }
                else
                {
                    nontaggedProlog.Add(ParseExpression(current, context));
                }
            }

        NewStms:
            TaggedStatements stms = new TaggedStatements(tag, new List<Expression>());

            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                if (IsGoTag(current))
                {
                    tag = current;
                    tagbodyforms.Add(stms);
                    goto NewStms;
                }
                else
                {
                    stms.Statements.Add(ParseExpression(current, context));
                }
            }
            if(tag!=null)
            tagbodyforms.Add(stms);
            return new TagBodyExpression(nontaggedProlog, tagbodyforms, context);
        }

        private static TheExpression ParseThe(Cons rest, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        private static ThrowExpression ParseThrow(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "throw");

            AssertArgsExistence(rest, 1, "throw");

            AssertMaxArgs(rest, 2, "throw");

            Expression catchTag = ParseExpression(rest.Car, context);
            Expression resultForm;
            if (rest.Count == 2)
                resultForm = ParseExpression(rest.Child.Car, context);
            else
                resultForm = new ConstantExpression(DefinedSymbols.NIL, context);

            return new ThrowExpression(catchTag, resultForm, context);
        }

        private static UnwindProtectExpression ParseUnwindProtect(Cons rest, ExpressionContext context)
        {
            AssertnonDottedList(rest, "unwind-protect");

            AssertArgsExistence(rest, 1, "unwind-protect");

            Expression protectedForm = ParseExpression(rest.Car, context);

            List<Expression> cleanupForms = BuildBody(rest.Cdr, context, "unwind-protect");

            return new UnwindProtectExpression(protectedForm, cleanupForms, context);
        }

        private static bool TryParseDeclaration(object decl, out List<Declaration> declarations)
        {
            Cons _declcons = decl as Cons;
            if (_declcons == null || _declcons.Car != DefinedSymbols.Declare)
            {
                declarations = null;
                return false;
            }
            if (_declcons.Count < 2)
            {
                throw new ReaderErrorException("empty declaration");
            }
            DispatchDeclarations(_declcons.Child, out declarations);
            return true;
        }

        public static void DispatchDeclarations(Cons _declars, out List<Declaration> declarations)
        {
            declarations = new List<Declaration>();

            foreach (var _decl in _declars)
            {
                Cons declspec = _decl as Cons;

                if (declspec == null || declspec.Count < 2)
                {
                    throw new SyntaxErrorException("invalid declaration specifier {0}", _decl);
                }

                var decl = ParseDeclaration(declspec);
                if (decl != null)
                    declarations.Add(decl);

                // ignore all remain
                /*else if (car == DefinedSymbols.Type)
                {
                    declarations.Add(ParseTypeDeclarations(declspec.Child));
                    return;
                }
                else if (car == DefinedSymbols.Inline)
                {
                    declarations.Add(ParseInlineDeclarations(declspec.Child));
                    return;
                }
                else if (car == DefinedSymbols.Ignorable)
                {
                    declarations.Add(ParseIgnorableDeclarations(declspec.Child));
                    return;
                }
                else if (car == DefinedSymbols.Notinline)
                {
                    declarations.Add(arseNotinlineDeclarations(declspec.Child));
                    return;
                }
                else if (car == DefinedSymbols.Optimize) // i.e. control type checking
                {
                    declarations.Add(ParseOptimizeDeclarations(declspec.Child));
                    return;
                }
                else if (car == DefinedSymbols.Ftype)
                {
                    declarations.Add(ParseFtypeDeclarations(declspec.Child));
                    return;
                }
                else if (car == DefinedSymbols.DynamicExtent)
                {
                    declarations.Add(ParseDynamicExtentDeclarations(declspec.Child));
                }
                else
                {
                    declarations.Add(ParseTypeDeclarations(declspec.Child));
                }*/

            }
        }

        public static Declaration ParseDeclaration(Cons declspec)
        {
            if (!declspec.IsProperList)
                ConditionsDictionary.TypeError(declspec + " is not a proper list. It's invalid declaration.");

            Symbol car = declspec.Car as Symbol;

            if (car == null)
                ConditionsDictionary.TypeError(declspec.Car + " is not a symbol. It's invalid declaration identifier.");

            DeclarationParser parser;

            if(Declaration.IsSupportedDeclaration(car, out parser))
                return parser(declspec.Child);

            if (Declaration.IsForeignDeclaration(car))
                return new ForeignDeclarationStub(declspec);

            ConditionsDictionary.Warn("Declaration specifier {0} is unknown. Type decl interpretation used.", car);
                return ParseTypeDeclaration(declspec.Child);
        }

        

          private static Declaration ParseTypeDeclaration(Cons cons)
          {
              return new ForeignDeclarationStub(cons);
          }

    /*    private static Declaration ParseAttributeDeclarations(Cons cons)
        {
            throw new NotImplementedException();
        }

        private static Declaration ParseIgnoreDeclarations(Cons cons)
        {
            throw new NotImplementedException();
        }

        private static Declaration ParseClrTypeDeclarations(Cons cons)
        {
            throw new NotImplementedException();
        }

          private static Declaration ParseIgnoreDeclarations(Cons cons)
          {
              return new ForeignDeclarationStub(cons)
          }

          private static Declaration ParseInlineDeclarations(Cons cons)
          {
              throw new NotImplementedException();
          }

          private static Declaration ParseIgnorableDeclarations(Cons cons)
          {
              throw new NotImplementedException();
          }

          private static Declaration ParseNotinlineDeclarations(Cons cons)
          {
              throw new NotImplementedException();
          }

          private static Declaration ParseOptimizeDeclarations(Cons cons)
          {
              throw new NotImplementedException();
          }

          private static Declaration ParseFtypeDeclarations(Cons cons)
          {
              throw new NotImplementedException();
          }

          private static Declaration ParseDynamicExtentDeclarations(Cons cons)
          {
              throw new NotImplementedException();
          }*/

        private static void ParseLetLetStar(Cons rest, ExpressionContext context, out List<SyntaxBinding> bindings, out List<Declaration> declarations, out List<Expression> forms, string opname)
        {
            Cons bindingslist;
            if (rest.Car == DefinedSymbols.NIL)
            {
                bindingslist = new SurrogateNilCons();
            }
            else
            {
                bindingslist = rest.Car as Cons;

                if (bindingslist == null)
                {
                    throw new ReaderErrorException(opname + ": bindings list not found " + rest);
                }
            }

            bindings = new List<SyntaxBinding>();

            foreach (var item in bindingslist)
            {
                Cons bindpair = item as Cons;
                if (bindpair != null)
                {
                    bindings.Add(BindingFromCons(bindpair, opname, context));
                    continue;
                }

                Symbol name = item as Symbol;

                if (name != null)
                {
                    bindings.Add(new SyntaxBinding(name, new ConstantExpression(DefinedSymbols.NIL, context)));
                    continue;
                }

                throw new ReaderErrorException(opname + ": invalid variable name " + item);
            }

            declarations = new List<Declaration>();
            forms = new List<Expression>();

            if (rest.Count > 1)
            {
                IEnumerator enumerator = rest.Child.GetEnumerator();

                List<Declaration> currentdecl;
                bool declexpected = true;
                while (enumerator.MoveNext())
                {
                    if (declexpected && TryParseDeclaration(enumerator.Current, out currentdecl))
                        declarations.AddRange(currentdecl);
                    else
                    {
                        declexpected = false;
                        forms.Add(LispParser.ParseExpression(enumerator.Current, context));
                    }
                }
            }
        }

        private static SyntaxBinding BindingFromCons(Cons bindpair, string op_name, ExpressionContext context)
        {
            if (bindpair.Count < 1)
                throw new ReaderErrorException("{0}: too short binding pair {1}", op_name, bindpair);

            if (bindpair.Count > 2)
                throw new ReaderErrorException("{0}: too long binding pair {1}", op_name, bindpair);

            Symbol name = bindpair.Car as Symbol;

            if (name == null)
            {
                throw new ReaderErrorException("{0}: invalid variable name {1}", op_name, bindpair.Car);
            }

            if (bindpair.Count == 2)
            {

                Expression value = LispParser.ParseExpression(bindpair.Child.Car, context);
                return new SyntaxBinding(name, value);
            }

            return new SyntaxBinding(name, new ConstantExpression(DefinedSymbols.NIL, context));
        }

        private static bool IsGoTag(object obj)
        {
            Symbol tag = obj as Symbol;
            if (tag != null)
            {
                return true;
            }
            int inttag;
            try
            {
                inttag = (int)obj;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            return true;
        }

        private static LambdaFunctionDesignator ParseLambda(Cons rest, ExpressionContext context)
        {
            int state = 0;
            KeywordSymbol lambdaType = null;
            Symbol lambdaName = null;
            LambdaList lambdaList = null;
            List<Declaration> declarations = new List<Declaration>();
            List<Expression> forms = new List<Expression>();
            foreach (var item in rest)
            {
                switch (state)
                {
                    case 0: // lambda list type designator
                        lambdaType = item as KeywordSymbol;
                        if (lambdaType != null)
                        {
                            if (lambdaType != DefinedSymbols.Ordinary && lambdaType != DefinedSymbols.Macros)
                            {
                                throw new SyntaxErrorException("lambda: unknow lambda list type " + lambdaType);
                            }

                            state = 1;
                        }
                        else
                        {
                            lambdaType = DefinedSymbols.Ordinary;
                            goto case 1;
                        }
                        break;
                    case 1:
                        if (item == DefinedSymbols.NIL)
                        {
                            lambdaList = OrdinaryLambdaList.FromNil();
                        }
                        else
                        {
                            Cons _llist = item as Cons;
                            if (_llist == null)
                            {
                                throw new SyntaxErrorException("lambda: lambda list not found, got " + item);
                            }

                            
                            lambdaList = OrdinaryLambdaList.CreateFromList(_llist);

                        }


                        state = 2;
                        break;
                    case 2:
                        List<Declaration> decls;
                        if (TryParseDeclaration(item, out decls))
                        {
                            declarations.AddRange(decls);
                        }
                        else
                        {
                            state = 3;
                            goto case 3;
                        }
                        break;
                    case 3:
                        forms.Add(ParseExpression(item, context));
                        break;
                }
            }

            if (state < 2)
            {
                throw new SyntaxErrorException("lambda: lambda list not found");
            }

            return  new LambdaFunctionDesignator(lambdaType, lambdaName, lambdaList, declarations, forms, context);
        }
    }
}
