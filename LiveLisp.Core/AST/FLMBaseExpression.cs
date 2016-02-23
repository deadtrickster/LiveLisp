namespace LiveLisp.Core.AST
{
    using LiveLisp.Core.AST.Expressions;
    using System;
    using System.Collections.Generic;

    public class FLMBaseExpression : DeclarationsContainer
    {
        private List<LambdaFunctionDesignator> _lambdas;

        public FLMBaseExpression(List<LambdaFunctionDesignator> lambdas, List<Declaration> declarations, List<Expression> forms, ExpressionContext context) : base(declarations, forms, context)
        {
            this._lambdas = new List<LambdaFunctionDesignator>();
            this._lambdas = lambdas;
        }

        protected void Visit(FletExpression fletExpression)
        {
            throw new NotImplementedException();
        }

        protected void Visit(LabelsExpression labelsExpression)
        {
            throw new NotImplementedException();
        }

        protected void Visit(MacroletExpression macroletExpression)
        {
            throw new NotImplementedException();
        }

        public List<LambdaFunctionDesignator> Lambdas
        {
            get
            {
                return this._lambdas;
            }
            set
            {
                this._lambdas = value;
            }
        }
    }
}

