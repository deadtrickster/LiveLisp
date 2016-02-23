namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class FletExpression : FLMBaseExpression
    {
        public FletExpression(List<LambdaFunctionDesignator> lambdas, List<Declaration> declarations, List<Expression> forms, ExpressionContext context) : base(lambdas, declarations, forms, context)
        {
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.FletExpression(this, context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.FletExpression(this, context);
        }
    }
}

