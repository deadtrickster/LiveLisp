namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Diagnostics;
#if !SILVERLIGHT
    [Serializable]
    
#endif
    [DebuggerDisplay("{ToString()}")]
    public class IfExpression : Expression
    {
        private Expression _condition;
        private Expression _else;
        private Expression _then;

        public IfExpression(Expression condition, Expression then, ExpressionContext context)
            : this(condition, then, new ConstantExpression(DefinedSymbols.NIL, context), context)
        {
        }

        public IfExpression(Expression condition, Expression then, Expression els, ExpressionContext context) : base(context)
        {
            this._condition = condition;
            this._then = then;
            this._else = els;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.IfExpression(this, context);
        }

        public Expression Condition
        {
            get
            {
                return this._condition;
            }
            set
            {
                this._condition = value;
            }
        }

        public Expression Else
        {
            get
            {
                return this._else;
            }
            set
            {
                this._else = value;
            }
        }

        public Expression Then
        {
            get
            {
                return this._then;
            }
            set
            {
                this._then = value;
            }
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            return new IfExpression(_condition.Copy(new ExpressionContext(new_context)), _then.Copy(new ExpressionContext(new_context)), _else.Copy(new ExpressionContext(new_context)), new_context);
        }

        public override string ToString()
        {
            string newVariable = "(if " + Condition.ToString() +
                    Then.ToString() +
                    (Else != null ? Else.ToString() : "") + ")";
            return newVariable;
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.IfExpression(this, context);
        }
    }
}

