namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using LiveLisp.Core.Compiler;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ClrMethodExpression : ClrMemberExpression
    {
      /*  private MethodAttributes Attributes;
        private Expression body;
        internal MethodInfo generatedMethod;
        private ClrClassMethodLambdaList lambdaList;
        private MethodParamSlot thisSlot;
        */
        private ClrMethodExpression(ExpressionContext context)
            : base(context)
        {
        }
        /*
        public static ClrMethodExpression Build(Cons cons, int level, StaticScope scope, ClrTypeExpression parent)
        {
            ClrMethodExpression ret = new ClrMethodExpression(level, new StaticScope(scope), parent);
            Cons FieldDecl = cons.Cdr as Cons;
            if ((FieldDecl == null) || (FieldDecl.Length == 0))
            {
                throw new Exception("Body of field declaration not found");
            }
            ParserState currentState = ParserState.Start;
            MethodAttributes attributes = MethodAttributes.ReuseSlot;
            string FirstSymbol = null;
            string SecondSymbol = null;
            ClrClassMethodLambdaList lambdaList = null;
            Expression bodyExpression = null;
            MethodParamSlot thisSlot = null;
            StaticScope bodyscope = new StaticScope(scope);
            foreach (object item in FieldDecl)
            {
                Cons paramsList;
                switch (currentState)
                {
                    case ParserState.Start:
                        if (!(item is KeywordSymbol))
                        {
                            break;
                        }
                        attributes |= KeywordToMethodAttribute(item as KeywordSymbol);
                        goto Label_0208;

                    case ParserState.FirstSymbol:
                        if ((attributes & MethodAttributes.Static) != MethodAttributes.Static)
                        {
                            Symbol thisSymbol = Package.Current.GetSymbol("this");
                            thisSlot = new MethodParamSlot(0);
                            bodyscope.SetSlot(thisSymbol, thisSlot);
                        }
                        if (item is Cons)
                        {
                            goto Label_012C;
                        }
                        SecondSymbol = ClrClassMemberExpression.GetString(item);
                        currentState = ParserState.SecondSymbol;
                        goto Label_0208;

                    case ParserState.SecondSymbol:
                        goto Label_012C;

                    case ParserState.Body:
                        bodyExpression = ExpressionBuilder.Build(item, level, false, bodyscope, ret);
                        currentState = ParserState.End;
                        goto Label_0208;

                    case ParserState.End:
                        throw new FormattedException("unknown part of method declaration {0}", item);

                    default:
                        goto Label_0208;
                }
                FirstSymbol = ClrClassMemberExpression.GetString(item);
                currentState = ParserState.FirstSymbol;
                goto Label_0208;
            Label_012C:
                paramsList = item as Cons;
                if (paramsList == null)
                {
                    throw new FormattedException("{0} is not a list of args");
                }
                lambdaList = new ClrClassMethodLambdaList(paramsList);
                int methodSlotNumber = 0;
                if ((attributes & MethodAttributes.Static) != MethodAttributes.Static)
                {
                    methodSlotNumber = 1;
                }
                foreach (LambdaListParameter param in lambdaList.Args)
                {
                    if (param.Kind == ParameterKind.Required)
                    {
                        bodyscope.SetSlot(param.Name, new MethodParamSlot(methodSlotNumber, false, typeof(object)));
                        methodSlotNumber++;
                    }
                }
                currentState = ParserState.Body;
            Label_0208: ;
            }
            ret.Attributes = attributes;
            if (SecondSymbol == null)
            {
                ret.Type = string.Empty;
                ret.Name = FirstSymbol;
            }
            else
            {
                ret.Type = FirstSymbol;
                ret.Name = SecondSymbol;
            }
            ret.lambdaList = lambdaList;
            ret.thisSlot = thisSlot;
            ret.body = bodyExpression;
            return ret;
        }

        private static MethodAttributes KeywordToMethodAttribute(KeywordSymbol keyword)
        {
            if (keyword == DefinedSymbols.CPublic)
            {
                return MethodAttributes.Public;
            }
            if (keyword == DefinedSymbols.CPrivate)
            {
                return MethodAttributes.Private;
            }
            if (keyword == DefinedSymbols.CFamily)
            {
                return MethodAttributes.Family;
            }
            if (keyword == DefinedSymbols.CStatic)
            {
                return MethodAttributes.Static;
            }
            if (keyword == DefinedSymbols.CVirtual)
            {
                return MethodAttributes.Virtual;
            }
            if (keyword != DefinedSymbols.CNew)
            {
                throw new FormattedException("Unknown method attribute {0}", keyword);
            }
            return MethodAttributes.VtableLayoutMask;
        }

        public override void Visit(TypeBuilder typeBuilder)
        {
            Symbol[] ArgsNames;
            CallingConventions cc;
            if ((base.RealType == null) && (base.Type == string.Empty))
            {
                base.RealType = typeof(object);
            }
            else
            {
                base.Visit(typeBuilder);
            }
            if (this.thisSlot != null)
            {
                this.thisSlot.StoreType = typeBuilder;
            }
            Type[] ArgsTypes = this.lambdaList.GetRealArgsTypes(out ArgsNames);
            if (this.thisSlot != null)
            {
                cc = CallingConventions.HasThis;
            }
            else
            {
                cc = CallingConventions.Standard;
            }
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(base.Name, this.Attributes, cc, base.RealType, ArgsTypes);
            LiveLisp.Interpreter.Interpreter emitter = new LiveLisp.Interpreter.Interpreter();
            ILGen ilgen = new ILGen(methodBuilder.GetILGenerator());
            emitter.EmitOnly(ilgen, this.body, false);
            if (base.RealType.IsValueType)
            {
                ilgen.Emit(OpCodes.Unbox_Any, base.RealType);
            }
            ilgen.EmitRet();
            this.generatedMethod = methodBuilder;
        }

        private enum ParserState
        {
            Start,
            FirstSymbol,
            SecondSymbol,
            LambdaList,
            Body,
            End
        }*/
    }
}

