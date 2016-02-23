using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST;
using System.Reflection.Emit;
using LiveLisp.Core.Compiler;
using System.Reflection;
using LiveLisp.Core.AST.Expressions.CLR;

namespace LiveLisp.Core.Runtime.OperatorsCache
{
    public static class AddOperatorCache 
    {
        static Dictionary<Type, Dictionary<Type, BinaryOperator>> _cache = new Dictionary<Type, Dictionary<Type, BinaryOperator>>();

        internal static object GetAndInvokeTarget(object arg1, object arg2)
        {
            return GeneralHelpers.GetAndInvokeTarget2(_cache, Operator.Add, arg1, arg2);
        }
    }
}
