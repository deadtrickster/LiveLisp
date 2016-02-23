using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.CLOS
{
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

    public enum CLOSSlotAttributes
    {
        Public,
        Protected,
        Private,

        Constant,
        PerThread
    }

    public class CLOSSlot : ILispObject
    {
        #region ILispObject Members

        public LispObjectTypeCode LispTypeCode
        {
            get { return LispObjectTypeCode.CLOSSlot; }
        }

        #endregion

        CLOSSlotAttributes attributes;

        public CLOSSlotAttributes Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
    }

    public class CLOSValueSlot : CLOSSlot
    {
        Symbol _name;

        public Symbol Name
        {
            get { return _name; }
            set { _name = value; }
        }

        bool _isConstant;

        public bool IsConstant
        {
            get { return _isConstant; }
            set { _isConstant = value; }
        }

        int id;

        public int Id
        {
            get { return id; }
        }

        #region value_related
        object _value = UnboundValue.Unbound;

        [ThreadStatic]
        static Dictionary<int, object> perThreadStore;

        public bool UsePerThreadStore;

        public virtual object Value
        {
            get
            {
                if (!Boundp)
                {
                    throw new UnboundVariableException("Unbound variable {0}.", _name);
                }
                if (!UsePerThreadStore)
                {
                    return _value;
                }

                if (perThreadStore.ContainsKey(id))
                    return perThreadStore[id];
                else
                    return _value;
            }
            set
            {
                if (IsConstant)
                {
                    new SimpleErrorException("The symbol {0} has been declared constant, and may not be assigned to {1}.", _name, value);
                }
                if (!UsePerThreadStore)
                    _value = value;

                if (perThreadStore == null)
                {
                    if (value != UnboundValue.Unbound)
                    {
                        perThreadStore = new Dictionary<int, object>();
                        perThreadStore.Add(id, value);
                    }
                }
                else
                {
                    if (value != UnboundValue.Unbound)
                    {
                        perThreadStore[id] = value;
                    }
                    else if (perThreadStore.ContainsKey(id))
                        perThreadStore.Remove(id);
                }
            }
        }

        public virtual object RawValue
        {
            get
            {
                if (!UsePerThreadStore)
                {
                    return _value;
                }

                if (perThreadStore.ContainsKey(Id))
                    return perThreadStore[Id];
                else
                    return _value;
            }
            set
            {
                if (IsConstant)
                {
                    new SimpleErrorException("The slot {0} has been declared constant, and may not be assigned to {1}.", value);
                }
                if (!UsePerThreadStore)
                    _value = value;

                if (perThreadStore == null)
                {
                    if (value != UnboundValue.Unbound)
                    {
                        perThreadStore = new Dictionary<int, object>();
                        perThreadStore.Add(Id, value);
                    }
                }
                else
                {
                    if (value != UnboundValue.Unbound)
                    {
                        perThreadStore[Id] = value;
                    }
                    else if (perThreadStore.ContainsKey(Id))
                        perThreadStore.Remove(Id);
                }
            }
        }

        public virtual bool Boundp
        {
            get
            {
                if (!UsePerThreadStore)
                    return _value != UnboundValue.Unbound;

                if (perThreadStore == null)
                {
                    perThreadStore = new Dictionary<int, object>();
                    return _value != UnboundValue.Unbound;
                }
                else if (perThreadStore.ContainsKey(Id))
                    return true;
                else
                    return _value != UnboundValue.Unbound;
            }
        }
        #endregion

        #region custom_attributes
        List<object> customAttributes;

        public List<object> CustomAttributes
        {
            get { return customAttributes; }
            set { customAttributes = value; }
        }
        #endregion

        public CLOSValueSlot(Symbol _name, int id)
        {
            this._name = _name;
            this.id = id;
        }
    }


    public class CLOSClass : CLOSClassInstance /*, IDynamicObject */
    {
        

        Symbol name;

        public int Id
        {
            get { return name.Id; }
        }

        internal bool InheritFrom(CLOSClass cc)
        {
            throw new NotImplementedException();
        }

        public CLOSClass(Symbol name)
        {
            this.name = name;
        }
    }


    /// <summary>
    /// каждый объект clos это экземпляр этого типа
    /// классы CLOS тоже экземпляры этого типа
    /// 
    /// все .net типы не наследуемые от CLOSClassInstance считаются встроенными
    /// 
    /// однако и для таких типов можно ввести "декоратор" наследуемый от CLOSClassInstance и для построения экземпляра CLOSClass использующий отражение
    /// </summary>
    public class CLOSClassInstance : ILispObject, INotifyPropertyChanged, INotifyPropertyChanging  {


        CLOSClass cclass;

        public CLOSClass Cclass
        {
            get { return cclass; }
            set { cclass = value; }
        }

        public bool Is(CLOSClass cc)
        {
            return cclass.InheritFrom(cc);
        }

        #region ILispObject Members

        public LispObjectTypeCode LispTypeCode
        {
            get { return LispObjectTypeCode.CLOSInstance; }
        }

        #endregion

        public object GetSlotValue(Symbol slot)
        {
            throw new NotImplementedException();
        }

        public void SetSlotValue(Symbol slot, object value)
        {
            throw new NotImplementedException();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;


        protected virtual void OnPropertyChanging(string property)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, new PropertyChangingEventArgs(property));
        }

        #endregion
    }
}
