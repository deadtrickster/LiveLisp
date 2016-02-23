namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class ClrMemberExpression : Expression
    {
        private List<ClrAttributeMark> attributes;
        private string name;

        public ClrMemberExpression(ExpressionContext context) : base(context)
        {
            this.attributes = new List<ClrAttributeMark>();
        }

        protected void Visit(ClrDelegateExpression clrDelegateExpression)
        {
            throw new NotImplementedException();
        }

        protected void Visit(ClrFieldExpression clrFieldExpression)
        {
            throw new NotImplementedException();
        }

        protected void Visit(ClrMethodExpression clrMethodExpression)
        {
            throw new NotImplementedException();
        }

        protected void Visit(ClrPropertyExpression clrPropertyExpression)
        {
            throw new NotImplementedException();
        }

        public List<ClrAttributeMark> Attributes
        {
            get
            {
                return this.attributes;
            }
            set
            {
                this.attributes = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

