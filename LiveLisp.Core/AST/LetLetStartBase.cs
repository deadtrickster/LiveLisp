namespace LiveLisp.Core.AST
{
    using LiveLisp.Core.AST.Expressions;
    using System;
    using System.Collections.Generic;
    using LiveLisp.Core.Types;

    public class LetLetStartBase : DeclarationsContainer, INewLexicalBindingsProvider
    {
        private List<SyntaxBinding> _bindings;

        public LetLetStartBase(List<SyntaxBinding> bindings, List<Declaration> declarations, List<Expression> forms, ExpressionContext context) : base(declarations, forms, context)
        {
            this._bindings = bindings;
        }

        protected void Visit(LetExpression letExpression)
        {
            throw new NotImplementedException();
        }

        protected void Visit(LetStarExpression letStarExpression)
        {
            throw new NotImplementedException();
        }

        public List<SyntaxBinding> NewBindings
        {
            get
            {
                return this._bindings;
            }
        }

        #region INewLexicalBindingsProvider Members

        public List<Symbol> IntroducesLexicalVariables
        {
            get
            {
                List<Symbol> vars = new List<Symbol>();

                for (int i = 0; i < NewBindings.Count; i++)
                {
                    if (!vars.Contains(NewBindings[i].Symbol))
                        vars.Add(NewBindings[i].Symbol);
                }

                for (int i = 0; i < Declarations.Count; i++)
                {
                    SpecialDeclaration specialdecl = Declarations[i] as SpecialDeclaration;
                    if (specialdecl != null)
                    {
                        for (int j = 0; j < specialdecl.Vars.Count; j++)
                        {
                            vars.Remove(specialdecl.Vars[j]);
                        }
                    }
                }

                return vars;
            }
        }

        #endregion
    }
}

