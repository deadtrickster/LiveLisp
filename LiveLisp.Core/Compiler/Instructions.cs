using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using LiveLisp.Core.AST;
using LiveLisp.Core.Types;
using System.Reflection;

namespace LiveLisp.Core.Compiler
{
    public class ILInstruction
    {
        public readonly OpCode OpCode;

        public ILInstruction(OpCode opCode)
        {
            OpCode = opCode;
        }

        public virtual void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            throw new InvalidOperationException();
        }
    }

    class SimpleILInstruction : ILInstruction
    {
        public SimpleILInstruction(OpCode opCode)
            : base(opCode)
        {

        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.Emit(OpCode);
        }

        public override string ToString()
        {
            return OpCode.ToString();
        }
    }

    class TransferILInstruction : ILInstruction
    {
        public readonly string Label;
        public TransferILInstruction(OpCode opCode, string label)
            : base(opCode)
        {
            this.Label = label;
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            if (iblock.Labels.Contains(Label))
            {
                ILGen.Emit(OpCode, iblock.Labels[Label].Label);
            }
            else
            throw new SyntaxErrorException("Label " + Label + " not found");
        }

        public override string ToString()
        {
            return OpCode.ToString() + " " + Label;
        }
    }


    class SimpleLocalTransferILInstruction : TransferILInstruction
    {
        public SimpleLocalTransferILInstruction(OpCode opCode, string label)
            : base(opCode, label)
        {
        }
    }

    class TypedILInstruction : ILInstruction
    {
        public readonly StaticTypeResolver TypeResolver;

        public TypedILInstruction(OpCode opCode, StaticTypeResolver typeResolver)
            : base(opCode)
        {
            TypeResolver = typeResolver;
        }

        public override string ToString()
        {
            return OpCode.ToString() + " " + TypeResolver.ToString();
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.Emit(OpCode, TypeResolver.Type);
        }
    }

    class AddInstruction : SimpleILInstruction
    {
        public AddInstruction()
            : base(OpCodes.Add)
        {

        }
    }

    class AddOvfInstruction : SimpleILInstruction
    {
        public AddOvfInstruction()
            : base(OpCodes.Add_Ovf)
        {

        }
    }

    class AddOvfUnInstruction : SimpleILInstruction
    {
        public AddOvfUnInstruction()
            : base(OpCodes.Add_Ovf_Un)
        {

        }
    }

    class AndInstruction : SimpleILInstruction
    {
        public AndInstruction()
            : base(OpCodes.And)
        {

        }
    }

    class ArglistInstruction : SimpleILInstruction
    {
        public ArglistInstruction()
            : base(OpCodes.Arglist)
        {

        }
    }

    class BeqInstruction : SimpleLocalTransferILInstruction
    {
        public BeqInstruction(string label)
            : base(OpCodes.Beq, label)
        {

        }
    }

    class BgeInstruction : SimpleLocalTransferILInstruction
    {
        public BgeInstruction(string label)
            : base(OpCodes.Bge, label)
        {

        }
    }

    class BgeUnInstruction : SimpleLocalTransferILInstruction
    {
        public BgeUnInstruction(string label)
            : base(OpCodes.Bge_Un, label)
        {

        }
    }

    class BgtInstruction : SimpleLocalTransferILInstruction
    {
        public BgtInstruction(string label)
            : base(OpCodes.Bgt, label)
        {

        }
    }

    class BgtUnInstruction : SimpleLocalTransferILInstruction
    {
        public BgtUnInstruction(string label)
            : base(OpCodes.Bgt_Un, label)
        {

        }
    }

    class BleInstruction : SimpleLocalTransferILInstruction
    {
        public BleInstruction(string label)
            : base(OpCodes.Ble, label)
        {

        }
    }

    class BleUnInstruction : SimpleLocalTransferILInstruction
    {
        public BleUnInstruction(string label)
            : base(OpCodes.Ble_Un, label)
        {

        }
    }

    class BltInstruction : SimpleLocalTransferILInstruction
    {
        public BltInstruction(string label)
            : base(OpCodes.Blt, label)
        {

        }
    }

    class BltUnInstruction : SimpleLocalTransferILInstruction
    {
        public BltUnInstruction(string label)
            : base(OpCodes.Blt_Un, label)
        {

        }
    }

    class BneUnInstruction : SimpleLocalTransferILInstruction
    {
        public BneUnInstruction(string label)
            : base(OpCodes.Bne_Un, label)
        {

        }
    }

    class BoxInstruction : TypedILInstruction
    {
        public BoxInstruction(StaticTypeResolver typeResolver)
            : base(OpCodes.Box, typeResolver)
        {

        }
    }

    class BrInstruction : TransferILInstruction
    {
        public BrInstruction(string label)
            : base(OpCodes.Br, label)
        {

        }

        public BrInstruction(LabelDeclaration label)
            : base(OpCodes.Br, label.Name)
        {

        }
    }

    class BreakInstruction : SimpleILInstruction
    {
        public BreakInstruction()
            : base(OpCodes.Break)
        {

        }
    }

    class BrfalseInstruction : SimpleLocalTransferILInstruction
    {
        public BrfalseInstruction(string label)
            : base(OpCodes.Brfalse, label)
        {

        }

        public BrfalseInstruction(LabelDeclaration label)
            :base(OpCodes.Brfalse, label.Name)
        {

        }
    }

    class BrtrueInstruction : SimpleLocalTransferILInstruction
    {
        public BrtrueInstruction(string label)
            : base(OpCodes.Brtrue, label)
        {

        }
    }

    class _CallInstruction : ILInstruction
    {
        public readonly IMethodBaseResolver resolver;

        public _CallInstruction(OpCode opCode, IMethodBaseResolver methodResolver)
            : base(opCode)
        {
            resolver = methodResolver;
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            switch (resolver.Type)
            {
                case IMethodBaseResolverType.Method:
                    ILGen.Emit(OpCode, (resolver as StaticMethodResolver).Method);
                    break;
                case IMethodBaseResolverType.Constructor:
                    ILGen.Emit(OpCode, (resolver as StaticConstructorResolver).Constructor);
                    break;
            }
        }

        public override string ToString()
        {
            return OpCode.ToString() + " " + resolver.ToString();
        }
    }

    class CallInstruction : _CallInstruction
    {

        public CallInstruction(IMethodBaseResolver methodResolver)
            : base(OpCodes.Call, methodResolver)
        {

        }
    }

    class CalliInstruction : _CallInstruction
    {

        public CalliInstruction(IMethodBaseResolver methodResolver)
            : base(OpCodes.Calli, methodResolver)
        {
        }
    }

    class CallvirtInstruction : _CallInstruction
    {

        public CallvirtInstruction(IMethodBaseResolver methodResolver)
            : base(OpCodes.Callvirt, methodResolver)
        {
        }
    }

    class CastclassInstruction : TypedILInstruction
    {
        public CastclassInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Castclass, typeName)
        {

        }
    }

    class CeqInstruction : SimpleILInstruction
    {
        public CeqInstruction()
            : base(OpCodes.Ceq)
        {

        }
    }

    class CgtInstruction : SimpleILInstruction
    {
        public CgtInstruction()
            : base(OpCodes.Cgt)
        {

        }
    }


    class CgtUnInstruction : SimpleILInstruction
    {
        public CgtUnInstruction()
            : base(OpCodes.Cgt_Un)
        {

        }
    }

    class CkfiniteInstruction : SimpleILInstruction
    {
        public CkfiniteInstruction()
            : base(OpCodes.Ckfinite)
        {

        }
    }

    class CltInstruction : SimpleILInstruction
    {
        public CltInstruction()
            : base(OpCodes.Clt)
        {

        }
    }


    class Clt_UnInstruction : SimpleILInstruction
    {
        public Clt_UnInstruction()
            : base(OpCodes.Clt_Un)
        {

        }
    }

    class ConstrainedInstruction : TypedILInstruction
    {
        public ConstrainedInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Constrained, typeName)
        {

        }
    }


    class Conv_IInstruction : SimpleILInstruction
    {
        public Conv_IInstruction()
            : base(OpCodes.Conv_I)
        {

        }
    }



    class Conv_I1Instruction : SimpleILInstruction
    {
        public Conv_I1Instruction()
            : base(OpCodes.Conv_I1)
        {

        }
    }


    class Conv_I2Instruction : SimpleILInstruction
    {
        public Conv_I2Instruction()
            : base(OpCodes.Conv_I2)
        {

        }
    }


    class Conv_I4Instruction : SimpleILInstruction
    {
        public Conv_I4Instruction()
            : base(OpCodes.Conv_I4)
        {

        }
    }


    class Conv_I8Instruction : SimpleILInstruction
    {
        public Conv_I8Instruction()
            : base(OpCodes.Conv_I8)
        {

        }
    }

    class Conv_Ovf_IInstruction : SimpleILInstruction
    {
        public Conv_Ovf_IInstruction()
            : base(OpCodes.Conv_Ovf_I)
        {

        }
    }


    class Conv_Ovf_I_UnInstruction : SimpleILInstruction
    {
        public Conv_Ovf_I_UnInstruction()
            : base(OpCodes.Conv_Ovf_I_Un)
        {

        }
    }

    class Conv_Ovf_I1Instruction : SimpleILInstruction
    {
        public Conv_Ovf_I1Instruction()
            : base(OpCodes.Conv_Ovf_I1)
        {

        }
    }

    class Conv_Ovf_I1_UnInstruction : SimpleILInstruction
    {
        public Conv_Ovf_I1_UnInstruction()
            : base(OpCodes.Conv_Ovf_I1_Un)
        {

        }
    }

    class Conv_Ovf_I2Instruction : SimpleILInstruction
    {
        public Conv_Ovf_I2Instruction()
            : base(OpCodes.Conv_Ovf_I2)
        {

        }
    }

    class Conv_Ovf_I2_UnInstruction : SimpleILInstruction
    {
        public Conv_Ovf_I2_UnInstruction()
            : base(OpCodes.Conv_Ovf_I2_Un)
        {

        }
    }

    class Conv_Ovf_I4Instruction : SimpleILInstruction
    {
        public Conv_Ovf_I4Instruction()
            : base(OpCodes.Conv_Ovf_I4)
        {

        }
    }

    class Conv_Ovf_I4_UnInstruction : SimpleILInstruction
    {
        public Conv_Ovf_I4_UnInstruction()
            : base(OpCodes.Conv_Ovf_I4_Un)
        {

        }
    }

    class Conv_Ovf_I8Instruction : SimpleILInstruction
    {
        public Conv_Ovf_I8Instruction()
            : base(OpCodes.Conv_Ovf_I8)
        {

        }
    }

    class Conv_Ovf_I8_UnInstruction : SimpleILInstruction
    {
        public Conv_Ovf_I8_UnInstruction()
            : base(OpCodes.Conv_Ovf_I8_Un)
        {

        }
    }

    class Conv_Ovf_UInstruction : SimpleILInstruction
    {
        public Conv_Ovf_UInstruction()
            : base(OpCodes.Conv_Ovf_U)
        {

        }
    }

    class Conv_Ovf_U_UnInstruction : SimpleILInstruction
    {
        public Conv_Ovf_U_UnInstruction()
            : base(OpCodes.Conv_Ovf_U_Un)
        {

        }
    }

    class Conv_Ovf_U1Instruction : SimpleILInstruction
    {
        public Conv_Ovf_U1Instruction()
            : base(OpCodes.Conv_Ovf_U1)
        {

        }
    }

    class Conv_Ovf_U1_UnInstruction : SimpleILInstruction
    {
        public Conv_Ovf_U1_UnInstruction()
            : base(OpCodes.Conv_Ovf_U1_Un)
        {

        }
    }

    class Conv_Ovf_U2Instruction : SimpleILInstruction
    {
        public Conv_Ovf_U2Instruction()
            : base(OpCodes.Conv_Ovf_U2)
        {

        }
    }

    class Conv_Ovf_U2_UnInstruction : SimpleILInstruction
    {
        public Conv_Ovf_U2_UnInstruction()
            : base(OpCodes.Conv_Ovf_U2_Un)
        {

        }
    }

    class Conv_Ovf_U4Instruction : SimpleILInstruction
    {
        public Conv_Ovf_U4Instruction()
            : base(OpCodes.Conv_Ovf_U4)
        {

        }
    }

    class Conv_Ovf_U4_UnInstruction : SimpleILInstruction
    {
        public Conv_Ovf_U4_UnInstruction()
            : base(OpCodes.Conv_Ovf_U4_Un)
        {

        }
    }

    class Conv_Ovf_U8Instruction : SimpleILInstruction
    {
        public Conv_Ovf_U8Instruction()
            : base(OpCodes.Conv_Ovf_U8)
        {

        }
    }

    class Conv_Ovf_U8_UnInstruction : SimpleILInstruction
    {
        public Conv_Ovf_U8_UnInstruction()
            : base(OpCodes.Conv_Ovf_U8_Un)
        {

        }
    }

    class Conv_R_UnInstruction : SimpleILInstruction
    {
        public Conv_R_UnInstruction()
            : base(OpCodes.Conv_R_Un)
        {

        }
    }

    class Conv_R4Instruction : SimpleILInstruction
    {
        public Conv_R4Instruction()
            : base(OpCodes.Conv_R4)
        {

        }
    }

    class Conv_R8Instruction : SimpleILInstruction
    {
        public Conv_R8Instruction()
            : base(OpCodes.Conv_R8)
        {

        }
    }

    class Conv_UInstruction : SimpleILInstruction
    {
        public Conv_UInstruction()
            : base(OpCodes.Conv_U)
        {

        }
    }

    class Conv_U1Instruction : SimpleILInstruction
    {
        public Conv_U1Instruction()
            : base(OpCodes.Conv_U1)
        {

        }
    }

    class Conv_U2Instruction : SimpleILInstruction
    {
        public Conv_U2Instruction()
            : base(OpCodes.Conv_U2)
        {

        }
    }

    class Conv_U4Instruction : SimpleILInstruction
    {
        public Conv_U4Instruction()
            : base(OpCodes.Conv_U4)
        {

        }
    }

    class Conv_U8Instruction : SimpleILInstruction
    {
        public Conv_U8Instruction()
            : base(OpCodes.Conv_U8)
        {

        }
    }

    class CpblkInstruction : SimpleILInstruction
    {
        public CpblkInstruction()
            : base(OpCodes.Cpblk)
        {

        }
    }

    class CpobjInstruction : TypedILInstruction
    {
        public CpobjInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Cpobj, typeName)
        {

        }
    }

    class DivInstruction : SimpleILInstruction
    {
        public DivInstruction()
            : base(OpCodes.Div)
        {

        }
    }

    class Div_UnInstruction : SimpleILInstruction
    {
        public Div_UnInstruction()
            : base(OpCodes.Div_Un)
        {

        }
    }

    class DupInstruction : SimpleILInstruction
    {
        public DupInstruction()
            : base(OpCodes.Dup)
        {

        }
    }

    class EndfilterInstruction : SimpleILInstruction
    {
        public EndfilterInstruction()
            : base(OpCodes.Endfilter)
        {

        }
    }

    class EndfinallyInstruction : SimpleILInstruction
    {
        public EndfinallyInstruction()
            : base(OpCodes.Endfinally)
        {

        }
    }

    class InitblkInstruction : SimpleILInstruction
    {
        public InitblkInstruction()
            : base(OpCodes.Initblk)
        {

        }
    }

    class InitobjInstruction : TypedILInstruction
    {
        public InitobjInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Initobj, typeName)
        {

        }
    }

    class IsinstInstruction : TypedILInstruction
    {
        public IsinstInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Isinst, typeName)
        {

        }
    }

    class JmpInstruction : ILInstruction
    {
        StaticMethodResolver resolver;

        public JmpInstruction(StaticMethodResolver methodResolver)
            : base(OpCodes.Jmp)
        {
            resolver = methodResolver;
        }

        public override string ToString()
        {
            return OpCode.ToString() + " " + resolver.ToString();
        }
    }

    class LdargInstruction : ILInstruction
    {
        public readonly int ArgumentNumber;

        public LdargInstruction(int argumentNumber)
            : base(OpCodes.Ldarg)
        {
            ArgumentNumber = argumentNumber;
        }

        public override string ToString()
        {
            return "Ldarg " + ArgumentNumber;
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitLdarg(ArgumentNumber);
        }
    }

    class LdargaInstruction : ILInstruction
    {
        public readonly short ArgumentNumber;

        public LdargaInstruction(short argumentNumber)
            : base(OpCodes.Ldarg)
        {
            ArgumentNumber = argumentNumber;
        }

        public override string ToString()
        {
            return "Ldarga " + ArgumentNumber;
        }
    }

    class Ldc_I4Instruction : ILInstruction
    {
        public readonly int Number;

        public Ldc_I4Instruction(int number)
            : base(OpCodes.Ldc_I4)
        {
            Number = number;
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitInt32(Number);
        }

        public override string ToString()
        {
            return "Ldc_I4 " + Number;
        }
    }

    class Ldc_I8Instruction : ILInstruction
    {
        public readonly Int64 Number;

        public Ldc_I8Instruction(long number)
            : base(OpCodes.Ldc_I8)
        {
            Number = number;
        }

        public override string ToString()
        {
            return "Ldc_I8 " + Number;
        }
    }

    class Ldc_R4Instruction : ILInstruction
    {
        public readonly Single Number;

        public Ldc_R4Instruction(Single number)
            : base(OpCodes.Ldc_R4)
        {
            Number = number;
        }

        public override string ToString()
        {
            return "Ldc_R4 " + Number;
        }
    }

    class Ldc_R8Instruction : ILInstruction
    {
        public readonly Double Number;

        public Ldc_R8Instruction(Double number)
            : base(OpCodes.Ldc_R8)
        {
            Number = number;
        }

        public override string ToString()
        {
            return "Ldc_R8 " + Number;
        }
    }

    class LdelemInstruction : TypedILInstruction
    {
        public LdelemInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Ldelem, typeName)
        {

        }
    }


    class Ldelem_IInstruction : SimpleILInstruction
    {
        public Ldelem_IInstruction()
            : base(OpCodes.Ldelem_I)
        {

        }
    }

    class Ldelem_I1Instruction : SimpleILInstruction
    {
        public Ldelem_I1Instruction()
            : base(OpCodes.Ldelem_I1)
        {

        }
    }

    class Ldelem_I2Instruction : SimpleILInstruction
    {
        public Ldelem_I2Instruction()
            : base(OpCodes.Ldelem_I2)
        {

        }
    }

    class Ldelem_I4Instruction : SimpleILInstruction
    {
        public Ldelem_I4Instruction()
            : base(OpCodes.Ldelem_I4)
        {

        }
    }

    class Ldelem_I8Instruction : SimpleILInstruction
    {
        public Ldelem_I8Instruction()
            : base(OpCodes.Ldelem_I8)
        {

        }
    }

    class Ldelem_R4Instruction : SimpleILInstruction
    {
        public Ldelem_R4Instruction()
            : base(OpCodes.Ldelem_R4)
        {

        }
    }

    class Ldelem_R8Instruction : SimpleILInstruction
    {
        public Ldelem_R8Instruction()
            : base(OpCodes.Ldelem_R8)
        {

        }
    }


    class Ldelem_RefInstruction : SimpleILInstruction
    {
        public Ldelem_RefInstruction()
            : base(OpCodes.Ldelem_Ref)
        {

        }
    }

    class Ldelem_U1Instruction : SimpleILInstruction
    {
        public Ldelem_U1Instruction()
            : base(OpCodes.Ldelem_U1)
        {

        }
    }

    class Ldelem_U2Instruction : SimpleILInstruction
    {
        public Ldelem_U2Instruction()
            : base(OpCodes.Ldelem_U2)
        {

        }
    }


    class Ldelem_U4Instruction : SimpleILInstruction
    {
        public Ldelem_U4Instruction()
            : base(OpCodes.Ldelem_U4)
        {

        }
    }


    class LdelemaInstruction : TypedILInstruction
    {
        public LdelemaInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Ldelema, typeName)
        {

        }
    }

    class LdfldInstruction : ILInstruction
    {
        StaticFieldResolver fieldResolver;

        public LdfldInstruction(StaticFieldResolver fieldResolver)
            : base(OpCodes.Ldfld)
        {
            this.fieldResolver = fieldResolver;
        }

        public override string ToString()
        {
            return "Ldfld " + fieldResolver.ToString();
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitFieldGet(fieldResolver.Field);
        }
    }

    class LdfldaInstruction : ILInstruction
    {
        StaticFieldResolver fieldResolver;

        public LdfldaInstruction(StaticFieldResolver fieldResolver)
            : base(OpCodes.Ldfld)
        {
            this.fieldResolver = fieldResolver;
        }

        public override string ToString()
        {
            return "Ldflda " + fieldResolver.ToString();
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            FieldInfo field = fieldResolver.Field;

            if ((field.Attributes & FieldAttributes.Static) == FieldAttributes.Static)
            {
                throw new FormattedException("fielda " + field + " is not an instance field. Cannot be argument for Stfld opcode");
            }

            ILGen.Emit(OpCodes.Ldflda, field);
        }

    }

    class LdftnInstruction : ILInstruction
    {
        StaticMethodResolver MethodResolver;

        public LdftnInstruction(StaticMethodResolver methodResolver)
            : base(OpCodes.Ldftn)
        {
            this.MethodResolver = methodResolver;
        }

        public override string ToString()
        {
            return "Ldftn " + MethodResolver.ToString();
        }
    }


    class Ldind_IInstruction : SimpleILInstruction
    {
        public Ldind_IInstruction()
            : base(OpCodes.Ldind_I)
        {

        }
    }

    class Ldind_I1Instruction : SimpleILInstruction
    {
        public Ldind_I1Instruction()
            : base(OpCodes.Ldind_I1)
        {

        }
    }

    class Ldind_I2Instruction : SimpleILInstruction
    {
        public Ldind_I2Instruction()
            : base(OpCodes.Ldind_I2)
        {

        }
    }

    class Ldind_I4Instruction : SimpleILInstruction
    {
        public Ldind_I4Instruction()
            : base(OpCodes.Ldind_I4)
        {

        }
    }

    class Ldind_I8Instruction : SimpleILInstruction
    {
        public Ldind_I8Instruction()
            : base(OpCodes.Ldind_I8)
        {

        }
    }

    class Ldind_R4Instruction : SimpleILInstruction
    {
        public Ldind_R4Instruction()
            : base(OpCodes.Ldind_R4)
        {

        }
    }

    class Ldind_R8Instruction : SimpleILInstruction
    {
        public Ldind_R8Instruction()
            : base(OpCodes.Ldind_R8)
        {

        }
    }


    class Ldind_RefInstruction : SimpleILInstruction
    {
        public Ldind_RefInstruction()
            : base(OpCodes.Ldind_Ref)
        {

        }
    }

    class Ldind_U1Instruction : SimpleILInstruction
    {
        public Ldind_U1Instruction()
            : base(OpCodes.Ldind_U1)
        {

        }
    }

    class Ldind_U2Instruction : SimpleILInstruction
    {
        public Ldind_U2Instruction()
            : base(OpCodes.Ldind_U2)
        {

        }
    }

    class Ldind_U4Instruction : SimpleILInstruction
    {
        public Ldind_U4Instruction()
            : base(OpCodes.Ldind_U4)
        {

        }
    }

    class LdlenInstruction : SimpleILInstruction
    {
        public LdlenInstruction()
            : base(OpCodes.Ldlen)
        {

        }
    }

    class LdlocInstruction : ILInstruction
    {
        public readonly Nullable<int> LocalNumber;
        public readonly Symbol SlotName;
        public LdlocInstruction(int var_num)
            : base(OpCodes.Ldloc)
        {
            LocalNumber = var_num;
        }

        public LdlocInstruction(Symbol slotName)
            : base(OpCodes.Ldloc)
        {
            SlotName = slotName;
        }

        public LdlocInstruction(VariableDeclaration var)
            :base(OpCodes.Ldloc)
        {
            LocalNumber = var.Id;
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitLocalGet(LocalNumber.Value);
        }

        public override string ToString()
        {
            return "Ldloc " + LocalNumber.ToString();
        }
    }

    class LdlocaInstruction : ILInstruction
    {
        public readonly Nullable<int> LocalNumber;
        public readonly Symbol SlotName;
        public LdlocaInstruction(short var_num)
            : base(OpCodes.Ldloca)
        {
            LocalNumber = var_num;
        }

        public LdlocaInstruction(Symbol slotName)
            : base(OpCodes.Ldloca)
        {
            SlotName = slotName;
        }

        public LdlocaInstruction(VariableDeclaration var)
            :base(OpCodes.Ldloc)
        {
            LocalNumber = var.Id;
        }

        public override string ToString()
        {
            return "Ldloca " + LocalNumber.ToString();
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            if (0 <= LocalNumber.Value && LocalNumber.Value < 256)
            {
                ILGen.Emit(OpCodes.Ldloca_S, LocalNumber.Value);
            }
            else
            {
                ILGen.Emit(OpCodes.Ldloca, LocalNumber.Value);
            }
        }
    }

    class LdnullInstruction : SimpleILInstruction
    {
        public LdnullInstruction()
            : base(OpCodes.Ldnull)
        {

        }
    }

    class LdobjInstruction : TypedILInstruction
    {
        public LdobjInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Ldobj, typeName)
        {

        }
    }

    class LdsfldInstruction : ILInstruction
    {
        StaticFieldResolver fieldResolver;

        public LdsfldInstruction(StaticFieldResolver fieldResolver)
            : base(OpCodes.Ldfld)
        {
            this.fieldResolver = fieldResolver;
        }

        public override string ToString()
        {
            return "Ldsfld " + fieldResolver.ToString();
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            FieldInfo field = fieldResolver.Field;

            if ((field.Attributes & FieldAttributes.Static) != FieldAttributes.Static)
            {
                throw new FormattedException("field " + field + " is not a static field. Cannot be argument for Ldsfld opcode");
            }

            ILGen.Emit(OpCodes.Ldsfld, field);
        }
    }

    class LdsfldaInstruction : ILInstruction
    {
        StaticFieldResolver fieldResolver;

        public LdsfldaInstruction(StaticFieldResolver fieldResolver)
            : base(OpCodes.Ldfld)
        {
            this.fieldResolver = fieldResolver;
        }

        public override string ToString()
        {
            return "Ldsflda " + fieldResolver.ToString();
        }
    }

    class LdstrInstruction : ILInstruction
    {
        public readonly String String;
        public LdstrInstruction(string str)
            : base(OpCodes.Ldstr)
        {
            String = str;
        }

        public override string ToString()
        {
            return "Ldstr \"" + String + "\"";
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitString(String);
        } 
    }

    class LdtokenInstruction : ILInstruction
    {
        public readonly StaticTypeResolver TypeResolver;
        public LdtokenInstruction(StaticTypeResolver typeResolver)
            : base(OpCodes.Ldtoken)
        {
            TypeResolver = typeResolver;
        }

        public readonly StaticMethodResolver MethodResolver;
        public LdtokenInstruction(StaticMethodResolver methodResolver)
            : base(OpCodes.Ldtoken)
        {
            MethodResolver = methodResolver;
        }

        public readonly StaticFieldResolver FieldResolver;
        public LdtokenInstruction(StaticFieldResolver fieldResolver)
            : base(OpCodes.Ldtoken)
        {
            FieldResolver = fieldResolver;
        }

        public override string ToString()
        {
            if (TypeResolver != null)
                return "Ldtoken " + TypeResolver.ToString();
            else if (MethodResolver != null)
                return "Ldtoken " + MethodResolver.ToString();
            else
                return "Ldtoken " + FieldResolver.ToString();
        }
    }


    class LdvirtftnInstruction : ILInstruction
    {
        public readonly StaticMethodResolver MethodResolver;
        public LdvirtftnInstruction(StaticMethodResolver methodResolver)
            : base(OpCodes.Ldvirtftn)
        {
            MethodResolver = methodResolver;
        }

        public override string ToString()
        {
            return "Ldvirtftn " + MethodResolver.ToString();
        }
    }


    class LeaveInstruction : TransferILInstruction
    {
        public LeaveInstruction(string label)
            : base(OpCodes.Leave, label) { }
    }

    class LocallocInstruction : SimpleILInstruction
    {
        public LocallocInstruction()
            : base(OpCodes.Localloc)
        {

        }
    }

    class MkrefanyInstruction : TypedILInstruction
    {
        public MkrefanyInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Mkrefany, typeName)
        {

        }
    }

    class MulInstruction : SimpleILInstruction
    {
        public MulInstruction()
            : base(OpCodes.Mul)
        {

        }
    }

    class Mul_OvfInstruction : SimpleILInstruction
    {
        public Mul_OvfInstruction()
            : base(OpCodes.Mul_Ovf)
        {

        }
    }

    class Mul_Ovf_UnInstruction : SimpleILInstruction
    {
        public Mul_Ovf_UnInstruction()
            : base(OpCodes.Mul_Ovf_Un)
        {

        }
    }

    class NegInstruction : SimpleILInstruction
    {
        public NegInstruction()
            : base(OpCodes.Neg)
        {

        }
    }

    class NewarrInstruction : TypedILInstruction
    {
        public NewarrInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Newarr, typeName)
        {

        }
    }

    class NewobjInstruction : ILInstruction
    {
        public readonly StaticConstructorResolver Ctor;
        public NewobjInstruction(StaticConstructorResolver ctor)
            : base(OpCodes.Newobj)
        {
            Ctor = ctor;
        }

        public override string ToString()
        {
            return "Newobj " + Ctor.ToString();
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitNew(Ctor.Constructor);
        }
    }

    class NopInstruction : SimpleILInstruction
    {
        public NopInstruction()
            : base(OpCodes.Nop)
        {

        }
    }

    class NotInstruction : SimpleILInstruction
    {
        public NotInstruction()
            : base(OpCodes.Not)
        {

        }
    }

    class OrInstruction : SimpleILInstruction
    {
        public OrInstruction()
            : base(OpCodes.Or)
        {

        }
    }

    class PopInstruction : SimpleILInstruction
    {
        public PopInstruction()
            : base(OpCodes.Pop)
        {

        }
    }

    class ReadonlyInstruction : SimpleILInstruction
    {
        public ReadonlyInstruction()
            : base(OpCodes.Readonly)
        {

        }
    }

    class RefanytypeInstruction : SimpleILInstruction
    {
        public RefanytypeInstruction()
            : base(OpCodes.Refanytype)
        {

        }
    }

    class RefanyvalInstruction : TypedILInstruction
    {
        public RefanyvalInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Refanyval, typeName)
        {

        }
    }

    class RemInstruction : SimpleILInstruction
    {
        public RemInstruction()
            : base(OpCodes.Rem)
        {

        }
    }

    class Rem_UnInstruction : SimpleILInstruction
    {
        public Rem_UnInstruction()
            : base(OpCodes.Rem_Un)
        {

        }
    }

    class RetInstruction : SimpleILInstruction
    {
        public RetInstruction()
            : base(OpCodes.Ret)
        {

        }
    }

    class RethrowInstruction : SimpleILInstruction
    {
        public RethrowInstruction()
            : base(OpCodes.Rethrow)
        {

        }
    }

    class ShlInstruction : SimpleILInstruction
    {
        public ShlInstruction()
            : base(OpCodes.Shl)
        {

        }
    }

    class ShrInstruction : SimpleILInstruction
    {
        public ShrInstruction()
            : base(OpCodes.Shr)
        {

        }
    }

    class Shr_UnInstruction : SimpleILInstruction
    {
        public Shr_UnInstruction()
            : base(OpCodes.Shr_Un)
        {

        }
    }

    class SizeofInstruction : TypedILInstruction
    {
        public SizeofInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Sizeof, typeName)
        {
        }
    }

    class StargInstruction : ILInstruction
    {
        public readonly short ArgumentSlot;
        public StargInstruction(short arg_slot)
            : base(OpCodes.Starg)
        {
            ArgumentSlot = arg_slot;
        }

        public override string ToString()
        {
            return "Starg " + ArgumentSlot;
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            if (ArgumentSlot < 256)
                ILGen.Emit(OpCodes.Starg_S, ArgumentSlot);
            else
                ILGen.Emit(OpCodes.Starg, ArgumentSlot);
        }
    }

    class StelemInstruction : TypedILInstruction
    {
        public StelemInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Stelem, typeName)
        {

        }
    }

    class Stelem_IInstruction : SimpleILInstruction
    {
        public Stelem_IInstruction()
            : base(OpCodes.Stelem_I)
        {

        }
    }
    class Stelem_I1Instruction : SimpleILInstruction
    {
        public Stelem_I1Instruction()
            : base(OpCodes.Stelem_I1)
        {

        }
    }
    class Stelem_I2Instruction : SimpleILInstruction
    {
        public Stelem_I2Instruction()
            : base(OpCodes.Stelem_I2)
        {

        }
    }

    class Stelem_I4Instruction : SimpleILInstruction
    {
        public Stelem_I4Instruction()
            : base(OpCodes.Stelem_I4)
        {

        }
    }

    class Stelem_I8Instruction : SimpleILInstruction
    {
        public Stelem_I8Instruction()
            : base(OpCodes.Stelem_I8)
        {

        }
    }

    class Stelem_R4Instruction : SimpleILInstruction
    {
        public Stelem_R4Instruction()
            : base(OpCodes.Stelem_R4)
        {

        }
    }

    class Stelem_R8Instruction : SimpleILInstruction
    {
        public Stelem_R8Instruction()
            : base(OpCodes.Stelem_R8)
        {

        }
    }

    class Stelem_RefInstruction : SimpleILInstruction
    {
        public Stelem_RefInstruction()
            : base(OpCodes.Stelem_Ref)
        {

        }
    }

    class StfldInstruction : ILInstruction
    {
        StaticFieldResolver fieldResolver;

        public StfldInstruction(StaticFieldResolver fieldResolver)
            : base(OpCodes.Stfld)
        {
            this.fieldResolver = fieldResolver;
        }

        public override string ToString()
        {
            return "Stfld " + fieldResolver.ToString();
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            FieldInfo field = fieldResolver.Field;

            if ((field.Attributes & FieldAttributes.Static) == FieldAttributes.Static)
            {
                throw new FormattedException("field " + field + " is not an instance field. Cannot be argument for Stfld opcode");
            }

            ILGen.Emit(OpCodes.Stfld, field);
        }
    }

    class Stind_IInstruction : SimpleILInstruction
    {
        public Stind_IInstruction()
            : base(OpCodes.Stind_I)
        {

        }
    }

    class Stind_I1Instruction : SimpleILInstruction
    {
        public Stind_I1Instruction()
            : base(OpCodes.Stind_I1)
        {

        }
    }

    class Stind_I2Instruction : SimpleILInstruction
    {
        public Stind_I2Instruction()
            : base(OpCodes.Stind_I2)
        {

        }
    }

    class Stind_I4Instruction : SimpleILInstruction
    {
        public Stind_I4Instruction()
            : base(OpCodes.Stind_I4)
        {

        }
    }

    class Stind_I8Instruction : SimpleILInstruction
    {
        public Stind_I8Instruction()
            : base(OpCodes.Stind_I8)
        {

        }
    }

    class Stind_R4Instruction : SimpleILInstruction
    {
        public Stind_R4Instruction()
            : base(OpCodes.Stind_R4)
        {

        }
    }

    class Stind_R8Instruction : SimpleILInstruction
    {
        public Stind_R8Instruction()
            : base(OpCodes.Stind_R8)
        {

        }
    }

    class Stind_RefInstruction : SimpleILInstruction
    {
        public Stind_RefInstruction()
            : base(OpCodes.Stind_Ref)
        {

        }
    }

    class StlocInstruction : ILInstruction
    {
        public readonly Nullable<int> LocalNumber;
        public readonly Symbol SlotName;
        public StlocInstruction(int var_num)
            : base(OpCodes.Stloc)
        {
            LocalNumber = var_num;
        }

        public StlocInstruction(Symbol slotName)
            : base(OpCodes.Stloc)
        {
            SlotName = slotName;
        }

        public StlocInstruction(VariableDeclaration var)
            :base(OpCodes.Stloc)
        {
            LocalNumber = var.Id;
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitLocalSet(LocalNumber.Value);
        }

        public override string ToString()
        {
            return "Stloc " + LocalNumber.ToString();
        }
    }

    class StobjInstruction : TypedILInstruction
    {
        public StobjInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Stobj, typeName)
        {

        }
    }

    class StsfldInstruction : ILInstruction
    {
        StaticFieldResolver fieldResolver;

        public StsfldInstruction(StaticFieldResolver fieldResolver)
            : base(OpCodes.Stsfld)
        {
            this.fieldResolver = fieldResolver;
        }

        public override string ToString()
        {
            return "Stsfld " + fieldResolver.ToString();
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            FieldInfo field = fieldResolver.Field;

            if ((field.Attributes & FieldAttributes.Static) != FieldAttributes.Static)
            {
                throw new FormattedException("field " + field + " is not a static field. Cannot be argument for Stsfld opcode");
            }

            ILGen.Emit(OpCodes.Stsfld, field);
        }
    }

    class SubInstruction : SimpleILInstruction
    {
        public SubInstruction()
            : base(OpCodes.Sub)
        {

        }
    }

    class Sub_OvfInstruction : SimpleILInstruction
    {
        public Sub_OvfInstruction()
            : base(OpCodes.Sub_Ovf)
        {

        }
    }

    class Sub_Ovf_UnInstruction : SimpleILInstruction
    {
        public Sub_Ovf_UnInstruction()
            : base(OpCodes.Sub_Ovf_Un)
        {

        }
    }


    class SwitchInstruction : ILInstruction
    {
        public readonly LabelDeclaration[] Labels;

        public SwitchInstruction(LabelDeclaration[] labels)
            :base(OpCodes.Switch)
        {
            Labels = labels;
        }

        public override string ToString()
        {
            return "Switch (" + PrintLabels() + ")";
        }

        private string PrintLabels()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Labels.Length - 1; i++)
            {
                sb.Append(Labels[i] + ", ");
            }

            if (Labels.Length > 0)
                sb.Append(Labels[Labels.Length - 1]);

            return sb.ToString();
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            Label[] rlabels = new Label[Labels.Length];

            for (int i = 0; i < Labels.Length; i++)
            {
                rlabels[i] = Labels[i].Label;
            }

            ILGen.Emit(OpCodes.Switch, rlabels);
        }
    }


    class TailcallInstruction : SimpleILInstruction
    {
        public TailcallInstruction()
            : base(OpCodes.Tailcall)
        {

        }
    }


    class ThrowInstruction : SimpleILInstruction
    {
        public ThrowInstruction()
            : base(OpCodes.Throw)
        {

        }
    }


    class UnalignedInstruction : ILInstruction
    {
        public readonly Nullable<long> LocalNumber;
        public readonly Symbol local;
        public UnalignedInstruction(long var_num)
            : base(OpCodes.Unaligned)
        {
            LocalNumber = var_num;
        }

        public UnalignedInstruction(Symbol local)
            : base(OpCodes.Unaligned)
        {
            this.local = local;
        }

        public override string ToString()
        {
            return "Unaligned " + LocalNumber;
        }
    }

    class UnboxInstruction : TypedILInstruction
    {
        public UnboxInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Unbox, typeName)
        {

        }
    }

    class Unbox_AnyInstruction : TypedILInstruction
    {
        public Unbox_AnyInstruction(StaticTypeResolver typeName)
            : base(OpCodes.Unbox_Any, typeName)
        {

        }
    }

    class VolatileInstruction : SimpleILInstruction
    {
        public VolatileInstruction()
            : base(OpCodes.Volatile)
        {

        }
    }

    class XorInstruction : SimpleILInstruction
    {
        public XorInstruction()
            : base(OpCodes.Xor)
        {

        }
    }


    class LboxInstruction : TypedILInstruction
    {
        public LboxInstruction(StaticTypeResolver typeName)
            : base(default(OpCode), typeName)
        {
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitLispBox(TypeResolver.Type);
        }

        public override string ToString()
        {
            return "Lbox " + TypeResolver.ToString();
        }
    }

    class LunboxInstruction : TypedILInstruction
    {
        public LunboxInstruction(StaticTypeResolver typeName)
            : base(default(OpCode), typeName)
        {
        }

        public override string ToString()
        {
            return "Lunbox " + TypeResolver.ToString();
        }
    }

    class LdnilInstruction : SimpleILInstruction
    {
        public LdnilInstruction()
            : base(default(OpCode))
        {

        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitNIL();
        }

        public override string ToString()
        {
            return "Ldnil";
        }
    }

    class LdtInstruction : SimpleILInstruction
    {
        public LdtInstruction()
            : base(default(OpCode))
        {

        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitT();
        }

        public override string ToString()
        {
            return "Ldt";
        }
    }


    class PushInstruction : ILInstruction
    {
        public readonly object Constant;
        public PushInstruction(object constant)
            : base(default(OpCode))
        {
            Constant = constant;
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EmitConstant(Constant);
        }

        

        public override string ToString()
        {
            return "Push " + Constant;
        }
    }

    class MarkLabelInstruction : ILInstruction
    {
        public readonly LabelDeclaration Label;

        public MarkLabelInstruction(LabelDeclaration label)
            : base(default(OpCode))
        {
            Label = label;
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.MarkLabel(Label.Label);
        }

        public override string ToString()
        {
            return Label.Name + ":";
        }
    }

    class BeginExceptionBlock : ILInstruction
    {
            public LabelDeclaration Label;

            public BeginExceptionBlock(LabelDeclaration label)
            : base(default(OpCode))
        {
            Label = label;
        }

            public BeginExceptionBlock()
                : base(default(OpCode))
            {

            }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            if (Label == null)
                Label = new LabelDeclaration(iblock.NewUniqueLabelName());

            Label.Label = ILGen.BeginExceptionBlock();
            try
            {
                iblock.Labels.Add(Label);
            }
            catch (ArgumentException)
            {
                throw new SimpleErrorException("It's look like u mark exception block end label before");
            }
        }

        public override string ToString()
        {
            return ".try " + Label.Name;
        }
    }


    class BeginCatchBlock : TypedILInstruction
    {
        public BeginCatchBlock(StaticTypeResolver typeName)
            : base(default(OpCode), typeName)
        {

        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.BeginCatchBlock(TypeResolver.Type);
        }

        public override string ToString()
        {
            return ".catch " + TypeResolver.TypeName;
        }
    }

    class BeginFinallyBlock : ILInstruction
    {
        public BeginFinallyBlock()
            :base(default(OpCode))
        {
        }

        public override string ToString()
        {
            return ".finally";
        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.BeginFinallyBlock();
        } 
    }

    class EndExceptionBlock : ILInstruction
    {
        public EndExceptionBlock()
            : base(default(OpCode))
        {

        }

        public override void Emit(InstructionsBlock iblock, ILGenerator ILGen)
        {
            ILGen.EndExceptionBlock();
        }

        public override string ToString()
        {
            return ".endtry";
        }
    } 

}
