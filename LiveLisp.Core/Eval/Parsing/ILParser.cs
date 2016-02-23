using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types;
using LiveLisp.Core.AST;
using LiveLisp.Core.BuiltIns.Numbers;

namespace LiveLisp.Core.Eval
{
    static class ILParser
    {
        internal static void EnsureNoArgs(object cdr)
        {
            if (cdr != DefinedSymbols.NIL)
                throw new SyntaxErrorException("noargs expected");
        }

        internal static Symbol ParseLabel(object cdr)
        {
            Cons cdr_cons = cdr as Cons;
            if (cdr_cons != null)
            {
                if (cdr_cons.Count > 1)
                    throw new SyntaxErrorException("too many args");
                cdr = cdr_cons.Car;
            }

            Symbol label = cdr as Symbol;
            if (label == null)
                throw new SyntaxErrorException("label name not found");

            return label;
        }

        internal static StaticTypeResolver ParseType(object cdr)
        {
            Cons cdr_cons = cdr as Cons;
            if (cdr_cons != null)
            {
                if (cdr_cons.Count > 1)
                    throw new SyntaxErrorException("too many args");
                cdr = cdr_cons.Car;
            }

            Symbol simpleTypeName = cdr as Symbol;
            if (simpleTypeName != null)
                return new StaticTypeResolver(simpleTypeName.Name.ToString());

            else throw new NotImplementedException("complex types not implemented");
        }

        internal static StaticMethodResolver  ParseMethod(object cdr)
        {
            Cons cdr_cons = cdr as Cons;
            if (cdr_cons != null)
            {
                if (cdr_cons.Count > 1)
                    throw new SyntaxErrorException("too many args");
                cdr = cdr_cons.Car;
            }

            Symbol simple_name = cdr as Symbol;

            if (simple_name != null)
                return StaticMethodResolver.Create(simple_name);

            cdr_cons = cdr as Cons;
            if (cdr_cons != null)
            {
                if (cdr_cons.Count < 2)
                    throw new SyntaxErrorException("too few args");

                StaticTypeResolver type = ParseType(cdr_cons.Car);
                simple_name = cdr_cons.Second as Symbol;
                if (simple_name == null)
                    throw new SyntaxErrorException("method name is not a symbol");

                if ((cdr_cons.Cdr as Cons).Cdr == DefinedSymbols.NIL)
                    return StaticMethodResolver.Create(type, simple_name);

                cdr_cons = (cdr_cons.Cdr as Cons).Cdr as Cons;
                if (cdr_cons == null)
                    throw new SyntaxErrorException("invalid method specializer list");

                List<StaticTypeResolver> args = new List<StaticTypeResolver>();
                foreach (var item in cdr_cons)
                {
                    args.Add(ParseType(item));
                }

                return StaticMethodResolver.Create(type, simple_name, args);
            }

            throw new SyntaxErrorException("method spec not found");
        }

        internal static short ParseSlotNum(object cdr)
        {
            throw new NotImplementedException();
        }

        internal static Int32 ParseInt32(object cdr)
        {
            Cons cdr_cons = cdr as Cons;
            if (cdr_cons != null)
            {
                if (cdr_cons.Count > 1)
                    throw new SyntaxErrorException("to many args");
                cdr = cdr_cons.Car;
            }

            if (!(cdr is int))
                throw new SyntaxErrorException("integer not found");

            return (int)cdr;
        }

        internal static long ParseInt64(object cdr)
        {
            throw new NotImplementedException();
        }

        internal static float ParseSignle(object cdr)
        {
            throw new NotImplementedException();
        }

        internal static float ParseSingle(object cdr)
        {
            throw new NotImplementedException();
        }

        internal static double ParseDouble(object cdr)
        {
            throw new NotImplementedException();
        }

        internal static StaticFieldResolver ParseField(object cdr)
        {
            throw new NotImplementedException();
        }

        internal static bool TryParseSlotNum(object cdr, out short? loc_slot_num)
        {
            throw new NotImplementedException();
        }

        internal static Symbol ParseSlotName(object cdr)
        {
            throw new NotImplementedException();
        }

        internal static string ParseString(object cdr)
        {
            throw new NotImplementedException();
        }

        internal static bool TryParseInt64(object cdr, out long? loc_slot_num)
        {
            throw new NotImplementedException();
        }

        internal static object ParseSingleObject(object cdr)
        {
            if (cdr == DefinedSymbols.NIL)
                throw new SyntaxErrorException("Object was expected by opcode");

            Cons cons = cdr as Cons;
            if (cons.Count > 1)
            {
                throw new SyntaxErrorException("Only one object was expected by opcode");
            }

            return cons.Car;
        }

        internal static StaticConstructorResolver ParseConstructor(object cdr)
        {
            throw new NotImplementedException();
        }
    }
}
