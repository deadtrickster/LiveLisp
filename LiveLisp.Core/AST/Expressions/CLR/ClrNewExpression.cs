using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.AST.Expressions
{
    public class ClrNewExpression : Expression
    {
        StaticTypeResolver _resolver;

        public StaticTypeResolver Resolver
        {
            get { return _resolver; }
            set { _resolver = value; }
        }

        List<StaticTypeResolver> _argsTypes;

        public List<StaticTypeResolver> ArgsTypes
        {
            get { return _argsTypes; }
            set { _argsTypes = value; }
        }

        List<Expression> _args;

        public List<Expression> Args
        {
            get { return _args; }
            set { _args = value; }
        }

        public ClrNewExpression(StaticTypeResolver reslover, List<StaticTypeResolver> argsTypes, List<Expression> args,  ExpressionContext context)
            : base(context)
        {
            _resolver = reslover;
            _argsTypes = argsTypes;
            _args = args;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.ClrNewExpression(this, context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
