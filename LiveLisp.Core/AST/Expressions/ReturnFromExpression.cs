namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using LiveLisp.Core.Types;
    using System;

    public class ReturnFromExpression : Expression
    {
        private Expression _result;
        private Symbol _tag;

        public ReturnFromExpression(Symbol tag, Expression result, ExpressionContext context) : base(context)
        {
            this._tag = tag;
            this._result = result;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.ReturnFromExpression(this, context);
        }

        public Expression Form
        {
            get
            {
                return this._result;
            }
            set
            {
                this._result = value;
            }
        }

        public Symbol Tag
        {
            get
            {
                return this._tag;
            }
            set
            {
                this._tag = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.ReturnFromExpression(this, context);
        }
    }
}

