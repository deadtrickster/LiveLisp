namespace LiveLisp.Core.AST
{
    using LiveLisp.Core.AST.Expressions;
    using System;
    using System.Collections.Generic;

    public class DeclarationsContainer : PrognExpression, IDeclarationsContainer
    {
        private List<Declaration> _Declarations;

        public DeclarationsContainer(List<Declaration> declarations, List<Expression> forms, ExpressionContext context) : base(forms, context)
        {
            this._Declarations = declarations;
        }

        protected void Visit(LocallyExpression locallyExpression)
        {
            throw new NotImplementedException();
        }

        public List<Declaration> Declarations
        {
            get
            {
                return this._Declarations;
            }
        }
    }
}

