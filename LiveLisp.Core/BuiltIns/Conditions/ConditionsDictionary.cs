using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using LiveLisp.Core.CLOS;
using LiveLisp.Core.Types.Streams;

namespace LiveLisp.Core.BuiltIns.Conditions
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class ConditionsDictionary
    {
        [Builtin("cell-error-name")]
        public static object CellErrorName(object condition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// As a consequence of calling invoke-debugger, error cannot directly return
        /// </summary>
        /// <param name="datum"></param>
        /// <param name="args"></param>
        [Builtin("error")]
        public static object Error(object datum, params object[] args)
        {
            string _format = datum as string;

            object condition = null;

            if (_format != null)
            {
                condition = ConstructSimpleErrorCondition(_format, args);
            }

            Symbol symbol = datum as Symbol;
            if (symbol != null)
            {
                condition = CreateCustomCondition(symbol, args);
            }

            CLOSClass type = datum as CLOSClass;
            if (type != null)
            {
                condition = CreateCustomCondition(type, args);
            }

            if (datum is LispCondition)
            {
                condition = datum;
            }
            else
            {
                condition = CreateCustomCondition(LispCondition.TypeError, new object[] { "ERROR: unknown condition designator", datum});
            }

            ConditionHandler handler;

            if (!HRManager.FindHandler(condition, out handler))
            {
                return InvokeDebugger(condition);
            }

           return handler.Handler.ValuesInvoke(condition);
        }

        private static object CreateCustomCondition(CLOSClass type, object[] args)
        {
            throw new NotImplementedException();
        }

        private static object CreateCustomCondition(Symbol symbol, object[] args)
        {
            throw new NotImplementedException();
        }

        private static object ConstructSimpleErrorCondition(string _format, object[] args)
        {
            object instance =  Conditions.LispCondition.SimpleError.CreateInstance();
            LispCondition.SimpleError.SetSlot(instance, KeywordPackage.Instance.Intern("FORMAT-CONTROL"), _format);
            LispCondition.SimpleError.SetSlot(instance, KeywordPackage.Instance.Intern("FORMAT-ARGUMENTS"), args);
            return instance;
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.Void)]
        public static void Cerror(object continue_format_control, object datum, params object[] args)
        {
            throw new NotImplementedException();
        }

        [Builtin("invalid-method-error", ValuesReturnPolitics = ValuesReturnPolitics.Void)]
        public static void InvalidMethodError(object method, object format_control, params object[] args)
        {
            throw new NotImplementedException();
        }

        [Builtin("method-combination-error", ValuesReturnPolitics = ValuesReturnPolitics.Void)]
        public static void MethodCombinationError(object format_control, params object[] args)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static void Signal(object datum, params object[] argument)
        {
            throw new NotImplementedException();
        }

        [Builtin("simple-condition-format-control")]
        public static object SimpleConditionFormatControl(object simple_condition)
        {
            throw new NotImplementedException();
        }

        [Builtin("simple-condition-format-arguments")]
        public static object SimpleConditionFormatArgumnets(object simple_sondition)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Warn(object datum, params object[] arguments)
        {
            (DefinedSymbols._Debug_Io_.Value as ICharacterOutputStream).WriteLine(string.Format(datum.ToString(), arguments));
            return DefinedSymbols.NIL;
        }

        [Builtin("invoke-debugger")]
        public static object InvokeDebugger(object condition)
        {
            LispCondition cond = condition as LispCondition;

            if (cond == null)
                TypeError("INVOKE-DEBUGGER: first argument is not a condition");

            IBidirectionalStream debug_io = DefinedSymbols._Debug_Io_.Value as IBidirectionalStream;

            if (debug_io == null)
                TypeError("INVOKE-DEBUGGER: *debug-io* value is not a condition");

            debug_io.WriteLine(cond.ToString());

            List<Restart> restarts = HRManager.FindRestarts(cond);

            PrintRestarts(restarts);

        restart_loop:
            int chi = debug_io.Read();
            if (chi != -1)
            {
                char ch = (char)chi;
                int restartnum;

                if (!Int32.TryParse(ch.ToString(), out restartnum))
                {
                    goto restart_loop;
                }

                if (restarts.Count <= restartnum)
                {
                    goto restart_loop;
                }

                return restarts[restartnum].Invoke(condition);
            }


            return DefinedSymbols.NIL;
        }

        private static void PrintRestarts(List<Restart> restarts)
        {
            IBidirectionalStream debug_io = DefinedSymbols._Debug_Io_.Value as IBidirectionalStream;
            debug_io.WriteLine("\r\n");
            for (int i = 0; i < restarts.Count; i++)
            {
                debug_io.Write(" " + i + " ");
                restarts[i].Report.VoidInvoke(debug_io);
                debug_io.WriteLine("");
            }
        }

        [Builtin]
        public static void Break([Optional] object format_control, params object[] format_arguments)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-condition")]
        public static void MakeCondition(object condition_type, [Rest] object args)
        {
            throw new NotImplementedException();
        }

        [Builtin("compute-restarts")]
        public static object ComputeRestarts([Optional] object condition)
        {
            throw new NotImplementedException();
        }

        [Builtin("find-restart")]
        public static object FindRestart(object identifier, [Optional] object condition)
        {
            throw new NotImplementedException();
        }

        [Builtin("invoke-restart", ValuesReturnPolitics = ValuesReturnPolitics.Sometimes)]
        public static object InvokeRestart(object restart, params object[] arguments)
        {
            throw new NotFiniteNumberException();
        }

        [Builtin("INVOKE-RESTART-INTERACTIVELY", ValuesReturnPolitics = ValuesReturnPolitics.Sometimes)]
        public static object InvokeRestartInteractively(object restart)
        {
            throw new NotImplementedException();
        }

        [Builtin("restart-name")]
        public static object RestartName(object restart)
        {
            throw new NotImplementedException();
        }

        /*
            abort &optional condition =>|
            continue &optional condition => nil
            muffle-warning &optional condition =>|
            store-value value &optional condition => nil
            use-value value &optional condition => nil
         */

        [Builtin]
        public static object Abort([Optional] object condition)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Continue([Optional] object condition)
        {
            throw new NotImplementedException();
        }

        [Builtin("muffle-warning")]
        public static object MuffleWarning([Optional] object condition)
        {
            throw new NotImplementedException();
        }

        [Builtin("store-value")]
        public static object StoreValue([Optional] object condition)
        {
            throw new NotImplementedException();
        }

        [Builtin("use-value")]
        public static object UseValue([Optional] object condition)
        {
            throw new NotImplementedException();
        }

        internal static object NotAFunctionName(object arg1)
        {
            throw new NotImplementedException();
        }

        internal static object SyntaxError(string p)
        {
           throw new NotImplementedException();
        }

        internal static object TypeError(string p)
        {
            throw new NotImplementedException();
        }

        internal static object UnboundVariable(Symbol symbol)
        {
            return Error(new UnboundVariable(symbol));
        }
    }
}

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol _Debugger_Hook_;
        public static Symbol _Break_On_Signals_;
        public static Symbol TypeError;
    }
}