using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.AST.Expressions
{
    public class ClrIfExpression : Expression
    {
          private Expression _condition;
        private Expression _else;
        private Expression _then;

        public ClrIfExpression(Expression condition, Expression then, ExpressionContext context)
            : this(condition, then, new ConstantExpression(DefinedSymbols.NIL, context), context)
        {
        }

        public ClrIfExpression(Expression condition, Expression then, Expression els, ExpressionContext context)
            : base(context)
        {
            this._condition = condition;
            this._then = then;
            this._else = els;
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

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.ClrIfExpression(this, context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
