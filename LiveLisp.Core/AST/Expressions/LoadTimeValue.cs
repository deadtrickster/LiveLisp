namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;

    public class LoadTimeValue : Expression
    {
        private Expression form;
        private bool readOnly;

        public LoadTimeValue(Expression form, ExpressionContext context) : base(context)
        {
            this.form = form;
        }

        public LoadTimeValue(Expression form, bool read_only, ExpressionContext context)
            : base(context)
        {
            this.form = form;
            readOnly = read_only;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.LoadTimeValueExpression(this, context);
        }

        public Expression Form
        {
            get
            {
                return this.form;
            }
            set
            {
                this.form = value;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return this.readOnly;
            }
            set
            {
                this.readOnly = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.LoadTimeValue(this, context);
        }
    }
}

