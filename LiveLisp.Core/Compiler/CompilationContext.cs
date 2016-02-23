using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST.Expressions.CLR;
using LiveLisp.Core.AST;
using System.Diagnostics;
using System.Reflection.Emit;

namespace LiveLisp.Core.Compiler
{
    public class CompilationContext
    {
        public readonly InstructionsBlock MainInstructionsBlock;

        public CompilationContext(InstructionsBlock MainInstructionsBlock)
        {
            this.MainInstructionsBlock = MainInstructionsBlock;
        }

        public readonly List<ClassDeclaration> new_classes = new List<ClassDeclaration>();

        public void AddClassDeclaration(ClassDeclaration new_class)
        {
            new_classes.Add(new_class);
        }
    }

    public class InstructionsBlock : List<ILInstruction>
    {
        // Fields
        public readonly LabelsCollection Labels = new LabelsCollection();
        public readonly VariablesCollection Locals = new VariablesCollection();

        // Methods
        internal void Add(InstructionsBlock current)
        {
            base.AddRange(current);
        }

        private void CheckTransferInstructions()
        {
            for (int i = 0; i < base.Count; i++)
            {
                if (base[i] is BeginExceptionBlock)
                {
                    i = this.StartWithExcBlock(i);
                }
            }
        }

        internal void Create(ILGenerator ilgen)
        {
            this.CheckTransferInstructions();
            this.InitLocals(ilgen);
            this.InitLabels(ilgen);
            for (int i = 0; i < base.Count; i++)
            {
                base[i].Emit(this, ilgen);
            }
        }

        public LabelDeclaration DefineLabel()
        {
            return this.Labels.WithUniqueName();
        }

        internal VariableDeclaration DefineLocal()
        {
            return this.DefineLocal(typeof(object));
        }

        public VariableDeclaration DefineLocal(StaticTypeResolver type)
        {
            return this.Locals.WithUniqueName(type);
        }

        public void InitLabels(ILGenerator ilGen)
        {
            for (int i = 0; i < this.Labels.Count; i++)
            {
                this.Labels[i].Label = ilGen.DefineLabel();
            }
        }

        public void InitLocals(ILGenerator ilGen)
        {
            for (int i = 0; i < this.Locals.Count; i++)
            {
                Type localType = this.Locals[i].Type.Type;
                this.Locals[i].Id = ilGen.DeclareLocal(localType).LocalIndex;
            }
        }

        internal string NewUniqueLabelName()
        {
            return this.Labels.SuggestName();
        }

        internal void RegisterLabel(LabelDeclaration labelDeclaration)
        {
            this.Labels.Add(labelDeclaration);
        }

        private int StartWithExcBlock(int i)
        {
            int i_backup = i;
            i++;
            bool end_founded = false;
            List<string> labels_marked_in_this_block = new List<string>();
            List<int> patchable_local_transfers = new List<int>();
            List<int> unpatchable_local_transfers = new List<int>();
            for (int j = i; j < base.Count; j++)
            {
                if (base[j] is EndExceptionBlock)
                {
                    end_founded = true;
                    break;
                }
                if (base[j] is MarkLabelInstruction)
                {
                    labels_marked_in_this_block.Add((base[j] as MarkLabelInstruction).Label.Name);
                }
                else if (base[j] is BrInstruction)
                {
                    patchable_local_transfers.Add(j);
                }
                else if (base[j] is SimpleLocalTransferILInstruction)
                {
                    unpatchable_local_transfers.Add(i);
                }
                else if (base[j] is BeginExceptionBlock)
                {
                    j = this.StartWithExcBlock(j);
                }
            }
            if (!end_founded)
            {
                throw new SimpleErrorException("End of exception block not found");
            }
            for (int p = 0; p < patchable_local_transfers.Count; p++)
            {
                if (!labels_marked_in_this_block.Contains((base[patchable_local_transfers[p]] as TransferILInstruction).Label))
                {
                    base[patchable_local_transfers[p]] = new LeaveInstruction((base[patchable_local_transfers[p]] as TransferILInstruction).Label);
                }
            }
            for (int up = 0; up < unpatchable_local_transfers.Count; up++)
            {
                if (!labels_marked_in_this_block.Contains((base[unpatchable_local_transfers[up]] as TransferILInstruction).Label))
                {
                    throw new SimpleErrorException("Cannot use " + base[unpatchable_local_transfers[up]].OpCode + " to transfer control out/into from/to exception block");
                }
            }
            return i;
        }

        public override string ToString()
        {
            int i;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            for (i = 0; i < this.Locals.Count; i++)
            {
                sb.Append("     ");
                sb.Append(this.Locals[i]).ToString();
            }
            for (i = 0; i < base.Count; i++)
            {
                if (base[i] is MarkLabelInstruction)
                {
                    sb.Append(" ");
                }
                else
                {
                    sb.Append("     ");
                }
                sb.AppendLine(base[i].ToString());
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }


}
