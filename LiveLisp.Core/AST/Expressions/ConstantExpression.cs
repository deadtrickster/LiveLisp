using System.Collections;
namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{ToString()}")]
    public class ConstantExpression : Expression
    {
        private object constant;

        internal ConstantExpression(object constant, ExpressionContext context) : base(context)
        {
            this.constant = constant;
        }

        internal ConstantExpression(object constant)
            : base(ExpressionContext.Root)
        {
            this.constant = constant;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.ConstantExpression(this, context);
        }

        public object Constant
        {
            get
            {
                return this.constant;
            }
            set
            {
                this.constant = value;
            }
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            return new ConstantExpression(constant, new_context);
        }

        public override string ToString()
        {
            return constant.ToString();
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.ConstantExpression(this, context);
        }
    }
}

