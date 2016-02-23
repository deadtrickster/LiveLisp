using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using LiveLisp.Core.CLOS;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.BuiltIns.Hashtables
{
    public interface LispEqualityComparer : IEqualityComparer<object>, IEqualityComparer {

        Symbol Name
        {
            get;
        }
    }

    public class EqEqualityComparer : LispEqualityComparer
    {

        #region IEqualityComparer<object> Members

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        #endregion

        #region IEqualityComparer<object> Members

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return DefinedSymbols.Eq.Invoke(x, y) == DefinedSymbols.NIL ? false : true;
        }

        #endregion

        #region IEqualityComparer<object> Members


        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEqualityComparer Members

        bool IEqualityComparer.Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region LispEqualityComparer Members

        public Symbol Name
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class EqlEqualityComparer : LispEqualityComparer
    {
        #region IEqualityComparer<object> Members


        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        #endregion
        #region IEqualityComparer<object> Members

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return DefinedSymbols.Eql.Invoke(x, y) == DefinedSymbols.NIL ? false : true;
        }

        #endregion

        #region IEqualityComparer<object> Members


        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEqualityComparer Members

        bool IEqualityComparer.Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region LispEqualityComparer Members

        public Symbol Name
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class EqualEqualityComparer : LispEqualityComparer
    {
        #region IEqualityComparer<object> Members


        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEqualityComparer<object> Members

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEqualityComparer Members

        bool IEqualityComparer.Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region LispEqualityComparer Members

        public Symbol Name
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class EqualpEqualityComparer : LispEqualityComparer
    {
        #region IEqualityComparer<object> Members


        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEqualityComparer<object> Members

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEqualityComparer Members

        bool IEqualityComparer.Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region LispEqualityComparer Members

        public Symbol Name
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }



    /// <summary>
    /// http://www.koders.com/csharp/fidF2C8E1786AE76101911914CCA9286E25B398AED6.aspx
    /// Isolate this the iteration from the collection. Allows you to 
    /// modify the underlying collection while in the middle of a foreach
    /// </summary>
    public class IterIsolate : IEnumerable
    {
        internal class IterIsolateEnumerator : IEnumerator
        {
            protected List<object> items;
            protected int currentItem;

            internal IterIsolateEnumerator(IEnumerator enumerator)
            {
                // if this is the enumerator from another iterator, we
                // don't have to enumerate it; we'll just steal the arraylist
                // to use for ourselves. 
                IterIsolateEnumerator chainedEnumerator =
                    enumerator as IterIsolateEnumerator;

                if (chainedEnumerator != null)
                {
                    items = chainedEnumerator.items;
                }
                else
                {
                    items = new List<object>();
                    while (enumerator.MoveNext() != false)
                    {
                        items.Add(enumerator.Current);
                    }
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                currentItem = -1;
            }

            public void Reset()
            {
                currentItem = -1;
            }

            public bool MoveNext()
            {
                currentItem++;
                if (currentItem == items.Count)
                    return false;

                return true;
            }

            public object Current
            {
                get
                {
                    return items[currentItem];
                }
            }
        }

        /// <summary>
        /// Create an instance of the IterIsolate Class
        /// </summary>
        /// <param name="enumerable">A class that implements IEnumerable</param>
        public IterIsolate(IEnumerable enumerable)
        {
            this.enumerable = enumerable;
        }

        public IEnumerator GetEnumerator()
        {
            return new IterIsolateEnumerator(enumerable.GetEnumerator());
        }

        protected IEnumerable enumerable;
    }

}
