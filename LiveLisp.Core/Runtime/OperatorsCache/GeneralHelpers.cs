using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using LiveLisp.Core.AST.Expressions.CLR;
using LiveLisp.Core.Compiler;

namespace LiveLisp.Core.Runtime.OperatorsCache
{
    public static class GeneralHelpers
    {

        private static MethodInfo _EqualsMethod  = typeof(object).GetMethod("Equals", BindingFlags.Public|BindingFlags.Static);

        public static object GetAndInvokeTarget2(Dictionary<Type, Dictionary<Type, BinaryOperator>> _cache, Operator op, object arg1, object arg2)
        {

            Dictionary<Type, BinaryOperator> val;

            Type arg1Type = arg1.GetType();
            Type arg2Type = arg2.GetType();

            // bool arithmetic = IsArithmetic(arg1Type) && IsArithmetic(arg2Type);

            BinaryOperator target;
            if (_cache.TryGetValue(arg1Type, out val))
            {
                if (val.TryGetValue(arg2Type, out target))
                {
                    return target(arg1, arg2);
                }
            }

            if (_cache.TryGetValue(arg2Type, out val))
            {
                if (val.TryGetValue(arg1Type, out target))
                {
                    return target(arg2, arg1);
                }
            }

            bool swap_args;
            target = GeneralHelpers.MakeNewBinaryOpTarget(op, arg1Type, arg2Type, out swap_args);


            if (swap_args)
            {
                Type tmp = arg1Type;
                arg1Type = arg2Type;
                arg2Type = tmp;
            }


            Dictionary<Type, BinaryOperator> newDictionary;
            if (!_cache.TryGetValue(arg1Type, out newDictionary))
            {
                newDictionary = new Dictionary<Type, BinaryOperator>();
                _cache.Add(arg1Type, newDictionary);
            }

            newDictionary.Add(arg2Type, target);

            if (swap_args)
                return target(arg2, arg1);

            return target(arg1, arg2);
        }


        public static BinaryOperator MakeNewBinaryOpTarget(Operator op, Type arg1Type, Type arg2Type, out bool swap_args)
        {
            bool both_a = GeneralHelpers.IsArithmetic(arg1Type) && GeneralHelpers.IsArithmetic(arg2Type);
            if (IsOpArithmetic(op) && both_a)
            {
                swap_args = false;
                return GeneralHelpers.EmitSimpleArithmeticOp(op, arg1Type, arg2Type);
            }
            else if (IsComparisonOp(op) && both_a)
            {
                swap_args = false;
                return GeneralHelpers.EmitSimpleComparisonOp(op, arg1Type, arg2Type, true);
            }
            else
            {
                return GeneralHelpers.EmitCustomBinaryOp(op, arg1Type, arg2Type, true, out swap_args);
            }
        }

        private static bool IsOpArithmetic(Operator op)
        {
            switch (op)
            {
                case Operator.Add:
                case Operator.Sub:
                case Operator.Mul:
                case Operator.Div:
                    return true;
                case Operator.LessThen:
                case Operator.GreaterThen:
                case Operator.LessOrEqual:
                case Operator.GreaterOrEqual:
                case Operator.Equal:
                case Operator.NotEqual:
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }

        private static bool IsComparisonOp(Operator op)
        {
            switch (op)
            {
                case Operator.Add:
                case Operator.Sub:
                case Operator.Mul:
                case Operator.Div:
                    return false;
                case Operator.LessThen:
                case Operator.GreaterThen:
                case Operator.LessOrEqual:
                case Operator.GreaterOrEqual:
                case Operator.Equal:
                case Operator.NotEqual:
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }

        public static BinaryOperator EmitSimpleArithmeticOp(Operator op, Type arg1Type, Type arg2Type)
        {
            DynamicMethod new_Target = new DynamicMethod("AddOp" + arg1Type.Name + arg2Type.Name, typeof(object), new Type[] { typeof(object), typeof(object) });
            Type upgradet = GeneralHelpers.UpgradeToType1(arg1Type, arg2Type);
            Type upgradet1 = GeneralHelpers.UpgradeToType2(arg1Type, arg2Type);
            ILGenerator gen = new_Target.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Unbox_Any, arg1Type);
            if (op == Operator.Div && (upgradet == typeof(int) || upgradet == typeof(long)))
                upgradet = typeof(double);
            if (upgradet != arg1Type)
                gen.EmitNumericConversion(arg1Type, upgradet, false);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Unbox_Any, arg2Type);
            if (op == Operator.Div && (upgradet1 == typeof(int) || upgradet1 == typeof(long)))
                upgradet1 = typeof(double);
            if (upgradet1 != arg2Type)
                gen.EmitNumericConversion(arg2Type, upgradet1, false);

            Type retType = GeneralHelpers.SimpleArithmeticResultType(upgradet, upgradet1);
            switch (op)
            {
                case Operator.Add:
                    if (upgradet == typeof(int))
                        gen.EmitCall(RuntimeHelpers.Int32AddMethod.Method);
                    else if(upgradet == typeof(long))
                        gen.EmitCall(RuntimeHelpers.Int64AddMethod.Method);
                    else
                        gen.Emit(OpCodes.Add);
                    break;
                case Operator.Sub: 
                    if (upgradet == typeof(int))
                        gen.EmitCall(RuntimeHelpers.Int32SubMethod.Method);
                    else if (upgradet == typeof(long))
                        gen.EmitCall(RuntimeHelpers.Int64SubMethod.Method);
                    else
                    gen.Emit(OpCodes.Sub);
                    break;
                case Operator.Mul:
                    if (upgradet == typeof(int))
                        gen.EmitCall(RuntimeHelpers.Int32MulMethod.Method);
                    else if (upgradet == typeof(long))
                        gen.EmitCall(RuntimeHelpers.Int64MulMethod.Method);
                    else
                    gen.Emit(OpCodes.Mul);
                    break;
                case Operator.Div:
                    if (upgradet == typeof(int))
                        gen.EmitCall(RuntimeHelpers.Int32DivMethod.Method);
                    else if (upgradet == typeof(long))
                        gen.EmitCall(RuntimeHelpers.Int64DivMethod.Method);
                    else
                    gen.Emit(OpCodes.Div);
                    break;
                default:
                    throw new NotImplementedException();
            }
            if (upgradet != typeof(int) && upgradet != typeof(long))
            {
                gen.Emit(OpCodes.Box, retType);
            }
            
            gen.Emit(OpCodes.Ret);

            return new_Target.CreateDelegate(typeof(BinaryOperator)) as BinaryOperator;
        }

        private static BinaryOperator EmitSimpleComparisonOp(Operator op, Type arg1Type, Type arg2Type, bool convert_to_lisp)
        {
            DynamicMethod new_Target = new DynamicMethod("AddOp" + arg1Type.Name + arg2Type.Name, typeof(object), new Type[] { typeof(object), typeof(object) });
            Type upgradet = GeneralHelpers.UpgradeToType1(arg1Type, arg2Type);
            Type upgradet1 = GeneralHelpers.UpgradeToType2(arg1Type, arg2Type);
            ILGenerator gen = new_Target.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Unbox_Any, arg1Type);
            if (upgradet != arg1Type)
                gen.EmitNumericConversion(arg1Type, upgradet, false);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Unbox_Any, arg2Type);
            if (upgradet != arg2Type)
                gen.EmitNumericConversion(arg1Type, upgradet, false);
            switch (op)
            {
                case Operator.LessThen:
                    gen.Emit(OpCodes.Clt);
                    break;
                case Operator.GreaterThen:
                    gen.Emit(OpCodes.Cgt);
                    break;
                case Operator.LessOrEqual:
                    gen.Emit(OpCodes.Cgt);
                    gen.Emit(OpCodes.Ldc_I4_0);
                    gen.Emit(OpCodes.Ceq);
                    break;
                case Operator.GreaterOrEqual:
                    gen.Emit(OpCodes.Clt);
                    gen.Emit(OpCodes.Ldc_I4_0);
                    gen.Emit(OpCodes.Ceq);
                    break;
                case Operator.Equal:
                    gen.Emit(OpCodes.Ceq);
                    break;
                case Operator.NotEqual:
                    gen.Emit(OpCodes.Ceq);
                    gen.Emit(OpCodes.Ldc_I4_0);
                    gen.Emit(OpCodes.Ceq);
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (convert_to_lisp)
            {
                Label else_label = gen.DefineLabel();
                Label end_label = gen.DefineLabel();
                gen.Emit(OpCodes.Brfalse, else_label);
                gen.EmitT();
                gen.JmpToLabel(end_label);
                gen.MarkLabel(else_label);
                gen.EmitNIL();
                gen.MarkLabel(end_label);
            }
            else
            {
                gen.Emit(OpCodes.Box, typeof(Boolean));
            }
            gen.Emit(OpCodes.Ret);

            return new_Target.CreateDelegate(typeof(BinaryOperator)) as BinaryOperator;
        }

        public static BinaryOperator EmitCustomBinaryOp(Operator op, Type arg1Type, Type arg2Type, bool convert_to_lisp, out bool swap_args)
        {
            swap_args = false;
            MethodInfo op_method = GetCustomBinaryOperator(op, arg1Type, arg2Type, ref swap_args);

            if (op_method == null)
                throw new OperatorNotFoundException("The binary operator {0} is not defined for the types '{1}' and '{2}'.", OpDisplayName(op), arg1Type, arg2Type);

            if (swap_args)
            {
                Type tmp = arg1Type;
                arg1Type = arg2Type;
                arg2Type = tmp;
            }

            DynamicMethod new_Target = new DynamicMethod(OpDisplayName(op) + " " + arg1Type.Name + arg2Type.Name, typeof(object), new Type[] { typeof(object), typeof(object) });
            ILGenerator gen = new_Target.GetILGenerator();

            var parms = op_method.GetParameters();

            gen.Emit(OpCodes.Ldarg_0);

            if (parms[0].ParameterType.IsValueType)
            {
                gen.Emit(OpCodes.Unbox_Any, arg1Type);
            }
            else
            {
                gen.Emit(OpCodes.Isinst, arg1Type);
            }

            gen.Emit(OpCodes.Ldarg_1);

            if (parms[1].ParameterType.IsValueType)
            {
                gen.Emit(OpCodes.Unbox_Any, arg2Type);
            }
            else
            {
                gen.Emit(OpCodes.Isinst, arg2Type);
            }

            gen.Emit(OpCodes.Call, op_method);

            if (convert_to_lisp)
            {
                if (op_method.ReturnType == typeof(Boolean))
                {
                    Label else_label = gen.DefineLabel();
                    Label end_label = gen.DefineLabel();
                    gen.Emit(OpCodes.Brfalse, else_label);
                    gen.EmitT();
                    gen.JmpToLabel(end_label);
                    gen.MarkLabel(else_label);
                    gen.EmitNIL();
                    gen.MarkLabel(end_label);
                }

                else
                {

                    if (op_method.ReturnType.IsValueType)
                    {
                        gen.Emit(OpCodes.Box, op_method.ReturnType);
                    }
                }
            }
            else
            {
                if (op_method.ReturnType.IsValueType)
                {
                    gen.Emit(OpCodes.Box, op_method.ReturnType);
                }
            }

            gen.EmitRet();


            return new_Target.CreateDelegate(typeof(BinaryOperator)) as BinaryOperator;
        }

        private static string OpDisplayName(Operator op)
        {
            switch (op)
            {
                case Operator.Add:
                    return "Add";
                case Operator.Sub:
                    return "Sub";
                case Operator.Mul:
                    return "Mul";
                case Operator.Div:
                    return "Div";
                case Operator.LessThen:
                    return "LessThen";
                case Operator.GreaterThen:
                    return "GreateThen";
                case Operator.LessOrEqual:
                    return "LessOrEqual";
                case Operator.GreaterOrEqual:
                    return "GreaterOrEqual";
                case Operator.Equal:
                    return "Equal";
                case Operator.NotEqual:
                    return "NotEqual";
                default:
                    throw new NotImplementedException();
            }
        }

        private static string OpMethodName(Operator op)
        {
            switch (op)
            {
                case Operator.Add:
                    return "op_Addition";
                case Operator.Sub:
                    return "op_Subtraction";
                case Operator.Mul:
                    return "op_Multiply";
                case Operator.Div:
                    return "op_Division";
                case Operator.LessThen:
                    return "op_LessThen";
                case Operator.GreaterThen:
                    return "op_GreaterThen";
                case Operator.LessOrEqual:
                    return "op_GreaterThanOrEqual";
                case Operator.GreaterOrEqual:
                    return "op_LessThanOrEqual";
                case Operator.Equal:
                    return "op_Equality";
                case Operator.NotEqual:
                    return "op_Inequality";
                default:
                    throw new NotImplementedException();
            }
        }

        public static MethodInfo GetCustomBinaryOperator(Operator op, Type arg1Type, Type arg2Type, ref bool swap_args)
        {
            MethodInfo method = null;

            string m_name = OpMethodName(op);
            try
            {
                method = arg1Type.GetMethod(m_name, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { arg1Type, arg2Type }, null);
                if (method != null)
                    return method;
            }
            catch (AmbiguousMatchException)
            {

            }

            try
            {
                method = arg1Type.GetMethod(m_name, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { arg2Type, arg1Type }, null);
                if (method != null)
                {
                    swap_args = true;
                    return method;
                }
            }
            catch (AmbiguousMatchException)
            {

            }

            try
            {
                method = arg2Type.GetMethod(m_name, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { arg1Type, arg2Type }, null);
                if (method != null)
                    return method;
            }
            catch (AmbiguousMatchException)
            {

            }

            try
            {
                method = arg2Type.GetMethod(m_name, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { arg2Type, arg1Type }, null);

                if (method != null)
                {
                    swap_args = true;
                    return method;
                }
            }
            catch (AmbiguousMatchException)
            {

            }

            //check for conversion possibilities

            // equality patch - if op is Equality or Inequality operator and method == null;
            // replace with object.Equals call
            if (method == null && (op == Operator.Equal || op == Operator.NotEqual))
            {
                return _EqualsMethod;
            }
            return method;
        }

        public static Type SimpleArithmeticResultType(Type ca1, Type ca2)
        {
            TypeCode tc1 = Type.GetTypeCode(ca1);
            TypeCode tc2 = Type.GetTypeCode(ca2);
            switch (tc1)
            {
                case TypeCode.Double:
                    return ca1;
                case TypeCode.Int32:
                    switch (tc2)
                    {

                        case TypeCode.Double:
                            return ca2;
                        case TypeCode.Int32:
                            return ca1;
                        case TypeCode.Int64:
                            return ca2;
                        case TypeCode.Single:
                            return ca2;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc2)
                    {

                        case TypeCode.Double:
                            return ca2;
                        case TypeCode.Int32:
                            return ca1;
                        case TypeCode.Int64:
                            return ca1;
                        case TypeCode.Single:
                            return ca2;
                    }
                    break;
                case TypeCode.Single:

                    switch (tc2)
                    {

                        case TypeCode.Double:
                            return ca2;
                        case TypeCode.Int32:
                            return ca1;
                        case TypeCode.Int64:
                            return ca1;
                        case TypeCode.Single:
                            return ca1;
                    }
                    break;
            }

            throw new Exception();
        }

        public static Type UpgradeToType2(Type arg1Type, Type arg2Type)
        {
            Type ca1 = UpgradeCLRType(arg1Type);
            Type ca2 = UpgradeCLRType(arg2Type);

            if (ca1 == ca2)
                return ca1;

            TypeCode tc1 = Type.GetTypeCode(ca1);
            TypeCode tc2 = Type.GetTypeCode(ca2);
            switch (tc1)
            {
                case TypeCode.Double:
                    return ca1;
                case TypeCode.Int32:
                    switch (tc2)
                    {

                        case TypeCode.Double:
                            return ca2;
                        case TypeCode.Int32:
                            return ca1;
                        case TypeCode.Int64:
                            return ca2;
                        case TypeCode.Single:
                            return ca2;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc2)
                    {

                        case TypeCode.Double:
                            return ca2;
                        case TypeCode.Int32:
                            return ca1;
                        case TypeCode.Int64:
                            return ca1;
                        case TypeCode.Single:
                            return ca2;
                    }
                    break;
                case TypeCode.Single:

                    switch (tc2)
                    {

                        case TypeCode.Double:
                            return ca2;
                        case TypeCode.Int32:
                            return ca1;
                        case TypeCode.Int64:
                            return ca1;
                        case TypeCode.Single:
                            return ca1;
                    }
                    break;
            }

            throw new Exception();
        }

        public static Type UpgradeToType1(Type arg1Type, Type arg2Type)
        {
            return UpgradeToType2(arg2Type, arg1Type);
        }

        public static Type UpgradeCLRType(Type argType)
        {
            TypeCode tc = Type.GetTypeCode(argType);

            switch (tc)
            {
                case TypeCode.Boolean:
                    return typeof(Int32);
                case TypeCode.Byte:
                    return typeof(Int32);
                case TypeCode.Char:
                    return typeof(Int32);
                case TypeCode.DBNull:
                    break;
                case TypeCode.DateTime:
                    break;
                case TypeCode.Decimal:
                    break;
                case TypeCode.Double:
                    return typeof(Double);
                case TypeCode.Empty:
                    break;
                case TypeCode.Int16:
                    return typeof(Int32);
                case TypeCode.Int32:
                    return typeof(Int32);
                case TypeCode.Int64:
                    return typeof(Int64);
                case TypeCode.Object:
                    break;
                case TypeCode.SByte:
                    return typeof(Int32);
                case TypeCode.Single:
                    return typeof(Single);
                case TypeCode.String:
                    break;
                case TypeCode.UInt16:
                    return typeof(Int32);
                case TypeCode.UInt32:
                    return typeof(Int64);
                case TypeCode.UInt64:
                    return typeof(Single);
                default:
                    break;
            }

            throw new ArgumentException("type " + argType + " is not simple numeric type");
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

        internal static bool IsArithmetic(Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }
    }
}
