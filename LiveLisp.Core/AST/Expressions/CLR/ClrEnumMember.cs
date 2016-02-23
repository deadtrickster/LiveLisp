namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;

    public class ClrEnumMember : ClrMemberExpression
    {
        private object value;

        public ClrEnumMember(ExpressionContext context) : base(context)
        {
        }

        public object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
    }
}

