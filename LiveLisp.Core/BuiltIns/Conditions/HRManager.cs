using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.CLOS;
using LiveLisp.Core.Types;
using LiveLisp.Core.BuiltIns.TypesAndClasses;

namespace LiveLisp.Core.BuiltIns.Conditions
{

    public class ConditionHandler
    {
        TypeName type;

        public TypeName Type
        {
            get { return type; }
            set { type = value; }
        }
        LispFunction handler;

        public LispFunction Handler
        {
            get { return handler; }
            set { handler = value; }
        }

        public ConditionHandler(TypeName type, LispFunction handler)
        {
            this.type = type;
            this.handler = handler;
        }

        internal bool Is(CLOSClass Class)
        {
            return type.Match(Class);
        }

        public override string ToString()
        {
            return "<Condition Hanlder: condition type - " + type.ToString() + " handler " + handler;
        }
    }

  // defaults for report and interactive
   public class DefaultInteractive : SingleFunction
   {

       public DefaultInteractive()
           :base("DEFAULT_INTERACTIVE"){}
       public override object  ValuesInvoke(object[] args)
{
 	 throw new Exception();
}
   }

     public class DefaultTest : SingleFunction
   {

       public DefaultTest()
           :base("DEFAULT_TEST"){}
       public override object  ValuesInvoke(object[] args)
{
 	 return DefinedSymbols.T;
}
     }

     public class DelegateWrapper : SingleFunction
     {
         Delegate _del;
         public DelegateWrapper(Delegate del)
             :base("DELEGATE_WRAPPER " + del.Method.Name)
         {
             _del = del;
         }

         public override object ValuesInvoke(object[] args)
         {
             return _del.DynamicInvoke(args);
         }
     }

     public class DefaultREPORT : SingleFunction
     {
         string _message;
         public DefaultREPORT(string message)
             : base("DEFAULT_REPORT") { _message = message; }

         public override object ValuesInvoke(dynamic[] args)
         {
             args[0].Write(_message);
             return DefinedSymbols.NIL;
         }
     }
    
    [CLOSBuiltinClass(ClassName="COMMON-LISP:RESTART")]
    public class Restart : SingleFunction
    {

        public Restart(Symbol Name, LispFunction func)
            :base("RESTART: " + Name.ToString())
        {
            this.func = func;
            this.Name = Name;
            Report = new DefaultREPORT(Name.ToString(false));
            Interactive = func;
            Test = new DefaultTest();
        }

        public Restart (Delegate del, string message)
            : base("RESTART: " + del.Method.Name)
        {
            this.func = new DelegateWrapper(del);
            this.Name = DefinedSymbols.NIL;
            Report = new DefaultREPORT(message);
            Interactive = func;
            Test = new DefaultTest();
        }

        public Restart(Delegate interactive, Delegate test, string message)
            : base("RESTART: " + interactive.Method.Name)
        {
            this.func = new DelegateWrapper(interactive);
            this.Name = DefinedSymbols.NIL;
            Report = new DefaultREPORT(message);
            Interactive = new DelegateWrapper(interactive);
            Test = new DelegateWrapper(test);
        }

        public Restart(Symbol Name, LispFunction func, LispFunction report, LispFunction interactive, LispFunction test)
            :base("RESTART: " + Name.ToString())
        {
            this.func = func;
            this.Name = Name;
            this.Report = report;
            this.Interactive = interactive;
            this.Test = test;
        }

        private LispFunction func;

        public override object ValuesInvoke(object[] args)
        {
            return func.ValuesInvoke(args);
        }

        public LispFunction Report { get; set; }

        public LispFunction Interactive { get; set; }

        public LispFunction Test {get; set;}
    }

    public class HRManager
    {
        [ThreadStatic]
        static LinkedList<ConditionHandler> handlers;
        public static LinkedList<ConditionHandler> ActiveHandlers
        {
            get
            {
                if (handlers == null)
                    handlers = new LinkedList<ConditionHandler>();

                return handlers;
            }
        }
        public static void AddHandler(ConditionHandler handler)
        {
            handlers.AddFirst(handler);
        }

        /// <summary>
        /// берёт первый хэндлер для такого типа (is)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetHandler(CLOSClass type)
        {
            var node = handlers.First;

            while (node != null)
            {
                if (node.Value.Is(type))
                    return node.Value;
            }

            return DefinedSymbols.NIL;
        }

        public void RemoveHandler(ConditionHandler handler)
        {
            var node = handlers.First;

            while (node != null)
            {
                if (node.Value == handler)
                {
                    if (node.Previous == null)
                        handlers.RemoveFirst();

                    else if (node.Next == null)
                        handlers.RemoveLast();

                    else
                        handlers.Remove(node);

                    return;
                }
            }

            throw new ArgumentException("RemoveHandler: unknown exception handler " + handler);
        }

        internal static bool FindHandler(object condition, out ConditionHandler handler)
        {
            LinkedListNode<ConditionHandler> first = ActiveHandlers.First;

            while (first != null)
            {
                if (first.Value.Is(condition.GetCLOSClass())
            )
                {
                    handler = first.Value;
                    return true;
                }
            }

            handler = null;
            return false;
        }

        [ThreadStatic]
        static List<Restart> restarts;

        internal static List<Restart> FindRestarts(LispCondition cond)
        {
            
            var ret = new List<Restart>();
            if (restarts != null)
            {
                for (int i = restarts.Count - 1; i >= 0; i--)
                {
                    if (restarts[i].Test.Invoke(cond) == DefinedSymbols.T)
                    {
                        ret.Add(restarts[i]);
                    }
                }
            }

            return ret;
        }

        public static void AddRestart(Restart restart)
        {
            if (restarts == null)
            {
                restarts = new List<Restart>();
            }
            
            restarts.Add(restart);
        }
    }
}
