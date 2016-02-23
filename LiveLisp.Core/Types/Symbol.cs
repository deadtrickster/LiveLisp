using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Reader;
using System.ComponentModel;

namespace LiveLisp.Core.Types
{
    /*public class Symbol: INotifyPropertyChanged
    {
        protected string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        int _Id;

        public int Id
        {
            get { return _Id; }
            internal set { _Id = value; }
        }

        internal Symbol(string name, int id)
        {
            _name = name;
            _Id = id;
            _function = new UnboundFunction(this);
        }

        internal Symbol(string name, int id, Package homePackage)
            : this(name, id)
        {
            _package = homePackage;
        }

        Package _package;

        public Package Package
        {
            get { return _package; }
            set { _package = value; }
        }

        object _value = UnboundValue.Unbound;

        public virtual object Value
        {
            get
            {
                if (!Boundp)
                {
                    throw new UnboundVariableException("Unbound variable {0}.", _name);
                }
                return _value;
            }
            set
            {
                if (IsConstant)
                {
                    new SimpleErrorException("The symbol {0} has been declared constant, and may not be assigned to {1}.", _name, value);
                }
                _value = value;
            }
        }

        LambdaMethod _function;

        public LambdaMethod Function
        {
            get { return _function; }
            set { _function = value; }
        }

        MacroFunction _macro;

        public MacroFunction Macro
        {
            get { return _macro; }
            set { _macro = value; }
        }

        public virtual bool Boundp
        {
            get
            {
                return _value != UnboundValue.Unbound;
            }
        }

        #region StronglyTypedAccessors
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

        public bool IsConstant
        {
            get;
            set;
        }

        public bool IsDynamic
        {
            get;
            set;
        }

        public override string ToString()
        {
            if (DefinedSymbols.PrintSymbolWithPackage.Boundp)
            {
                return ToString(DefinedSymbols.PrintSymbolWithPackage._value != DefinedSymbols.NIL);
            }

            return ToString(false);
        }

        internal string ToString(bool printpackage)
        {
            if(!printpackage)
                return _name;
            if (_package == null)
                return "#:" + _name;
            string visibility = ":";
            if (_package.IsInternal(this))
                visibility = "::";
            
            return _package.Name + visibility + _name;
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
        public object ValuesInvoke()
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, params object[] args)
        {
            return _function.ValuesInvoke();
        }
        public object ValuesInvoke(object[] args)
        {
            return _function.ValuesInvoke();
        }
        #endregion

        #region void return
        public void VoidInvoke()
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, params object[] args)
        {
            _function.VoidInvoke();
        }
        public void VoidInvoke(object[] args)
        {
            _function.VoidInvoke();
        }
        #endregion
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    public class KeywordSymbol : Symbol
    {
        public KeywordSymbol(string name)
            : base(name, -666)
        {
        }

        public override object Value
        {
            get
            {
                return this;
            }
            set
            {
                throw new ConstantException("The symbol " + _name + " has been declared constant, and may not be assigned to");
            }
        }

        public override bool Boundp
        {
            get
            {
                return true;
            }
        }

        public override string ToString()
        {
            return ":" + _name;
        }
    }*/
}
