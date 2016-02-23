using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LiveLisp.Core.Reader;
using LiveLisp.Core.Types.Streams;
using LiveLisp.Core.Compiler;
using LiveLisp.Core;
using System.Reflection;
using LiveLisp.Core.BuiltIns.Arrays;
using LiveLisp.Core.BuiltIns.Characters;
using LiveLisp.Core.Types;
using LiveLisp.Core.BuiltIns.Numbers;
using System.Threading;
using System.Collections;
using LiveLisp.Core.CLOS;
using ilasmtest;
using System.Diagnostics;
using LiveLisp.Core.BuiltIns.Conditions;
using System.Windows.Forms;
using LiveLisp.Core.CLOS.Standard_classes;

namespace LiveLisp
{
    delegate void SampleDelegate();

    public delegate Predicate<int> MyPredicate<T>(T obj);
    
    public delegate object SimpleRestart(dynamic cond);
    public delegate object RestartDel(dynamic cond);

    class Program
    {
        public static object Mod(dynamic number, dynamic divisor)
        {
            return number % divisor;
        }

        static void Main(string[] args)
        {
            LiveLisp.Core.Initialization.Initialize();

            Guid g = Guid.NewGuid();

            CharacterInputStream inputStream = new CharacterInputStream(Console.In);
            DefinedSymbols._Standard_Input_.Value = inputStream;
            CharacterOutputStream outputStream = new CharacterOutputStream(Console.Out);
            DefinedSymbols._Standard_Output_.Value = outputStream;
            DefinedSymbols._Debug_Io_.Value = new BidirectionalStream(inputStream, outputStream);

            var stream = new CharacterInputStream(Console.In);


            HRManager.AddRestart(new Restart((RestartDel)delegate(dynamic con)
            {
                throw new BlockNonLocalTransfer(g, DefinedSymbols.T);
            }, "Return to top level"));

            HRManager.AddRestart(new Restart((RestartDel)delegate(dynamic con)
            {
                DefinedSymbols._Debug_Io_.Value.WriteLine("You may input a new value for " + con.CellName);
                object form = DefinedSymbols.Read.Invoke(stream);
                object result = DefinedSymbols.Eval.Invoke(form);
                con.CellName.Value = result;
                return result;
            }, (RestartDel)delegate(dynamic con)
            {
                if (con is UnboundVariable)
                    return DefinedSymbols.T;
                else return DefinedSymbols.NIL;
            }, "Enter new value "));

            HRManager.AddRestart(new Restart((RestartDel)delegate(dynamic con)
            {
                DefinedSymbols._Debug_Io_.Value.WriteLine("You may input a value to be used instead of " + con.CellName);
                object form = DefinedSymbols.Read.Invoke(stream);
                object result = DefinedSymbols.Eval.Invoke(form);
                return result;
            }, (RestartDel)delegate(dynamic con)
            {
                if (con is UnboundVariable)
                    return DefinedSymbols.T;
                else return DefinedSymbols.NIL;
            }, "Use instead of "));


            var load_form = DefinedSymbols.Read.Invoke(new CharacterInputStream(new StringReader("(load \"load.llisp\")")));
            DefinedSymbols.Eval.Invoke(load_form);
            while (true)
            {
                try
                {
                    Console.Write(">>>> ");
                    object form = DefinedSymbols.Read.Invoke(stream);
                    object result = DefinedSymbols.Eval.Invoke(form);
                    DefinedSymbols.Print.Invoke(result);
                }
                catch (TargetInvocationException e)
                {
                    while (e.InnerException is TargetInvocationException)
                    {
                        e = e.InnerException as TargetInvocationException;
                    }

                    BlockNonLocalTransfer bt = e.InnerException as BlockNonLocalTransfer;
                    if (bt != null && bt.TagId == g)
                    {
                        continue;
                    }
                    else
                    {
                        DefinedSymbols.Print.Invoke(e.InnerException);
                    }
                }
                catch (Exception e)
                {
                    DefinedSymbols.Print.Invoke(e.Message);
                }
            }

        }
    }
}