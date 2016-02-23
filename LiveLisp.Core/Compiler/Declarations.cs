using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST;
using LiveLisp.Core.Types;
using System.Collections.ObjectModel;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LiveLisp.Core.Compiler
{
    public class VariableDeclaration
    {
        public readonly StaticTypeResolver Type;
        public readonly String Name;
        public Int32 Id;

        public VariableDeclaration(StaticTypeResolver type, String name, Int32 id)
        {
            Type = type;
            Name = name;
            Id = id;
        }

        public override string ToString()
        {
            return ".local " + Type.ToString() + " " + Name;
        }
    }

    public class VariablesCollection : KeyedCollection<String, VariableDeclaration>
    {
        protected override String GetKeyForItem(VariableDeclaration item)
        {
            return item.Name;
        }

        public VariableDeclaration WithUniqueName(StaticTypeResolver type)
        {
            VariableDeclaration new_var = new VariableDeclaration(type, SuggestName(), Count);
            Add(new_var);
            return new_var;
        }

        int counter = 0;
        public string SuggestName()
        {
            string nextname = "local_$" + counter++;
            if (Contains(nextname))
                return SuggestName();
            else
                return nextname;
        }
    }

    public class LabelDeclaration
    {
        public readonly string Name;
        public Label Label;

        public LabelDeclaration(string name)
        {
            Name
                = name;
        }
    }

    public class LabelsCollection : KeyedCollection<string, LabelDeclaration>
    {
        protected override string GetKeyForItem(LabelDeclaration item)
        {
            return item.Name;
        }

        internal LabelDeclaration WithUniqueName()
        {
            LabelDeclaration new_label = new LabelDeclaration(SuggestName());
            Add(new_label);
            return new_label;
        }

        int counter = 0;
        public string SuggestName()
        {
            string nextname = "label_$" + counter++;
            if (Contains(nextname))
                return SuggestName();
            else
                return nextname;
        }
    }

    public class ClassDeclaration
    {

        public ClassDeclaration()
        {
            BaseType = typeof(object);
        }

        public TypeAttributes Attributes;

        public string Name;

        public StaticTypeResolver BaseType;

        public List<StaticTypeResolver> Interfaces = new List<StaticTypeResolver>();

        public List<CustomAttributeDeclaration> CustomAttributes = new List<CustomAttributeDeclaration>();

        public FieldsCollection Fields = new FieldsCollection();

        public List<ConstructorDeclaration> Constructors = new List<ConstructorDeclaration>();

        public List<MethodDeclaration> Methods = new List<MethodDeclaration>();

        TypeConstructorDeclaration _TypeConstructor;

        public TypeConstructorDeclaration TypeConstructor
        {
            get
            {
                if (_TypeConstructor == null)
                    _TypeConstructor = new TypeConstructorDeclaration();

                return _TypeConstructor;
            }
        }

        public EventsCollection Events;

        Type _createdType;
        public Type CreatedType
        {
            get { return _createdType; }
        }

        public void Compile(CompilationContext context)
        {
            _createdType = null;
        }

        TypeBuilder tbuilder;

        internal void CreateBuilders(ModuleBuilder module)
        {
            Type baseType;
            if (!BaseType.TryResolve(out baseType))
            {
                throw new FormattedException("Enable to resolve base type " + BaseType.ToString());
            }
            _createdType = tbuilder = module.DefineType(Name, Attributes | TypeAttributes.Class, baseType);

            if (Constructors.Count == 0)
            {
                AddDefaultContsructor();
            }
            for (int i = 0; i < Constructors.Count; i++)
            {
                Constructors[i].CreateBuilder(tbuilder);
            }

            for (int i = 0; i < Methods.Count; i++)
            {
                Methods[i].CreateBuilder(tbuilder);
            }

            for (int i = 0; i < Fields.Count; i++)
            {
                Fields[i].CreateBuilder(tbuilder);
            }

            if (_TypeConstructor != null)
                _TypeConstructor.CreateBuilder(tbuilder);
        }

        private void AddDefaultContsructor()
        {
            ConstructorDeclaration cdecl = new ConstructorDeclaration();
            cdecl.Attributes = MethodAttributes.Public;
            InstructionsBlock instructionsBlock = cdecl.Instructions;
            instructionsBlock.Add(new LdargInstruction(0));
            instructionsBlock.Add(new CallInstruction(StaticConstructorResolver.Create(BaseType)));
            instructionsBlock.Add(new RetInstruction());
            Constructors.Add(cdecl);
        }

        internal void Create()
        {
            for (int i = 0; i < Constructors.Count; i++)
            {
                Constructors[i].Create(tbuilder);
            }

            for (int i = 0; i < Methods.Count; i++)
            {
                Methods[i].Create(tbuilder);
            }

            if (_TypeConstructor != null)
                _TypeConstructor.Create(tbuilder);

            _createdType = tbuilder.CreateType();

            for (int i = 0; i < Fields.Count; i++)
            {
                Fields[i].TypeCreated(_createdType);
            }

            for (int i = 0; i < Constructors.Count; i++)
            {
                Constructors[i].TypeCreated(_createdType);
            }

            for (int i = 0; i < Methods.Count; i++)
            {
                Methods[i].TypeCreated(_createdType);
            }

            if (_TypeConstructor != null)
                _TypeConstructor.TypeCreated(tbuilder);
        }

        FieldsCollection generatedFields = new FieldsCollection();

        internal FieldDeclaration NewGeneratedField(string name, Type type)
        {
            return NewGeneratedField(name, (StaticTypeResolver)type);
        }

        internal FieldDeclaration NewGeneratedField(string name, StaticTypeResolver type)
        {

            FieldDeclaration field;

            string tmp = name;
            if (Fields.Contains(tmp))
            {
                int counter = 1;

                do
                {
                    tmp = name + "$" + counter++;
                }
                while (Fields.Contains(tmp));
            }


            field = new FieldDeclaration(default(FieldAttributes), type, name);
            Fields.Add(field);
            generatedFields.Add(field);

            field.CompilerGenerated = true;

            return field;
        }

        internal void AddField(FieldDeclaration newField)
        {
            if (Fields.Contains(newField.Name))
            {
                if (generatedFields.Contains(newField.Name))
                {
                    FieldDeclaration generated = generatedFields[newField.Name];

                    int counter = 1;
                    string name;
                    do
                    {
                        name = generated.Name + "$" + counter++;
                    }
                    while (Fields.Contains(name));

                    generated.Name = name;
                }
                else
                {
                    new FormattedException("Field with name " + newField.Name + " already added to class " + Name);
                }
            }

            Fields.Add(newField);
        }
    }

    public class LambdaDeclaration : ClassDeclaration
    {

    }

    public class CustomAttributeDeclaration
    {
        public readonly StaticTypeResolver Type;
        public readonly List<object> Args;
    }

    public class FieldDeclaration
    {
        public FieldAttributes Attributes;
        public readonly List<CustomAttributeDeclaration> CustomAttributes;
        public readonly StaticTypeResolver fType;
        public String Name;

        public bool CompilerGenerated
        {
            get;
            set;
        }
        public FieldDeclaration()
        {

        }

        public FieldDeclaration(FieldAttributes attributes, StaticTypeResolver type, string name)
        {
            Attributes = attributes;
            fType = type;
            Name = name;
        }
        public FieldInfo CreatedField;
        FieldBuilder fbuilder;
        internal void CreateBuilder(TypeBuilder tbuilder)
        {
            Type _ftype;

            if (!fType.TryResolve(out _ftype))
            {
                throw new FormattedException("Enable to resolve field " + Name + " type " + fType);
            }

           CreatedField = fbuilder = tbuilder.DefineField(Name, _ftype, Attributes);
           if (CompilerGenerated)
           {
               CustomAttributeBuilder cab = new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes), new object[0]);
               fbuilder.SetCustomAttribute(cab);
           }
        }

        internal void TypeCreated(Type _createdType)
        {

            // note when field is not public this will return null;
            CreatedField = _createdType.GetField(Name);
        }
    }

    public class FieldsCollection : KeyedCollection<string, FieldDeclaration>
    {
        protected override string GetKeyForItem(FieldDeclaration item)
        {
            return item.Name;
        }
    }

    public class _MethodDeclaration
    {
        public MethodAttributes Attributes;
        public List<CustomAttributeDeclaration> CustomAttributes = new List<CustomAttributeDeclaration>();
        public String Name;
        public ParameterDeclarationsCollection Args = new ParameterDeclarationsCollection();
        public StaticTypeResolver ReturnType;
        public InstructionsBlock Instructions = new InstructionsBlock();

        public InstructionsBlock MethodProlog = new InstructionsBlock();

        internal virtual void CreateBuilder(TypeBuilder tbuilder)
        {
            throw new NotImplementedException();
        }
    }

    public class MethodDeclaration : _MethodDeclaration
    {
        private Type[] _ArgsTypes;
        MethodInfo method;

        public MethodInfo CreatedMethod
        {
            get { return method; }
        }

        MethodBuilder mbuilder;

        internal override void CreateBuilder(TypeBuilder tbuilder)
        {
            Type returnType;

            if (!ReturnType.TryResolve(out returnType))
            {
                throw new FormattedException("Enable to resolve return type " + ReturnType.ToString());
            }

            _ArgsTypes = new Type[Args.Count];
            for (int i = 0; i < Args.Count; i++)
            {
                if (!Args[i].Type.TryResolve(out _ArgsTypes[i]))
                {
                    throw new FormattedException("Enable to resolve parameter " + Args[i].Name + " type " + Args[i].Type);
                }
            }

           method = mbuilder = tbuilder.DefineMethod(Name, Attributes, CallingConventions.Standard, returnType, _ArgsTypes);
        }

        internal virtual void Create(TypeBuilder tbuilder)
        {
            ILGenerator ilgen = mbuilder.GetILGenerator();

            MethodProlog.Create(ilgen);
            Instructions.Create(ilgen);
        }

        internal virtual void TypeCreated(Type _createdType)
        {
            method = _createdType.GetMethod(Name, _ArgsTypes);

            if (method == null)
                throw new InvalidOperationException("something going wrong");
        }
    }

    public class ParameterDeclaration
    {
        public String Name;
        public ParameterAttributes Attributes;
        List<CustomAttributeDeclaration> CustomAttributes;
        public StaticTypeResolver Type;

        public ParameterDeclaration(string Name)
            : this(Name, new List<CustomAttributeDeclaration>())
        {
        }
        public ParameterDeclaration(string Name, List<CustomAttributeDeclaration> CustomAttributes)
            : this(Name, ParameterAttributes.In, CustomAttributes)
        {
        }
        public ParameterDeclaration(string Name, ParameterAttributes Attributes, List<CustomAttributeDeclaration> CustomAttributes)
            : this(Name, Attributes, CustomAttributes, typeof(object))
        {
        }
        public ParameterDeclaration(string Name, StaticTypeResolver Type)
            : this(Name, ParameterAttributes.None, new List<CustomAttributeDeclaration>(), Type)
        {
        }

        public ParameterDeclaration(string Name, ParameterAttributes Attributes, List<CustomAttributeDeclaration> CustomAttributes, StaticTypeResolver Type)
        {
            this.Name = Name;
            this.Attributes = Attributes;
            this.CustomAttributes = CustomAttributes;
            this.Type = Type;
        }
    }

    public class ParameterDeclarationsCollection : KeyedCollection<string, ParameterDeclaration>
    {
        protected override string GetKeyForItem(ParameterDeclaration item)
        {
            return item.Name;
        }

        internal void AddNewAutoGenerated(string prefix, StaticTypeResolver staticTypeResolver)
        {
            ParameterDeclaration new_var = new ParameterDeclaration(SuggestName(prefix), staticTypeResolver);
            Add(new_var);
        }

        int counter = 0;
        public string SuggestName(string prefix)
        {
            string nextname = prefix + counter++;
            if (Contains(nextname))
                return SuggestName(prefix);
            else
                return nextname;
        }
    }

    public class ConstructorDeclaration : MethodDeclaration
    {
        private Type[] _ArgsTypes;
        ConstructorInfo ctor;

        public ConstructorInfo CreatedConstructor
        {
            get { return ctor; }
        }

        ConstructorBuilder cbuilder;
        internal override void CreateBuilder(TypeBuilder tbuilder)
        {
            _ArgsTypes = new Type[Args.Count];
            for (int i = 0; i < Args.Count; i++)
            {
                if (!Args[i].Type.TryResolve(out _ArgsTypes[i]))
                {
                    throw new FormattedException("Enable to resolve parameter " + Args[i].Name + " type " + Args[i].Type);
                }
            }

           ctor = cbuilder = tbuilder.DefineConstructor(Attributes, CallingConventions.Standard, _ArgsTypes);
        }

        
        internal override void Create(TypeBuilder tbuilder)
        {
            ILGenerator ilgen = cbuilder.GetILGenerator();
            MethodProlog.Create(ilgen);
            Instructions.Create(ilgen);
        }

        internal override void TypeCreated(Type _createdType)
        {
            ctor = _createdType.GetConstructor(_ArgsTypes);
        }
    }

    public class TypeConstructorDeclaration : MethodDeclaration
    {
        ConstructorBuilder cbuilder;

        internal override void CreateBuilder(TypeBuilder tbuilder)
        {
           cbuilder = tbuilder.DefineTypeInitializer();
        }

        internal override void Create(TypeBuilder _createdType)
        {
            ILGenerator ilgen = cbuilder.GetILGenerator();
            MethodProlog.Create(ilgen);
            Instructions.Add(new RetInstruction());
            Instructions.Create(ilgen);
        }

        internal override void TypeCreated(Type _createdType)
        {
            
        }
    }

    public class DelegateDeclaration
    {
        public readonly TypeAttributes Attributes;
        public readonly List<CustomAttributeDeclaration> CustomAttributes;
        public readonly Symbol Name;
        public readonly VariablesCollection Args;
        public readonly StaticTypeResolver ReturnType;
    }

    public class EventDeclaration
    {
        public readonly FieldAttributes Attributes;
        public readonly List<CustomAttributeDeclaration> CustomAttributes;
        public readonly DelegateDeclaration Type;
        public readonly Symbol Name;

        public readonly MethodDeclaration AddMethod;
        public readonly MethodDeclaration RemoveMethod;
    }

    public class EventsCollection : KeyedCollection<Symbol, EventDeclaration>
    {
        protected override Symbol GetKeyForItem(EventDeclaration item)
        {
            return item.Name;
        }
    }

}
