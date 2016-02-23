namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;
using LiveLisp.Core.Types;
    using LiveLisp.Core.Compiler;

    public class SetqExpression : Expression
    {
        private List<SyntaxBinding> _assings;

        public List<SyntaxBinding> Assings
        {
            get
            {
                return this._assings;
            }
            set
            {
                this._assings = value;
            }
        }

        public SetqExpression(List<SyntaxBinding> assign, ExpressionContext context)
            : base(context)
        {
            this._assings = assign;
        }

        public SetqExpression(Symbol symbol, Expression value, ExpressionContext context)
            : base(context)
        {
            _assings = new List<SyntaxBinding>();
            _assings.Add(new SyntaxBinding(symbol, value));
        }

        public SetqExpression(Slot slot, Expression value, ExpressionContext context)
            : base(context)
        {
            _assings = new List<SyntaxBinding>();
            _assings.Add(new SyntaxBinding(slot, value));
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.SetqExpression(this, context);
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            List<SyntaxBinding> new_assigns = new List<SyntaxBinding>(_assings.Count);

            for (int i = 0; i < _assings.Count; i++)
            {
                SyntaxBinding binding = new SyntaxBinding(_assings[i].Symbol, _assings[i].Value.Copy(new ExpressionContext(new_context)));
                binding.Slot = _assings[i].Slot;
                new_assigns.Add(binding);
            }

            return new SetqExpression(new_assigns, new_context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.SetqExpression(this, context);
        }
    }
}

