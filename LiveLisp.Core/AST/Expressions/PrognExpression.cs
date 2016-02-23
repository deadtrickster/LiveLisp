namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class PrognExpression : FormsContainer
    {
        public PrognExpression(List<Expression> forms, ExpressionContext context) : base(forms, context)
        {
        }

        public PrognExpression(ExpressionContext context)
            : base(new List<Expression>(), context)
        {
        }

        protected void Visit(CatchExpression catchExpression)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.PrognExpression(this, context);
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            List<Expression> new_forms = new List<Expression>();
            for (int i = 0; i < Forms.Count; i++)
            {
                new_forms.Add(Forms[i].Copy(new ExpressionContext(new_context)));
            }

            return new PrognExpression(new_forms, new_context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.PrognExpression(this, context);
        }
    }
}

