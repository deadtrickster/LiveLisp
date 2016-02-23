namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class MacroletExpression : FLMBaseExpression
    {
        public MacroletExpression(List<LambdaFunctionDesignator> lambdas, List<Declaration> declarations, List<Expression> forms, ExpressionContext context) : base(lambdas, declarations, forms, context)
        {
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.MacroletExpression(this, context);
        }
    }
}

