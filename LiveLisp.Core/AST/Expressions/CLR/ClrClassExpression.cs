namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;
    using LiveLisp.Core.Types;
    using LiveLisp.Core.Reader;
    using System.Reflection;
    using LiveLisp.Core.Compiler;
    using System.Reflection.Emit;

#if !SILVERLIGHT
    [Serializable]
#endif
    public class ClrClassExpression : Expression
    {
     //   private ProtoClass protoClass;

        public ClrClassExpression(ExpressionContext context)
            : base(context)
        {
        }

     /*   public static Expression Build(Cons cons, int level, StaticScope scope, Expression parent)
        {
            Cons classDeclaration = cons.Cdr as Cons;
            if (classDeclaration == null)
            {
                throw new Exception("defclrclass: class body not found");
            }
            ClassDeclParserState nextEntry = ClassDeclParserState.Start;
            Queue<object> objs = new Queue<object>(classDeclaration.ToArray());
            ProtoClass protoClass = new ProtoClass();
            ClrClassExpression ret = new ClrClassExpression(level, new StaticScope(scope), parent);
            while (objs.Count != 0)
            {
                string attributeTypeName;
                Cons attributeDecl;
                string baseTypeName;
                object item = objs.Dequeue();
                switch (nextEntry)
                {
                    case ClassDeclParserState.Start:
                        if (!IsAttributeDecl(item))
                        {
                            break;
                        }
                        goto Label_00CC;

                    case ClassDeclParserState.Attribute:
                        goto Label_00CC;

                    case ClassDeclParserState.ClassName:
                        goto Label_0124;

                    case ClassDeclParserState.BaseType:
                        if (item != DefinedSymbols.KWBase)
                        {
                            goto Label_023F;
                        }
                        nextEntry = ClassDeclParserState.ParseBaseType;
                        goto Label_0259;

                    case ClassDeclParserState.ParseBaseType:
                        goto Label_0174;

                    case ClassDeclParserState.ClassBody:
                        goto Label_023F;

                    case ClassDeclParserState.End:
                        throw new Exception("defclrclass: invalid class declaration length");

                    default:
                        goto Label_0259;
                }
                if (!(item is KeywordSymbol))
                {
                    goto Label_0124;
                }
                protoClass.Attrs |= GetTypeAttributeFromKeyword(item as KeywordSymbol);
                goto Label_0259;
            Label_00CC:
                attributeDecl = item as Cons;
                if (!DynamicTypesHelper.GetStringFromDesignator(attributeDecl.Second, out attributeTypeName))
                {
                    throw new Exception(string.Format("defclrclass: object {0} is not valid custom attribute type name", attributeDecl.Second));
                }
                object[] attributeParams = ValidateAttributeParams(attributeDecl);
                protoClass.CustomAttrs.Add(new ProtoCustomAttribute(attributeTypeName, attributeParams));
                goto Label_0259;
            Label_0124:
                if (!DynamicTypesHelper.GetStringFromDesignator(item, out protoClass.Name))
                {
                    throw new Exception(string.Format("defclrclass: object {0} is not valid class name", item));
                }
                nextEntry = ClassDeclParserState.BaseType;
                goto Label_0259;
            Label_0174:
                if (item is Cons)
                {
                    Cons basetypes = item as Cons;
                    foreach (object bt in basetypes)
                    {
                        if (!DynamicTypesHelper.GetStringFromDesignator(bt, out baseTypeName))
                        {
                            throw new Exception(string.Format("defclrclass: object {0} is not valid base type name", item));
                        }
                        protoClass.BaseTypes.Add(baseTypeName);
                    }
                }
                else
                {
                    if (!DynamicTypesHelper.GetStringFromDesignator(item, out baseTypeName))
                    {
                        throw new Exception(string.Format("defclrclass: object {0} is not valid base type name", item));
                    }
                    protoClass.BaseTypes.Add(baseTypeName);
                }
                goto Label_0259;
            Label_023F:
                ret.FillClassBody(protoClass, item);
            Label_0259: ;
            }
            ret.protoClass = protoClass;
            return ret;
        }

        private void FillClassBody(ProtoClass protoClass, object classbody)
        {
            if ((classbody is Cons) && ((classbody as Cons).Car == DefinedSymbols.CField))
            {
                protoClass.AddField(ClrFieldExprerssion.Build(classbody as Cons, base.Level, base.Scope, this));
            }
            else if ((classbody is Cons) && ((classbody as Cons).Car == DefinedSymbols.CMethod))
            {
                protoClass.AddMethod(ClrMethodExpression.Build(classbody as Cons, base.Level, base.Scope, this));
            }
        }

        private static TypeAttributes GetTypeAttributeFromKeyword(KeywordSymbol keyword)
        {
            if (keyword == DefinedSymbols.CInternal)
            {
                return TypeAttributes.AutoLayout;
            }
            if (keyword == DefinedSymbols.CPublic)
            {
                return TypeAttributes.Public;
            }
            if (keyword != DefinedSymbols.CSealed)
            {
                throw new Exception("defclrclass: only internal, public and sealed type attributes allowed");
            }
            return TypeAttributes.Sealed;
        }

        private static bool IsAttributeDecl(object dec)
        {
            Cons candidate = dec as Cons;
            if ((candidate == null) || (candidate.Count < 2))
            {
                return false;
            }
            return (candidate.Car == DefinedSymbols.Attribute);
        }

        private static object[] ValidateAttributeParams(Cons attributeDecl)
        {
            throw new NotImplementedException();
        }

        private static bool ValidateMembersKeyword(KeywordSymbol key)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IASTVisiter visiter)
        {
            TypeBuilder tb;
            try
            {
                tb = DelegatesCache.moduleBuilder.DefineType(this.protoClass.Name, this.protoClass.Attrs);
            }
            catch (ArgumentException)
            {
                tb = DelegatesCache.moduleBuilder.DefineType(this.protoClass.Name + ClrTypeExpression.counter, this.protoClass.Attrs);
                ClrTypeExpression.counter++;
            }
            foreach (ClrFieldExprerssion field in this.protoClass.protoFields)
            {
                field.Visit(tb);
            }
            foreach (ClrMethodExpression method in this.protoClass.protoMethods)
            {
                method.Visit(tb);
            }
            Type GeneratedType = tb.CreateType();
            TypeCache.RedefineAlias(this.protoClass.Name, GeneratedType);
            TypeCache.DynamicGeneratedTypes.Add(GeneratedType.GUID);
            visiter.Emitter.EmitTypeSkipVisible(GeneratedType);
        }

        private enum ClassDeclParserState
        {
            Start,
            Attribute,
            ClassAttribute,
            ClassName,
            BaseType,
            ParseBaseType,
            ClassBody,
            End
        }

        private class ProtoClass
        {
            internal TypeAttributes Attrs;
            internal List<string> BaseTypes;
            internal List<ClrClassExpression.ProtoCustomAttribute> CustomAttrs;
            internal string Name;
            internal List<ClrFieldExprerssion> protoFields;
            internal List<ClrMethodExpression> protoMethods;

            public ProtoClass()
            {
                this.protoFields = new List<ClrFieldExprerssion>();
                this.protoMethods = new List<ClrMethodExpression>();
                this.Attrs = TypeAttributes.AutoLayout;
                this.CustomAttrs = new List<ClrClassExpression.ProtoCustomAttribute>();
                this.BaseTypes = new List<string>();
            }

            public ProtoClass(TypeAttributes attrs, List<ClrClassExpression.ProtoCustomAttribute> customAttrs, List<string> baseTypes, string name)
            {
                this.protoFields = new List<ClrFieldExprerssion>();
                this.protoMethods = new List<ClrMethodExpression>();
                this.Attrs = attrs;
                this.CustomAttrs = customAttrs;
                this.BaseTypes = baseTypes;
                this.Name = name;
            }

            internal void AddField(ClrFieldExprerssion pf)
            {
                this.protoFields.Add(pf);
            }

            internal void AddMethod(ClrMethodExpression expression)
            {
                this.protoMethods.Add(expression);
            }
        }

        private class ProtoCustomAttribute : Pair<string, object[]>
        {
            public ProtoCustomAttribute(string typeName, object[] cParams)
            {
                base.First = typeName;
                base.Second = cParams;
            }
        }

        private class ProtoField
        {
            internal FieldAttributes Attrs;
            internal Expression InitialValue;
            internal Symbol Name;
            internal System.Type Type;

            public ProtoField(FieldAttributes Attrs, System.Type Type, Symbol Name, Expression InitialValue)
            {
                this.Attrs = Attrs;
                this.Type = Type;
                this.Name = Name;
                this.InitialValue = InitialValue;
            }
        }*/

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

