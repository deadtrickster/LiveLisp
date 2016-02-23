namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;

    public class ThrowExpression : Expression
    {
        private Expression _catchTag;
        private Expression _resultForm;

        public ThrowExpression(Expression catchTag, Expression resultForm, ExpressionContext context) : base(context)
        {
            this._catchTag = catchTag;
            this._resultForm = resultForm;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.ThrowExpression(this, context);
        }

        public Expression CatchTag
        {
            get
            {
                return this._catchTag;
            }
            set
            {
                this._catchTag = value;
            }
        }

        public Expression ResultForm
        {
            get
            {
                return this._resultForm;
            }
            set
            {
                this._resultForm = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.ThrowExpression(this, context);
        }
    }
}

