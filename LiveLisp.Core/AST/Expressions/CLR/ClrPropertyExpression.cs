namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;

    public class ClrPropertyExpression : ClrMemberExpression
    {
        private ClrMethodExpression getter;
        private ClrMethodExpression setter;

        public ClrPropertyExpression(ExpressionContext context) : base(context)
        {
        }

        public ClrMethodExpression Getter
        {
            get
            {
                return this.getter;
            }
            set
            {
                this.getter = value;
            }
        }

        public ClrMethodExpression Setter
        {
            get
            {
                return this.setter;
            }
            set
            {
                this.setter = value;
            }
        }
    }
}

