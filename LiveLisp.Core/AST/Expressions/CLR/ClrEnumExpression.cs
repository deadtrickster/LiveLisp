namespace LiveLisp.Core.AST.Expressions
{
    using LiveLisp.Core.AST;
    using System;
    using System.Collections.Generic;
    using LiveLisp.Core.Types;
    using System.Linq;
    using LiveLisp.Core.Reader;
    using System.Reflection;
    using LiveLisp.Core.Compiler;
    using System.Reflection.Emit;
    public class ClrEnumExpression : ClrClassExpression
    {
     /*   private Type baseType;
        private List<LiveLisp.Pair<String, Object>> enumMembers;
        private string name;
        private TypeAttributes tattrs;*/

        public ClrEnumExpression(ExpressionContext context)
            : base(context)
        {
            //this.enumMembers = new List<LiveLisp.Pair<String, Object>>();
        }
/*
        public static Expression Build(Cons cons, int level, StaticScope scope, Expression parent)
        {
            if (!(cons.Cdr is Cons))
            {
                throw new Exception("CLR::defenum: form is not a list.");
            }
            Queue<object> items = new Queue<object>((cons.Cdr as Cons).ToArray());
            State currentState = State.Start;
            ClrEnumExpression ret = new ClrEnumExpression(level, scope, parent);
            while (items.Count != 0)
            {
                string name;
                string baseTypeName;
                Type baseType;
                Pair<String, Object> member;
                object membernameproto;
                string memberName;
                object item = items.Dequeue();
                switch (currentState)
                {
                    case State.Start:
                        if (!(item is KeywordSymbol))
                        {
                            break;
                        }
                        ret.tattrs |= (TypeAttributes)Enum.Parse(typeof(TypeAttributes), ((KeywordSymbol)item).Name, true);
                        goto Label_022A;

                    case State.EnumNameReached:
                        if (item != DefinedSymbols.KWStore)
                        {
                            goto Label_00F5;
                        }
                        currentState = State.EnumBaseTypeExpected;
                        goto Label_022A;

                    case State.EnumBaseTypeExpected:
                        if (!DynamicTypesHelper.GetStringFromDesignator(item, out baseTypeName))
                        {
                            throw new Exception("clr::defenum: enum base type expected.");
                        }
                        goto Label_0124;

                    case State.EnumBaseReached:
                        goto Label_0168;

                    default:
                        goto Label_022A;
                }
                if (!DynamicTypesHelper.GetStringFromDesignator(item, out name))
                {
                    throw new Exception("clr::defenum: enum name not found. string designator expected");
                }
                ret.name = name;
                currentState = State.EnumNameReached;
                goto Label_022A;
            Label_00F5:
                currentState = State.EnumBaseReached;
                ret.baseType = typeof(int);
                goto Label_0168;
            Label_0124:
                baseType = TypeCache.Instance.FindType(baseTypeName);
                if (!Validator.ValidateBaseEnumType(baseType))
                {
                    throw new Exception("clr::defenum: type " + baseType + " is unvalid base type for enum.");
                }
                ret.baseType = baseType;
                currentState = State.EnumBaseReached;
                goto Label_022A;
            Label_0168:
                member = new Pair<String, Object>();
                if (item is Cons)
                {
                    membernameproto = (item as Cons).Car;
                    object val = (item as Cons).Second;
                    if (!DynamicTypesHelper.IsImplicitlyConvertibleNumeric(val.GetType(), ret.baseType))
                    {
                        try
                        {
                            Convert.ChangeType(val, ret.baseType);
                        }
                        catch (InvalidCastException)
                        {
                            throw new Exception(string.Format("clr::defenum: object {0} can't be implicitly converted to base enum type {1}", val, ret.baseType));
                        }
                    }
                    member.Second = val;
                }
                else
                {
                    membernameproto = item;
                }
                if (!DynamicTypesHelper.GetStringFromDesignator(membernameproto, out memberName))
                {
                    throw new Exception("clr::defenum: enum member name is not a string designator.");
                }
                member.First = memberName;
                ret.enumMembers.Add(member);
            Label_022A: ;
            }
            return ret;
        }

        private void EmitByteEnum(EnumBuilder eb)
        {
            byte counter = 0;
            foreach (Pair<String, Object> item in this.enumMembers)
            {
                if (counter == 0xff)
                {
                    this.ThrowCantFit();
                }
                if (item.Second != null)
                {
                    counter = (byte)item.Second;
                }
                eb.DefineLiteral(item.First, counter);
                counter = (byte)(counter + 1);
            }
        }

        private void EmitInt16Enum(EnumBuilder eb)
        {
            short counter = 0;
            foreach (Pair<String, Object> item in this.enumMembers)
            {
                if (counter == 0x7fff)
                {
                    this.ThrowCantFit();
                }
                if (item.Second != null)
                {
                    counter = (short)item.Second;
                }
                eb.DefineLiteral(item.First, counter);
                counter = (short)(counter + 1);
            }
        }

        private void EmitInt32Enum(EnumBuilder eb)
        {
            int counter = 0;
            foreach (Pair<String, Object> item in this.enumMembers)
            {
                if (counter == 0x7fffffff)
                {
                    this.ThrowCantFit();
                }
                if (item.Second != null)
                {
                    counter = (int)item.Second;
                }
                eb.DefineLiteral(item.First, counter);
                counter++;
            }
        }

        private void EmitInt64Enum(EnumBuilder eb)
        {
            long counter = 0L;
            foreach (Pair<String, Object> item in this.enumMembers)
            {
                if (counter == 0x7fffffffffffffffL)
                {
                    this.ThrowCantFit();
                }
                if (item.Second != null)
                {
                    counter = (long)item.Second;
                }
                eb.DefineLiteral(item.First, counter);
                counter += 1L;
            }
        }

        private void EmitSByteEnum(EnumBuilder eb)
        {
            sbyte counter = 0;
            foreach (Pair<String, Object> item in this.enumMembers)
            {
                if (counter == 0x7f)
                {
                    this.ThrowCantFit();
                }
                if (item.Second != null)
                {
                    counter = (sbyte)item.Second;
                }
                eb.DefineLiteral(item.First, counter);
                counter = (sbyte)(counter + 1);
            }
        }

        private void EmitUInt16Enum(EnumBuilder eb)
        {
            ushort counter = 0;
            foreach (Pair<String, Object> item in this.enumMembers)
            {
                if (counter == 0xffff)
                {
                    this.ThrowCantFit();
                }
                if (item.Second != null)
                {
                    counter = (ushort)item.Second;
                }
                eb.DefineLiteral(item.First, counter);
                counter = (ushort)(counter + 1);
            }
        }

        private void EmitUInt32Enum(EnumBuilder eb)
        {
            uint counter = 0;
            foreach (Pair<String, Object> item in this.enumMembers)
            {
                if (counter == uint.MaxValue)
                {
                    this.ThrowCantFit();
                }
                if (item.Second != null)
                {
                    counter = (uint)item.Second;
                }
                eb.DefineLiteral(item.First, counter);
                counter++;
            }
        }

        private void EmitUInt64Enum(EnumBuilder eb)
        {
            ulong counter = 0L;
            foreach (Pair<String, Object> item in this.enumMembers)
            {
                if (counter == ulong.MaxValue)
                {
                    this.ThrowCantFit();
                }
                if (item.Second != null)
                {
                    counter = (ulong)item.Second;
                }
                eb.DefineLiteral(item.First, counter);
                counter += (ulong)1L;
            }
        }

        private void ThrowCantFit()
        {
            throw new Exception(string.Format("defenum {0}: the enumerator value is too large to fit in its type({1})", this.name, this.baseType));
        }

        public override void Visit(IASTVisiter visiter)
        {
            EnumBuilder eb;
            try
            {
                eb = DelegatesCache.moduleBuilder.DefineEnum(this.name, this.tattrs, this.baseType);
            }
            catch (ArgumentException)
            {
                eb = DelegatesCache.moduleBuilder.DefineEnum(this.name + ClrTypeExpression.counter, this.tattrs, this.baseType);
                ClrTypeExpression.counter++;
            }
            switch (Type.GetTypeCode(this.baseType))
            {
                case TypeCode.SByte:
                    this.EmitSByteEnum(eb);
                    break;

                case TypeCode.Byte:
                    this.EmitByteEnum(eb);
                    break;

                case TypeCode.Int16:
                    this.EmitInt16Enum(eb);
                    break;

                case TypeCode.UInt16:
                    this.EmitUInt16Enum(eb);
                    break;

                case TypeCode.Int32:
                    this.EmitInt32Enum(eb);
                    break;

                case TypeCode.UInt32:
                    this.EmitUInt32Enum(eb);
                    break;

                case TypeCode.Int64:
                    this.EmitInt64Enum(eb);
                    break;

                case TypeCode.UInt64:
                    this.EmitUInt64Enum(eb);
                    break;
            }
            Type GeneratedEnum = eb.CreateType();
            TypeCache.RedefineAlias(this.name, GeneratedEnum);
            TypeCache.DynamicGeneratedTypes.Add(GeneratedEnum.GUID);
            visiter.Emitter.EmitType(GeneratedEnum);
        }

        private enum State
        {
            Start,
            EnumNameReached,
            EnumBaseTypeExpected,
            EnumBaseReached
        }*/
    }
}

