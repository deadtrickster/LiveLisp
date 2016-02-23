namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    #if !SILVERLIGHT
    [Serializable]
#endif
    [DebuggerDisplay("{ToString()}")]
    public class CallExpression : Expression
    {
        private FunctionNameDesignator _function;
        private List<Expression> _parameters;

        public CallExpression(FunctionNameDesignator function, List<Expression> parameters, ExpressionContext context) : base(context)
        {
            this._function = function;
            this._parameters = parameters;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.CallExpression(this, context);
        }

        public FunctionNameDesignator Function
        {
            get
            {
                return this._function;
            }
            set
            {
                this._function = value;
            }
        }

        public List<Expression> Parameters
        {
            get
            {
                return this._parameters;
            }
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            List<Expression> new_params = new List<Expression>(_parameters.Count);
            foreach (var item in _parameters)
            {
                new_params.Add(item.Copy(new ExpressionContext(new_context)));
            }

            return new CallExpression(_function.Copy(new ExpressionContext(new_context)) as FunctionNameDesignator, new_params, new_context);
        }

        public override string ToString()
        {
            return "(" + Function.ToString() + PrintArgs() + ")";
        }

        private string PrintArgs()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Parameters.Count; i++)
            {
                sb.Append(" " + Parameters[i].ToString());
            }

            return sb.ToString();
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.CallExpression(this, context);
        }
    }
}

