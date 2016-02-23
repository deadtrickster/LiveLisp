using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types;
using System.Reflection;
using LiveLisp.Core.AST;
using LiveLisp.Core.CLOS;

namespace LiveLisp.Core.Compiler
{
    /*
     * Начальные условия:
     * 1) Функция это объект с некоторым количеством достаточно произвольных(ламбда-лист, информация для встраивания и тд)
     * 2) не поддерживаемые .net ключевые и прочие параметры
     * 3) в особых случая возможность возвращать несколько значений
     * 4) каждая форма (в том числе и вызов функций) обязательно должна возращать значение, однако во множестве случаев это 
     *      значение просто отбрасывается
     *      
     * Какие близкие механизмы уже существуют в .net
     * Делегаты. Основной проблемой при использовании делегатов в качестве непосредственного контейнера для функции - 
     *  ограниченность его сигнатуры - всегда придётся использовать сигнатуру ojbect[] потому что встроенные средства .net
     *     не смогут правильно обрабатывать некоторые типы параметров. К тому же этот подход будет слишком затратен.
     *     
     * 
     * Вывод: 
     * 
     * Лисп функция должна представлять собой класс методы которого деляться н три категории:
     * 1) Точно возращающие одно значение
     * 2) Могущие возращать несколько значений
     * 3) Не возвращающие значений
     * 
     * В каждой группе содержаться методы с одинковыми параметрами object arg1; object arg1, object arg2 и тд. до object[] args 
     * 
     * Поддержка перегрузок позволяет упростить вызов функции для фиксированного, заранее известного 
     *      числа аргументов отказаться от формирования массива.
     *      
     * кроме того это позволяет проводить множество оптимизаций - заранее известно будет ли предоставлен опциональный аргумент
     *      обеспечиться ли чётное число параметров с ключевым словом, в конце концов заранее известно правильное ли число аргументов
     *      передаётся.
     *      
     * При таком подходе возникают следующие вопросы
     * 
     * 1) Скорость компиляции
     * 2) Скорость вызова для контекста с одним возвращаемым значением
     * 3) Скорость вызова для контекста с нескольким возвращаемым значением
     * 4) Взаимодействие с "обычным" .net кодом, в частности, использование лисп-функций
     *      в качестве обработчиков событий.
     *      
     * 
     * 1) Скорость компиляции 
     *      Действительно, если используется компиляция перегрузок N, то общее число методов 3(N+1). 
     *      При этом для каждого метода проводится оптимизация, что действительно может сказаться на производительности
     *      Особенно нужно учесть, что функция eval может вызываться в готовой программе - и существенно снижать производительность-
     *      например, когда компиляция может занимать больше времени чем общее время выполнения функции во всех вызовах.
     *  Поэтому предлагается три режима 
     *      В-первом(по-умолчанию) компилируются все необходимые перегрузки - наибольшее время компиляции и наименьшее время выполнения
     *      Во-втором компилируются только три метода с сигнатурами object[] args - остальные методы просто содержат 
     *          код формиравания массивов и вызова соответствующих args перегрузок - мало времени компилции, замедленное выполнение
     *      
     *      В-третьем режиме коплируется только перегрузка могущая возвращать несколько значений - наибыстрейшая комплияция и самое медленное выполнение.
     *      
     *      При компиляции в функции также сохраняется инофрмация о режиме компиляции. её можно использовать например в функции funcall
     *          для оптимизации стратегии вызовов.
     *  
     * 2) Скорость вызова для контекста с одним возвращаемым значением
     *  Первый режим
     *      В большинстве случаев при вызове функции если и будет использоваться то только единственное возвращённое значение
     *      Поэтому была введена группа методов гарантированно возвращающих только одно значение.
     *      А т.к. несколько значений может возвращать лишь функция values то при ручной реализации всех необходимых перегрузок
     *      можно оптимизировать соответствующие группы - в группе для одного значение используются только вызовы с одним значением
     *      в группе с возможностью нескольких значений - перегрузки с возможностью нескольких значений.
     *  Второй режим - тоже самое
     *  В третьем режиме 
     *      реализуется только перегрузка с возможностью нескольких значений - в перегрузках для одного вызывается эта перегрузка и происходит
     *      проверка возвращённого объекта - одно или несколько значений и возвращается первое (или в случае 0 значений nil)
     *      
     *  В четвёртом режиме функция компилируется в динамический метод функционально эквивалентный переопределяемому их 3-го режима
     *      Сгенерированный метод укладывается в типовой контейнер. Это самая меделенная реализация, но она позволяет полностью
     *      отказаться от тяжеловесной генерации и использовать только gc-collected код
     * 
     * 3) Скорость вызова для контекста с нескольким возвращаемым значением
     *  Стандартом определяется ограниченное количество форм которые могут работать с несколькими возвращаемыми значениями
     *  Соответствующие перегрузки не обязательно должны возвращать контейнер - в случае возврата одного значения они возвращают собственно это значение
     *  
     * 4) Взаимодействие с "обычным" .net кодом, в частности, использование лисп-функций
     *      в качестве обработчиков событий.
     *  Использовать лисп-функцию в качестве обработчика событий можно, для этого, генерируется динамический метод-обёртка с сигнатурой подходящей для события 
     *  внутри которго происходит необходимое преобразование типов (и/или инициализация массива) и вызов лисп-функции
     *  
     * Лисп-функции не поддерживают ref параметры в виду единообразной сигнатуры. Однако методы с ref параметрами можно вызывать
     *  используя ассемблерные формы.
     *  
     * Важнейшим моментом во взаимодействии обчного дотнет кода и лисп-кода явлеяются преобразование типов
     * В случае подключения к событию это производится автоматически. 
     * В случае вызова лисп функции из обычного дотнет кода вызывающий код должен сам позаботиться о соответствующих преобразованиях
     * При вызове .net кода из лисп-кода преобразованиями должен заниматься лисп код.
     * 
     * Тут же затроним тему 
     */


    public enum OverrideModes
    {
        Full,
        Partial,
        Single
    }

    public abstract class LispFunction : CLRCLOSInstance
    {
        public const int MAX_NONPARAMS_ARGS = 14;

        protected Symbol _name;

        protected string _symbol_name;

        public Symbol Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual object LambdaExpression
        {
            get { return DefinedSymbols.NIL; }
            set
            {
                throw new NotImplementedException();
                //things like dynamic code injecting ^-)
            }
        }

        public virtual bool IsCompiled
        {
            get { return true; }

            set
            {
                throw new NotImplementedException();
                //things like dynamic compilation / decompilation ^-)
            }
        }

        bool isClosure = false;

        public virtual bool IsClosure
        {
            get { return isClosure; }
        }

        public virtual Delegate ToDelegate(Type delegateType)
        {
            throw new NotImplementedException();
        }

        protected LispFunction(Symbol name, bool isunbound)
        {
            _name = name;
            _symbol_name = _name.ToString(true);
            IsUnbound = isunbound;
        }

        protected LispFunction(string _symbol_name, bool isunbound)
        {
            this._symbol_name = _symbol_name;
            IsUnbound = isunbound;
        }
        public int MinArgsCount;

        public int MaxArgsCount;

        public readonly bool IsUnbound;

        protected void CheckArgsCount(int actual_args_count)
        {
            if (MinArgsCount > actual_args_count)
                throw new TooFewArgsException(_symbol_name, /*actual args count*/actual_args_count, /*requred minimum args count*/MinArgsCount);
            if (MaxArgsCount < actual_args_count)
                throw new TooManyArgsException(_symbol_name, actual_args_count, MaxArgsCount);
        }
        #region single return
        public abstract object Invoke();
        public abstract object Invoke(object arg1);
        public abstract object Invoke(object arg1, object arg2);
        public abstract object Invoke(object arg1, object arg2, object arg3);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13);
        public abstract object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14);
        public abstract object Invoke(object[] args);
        #endregion

        #region multiple return
        public abstract object ValuesInvoke();
        public abstract object ValuesInvoke(object arg1);
        public abstract object ValuesInvoke(object arg1, object arg2);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13);
        public abstract object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14);
        public abstract object ValuesInvoke(object[] args);
        #endregion

        #region void return
        public abstract void VoidInvoke();
        public abstract void VoidInvoke(object arg1);
        public abstract void VoidInvoke(object arg1, object arg2);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13);
        public abstract void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14);
        public abstract void VoidInvoke(object[] args);
        #endregion
    }

    public sealed class UnboundFunction : LispFunction
    {
        string message;
        public UnboundFunction(Symbol name)
            : base(name, true)
        {
            message = "Unbound function " + name.ToString(true);
        }

        public override object ValuesInvoke()
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            throw new UnboundFunctionException(message);
        }

        public override object ValuesInvoke(object[] args)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke()
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            throw new UnboundFunctionException(message);
        }

        public override void VoidInvoke(object[] args)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke()
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            throw new UnboundFunctionException(message);
        }

        public override object Invoke(object[] args)
        {
            throw new UnboundFunctionException(message);
        }
    }

    public abstract class DefaultBehavior : LispFunction
    {
        public DefaultBehavior(Symbol name)
            : base(name, false)
        {

        }

        public DefaultBehavior(string function_name)
            : base(function_name, false)
        {

        }

        protected abstract object DoDefault();

        public override object ValuesInvoke()
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            return DoDefault();
        }

        public override object ValuesInvoke(object[] args)
        {
            return DoDefault();
        }

        public override void VoidInvoke()
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            DoDefault();
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            DoDefault();
        }

        public override void VoidInvoke(object[] args)
        {
            DoDefault();
        }

        public override object Invoke()
        {
            return DoDefault();
        }

        public override object Invoke(object arg1)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            return DoDefault();
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            return DoDefault();
        }

        public override object Invoke(object[] args)
        {
            return DoDefault();
        }
    }

    public abstract class FullLispFunction : LispFunction
    {
        public FullLispFunction(string functionName)
            : base(functionName, false)
        {

        }

        #region single return

        public override object Invoke()
        {
            CheckArgsCount(0);

            throw new InvalidOperationException("if this exception was thrown then lambda(0) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1)
        {
            CheckArgsCount(1);

            throw new InvalidOperationException("if this exception was thrown then lambda(1) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2)
        {
            CheckArgsCount(2);

            throw new InvalidOperationException("if this exception was thrown then lambda(2) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3)
        {
            CheckArgsCount(3);

            throw new InvalidOperationException("if this exception was thrown then lambda(3) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4)
        {
            CheckArgsCount(4);

            throw new InvalidOperationException("if this exception was thrown then lambda(4) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            CheckArgsCount(5);

            throw new InvalidOperationException("if this exception was thrown then lambda(5) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            CheckArgsCount(6);

            throw new InvalidOperationException("if this exception was thrown then lambda(6) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            CheckArgsCount(7);

            throw new InvalidOperationException("if this exception was thrown then lambda(7) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            CheckArgsCount(8);

            throw new InvalidOperationException("if this exception was thrown then lambda(8) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            CheckArgsCount(9);

            throw new InvalidOperationException("if this exception was thrown then lambda(9) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            CheckArgsCount(10);

            throw new InvalidOperationException("if this exception was thrown then lambda(10) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            CheckArgsCount(11);

            throw new InvalidOperationException("if this exception was thrown then lambda(11) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            CheckArgsCount(12);

            throw new InvalidOperationException("if this exception was thrown then lambda(12) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            CheckArgsCount(13);

            throw new InvalidOperationException("if this exception was thrown then lambda(13) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            CheckArgsCount(14);

            throw new InvalidOperationException("if this exception was thrown then lambda(13 + args.Length) generator totally fail when it generated Invokes");
        }

        public override object Invoke(object[] args)
        {
            CheckArgsCount(args.Length);

            throw new InvalidOperationException("if this exception was thrown then lambda(args.Length) generator totally fail when it generated Invokes");
        }
        #endregion

        #region values return

        public override object ValuesInvoke()
        {
            CheckArgsCount(0);

            throw new InvalidOperationException("if this exception was thrown then lambda(0) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1)
        {
            CheckArgsCount(1);

            throw new InvalidOperationException("if this exception was thrown then lambda(1) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2)
        {
            CheckArgsCount(2);

            throw new InvalidOperationException("if this exception was thrown then lambda(2) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3)
        {
            CheckArgsCount(3);

            throw new InvalidOperationException("if this exception was thrown then lambda(3) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            CheckArgsCount(4);

            throw new InvalidOperationException("if this exception was thrown then lambda(4) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            CheckArgsCount(5);

            throw new InvalidOperationException("if this exception was thrown then lambda(5) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            CheckArgsCount(6);

            throw new InvalidOperationException("if this exception was thrown then lambda(6) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            CheckArgsCount(7);

            throw new InvalidOperationException("if this exception was thrown then lambda(7) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            CheckArgsCount(8);

            throw new InvalidOperationException("if this exception was thrown then lambda(8) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            CheckArgsCount(9);

            throw new InvalidOperationException("if this exception was thrown then lambda(9) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            CheckArgsCount(10);

            throw new InvalidOperationException("if this exception was thrown then lambda(10) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            CheckArgsCount(11);

            throw new InvalidOperationException("if this exception was thrown then lambda(11) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            CheckArgsCount(12);

            throw new InvalidOperationException("if this exception was thrown then lambda(12) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            CheckArgsCount(13);

            throw new InvalidOperationException("if this exception was thrown then lambda(13) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            CheckArgsCount(14);

            throw new InvalidOperationException("if this exception was thrown then lambda(13 + args.Length) generator totally fail when it generated Invokes");
        }

        public override object ValuesInvoke(object[] args)
        {
            CheckArgsCount(args.Length);

            throw new InvalidOperationException("if this exception was thrown then lambda(args.Length) generator totally fail when it generated Invokes");
        }
        #endregion 

        #region void return
        public override void VoidInvoke()
        {
            CheckArgsCount(0);

            throw new InvalidOperationException("if this exception was thrown then lambda(0) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1)
        {
            CheckArgsCount(1);

            throw new InvalidOperationException("if this exception was thrown then lambda(1) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2)
        {
            CheckArgsCount(2);

            throw new InvalidOperationException("if this exception was thrown then lambda(2) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3)
        {
            CheckArgsCount(3);

            throw new InvalidOperationException("if this exception was thrown then lambda(3) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            CheckArgsCount(4);

            throw new InvalidOperationException("if this exception was thrown then lambda(4) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            CheckArgsCount(5);

            throw new InvalidOperationException("if this exception was thrown then lambda(5) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            CheckArgsCount(6);

            throw new InvalidOperationException("if this exception was thrown then lambda(6) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            CheckArgsCount(7);

            throw new InvalidOperationException("if this exception was thrown then lambda(7) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            CheckArgsCount(8);

            throw new InvalidOperationException("if this exception was thrown then lambda(8) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            CheckArgsCount(9);

            throw new InvalidOperationException("if this exception was thrown then lambda(9) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            CheckArgsCount(10);

            throw new InvalidOperationException("if this exception was thrown then lambda(10) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            CheckArgsCount(11);

            throw new InvalidOperationException("if this exception was thrown then lambda(11) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            CheckArgsCount(12);

            throw new InvalidOperationException("if this exception was thrown then lambda(12) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            CheckArgsCount(13);

            throw new InvalidOperationException("if this exception was thrown then lambda(13) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            CheckArgsCount(14);

            throw new InvalidOperationException("if this exception was thrown then lambda(13 + args.Length) generator totally fail when it generated Invokes");
        }

        public override void VoidInvoke(object[] args)
        {
            CheckArgsCount(args.Length);

            throw new InvalidOperationException("if this exception was thrown then lambda(args.Length) generator totally fail when it generated Invokes");
        }
        #endregion
    }

    public abstract class PartialFunction : LispFunction
    {
        public PartialFunction(string functionName)
            : base(functionName, false)
        {

        }

        #region single return
        public override object Invoke()
        {
            return Invoke(new object[0]);
        }

        public override object Invoke(object arg1)
        {
            return Invoke(new object[] { arg1});
        }

        public override object Invoke(object arg1, object arg2)
        {
            return Invoke(new object[] { arg1, arg2});
        }

        public override object Invoke(object arg1, object arg2, object arg3)
        {
            return Invoke(new object[] { arg1, arg2, arg3});
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4)
        {
            return Invoke(new object[] { arg1, arg2, arg3, arg4});
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return Invoke(new object[] { arg1, arg2, arg3, arg4, arg5});
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return Invoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6});
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return Invoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7});
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return Invoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8});
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            return Invoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9});
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            return Invoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10});
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            return Invoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 });
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            return Invoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12});
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            return Invoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 });
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            return Invoke(new object[]{arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14});
        }

        public override object Invoke(object[] args)
        {
            CheckArgsCount(args.Length);

            throw new InvalidOperationException("if this exception was thrown then lambda(args.Length) generator totally fail when it generated Invokes");
        
        }
        #endregion

        #region values return

        public override object ValuesInvoke()
        {
            return ValuesInvoke(new object[0]);
        }

        public override object ValuesInvoke(object arg1)
        {
            return ValuesInvoke(new object[] { arg1 });
        }

        public override object ValuesInvoke(object arg1, object arg2)
        {
            return ValuesInvoke(new object[] { arg1, arg2 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 });
        }

        public override object ValuesInvoke(object[] args)
        {
            CheckArgsCount(args.Length);

            throw new InvalidOperationException("if this exception was thrown then lambda(args.Length) generator totally fail when it generated Invokes");
        }
        #endregion

        #region void return
        public override void VoidInvoke()
        {
            VoidInvoke(new object[0]);
        }

        public override void VoidInvoke(object arg1)
        {
            VoidInvoke(new object[] { arg1 });
        }

        public override void VoidInvoke(object arg1, object arg2)
        {
            VoidInvoke(new object[] { arg1, arg2 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3, arg4 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3, arg4, arg5 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
           VoidInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            VoidInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 });
        }

        public override void VoidInvoke(object[] args)
        {
            CheckArgsCount(args.Length);

            throw new InvalidOperationException("if this exception was thrown then lambda(args.Length) generator totally fail when it generated Invokes");
        }
        #endregion
    }

    public abstract class SingleFunction : LispFunction
    {
        public SingleFunction(string functionName)
            : base(functionName, false)
        {

        }

        #region single return
        public override object Invoke()
        {
            Object ret = ValuesInvoke(new object[0]);

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1)
        {
            Object ret = ValuesInvoke(new object[] { arg1});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13});

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            Object ret = ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }

        public override object Invoke(object[] args)
        {
            Object ret = ValuesInvoke(args);

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return ret;
        }
        #endregion

        #region values return

        public override object ValuesInvoke()
        {
            return ValuesInvoke(new object[0]);
        }

        public override object ValuesInvoke(object arg1)
        {
            return ValuesInvoke(new object[] { arg1 });
        }

        public override object ValuesInvoke(object arg1, object arg2)
        {
            return ValuesInvoke(new object[] { arg1, arg2 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            return ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 });
        }

        public override object ValuesInvoke(object[] args)
        {
            CheckArgsCount(args.Length);

            throw new InvalidOperationException("if this exception was thrown then lambda(args.Length) generator totally fail when it generated Invokes");
        }
        #endregion

        #region void return
        public override void VoidInvoke()
        {
            ValuesInvoke(new object[0]);
        }

        public override void VoidInvoke(object arg1)
        {
            ValuesInvoke(new object[] { arg1 });
        }

        public override void VoidInvoke(object arg1, object arg2)
        {
            ValuesInvoke(new object[] { arg1, arg2 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            ValuesInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 });
        }

        public override void VoidInvoke(object[] args)
        {
            ValuesInvoke(args);
        }
        #endregion
    }

    public delegate object DynamicFunctionDelegate(object[] args);

    public abstract class DynamicMethodBasedFunction : LispFunction
    {
        public DynamicMethodBasedFunction(string functionName, DynamicFunctionDelegate dynamic, List<object> scope)
            : base(functionName, false)
        {
            DynamicMethodInstance = dynamic;
        }

        DynamicFunctionDelegate DynamicMethodInstance;

        #region single return
        public override object Invoke()
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[0]);

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 });

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }

        public override object Invoke(object[] args)
        {
            Object ret = DynamicMethodInstance.DynamicInvoke(args);

            MultipleValuesContainer values = ret as MultipleValuesContainer;

            if (values != null)
                if (values.Count == 0)
                    return DefinedSymbols.NIL;
                else
                    return values.First;

            else return values;
        }
        #endregion

        #region values return

        public override object ValuesInvoke()
        {
            return DynamicMethodInstance.DynamicInvoke(new object[0]);
        }

        public override object ValuesInvoke(object arg1)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1 });
        }

        public override object ValuesInvoke(object arg1, object arg2)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 });
        }

        public override object ValuesInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            return DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 });
        }

        public override object ValuesInvoke(object[] args)
        {
            return DynamicMethodInstance.DynamicInvoke(args);
        }
        #endregion

        #region void return
        public override void VoidInvoke()
        {
            DynamicMethodInstance.DynamicInvoke(new object[0]);
        }

        public override void VoidInvoke(object arg1)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1 });
        }

        public override void VoidInvoke(object arg1, object arg2)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13 });
        }

        public override void VoidInvoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)
        {
            DynamicMethodInstance.DynamicInvoke(new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14 });
        }

        public override void VoidInvoke(object[] args)
        {
            DynamicMethodInstance.DynamicInvoke(args);
        }
        #endregion
    }

    public static class LambdaCallEmitterHelper
    {
        public static FieldInfo MinArgsField = typeof(LispFunction).GetField("MinArgsCount");
        public static FieldInfo MaxArgsField = typeof(LispFunction).GetField("MaxArgsCount");

        internal static readonly List<MethodInfo> InvokeMethods;
        internal static readonly List<MethodInfo> ValuesInvokeMethods;
        internal static readonly List<MethodInfo> VoidInvokeMethods;
        internal static readonly List<Type[]> argsLists;
        internal static readonly StaticMethodResolver CheckArgsMethod;
        static LambdaCallEmitterHelper()
        {
            argsLists = new List<Type[]>();
            argsLists.Add(Type.EmptyTypes);
            argsLists.Add(new Type[] { typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object), typeof(object) });
            argsLists.Add(new Type[] { typeof(object[]) });

            InvokeMethods = new List<MethodInfo>();
            fillinvoke(InvokeMethods, "Invoke");
            ValuesInvokeMethods = new List<MethodInfo>();
            fillinvoke(ValuesInvokeMethods, "ValuesInvoke");
            VoidInvokeMethods = new List<MethodInfo>();
            fillinvoke(VoidInvokeMethods, "VoidInvoke");

            CheckArgsMethod  = typeof(LispFunction).GetMethod("CheckArgsCount", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static void fillinvoke(List<MethodInfo> list, string name)
        {
            foreach (var item in argsLists)
            {
                list.Add(typeof(LispFunction).GetMethod(name, BindingFlags.Public | BindingFlags.Instance, null, item, null));
            }
        }
    }
}
