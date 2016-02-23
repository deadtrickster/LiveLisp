using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;

namespace LiveLisp.Core.AST.Expressions.CLR.Operators
{
    /// <summary>
    /// operators handled by il opcodes
    /// </summary>
    public class SimpleBinaryOperator : Expression
    {
        Expression _left;
        Expression _right;
        Operator _op;

        public SimpleBinaryOperator(Operator op, Expression left, Expression right, ExpressionContext ctx)
            : base(ctx)
        {
            _left = left;
            _right = right;
            _op = op;
        }

        public SimpleBinaryOperator(Operator op, Expression left, Expression right)
            : base(null)
        {
            _left = left;
            _right = right;
            _op = op;
        }


        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;

            _left.Visit(visitor, context);
            _right.Visit(visitor, context);

            switch (_op)
            {
                case Operator.Add:
                    cec.InstructionsBlock.Add(new AddInstruction());
                    break;
                case Operator.Sub:
                    cec.InstructionsBlock.Add(new SubInstruction());
                    break;
                case Operator.Mul:
                    cec.InstructionsBlock.Add(new MulInstruction());
                    break;
                case Operator.Div:
                    cec.InstructionsBlock.Add(new DivInstruction());
                    break;
                default:
                    break;
            }
        }
    }
}
