namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Reflection;

    public class ClrFieldExpression : Expression
    {
    /*    private FieldAttributes Attributes;
        private Expression initFrom;
        private FieldSlot slot;*/

        public ClrFieldExpression(ExpressionContext context)
            : base(context)
        {
        }

      /*  public static ClrFieldExprerssion Build(Cons cons, int level, StaticScope scope, ClrTypeExpression parent)
        {
            string TypeName;
            string Name;
            Cons FieldDecl = cons.Cdr as Cons;
            if ((FieldDecl == null) || (FieldDecl.Length == 0))
            {
                throw new Exception("Body of field declaration not found");
            }
            ParserState currentState = ParserState.Start;
            FieldAttributes attributes = FieldAttributes.PrivateScope;
            string FirstSymbol = null;
            string SecondSymbol = null;
            Expression initForm = null;
            foreach (object item in FieldDecl)
            {
                switch (currentState)
                {
                    case ParserState.Start:
                        if (!(item is KeywordSymbol))
                        {
                            break;
                        }
                        attributes |= KeyworedToFieldAttribute(item as KeywordSymbol);
                        goto Label_00F1;

                    case ParserState.FirstSymbol:
                        if (item != DefinedSymbols.CInit)
                        {
                            goto Label_00C3;
                        }
                        currentState = ParserState.SecondSymol;
                        goto Label_00F1;

                    case ParserState.SecondSymol:
                        initForm = ExpressionBuilder.Build(item, level, true, scope.Parent, parent);
                        currentState = ParserState.End;
                        goto Label_00F1;

                    case ParserState.End:
                        throw new Exception("Too long field declaration");

                    default:
                        goto Label_00F1;
                }
                FirstSymbol = ClrClassMemberExpression.GetString(item);
                currentState = ParserState.FirstSymbol;
                goto Label_00F1;
            Label_00C3:
                SecondSymbol = ClrClassMemberExpression.GetString(item);
                currentState = ParserState.SecondSymol;
            Label_00F1: ;
            }
            Type RealType = null;
            if (SecondSymbol == null)
            {
                TypeName = string.Empty;
                Name = FirstSymbol;
            }
            else
            {
                TypeName = FirstSymbol;
                Name = SecondSymbol;
            }
            if ((initForm != null) && (initForm is ConstantExpression))
            {
                RealType = (initForm as ConstantExpression).ReturnType;
            }
            ClrFieldExprerssion ret = new ClrFieldExprerssion(level, scope, parent);
            ret.Attributes = attributes;
            ret.Name = Name;
            ret.Type = TypeName;
            ret.initFrom = initForm;
            ret.RealType = RealType;
            return ret;
        }

        public static FieldAttributes KeyworedToFieldAttribute(KeywordSymbol keyword)
        {
            if (keyword == DefinedSymbols.CPublic)
            {
                return FieldAttributes.Public;
            }
            if (keyword == DefinedSymbols.CFamily)
            {
                return FieldAttributes.Family;
            }
            if (keyword == DefinedSymbols.CPrivate)
            {
                return FieldAttributes.Private;
            }
            if (keyword != DefinedSymbols.CStatic)
            {
                throw new Exception(string.Format(":field : invalid FieldAtttribute {0}", keyword));
            }
            return FieldAttributes.Static;
        }

        public override void Visit(TypeBuilder typeBuilder)
        {
            if (this.initFrom != null)
            {
                if ((base.RealType == null) && (base.Type != string.Empty))
                {
                    base.Visit(typeBuilder);
                }
                (base.Parent as ClrTypeExpression).AddFieldInitalization(this);
            }
            if ((base.RealType == null) && (base.Type == string.Empty))
            {
                base.RealType = typeof(object);
            }
            else
            {
                base.Visit(typeBuilder);
            }
            FieldBuilder fb = typeBuilder.DefineField(base.Name, base.RealType, this.Attributes);
            FieldSlot fs = new FieldSlot();
            fs.IsInstance = (this.Attributes & FieldAttributes.Static) != FieldAttributes.Static;
            fs.Store = fb;
            fs.StoreType = base.RealType;
            fs.IsValueType = base.RealType.IsValueType;
            Symbol filedname = Package.Current.GetSymbol(base.Name);
            base.Scope.SetSlot(filedname, fs);
            base.Scope.AddToRefreshList(filedname);
        }

        private enum ParserState
        {
            Start,
            FirstSymbol,
            SecondSymol,
            InitForm,
            End
        }*/

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

