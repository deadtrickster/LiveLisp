namespace LiveLisp.Core.AST
{
    using LiveLisp.Core.AST.Expressions;
    using System;
    using System.Collections.Generic;

    public class FormsContainer : Expression, IImplicitProgn
    {
        protected List<Expression> _forms;

        public FormsContainer(List<Expression> forms, ExpressionContext context) : base(context)
        {
            this._forms = forms;
        }

        

        public List<Expression> Forms
        {
            get
            {
                return this._forms;
            }
        }

        public Expression this[int index]
        {
            get { return _forms[index]; }
        }

        public int Count
        {
            get { return _forms.Count; }
        }

        public Expression Last
        {
            get { return _forms[_forms.Count -1];}
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

