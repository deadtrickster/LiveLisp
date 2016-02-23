using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using LiveLisp.Core.CLOS;

namespace LiveLisp.Core.BuiltIns.TypesAndClasses
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class TypesAndClassesDictionary
    {
        [Builtin]
        public static object Coerce(object obj, object result_type)
        {
            throw new NotImplementedException();
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static object Subtypep(object obj1, object obj2, [Optional] object env)
        {
            throw new NotImplementedException();
        }

        [Builtin("type-of")]
        public static object TypeOf(object obj, [Optional] object return_dotnet_type)
        {
            if (return_dotnet_type == DefinedSymbols.T)
            {
                return obj.GetType();
            }
            throw new NotImplementedException();
        }

        [Builtin(Predicate=true)]
        public static object Typep(object obj1, object type_spec, [Optional] object env)
        {
            throw new NotImplementedException();
        }

        [Builtin("type-error-datum")]
        public static object TypeErrorDatum(object condition)
        {
            throw new NotImplementedException();
        }

        [Builtin("type-error-expected-type")]
        public static object TypeErrorExpectedType(object condition)
        {
            throw new NotImplementedException();
        }

        [Builtin("SYSTEM::MAKE-CLASS-BUILDER", OverridePackage = true)]
        public static object MakeClassBuilder(object name, [Optional] object ErrorIfExist)
        {
            Symbol symbol = name as Symbol;

            if (symbol == null)
                throw new SimpleErrorException("MAKE-CLASS-BUILDER: class name is not a symbol " + name);

            if (CLOSTypeTable.Instance.Contains(symbol.Id))
            {
                if (ErrorIfExist == DefinedSymbols.T)
                    return DefinedSymbols.NIL;
                else
                    throw new NotImplementedException(); // return new CLOSClassBuilder(CLOSTypeTable.Instance[symbol.Id]);
            }
            else
                throw new NotImplementedException(); // return new CLOSClassBuilder(symbol);
        }

        [Builtin("SYSTEM::ADD-INSTANCE-SLOT", OverridePackage = true)]
        public static object AddInstanceSlot(object _classBuilder, object _name, object _defvalue)
        {
            throw new NotImplementedException();
            /*
            CLOSClassBuilder builder = _classBuilder as CLOSClassBuilder;

            if (builder == null)
                throw new SimpleErrorException("ADD-INSTANCE-SLOT: first argument is not a ClassBuilder");

            Symbol name = _name as Symbol;

            if (name == null)
                throw new SimpleErrorException("ADD-INSTANCE-SLOT: slot name is not a symbol");

            builder.ChangeSlot(name, _defvalue);

            return name;*/
        }

        [Builtin("SYSTEM::FINALIZE-CLASS", OverridePackage = true)]
        public static object FinilizeClass(object _classBuilder)
        {
            throw new NotImplementedException();
          /*  CLOSClassBuilder builder = _classBuilder as CLOSClassBuilder;

            if (builder == null)
                throw new SimpleErrorException("FINALIZE-CLASS: first argument is not a ClassBuilder");

            return builder.GetClass();*/
        }

        [Builtin("SYSTEM::MAKE-INSTANCE", OverridePackage = true)]
        public static object CreateInstance(object _classBuilder)
        {
            CLOSClass cl = _classBuilder as CLOSClass;

            if (cl == null)
            {
                Symbol name = _classBuilder as Symbol;
                if (name == null)
                    throw new SimpleErrorException("MAKE-INSTANCE: first argument is not a Class designator");

                if(!CLOSTypeTable.Instance.Contains(name.Id))
                    throw new SimpleErrorException("MAKE-INSTANCE: class " + name + " not found");

                cl = CLOSTypeTable.Instance[name.Id];

            }

            return cl.CreateInstance();
        }

        [Builtin("SYSTEM::GET-INSTANCE-SLOT-VALUE", OverridePackage = true)]
        public static object GetInstanceSlotValue(object _instance, object _name)
        {
            CLOSInstance instance = _instance as CLOSInstance;

            //if(instance == null)
           //     instance = new ClosClrObjectInstance

            if (instance == null)
                throw new SimpleErrorException("GET-INSTANCE-SLOT-VALUE: first argument is not a CLOS instance");

            Symbol name = _name as Symbol;

            if (name == null)
                throw new SimpleErrorException("GET-INSTANCE-SLOT-VALUE: slot name is not a symbol");

            return instance[name];
        }

        [Builtin("SYSTEM::SET-INSTANCE-SLOT-VALUE", OverridePackage = true)]
        public static object SetInstanceSlotValue(object _instance, object _name, object value)
        {
            CLOSInstance instance = _instance as CLOSInstance;

            //if(instance == null)
            //     instance = new ClosClrObjectInstance

            if (instance == null)
                throw new SimpleErrorException("SET-INSTANCE-SLOT-VALUE: first argument is not a CLOS instance");

            Symbol name = _name as Symbol;

            if (name == null)
                throw new SimpleErrorException("SET-INSTANCE-SLOT-VALUE: slot name is not a symbol");

            instance[name] = value;

            return value;
        }
    }
}
