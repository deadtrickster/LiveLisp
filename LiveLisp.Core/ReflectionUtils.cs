using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LiveLisp.Core.Utils
{
    public class ReflectionUtils
    {

        internal static Type[] GetTypesMarkedwithAttr(System.Reflection.Assembly[] assemblies, Type attrType)
        {
            List<Type> ret = new List<Type>();

            foreach (var assembly in assemblies)
            {
                ret.AddRange(assembly.GetTypes().Where(type => type.GetCustomAttributes(attrType, true).Length != 0));
            }

            return ret.ToArray();
        }

        internal static System.Reflection.MethodInfo[] GetMethodMarkedwithAttr(Type[] types, Type attrType)
        {
            List<MethodInfo> ret = new List<MethodInfo>();

            foreach (var type in types)
            {
                ret.AddRange(type.GetMethods().Where(mi => mi.GetCustomAttributes(attrType, true).Length != 0));
            }

            return ret.ToArray();
        }

        internal static T1 GetFirstAttrInstance<T1>(ICustomAttributeProvider member)
        {
            return (T1)member.GetCustomAttributes(typeof(T1), false)[0];
        }
    }
}

