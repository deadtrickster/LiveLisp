namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;
    #if !SILVERLIGHT
    [Serializable]
#endif
    public class CatchExpression : PrognExpression
    {
        private Expression _tag;

        public Expression Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public CatchExpression(Expression tag, List<Expression> forms, ExpressionContext context) : base(forms, context)
        {
            this._tag = tag;
            _forms = forms;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.CatchExpression(this, context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.CatchExpression(this, context);
        }
    }
}

