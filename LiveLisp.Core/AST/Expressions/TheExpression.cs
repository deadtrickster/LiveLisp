namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;

    public class TheExpression : Expression
    {
        private Expression form;
        private LispTypeSpecifier typeSpecifier;

        public TheExpression(ExpressionContext context) : base(context)
        {
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.TheExpression(this, context);
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

        public LispTypeSpecifier TypeSpecifier
        {
            get
            {
                return this.typeSpecifier;
            }
            set
            {
                this.typeSpecifier = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.TheExpression(this, context);
        }
    }
}

