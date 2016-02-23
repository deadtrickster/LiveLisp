using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;

namespace LiveLisp.Core.Compiler
{
    public class MultipleValuesContainer : List<object>
    {
        public MultipleValuesContainer()
        {

        }

        public MultipleValuesContainer(object arg1)
        {
            Add(arg1);
        }

        public MultipleValuesContainer(object arg1, int capacity)
            : base(capacity)
        {
            Add(arg1);
        }

        public MultipleValuesContainer(object[] values)
            : base(values)
        {

        }

        public object First
        {
            get
            {
                if (Count != 0)
                    return this[0];
                else return DefinedSymbols.NIL;
            }
        }

        public object GetValueForBinding(int index)
        {
            if (index >= Count)
                return DefinedSymbols.NIL;
            else
                return this[index];
        }

        public void Combine(object obj)
        {
            MultipleValuesContainer values = obj as MultipleValuesContainer;
            if (obj != null)
                AddRange(values);
            else
                Add(obj);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(MultipleValuesContainer values)
        {
            if (this.Count != values.Count)
                return false;

            for (int i = 0; i < Count; i++)
            {
                if (!this[i].Equals(values[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static MultipleValuesContainer Void = new MultipleValuesContainer();

        public static ConstructorInfo Values0Ctor = typeof(MultipleValuesContainer).GetConstructor(Type.EmptyTypes);
        public static ConstructorInfo Values1Ctor = typeof(MultipleValuesContainer).GetConstructor(LambdaCallEmitterHelper.argsLists[1]);

        public static MethodInfo CombineMethod = typeof(MultipleValuesContainer).GetMethod("Combine");
        public static MethodInfo ToArrayMethod = typeof(MultipleValuesContainer).GetMethod("ToArray");
    }
}
