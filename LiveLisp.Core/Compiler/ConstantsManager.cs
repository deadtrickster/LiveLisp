using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.Compiler
{
    internal abstract class ConstantsManager
    {
        protected Dictionary<object, ConstantSlot> store = new Dictionary<object, ConstantSlot>();

        internal abstract ConstantSlot GetSlot(object constant, Type type);
    }

    internal class ClassScopedConstantManager : ConstantsManager
    {
        ClassDeclaration decl;

        public ClassScopedConstantManager(ClassDeclaration decl)
        {
            this.decl = decl;
        }

        internal override ConstantSlot GetSlot(object constant, Type type)
        {
            if (store.ContainsKey(constant))
                return store[constant];

            else
            {
                string name = SuggestName(constant);
                FieldDeclaration field = decl.NewGeneratedField(name, constant.GetType());
                field.Attributes |= System.Reflection.FieldAttributes.Static | System.Reflection.FieldAttributes.Private;
                ConstantSlot slot = new FieldConstantSlot(field, constant, type);
                store.Add(constant, slot);

                slot.EmitSet(decl.TypeConstructor.MethodProlog);
                return slot;
            }
        }

        private string SuggestName(object constant)
        {
            Symbol symbol = constant as Symbol;

            if (symbol != null)
                return "Constant$" + symbol.ToString(true);

            else return constant.GetType().ToString();
        }
    }

    internal class MethodScopedConstantsManager : ConstantsManager
    {
        MethodDeclaration _decl;
        public MethodScopedConstantsManager(MethodDeclaration decl)
        {
            _decl = decl;
        }

        internal override ConstantSlot GetSlot(object constant, Type type)
        {
            if (store.ContainsKey(constant))
                return store[constant];

            else
            {
                VariableDeclaration name = _decl.Instructions.DefineLocal(type);
                ConstantSlot slot = new LocalConstantSlot(name, constant, type);
                store.Add(constant, slot);

                slot.EmitSet(_decl.MethodProlog);
                return slot;
            }
        }
    }


    internal abstract class ConstantSlot : Slot
    {

    }

    internal class LocalConstantSlot : ConstantSlot
    {
        VariableDeclaration _var;

        object constant;
        Type boxType;

        public LocalConstantSlot(VariableDeclaration var, object constant, Type type)
        {
            this._var = var;
            this.constant = constant;
            this.boxType = type;
        }

        public override void EmitGet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new LdlocInstruction(_var));
        }

        public override void EmitSetProlog(InstructionsBlock instructionsBlock)
        {

        }

        public override void EmitSet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new PushInstruction(constant));
            if (boxType != null)
                instructionsBlock.Add(new LboxInstruction(boxType));
            instructionsBlock.Add(new StlocInstruction(_var));
        }
    }

    internal class FieldConstantSlot : ConstantSlot
    {
        FieldDeclaration field;
        object constant;
        Type boxType;

        public FieldConstantSlot(FieldDeclaration field, object constant, Type boxType)
        {
            this.field = field;
            this.constant = constant;
            this.boxType = boxType;
        }

        public override void EmitGet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new LdsfldInstruction(field));
        }

        public override void EmitSetProlog(InstructionsBlock instructionsBlock)
        {
            
        }

        public override void EmitSet(InstructionsBlock instructionsBlock)
        {
            instructionsBlock.Add(new PushInstruction(constant));
            if(boxType != null)
                instructionsBlock.Add(new LboxInstruction(boxType));
            instructionsBlock.Add(new StsfldInstruction(field));
        }

        public void Set(object value)
        {
            if (field.CreatedField == null || field.CreatedField is FieldBuilder)
                throw new FormattedException("Constant field is not cooked");

            field.CreatedField.SetValue(null, value);
        }

        public object Get()
        {
            if (field.CreatedField == null || field.CreatedField is FieldBuilder)
                throw new FormattedException("Constant field is not cooked");

            return field.CreatedField.GetValue(null);
        }
    }

}
