namespace LiveLisp.Core.AST.Expressions.CLR
{
    using LiveLisp.Core.AST;
    using System;
    using LiveLisp.Core.Compiler;

    public class ILCodeExpression : Expression
    {
        public readonly ILInstruction Instruction;
        public ILCodeExpression(ILInstruction instruction, ExpressionContext context)
            : base(context)
        {
            Instruction = instruction;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.ILCodeExpression(this, context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }
}

