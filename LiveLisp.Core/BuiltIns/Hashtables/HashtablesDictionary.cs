using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using System.Collections;
using LiveLisp.Core.BuiltIns.Numbers;
using System.Reflection;

namespace LiveLisp.Core.BuiltIns.Hashtables
{
    /*
     * хэштаблица в лиспе это ИЛИ System.Collections.Hashtable ИЛИ System.Collections.Generic.Dictionary<object, object>
     * make-hash-table создаёт только System.Collections.Generic.Dictionary<object, object>
     * игнорируя следующие аргументы rehash_size & rehash_thresold. Нужно ли проверять правильность их типов?
     */
    [BuiltinsContainer("COMMON-LISP")]
    public static class HashtablesDictionary
    {
        [Builtin("make-hash-table")]
        public static object MakeHashTable([Key] object test, [Key] object size, [Key] object rehash_size, [Key] object rehash_threshold)
        {
            throw new NotImplementedException();
        }

        [Builtin("hash-table-p")]
        public static object HashTableP(object obj)
        {
            if (obj is Dictionary<object, object>)
                return true;

            if (obj is Hashtable)
                return true;

            return false;
        }

        [Builtin("hash-table-count")]
        public static object HashTableCount(object hash_table)
        {
            Dictionary<object, object> hash1 = hash_table as Dictionary<object, object>;
            if (hash1 != null)
                return hash1.Count;

            Hashtable hash2 = hash_table as Hashtable;
            if (hash2 != null)
                return hash2.Count;

            throw new TypeErrorException("HASH-TABLE-COUNT: hash-table is not a Hashtable");
        }

        [Builtin("hash-table-rehash-size")]
        public static object HashTableRehashSize(object hash_table)
        {
            Dictionary<object, object> hash1 = hash_table as Dictionary<object, object>;
            if (hash1 != null)
                return 1;

            Hashtable hash2 = hash_table as Hashtable;
            if (hash2 != null)
                return 1;

            throw new TypeErrorException("HASH-TABLE-REHASH-SIZE: hash-table is not a Hashtable"); 
        }

        [Builtin("hash-table-rehash-threshold")]
        public static object HashTableRehashThreshold(object hash_table)
        {
            Dictionary<object, object> hash1 = hash_table as Dictionary<object, object>;
            if (hash1 != null)
                return 1;

            Hashtable hash2 = hash_table as Hashtable;
            if (hash2 != null)
                return 1;

            throw new TypeErrorException("HASH-TABLE-REHASH-THRESHOLD: hash-table is not a Hashtable"); 
        }

        [Builtin("hash-table-size")]
        public static object HashTableSize(object hash_table)
        {
            Dictionary<object, object> hash1 = hash_table as Dictionary<object, object>;
            if (hash1 != null)
                return hash1.Count; 

            Hashtable hash2 = hash_table as Hashtable;
            if (hash2 != null)
                return hash2.Count;

            throw new TypeErrorException("HASH-TABLE-SIZE: hash-table is not a Hashtable"); 
        }

        [Builtin("hash-table-test")]
        public static object HashTableTest(object hash_table)
        {
            Dictionary<object, object> hash1 = hash_table as Dictionary<object, object>;
            if (hash1 != null)
            {
                LispEqualityComparer lec = hash1.Comparer as LispEqualityComparer;
                if (lec != null)
                    return lec.Name;
                else
                    return ClrMethodImporter.Import(hash1.Comparer.GetType().GetMethod("GetHashCode"));
            }

            Hashtable hash2 = hash_table as Hashtable;
            if (hash2 != null)
            {
                object comparer = HashtableEqualityComparer.GetValue(hash2);
                LispEqualityComparer lec = comparer as LispEqualityComparer;

                if (lec != null)
                    return lec.Name;
                else
                    return ClrMethodImporter.Import(comparer.GetType().GetMethod("GetHashCode"));
            }

            throw new TypeErrorException("HASH-TABLE-TESt: hash-table is not a Hashtable");
        }

        [Builtin(ValuesReturnPolitics=ValuesReturnPolitics.AlwaysNonZero)]
        public static object Gethash(object key, object hash_table, [Optional] object def)
        {
            throw new NotImplementedException();
        }

        [Builtin("SYSTEM::set-gethash", OverridePackage=true)]
        public static object set_Gethash(object key, object hash_table, object hash)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Remhash(object key, object hash_table)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Maphash(object function, object hash_table)
        {
            /*
            foreach (var key in new IterIsolate(Keys))
            {
                method.VoidInvoke(key, this[key]);
            }
             */

            throw new NotImplementedException();
        }

        [Builtin]
        public static object Clrhash(object hash_table)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Sxhash(object hash_table)
        {
            throw new NotImplementedException();
        }

        public static FieldInfo HashtableEqualityComparer = typeof(Hashtable).GetField("_keycomparer", BindingFlags.NonPublic | BindingFlags.Instance);
    }
}
