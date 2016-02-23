using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.BuiltIns.Conditions;
using LiveLisp.Core.BuiltIns.Numbers;
using LiveLisp.Core.BuiltIns.Sequences;

namespace LiveLisp.Core.BuiltIns.Conses
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class ConsesDictionary
    {
        [Builtin]
        public static Cons Cons(object car, object cdr)
        {
            return new Cons(car, cdr);
        }

        [Builtin(Predicate = true)]
        public static object Consp(object obj)
        {
           return obj is Cons ? DefinedSymbols.T : DefinedSymbols.NIL;
        }

        [Builtin(Predicate = true)]
        public static object Atomp(object obj)
        {
            Cons cons = obj as Cons;
            if (cons == null)
                return obj;

            return null;
        }

        [Builtin]
        public static Cons Rplaca(object cons, object new_car)
        {
            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "RPLACA", "cons");

            _cons.Car = new_car;

            return _cons;
        }

        [Builtin]
        public static Cons Rplacd(object cons, object new_cdr)
        {
            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "RPLACD", "cons");

            _cons.Cdr = new_cdr;

            return _cons;
        }

        #region CAR, CDR, CAAR, CADR, CDAR, CDDR, CAAAR, CAADR, CADAR, CADDR, CDAAR, CDADR, CDDAR, CDDDR, CAAAAR, CAAADR, CAADAR, CAADDR, CADAAR, CADADR, CADDAR, CADDDR, CDAAAR, CDAADR, CDADAR, CDADDR, CDDAAR, CDDADR, CDDDAR, CDDDDR
        [Builtin]
        public static object Car(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object Cdr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Caar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cadr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CADR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cdar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cddr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Caaar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Caadr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAADR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cadar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CADAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Caddr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CADDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cdaar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cdadr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDADR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cddar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cdddr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Caaaar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Caaadr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAAADR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Caadar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAADAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Caaddr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAADDR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cadaar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CADAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cadadr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAADDR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Caddar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child; 
            
            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cadddr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CADDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cdaaar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cdaadr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDAADR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cdadar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Child == null)
                if (_cons.Cdr == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;
                else
                    throw new TypeErrorException("CDR: " + _cons.Cdr + " is not a CONS");

            _cons = _cons.Child;

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cdaddr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cddaar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cddadr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDADR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cdddar(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object Cddddr(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        public static object set_Car(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object set_Cdr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Caar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cadr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CADR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cdar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cddr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Caaar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Caadr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAADR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cadar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CADAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Caddr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CADDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cdaar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cdadr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDADR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cddar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cdddr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Caaaar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Caaadr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAADR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Caadar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAADAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CDR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Caaddr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAADDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CDR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cadaar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cadadr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Caddar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cadddr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CADDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Car;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cdaaar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CAAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Car, "CAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cdaadr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cdadar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cdaddr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cddaar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cddadr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDADR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cdddar(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDAR", "cons");

            if (_cons.Car == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidExcessiveComplexity")]
        public static object set_Cddddr(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "CDDDDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            if (_cons.Cdr == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            _cons = RuntimeHelpers.Coerce<Cons>(_cons.Cdr, "CDR", "cons");

            return _cons.Cdr;
        } 
        #endregion

        [Builtin("copy-tree")]
        public static object CopyTree(object tree)
        {
            Cons cons = tree as Cons;

            if (cons == null)
                return tree;


            return new Cons(CopyTree(cons.Car), CopyTree(cons.Cdr));
        }

        //sublis alist tree &key key test test-not => new-tree

        [Builtin]
        public static object SUBLIS(object alist, object tree, [Key] object key, [Key]object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object NSUBLIS(object alist, object tree, [Key] object key, [Key]object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        #region SUBST, SUBST-IF, SUBST-IF-NOT, NSUBST, NSUBST-IF, NSUBST-IF-NOT
        /* subst new old tree &key key test test-not => new-tree

            subst-if new predicate tree &key key => new-tree

            subst-if-not new predicate tree &key key => new-tree


            nsubst new old tree &key key test test-not => new-tree

            nsubst-if new predicate tree &key key => new-tree

            nsubst-if-not new predicate tree &key key => new-tree
        */

        [Builtin]
        public static object SUBST(object _new, object old, object tree, [Key] object key, [Key]object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("SUBST-IF")]
        public static object SUBST_IF(object _new, object predicate, object tree, [Key] object key, [Key]object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("SUBST-IF-NOT")]
        public static object SUBST_IF_NOT(object _new, object predicate, object tree, [Key] object key, [Key]object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        public static object NSUBST(object _new, object old, object tree, [Key] object key, [Key]object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("NSUBST-IF")]
        public static object NSUBST_IF(object _new, object predicate, object tree, [Key] object key, [Key]object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("NSUBST-IF-NOT")]
        public static object NSUBST_IF_NOT(object _new, object predicate, object tree, [Key] object key, [Key]object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }
        #endregion

        [Builtin("TREE-EQUAL", Predicate = true)] //tree-equal tree-1 tree-2 &key test test-not 
        public static object Tree_EQUAL(object tree1, object tree2, [Key]object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("COPY-LIST")]
        public static object COPY_LIST(object list)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object LIST([Rest] object args)
        {
            return args;
        }

        [Builtin("LIST*")]
        public static object LISTStar([Rest] object args)
        {
            throw new NotImplementedException();
        }

        [Builtin("LIST-LENGTH")]
        public static object LIST_LENGTH(object list)
        {
            throw new NotImplementedException();
        }

        [Builtin("MAKE-LIST")]
        public static object MAKE_LIST(object size,[Key("initial-element")] object initial_element)
        {
            throw new NotImplementedException();
        }

        #region FIRST, SECOND, THIRD, FOURTH, FIFTH, SIXTH, SEVENTH, EIGHTH, NINTH, TENTH
        [Builtin]
        public static object FIRST(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "FIRST", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object SECOND(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "SECOND", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object THIRD(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "THIRD", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object FOURTH(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "FOURTH", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object FIFTH(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "FIFTH", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object SIXTH(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "SIXTH", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object SEVENTH(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "SEVENTH", "cons");

            return _cons.Car;
        }
        [Builtin]
        public static object EIGHTH(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "EIGHTH", "cons");

            return _cons.Car;
        }
        [Builtin]
        public static object NINTH(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "NINTH", "cons");

            return _cons.Car;
        }
        [Builtin]
        public static object TENTH(object cons)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "TENTH", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object set_FIRST(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "FIRST", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object set_SECOND(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "SECOND", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object set_THIRD(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "THIRD", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object set_FOURTH(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "FOURTH", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object set_FIFTH(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "FIFTH", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object set_SIXTH(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "SIXTH", "cons");

            return _cons.Car;
        }

        [Builtin]
        public static object set_SEVENTH(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "SEVENTH", "cons");

            return _cons.Car;
        }
        [Builtin]
        public static object set_EIGHTH(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "EIGHTH", "cons");

            return _cons.Car;
        }
        [Builtin]
        public static object set_NINTH(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "NINTH", "cons");

            return _cons.Car;
        }
        [Builtin]
        public static object set_TENTH(object cons, object value)
        {
            if (cons == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(cons, "TENTH", "cons");

            return _cons.Car;
        }
        #endregion

        [Builtin]
        public static object NTH(object n, object list)
        {
            return (list as Cons)[(int)n];
        }

        [Builtin]
        public static object set_NTH(object n, object list, object value)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object ENDP(object list)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate = true)]
        public static object NULL(object obj)
        {
            return obj == DefinedSymbols.NIL?DefinedSymbols.T:DefinedSymbols.NIL;
        }

        [Builtin]
        public static object NCONC([Rest]object lists)
        {
            if (lists == DefinedSymbols.NIL)
                return lists;

            object x = lists;
            object list = Car(x);
            object ret = DefinedSymbols.NIL;

            while (NULL(x) != DefinedSymbols.T)
            {
                if (!(x is Cons)) ConditionsDictionary.Error("Not a list: " + x);

                if (ret != DefinedSymbols.NIL)
                    Rplacd(LAST(ret, 1), list);
                else
                    ret = list;

                x = Cdr(x);
                list = Car(x);
            }

            return ret;
        }

        [Builtin]
        public static object APPEND(params object[] lists)
        {
            if (lists.Length == 0)
                return DefinedSymbols.NIL;

           var this_ = new Cons(null);
            Cons list = this_;
            for (int i = 0; i < lists.Length -1; i++)
            {
                if (lists[i] != DefinedSymbols.NIL)
                {
                    Cons current_list = lists[i] as Cons;
                    if (current_list == null)
                        ConditionsDictionary.TypeError("APPEND: " + lists[i] + " is not a list");
                    if(!current_list.IsProperList)
                        ConditionsDictionary.TypeError("APPEND: " + lists[i] + " is not a proper list");

                    while (current_list != null)
                    {
                        list.Car = current_list.Car;
                        Cons next = new Cons(null);
                        list.SetCdrDelayed(next);
                        list = next;
                        current_list = current_list.Child;
                    }
                }
            }

             list.Parent.SetCdrDelayed(lists[lists.Length -1]);
            list.Parent.CalculateDelayedLength();
            return this_;

            /*
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
            return this_;*/
           /* Cons current = lists as Cons;
            object last;

            while (current.Cdr != DefinedSymbols.NIL)
            {
                Cons current_list = current.Car as Cons;
                if (current_list == null)
                    ConditionsDictionary.TypeError("APPEND: " + current.Car + " is not a list");
                //
                Cons copy = current_list.MakeCopy();
                while (copy != null)
                {
                    ret.Append(copy.Car);
                    copy = copy.Child;
                }


                current = current.Child;
                last = current.Cdr;
            }

            ret.Last.Cdr = current.Car;

            return ret.Cdr;*/

            // throw new NotImplementedException();
        }

        [Builtin]
        public static object REVAPPEND(object list, object tail)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object NRECONC(object list, object tail)
        {
            Cons cons = list as Cons;
            if(cons == null)
                ConditionsDictionary.TypeError("NRECONC: argument 1 is not a list " + list);
            object reversed = SequencesDictionary.NReverse(cons);

            return ConsesDictionary.NCONC(new Cons(reversed, new Cons(tail)));
        }

        [Builtin]
        public static object BUTLAST(object list, [Optional] object n)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object NBUTLAST(object list, [Optional] object n)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object LAST(object list, [Optional] object n)
        {
            Cons _list = list as Cons;

            if (_list == null)
                ConditionsDictionary.TypeError("LAST: argument 1 is not a list (" + list+ ")");

            
            if (!(n is int))
                ConditionsDictionary.TypeError("DIGIT-CHAR-P: argument 2 is not a integer (" + n + ")");

            int _n = (int)n;

            int skip = _list.Count - _n;
            if (skip > 0)
            {
                for (int i = 0; i < skip; i++)
                {
                    list = Cdr(list);
                }

                return list;
            }
            else
            {
                return list;
            }
        }

        [Builtin]
        public static object LDIFF(object list, object obj)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate=true)]
        public static object TAILP(object obj, object list)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object NTHCDR(object n, object list)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object REST(object list)
        {
            if (list == DefinedSymbols.NIL)
                return DefinedSymbols.NIL;

            Cons _cons = RuntimeHelpers.Coerce<Cons>(list, "REST", "cons");

            return _cons.Cdr;
        }

        [Builtin]
        public static object set_REST(object list)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object MEMBER(object item, object list, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("MEMBER-IF")]
        public static object MEMBER_IF(object predicate, object list, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("MEMBER-IF-NOT")]
        public static object MEMBER_IF_NOT(object predicate, object list, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object MAPC(object function, [Rest] object lists)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object MAPCAR(object function, params object[] lists)
        {
            LispFunction _function = function as LispFunction;
            if (_function == null)
            {
                FunctionName name;
                FunctionNameParseResult pres;
                pres = FunctionName.Create(function, out name);

                switch (pres)
                {
                    case FunctionNameParseResult.NotAName:
                        ConditionsDictionary.TypeError("MAPCAR: " + function + " is not a function name");
                        break;
                    case FunctionNameParseResult.MalformedSetfList:
                        ConditionsDictionary.Error("MAPCAR: " + function + " is malformed setf function name");
                        break;
                    case FunctionNameParseResult.SetfNameNotASymbol:
                        ConditionsDictionary.Error("MAPCAR: " + function + " object after setf is not a symbol");
                        break;
                }

                _function = name.Function;
            }

            Cons[] conses = new Cons[lists.Length];
            int maxlen = 0;
            for (int i = 0; i < lists.Length; i++)
            {
                if (lists[i] == DefinedSymbols.NIL)
                    return DefinedSymbols.NIL;

                Cons c = lists[i] as Cons;

                if(c == null)
                    ConditionsDictionary.TypeError("MAPCAR: " + lists[i] + " is not a list");

                if (!c.IsProperList)
                    ConditionsDictionary.TypeError("MAPCAR: " + c + " is not a proper list");

                if (c.Count > maxlen)
                    maxlen = c.Count;

                conses[i] = c;
            }

            object[] args = new object[lists.Length];

            var this_ = new Cons(null);
            Cons list = this_;
            for (int i = 0; i < maxlen; i++)
            {
                for (int j = 0; j < lists.Length; j++)
                {
                    args[j] = conses[j].Car;
                    conses[j] = conses[j].Child;
                }

                list.Car = _function.Invoke(args);
                Cons next = new Cons(null);
                list.SetCdrDelayed(next);
                list = next;
            }


            list.Parent.SetCdrDelayed(DefinedSymbols.NIL);
            list.Parent.CalculateDelayedLength();

            return this_;
        }

        [Builtin]
        public static object MAPCAN(object function, [Rest] object lists)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object MAPL(object function, [Rest] object lists)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object MAPLIST(object function, [Rest] object lists)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object MAPCON(object function, [Rest] object lists)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object ACONS(object key, object datum, object alist)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object ASSOC(object item, object alist, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("ASSOC-IF")]
        public static object ASSOC_IF(object predicate, object alist, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("ASSOC-IF_NOT")]
        public static object ASSOC_IF_NOT(object predicate, object alist, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("COPY-ALIST")]
        public static object COPY_ALIST(object alist)
        {
            throw new NotImplementedException();
        }

        //pairlis keys data &optional alist 
        [Builtin]
        public static object PAIRLIS(object key, object data, [Optional] object alist)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object RASSOC(object item, object alist, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("RASSOC-IF")]
        public static object RASSOC_IF(object predicate, object alist, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("RASSOC-IF_NOT")]
        public static object RASSOC_IF_NOT(object predicate, object alist, [Key] object key)
        {
            throw new NotImplementedException();
        }

        [Builtin("GET-PROPERTIES", ValuesReturnPolitics=ValuesReturnPolitics.AlwaysNonZero)]
        public static object GET_PROPERTIES(object plist, object indicator_list)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object GETF(object plist, object indicator, [Optional] object def)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object set_GETF(object plist, object indicator, object store)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object INTERSECTION(object list1, object list2, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object NINTERSECTION(object list1, object list2, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object ADJOIN(object item, object list, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("SET-DIFFERENCE")]
        public static object SET_DIFFERENCE(object list1, object list2, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("NSET-DIFFERENCE")]
        public static object NSET_DIFFERENCE(object list1, object list2, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("SET-EXCLUSIVE-OR")]
        public static object SET_EXCLUSIVE_OR(object list1, object list2, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin("NSET-EXCLUSIVE-OR")]
        public static object NSET_EXCLUSIVE_OR(object list1, object list2, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object SUBSETP(object list1, object list2, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object UNION(object list1, object list2, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object NUINON(object list1, object list2, [Key] object key, [Key] object test, [Key("test-not")] object test_not)
        {
            throw new NotImplementedException();
        }
    }
}

#region Compiler Symbols
namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol Nreconc;
        public static Symbol List;
        public static Symbol Cadr;
        public static Symbol Append;
        public static Symbol Nconc;
        public static Symbol ListStar;
        public static Symbol Cons;

        internal static void InitializeConsesDictiononarySymbols()
        {
            Nreconc = cl.Intern("NRECONC", true);
            List = cl.Intern("LIST", true);
            Cadr = cl.Intern("CADR", true);
            Append = cl.Intern("APPEND", true);
            Nconc = cl.Intern("NCONC", true);
            ListStar = cl.Intern("LIST*", true);
            Cons = cl.Intern("CONS", true);
        }
    }
}
#endregion
