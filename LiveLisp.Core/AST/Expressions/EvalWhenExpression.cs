namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;
    using LiveLisp.Core.Types;

    public class EvalWhenExpression : FormsContainer
    {
        private List<Symbol> conditions;

        public EvalWhenExpression(List<Symbol> conditions, List<Expression> forms, ExpressionContext context) : base(forms, context)
        {
            this.conditions = conditions;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.EvalWhenExpression(this, context);
        }

        public List<Symbol> Conditions
        {
            get
            {
                return this.conditions;
            }
            set
            {
                this.conditions = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.EvalWhenExpression(this, context);
        }
    }
}

