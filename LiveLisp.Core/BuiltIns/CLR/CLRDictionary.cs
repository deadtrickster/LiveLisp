using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.AST;
using LiveLisp.Core.Types;
using LiveLisp.Core.BuiltIns.Conditions;
using System.Reflection;

namespace LiveLisp.Core.BuiltIns.CLR
{
    [BuiltinsContainer("CLR")]
    public static class CLRDictionary
    {
        [Builtin("d-reference", Predicate=true)]
        public static object Reference(object assemblyDesignator)
        {
            if (assemblyDesignator is string)
            {
                return AssemblyCache.AddAssembly(assemblyDesignator.ToString()) == null ? DefinedSymbols.NIL : DefinedSymbols.T;
            }
            else if (assemblyDesignator is string)
            {
                return AssemblyCache.AddAssembly(assemblyDesignator.ToString()) == null ? DefinedSymbols.NIL : DefinedSymbols.T;
            }
            else if (assemblyDesignator is Assembly)
            {
                AssemblyCache.AddAssembly(assemblyDesignator as Assembly);
                return DefinedSymbols.T;
            }

            ConditionsDictionary.Error("Invalid Assembly designator: " + assemblyDesignator);
            return DefinedSymbols.NIL;
        }


        [Builtin("d-new")]
        public static object New(object nameDesignator, params object[] args)
        {
            string clrTypeName = null;

            if (nameDesignator is string)
            {
                clrTypeName = nameDesignator.ToString();
            }
            else if (nameDesignator is Symbol)
            {
                clrTypeName = (nameDesignator as Symbol).Name;
            }
            else if (nameDesignator is Cons)
            {
                ConditionsDictionary.Error("Generic types not supported");
            }
            Type type = null;

            if (clrTypeName != null)
            {
                StaticTypeResolver tr = new StaticTypeResolver(clrTypeName);
                type = tr.Type;
            }
            else
            {
                type = nameDesignator.GetType();
            }

            return Activator.CreateInstance(type, LispMarshaler.ToClr(args));
        }

        [Builtin("d-call")]
        public static object Call(object instance, object nameDesignator, params object[] args)
        {
            string methodName = null;

            if (nameDesignator is string)
            {
                methodName = nameDesignator.ToString();
            }
            else if (nameDesignator is Symbol)
            {
                methodName = (nameDesignator as Symbol).Name;
            }
            else if (nameDesignator is Cons)
            {
                ConditionsDictionary.Error("Generic methods not supported");
            }
            MethodInfo mi = null;
            if (methodName == null)
            {
                if (nameDesignator is MethodInfo)
                {
                    mi = nameDesignator as MethodInfo;
                }
            }
            else
            {
                mi = StaticMethodResolver.Create(StaticTypeResolver.Create(instance.GetType()), methodName, LispMarshaler.ToClr(args)).Method;
            }

            if (mi == null)
            {
                ConditionsDictionary.Error("Unable to resolve method " + nameDesignator);
            }

            return mi.Invoke(instance, args);
        }

        [Builtin("d-scall")]
        public static object sCall(object typeDesignator, object nameDesignator, params object[] args)
        {
            string clrTypeName = null;

            if (typeDesignator is string)
            {
                clrTypeName = typeDesignator.ToString();
            }
            else if (typeDesignator is Symbol)
            {
                clrTypeName = (typeDesignator as Symbol).Name;
            }
            else if (typeDesignator is Cons)
            {
                ConditionsDictionary.Error("Generic types not supported");
            }
            Type type = null;

            if (clrTypeName != null)
            {
                StaticTypeResolver tr = new StaticTypeResolver(clrTypeName);
                type = tr.Type;
            }
            else
            {
                type = nameDesignator.GetType();
            }

            string methodName = null;

            if (nameDesignator is string)
            {
                methodName = nameDesignator.ToString();
            }
            else if (nameDesignator is Symbol)
            {
                methodName = (nameDesignator as Symbol).Name;
            }
            else if (nameDesignator is Cons)
            {
                ConditionsDictionary.Error("Generic methods not supported");
            }
            MethodInfo mi = null;
            if (methodName == null)
            {
                if (nameDesignator is MethodInfo)
                {
                    mi = nameDesignator as MethodInfo;
                }
            }
            else
            {
                mi = StaticMethodResolver.Create(StaticTypeResolver.Create(type), methodName, LispMarshaler.ToClr(args)).Method;
            }

            if (mi == null)
            {
                ConditionsDictionary.Error("Unable to resolve method " + nameDesignator);
            }

            return mi.Invoke(null, args);
        }
    }
}
