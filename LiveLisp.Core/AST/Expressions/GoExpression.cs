namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
using System.Reflection.Emit;
using LiveLisp.Core.Types;
    using LiveLisp.Core.Compiler;

    public class GoExpression : Expression
    {
        private object tag;

        public GoExpression(object tag, ExpressionContext context) : base(context)
        {
            this.tag = tag;
        }

        public override void Visit(IASTWalker visitor, ExpressionContext context)
        {
            visitor.GoExpression(this, context);
        }

        public object Tag
        {
            get
            {
                return this.tag;
            }
            set
            {
                this.tag = value;
            }
        }

        internal override Expression Copy(ExpressionContext new_context)
        {
            return new GoExpression(tag, new_context);
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.GoExpression(this, context);
        }
    }

    internal class GoLocalGenerator : IGenerator
    {
        public Label go_location;

        #region IGenerator Members

        public void Generate(ILGenerator gen)
        {
            gen.JmpToLabel(go_location);
        }

        #endregion
    }

    internal class GoNonlocalGenerator : IGenerator
    {
        public FieldBuilder TagbodyName;
        public int LabelId;

        #region IGenerator Members

        public void Generate(ILGenerator gen)
        {
            gen.EmitFieldGet(TagbodyName);
            gen.EmitInt32(LabelId);
            gen.EmitFieldSet(BlockNonLocalTransfer.TagIdField);
            gen.EmitFieldGet(TagbodyName);
            gen.Emit(OpCodes.Throw);
        }

        #endregion
    }
}

