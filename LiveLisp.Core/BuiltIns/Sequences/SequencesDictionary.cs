using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using System.Collections;
using LiveLisp.Core.BuiltIns.Conditions;


namespace LiveLisp.Core.BuiltIns.Sequences
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class SequencesDictionary
    {
        [Builtin("copy-seq")]
        public static object CopySeq(object sequence)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Elt(object sequence, object index)
        {
            throw new NotImplementedException();
        }

        [Builtin("SYSTEM::set-elt", OverridePackage = true)]
        public static object set_Elt(object sequence, object index, object value)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Fill(object sequence, object item, [Key(DefaultValue = "0")]object start, [Key] object end)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-sequence")]
        public static object MakeSequence(object result_type, object size, [Key] object initial_element)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Subseq(object sequence, object start, [Optional] object end)
        {
            throw new NotImplementedException();
        }

        [Builtin("SYSTEM::set-subseq")]
        public static object set_Subseq(object sequence, object start, object new_subsequence)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Map(object result_type, object function, params object[] sequences)
        {
            throw new NotImplementedException();
        }

        [Builtin("map-into")]
        public static object MapInto(object result_sequence, object function, params object[] sequences)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Reduce(object function, object sequence, [Key] object key, [Key] object from_end, [Key] object start, [Key] object end, [Key] object initial_value)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Count(object item, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object key, [Key] object test, [Key] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("count-if")]
        public static object CountIf(object predicate, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object key, [Key] object test, [Key] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("count-if-not")]
        public static object CountIfNot(object predicate, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object key, [Key] object test, [Key] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Length(object sequence)
        {
            ICollection list = sequence as ICollection;
            if (list != null)
                return list.Count;
            String str = sequence as String;
                if(str != null)
                    return str.Length;
            Array ar = sequence as Array;
            if (ar != null)
                return ar.Length;
            IEnumerable enumerable = sequence as IEnumerable;
            if (enumerable != null)
            {
                int num = 0;
                var enumerator = enumerable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    num++;
                }
                return num;

            }
            ConditionsDictionary.TypeError("LENGTH: argument 1 is not a sequence " + sequence);
            throw new Exception("never reached");
        }

        [Builtin]
        public static object Reverse(object sequence)
        {

            if (sequence == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;
            Cons cons = sequence as Cons;

            if (cons != null)
            {
                object r = DefinedSymbols.NIL;
                Cons x = cons;

            Loop:
                if (x == null)
                {
                    return r;
                }
                else
                {
                    r = new Cons(x.Car, r);
                }

                x = x.Child;
                goto Loop;
            }

            IList<object> vector = sequence as IList<object>;

            if (vector != null)
                return  new List<object>(vector.Reverse());

            ConditionsDictionary.TypeError("REVERSE: argument 1 is not a sequence " + sequence);
            throw new InvalidOperationException();
        }

        [Builtin]
        public static object NReverse(object sequence)
        {
            if (sequence == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;
            Cons cons = sequence as Cons;

            if (cons != null)
            {
                object r = DefinedSymbols.NIL;
                Cons x = cons;

            Loop:
                if (x == null)
                {
                    return r;
                }
                else
                {
                    r = new Cons(x.Car, r);
                }

                x = x.Child;
                goto Loop;
            }

            IList<object> vector = sequence as IList<object>;

            if (vector != null)
                return new List<object>(vector.Reverse());

            ConditionsDictionary.TypeError("REVERSE: argument 1 is not a sequence " + sequence);
            throw new InvalidOperationException();
        }

        [Builtin]
        public static object Sort(object sequence, object predicate, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("stable-sort")]
        public static object StableSort(object sequence, object predicate, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Find(object item, object sequence, [Key] object from_end, [Key] object test, [Key] object test_not, [Key] object start, [Key] object end, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("find-if")]
        public static object FindIf(object predicate, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("find-if-not")]
        public static object FindIfNot(object predicate, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Position(object item, object sequence, [Key] object from_end, [Key] object test, [Key] object test_not, [Key] object start, [Key] object end, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("Position-if")]
        public static object PositionIf(object predicate, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("Position-if-not")]
        public static object PositionIfNot(object predicate, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Search(object sequence1, object sequence2, [Key] object from_end, [Key] object test, [Key] object test_not, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Mismatch(object sequence1, object sequence2, [Key] object from_end, [Key] object test, [Key] object test_not, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Replace(object sequence1, object sequence2, [Key] object start1, [Key] object start2, [Key] object end1, [Key] object end2)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Substitute(object newitem, object olditem, object sequence, [Key] object from_end, [Key] object test, [Key] object test_not, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("Substitute-if")]
        public static object SubstituteIf(object newitem, object predicate, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("Substitute-if-not")]
        public static object SubstituteIfNot(object newitem, object predicate, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object NSubstitute(object newitem, object olditem, object sequence, [Key] object from_end, [Key] object test, [Key] object test_not, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("NSubstitute-if")]
        public static object NSubstituteIf(object newitem, object predicate, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("NSubstitute-if-not")]
        public static object NSubstituteIfNot(object newitem, object predicate, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Concatenate(object result_type, params object[] sequences)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Merge(object result_type, object sequence1, object sequence2, object predicate, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Remove(object item, object sequence, [Key] object from_end, [Key] object test, [Key] object test_not, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("Remove-if")]
        public static object RemoveIf(object test, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("Remove-if-not")]
        public static object RemoveIfNot(object test, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Delete(object item, object sequence, [Key] object from_end, [Key] object test, [Key] object test_not, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("Delete-if")]
        public static object DeleteIf(object test, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("Delete-if-not")]
        public static object DeleteIfNot(object test, object sequence, [Key] object from_end, [Key] object start, [Key] object end, [Key] object count, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("remove-duplicates")]
        public static object RemoveDuplicates(object sequence, [Key] object from_end, [Key] object test, [Key] object test_not, [Key] object start, [Key] object end, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("delete-duplicates")]
        public static object DeleteDuplicates(object sequence, [Key] object from_end, [Key] object test, [Key] object test_not, [Key] object start, [Key] object end, [Key] object key)
        {
            throw new NotImplementedException();
        }
    }
}