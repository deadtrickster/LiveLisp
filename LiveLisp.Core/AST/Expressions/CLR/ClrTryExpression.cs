namespace LiveLisp.Core.AST.Expressions
{
    using System;
using System.Collections.Generic;

    public class ClrTryExpression : Expression
    {
        List<Expression> _tryExpressions;

        Dictionary<StaticTypeResolver, List<Expression>> _catchExpressions;

        List<Expression> _finallyExpressions;
        public ClrTryExpression(List<Expression> tryExpressions, ExpressionContext context)
            : this(tryExpressions, new Dictionary<StaticTypeResolver, List<Expression>>(), new List<Expression>(), context)
        {

        }

        public ClrTryExpression(List<Expression> tryExpressions,
            List<Expression> FinallyExpressions, ExpressionContext context)
            : this(tryExpressions, new Dictionary<StaticTypeResolver, List<Expression>>(), FinallyExpressions, context)
        {

        }

        public ClrTryExpression(List<Expression> tryExpressions,
            Dictionary<StaticTypeResolver, List<Expression>> CatchExpressions, ExpressionContext context)
            : this(tryExpressions, CatchExpressions, new List<Expression>(), context)
        {

        }


        public ClrTryExpression(List<Expression> tryExpressions,
            Dictionary<StaticTypeResolver, List<Expression>> CatchExpressions,
            List<Expression> FinallyExpressions, ExpressionContext context)
            : base(context)
        {
            _tryExpressions = tryExpressions;
            _catchExpressions = CatchExpressions;
            _finallyExpressions = FinallyExpressions;
        }


        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.ClrTryExpression(this, context);
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            return new ClrTryExpression(CopyTryExpressions(new_context), CopyCatchExpressions(new_context), CopyFinally(new_context), new_context);
        }

        private List<Expression> CopyTryExpressions(ExpressionContext new_context)
        {
            throw new NotImplementedException();
        }

        private Dictionary<StaticTypeResolver, List<Expression>> CopyCatchExpressions(ExpressionContext new_context)
        {
            throw new NotImplementedException();
        }

        private List<Expression> CopyFinally(ExpressionContext new_context)
        {
            throw new NotImplementedException();
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

