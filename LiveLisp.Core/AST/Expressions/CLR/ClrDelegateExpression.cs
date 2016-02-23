namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using LiveLisp.Core.Compiler;
    using System;
    using System.Reflection;

    public class ClrDelegateExpression : ClrMemberExpression
    {
        private OrdinaryLambdaList parametersList;
        private StaticTypeResolver returnType;
        private System.Reflection.TypeAttributes typeAttributes;

        public ClrDelegateExpression(ExpressionContext context) : base(context)
        {
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.ClrDelegateExpression(this, context);
        }

        public OrdinaryLambdaList ParametersList
        {
            get
            {
                return this.parametersList;
            }
            set
            {
                this.parametersList = value;
            }
        }

        public StaticTypeResolver ReturnType
        {
            get
            {
                return this.returnType;
            }
            set
            {
                this.returnType = value;
            }
        }

        public System.Reflection.TypeAttributes TypeAttributes
        {
            get
            {
                return this.typeAttributes;
            }
            set
            {
                this.typeAttributes = value;
            }
        }
    }
}

