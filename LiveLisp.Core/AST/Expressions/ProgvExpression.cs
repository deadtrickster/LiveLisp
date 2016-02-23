namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class ProgvExpression : PrognExpression
    {
        private Expression _symbols;
        private Expression _values;

        public ProgvExpression(Expression symbols, Expression values, List<Expression> forms, ExpressionContext context) : base(forms, context)
        {
            this._symbols = symbols;
            this._values = values;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.ProgvExpression(this, context);
        }

        public Expression Symbols
        {
            get
            {
                return this._symbols;
            }
            set
            {
                this._symbols = value;
            }
        }

        public Expression Values
        {
            get
            {
                return this._values;
            }
            set
            {
                this._values = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.ProgvExpression(this, context);
        }
    }
}

