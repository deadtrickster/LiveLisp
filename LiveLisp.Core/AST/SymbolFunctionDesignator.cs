namespace LiveLisp.Core.AST
{
    using LiveLisp.Core.Types;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{ToString()}")]
    public class SymbolFunctionDesignator : FunctionNameDesignator
    {
        private Symbol _name;


        public override FunctionNameDesignatorType DesignatorType
        {
            get
            {
                return FunctionNameDesignatorType.Symbol;
            }
        }

        public SymbolFunctionDesignator(Symbol name, ExpressionContext context) : base(context)
        {
            this._name = name;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.SymbolFunctionDesignator(this, context);
        }

        public override void Visit(IASTWalker visiter, LiveLisp.Core.AST.Expressions.CallExpression call, ExpressionContext context)
        {
            visiter.EmitMethodInfoDesignatorCall(this, call, context);
        }

        public Symbol Name
        {
            get
            {
                return this._name;
            }
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            return new SymbolFunctionDesignator(_name, new_context);
        }

        public override string ToString()
        {
            return _name.ToString();
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.SymbolFunctionDesignator(this, context);
        }
    }
}

