using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using LiveLisp.Core.AST;
using LiveLisp.Core.Types;
using System.Reflection;

namespace LiveLisp.Core.Compiler
{

    internal class LambdaGenerator
    {
        /// <summary>
        /// для каждой перегрузки нужно сделать маппинг между параметрами перегрузки и параметрами с которыми работает тело лямбды
        /// в передидущей версии это делалось в цикле в методе класса LambdaMethod сейчас, для ускорения предлагается генерировать "раскрытый"
        /// цикл прямо в метод.
        /// </summary>
        /// <param name="paramscount"></param>
        /// <param name="list"></param>
        /// <param name="ilgen"></param>
        /// <param name="currentBindings">bindings initialized with current overload slots</param>
        public static void GeneratePrologForOverload(Symbol method_name, int paramscount, LambdaList list, ILGenerator ilgen, List<Binding> currentBindings)
        {
            if (list.MinParamsCount > paramscount)
                GenerateTooFewArgsException(method_name.ToString(), ilgen, list.MinParamsCount);

            if (list.MaxParamsCount != -1 && list.MaxParamsCount < paramscount)
                GenerateTooManyArgsException(method_name.ToString(), ilgen, list.MaxParamsCount);

            int align = 1;
            
           // int startindexforrest = 0;

            if (list.RequireEnvironment) // true for all macros, slot for environment already setuped before this call
            {
                align = 2;
            }
            int i = align;
            for (; i < paramscount; i++) // param0 for daved for this
            {
                int alginedindex = i - align;
                LambdaParameter parameter = list[alginedindex];

                switch (parameter.Kind)
                {
                    case ParameterKind.Required:
                      /*  ListParameterName destruct = parameter.Name as ListParameterName;
                        if(destruct != null)
                        {
                            EmitDestructuring(destruct, currentBindings[alginedindex], ilgen);
                        }*/
                        break;
                    case ParameterKind.Optional:
                        OptionalParameter oparameter = parameter as OptionalParameter;
                      /*  destruct = parameter.Name as ListParameterName;
                        if (destruct != null)
                        {
                            EmitDestructuring(destruct, currentBindings[alginedindex], ilgen);
                        }*/
                        if (oparameter.Suppliedp)
                        {
                            // следующий слот в currentBindings должен быть на этот параметер
                            LocalBuilder supp = ilgen.DeclareLocal(typeof(bool));
                            ilgen.EmitBoolean(true);
                            ilgen.EmitLocalSet(supp);
                            currentBindings[alginedindex + 1].Slot = new LocalSlot(supp.LocalIndex);
                        }
                        break;
                    case ParameterKind.Key:
                        break;
                    case ParameterKind.Aux:
                        throw new NotSupportedException("aux parameter must be removed in function special form");
                    default:
                        break;
                }
            }

            if (list.ParamsKind != ParamsKind.None)
            {
                if (list.ParamsIndex >= paramscount)
                {
                    if (list.ParamsKind == ParamsKind.Params)
                    {
                        // просто загрузить пустой массив
                    }
                    else
                    {
                        // просто загрузить пустой список
                    }
                }

                // создать массив заполненный соответствующими слотами начиная с ParamsIndex

                // если ParamsKind == ParamsKind.Rest создать из массива список, иначе просто загрузить в слот
            }


            // после установки всех слотов формируем &whole параметр
            if (list.WholeIndex != -1)
            {
                // загружаем имя

                // создаём массив для всех параметров, причём заполняем его уже значениями из актуальных слотов,
                // с которыми будет работать тело.
            }
        }

        private static void EmitDestructuring(Symbol destruct, Binding binding, ILGenerator ilgen)
        {
            throw new NotImplementedException();
        }

        

        private static void GenerateTooFewArgsException(string method_name, ILGenerator ilgen, int minargs)
        {
            Type exType = typeof(TooFewArgsException);
            var constrnfo = exType.GetConstructor(new Type[] { typeof(string), typeof(object) });

            ilgen.EmitString(method_name + ": too few args. Minimum number of args is " + minargs + ".");
            ilgen.EmitInt32(minargs);

            ilgen.EmitNew(constrnfo);

            ilgen.ThrowException(typeof(TooFewArgsException));
        }

        private static void GenerateTooManyArgsException(string method_name, ILGenerator ilgen, int maxargs)
        {
            Type exType = typeof(TooManyArgsException);
            var constrnfo = exType.GetConstructor(new Type[] { typeof(string), typeof(object) });

            ilgen.EmitString(method_name + ": too many args. Maximum number of args is " + maxargs + ".");
            ilgen.EmitInt32(maxargs);

            ilgen.EmitNew(constrnfo);

            ilgen.ThrowException(typeof(TooManyArgsException));
        }



    /*    // две функции импорта - представляют соответствующие методы как тело лямбды
        // и поэтому генерирует перегрузки для различного числа параметров.
        //
        // в случае если в исходном типе есть перегрузка метода по типам, причём для одинакового числа 
        // параметров, генерируется диспетчеризация, потипу generic methods
        public static void ImportStaticMethodsFromType(Type type, BindingFlags flags)
        {
            if ((flags & BindingFlags.Instance) == BindingFlags.Instance)
            {
                throw new ArgumentException("ImportStaticMethodsFromType: instance bindingflag not allowed");
            }

            _ImportMethods(type, flags, null);
        }

        public static void ImportInstanceMethodsFromType(object instance, BindingFlags flags)
        {
            if ((flags & BindingFlags.Static) == BindingFlags.Static)
            {
                throw new ArgumentException("ImportInstanceMethodsFromType: static bindingflag not allowed");
            }

            _ImportMethods(instance.GetType(), flags, instance);
        }

        /// <summary>
        /// before call this ensure that if instance not null Instance binding flag setted without Static binding flag
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <param name="instance"></param>
        static void _ImportMethods(Type type, BindingFlags flags, object instance)
        {
            MethodInfo[] _methods = type.GetMethods(flags);

            for (int i = 0; i < _methods.Length; i++)
            {
                _ImportMethod(_methods[i], instance);
            }
        }

        private static void _ImportMethod(MethodInfo methodInfo, object instance)
        {
            LambdaList llist = new ClrMethodLambdaList(methodInfo);


        }*/
    }
}
