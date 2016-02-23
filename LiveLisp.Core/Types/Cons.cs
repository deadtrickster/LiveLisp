using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace LiveLisp.Core.Types
{
    // TODO: подумать о введении флага противоречивого состояния - когда SetCdrDelayed использовано
    // но пока не вызвана рекалькуляция.
    public class Cons : IEnumerable, IList
    {
        protected object car;

        public virtual object Car
        {
            get { return car; }
            set { car = value; }
        }

        protected object cdr;

        public virtual object Cdr
        {
            get { return cdr; }
            set
            {
                cdr = value;

                child = value as Cons;
                if (child != null)
                {
                    child.parent = this;
                    IsProperList = child._properlist;
                    _last = child._last;
                }
                else if (value == DefinedSymbols.NIL)
                {
                    IsProperList = true;
                    _last = this;
                }
                else
                {
                    IsProperList = false;
                    _last = this;
                }

                if (_properlist)
                {
                    // recalculate self length
                    CalculateLength();
                    //send length to parent with self so parent stop recalculating after reach us.
                    if (parent != null)
                    {
                        parent.RecalculateCount(_count);
                    }
                }
            }
        }

        Cons _last;
        // TODO: implement caching
        public Cons Last
        {
            get
            {
                if (child == null)
                    return this;
                else
                    return child.Last;
            }
            set {
                if (child == null)
                    this.cdr = value;
                else
                    child.Last = value;
            }
        }
        public void SetCdrDelayed(object value)
        {
            cdr = value;

            child = value as Cons;
            if (child != null)
            {
                child.parent = this;
                _properlist = true;
            }

        }

        private void CalculateLength()
        {
            _count = 0;

            if (!_properlist)
                return; // no throw here because we will throw in Count getter

            _count = 0;

            //  if (car == DefinedSymbols.NIL && cdr == DefinedSymbols.NIL)
            //  {
            //      return;
            //  }

            Cons current = this;

        Next:
            _count++;

            if (current.child == null)
            {
                return;
            }
            else
            {
                current = current.child;
                goto Next;
            }
        }

        public void CalculateDelayedLengthNoThrow()
        {
            if (!_properlist)
                return;

            Cons current = this;

        Next:
            if (current.child == null)
            {
                current.RecalculateCount(0);
            }
            else
            {
                current = current.child;
                goto Next;
            }
        }

        public void CalculateDelayedLength()
        {
            if (!_properlist)
                throw new SimpleErrorException("list {0} is not a proper list, can't calculate length", this);

            Cons current = this;

        Next:
            if (current.child == null)
            {
                current.RecalculateCount(0);
            }
            else
            {
                current = current.child;
                goto Next;
            }
        }

        private void RecalculateCount(int count)
        {
            Cons _parent = this;
        Next:
            if (_parent != null)
            {
                _parent._count = ++count;
                _parent = _parent.parent;
                goto Next;
            }

        }

        // speedup
        Cons parent;

        public Cons Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        Cons child;

        public Cons Child
        {
            get { return child; }
            set
            {
                child = value;
                cdr = value;
                child.parent = this;
            }
        }

        public object First
        {
            get
            {
                if (_count > 0)
                {
                    return car;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("First");
                }
            }
            set
            {
                car = value;
            }
        }

        public object Second
        {
            get
            {
                if (_count > 1)
                {
                    return child.car;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Second");
                }
            }
            set
            {
                if (_count > 1)
                {
                    child.child.car = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Second");
                }
            }
        }

        protected Cons()
        {
        }

        public Cons(object car, object cdr)
        {
            this.car = car;
            this.Cdr = cdr;
        }

        public Cons(object car)
            : this(car, DefinedSymbols.NIL)
        {
        }

        public static Cons FromCollection(IEnumerable collection)
        {
            /* Cons current = this;
             foreach (var item in collection)
             {
                 current._properlist = true;
                 current.car = item;
                 current.Child = new Cons();
                 current = current.child;
             }

             current.cdr = DefinedSymbols.NIL;
             current.parent.RecalculateCount(-1);*/
            var this_ = new Cons();
            Cons list = this_;
            foreach (object o in collection)
            {
                list.car = o;
                Cons next = new Cons();
                list.SetCdrDelayed(next);
                list = next;
            }
            list.parent.SetCdrDelayed(DefinedSymbols.NIL);
            list.parent.RecalculateCount(0);
            return this_;
        }
        protected bool _properlist;
        public bool IsProperList
        {
            get { return _properlist; }
            set
            {
                if (parent != null)
                {
                    parent._properlist = value;
                }
                _properlist = value;
            }
        }

        public static implicit operator Cons(Array collection)
        {
            return new Cons(collection);
        }

        #region ToString implementation
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            ToString(buffer);

            return buffer.ToString();
        }

        public void ToString(StringBuilder buffer)
        {
            //if (car == DefinedSymbols.NIL && cdr == DefinedSymbols.NIL)
            //{
            //    buffer.Append("()"); return;
            //}

            buffer.Append("(");

            ToStringWithoutBraces(buffer);

            buffer.Append(")");
        }

        private void ToStringWithoutBraces(StringBuilder buffer)
        {
            if (car == null)
            {
                buffer.Append("clrnull");
            }
            else if (car is Cons) // speedup
            {
                ((Cons)car).ToString(buffer);
            }
            else
            {
                buffer.Append(car.ToString());
            }

            if (child == null)
            {
                if (cdr == DefinedSymbols.NIL)
                {
                    return;
                }


                buffer.Append(" . ");
                if (cdr == null)
                {
                    buffer.Append("clrnull");
                }
                else
                {
                    buffer.Append(cdr.ToString());
                }
            }
            else
            {
                buffer.Append(" ");
                child.ToStringWithoutBraces(buffer);
            }
        }

        #endregion //ToString

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            if (!_properlist)
            {
                throw new SimpleErrorException("can't enumerate through non proper list: {0}", this);
            }
            return new DefaultConsEnumerator(this);
        }

        #endregion

        #region IList Members


        /// <summary>
        /// ?(setq list1 '(1 2 3))
        ///(1 2 3)
        ///?(setq list2 '(3 4 5))
        ///(3 4 5)
        ///?(nconc list1 list2)
        ///(1 2 3 3 4 5)
        ///?(nconc list2 '(6 7))
        ///(3 4 5 6 7)
        ///?list1
        ///(1 2 3 3 4 5 6 7)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Add(object value)
        {
            if (!_properlist)
            {
                throw new SimpleErrorException("can't add new item to non proper list: {0}, use setf for this", this);
            }
            if (_count == 0)
            {
                car = value;
                _count = 1;
                return 0;
            }
            Cons current = this;

            while (current.child != null)
            {
                current = current.child;
            }

            current.Cdr = new Cons(value);

            return this._count - 1;
        }

        public void Append(object value)
        {
            if (!_properlist)
            {
                throw new SimpleErrorException("can't add new item to non proper list: {0}, use setf for this", this);
            }
            if (_count == 0)
            {
                car = value;
                _count = 1;
                return;
            }
            Cons current = this;

            while (current.child != null)
            {
                current = current.child;
            }

            current.Cdr = new Cons(value);
        }


        public void Clear()
        {
            car = DefinedSymbols.NIL;

            if (child != null)
            {
                child.parent = null;
            }

            Cdr = DefinedSymbols.NIL;
            parent = null;
        }

        public bool Contains(object value)
        {
            if (!_properlist)
                throw new SimpleErrorException("Contains work only with proper lists");

            Cons current = this;
        NextChild:
            if (current.car == value)
            {
                return true;
            }
            else
            {
                if (current.child == null)
                    return false;
                current = current.child;
                goto NextChild;
            }
        }

        public int IndexOf(object value)
        {
            if (!_properlist)
                throw new SimpleErrorException("IndexOf work only with proper lists");

            if (this.car == value)
                return 0;

            Cons current = this;
            int counter = 0;

            while (current.child != null)
            {
                current = current.child;
                counter++;
                if (current.car == value)
                    return counter;
            }

            return -1;


            /*
            int counter = 0;
            Next:
            if (current.car == value)
            {
                return counter;
            }
            else
            {
                counter++;
                if (counter != _count)
                {
                    current = current.child;
                    goto Next;
                }
            }

            return -1;*/
        }

        public void Insert(int index, object value)
        {
            if (!_properlist)
                throw new SimpleErrorException("Insert work only with proper lists");

            if (index < 0 || index > _count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (index == 0)
            {
                this.Cdr = new Cons(this.car, this.cdr);
                this.car = value;
                return;
            }

            Cons current = this;

            if (index == _count)
            {
                while (current.child != null)
                {
                    current = current.child;
                }

                current.Cdr = new Cons(value);

                return;
            }

            int counter = 1;
        NextChild:
            if (index == counter)
            {
                current.Cdr = new Cons(value, current.cdr);
                return;
            }
            else
            {
                current = current.child;
                counter++;
                goto NextChild;
            }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            if (!_properlist)
                throw new SimpleErrorException("RemoveAt work only with proper lists");

            Cons current = this;
            if (current.car == value)
            {
                if (current.child != null)
                {
                    current.car = current.child.car;
                    current.Cdr = child.cdr;
                }
                else
                {
                    current.car = DefinedSymbols.NIL;
                }

                return;
            }

        Next:
            current = current.child;

            if (current == null)
            {
                return;
            }

            if (current.car == value)
            {

                if (current.child != null)
                {
                    current.parent.Cdr = current.child;
                    current.child = null;
                    current.parent = null;
                }
                else
                {
                    current.parent.Cdr = DefinedSymbols.NIL;
                    current.parent = null;
                }
            }
            else
            {
                goto Next;
            }

        }

        public void RemoveAt(int index)
        {
            if (!_properlist)
                throw new SimpleErrorException("RemoveAt work only with proper lists");

            if (index < 0 || index > (_count - 1))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            Cons current = this;
            if (index == 0)
            {
                if (current.child != null)
                {
                    current.car = current.child.car;

                    current.Cdr = child.cdr;
                }
                else
                {
                    current.car = DefinedSymbols.NIL;
                }

                return;
            }


            int currentindex = 0;

        Next:
            if (currentindex == index)
            {

                if (current.child != null)
                {
                    current.parent.Cdr = current.child;
                    current.child = null;
                    current.parent = null;
                }
                else
                {
                    current.parent.Cdr = DefinedSymbols.NIL;
                    current.parent = null;
                }
            }
            else
            {
                currentindex++;
                current = current.child;
                goto Next;
            }

        }

        public object this[int index]
        {
            get
            {
                if (!_properlist)
                    throw new SimpleErrorException("Index work only with proper lists");
                if (index == -1)
                    throw
                        new ArgumentOutOfRangeException("index");
                if (index == 0)
                {
                    return car;
                }
                int counter = 0;
                Cons next = this;
            NextChild:
                next = next.child;
                counter++;
                if (next == null)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                if (index == counter)
                {

                    return next.car;
                }
                else
                    goto NextChild;
            }
            set
            {
                if (!_properlist)
                    throw new SimpleErrorException("Index work only with proper lists");
                if (index == -1)
                    throw
                        new ArgumentOutOfRangeException("index");
                if (index == 0)
                {
                    car = value;
                    return;
                }

                int counter = 0;
                Cons next = this;
            NextChild:
                next = next.child;
                counter++;
                if (next == null)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                if (index == counter)
                {
                    next.car = value;
                    return;
                }
                else
                    goto NextChild;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            int currentindex = index;
            foreach (var item in this)
            {
                array.SetValue(item, currentindex);
                currentindex++;
            }
        }

        int _count;
        public virtual int Count
        {
            get
            {
                if (!_properlist)
                    throw new SimpleErrorException("list {0} is not a proper list. can't calculate length.", this);
                return _count;
            }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        object _syncRoot;

        public object SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
                }
                return this._syncRoot;
            }
        }

        #endregion

        public object this[object key]
        {
            get
            {
                if (!_properlist)
                    throw new SimpleErrorException("Index work only with proper lists");
                var count = Count;
                if ((count % 2) != 0)
                {
                    throw new SimpleErrorException("getf must be applyed to property list not to simple list ({0} - odd length).", this);
                }

                if (count == 0)
                {
                    return UnboundValue.Unbound;
                }

                Cons current = this;

            Next:

                if (current.car == key)
                {
                    return current.child.car;
                }
                else
                {
                    if (current.child == null)
                    {
                        return UnboundValue.Unbound;
                    }

                    current = current.child;
                    goto Next;
                }
            }
            set
            {
                if (!_properlist)
                    throw new SimpleErrorException("Index work only with proper lists");
                if ((Count % 2) != 0)
                {
                    throw new SimpleErrorException("getf must be applyed to property list not to simple list ({0} - odd length).", this);
                }
                Cons current = this;

            Next:

                if (current.car == key)
                {
                    current.child.car = value;
                    return;
                }
                else
                {
                    if (current.child != null)
                    {
                        current = current.child;
                        goto Next;
                    }
                }

                throw new KeyNotFoundException(key.ToString());
            }
        }

        /// <summary>
        /// Achtung!!! 
        /// if key already present value just replaced
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddKeyValue(object key, object value)
        {
            if (!_properlist)
                throw new SimpleErrorException("Index work only with proper lists");
            if ((Count % 2) != 0)
            {
                throw new SimpleErrorException("getf must be applyed to property list not to simple list ({0} - odd length).", this);
            }
            Cons current = this;

        Next:

            if (current.car == key)
            {
                current.child.car = value;
                return;
            }
            else
            {
                if (current.child != null)
                {
                    current = current.child;
                    goto Next;
                }
            }

            current.SetCdrDelayed(new Cons(key, new Cons(value)));
            current.CalculateDelayedLength();
        }

        public class DefaultConsEnumerator : IEnumerator
        {
            Cons list;
            Cons _oldlist;

            public DefaultConsEnumerator(Cons list)
            {
                reset(list);
            }

            private void reset(Cons list)
            {
                this.list = new Cons(null, list);
                this._oldlist = new Cons(null, list);
            }

            #region IEnumerator Members

            public object Current
            {
                get { return list.car; }
            }

            public bool MoveNext()
            {
                if (list == null)
                    return false;
                list = list.child;
                return list != null && list.Count != 0;
            }

            public void Reset()
            {
                reset(_oldlist.Child);
            }

            #endregion
        }

        internal List<T1> ToList<T1>() where T1 : class
        {
            List<T1> ret = new List<T1>();
            foreach (var item in this)
            {
                ret.Add(item as T1);
            }

            return ret;
        }

        public virtual Cons MakeCopy()
        {
            Cons cons = this;
            Cons copy = new Cons(null);
            var current = copy;
        Next:

            current.Cdr = new Cons(cons.Car);
            current = current.Child;

            if (cons.Child != null)
            {
                cons = cons.Child;
                goto Next;
            }
            else
            {
                current.Cdr = cons.Cdr;
            }

            return copy.Child;
        }

        internal bool RemoveKey(object indicator)
        {
            if (!_properlist)
                throw new SimpleErrorException("Index work only with proper lists");
            var count = Count;
            if ((count % 2) != 0)
            {
                throw new SimpleErrorException("getf must be applyed to property list not to simple list ({0} - odd length).", this);
            }

            if (count == 0)
            {
                return false;
            }

            Cons current = this;

        Next:

            if (current.car == indicator)
            {
                current.parent.Cdr = current.child.child;
                current.parent = null;
                current.child = null;
                return true;
            }
            else
            {
                if (current.child == null)
                {
                    return false;
                }

                current = current.child.child;
                goto Next;
            }
        }
    }
}
