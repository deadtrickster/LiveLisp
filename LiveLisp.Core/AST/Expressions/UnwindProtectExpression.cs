namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class UnwindProtectExpression : Expression
    {
        private List<Expression> _cleanupForms;
        private Expression _protectedForm;

        public UnwindProtectExpression(Expression protectedForm, List<Expression> cleanupForms, ExpressionContext context) : base(context)
        {
            this._cleanupForms = new List<Expression>();
            this._protectedForm = protectedForm;
            this._cleanupForms = cleanupForms;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.UnwindProtectExpression(this, context);
        }

        public List<Expression> CleanupForms
        {
            get
            {
                return this._cleanupForms;
            }
            set
            {
                this._cleanupForms = value;
            }
        }

        public Expression ProtectedForm
        {
            get
            {
                return this._protectedForm;
            }
            set
            {
                this._protectedForm = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.UnwindProtectExpression(this, context);
        }
    }
}

