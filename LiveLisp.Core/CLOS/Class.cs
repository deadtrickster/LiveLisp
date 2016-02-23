using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using System.Collections;
using System.ComponentModel;
using Microsoft.Scripting.Runtime;
using System.Diagnostics;
using System.Reflection;
using LiveLisp.Core.Runtime;
using System.Dynamic;

using LinqExp = System.Linq.Expressions.Expression;
using LiveLisp.Core.BuiltIns.Conditions;
using LiveLisp.Core.CLOS.Standard_classes;

namespace LiveLisp.Core.CLOS
{
    public static class SomeCLOSHelpers
    {
        public static CLOSClass GetCLOSClass(this object obj)
        {
            CLOSInstance instance = obj as CLOSInstance;
            if (instance != null)
                return instance.Class;

            else
                return CLOSCLRClass.ConstructClass(obj.GetType());
        }
    }

    public enum LispObjectTypeCode
    {
        Symbol,
        Cons,
        Number,
        CLOSClass,
        CLOSInstance,
        CLOSSlot,
        String,
        Hashtable
    }
    public interface ILispObject
    {
        LispObjectTypeCode LispTypeCode
        {
            get;
        }
    }

    /*  public class CLOSClassBuilder
      {
          public CLOSClassBuilder(CLOSClass c)
          {

          }

          Symbol Name;
          protected Dictionary<int, object> _instanceSlots_init = new Dictionary<int, object>();

          public CLOSClassBuilder(Symbol name)
          {
              Name = name;
          }

          public CLOSClassBuilder(Symbol name, List<CLOSClassBuilder> bases)
          {
              Name = name;
          }

          public void AddSlot(Symbol name, object def_Val)
          {
              if (_instanceSlots_init.ContainsKey(name.Id))
                  throw new SimpleErrorException("Class " + Name + " already has slot " + name);

              _instanceSlots_init.Add(name.Id, def_Val);
          }

          public CLOSClass GetClass()
          {
              return new CLOSClass(Name, _instanceSlots_init);
          }

          internal bool HasSlot(Symbol name)
          {
              throw new NotImplementedException();
          }

          internal void ChangeSlot(Symbol name, object _defvalue)
          {
              if (_instanceSlots_init.ContainsKey(name.Id))
                  _instanceSlots_init[name.Id] = _defvalue;
              else
                  _instanceSlots_init.Add(name.Id, _defvalue);
          }
      }*/


    public enum SlotKind
    {
        _instance,
        _static,
        _method
    }

    [DebuggerDisplay("{ToString()}")]
    public class CLOSClass : DynamicCLOSInstance, ILispObject
    {
        public static CLOSClass SelfClass;

        public static CLOSClass T;

        static CLOSClass()
        {
            SelfClass = new CLOSClass();
        }

        protected CLOSClass()
        {
            Bases = new List<CLOSClass>();
            Bases.Add(CLOS_T.Instance);
        }

        public CLOSClass(Symbol name, Dictionary<Symbol, object> instanceSlots)
            :this()
        {
            _name = name;
            _MetainstanceSlots = instanceSlots;
            _MetastaticSlots = new Dictionary<int, object>();
            _baseClasses.Add(CLOSCLRClass.ObjectClass);

            CLOSTypeTable.Instance.Add(this);
        }

        List<CLOSClass> _baseClasses = new List<CLOSClass>();

        public List<CLOSClass> Bases
        {
            get { return _baseClasses; }
            set { _baseClasses = value; }
        }

        protected Symbol _name;

        public Symbol Name
        {
            get { return _name; }
        }

        public int Id
        {
            get { return _name.Id; }
        }

        #region ILispObject Members

        LispObjectTypeCode ILispObject.LispTypeCode
        {
            get { return LispObjectTypeCode.CLOSClass; }
        }

        #endregion

        public override string ToString()
        {
            return "<CLOS CLASS " + Name + " class id: " + Id + " object id: " + DisplayID + ">";
        }

        internal bool IsSubTypeOf(CLOSClass c2)
        {
            if (this == c2)
                return true;

            for (int i = 0; i < Bases.Count; i++)
            {
                if (Bases[i].IsSubTypeOf(c2))
                    return true;
            }

            return false;
        }

        public virtual object CreateInstance()
        {
            DynamicCLOSInstance dci = new DynamicCLOSInstance(this);
            return dci;
        }

        public virtual object CreateInstance(object[] args)
        {
            throw new NotImplementedException();
        }

        public virtual void SetSlot(object instance, Symbol symbol, object _format)
        {
            throw new NotImplementedException();
        }

        List<WeakReference> instances = new List<WeakReference>();

        bool wait_for_end = false;

        public void StartChanges()
        {
            wait_for_end = true;

        }

        public void EndChanges()
        {
            wait_for_end = false;

            ChangeStructure();
        }

        void RemoveDeadLinks()
        {
            instances.RemoveAll(wr => !wr.IsAlive);
        }

        private void ChangeStructure()
        {
            bool needcleanup = false;
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i].IsAlive)
                    _ChangeStructure(instances[i]);
                else
                {
                    needcleanup = true;
                }
            }

            if (needcleanup)
                RemoveDeadLinks();
        }

        private void _ChangeStructure(WeakReference weakReference)
        {
            throw new NotImplementedException();
        }

        protected Dictionary<Symbol, object> _MetainstanceSlots = new Dictionary<Symbol, object>();

        protected Dictionary<int, object> _MetastaticSlots = new Dictionary<int, object>();

        public void AddSlot(Symbol name, SlotKind kind, object initial_value)
        {
            switch (kind)
            {
                case SlotKind._instance:
                    _MetainstanceSlots.Add(name, initial_value);
                    break;
                case SlotKind._static:
                    _MetastaticSlots.Add(name.Id, initial_value);
                    break;
                case SlotKind._method:
                    throw new NotImplementedException();
                default:
                    break;
            }

            if (!wait_for_end)
            {
                ChangeStructure();
            }
        }

        internal virtual DynamicMetaObject GetDynamicMetaObject(System.Linq.Expressions.Expression parameter, CLOSInstance instance)
        {
            if (instance is CLRCLOSInstance)
            {
                return new DynamicMetaObject(parameter, BindingRestrictions.Empty, instance);
            }
            else
            {
                return new DCDynamicMetaObject(parameter, instance);
            }
        }

        internal Dictionary<Symbol, object> InstanceSlots { get { return _MetainstanceSlots; } }

        public virtual bool Is(object obj)
        {
            if (obj is CLRCLOSInstance)
            {
                return (obj as CLRCLOSInstance).Class == this;
            }

            return false;
        }

        public virtual CLOSInstance The(object obj)
        {
            throw new NotImplementedException();
        }
    }

    // wrapper around clr objects
    public class CLOSCLRClass : CLOSClass
    {
        public Type Type;

        protected CLOSCLRClass(Type type)
        {
            Type = type;

        }

        public static CLOSCLRClass ObjectClass;

        static CLOSCLRClass()
        {
            ObjectClass = new CLOSCLRClass(typeof(object));
        }

        static Dictionary<Type, CLOSCLRClass> _cache = new Dictionary<Type, CLOSCLRClass>();

        internal static CLOSClass ConstructClass(Type type)
        {
            CLOSCLRClass ret;
            if (_cache.TryGetValue(type, out ret))
                return ret;

            ret = new CLOSCLRClass(type);
            _cache.Add(type, ret);

            return ret;
        }

        public override object CreateInstance()
        {
            return Activator.CreateInstance(Type);
        }

        public override object CreateInstance(object[] args)
        {
            return Activator.CreateInstance(Type, args);
        }

        Dictionary<string, MemberInfo> slots = new Dictionary<string, MemberInfo>();

        public override void SetSlot(object instance, Symbol symbol, object value)
        {
            if (!Type.IsInstanceOfType(instance))
                throw new ArgumentException();

            string name;

            if (Settings.ReflectionMode == ReflectionMode.NameAndPackage)
                name = symbol.ToString(true);
            else
                name = symbol.Name;

            MemberInfo mi;
            if (!slots.TryGetValue(name, out mi))
            {
                var ms = Type.GetMember(name, MemberTypes.Property | MemberTypes.Field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (ms.Length == 0)
                {
                    ms = Type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    for (int i = 0; i < ms.Length; i++)
                    {
                        var attrs = ms[i].GetCustomAttributes(typeof(CLOSSlotNameAttribute), true);

                        if (attrs.Length != 0)
                        {
                            for (int j = 0; j < attrs.Length; j++)
                            {
                                if ((attrs[j] as CLOSSlotNameAttribute).Name == name)
                                {
                                    mi = ms[i];
                                    break;
                                }
                            }
                        }

                        if (mi != null)
                            break;
                    }
                }
                else if (ms.Length == 1)
                {
                    mi = ms[0];

                    if (mi.MemberType == MemberTypes.Property)
                    {
                        var indexes = (mi as PropertyInfo).GetIndexParameters();
                        if (indexes.Length != 0)
                            throw new ArgumentException();
                    }
                    else if (mi.MemberType != MemberTypes.Field)
                        throw new ArgumentException();

                    if (Settings.MetadataCachingModeInClos == MetadataCachingModeInClos.WhenReflected)
                    {
                        slots.Add(name, mi);
                    }
                }
            }

            SetValue(instance, mi, value);
        }

        private void SetValue(object instance, MemberInfo mi, object value)
        {
            if (mi.MemberType == MemberTypes.Field)
                (mi as FieldInfo).SetValue(instance, value); // should we move to dynamic generated code?
            else
                (mi as PropertyInfo).SetValue(instance, value, null);
        }
    }

    public abstract class CLOSInstance : DynamicObject, ILispObject, INotifyPropertyChanged
    {
        protected CLOSClass _class = null;

        public virtual CLOSClass Class
        {
            get { return _class; }
        }

        public abstract object this[Symbol name]
        {
            get;
            set;
        }

        #region ILispObject Members

        public LispObjectTypeCode LispTypeCode
        {
            get { return LispObjectTypeCode.CLOSInstance; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public string DisplayID
        {
            get
            {
                return string.Format("0x{0:X16}", IdDispenser.GetId(this));
            }
        }

        public abstract bool Eq(object obj);

        public abstract IEnumerable<Symbol> GetAllFields();


        public abstract object ToNearestCLRType();
    }

    public class CLRCLOSInstance : CLOSInstance
    {
        object instance;

        public CLRCLOSInstance(object obj, CLOSClass _class)
        {
            instance = obj;
            this._class = _class;
        }

        protected CLRCLOSInstance()
        {

        }

        public override CLOSClass Class
        {
            get
            {
                if(_class==null)
                    _class = CLOSCLRClass.ConstructClass(instance==null?this.GetType():instance.GetType());

                return _class;
            }
        }

        public override object this[Symbol name]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool Eq(object obj)
        {
            return object.ReferenceEquals(this, obj);
        }

        public override IEnumerable<Symbol> GetAllFields()
        {
            throw new NotImplementedException();
        }

        public override object ToNearestCLRType()
        {
            return instance;
        }
    }

    public class DynamicCLOSInstance : CLOSInstance
    {
        internal DynamicCLOSInstance()
        {

        }

        public DynamicCLOSInstance(CLOSClass c)
        {
            if (c != null)
                Initialize(c);
            _class = c;
        }

        private void Initialize(CLOSClass c)
        {
            for (int i = 0; i < c.Bases.Count; i++)
            {
                Initialize(c.Bases[i]);
            }

            foreach (var item in c.InstanceSlots)
            {
                if (_instanceSlots.ContainsKey(item.Key.Id))
                    _instanceSlots[item.Key.Id] = item.Value;
                else
                    _instanceSlots.Add(item.Key.Id, item.Value);
            }
        }


        protected Dictionary<int, object> _instanceSlots = new Dictionary<int, object>();

        public override object this[Symbol slot]
        {
            get
            {
                if (_instanceSlots.ContainsKey(slot.Id))
                    return _instanceSlots[slot.Id];
                else
                    throw new MissingMemberException("Slot: " + slot + " is unlnown in class " + _class);
            }
            set
            {
                if (_instanceSlots.ContainsKey(slot.Id))
                    _instanceSlots[slot.Id] = value;
                else
                    throw new MissingMemberException("Slot: " + slot + " is unlnown in class " + _class);
            }
        }

        public object this[int id]
        {
            get
            {
                if (_instanceSlots.ContainsKey(id))
                    return _instanceSlots[id];
                else
                    throw new MissingMemberException("Slot with id: " + id + " is unlnown in instance of class " + _class);
            }
            set
            {
                if (_instanceSlots.ContainsKey(id))
                    _instanceSlots[id] = value;
                else
                    throw new MissingMemberException("Slot with id: " + id + " is unlnown in instance of class " + _class);
            }
        }



        public override string ToString()
        {
            return "<" + _class.Name + " " + DisplayID + ">";
        }

        public override bool Eq(object obj)
        {
            return object.ReferenceEquals(this, obj);
        }

        public override IEnumerable<Symbol> GetAllFields()
        {
            List<Symbol> ret = new List<Symbol>();
            foreach (var item in _instanceSlots.Keys)
            {
                ret.Add(SymbolTable.GetSymbol(item));
            }

            return ret;
        }

        public override object ToNearestCLRType()
        {
            throw new NotImplementedException();
        }
    }


    class DCDynamicMetaObject : DynamicMetaObject
    {
        CLOSInstance instance;

        public DCDynamicMetaObject(System.Linq.Expressions.Expression parameter, CLOSInstance instance)
            : base(parameter, BindingRestrictions.Empty, instance)
        {
            this.instance = instance;
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
             var names = GetSymbol(binder);

            switch (names.Count())
            {
                case 0:
                    return base.BindGetMember(binder);
                case 1:
                    return new DynamicMetaObject(LinqExp.Constant(instance[names.ElementAt(0)]), binder.FallbackGetMember(this).Restrictions);
                default:
                    throw new InvalidOperationException("Ambiguity in field " + binder.Name + " resolving for instance " + instance + " of class " + instance.Class);
            }
        }

        private IEnumerable<Symbol> GetSymbol(dynamic binder)
        {
            string memeber_name = "";
            switch (Readtable.Current.ReadtableCase)
            {
                case ReadtableCases.Upcase:
                    memeber_name = binder.Name.ToUpper();
                    break;
                case ReadtableCases.Downcase:
                    memeber_name = binder.Name.ToLower();
                    break;
                case ReadtableCases.Preserve:
                    memeber_name = binder.Name;
                    break;
                case ReadtableCases.Invert:
                    memeber_name = binder.Name.InvertCase();
                    break;
                default:
                    break;
            }

            var temp = memeber_name.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
            Package package;
            Symbol symbol;
            if (temp.Length == 2)
            {
                package = PackagesCollection.FindByNameOrNickname(temp[0]);

                if (!package.TryGetSymbol(temp[1], out symbol))
                {
                    throw new InvalidOperationException("Package " + package.Name + " does not export symbol " + temp[1]);
                }

                return instance.GetAllFields().Where(s => s == symbol);
            }
            else
            {
                package = Package.Current;
                if (!Package.Current.TryGetAccessibleSymbol(memeber_name, out symbol))
                {
                    throw new InvalidOperationException("Current package " + package + " has no accessible symbol " + memeber_name);
                }


                return instance.GetAllFields().Where(s => s.Name == memeber_name);
            }
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {

            var names = GetSymbol(binder);

            switch (names.Count())
            {
                case 0:
                    return base.BindSetMember(binder, value);
                case 1:
                    instance[names.ElementAt(0)] = value.Value;
                    return new DynamicMetaObject(LinqExp.Constant(value.Value), binder.FallbackSetMember(this, value).Restrictions);
                default:
                    throw new InvalidOperationException("Ambiguity in field " + binder.Name + " resolving for instance " + instance + " of class " + instance.Class);
            }
        }
    }


}
