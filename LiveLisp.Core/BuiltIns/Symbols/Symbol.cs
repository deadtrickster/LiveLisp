using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.CLOS;
//using LiveLisp.Core.BuiltIns.Strings;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.BuiltIns.Numbers;
using LiveLisp.Core.BuiltIns.Conditions;
using System.Diagnostics;

namespace LiveLisp.Core.Types
{
    [DebuggerDisplay("{ToString(true)}")]
    public class Symbol : CLRCLOSInstance
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Symbol(string name, int id)
            : this(name, id, null)
        {
        }

        public Symbol(string name)
            : this(name, GetNextFreeTempSymbolId(), null)
        {

        }
        static int currenttempid = -1;
        private static int GetNextFreeTempSymbolId()
        {
            return currenttempid--;
        }

        internal Symbol(string name, int id, Package homePackage)
        {
            _package = homePackage;
            _name = name;
            _Id = id;
            _function = new UnboundFunction(this); // TODO: отказаться от этого. Использовать сравнение с null и undefined-function-condition
          
            PropertyList = new SurrogateNilCons();
        }

        int _Id;

        public int Id
        {
            get { return _Id; }
            internal set { _Id = value; }
        }

        Package _package;

        public Package Package
        {
            get { return _package; }
            set { _package = value; }
        }

        object _value = UnboundValue.Unbound;

        [ThreadStatic]
        static Dictionary<int, object> perThreadStore;

        public bool UsePerThreadStore;

        public virtual dynamic Value
        {
            get
            {
                if (!Boundp)
                {
                    return ConditionsDictionary.UnboundVariable(this);
                }
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
                object val = value;
                if (IsConstant)
                {
                    new SimpleErrorException("The symbol {0} has been declared constant, and may not be assigned to {1}.", _name, value);
                }
                if (!UsePerThreadStore)
                    _value = value;

                if (perThreadStore == null)
                {
                    if (val != UnboundValue.Unbound)
                    {
                        perThreadStore = new Dictionary<int, object>();
                        perThreadStore.Add(Id, value);
                    }
                }
                else
                {
                    if (val != UnboundValue.Unbound)
                    {
                        perThreadStore[Id] = val;
                    }
                    else if (perThreadStore.ContainsKey(Id))
                        perThreadStore.Remove(Id);
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
                    new SimpleErrorException("The symbol {0} has been declared constant, and may not be assigned to {1}.", _name, value);
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

        LispFunction _function;

        public LispFunction Function
        {
            get { return _function; }
            set { _function = value; }
        }

        public bool FBound
        {
            get { return !_function.IsUnbound; }
            set { if(!value) _function = new UnboundFunction(this); }
        }

        LispFunction _setf_function;
        public LispFunction SetfFunction
        {
            get { if (_setf_function == null) throw new UnboundFunctionException(this.ToString()); return _setf_function; }
            set { _setf_function = value; }
        }

        public bool SetfFBound
        {
            get { return _setf_function != null; }
            set { if (!value)_setf_function = null; }
        }

        LispFunction _macro;

        public LispFunction Macro
        {
            get { return _macro; }
            set { _macro = value; }
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

        public Cons PropertyList
        {
            get;
            set;
        }
        #region StronglyTypedAccessors
        public int ValueAsInteger
        {
            get
            {
                if (!(Value is int))
                {
                    ConditionsDictionary.TypeError("Can't coerce symbol " + _name + " value to integer");
                }

                return (int)Value;
            }
        }

        public Package ValueAsPackage
        {
            get
            {
                var ret = Value as Package;
                if (ret == null)
                {
                    throw new SimpleTypeException("Can't coerce symbol {0} value to Package", _name);
                }

                return ret;
            }
        }

        public bool ValueAsBool
        {
            get
            {
                return (Boolean)Value;
            }
        }

        public Readtable ValueAsReadtable
        {
            get
            {
                var ret = Value as Readtable;
                if (ret == null)
                {
                    throw new SimpleTypeException("Can't coerce symbol {0} value to Readtable", _name);
                }

                return ret;
            }
        }
        #endregion

        public void Makunbound()
        {
            _value = UnboundValue.Unbound;
        }

        public virtual bool IsConstant
        {
            get;
            set;
        }

        public bool IsDynamic
        {
            get;
            set;
        }

        #region Function Invocation shortcut

        #region single return
        public object Invoke()
        {
            return _function.Invoke();
        }

        public object Invoke(object arg1)
        {
            return _function.Invoke(arg1);
        }

        public object Invoke(object arg1, object arg2)
        {
            return _function.Invoke(arg1, arg2);
        }

        public object Invoke(object arg1, object arg2, object arg3)
        {
            return _function.Invoke(arg1, arg2, arg3);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, params object[] args)
        {
            return _function.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, args);
        }

        public object Invoke(object[] args)
        {
            return _function.Invoke(args);
        }
        #endregion

        #region multiple return

        #endregion

        #region void return
        public void VoidInvoke()
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1)
        {
            _function.VoidInvoke(arg1);
        }
        public void VoidInvoke(object arg1, object arg2)
        {
            _function.VoidInvoke(arg1, arg2);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3)
        {
            _function.VoidInvoke(arg1, arg2, arg3);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            _function.VoidInvoke(arg1, arg2, arg3, arg4);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            _function.VoidInvoke(arg1, arg2, arg3, arg4, arg5);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            _function.VoidInvoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            _function.VoidInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            _function.VoidInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            _function.VoidInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            _function.VoidInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            _function.VoidInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            _function.VoidInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            _function.VoidInvoke( arg1,  arg2,  arg3,  arg4,  arg5,  arg6,  arg7,  arg8,  arg9,  arg10, arg11, arg12,arg13);
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, params object[] args)
        {
            _function.VoidInvoke( arg1,  arg2,  arg3,  arg4,  arg5,  arg6,  arg7,  arg8,  arg9,  arg10, arg11, arg12,arg13, args);
        }
        public void VoidInvoke(object[] args)
        {
            _function.VoidInvoke(args);
        }
        #endregion
        #endregion

        public override string ToString()
        {
            if (DefinedSymbols.PrintSymbolWithPackage.Boundp)
            {
                return ToString(DefinedSymbols.PrintSymbolWithPackage._value != DefinedSymbols.NIL);
            }

            return ToString(false);
        }

        public virtual string ToString(bool printpackage)
        {
            if (!printpackage)
                return _name;
            if (_package == null)
                return "#:" + _name;
            string visibility = ":";
            if (_package.IsInternal(this))
                visibility = "::";

            return _package.Name + visibility + _name;
        }
    }

    [DebuggerDisplay("{ToString(true)}")]
    public class KeywordSymbol : Symbol
    {
        public KeywordSymbol(string name, int id)
            :base(name , id)
        {

        }

        public override bool IsConstant
        {
            get
            {
                return true;
            }
            set
            {
                if (!value)
                    throw new InvalidOperationException();
            }
        }

        public override string ToString(bool printpackage)
        {
            if (!printpackage)
                return Name;
            else
                return ":" + Name;
        }

        public override string ToString()
        {
           return  ToString(true);
        }
    }
}