using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST;
using LiveLisp.Core.AST.Expressions;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using LiveLisp.Core.Reader;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.BuiltIns.Numbers;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.Interpreter
{
    public class EvaluationContext : IDeclarationsClient
    {
        EvaluationContext parent;

        public EvaluationContext(EvaluationContext parent)
        {
            this.parent = parent;
            localFunctions = null;
        }

        Dictionary<Symbol, LispFunction> localFunctions;

        public LispFunction GetFunction(Symbol symbol)
        {
            if (localFunctions != null && localFunctions.ContainsKey(symbol))
                return localFunctions[symbol];
            else if (parent != null)
                return parent.GetFunction(symbol);
            else
                return symbol.Function;
        }

        internal void AddFunction(Symbol symbol, LispFunction function)
        {
            if (localFunctions == null)
                localFunctions = new Dictionary<Symbol, LispFunction>();

            localFunctions.Add(symbol, function);
        }

        private bool? _SaveValues;

        public bool SaveValues
        {
            get
            {
                if (_SaveValues.HasValue)
                    return _SaveValues.Value;
                else if (parent != null)
                    return parent.SaveValues;
                else return false;
            }
            set { _SaveValues = value; }
        }


        Dictionary<int, object> local_bindings;

        public void Bind(Symbol symbol, object value)
        {
            if (IsConstant(symbol))
                ConditionsDictionary.Error(symbol + " is a constant and cannot be set or bound.");
            if (local_bindings == null)
                local_bindings = new Dictionary<int, object>();

            if (local_bindings.ContainsKey(symbol.Id))
                throw new InvalidOperationException("Cannot rebind variable in the same environment, try Assign");

            local_bindings.Add(symbol.Id, value);
        }

        public object GetValue(Symbol symbol)
        {
            if (IsSpecial(symbol))
                return symbol.Value;

            return GetValueFromSyntaxBinding(symbol);
        }

        private object GetValueFromSyntaxBinding(Symbol symbol)
        {
            if (local_bindings != null && local_bindings.ContainsKey(symbol.Id))
                return local_bindings[symbol.Id];
            else if (parent != null)
                return parent.GetValue(symbol);
            else
                return symbol.Value;
        }

        internal void Assign(Symbol symbol, object value)
        {
            if (IsConstant(symbol))
                ConditionsDictionary.Error(symbol + " is a constant and cannot be set or bound.");
            if (IsSpecial(symbol))
            {
                symbol.Value = value;
                return;
            }
            AssignToSyntaxBinding(symbol, value);
        }

        private void AssignToSyntaxBinding(Symbol symbol, object value)
        {
            if (local_bindings != null && local_bindings.ContainsKey(symbol.Id))
                local_bindings[symbol.Id] = value;
            else if (parent != null)
                parent.Assign(symbol, value);
            else
                symbol.Value = value;
        }

        BlockExitPoint _blockExitPoint;

        internal BlockExitPoint BlockExitPoint
        {
            get
            {
                if (_blockExitPoint != null)
                {
                    return _blockExitPoint;
                }
                else if (parent != null)
                    return parent.BlockExitPoint;
                else return null;
            }
            set
            {
                _blockExitPoint = value;
            }
        }

        internal BlockExitPoint FindBlock(Symbol name)
        {
            return _findBlockWorker(this, name);
        }

        private BlockExitPoint _findBlockWorker(EvaluationContext evaluationContext, Symbol name)
        {
            if (evaluationContext._blockExitPoint != null && evaluationContext._blockExitPoint.Name == name)
                return evaluationContext._blockExitPoint;
            else if (evaluationContext.parent != null)
                return _findBlockWorker(evaluationContext.parent, name);
            else
                return null;
        }

        List<int> specials;

        internal bool IsSpecial(Symbol symbol)
        {
            if (specials != null && specials.Contains(symbol.Id))
                return true;
            else if (parent != null)
                return parent.IsSpecial(symbol);
            else
                return symbol.IsDynamic;
        }

        public void SetSpecial(Symbol symbol)
        {
            if (specials == null)
                specials = new List<int>();
            specials.Add(symbol.Id);
        }

        Dictionary<object, TagbodyExitPoint> tags;

        internal void AddTag(object tag, TagbodyExitPoint exitPoint)
        {
            if (tags == null)
                tags = new Dictionary<object, TagbodyExitPoint>();

            tags.Add(tag, exitPoint);
        }

        internal TagbodyExitPoint GetTag(object tag)
        {
            if (tags != null && tags.ContainsKey(tag))
                return tags[tag];
            else if (parent != null)
                return parent.GetTag(tag);
            else
                return null;
        }
        List<int> constants;

        internal bool IsConstant(Symbol symbol)
        {
            if (constants != null && constants.Contains(symbol.Id))
                return true;
            else if (parent != null)
                return parent.IsConstant(symbol);
            else
                return symbol.IsConstant;
        }

        internal void AddConstant(Symbol symbol)
        {
            if (constants == null)
                constants = new List<int>();
            if (!constants.Contains(symbol.Id))
                constants.Add(symbol.Id);
        }
    }

    class Interpreter : IEvalWalker
    {
        [ThreadStatic]
        static List<Symbol> tracedFunctions;

        public static void AddFunctionsToTrace(IEnumerable<Symbol> symbols)
        {
            if (tracedFunctions == null)
                tracedFunctions = new List<Symbol>();
            foreach (var item in symbols)
            {
                if (!tracedFunctions.Contains(item))
                    tracedFunctions.Add(item);
            }
        }

        public static void Untrace(IEnumerable<Symbol> symbols)
        {
            if (tracedFunctions == null)
                return;

            foreach (var item in symbols)
            {
                tracedFunctions.Remove(item);
            }
        }

        [ThreadStatic]
        static int callLevel;

        public object Eval(Expression exp)
        {
            callLevel = 0;
            EvaluationContext ctx = new EvaluationContext(null);
            ctx.SaveValues = true;
            return exp.Eval(this, ctx);
        }

        private void ApplyDeclarations(IDeclarationsContainer declarationsContainer, EvaluationContext cec)
        {
            for (int i = 0; i < declarationsContainer.Declarations.Count; i++)
            {
                declarationsContainer.Declarations[i].ApplyToEnvironment(cec);
            }
        }

        #region IEvalWalker Members

        public object ConstantExpression(ConstantExpression constantExpression, EvaluationContext context)
        {
            return constantExpression.Constant;
        }

        public object CallExpression(CallExpression callExpression, EvaluationContext context)
        {
            bool values_backup = context.SaveValues;
            

            bool tracedcall = false;

            if (tracedFunctions != null && callExpression.Function.DesignatorType == FunctionNameDesignatorType.Symbol && (tracedFunctions.Contains((callExpression.Function as SymbolFunctionDesignator).Name)))
            {
                tracedcall = true;
                callLevel++;
            }

            var func = callExpression.Function.Eval(this, context) as LispFunction;

            if (func == null)
                throw new TypeErrorException(func + " is not a function");


            if (tracedcall)
                RuntimeHelpers.PrintTraceEnter(callExpression.Function as SymbolFunctionDesignator, callLevel);

            object[] args = new object[callExpression.Parameters.Count];
            context.SaveValues = false;
            for (int i = 0; i < callExpression.Parameters.Count; i++)
            {
                args[i] = callExpression.Parameters[i].Eval(this, context);
            }
            

            context.SaveValues = values_backup;
            object ret;
            if (values_backup)
            {
                ret = func.ValuesInvoke(args);
            }
            else
            {
                ret = func.Invoke(args);
            }

            if (tracedcall)
            {
                RuntimeHelpers.PrintTraceExit(callExpression.Function as SymbolFunctionDesignator, args, ret, callLevel);

                callLevel--;
            }
            return ret;
        }

        public object SymbolFunctionDesignator(SymbolFunctionDesignator symbolFunctionDesignator, EvaluationContext context)
        {
            return context.GetFunction(symbolFunctionDesignator.Name);
        }

        public object VariableExpression(VariableExpression variableExpression, EvaluationContext context)
        {
            return context.GetValue(variableExpression.VariableName);
        }

        public object SetqExpression(SetqExpression setqExpression, EvaluationContext context)
        {
            // avoid multiple values saving
            var new_ctx = new EvaluationContext(context);


            object ret = UnboundValue.Unbound;
            for (int i = 0; i < setqExpression.Assings.Count; i++)
            {
                ret = setqExpression.Assings[i].Value.Eval(this, new_ctx);
                context.Assign(setqExpression.Assings[i].Symbol, ret);
            }

            if (ret == UnboundValue.Unbound)
                return DefinedSymbols.NIL;
            else
                return ret;
        }

        public object LambdaFunctionDesignator(LambdaFunctionDesignator lambdaFunctionDesignator, EvaluationContext context)
        {

            EvaluationContext new_ctx = context as EvaluationContext;
            ApplyDeclarations(lambdaFunctionDesignator, new_ctx);
            return new InterpretedLispFunction(lambdaFunctionDesignator.Name, lambdaFunctionDesignator.LambdaList, new PrognExpression(lambdaFunctionDesignator.Forms, lambdaFunctionDesignator.Context), new_ctx);
        }

        public object PrognExpression(PrognExpression prognExpression, EvaluationContext context)
        {
            object ret = UnboundValue.Unbound;

            for (int i = 0; i < prognExpression.Count; i++)
            {
                ret = prognExpression[i].Eval(this, context);
            }

            if (ret == UnboundValue.Unbound)
                return DefinedSymbols.NIL;
            else
                return ret;
        }

        public object BlockExpression(BlockExpression blockExpression, EvaluationContext context)
        {
            var new_ctx = new EvaluationContext(context);

            Guid guid = Guid.NewGuid();
            new_ctx.BlockExitPoint = new BlockExitPoint(blockExpression.BlockName, null, guid, null, true);
            object ret = UnboundValue.Unbound;

            try
            {

                for (int i = 0; i < blockExpression.Count; i++)
                {
                    ret = blockExpression[i].Eval(this, new_ctx);
                }
            }
            catch(BlockNonLocalTransfer rf)
            {
                if (rf.TagId == guid)
                    return rf.Result;
                else
                    throw;
            }

            if (ret == UnboundValue.Unbound)
                return DefinedSymbols.NIL;
            else
                return ret;
        }

        public object ReturnFromExpression(ReturnFromExpression returnFromExpression, EvaluationContext context)
        {
            BlockExitPoint exit_point = context.FindBlock(returnFromExpression.Tag);

            if (exit_point == null)
                throw new SimpleErrorException("Block " + returnFromExpression.Tag + " not found");

            object ret = returnFromExpression.Form.Eval(this, context);

            throw new BlockNonLocalTransfer(exit_point.Id, returnFromExpression.Tag, ret);
        }

        public object LetExpression(LetExpression letExpression, EvaluationContext context)
        {
            Stack<Pair<Symbol, object>> backups = new  Stack<Pair<Symbol, object>>();
            Queue<Pair<Symbol, object>> new_bindings = new Queue<Pair<Symbol, object>>();

            EvaluationContext bindings_context = new EvaluationContext(context);
            bindings_context.SaveValues = false;
            EvaluationContext forms_context = new EvaluationContext(bindings_context);
            forms_context.SaveValues = context.SaveValues;
            try
            {
                for (int i = 0; i < letExpression.NewBindings.Count; i++)
                {
                    SyntaxBinding binding = letExpression.NewBindings[i];
                    object new_val = binding.Value.Eval(this, bindings_context);
                    new_bindings.Enqueue(new Pair<Symbol, object>(binding.Symbol, new_val));
                }

                ApplyDeclarations(letExpression, bindings_context);

                while (new_bindings.Count != 0)
                {
                    var binding = new_bindings.Dequeue();
                    if (bindings_context.IsSpecial(binding.First))
                    {
                        backups.Push(new Pair<Symbol, object>(binding.First, binding.First.RawValue));
                        binding.First.Value = binding.Second;
                    }
                    else
                        bindings_context.Bind(binding.First, binding.Second);
                }

                object ret = UnboundValue.Unbound;
                for (int i = 0; i < letExpression.Count; i++)
                {
                    ret = letExpression[i].Eval(this, forms_context);
                }

                if (ret == UnboundValue.Unbound)
                    return DefinedSymbols.NIL;

                return ret;
            }
            finally
            {
                while (backups.Count != 0)
                {
                    var binding = backups.Pop();
                    binding.First.Value = binding.Second;
                }
            }
        }

        public object TagbodyExpression(TagBodyExpression tagBodyExpression, EvaluationContext context)
        {
            NonLocalHook nlhook = new NonLocalHook(null);
            var tagbodyId = Guid.NewGuid();
            for (int i = 0; i < tagBodyExpression.tags.Count; i++)
            {
                TagbodyExitPoint exitPoint = new TagbodyExitPoint(null, tagbodyId, i,null, tagBodyExpression.tags[i]);
                exitPoint.NonLocalHook = nlhook;
                context.AddTag(tagBodyExpression.tags[i], exitPoint);
            }

            int tag = -1;
        Repeat:
            try
            {
                if (tag == -1)
                {
                    for (int i = 0; i < tagBodyExpression.NontaggedProlog.Count; i++)
                    {
                        tagBodyExpression.NontaggedProlog[i].Eval(this, context);
                    }
                }
                else
                {
                    if (tag >= tagBodyExpression.TaggedStatements.Count)
                        return DefinedSymbols.NIL;

                    var stmts = tagBodyExpression.TaggedStatements[tag];

                    for (int i = 0; i < stmts.Statements.Count; i++)
                    {
                        stmts.Statements[i].Eval(this, context);
                    }
                }

                tag++;
                goto Repeat;
            }
            catch (TagBodyNonLocalTransfer ex)
            {
                if (ex.Id == tagbodyId)
                {

                    if (ex.TagId < tagBodyExpression.tags.Count)
                        tag = ex.TagId;
                    else
                        throw;

                    goto Repeat;
                }
            }

            return DefinedSymbols.NIL;
        }

        public object CatchExpression(CatchExpression catchExpression, EvaluationContext context)
        {
            try
            {
                object ret = UnboundValue.Unbound;

                for (int i = 0; i < catchExpression.Count; i++)
                {
                   ret = catchExpression[i].Eval(this, context);
                }

                if (ret == UnboundValue.Unbound)
                    return DefinedSymbols.NIL;
                return ret;
            }
            catch (CatchThrowException ex)
            {
                if (ex.Tag == catchExpression.Tag)
                {
                    MultipleValuesContainer vals = ex.Result as MultipleValuesContainer;

                    if (!context.SaveValues && vals != null)
                    {
                        return vals.First;
                    }

                    return ex.Result;
                }
                else
                    throw;
            }
        }

        public object EvalWhenExpression(EvalWhenExpression evalWhenExpression, EvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public object FletExpression(FletExpression fletExpression, EvaluationContext context)
        {
            var new_ctx = new EvaluationContext(context);
            var forms_ctx = new EvaluationContext(new_ctx);

            for (int i = 0; i < fletExpression.Lambdas.Count; i++)
            {
                var lambda = fletExpression.Lambdas[i];
                var _function = lambda.Eval(this, context);
                var function = _function as LispFunction;
                if (function == null)
                    throw new SimpleErrorException("FLET: " + _function + " is not a function");
                forms_ctx.AddFunction(fletExpression.Lambdas[i].Name, function);
            }

            if (fletExpression.Count == 0)
                return DefinedSymbols.NIL;


            ApplyDeclarations(fletExpression, forms_ctx);

            object ret = null;

            for (int i = 0; i < fletExpression.Count; i++)
            {
                ret = fletExpression[i].Eval(this, forms_ctx);
            }

            return ret;
        }

        public object GoExpression(GoExpression goExpression, EvaluationContext context)
        {
            TagbodyExitPoint label = context.GetTag(goExpression.Tag);

            if (label == null)
                throw new SimpleErrorException("GO: tag " + goExpression.Tag + " is unknown.");

            throw new TagBodyNonLocalTransfer(label.TagbodyId, goExpression.Tag, label.TagId);
        }

        public object IfExpression(IfExpression ifExpression, EvaluationContext context)
        {
            bool values_backup = context.SaveValues;

            context.SaveValues = false;

            object condition = ifExpression.Condition.Eval(this, context);

            context.SaveValues = values_backup;
            if (condition == DefinedSymbols.NIL || condition == null)
            {
                return ifExpression.Else.Eval(this, context);
            }
            else
            {
                return ifExpression.Then.Eval(this, context);
            }
        }

        public object LabelsExpression(LabelsExpression labelsExpression, EvaluationContext context)
        {
            var forms_ctx = new EvaluationContext(context);

            for (int i = 0; i < labelsExpression.Lambdas.Count; i++)
            {
                var lambda = labelsExpression.Lambdas[i];
                var _function = lambda.Eval(this, context);
                var function = _function as LispFunction;
                if (function == null)
                    throw new SimpleErrorException("LABELS: " + _function + " is not a function");
                forms_ctx.AddFunction(labelsExpression.Lambdas[i].Name, function);
            }

            if (labelsExpression.Count == 0)
                return DefinedSymbols.NIL;


            ApplyDeclarations(labelsExpression, forms_ctx);

            object ret = null;

            for (int i = 0; i < labelsExpression.Count; i++)
            {
                ret = labelsExpression[i].Eval(this, forms_ctx);
            }

            return ret;
        }

        public object LetStarExpression(LetStarExpression letStarExpression, EvaluationContext context)
        {
            Stack<Pair<Symbol, object>> backups = new Stack<Pair<Symbol, object>>();
            Queue<Pair<Symbol, object>> new_bindings = new Queue<Pair<Symbol, object>>();

            EvaluationContext bindings_context = new EvaluationContext(context);
            bindings_context.SaveValues = false;
            EvaluationContext forms_context = new EvaluationContext(bindings_context);
            forms_context.SaveValues = context.SaveValues;
            try
            {
                ApplyDeclarations(letStarExpression, bindings_context);

                for (int i = 0; i < letStarExpression.NewBindings.Count; i++)
                {
                    SyntaxBinding binding = letStarExpression.NewBindings[i];
                    object new_val = binding.Value.Eval(this, bindings_context);

                    if (bindings_context.IsSpecial(binding.Symbol))
                    {
                        backups.Push(new Pair<Symbol, object>(binding.Symbol, binding.Symbol.RawValue));
                        binding.Symbol.Value = new_val;
                    }
                    else
                        bindings_context.Bind(binding.Symbol, new_val);
                }

                object ret = UnboundValue.Unbound;
                for (int i = 0; i < letStarExpression.Count; i++)
                {
                    ret = letStarExpression[i].Eval(this, forms_context);
                }

                if (ret == UnboundValue.Unbound)
                    return DefinedSymbols.NIL;

                return ret;
            }
            finally
            {
                while (backups.Count != 0)
                {
                    var binding = backups.Pop();
                    binding.First.Value = binding.Second;
                }
            }
        }

        public object LoadTimeValue(LoadTimeValue loadTimeValue, EvaluationContext context)
        {
            return loadTimeValue.Eval(this, context);
        }

        public object LocallyExpression(LocallyExpression locallyExpression, EvaluationContext context)
        {
            EvaluationContext ctx = new EvaluationContext(context);

            ApplyDeclarations(locallyExpression, ctx);

            if (locallyExpression.Count == 0)
                return DefinedSymbols.NIL;

            object ret = null;

            for (int i = 0; i < locallyExpression.Count; i++)
            {
                ret = locallyExpression[i].Eval(this, ctx);
            }

            return ret;
        }

        public object MultipleValueCallExpression(MultipleValueCallExpression multipleValueCallExpression, EvaluationContext context)
        {
            object func_ = multipleValueCallExpression.Function.Eval(this, context);
            LispFunction function = func_ as LispFunction;

            if (function == null)
                throw new SimpleErrorException("MULTIPLE-VALUE-CALL: function form is not a function: " + func_);

            EvaluationContext ctx = new EvaluationContext(context);

            ctx.SaveValues = true;

            MultipleValuesContainer values = new MultipleValuesContainer();

            if (multipleValueCallExpression.ValuesProducers.Count != 0)
            {
                for (int i = 0; i < multipleValueCallExpression.ValuesProducers.Count; i++)
                {
                    object ret = multipleValueCallExpression.ValuesProducers[i].Eval(this, ctx);

                    values.Combine(ret);
                }
            }

            if (context.SaveValues)
                return function.ValuesInvoke(values.ToArray());
            else
                return function.Invoke(values.ToArray());
        }

        public object MultipleValueProg1Expression(MultipleValueProg1Expression multipleValueProg1Expression, EvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public object ProgvExpression(ProgvExpression progvExpression, EvaluationContext context)
        {
            EvaluationContext ctx = new EvaluationContext(context);
            ctx.SaveValues = false;

            object sym_list_ = progvExpression.Symbols.Eval(this, ctx);
            Cons sym_list = sym_list_ as Cons;
            if (sym_list == null)
                throw new SimpleErrorException("PROGV: symbols list not found got " + sym_list_);

            Queue<Symbol> symbols = new Queue<Symbol>();

            foreach (var item in sym_list)
            {
                Symbol sym = item as Symbol;
                if (sym == null)
                    throw new SimpleErrorException("PROGV: object in symbols list is not a symbol " + item);

                symbols.Enqueue(sym);
            }

            Stack<Pair<Symbol, object>> backup = new Stack<Pair<Symbol, object>>();
            try
            {
                object val_list_ = progvExpression.Values.Eval(this, ctx);
                Cons values_ = val_list_ as Cons;
                if (values_ == null)
                    throw new SimpleErrorException("PROGV: values list not found");

                while (symbols.Count != 0)
                {
                    Symbol var = symbols.Dequeue();

                    backup.Push(new Pair<Symbol, object>(var, var.RawValue));

                    if (values_ != null)
                        var.RawValue = values_.Car;
                    else
                        var.RawValue = UnboundValue.Unbound;

                    sym_list = sym_list.Child;

                    if (values_ != null)
                        values_ = values_.Child;
                }
                ctx.SaveValues = context.SaveValues;
                object ret = null;

                if (progvExpression.Count == 0)
                    return DefinedSymbols.NIL;

                for (int i = 0; i < progvExpression.Count; i++)
                {
                    ret = progvExpression[i].Eval(this, ctx);
                }

                return ret;
            }
            finally
            {
                while (backup.Count != 0)
                {
                    var pair = backup.Pop();
                    pair.First.RawValue = pair.Second;
                }
            }
        }

        public object SymbolMacroletExpression(SymbolMacroletExpression symbolMacroletExpression, EvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public object TheExpression(TheExpression theExpression, EvaluationContext context)
        {
            object obj = theExpression.Form.Eval(this, context);

            return obj;
        }

        public object ThrowExpression(ThrowExpression throwExpression, EvaluationContext context)
        {
            EvaluationContext ctx = new EvaluationContext(context);
            ctx.SaveValues = true;
            throw new CatchThrowException(throwExpression.CatchTag, throwExpression.ResultForm.Eval(this, ctx));
        }

        public object UnwindProtectExpression(UnwindProtectExpression unwindProtectExpression, EvaluationContext context)
        {
            try
            {
               return unwindProtectExpression.ProtectedForm.Eval(this, context);
            }
            finally
            {
                for (int i = 0; i < unwindProtectExpression.CleanupForms.Count; i++)
                {
                    unwindProtectExpression.CleanupForms[i].Eval(this, context);
                }
            }
        }

        #endregion
    }
}
