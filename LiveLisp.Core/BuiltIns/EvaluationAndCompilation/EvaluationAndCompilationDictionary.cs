using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using LiveLisp.Core.AST;
using LiveLisp.Core.Eval;
using LiveLisp.Core.BuiltIns.Conditions;
using LiveLisp.Core.Interpreter;

namespace LiveLisp.Core.BuiltIns.EvaluationAndCompilation
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class EvaluationAndCompilationDictionary
    {
        [Builtin]
        public static object Compile(object name, [Optional] object definition)
        {
            throw new NotImplementedException();
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.Sometimes)]
        public static object Eval(object form)
        {
            Expression exp = LispParser.ParseExpression(form, ExpressionContext.Root);

          //  Expression rewrited = DefaultRewriter.Rewrite(exp);
            if (Settings.UseCompilerInREPL)
            {
                return new DefaultASTCompiler().Compile(exp).DynamicInvoke();
            }
            else
            {
                return new Interpreter.Interpreter().Eval(exp);
            }
        }

        
        [Builtin("macro-function")]
        public static object MacroFunction(object symbol, [Optional] object env)
        {
            Symbol sym = symbol as Symbol;

            if (sym == null)
                ConditionsDictionary.TypeError("MACRO-FUNCTION: first argument is not a symbol " + symbol);

            if (env != DefinedSymbols.NIL)
                ConditionsDictionary.Error("MACRO-FUNCTION: only the global environment currently supported");

            if (sym.Macro == null)
                return DefinedSymbols.NIL;
            else
                return sym.Macro;
        }

        [Builtin("SYSTEM::%set-macro-function", OverridePackage=true)]
        public static object SetMacroFunction(object symbol, object macro, [Optional] object env)
        {
            Symbol sym = symbol as Symbol;

            if (sym == null)
                ConditionsDictionary.TypeError("(SETF MACRO-FUNCTION): first argument is not a symbol " + symbol);

            LispFunction func = macro as LispFunction;
            if(func == null)
                ConditionsDictionary.TypeError("(SETF MACRO-FUNCTION): second argument is not a function " + macro);

            if (env == DefinedSymbols.NIL)
                ConditionsDictionary.Error("(SETF MACRO-FUNCTION): only the global environment currently supported");

            sym.Macro = func;
            return func;
        }

        [Builtin(ValuesReturnPolitics=ValuesReturnPolitics.AlwaysNonZero)]
        public static object Macroexpand(object form, [Optional] object env)
        {
            Cons cons = form as Cons;
            if (cons == null)
                return form;

            Symbol symb = cons.Car as Symbol;
            if (symb == null)
                return form;

            if (symb.Macro != null)
            {
                LispParser.TryMacroExpansion(symb, ExpressionContext.Root, cons.Child, out form);
            }
           
            return form;
        }

        [Builtin("macroexpand-1", ValuesReturnPolitics = ValuesReturnPolitics.AlwaysNonZero)]
        public static object Macroexpnad1(object form, [Optional]object env)
        {
            throw new NotImplementedException();
        }

        [Builtin(ValuesReturnPolitics = ValuesReturnPolitics.Void, VoidReturn = "T")]
        public static void Proclaim(object declaration_specifier)
        {
            if (declaration_specifier == DefinedSymbols.NIL)
                return;

            Cons declaration = declaration_specifier as Cons;
            if (declaration == null)
                ConditionsDictionary.TypeError("PROCLAIM: argument 1 is not a list: " + declaration_specifier);

            Declaration decl = LispParser.ParseDeclaration(declaration);
            if (decl == null)
                return;

            if(!decl.ValidForProclaim)
                ConditionsDictionary.Error("PROCLAIM: dclaration " + decl.ToString() + " can not be used with proclaim");

            decl.ApplyForProclaim();
        }

        [Builtin("special-operator-p", Predicate = true)]
        public static object SpecialOperatorP(object symbol)
        {
            Symbol sym = symbol as Symbol;

            if(sym == null)
                ConditionsDictionary.TypeError("SPECIAL-OPERATOR-P: argument 1 is not a symbol: " + symbol);

            if (sym.Id < 31)
                return DefinedSymbols.T;
            else
                return DefinedSymbols.NIL;
        }

        [Builtin(Predicate = true)]
        public static object Constantp(object form, [Optional] object env)
        {
            Cons cform = form as Cons;

            if (cform == null)
            {
                Symbol s = form as Symbol;
                if (s == null)
                    return DefinedSymbols.T;
                else
                {
                    EvaluationContext ctx = env as EvaluationContext;
                    if (ctx == null)
                        ConditionsDictionary.TypeError("CONSTANTP: argument 2 is not an environment");

                    return ctx.IsConstant(s) ? DefinedSymbols.T : DefinedSymbols.NIL;
                }
            }
            else if (cform.Car == DefinedSymbols.Quote)
                return DefinedSymbols.T;
            else return
                DefinedSymbols.NIL; 
        }

        public class DefaultMacroExpandHook : FullLispFunction
        {
            public DefaultMacroExpandHook()
                : base("%default-macroexpand-hook")
            {

            }

            public override object Invoke(object arg1, object arg2, object arg3)
            {
                Start:
                LispFunction macroFunction = arg1 as LispFunction;
                if (macroFunction == null)
                {
                    Symbol funcName = arg1 as Symbol;

                    if (funcName == null)
                    {
                        arg1 = ConditionsDictionary.NotAFunctionName(arg1);
                        goto Start;
                    }

                    macroFunction = DefinedSymbols.SymbolFunction.Invoke(funcName) as LispFunction;
                }

                return macroFunction.Invoke(arg2, arg1);
            }
        }
    }
}
namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol _Macroexpand_Hook_;

        public static Symbol Declare;
    }

    public partial class Initialization
    {
        public static void InitEvalAndCompile()
        {
            DefinedSymbols._Macroexpand_Hook_ = CommonLispPackage.Intern("*MACROEXPAND-HOOK*", true);
            DefinedSymbols._Macroexpand_Hook_.IsDynamic = true;

        }
    }
}