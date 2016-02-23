using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types;
using System.Reflection.Emit;
using LiveLisp.Core.AST;
using LiveLisp.Core.BuiltIns.Numbers;
using System.Reflection;

namespace LiveLisp.Core.Compiler
{
    internal class Binding
    {
        Symbol name;

        public Symbol Name
        {
            get { return name; }
            set { name = value; }
        }
        Slot slot;

        internal Slot Slot
        {
            get { return slot; }
            set { slot = value; }
        }

        public Binding(Symbol name, Slot slot)
        {
            this.name = name;
            this.slot = slot;
        }
    }

    public abstract class Slot
    {
        public abstract void EmitGet(InstructionsBlock instructionsBlock);

        public abstract void EmitSetProlog(InstructionsBlock instructionsBlock);

        public abstract void EmitSet(InstructionsBlock instructionsBlock);
    }

    internal class LocalSlot : Slot
    {
        int slot_num;

        public int Slot_num
        {
            get { return slot_num; }
        }

        public LocalSlot(VariableDeclaration var)
        {
            slot_num = var.Id;
        }

        public LocalSlot(int slot)
        {
            slot_num = slot;
        }

        public override void EmitGet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new LdlocInstruction((short)slot_num));
        }

        public override void EmitSetProlog(InstructionsBlock instructionsBlock)
        {

        }

        public override void EmitSet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new StlocInstruction((short)slot_num));
        }
    }

    internal class MethodParameterSlot : Slot
    {
        int slot_num;

        public MethodParameterSlot(int slot)
        {
            slot_num = slot;
        }

        public override void EmitGet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new LdargInstruction((short)slot_num));
        }

        public override void EmitSetProlog(InstructionsBlock instructionsBlock)
        {

        }

        public override void EmitSet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new StargInstruction((short)slot_num));
        }
    }
    internal class FieldSlot : Slot
    {
        protected Slot instanceSlot;
        StaticFieldResolver field;

        public FieldSlot(Slot instanceSlot, StaticFieldResolver field)
        {
            this.instanceSlot = instanceSlot;
            this.field = field;
        }

        public override void EmitGet(InstructionsBlock instructionsBlock)
        {
            if (instanceSlot != null)
            {
                instanceSlot.EmitGet(instructionsBlock);
                instructionsBlock.Add(new LdfldInstruction(field));
            }
            else
            {
                instructionsBlock.Add(new LdsfldInstruction(field));
            }
        }

        public override void EmitSetProlog(InstructionsBlock instructionsBlock)
        {
            if (instanceSlot != null)
                instanceSlot.EmitGet(instructionsBlock);
        }

        public override void EmitSet(InstructionsBlock instructionsBlock)
        {
            if (instanceSlot != null)
            {
                instructionsBlock.Add(new StfldInstruction(field));
            }
            else
            {
                instructionsBlock.Add(new StsfldInstruction(field));
            }
        }
    }


    internal class GlobalSlot : Slot
    {
        Symbol name;

        public GlobalSlot(Symbol slot)
        {
            name = slot;
        }

        public override void EmitGet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new PushInstruction(name));
            instructionsBlock.Add(new CallvirtInstruction(get_ValueMethod));
        }

        public override void EmitSetProlog(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new PushInstruction(name));
        }

        public override void EmitSet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new CallvirtInstruction(set_ValueMethod));
        }

        static MethodInfo get_ValueMethod = typeof(Symbol).GetMethod("get_Value");
        static MethodInfo set_ValueMethod = typeof(Symbol).GetMethod("set_Value");
    }

    internal class GlobalRawSlot : Slot
    {
        Symbol name;

        public GlobalRawSlot(Symbol slot)
        {
            name = slot;
        }

        public override void EmitGet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new PushInstruction(name));
            instructionsBlock.Add(new CallvirtInstruction(GetRawSymbolValueMethod));
        }

        public override void EmitSetProlog(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new PushInstruction(name));
        }

        public override void EmitSet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new CallvirtInstruction(SetRawSymbolValueMethod));
        }

        public static StaticMethodResolver GetRawSymbolValueMethod = typeof(Symbol).GetMethod("get_RawValue");
        public static StaticMethodResolver SetRawSymbolValueMethod = typeof(Symbol).GetMethod("set_RawValue");
    }

    internal class GlobalFunctionSlot : Slot
    {
        Symbol name;

        public GlobalFunctionSlot(Symbol slot)
        {
            name = slot;
        }

        public override void EmitGet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new PushInstruction(name));
            instructionsBlock.Add(new CallvirtInstruction(get_FunctionMethod));
        }

        public override void EmitSetProlog(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new PushInstruction(name));
        }

        public override void EmitSet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new CallvirtInstruction(set_FunctionMethod));
        }

        static MethodInfo get_FunctionMethod = typeof(Symbol).GetMethod("get_Function");
        static MethodInfo set_FunctionMethod = typeof(Symbol).GetMethod("set_Function");
    }
}
