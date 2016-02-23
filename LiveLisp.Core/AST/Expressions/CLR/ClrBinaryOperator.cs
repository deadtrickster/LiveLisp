using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.AST.Expressions.CLR
{
    public enum Operator
    {
        Add,
        Sub,
        Mul,
        Div,
        LessThen,
        GreaterThen,
        LessOrEqual,
        GreaterOrEqual,
        Equal,
        NotEqual
    }

 /*   public class ClrBinaryOperator : Expression
    {
        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            base.Visit(visitor, context);
        }
    }*/
}
