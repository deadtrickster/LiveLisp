using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST.Expressions;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.AST
{
    public class LambdaOverload : PrognExpression
    {
        List<Slot> _localSlots;

        public List<Slot> LocalSlots
        {
            get { return _localSlots; }
            set { _localSlots = value; }
        }

        public LambdaOverload(List<Slot> localSlots, List<Expression> forms, ExpressionContext context)
            : base(forms, context)
        {
            _localSlots = localSlots;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.LambdaOverload(this, context);
        }
    }

    public class LambdaOverloads : Expression
    {
        List<LambdaOverload> _overloads;

        public List<LambdaOverload> Overloads
        {
            get { return _overloads; }
            set { _overloads = value; }
        }

        LambdaList _lambdaList;

        public LambdaList LambdaList
        {
            get { return _lambdaList; }
            set { _lambdaList = value; }
        }

        Symbol _name;

        public Symbol Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// index in overloads list correspond to params number in Overload
        /// if overload is null then method wouldn't be overrided
        /// </summary>
        /// <param name="overloads"></param>
        /// <param name="context"></param>
        public LambdaOverloads(LambdaList ll, Symbol name, List<LambdaOverload> overloads, ExpressionContext context)
            :base(context)
        {
            _lambdaList = ll;
            if (name != null)
                _name = name;
            else
                _name = DefinedSymbols.Lambda;
            _overloads = overloads;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.LambdaOverloadsExpression(this, context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
