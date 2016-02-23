namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;

    public class FunctionExpression : FunctionNameDesignator
    {
        private FunctionNameDesignator _designator;

        public FunctionExpression(FunctionNameDesignator designator, ExpressionContext context) : base(context)
        {
            this._designator = designator;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.FunctionExpression(this, context);
        }

        public override void Visit(IASTWalker visitor, CallExpression call, ExpressionContext context)
        {
            visitor.EmitFunctionDesignatorCall(this, call, context);
        }

        public FunctionNameDesignator Designator
        {
            get
            {
                return this._designator;
            }
            set
            {
                this._designator = value;
            }
        }
        public override FunctionNameDesignatorType DesignatorType
        {
            get
            {
                return _designator.DesignatorType;
            }
        }

        public Expression Substitution
        {
            get;
            set;
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return Designator.Eval(evaluator, context);
        }
    }
}

