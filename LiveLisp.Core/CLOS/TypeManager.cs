using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace LiveLisp.Core.CLOS
{
    /// <summary>
    /// How it works: 
    /// 
    /// when this attribute comes to play runtime will add this class to typeTable with specified name
    /// 
    /// After CLOS class created it is necessary to fill it superclass table, so base types of the currentType
    /// enumerated and if they marked with this attribute process recursivly repeated else (when one of base class or interface in inheritance
    /// tree not marked) searching stops.
    /// 
    /// if type inherit from CLOSInstance this attribute will be ignored
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    sealed class CLOSBuiltinClassAttribute : Attribute
    {
        public CLOSBuiltinClassAttribute()
        {
        }

        /// <summary>
        /// Full symbol name, so if package not defined symbol will be interned in current package
        /// </summary>
        public string ClassName
        {
            get;
            set;
        }
    }

    [global::System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    sealed class CLOSSlotNameAttribute : Attribute
    {
        string name;

        public CLOSSlotNameAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }

    /// <summary>
    /// спецификаторами типа могут быть как clos классы так и .net типы
    /// 
    /// задача этого кода - обеспечить интероперабельность
    /// </summary>
    public class TypeManager
    {
        public static void InitializeBuiltinClasses()
        {
            InitializeConditions();
        }

        private static void InitializeConditions()
        {
            // создать классы и добавить в таблицу
        }
    }

    /// <summary>
    /// так как какждый CLOSClass имеет имя которое символ который иммет идентификатор
    /// мы можем сделать этот идентификтор идентификатором класса
    /// </summary>
    public class CLOSTypeTable : KeyedCollection<int, CLOSClass>
    {
        private CLOSTypeTable()
        {

        }

        protected override int GetKeyForItem(CLOSClass item)
        {
            return item.Id;
        }

        static CLOSTypeTable instance;

        public static CLOSTypeTable Instance
        {
            get { return CLOSTypeTable.instance; }
        }

        static CLOSTypeTable()
        {
            instance = new CLOSTypeTable();
        }
    }

    public enum DynamicTypeKind
    {
        CLR,
        CLOS
    }

    public abstract class DynamicType
    {
        DynamicTypeKind kind;

        public DynamicTypeKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        public DynamicType(DynamicTypeKind kind)
        {
            this.kind = kind;
        }

        public abstract bool Is(object obj);

        public abstract Type CLREquivalent
        {
            get;
        }
    }

    public class CLOSDynamicType : DynamicType
    {
        CLOSClass cclass;

        public CLOSClass CClass
        {
            get { return cclass; }
            set { cclass = value; }
        }

        public CLOSDynamicType(CLOSClass cclass)
            : base(DynamicTypeKind.CLOS)
        {
            this.cclass = cclass;
        }

        public override bool Is(object obj)
        {
            throw new NotImplementedException();
        }

        public override Type CLREquivalent
        {
            get { throw new NotImplementedException(); }
        } 
    }

    public class CLRDynamicType : DynamicType
    {
        Type type;

        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        public CLRDynamicType(Type type)
            :base(DynamicTypeKind.CLR)
        {
            this.type = type;
        }

        public override bool Is(object obj)
        {
            return type.IsInstanceOfType(obj);
        }

        public override Type CLREquivalent
        {
            get { return type; }
        }
    }

    /// <summary>
    /// в отличие от StaticTypeResolver который "видит" только .net тип объекта 
    /// это резолвер может работать и с CLOS классами 
    /// 
    /// 
    /// при поиске типа по названию и в случае если параметры явно не указывают 
    /// но .net тип (например в случае обощений) вначале поиск производится 
    /// по таблице CLOS классов
    /// </summary>
    public class DynamicTypeResolver
    {
        public virtual DynamicType DynamicType
        {
            get { throw new NotImplementedException(); }
        }

        public static DynamicType CreateDynamicType(object type_spec)
        {
            if (type_spec is Type)
            {
                return new CLRDynamicType(type_spec as Type);
            }

            throw new NotImplementedException();
        }
    }
}
