namespace LiveLisp.Core.AST
{
    using System;
using LiveLisp.Core.AST.Expressions;
    public enum FunctionNameDesignatorType
    {
        Symbol,
        Lambda,
        MethodInfo
    }
    public class FunctionNameDesignator : Expression
    {
        public virtual FunctionNameDesignatorType DesignatorType
        {
            get { return FunctionNameDesignatorType.MethodInfo; }
        }

        public FunctionNameDesignator(ExpressionContext context) : base(context)
        {
        }

        /// <summary>
        /// use this to call holded method, use Expression.Visit to load named function on the stack
        /// </summary>
        /// <param name="visiter"></param>
        /// <param name="call"></param>
        public virtual void Visit(IASTWalker visiter, CallExpression call, ExpressionContext context)
        {
            throw new InvalidOperationException("Visit(IASTWalker visiter, CallExpression call)");
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

