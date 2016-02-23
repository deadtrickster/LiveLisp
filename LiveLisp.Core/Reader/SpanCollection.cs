using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LiveLisp.Core.Reader
{
    /// <summary>
    /// 
    /// </summary>
    public class SpanCollection
    {
        static Dictionary<string, Mirror<object, SourceSpan>> Store = new Dictionary<string, Mirror<object, SourceSpan>>();

        internal static void Add(string docid, object ret, SourceSpan span)
        {
            if (!Store.ContainsKey(docid))
            {
                Store.Add(docid, new Mirror<object, SourceSpan>());
            }

            Store[docid].Add(ret, span);
        }

        public static SourceSpan getSpan(string docid, object o)
        {
            return Store[docid][o];
        }

        public static object getObjectAt(string docId, int line, int col)
        {
            Mirror<object, SourceSpan> docspans = Store[docId];

            List<SourceSpan> Candidates = new List<SourceSpan>();

            foreach (var item in docspans.secondList)
            {
                if (item.Containts(line, col))
                {
                    Candidates.Add(item);
                }
            }

            return Distil(docId, Candidates);
        }

        public static object getObjectAt(string docId, int i)
        {
            List<SourceSpan> Candidates = new List<SourceSpan>();

            Mirror<object, SourceSpan> docspans = Store[docId];

            foreach (var item in docspans.secondList)
            {
                if (item.Containts(i))
                {
                    Candidates.Add(item);
                }
            }

            return Distil(docId, Candidates);
        }

        /// <summary>
        /// get object exactly under cursor
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="spans"></param>
        /// <returns></returns>
        public static object Distil(string docId, List<SourceSpan> spans)
        {
            object ret = null;
            foreach (var item in spans)
            {
                ret = getObject(docId, item);

                if (ret is CommentPlaceholder)
                {
                    return ret;
                }
            }

            return ret;
        }

        private static object getObject(string docId, SourceSpan item)
        {
            return Store[docId][item];
        }
    }

    public class Pair<T1, T2>
    {
        public T1 First
        {
            get;
            set;
        }

        public T2 Second
        {
            get;
            set;
        }

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }

    public class Mirror<T1, T2> : IEnumerable
    {
        public Mirror()
        {
            firstList = new List<T1>();
            secondList = new List<T2>();
        }

        public Mirror(int capacity)
        {
            firstList = new List<T1>(capacity);
            secondList = new List<T2>(capacity);
        }

        public Mirror(Mirror<T1, T2> copyfrom)
        {
            firstList = new List<T1>(copyfrom.firstList);
            secondList = new List<T2>(copyfrom.secondList);
        }

        List<T1> firstList;
        public List<T2> secondList;

        public void Add(T1 first, T2 second)
        {
            if (!firstList.Contains(first) && !secondList.Contains(second))
            {
                firstList.Add(first);
                secondList.Add(second);
            }
        }

        /*   public Pair<T1, T2> this[int index]
           {
               get
               {
                   if (index > firstList.Count || index > secondList.Count)
                   {
                       throw new Exception("invalid mirror index");
                   }

                   return new Pair<T1, T2> { First = firstList[index], Second = secondList[index] };
               }
           }
           */
        public T2 this[T1 key]
        {
            get
            {
                var index = firstList.IndexOf(key);
                if (index == -1)
                    return default(T2);
                return secondList[index];
            }
        }

        public T1 this[T2 key]
        {
            get
            {
                var index = secondList.IndexOf(key);
                if (index == -1)
                    return default(T1);
                return firstList[index];
            }
        }


        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new MirrorEnumerator<KeyValuePair<T1, T2>>(this);
        }

        #endregion


        public class MirrorEnumerator<T> : IEnumerator<KeyValuePair<T1, T2>>
        {
            private Mirror<T1, T2> mirror;

            int index = -1;

            public MirrorEnumerator(Mirror<T1, T2> mirror)
            {
                this.mirror = mirror;
            }

            #region IEnumerator<T> Members

            public KeyValuePair<T1, T2> Current
            {
                get { return new KeyValuePair<T1, T2>(mirror.firstList[index], mirror.secondList[index]); }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                mirror = null;
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get
                {
                    return new KeyValuePair<T1, T2>(mirror.firstList[index], mirror.secondList[index]);
                }
            }

            public bool MoveNext()
            {
                return index++ < mirror.firstList.Count ? true : false;
            }

            public void Reset()
            {
                index = -1;
            }

            #endregion
        }

        internal bool FirstExists(T1 first)
        {
            return firstList.Contains(first);
        }

        internal bool SecondExists(T2 second)
        {
            return secondList.Contains(second);
        }
    }
}
