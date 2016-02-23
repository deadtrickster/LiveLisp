using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.BuiltIns.Numbers;
using LiveLisp.Core.BuiltIns.Sequences;
using System.Collections;
using LiveLisp.Core.CLOS;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.BuiltIns.Arrays
{
    public interface ILispArray 
    {
        
    }

    public interface ISpecializedByDynamicType
    {
        DynamicType DynamicType
        {
            get;
            set;
        }
    }

    public class LispArray : ILispArray, ISpecializedByDynamicType
    {
        Array ar;

        public int FillPointer
        {
            get;
            set;
        }

        public DynamicType DynamicType
        {
            get;
            set;
        }

        internal Array ToCLRArray()
        {
            throw new NotImplementedException();
        }

        public static LispArray FromCLRArray(Array array)
        {
            LispArray ar = new LispArray();
            ar.ar = array;
            ar.DynamicType = DynamicTypeResolver.CreateDynamicType(array.GetType().GetElementType());
            return ar;
        }

        //public static LispArray FromCLRVector(List<object> list)
        //{
        //    
        //}
    }

    public class LispVector : IList<object>, ISpecializedByDynamicType, ILispObject
    {
        DynamicType specializer;

        List<object> list = new List<object>();

        public LispVector(DynamicType specializer)
        {
            this.specializer = specializer;
        }

        void CheckType(object item)
        {
            if (specializer.Is(item))
            {
                ConditionsDictionary.TypeError(item + " is not " + specializer.ToString());
            }
        }

        public int IndexOf(object item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, object item)
        {
            CheckType(item);
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                CheckType(value);
                this[index] = value;
            }
        }

        public void Add(object item)
        {
            CheckType(item);
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(object item)
        {
            return list.Contains(item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(object item)
        {
            return list.Remove(item);
        }

        public IEnumerator<object> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public DynamicType DynamicType
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

        public LispObjectTypeCode LispTypeCode
        {
            get { return LispObjectTypeCode.CLOSInstance; }
        }

        public int FillPointer
        {
            get;
            set;
        }
    }
}
