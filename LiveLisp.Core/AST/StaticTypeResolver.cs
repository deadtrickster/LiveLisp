namespace LiveLisp.Core.AST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
using System.Reflection;
    using LiveLisp.Core.Runtime;
    using LiveLisp.Core.Types;
using LiveLisp.Core.Compiler;

    public class StaticTypeResolver
    {
        private List<StaticTypeResolver> genericTypeArgs;
        public static StaticTypeResolver ObjectType = new StaticTypeResolver("System.Object");
        private string typeName;

        public StaticTypeResolver()
        {
            this.genericTypeArgs = new List<StaticTypeResolver>();
        }

        public StaticTypeResolver(string typeName)
        {
            this.genericTypeArgs = new List<StaticTypeResolver>();
            this.typeName = typeName;
        }

        public StaticTypeResolver(string typeName, List<StaticTypeResolver> genericTypeArgs)
        {
            this.genericTypeArgs = new List<StaticTypeResolver>();
            this.typeName = typeName;
            this.genericTypeArgs = genericTypeArgs;
        }

        public static StaticTypeResolver Create(string typename)
        {
            return new StaticTypeResolver(typename);
        }

        public static StaticTypeResolver Create(Type type)
        {
            return new ManuallyResolvedType(type);
        }

        public static StaticTypeResolver Create(string typename, List<StaticTypeResolver> typeargs)
        {
            return new StaticTypeResolver(typename, typeargs);
        }

        public static StaticTypeResolver Create(string typename, StaticTypeResolver typearg1)
        {
            List<StaticTypeResolver> typeargs = new List<StaticTypeResolver>();
            typeargs.Add(typearg1);
            return new StaticTypeResolver(typename, typeargs);
        }

        public static StaticTypeResolver Create(string typename, StaticTypeResolver typearg1, StaticTypeResolver typearg2)
        {
            List<StaticTypeResolver> typeargs = new List<StaticTypeResolver>();
            typeargs.Add(typearg1);
            typeargs.Add(typearg2);
            return new StaticTypeResolver(typename, typeargs);
        }

        public static implicit operator StaticTypeResolver(string typename)
        {
            return new StaticTypeResolver(typename);
        }

        public static implicit operator StaticTypeResolver(Type type)
        {
            return new ManuallyResolvedType(type);
        }

        public List<StaticTypeResolver> GenericTypeArgs
        {
            get
            {
                return this.genericTypeArgs;
            }
            set
            {
                this.genericTypeArgs = value;
            }
        }

        public virtual string TypeName
        {
            get
            {
                return this.typeName;
            }
            set
            {
                this.typeName = value;
            }
        }

        public virtual Type Type
        {
            get
            {
                Type ret;

                if (TypeCache.TryGetType(TypeName, out ret))
                    return ret;

                throw new FormattedException("type " + TypeName + " not found");
            }
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        internal virtual bool TryResolve(out Type t)
        {
            return TypeCache.TryGetType(TypeName, out t);
        }

        public static StaticTypeResolver Create(ClassDeclaration lambda_holder)
        {
            return new ClassDeclBasedTypeResolver(lambda_holder);
        }
    }

    public class ClassDeclBasedTypeResolver : StaticTypeResolver
    {
        internal ClassDeclaration _class_decl;

        public ClassDeclBasedTypeResolver(ClassDeclaration class_decl)
        {
            _class_decl = class_decl;
        }

        public override Type Type
        {
            get
            {
                if (_class_decl.CreatedType == null)
                {
                    throw new FormattedException("class " + _class_decl.Name + " still under construction.");
                }

                return _class_decl.CreatedType;
            }
        }

        internal override bool TryResolve(out Type t)
        {
            t = _class_decl.CreatedType;
            return t != null;
        }
    }

    public class ManuallyResolvedType : StaticTypeResolver
    {
        Type t;
        public ManuallyResolvedType(Type t)
        {
            this.t = t;
        }

        public override Type Type
        {
            get
            {
                return t;
            }
        }

        internal override bool TryResolve(out Type t)
        {
            t = this.t;
            return true;
        }
    }

    public enum IMethodBaseResolverType
    {
        Method,
        Constructor
    }

    public abstract class IMethodBaseResolver
    {
        public abstract MethodBase MethodBase
        {
            get;
        }

        public abstract IMethodBaseResolverType Type
        {
            get;
        }

        public static implicit operator IMethodBaseResolver(ConstructorInfo method)
        {
            return new ManuallyResolvedConstructor(method);
        }

        public static implicit operator IMethodBaseResolver(MethodInfo method)
        {
            return new ManuallyResolvedMethod(method);
        }

        public static implicit operator IMethodBaseResolver(MethodDeclaration method)
        {
            return new MethodDeclBasedMethodResolver(method);
        }
    }

    public class StaticMethodResolver : IMethodBaseResolver
    {
        StaticTypeResolver TypeName;
        string methodName;
        StaticTypeResolver[] argTypes;


        //TODO: rewrite without linq
        public virtual MethodInfo Method
        {
            get
            {
                /* Type t = TypeName.Type;

                 var methods = t.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name == methodName);

                 if (methods.Count() == 1)
                 {
                     return methods.First();
                 }
                 else if (methods.Count() == 0)
                 {
                     throw new FormattedException("Method " + methodName + " not found in type " + t.Name);
                 }
                 else
                 {
                     Type[] args = new Type[argTypes.Length];


                     for (int i = 0; i < argTypes.Length; i++)
                     {
                         args[i] = argTypes[i].Type;
                     }

                     var method = t.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public, null, args, null);

                     if(method == null)
                         throw new FormattedException("Method " + methodName + " not found in type " + t.Name);
                     return method;*/

                MethodInfo ret;
                if (MethodCache.TryGetMethod(TypeName.Type, methodName, argTypes, out ret))
                {
                    return ret;
                }

                throw new FormattedException("Method " + methodName + " not found in type " + TypeName == null ? "" : TypeName.TypeName);
            }
        }

        protected StaticMethodResolver()

        {
        }

        private StaticMethodResolver(StaticTypeResolver typeName, string methodName, StaticTypeResolver[] argTypes)
        {
            TypeName = typeName;
            this.methodName = methodName;
            this.argTypes = argTypes;
        }

        public override string ToString()
        {
            return Method.ToString();
        }

        internal static StaticMethodResolver Create(Symbol simple_name)
        {
            return new StaticMethodResolver(null, simple_name.Name.ToString(), null);
        }

        internal static StaticMethodResolver Create(StaticTypeResolver type, Symbol simple_name)
        {
            return new StaticMethodResolver(type, simple_name.Name.ToString(), null);
        }

        internal static StaticMethodResolver Create(StaticTypeResolver type, Symbol simple_name, List<StaticTypeResolver> args)
        {
            return new StaticMethodResolver(type, simple_name.Name.ToString(), args.ToArray());
        }

        internal static StaticMethodResolver Create(MethodInfo methodInfo)
        {
            return new ManuallyResolvedMethod(methodInfo);
        }

        internal static StaticMethodResolver Create(StaticTypeResolver typeName, string methodName, object[] args)
        {
            if (args.Length != 0)
            {
                StaticTypeResolver[] resolvers = new StaticTypeResolver[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    resolvers[i] = StaticTypeResolver.Create(args[i].GetType());
                }

                return new StaticMethodResolver(typeName, methodName, resolvers);
            }

            else
                return new StaticMethodResolver(typeName, methodName, null);
        }

        public static implicit operator StaticMethodResolver(MethodInfo method)
        {
            return new ManuallyResolvedMethod(method);
        }

        public static implicit operator StaticMethodResolver(MethodDeclaration method)
        {
            return new MethodDeclBasedMethodResolver(method);
        }

        #region IMethodBaseResolver Members

        public override MethodBase MethodBase
        {
            get { return Method; }
        }

        public override IMethodBaseResolverType Type
        {
            get { return IMethodBaseResolverType.Method; }
        }

        #endregion
    }

    public class MethodDeclBasedMethodResolver : StaticMethodResolver
    {
        MethodDeclaration _decl;

        public MethodDeclBasedMethodResolver(MethodDeclaration
            decl)
        {
            _decl = decl;
        }

        public override MethodBase MethodBase
        {
            get { return Method; }
        }

        public override IMethodBaseResolverType Type
        {
            get { return IMethodBaseResolverType.Method; }
        }

        public override MethodInfo Method
        {
            get
            {
                if (_decl.CreatedMethod == null)
                {
                    throw new FormattedException("method " + _decl.ToString() + " still under construction.");
                }

                return _decl.CreatedMethod;
            }
        }
    }

    class ManuallyResolvedMethod : StaticMethodResolver
    {
        MethodInfo method;

        public ManuallyResolvedMethod(MethodInfo method)
        {
            this.method = method;
        }

        public override MethodInfo Method
        {
            get
            {
                return method;
            }
        }
    }

    public class ConstructorDeclBasedConstructorResolver : StaticConstructorResolver
    {
        ConstructorDeclaration _decl;

        public ConstructorDeclBasedConstructorResolver(ConstructorDeclaration 
            decl)
        {
            _decl = decl;
        }

        public override MethodBase MethodBase
        {
            get { return Constructor; }
        }

        public override IMethodBaseResolverType Type
        {
            get { return IMethodBaseResolverType.Constructor; }
        }

        public override ConstructorInfo Constructor
        {
            get
            {
                if (_decl.CreatedConstructor == null)
                {
                    throw new FormattedException("constructor " + _decl.ToString() + " still under construction.");
                }

                return _decl.CreatedConstructor;
            }
        }
    }

    public class StaticConstructorResolver : IMethodBaseResolver
    {
        StaticTypeResolver _Type;

        StaticTypeResolver[] ArgTypes;

        protected StaticConstructorResolver() { }

        private StaticConstructorResolver(StaticTypeResolver type, StaticTypeResolver[] argtypes)
        {
            _Type = type;
            ArgTypes = argtypes;
        }

        public virtual ConstructorInfo Constructor
        {
            get
            {
                Type t = _Type.Type;
                ConstructorInfo ci;

                if (ArgTypes == null)
                {
                    ci = t.GetConstructor(System.Type.EmptyTypes);
                    if (ci == null && t.GetConstructors().Length == 1)
                        ci = t.GetConstructors()[0];

                }
                else
                {
                    Type[] types = new Type[ArgTypes.Length];

                    for (int i = 0; i < ArgTypes.Length; i++)
                    {
                        types[i] = ArgTypes[i].Type;
                    }

                    ci = t.GetConstructor(types);
                }
                if (ci == null)
                    throw new FormattedException("Constructor for type " + t + " not found");

                return ci;
            }
        }



        public static implicit operator StaticConstructorResolver(ConstructorInfo method)
        {
            return new ManuallyResolvedConstructor(method);
        }

        public static implicit operator StaticConstructorResolver(StaticTypeResolver type)
        {
            return new StaticConstructorResolver(type, null);
        }

        public static StaticConstructorResolver Create(StaticTypeResolver type, StaticTypeResolver[] argtypes)
        {
            return new StaticConstructorResolver(type, argtypes);
        }

        public static implicit operator StaticConstructorResolver(ConstructorDeclaration decl)
        {
            return new ConstructorDeclBasedConstructorResolver(decl);
        }

        #region IMethodBaseResolver Members

        public override MethodBase MethodBase
        {
            get { return Constructor; }
        }

        public override IMethodBaseResolverType Type
        {
            get { return IMethodBaseResolverType.Constructor; }
        }

        #endregion

        internal static StaticConstructorResolver Create(Type type, Type firstarg)
        {
            ConstructorInfo ci = type.GetConstructor(new Type[] { firstarg });
            if (ci == null)
                throw new FormattedException("Type " + type + " doesn't containts constructor with single parameter of type " + firstarg);

            return new ManuallyResolvedConstructor(ci);
        }

        internal static IMethodBaseResolver Create(StaticTypeResolver BaseType)
        {
            return new StaticConstructorResolver(BaseType, null);
        }
    }

    public class ManuallyResolvedConstructor : StaticConstructorResolver
    {
        ConstructorInfo _constructor;
        public ManuallyResolvedConstructor(ConstructorInfo constructor)
        {
            _constructor = constructor;
        }

        public override ConstructorInfo Constructor
        {
            get
            {
                return _constructor;
            }
        }
    }

    public class StaticFieldResolver
    {
        StaticTypeResolver typeResolver;
        public readonly string fieldName;

        protected StaticFieldResolver()
        {

        }

        public StaticFieldResolver(StaticTypeResolver type, string fieldName)
        {
            this.typeResolver = type;
            this.fieldName = fieldName;
        }

        public virtual FieldInfo Field
        {
            get
            {
                var ret = typeResolver.Type.GetField(fieldName);

                if (ret == null)
                    throw new FormattedException("Field with name " + fieldName + " not found in type " + typeResolver.TypeName);

                return ret;
            }
        }

        public override string ToString()
        {
            return Field.ToString();
        }

        public static implicit operator StaticFieldResolver(FieldInfo field)
        {
            return new ManuallyResolvedField(field);
        }

        public static implicit operator StaticFieldResolver(FieldDeclaration field)
        {
            return new FieldDeclarationBasedFieldResolver(field);
        }
    }

    class ManuallyResolvedField : StaticFieldResolver
    {
        public ManuallyResolvedField(FieldInfo field)
        {
            this.field = field;
        }
        FieldInfo field;

        public override FieldInfo Field
        {
            get
            {
                return field;
            }
        }
    }

    class FieldDeclarationBasedFieldResolver : StaticFieldResolver
    {
        public FieldDeclarationBasedFieldResolver(FieldDeclaration field)
        {
            this.field = field;
        }
        internal FieldDeclaration field;

        public override FieldInfo Field
        {
            get
            {
                if (field.CreatedField == null)
                {
                    throw new FormattedException("field " + field.ToString() + " still under construction.");
                }

                return field.CreatedField;
            }
        }
    }
}

