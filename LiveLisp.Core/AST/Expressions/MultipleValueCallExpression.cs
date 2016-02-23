namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class MultipleValueCallExpression : Expression
    {
        private Expression _function;
        private List<Expression> valuesProducers;

        public MultipleValueCallExpression(Expression function, List<Expression> values, ExpressionContext context) : base(context)
        {
            this._function = function;
            this.valuesProducers = values;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.MultipleValueCallExpression(this, context);
        }

        public Expression Function
        {
            get
            {
                return this._function;
            }
            set
            {
                this._function = value;
            }
        }

        public List<Expression> ValuesProducers
        {
            get
            {
                return this.valuesProducers;
            }
            set
            {
                this.valuesProducers = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.MultipleValueCallExpression(this, context);
        }
    }
}

