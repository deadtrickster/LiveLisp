namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class SymbolMacroletExpression : DeclarationsContainer
    {
        private List<SyntaxBinding> symbolMacros;

        public SymbolMacroletExpression(List<Declaration> declarations, List<Expression> forms, ExpressionContext context) : base(declarations, forms, context)
        {
            this.symbolMacros = new List<SyntaxBinding>();
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.SymbolMacroletExpression(this, context);
        }

        public List<SyntaxBinding> SymbolMacros
        {
            get
            {
                return this.symbolMacros;
            }
            set
            {
                this.symbolMacros = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.SymbolMacroletExpression(this, context);
        }
    }
}

