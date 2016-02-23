using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LiveLisp.Core.Types;
using System.IO;
using LiveLisp.Core.AST;
using System.Security.Policy;

namespace LiveLisp.Core.Runtime
{
    public class TypeCache
    {
        static TypeCache()
        {
            namespaces.Add("");
            namespaces.Add("system");
            namespaces.Add("livelisp");
            namespaces.Add("system.windows.forms");

            SetAlias("sbyte", typeof(SByte));
            SetAlias("byte", typeof(Byte));
            SetAlias("bool", typeof(Boolean));
            SetAlias("char", typeof(Char));
            SetAlias("short", typeof(Int16));
            SetAlias("ushort", typeof(UInt16));
            SetAlias("int", typeof(Int32));
            SetAlias("uint", typeof(UInt32));
            SetAlias("long", typeof(Int64));
            SetAlias("ulong", typeof(UInt64));
            SetAlias("float", typeof(Single));
            SetAlias("double", typeof(Double));
            SetAlias("decimal", typeof(Decimal));
            SetAlias("bigint", typeof(BigInteger));
            SetAlias("complex", typeof(Complex));
         //   SetAlias("ratio", typeof(Rational));
        }

        static List<string> namespaces = new List<string>();
        public static void AddNamespace(string ns)
        {
            ns = ns.ToLower();
            if (!namespaces.Contains(ns))
                namespaces.Add(ns);
        }

        public static void RemoveNamespace(string ns)
        {
            ns = ns.ToLower();
            if (namespaces.Contains(ns))
                namespaces.Remove(ns);
        }

        static Dictionary<string, Type> aliases = new Dictionary<string, Type>();


        /// <summary>
        /// if alias is name without dots then new alias just setted
        /// 
        /// else name is |namespace|.name
        /// 
        /// alias for name added
        /// alias for |namespace|.name added
        /// 
        /// and |namespace| added to namespaces
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="type"></param>
        public static void SetAlias(string alias, Type type)
        {
            alias = alias.ToLower();
            int last_dot_index = alias.LastIndexOf('.');
            if (last_dot_index == -1)
                _SetAlias(alias, type);
            else
            {
                string ns = alias.Substring(0, last_dot_index + 1);
                bool valid_ns = ValidateNamespace(ns);

                if (valid_ns)
                {
                    string name = alias.Substring(last_dot_index, alias.Length - last_dot_index);
                    _SetAlias(name, type);
                    AddNamespace(ns);
                }
                _SetAlias(alias, type);
            }
        }

        private static bool ValidateNamespace(string ns)
        {
            if(ns == "")
                return false;
            if (char.IsDigit(ns, 0))
                return false;

            bool dot = false; ;

            for (int i = 0; i < ns.Length; i++)
            {
                if (ns[i] == '.')
                {
                    if (dot)
                        return false;
                    dot = true;
                }
                else
                    dot = false;
            }

            return true;
        }

        static void _SetAlias(string alias, Type type)
        {
            if (aliases.ContainsKey(alias))
                aliases[alias] = type;
            else
            {
                aliases.Add(alias, type);
                FieldCache.RemoveFromCacheForType(alias);
                MethodCache.RemoveFromCacheForType(alias);
            }
        }

        public static void RemoveAliasForType(string alias)
        {
            if (aliases.ContainsKey(alias))
                aliases.Remove(alias);
        }

        static Dictionary<string, Type> cache = new Dictionary<string, Type>();
        public static bool TryGetType(string name, out Type type)
        {
            name = name.ToLower();
            // first of all see at aliases
            if (aliases.ContainsKey(name))
            {
                type = aliases[name];
                return true;
            }

            if (cache.ContainsKey(name))
            {
                type = cache[name];
                return true;
            }

            return ResolveType(name, out type);
        }

        private static bool ResolveType(string name, out Type type)
        {
            bool finded = false;
            Type t = null;
            type = null;
            for (int i = 0; i < AssemblyCache.Count; i++)
            {
                for (int j = 0; j < namespaces.Count; j++)
                {
                    #if !SILVERLIGHT
                    t = AssemblyCache.GetAssembly(i).GetType(namespaces[j] == "" ? name : namespaces[j] + "." + name, false, true);
#else
                    t = AssemblyCache.GetAssembly(i).GetType(namespaces[j] == "" ? name : namespaces[j] + "." + name, false);
#endif
                    if (t != null)
                    {
                        if (finded)
                            throw new FormattedException("Types Ambiguity between " + type + " and " + t);
                        else
                        {
                            type = t;
                            finded = true;
                        }
                    }
                }
            }

            return finded;
        }

        internal static bool IsAlias(string type_name)
        {
            return aliases.ContainsKey(type_name);
        }
    }

    public class FieldCache
    {
        static Dictionary<string, FieldInfo> aliases = new Dictionary<string, FieldInfo>();

        public static void SetAlias(string alias, FieldInfo field)
        {
            if (aliases.ContainsKey(alias))
                aliases[alias] = field;
            else
                aliases.Add(alias, field);
        }
        static Dictionary<string, FieldInfo> cache = new Dictionary<string, FieldInfo>();
        public static bool TryGetField(string type_name, string field_name, out FieldInfo field)
        {
            if (type_name == "")
            {
                if (aliases.ContainsKey(field_name))
                {
                    field = aliases[field_name];
                    return true;
                }
                else
                {
                    field = null;
                    return false;
                }
            }

            string fullname = type_name + "." + field_name;
            if (aliases.ContainsKey(fullname))
            {
                field = aliases[field_name];
                return true;
            }

            if(!TypeCache.IsAlias(type_name))
                if (cache.ContainsKey(fullname))
                {
                    field = aliases[field_name];
                    return true;
                }

            Type t;

            if (TypeCache.TryGetType(type_name, out t))
            {
               field = t.GetField(field_name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
               if (field != null)
               {
                   cache.Add(fullname, field);
                   return true;
               }
               else
                   return false;
            }
            else
            {
                field = null;
                return false;
            }
        }

        internal static void RemoveFromCacheForType(string alias)
        {
            List<string> keys_to_remove = new List<string>();

            foreach (var item in cache)
            {
                if (item.Value.DeclaringType.Name == alias)
                    keys_to_remove.Add(item.Key);
            }

            for (int i = 0; i < keys_to_remove.Count; i++)
            {
                cache.Remove(keys_to_remove[i]);
            }
        }
    }

    public class MethodCache
    {
        static Dictionary<string, MethodInfo> aliases = new Dictionary<string, MethodInfo>();

        public static void SetAlias(string alias, MethodInfo method)
        {
            alias = alias.ToLower();
            if (aliases.ContainsKey(alias))
                aliases[alias] = method;
            else
                aliases.Add(alias, method);
        }
        static Dictionary<string, MethodInfo> cache = new Dictionary<string, MethodInfo>();
        public static bool TryGetMethod(string type_name, string method_name, StaticTypeResolver[] paramsTypes, out MethodInfo method)
        {
            type_name = type_name.ToLower();
            method_name = method_name.ToLower();
            if (type_name == "")
            {
                if (aliases.ContainsKey(method_name))
                {
                    method = aliases[method_name];
                    return true;
                }
                else
                {
                    method = null;
                    return false;
                }
            }

            string fullname = type_name + "." + method_name;
            if (aliases.ContainsKey(fullname))
            {
                method = aliases[method_name];
                return true;
            }

           /* if (!TypeCache.IsAlias(type_name))
                if (cache.ContainsKey(fullname))
                {
                    method = cache[fullname];
                    return true;
                }*/

            Type t;

            if (TypeCache.TryGetType(type_name, out t))
            {
                /*if (paramsTypes != null)
                {
                    Type[] argTypes = new Type[paramsTypes.Length];

                    for (int i = 0; i < argTypes.Length; i++)
                    {
                        argTypes[i] = paramsTypes[i].Type;
                    }
                    method = t.GetMethod(method_name, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic, null, argTypes, null);   
                }
                else
                {
                    method = t.GetMethod(method_name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);                    
                }

                if (method != null)
                {
                    cache.Add(fullname, method);
                    return true;
                }
                else
                    return false;*/

                return _tryGetMethod(t, method_name, paramsTypes, out method);
            }
            else
            {
                method = null;
                return false;
            }
        }

        public static bool TryGetMethod(Type type, string method_name, StaticTypeResolver[] paramsTypes, out MethodInfo method)
        {
            /*if (cache.ContainsKey(type.FullName + "." + method_name))
            {
                method = cache[type.FullName + "." + method_name];
                return true;
            }
            else*/
                return _tryGetMethod(type, method_name, paramsTypes, out method);
        }

        private static bool _tryGetMethod(Type type, string method_name, StaticTypeResolver[] paramsTypes, out MethodInfo method)
        {
            if (paramsTypes != null)
            {
                Type[] argTypes = new Type[paramsTypes.Length];

                for (int i = 0; i < argTypes.Length; i++)
                {
                    argTypes[i] = paramsTypes[i].Type;
                }
                method = type.GetMethod(method_name, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic, null, argTypes, null);
            }
            else
            {
                method = type.GetMethod(method_name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase, null, Type.EmptyTypes, null);
            }

            if (method != null)
            {
                //cache.Add(type.FullName + "." + method_name, method);
                return true;
            }
            else
                return false;
        }

        internal static void RemoveFromCacheForType(string alias)
        {
            List<string> keys_to_remove = new List<string>();

            foreach (var item in cache)
            {
                if (item.Value.DeclaringType.Name == alias)
                    keys_to_remove.Add(item.Key);
            }

            for (int i = 0; i < keys_to_remove.Count; i++)
            {
                cache.Remove(keys_to_remove[i]);
            }
        }
    }


#pragma warning disable 0618
    public class AssemblyCache
    {
        static List<Assembly> cache = new List<Assembly>();

        public static Assembly AddAssembly(string name)
        {
            Assembly o = null;
            try
            {
                if (Path.IsPathRooted(name))
                    o = Assembly.LoadFrom(name);
                else
                    o = Assembly.Load(name);
            }
            catch (Exception)
            {
#if !SILVERLIGHT
                o = Assembly.LoadWithPartialName(name);
#endif
            }

            if (o != null)
            {

                if (!cache.Contains(o))
                    cache.Add(o);
            }

            return o;
        }

        public static void AddAssembly(AssemblyName name)
        {
            Assembly.Load(name, null);
        }

        public static void AddAssembly(Assembly assembly)
        {
            if (!cache.Contains(assembly))
                cache.Add(assembly);
        }

        public static int Count
        {
            get { return cache.Count; }
        }

        internal static Assembly GetAssembly(int i)
        {
            return cache[i];
        }
        #if !SILVERLIGHT
        static AssemblyCache()
        {
            var enas = Assembly.GetEntryAssembly();
            if (enas != null)
            {
                //   Add(Assembly.GetExecutingAssembly());
                AddAssembly(Assembly.GetEntryAssembly());
                foreach (var item in enas.GetReferencedAssemblies())
                {
                    AddAssembly(Assembly.Load(item));
                }
            }
        }
#else
        static AssemblyCache()
        {
            var enas = Assembly.GetExecutingAssembly();
            if (enas != null)
            {
                //   Add(Assembly.GetExecutingAssembly());
                AddAssembly(Assembly.GetExecutingAssembly());
            }
        }
        #endif

    }

#pragma warning restore
}
