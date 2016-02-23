namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class LocallyExpression : DeclarationsContainer
    {
        public LocallyExpression(List<Declaration> declarations, List<Expression> forms, ExpressionContext context) : base(declarations, forms, context)
        {
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.LocallyExpression(this, context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.LocallyExpression(this, context);
        }
    }
}

