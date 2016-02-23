namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;

    public class MultipleValueProg1Expression : FormsContainer
    {
        private Expression _firstForm;
        private List<Expression> discarededForms;

        public MultipleValueProg1Expression(Expression firstForm, List<Expression> forms, ExpressionContext context) : base(forms, context)
        {
            this._firstForm = firstForm;
            discarededForms = forms;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.MultipleValueProg1Expression(this, context);
        }

        public List<Expression> DiscarededForms
        {
            get
            {
                return this.discarededForms;
            }
            set
            {
                this.discarededForms = value;
            }
        }

        public Expression FirstForm
        {
            get
            {
                return this._firstForm;
            }
            set
            {
                this._firstForm = value;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.MultipleValueProg1Expression(this, context);
        }
    }
}

