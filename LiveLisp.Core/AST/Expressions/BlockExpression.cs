namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using LiveLisp.Core.Types;
    using System;
    using System.Collections.Generic;

    public class BlockExpression : PrognExpression
    {
        private Symbol _blockName;

        public BlockExpression(Symbol blockName) : base(null, null)
        {
            this._blockName = blockName;
        }

        public BlockExpression(Symbol blockName, List<Expression> forms, ExpressionContext context) : base(forms, context)
        {
            this._blockName = blockName;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.BlockExpression(this, context);
        }

        public Symbol BlockName
        {
            get
            {
                return this._blockName;
            }
            set
            {
                this._blockName = value;
            }
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            List<Expression> new_params = new List<Expression>(Forms.Count);
            foreach (var item in Forms)
            {
                new_params.Add(item.Copy(new ExpressionContext(new_context)));
            }

            return new BlockExpression(_blockName, new_params, new_context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.BlockExpression(this, context);
        }
    }
}

