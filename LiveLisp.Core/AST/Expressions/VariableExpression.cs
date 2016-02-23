namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using LiveLisp.Core.Types;
    using System;
using LiveLisp.Core.Compiler;
    using System.Diagnostics;
    [DebuggerDisplay("{ToString()}")]
    public class VariableExpression : Expression
    {
        private Symbol _VariableName;

        public Symbol VariableName
        {
            get
            {
                return this._VariableName;
            }
            set
            {
                this._VariableName = value;
            }
        }

        private Slot _Slot;

        public Slot Slot
        {
            get { return _Slot; }
            set { _Slot = value; }
        }

        internal VariableExpression(Symbol varName) : base(ExpressionContext.Root)
        {
            this._VariableName = varName;
        }

        internal VariableExpression(Symbol varName, ExpressionContext context) : base(context)
        {
            this._VariableName = varName;
        }

        internal VariableExpression(Slot slot, ExpressionContext context)
            : base(context)
        {
            _Slot = slot;
        }

        internal VariableExpression(Slot slot)
            : base(ExpressionContext.Root)
        {
            _Slot = slot;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.VariableExpression(this, context);
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            return new VariableExpression(VariableName, new_context);
        }

        public override string ToString()
        {
            return VariableName.ToString();
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
           return  evaluator.VariableExpression(this, context);
        }
    }
}

