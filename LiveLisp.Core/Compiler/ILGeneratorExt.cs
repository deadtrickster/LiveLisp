using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Reflection;
using LiveLisp.Core;
using LiveLisp.Core.Types;
using LiveLisp.Core.BuiltIns.Numbers;
using LiveLisp.Core.CLOS;
namespace LiveLisp.Core.Compiler
{
    public static class ILGen
    {
        public static void EmitConstant(this ILGenerator Real, object constant)
        {
            if (constant == null)
            {
                Real.Emit(OpCodes.Ldnull);
                return;
            }

            switch (Type.GetTypeCode(constant.GetType()))
            {
                case TypeCode.Boolean:
                    EmitBoolean(Real, (Boolean)constant);
                    break;
                case TypeCode.Byte:
                    EmitByte(Real, (byte)constant);
                    break;
                case TypeCode.Char:
                    EmitChar(Real, (Char)constant);
                    break;
                case TypeCode.Decimal:
                    EmitDecimal(Real, (decimal)constant);
                    break;
                case TypeCode.Double:
                    EmitDouble(Real, (double)constant);
                    break;
                case TypeCode.Int16:
                    EmitShort(Real, (short)constant);
                    break;
                case TypeCode.Int32:
                    EmitInt32(Real, (Int32)constant);
                    break;
                case TypeCode.Int64:
                    EmitInt64(Real, (Int64)constant);
                    break;
                case TypeCode.SByte:
                    EmitSByte(Real, (sbyte)constant);
                    break;
                case TypeCode.Single:
                    EmitSingle(Real, (float)constant);
                    break;
                case TypeCode.String:
                    EmitString(Real, constant as string);
                    break;
                case TypeCode.UInt16:
                    EmitUShort(Real, (ushort)constant);
                    break;
                case TypeCode.UInt32:
                    EmitUInt32(Real, (UInt32)constant);
                    break;
                case TypeCode.UInt64:
                    EmitUInt64(Real, (UInt64)constant);
                    break;
                default:
                    EmitCustomTypeConstant(Real, constant);
                    break;
            }
        }

        static MethodInfo GetSymbolMethod = typeof(SymbolTable).GetMethod("GetSymbol", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(int) }, null);

        public static void EmitSymbol(this ILGenerator Real, Symbol symbol)
        {
            EmitInt32(Real, symbol.Id);
            EmitCall(Real, GetSymbolMethod);
        }

        public static void EmitBoolean(this ILGenerator Real, bool value)
        {
            if (value)
            {
                Real.Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                Real.Emit(OpCodes.Ldc_I4_0);
            }
        }

        public static void EmitChar(this ILGenerator Real, char value)
        {
            EmitInt32(Real, value);
            Real.Emit(OpCodes.Conv_U2);
        }

        public static void EmitRet(this ILGenerator Real)
        {
            Real.Emit(OpCodes.Ret);
        }

        public static void EmitPop(this ILGenerator Real)
        {
            Real.Emit(OpCodes.Pop);
        }

        public static void MarkLabel(this ILGenerator Real, Label label)
        {
            Real.MarkLabel(label);
        }

        public static Label DefineLabel(this ILGenerator Real)
        {
            return Real.DefineLabel();
        }

        public static void BeginLocalScope(this ILGenerator Real)
        {
            throw new NotImplementedException();
        }

        public static void EndLocalScope(this ILGenerator Real)
        {
            throw new NotImplementedException();
        }

        public static void JmpToLabel(this ILGenerator Real, Label label)
        {
            Real.Emit(OpCodes.Br, label);
        }

        public  static LocalBuilder DeclareLocal(this ILGenerator Real)
        {
            return DeclareLocal(Real, typeof(Object));
        }
        public static LocalBuilder DeclareLocal(this ILGenerator Real, Type type)
        {
            return Real.DeclareLocal(type);
        }

        public static void GetLocal(this ILGenerator Real, LocalBuilder first_form_result)
        {
            Real.Emit(OpCodes.Ldloc, first_form_result);
        }

        #region Fields, properties and methods

        public static void EmitPropertyGet(this ILGenerator Real, Type type, string name)
        {
            PropertyInfo pi = type.GetProperty(name);
            EmitPropertyGet(Real, pi);
        }

        public static void EmitPropertyGet(this ILGenerator Real, PropertyInfo pi)
        {
            if (!pi.CanRead)
            {
                throw new InvalidOperationException();
            }

            Real.Emit(OpCodes.Callvirt, pi.GetGetMethod());
        }

        public static void EmitPropertySet(this ILGenerator Real, Type type, string name)
        {
            PropertyInfo pi = type.GetProperty(name);

            EmitPropertySet(Real, pi);
        }

        public static void EmitPropertySet(this ILGenerator Real, PropertyInfo pi)
        {
            if (!pi.CanWrite)
            {
                throw new InvalidOperationException();
            }

            Real.Emit(OpCodes.Callvirt, pi.GetSetMethod());
        }

        public static void EmitFieldAddress(this ILGenerator Real, FieldInfo fi)
        {
            if (fi.IsStatic)
            {
                Real.Emit(OpCodes.Ldsflda, fi);
            }
            else
            {
                Real.Emit(OpCodes.Ldflda, fi);
            }
        }

        public static void EmitFieldGet(this ILGenerator Real, Type type, String name)
        {

            FieldInfo fi = type.GetField(name);
            EmitFieldGet(Real, fi);
        }

        public static void EmitFieldSet(this ILGenerator Real, Type type, String name)
        {
            FieldInfo fi = type.GetField(name);
            EmitFieldSet(Real, fi);
        }

        public static void EmitFieldGet(this ILGenerator Real, FieldInfo fi)
        {
            if (fi.IsStatic)
            {
                Real.Emit(OpCodes.Ldsfld, fi);
            }
            else
            {
                Real.Emit(OpCodes.Ldfld, fi);
            }
        }

        public static void EmitFieldSet(this ILGenerator Real, FieldInfo fi)
        {
            if (fi.IsStatic)
            {
                Real.Emit(OpCodes.Stsfld, fi);
            }
            else
            {
                Real.Emit(OpCodes.Stfld, fi);
            }
        }

        public static void EmitNew(this ILGenerator Real, ConstructorInfo ci)
        {
            if (ci.DeclaringType.ContainsGenericParameters)
            {
                throw new ArgumentException();
            }

            Real.Emit(OpCodes.Newobj, ci);
        }

        public static void EmitNew(this ILGenerator Real, Type type, Type[] paramTypes)
        {
            ConstructorInfo ci = type.GetConstructor(paramTypes);
            EmitNew(Real, ci);
        }

        public static void EmitCall(this ILGenerator Real, MethodInfo mi)
        {
            if (/*mi.IsVirtual && */!mi.DeclaringType.IsValueType && !mi.IsStatic)
            {
                Real.Emit(OpCodes.Callvirt, mi);
            }
            else
            {
               Real.Emit(OpCodes.Call, mi);
            }
        }

        public static void EmitCall(this ILGenerator Real, Type type, String name)
        {
            if (!type.IsVisible)
            {
                throw new ArgumentException();
            }

            MethodInfo mi = type.GetMethod(name);

            EmitCall(Real, mi);
        }

        public static void EmitCall(this ILGenerator Real, Type type, String name, Type[] paramTypes)
        {
            MethodInfo mi = type.GetMethod(name, paramTypes);
            EmitCall(Real, mi);
        }

        #endregion

        #region Helpers

        public static bool EmitDynamicResources = false;

        public static ModuleBuilder MainModuleBuilder
        {
            get;
            set;
        }

        public static void EmitCustomTypeConstant(this ILGenerator Real, object constant)
        {
         /*   int resid;
            //if (EmitDynamicResources)
            //{
            resid = ResourceHelper.AddDynamicResource(o);
            //}
            //else
            //{
            //   resid =ResourceHelper.AddStaticResource(o, MainModuleBuilder);
            //}

            // EmitType(o.GetType());
            EmitInt32(Real, resid);
            EmitCall(Real, typeof(ResourceHelper).GetMethod("GetConstant"));*/

            ILispNumber lnum = constant as ILispNumber;
            if (lnum != null)
            {
                switch (lnum.NumberType)
                {
                    case NumberType.Byte:
                        EmitByte(Real, lnum.Byte);
                        break;
                    case NumberType.Int:
                        EmitInt32(Real, lnum.Int32);
                        break;
                    case NumberType.BigInt:
                        EmitBigInteger(Real, lnum.BigInteger);
                        break;
                    case NumberType.Long:
                        EmitInt64(Real, lnum.Int64);
                        break;
                    case NumberType.Ratio:
                        EmitRatio(Real, lnum.Ratio);
                        break;
                    case NumberType.Single:
                        EmitSingle(Real, lnum.Single);
                        break;
                    case NumberType.Double:
                        EmitDouble(Real, lnum.Double);
                        break;
                    case NumberType.Complex:
                        EmitComplex(Real, lnum.Complex);
                        break;
                    case NumberType.Decimal:
                        EmitDecimal(Real, lnum.Decimal);
                        break;
                    default:
                        break;
                }
                return;
            }


            ILispObject lobj = constant as ILispObject;


            if (constant == DefinedSymbols.NIL)
            {
                EmitNIL(Real);
                return;
            }
            if (constant == DefinedSymbols.T)
            {
                EmitT(Real);
                return;
            }
            if (constant == null)
            {
                Real.Emit(OpCodes.Ldnull);
                return;
            }
            if (constant is Symbol)
            {
                EmitSymbol(Real, constant as Symbol);
                return;
            }
            if (constant is Cons)
            {
                EmitCons(Real, constant as Cons);
                return;
            }

            if (constant is Guid)
            {
                EmitGuid(Real, (Guid)constant);
                return;
            }

            throw new NotImplementedException();

        }

        private static void EmitGuid(this ILGenerator Real, Guid guid)
        {
            byte[] byte_arr = guid.ToByteArray();

            EmitArray(Real, byte_arr);

            Real.EmitNew(typeof(Guid).GetConstructor(new Type[] { typeof(byte[]) }));
        }

        private static void EmitArray<T>(this ILGenerator Real, T[] array)
        {
            var loc = Real.DeclareLocal(array.GetType());

            Real.EmitInt32(array.Length);
            Real.Emit(OpCodes.Newarr, (typeof(T)));

            Real.EmitLocalSet(loc);

            for (int i = 0; i < array.Length; i++)
            {
                Real.EmitLocalGet(loc);
                Real.EmitInt32(i);

                Real.EmitConstant(array[i]);
                //        if (exps[i].ReturnType.IsValueType)
                //            ILGen.Emit(OpCodes.Box, exps[i].ReturnType);

                Real.EmitStoreElement(typeof(T));
            }

            Real.EmitLocalGet(loc);
        }

        private static void EmitComplex(ILGenerator Real, Complex complex)
        {
            throw new NotImplementedException();
        }

        private static void EmitRatio(ILGenerator Real, Ratio ratio)
        {
            throw new NotImplementedException();
        }

        private static void EmitBigInteger(ILGenerator Real, BigInteger bigInteger)
        {
            throw new NotImplementedException();
        }

        private static void EmitCons(this ILGenerator Real, Cons cons)
        {
            var cons_slot = Real.DeclareLocal(typeof(Cons));
            Real.Emit(OpCodes.Ldnull);
            EmitNew(Real, typeof(Cons), new Type[] { typeof(object) });
            EmitLocalSet(Real, cons_slot);

            var current_slot = Real.DeclareLocal(typeof(Cons));
            EmitLocalGet(Real, cons_slot);
            EmitLocalSet(Real, current_slot);

           Next:
            EmitLocalGet(Real, current_slot);
            EmitConstant(Real, cons.Car);
            EmitLispBox(Real, cons.Car.BoxType());
            EmitNew(Real, typeof(Cons), new Type[] { typeof(object) });
            EmitPropertySet(Real, typeof(Cons), "Cdr");

            EmitLocalGet(Real, current_slot);
            EmitPropertyGet(Real, typeof(Cons), "Child");
            EmitLocalSet(Real, current_slot);

            if (cons.Child != null)
            {
                cons = cons.Child;
                goto Next;
            }
            else
            {
                EmitLocalGet(Real, current_slot);
                EmitConstant(Real, cons.Cdr);
                EmitLispBox(Real, cons.Cdr.BoxType());
                EmitPropertySet(Real, typeof(Cons), "Cdr");
            }

            EmitLocalGet(Real, cons_slot);
            EmitPropertyGet(Real, typeof(Cons), "Child");
        }



        public static void EmitLispBox(this ILGenerator ILGen, Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Boolean:
                    Label falseLabel = ILGen.DefineLabel();
                    Label endLabel = ILGen.DefineLabel();
                    ILGen.Emit(OpCodes.Brfalse, falseLabel);
                    ILGen.EmitT();
                    ILGen.JmpToLabel(endLabel);
                    ILGen.MarkLabel(falseLabel);
                    ILGen.EmitNIL();
                    ILGen.MarkLabel(endLabel);
                    break;
                case TypeCode.Byte:
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.DateTime:
                    break;
                case TypeCode.Decimal:
                    break;
                case TypeCode.Double:
                    break;
                case TypeCode.Empty:
                    break;
                case TypeCode.Int16:
                    break;
                case TypeCode.Int32:
                    ILGen.Emit(OpCodes.Box, typeof(Int32));
                    break;
                case TypeCode.Int64:
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.SByte:
                    break;
                case TypeCode.Single:
                    break;
                case TypeCode.String:
                    break;
                case TypeCode.UInt16:
                    break;
                case TypeCode.UInt32:
                    break;
                case TypeCode.UInt64:
                    break;
                default:
                    break;
            }
        }


        //CONFORMING
        internal static bool IsUnsigned(Type type)
        {
            type = GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.Char:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }



        internal static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        //CONFORMING
        internal static Type GetNullableType(Type type)
        {
            System.Diagnostics.Debug.Assert(type != null, "type cannot be null");
            if (type.IsValueType && !IsNullableType(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }

        //CONFORMING
        internal static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        //CONFORMING
        internal static bool IsFloatingPoint(Type type)
        {
            type = GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
                default:
                    return false;
            }
        }

        public static void EmitNumericConversion(this ILGenerator Real, Type typeFrom, Type typeTo, bool isChecked)
        {
            bool isFromUnsigned = IsUnsigned(typeFrom);
            bool isFromFloatingPoint = IsFloatingPoint(typeFrom);
            if (typeTo == typeof(Single))
            {
                if (isFromUnsigned)
                    Real.Emit(OpCodes.Conv_R_Un);
                Real.Emit(OpCodes.Conv_R4);
            }
            else if (typeTo == typeof(Double))
            {
                if (isFromUnsigned)
                    Real.Emit(OpCodes.Conv_R_Un);
                Real.Emit(OpCodes.Conv_R8);
            }
            else
            {
                TypeCode tc = Type.GetTypeCode(typeTo);
                if (isChecked)
                {
                    if (isFromUnsigned)
                    {
                        switch (tc)
                        {
                            case TypeCode.SByte:
                                Real.Emit(OpCodes.Conv_Ovf_I1_Un);
                                break;
                            case TypeCode.Int16:
                                Real.Emit(OpCodes.Conv_Ovf_I2_Un);
                                break;
                            case TypeCode.Int32:
                                Real.Emit(OpCodes.Conv_Ovf_I4_Un);
                                break;
                            case TypeCode.Int64:
                                Real.Emit(OpCodes.Conv_Ovf_I8_Un);
                                break;
                            case TypeCode.Byte:
                                Real.Emit(OpCodes.Conv_Ovf_U1_Un);
                                break;
                            case TypeCode.UInt16:
                            case TypeCode.Char:
                                Real.Emit(OpCodes.Conv_Ovf_U2_Un);
                                break;
                            case TypeCode.UInt32:
                                Real.Emit(OpCodes.Conv_Ovf_U4_Un);
                                break;
                            case TypeCode.UInt64:
                                Real.Emit(OpCodes.Conv_Ovf_U8_Un);
                                break;
                            default:
                                throw new InvalidCastException();
                        }
                    }
                    else
                    {
                        switch (tc)
                        {
                            case TypeCode.SByte:
                                Real.Emit(OpCodes.Conv_Ovf_I1);
                                break;
                            case TypeCode.Int16:
                                Real.Emit(OpCodes.Conv_Ovf_I2);
                                break;
                            case TypeCode.Int32:
                                Real.Emit(OpCodes.Conv_Ovf_I4);
                                break;
                            case TypeCode.Int64:
                                Real.Emit(OpCodes.Conv_Ovf_I8);
                                break;
                            case TypeCode.Byte:
                                Real.Emit(OpCodes.Conv_Ovf_U1);
                                break;
                            case TypeCode.UInt16:
                            case TypeCode.Char:
                                Real.Emit(OpCodes.Conv_Ovf_U2);
                                break;
                            case TypeCode.UInt32:
                                Real.Emit(OpCodes.Conv_Ovf_U4);
                                break;
                            case TypeCode.UInt64:
                                Real.Emit(OpCodes.Conv_Ovf_U8);
                                break;
                            default:
                                throw new InvalidCastException();
                        }
                    }
                }
                else
                {
                    if (isFromUnsigned)
                    {
                        switch (tc)
                        {
                            case TypeCode.SByte:
                            case TypeCode.Byte:
                                Real.Emit(OpCodes.Conv_U1);
                                break;
                            case TypeCode.Int16:
                            case TypeCode.UInt16:
                            case TypeCode.Char:
                                Real.Emit(OpCodes.Conv_U2);
                                break;
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                                Real.Emit(OpCodes.Conv_U4);
                                break;
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                Real.Emit(OpCodes.Conv_U8);
                                break;
                            default:
                                throw new InvalidCastException();
                        }
                    }
                    else
                    {
                        switch (tc)
                        {
                            case TypeCode.SByte:
                            case TypeCode.Byte:
                                Real.Emit(OpCodes.Conv_I1);
                                break;
                            case TypeCode.Int16:
                            case TypeCode.UInt16:
                            case TypeCode.Char:
                                Real.Emit(OpCodes.Conv_I2);
                                break;
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                                Real.Emit(OpCodes.Conv_I4);
                                break;
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                Real.Emit(OpCodes.Conv_I8);
                                break;
                            default:
                                throw new InvalidCastException();
                        }
                    }
                }
            }
        }

        public static void EmitByte(this ILGenerator Real, byte value)
        {
            EmitInt32(Real, value);
            Real.Emit(OpCodes.Conv_U1);
        }

        public static void EmitSByte(this ILGenerator Real, sbyte value)
        {
            EmitInt32(Real, value);
            Real.Emit(OpCodes.Conv_I1);
        }

        public static void EmitShort(this ILGenerator Real, short value)
        {
            EmitInt32(Real, value);
            Real.Emit(OpCodes.Conv_I2);
        }

        public static void EmitUShort(this ILGenerator Real, ushort value)
        {
            EmitInt32(Real, value);
            Real.Emit(OpCodes.Conv_U2);
        }

        public static void EmitDecimal(this ILGenerator Real, decimal value)
        {
            if (Decimal.Truncate(value) == value)
            {
                if (Int32.MinValue <= value && value <= Int32.MaxValue)
                {
                    int intValue = Decimal.ToInt32(value);
                    EmitInt32(Real, intValue);
                    EmitNew(Real, typeof(Decimal).GetConstructor(new Type[] { typeof(int) }));
                }
                else if (Int64.MinValue <= value && value <= Int64.MaxValue)
                {
                    long longValue = Decimal.ToInt64(value);
                    EmitInt64(Real, longValue);
                    EmitNew(Real, typeof(Decimal).GetConstructor(new Type[] { typeof(long) }));
                }
                else
                {
                    EmitDecimalBits(Real, value);
                }
            }
            else
            {
                EmitDecimalBits(Real, value);
            }
        }


        private static void EmitDecimalBits(ILGenerator Real, decimal value)
        {
            int[] bits = Decimal.GetBits(value);
            EmitInt32(Real, bits[0]);
            EmitInt32(Real, bits[1]);
            EmitInt32(Real, bits[2]);
            EmitBoolean(Real, (bits[3] & 0x80000000) != 0);
            EmitByte(Real, (byte)(bits[3] >> 16));
            EmitNew(Real, typeof(decimal).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) }));
        }
      //  static ConstructorInfo LispIntegerConstructor = typeof(LispInteger).GetConstructor(new Type[] { typeof(int) });
        public static void EmitInt32(this ILGenerator Real, int i)
        {
            OpCode c;
            switch (i)
            {
                case -1: c = OpCodes.Ldc_I4_M1; break;
                case 0: c = OpCodes.Ldc_I4_0; break;
                case 1: c = OpCodes.Ldc_I4_1; break;
                case 2: c = OpCodes.Ldc_I4_2; break;
                case 3: c = OpCodes.Ldc_I4_3; break;
                case 4: c = OpCodes.Ldc_I4_4; break;
                case 5: c = OpCodes.Ldc_I4_5; break;
                case 6: c = OpCodes.Ldc_I4_6; break;
                case 7: c = OpCodes.Ldc_I4_7; break;
                case 8: c = OpCodes.Ldc_I4_8; break;
                default:
                    if (i >= 0 && i <= 127)
                    {
                        Real.Emit(OpCodes.Ldc_I4_S, i);
                    }
                    else
                    {
                        Real.Emit(OpCodes.Ldc_I4, i);
                    }

                    return;
            }
            Real.Emit(c);
        }

        public static void EmitUInt32(this ILGenerator Real, uint i)
        {
            EmitInt32(Real, (int)i);
            Real.Emit(OpCodes.Conv_U4);
        }

        public static void EmitInt64(this ILGenerator Real, Int64 i)
        {
            Real.Emit(OpCodes.Ldc_I8, i);
        }

        public static void EmitUInt64(this ILGenerator Real, UInt64 i)
        {//TODO: remove conversion here, how c# compiler do that?
            Real.Emit(OpCodes.Ldc_I8, (Int64)i);
            Real.Emit(OpCodes.Conv_U8);
        }

        public static void EmitSingle(this ILGenerator Real, float value)
        {
            Real.Emit(OpCodes.Ldc_R4, value);
        }

        public static void EmitDouble(this ILGenerator Real, double value)
        {
            Real.Emit(OpCodes.Ldc_R8, value);
        }

        public static void EmitType(this ILGenerator Real, Type type)
        {
            if (!(type is TypeBuilder) && !type.IsGenericParameter && !type.IsVisible)
            {
                throw new InvalidOperationException("Cannot emit type");
            }

            Real.Emit(OpCodes.Ldtoken, type);
            EmitCall(Real, typeof(Type), "GetTypeFromHandle");
        }

        public static void EmitTypeSkipVisible(this ILGenerator Real, Type type)
        {
            if (type is TypeBuilder)
            {
                throw new InvalidOperationException("Cannot emit type");
            }

            Real.Emit(OpCodes.Ldtoken, type);
            EmitCall(Real, typeof(Type), "GetTypeFromHandle");
        }

        public static void EmitString(this ILGenerator Real, string value)
        {
            Real.Emit(OpCodes.Ldstr, value);
        }

        #endregion

        public static void EmitLocalSet(this ILGenerator Real, LocalBuilder var)
        {
            int locIndex = var.LocalIndex;
            EmitLocalSet(Real, locIndex);
        }

        public static void EmitLocalSet(this ILGenerator Real, int locIndex)
        {
            switch (locIndex)
            {
                case 0:
                    Real.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    Real.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    Real.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    Real.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    if (locIndex < 256)
                        Real.Emit(OpCodes.Stloc_S, locIndex);
                    else
                        Real.Emit(OpCodes.Stloc, locIndex);
                    break;
            }
        }

        public static void EmitLocalGet(this ILGenerator Real, LocalBuilder var)
        {
            int locIndex = var.LocalIndex;
            EmitLocalGet(Real, locIndex);

        }

        public static void EmitLocalGet(this ILGenerator Real, int locIndex)
        {
            switch (locIndex)
            {
                case 0:
                    Real.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    Real.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    Real.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    Real.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (locIndex < 256)
                        Real.Emit(OpCodes.Ldloc_S, locIndex);
                    else
                        Real.Emit(OpCodes.Ldloc, locIndex);
                    break;
            }
        }

        public static void EmitStoreElement(this ILGenerator Real, Type type)
        {
            if (type.IsValueType)
            {
                if (type == typeof(int) || type == typeof(uint))
                {
                    Real.Emit(OpCodes.Stelem_I4);
                }
                else if (type == typeof(short) || type == typeof(ushort))
                {
                    Real.Emit(OpCodes.Stelem_I2);
                }
                else if (type == typeof(long) || type == typeof(ulong))
                {
                    Real.Emit(OpCodes.Stelem_I8);
                }
                else if (type == typeof(char))
                {
                    Real.Emit(OpCodes.Stelem_I2);
                }
                else if (type == typeof(bool))
                {
                    Real.Emit(OpCodes.Stelem_I4);
                }
                else if (type == typeof(float))
                {
                    Real.Emit(OpCodes.Stelem_R4);
                }
                else if (type == typeof(double))
                {
                    Real.Emit(OpCodes.Stelem_R8);
                }
                else
                {
                    Real.Emit(OpCodes.Stelem, type);
                }
            }
            else
            {
                Real.Emit(OpCodes.Stelem_Ref);
            }
        }

        public static void EmitBox(this ILGenerator Real, Type type)
        {
            Real.Emit(OpCodes.Box, type);
        }

        public static void EmitUnboxAny(this ILGenerator Real, Type type)
        {
            Real.Emit(OpCodes.Unbox_Any, type);
        }

        public static void EmitLdarg(this ILGenerator Real, int ArgNum)
        {
            switch (ArgNum)
            {
                case 0:
                    Real.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    Real.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    Real.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    Real.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (ArgNum < 256)
                        Real.Emit(OpCodes.Ldarg_S, ArgNum);
                    else
                        Real.Emit(OpCodes.Ldarg, ArgNum);
                    break;
            }
        }

        static FieldInfo NILfield = typeof(DefinedSymbols).GetField("NIL", BindingFlags.Static | BindingFlags.Public);

        public static void EmitNIL(this ILGenerator Real)
        {
            Real.Emit(OpCodes.Ldsfld, NILfield);
        }

        static FieldInfo Tfield = typeof(DefinedSymbols).GetField("T", BindingFlags.Static | BindingFlags.Public);

        public static void EmitT(this ILGenerator Real)
        {
            Real.Emit(OpCodes.Ldsfld, Tfield);
        }
        /*
        #region shortcut
        public static void Emit(OpCode opcode)
        {
            WriteIL(opcode);
            Real.Emit(opcode);
        }
        public static void Emit(OpCode opcode, byte arg)
        {
            WriteIL(opcode, arg);
            Real.Emit(opcode, arg);
        }
        public static void Emit(OpCode opcode, ConstructorInfo con)
        {
            WriteIL(opcode, con);
            Real.Emit(opcode, con);
        }
        public static void Emit(OpCode opcode, double arg)
        {
            WriteIL(opcode, arg);
            Real.Emit(opcode, arg);
        }
        public static void Emit(OpCode opcode, FieldInfo field)
        {
            if (field == null)
            {
                throw new Exception();
            }
            WriteIL(opcode, field);
            ((ILGenerator)Real).Emit(opcode, field);
        }
        public static void Emit(OpCode opcode, float arg)
        {
            WriteIL(opcode, arg);
            Real.Emit(opcode, arg);
        }
        public static void Emit(OpCode opcode, int arg)
        {
            WriteIL(opcode, arg);
            Real.Emit(opcode, arg);
        }
        public static void Emit(OpCode opcode, Label label)
        {
            WriteIL(opcode, label);
            Real.Emit(opcode, label);
        }
        public static void Emit(OpCode opcode, Label[] labels)
        {
            WriteIL(opcode, labels);
            Real.Emit(opcode, labels);
        }
        public static void Emit(OpCode opcode, LocalBuilder local)
        {
            WriteIL(opcode, local);
            Real.Emit(opcode, local);
        }
        public static void Emit(OpCode opcode, long arg)
        {
            WriteIL(opcode, arg);
            Real.Emit(opcode, arg);
        }
        public static void Emit(OpCode opcode, MethodInfo meth)
        {
            WriteIL(opcode, meth);
            Real.Emit(opcode, meth);
        }
        public static void Emit(OpCode opcode, sbyte arg)
        {
            WriteIL(opcode, arg);
            Real.Emit(opcode, arg);
        }
        public static void Emit(OpCode opcode, short arg)
        {
            WriteIL(opcode, arg);
            Real.Emit(opcode, arg);
        }
        public static void Emit(OpCode opcode, SignatureHelper signature)
        {
            WriteIL(opcode, signature);
            Real.Emit(opcode, signature);
        }
        public static void Emit(OpCode opcode, string str)
        {
            WriteIL(opcode, str);
            Real.Emit(opcode, str);
        }
        public static void Emit(OpCode opcode, Type cls)
        {
            WriteIL(opcode, cls);
            Real.Emit(opcode, cls);
        }
        public static void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
        {
            WriteIL(opcode, methodInfo, optionalParameterTypes);
            Real.EmitCall(opcode, methodInfo, optionalParameterTypes);
        }
        #endregion //shortcut

        #region debug
        [Conditional("DEBUG")]
        private void WriteSignature(string name, Type[] paramTypes)
        {
            WriteIL("{0} (", name);
            foreach (Type type in paramTypes)
            {
                WriteIL("\t{0}", type.FullName);
            }
            WriteIL(")");
        }
        [Conditional("DEBUG")]
        private void WriteIL(string format, object arg0)
        {
            // WriteIL(String.Format(format, arg0));
        }
        [Conditional("DEBUG")]
        private void WriteIL(string format, object arg0, object arg1)
        {
            //WriteIL(String.Format(format, arg0, arg1));
        }
        [Conditional("DEBUG")]
        private void WriteIL(string format, object arg0, object arg1, object arg2)
        {
            // WriteIL(String.Format(format, arg0, arg1, arg2));
        }
        [Conditional("DEBUG")]
        private void WriteIL(string str)
        {
            if (!Options.ILDebug) return;

            if (ilOut == null)
            {
                InitializeILWriter();
            }

            curLine++;
            ilOut.WriteLine(str);
            ilOut.Flush();

            if (debugSymbolWriter != null)
            {
                MarkSequencePoint(
                 debugSymbolWriter,
                 curLine, 1,
                 curLine, str.Length + 1
                 );
            }
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode op)
        {
            // if (Options.ILDebug) WriteIL(op.ToString());
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, byte arg)
        {
            // if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, arg);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, ConstructorInfo con)
        {
            // if (Options.ILDebug) WriteIL("{0}\t{1}({2})", opcode, con.DeclaringType, MakeSignature(con));
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, double arg)
        {
            //  if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, arg);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, FieldInfo field)
        {
            // if (Options.ILDebug) WriteIL("{0}\t{1}.{2}", opcode, field.DeclaringType, field.Name);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, float arg)
        {
            //  if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, arg);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, int arg)
        {
            //if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, arg);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, Label label)
        {
            // if (Options.ILDebug) WriteIL("{0}\tlabel_{1}", opcode, GetLabelId(label));
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, Label[] labels)
        {
            if (Options.ILDebug)
             {
                 System.Text.StringBuilder sb = new System.Text.StringBuilder();
                 sb.Append(opcode.ToString());
                 sb.Append("\t[");
                 for (int i = 0; i < labels.Length; i++)
                 {
                     if (i != 0) sb.Append(", ");
                     sb.Append("label_" + GetLabelId(labels[i]).ToString());
                 }
                 sb.Append("]");
                 WriteIL(sb.ToString());
             }
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, LocalBuilder local)
        {
            // if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, local);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, long arg)
        {
            // if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, arg);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, MethodInfo meth)
        {
            //if (Options.ILDebug) WriteIL("{0}\t{1} {2}.{3}({4})", opcode, meth.ReturnType.FullName, meth.DeclaringType, meth.Name, MakeSignature(meth));
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, sbyte arg)
        {
            //if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, arg);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, short arg)
        {
            //if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, arg);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, SignatureHelper signature)
        {
            // if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, signature);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, string str)
        {
            // if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, str);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, Type cls)
        {
            // if (Options.ILDebug) WriteIL("{0}\t{1}", opcode, cls.FullName);
        }
        [Conditional("DEBUG")]
        private void WriteIL(OpCode opcode, MethodInfo meth, Type[] optionalParameterTypes)
        {
            //  if (Options.ILDebug) WriteIL("{0}\t{1} {2}.{3}({4})", opcode, meth.ReturnType.FullName, meth.DeclaringType, meth.Name, MakeSignature(meth));
        }
        [Conditional("DEBUG")]
        private void WriteIL(Label l)
        {
            // if (Options.ILDebug) WriteIL("label_{0}:", GetLabelId(l).ToString());
        }
        #endregion //debug

        

        internal Label BeginExceptionBlock()
        {
            return Real.BeginExceptionBlock();
        }

        internal void BeginCatchBlock(Type type)
        {
            Real.BeginCatchBlock(type);
        }

        internal void EndExceptionBlock()
        {
            Real.EndExceptionBlock();
        }*/

        #region boxing
        public static void BoxLispInteger(this ILGenerator Real)
        {
            Real.EmitBox(typeof(Int32));
        }
        #endregion
    }
}
