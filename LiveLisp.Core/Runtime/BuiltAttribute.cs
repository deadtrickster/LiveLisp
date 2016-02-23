using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.Runtime
{
    public enum ValuesReturnPolitics
    {
        Never, // функция никогда не возвращает несколько значений, если speed 3 & safe 0 то не делается никаких проверок
        //		иначе, если функция всёже вернула несколько значений вылезает исключение
        Sometimes,

        Always,

        AlwaysNonZero,

        Void
    }

    [global::System.AttributeUsage(AttributeTargets.Class)]
    sealed class BuiltinsContainerAttribute : Attribute
    {
        readonly string _package;

        // This is a positional argument
        public BuiltinsContainerAttribute(string package)
        {
            this._package = package;
        }

        public string Package
        {
            get { return _package; }
        }
    }

    [global::System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class BuiltinAttribute : Attribute
    {

        public BuiltinAttribute(string name, ValuesReturnPolitics valuesReturnKind, bool canReturnZeroValues)
        {
            Name = name;
            ValuesReturnPolitics = valuesReturnKind;
            CanReturnZeroValues = canReturnZeroValues;
        }

        public BuiltinAttribute(string name)
        {
            Name = name;
            ValuesReturnPolitics = ValuesReturnPolitics.Never;
            CanReturnZeroValues = true;
        }
        public BuiltinAttribute()
        {
            ValuesReturnPolitics = ValuesReturnPolitics.Never;
        }

        public string Name
        {
            get;
            set;
        }

        public ValuesReturnPolitics ValuesReturnPolitics
        {
            get;
            set;
        }

        public bool CanReturnZeroValues
        {
            get;
            set;
        }

        public bool OverridePackage
        {
            get;
            set;
        }
        bool _predicate;
        /// <summary>
        /// Предикаты имеют тип возвращаемого значения - object значит единственный способ перейтив ветку false - вернуть clrnull
        /// </summary>
        public bool Predicate
        {
            get { return _predicate; }
            set { _predicate = value; 
                ValuesReturnPolitics = ValuesReturnPolitics.Never; }
        }

        public bool AllowOtherKeys
        {
            get;
            set;
        }

        // TODO: check for predicate. default - nil, _predicate = true;
        public object VoidReturn
        {
            get;
            set;
        }


        public int MinArgs
        {
            get;
            set;
        }

        public int MaxArgs
        {
            get;
            set;
        }
    }
}
