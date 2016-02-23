namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class LetExpression : LetLetStartBase
    {
        public LetExpression(List<SyntaxBinding> bindings, List<Declaration> declarations, List<Expression> forms, ExpressionContext context) : base(bindings, declarations, forms, context)
        {
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.LetExpression(this, context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.LetExpression(this, context);
        }
    }
}

