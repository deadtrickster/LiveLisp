using System.Collections;
namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;

    public class ClrConstantExpression : Expression
    {
        private object constant;

        internal ClrConstantExpression(object constant, ExpressionContext context)
            : base(context)
        {
            this.constant = constant;
        }

        internal ClrConstantExpression(object constant)
            : base(ExpressionContext.Root)
        {
            this.constant = constant;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.ClrConstantExpression(this, context);
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

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

