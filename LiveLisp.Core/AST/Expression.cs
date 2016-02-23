namespace LiveLisp.Core.AST
{
    using LiveLisp.Core.AST.Expressions;
    using LiveLisp.Core.Types;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
using LiveLisp.Core.Interpreter;

    [DebuggerDisplay("{ToString()}")]
    public abstract class Expression
    {
        private ExpressionContext context;

        protected Expression(ExpressionContext context)
        {
            this.context = context;
            context.Expression = this;
        }

        public virtual void Visit(IASTWalker visitor, ExpressionContext context)
        {
            throw new NotImplementedException("Expression.Visit must be overrided in derived class");
        }

        public ExpressionContext Context
        {
            get
            {
                return this.context;
            }
            set
            {
                this.context = value;
            }
        }

        public Expression Parent
        {
            get
            {
                if (context.Parent == null)
                    return null;
                return context.Parent.Expression;
            }
        }

        internal virtual Expression Copy(ExpressionContext new_context)
        {
            throw new NotImplementedException();
        }

        public abstract object Eval(IEvalWalker evaluator, EvaluationContext context);
    }
}

