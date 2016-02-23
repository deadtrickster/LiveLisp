using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LiveLisp.Core.AST.Expressions;
using System.Collections.ObjectModel;
using System.Reflection.Emit;

namespace LiveLisp.Core.AST
{
    public class ExpressionContext
    {
        private ExpressionContext _parent;

        public ExpressionContext Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public Expression Expression;

        #region CustomProperties
        Dictionary<object, object> props = new Dictionary<object, object>();

        public object this[object name]
        {
            get
            {
                if (!props.ContainsKey(name))
                    return DefinedSymbols.NIL;

                else return props[name];
            }
            set
            {
                if (props.ContainsKey(name))
                    props[name] = value;

                else
                    props.Add(name, value);
            }
        }
        #endregion

        public ExpressionContext(ExpressionContext parent)
        {
            _parent = parent;
        }

        public ExpressionContext()
        {
            _parent = null;
        }

        public static ExpressionContext Root = new ExpressionContext();

        Dictionary<Symbol, LispFunction> macros = new Dictionary<Symbol, LispFunction>();

        internal bool GetMacro(Symbol car, out LispFunction macrofunc)
        {
            if (macros.ContainsKey(car))
            {
                macrofunc = macros[car];
                return true;
            }

            if (_parent != null)
                return _parent.GetMacro(car, out macrofunc);
            else if (car.Macro != null)
            {
                macrofunc = car.Macro;
                return true;
            }
            else
            {
                macrofunc = null;
                return false;
            }
        }

        protected bool? _SaveValues;
        protected bool? _NonVoidReturn;

        public bool SaveValues
        {
            get
            {
                if (_SaveValues.HasValue)
                    return _SaveValues.Value;
                else if (_parent != null)
                    return _parent.SaveValues;
                else
                    return false;
            }
            set
            {
                _SaveValues = value;
            }
        }

        public bool NonVoidReturn
        {
            get
            {
                if (_NonVoidReturn.HasValue)
                    return _NonVoidReturn.Value;
                else if (_parent != null)
                    return _parent.NonVoidReturn;
                else
                    return false;
            }
            set
            {
                _NonVoidReturn = value;
            }
        }

        List<Symbol> specials = new List<Symbol>();

        internal bool IsSpecial(Symbol symbol)
        {
            if (specials.Contains(symbol))
                return true;
            else
                return symbol.IsDynamic;
        }

        internal void SetSpecial(Symbol symbol)
        {
            if (!specials.Contains(symbol))
                specials.Add(symbol);
        }

        Dictionary<Symbol, Slot> var_slots = new Dictionary<Symbol, Slot>();

        internal Slot GetSlot(Symbol symbol)
        {
            if (var_slots.ContainsKey(symbol))
                return var_slots[symbol];
            else if (Parent != null)
                return (Parent as CompilationExpressionContext).GetSlot(symbol);
            else return null;
        }

        internal Slot AddSlot(Symbol symbol, Slot Slot)
        {
            var_slots.Add(symbol, Slot);
            return Slot;
        }
    }

    public class CompilationExpressionContext : ExpressionContext, IDeclarationsClient
    {
        public CompilationExpressionContext(CompilationExpressionContext parent, CompilationContext ccontext)
            : base(parent)
        {
            this.ccontext = ccontext;
        }

        public CompilationExpressionContext(CompilationExpressionContext parent)
            : base(parent)
        {
            this.ccontext = parent.ccontext;
        }

        public CompilationExpressionContext(CompilationContext ccontext)
        {
            this.ccontext = ccontext;
        }

        CompilationContext ccontext = null;
        public CompilationContext CompilationContext
        {
            get { return ccontext; }
        }

        InstructionsBlock _InstructionsBlock;
        public InstructionsBlock InstructionsBlock
        {
            get
            {
                if (_InstructionsBlock != null)
                    return _InstructionsBlock;
                else if (Parent != null)
                    return (Parent as CompilationExpressionContext).InstructionsBlock;
                else
                    return ccontext.MainInstructionsBlock;
            }
            set { _InstructionsBlock = value; }
        }

        public bool NoBoxing;

        Dictionary<Symbol, BlockExitPoint> BlocksExits = new Dictionary<Symbol, BlockExitPoint>();

        internal void AddBlockExitPoint(Symbol symbol, BlockExitPoint exit_point)
        {
            BlocksExits.Add(symbol, exit_point);
        }

        internal BlockExitPoint GetBlockExitPoint(Symbol symbol)
        {
            if (BlocksExits.ContainsKey(symbol))
                return BlocksExits[symbol];
            else if (Parent != null)
            {
                return (Parent as CompilationExpressionContext).GetBlockExitPoint(symbol);
            }
            else
                return null;
        }

        Dictionary<object, TagbodyExitPoint> tags = new Dictionary<object, TagbodyExitPoint>();

        internal void AddTag(object tag, TagbodyExitPoint tagbodyExitPoint)
        {
            tags.Add(tag, tagbodyExitPoint);
        }

        internal TagbodyExitPoint GetTagExitPoint(object tag)
        {
            if (tags.ContainsKey(tag))
                return tags[tag];
            else if (Parent != null)
            {
                return (Parent as CompilationExpressionContext).GetTagExitPoint(tag);
            }
            else
                return null;
        }

        Dictionary<Symbol, Slot> func_slots = new Dictionary<Symbol, Slot>();

        internal Slot AddLocalFunction(Symbol symbol, Slot function)
        {
            func_slots.Add(symbol, function);
            return function;
        }

        internal Slot GetLocalFunction(Symbol symbol)
        {
            if (func_slots.ContainsKey(symbol))
                return func_slots[symbol];
            else if (Parent != null)
                return (Parent as CompilationExpressionContext).GetLocalFunction(symbol);
            else return null;
        }

        ConstantsManager _cmanager;

        internal ConstantsManager ConstantsManager
        {
            get
            {
                if (_cmanager != null)
                    return _cmanager;
                else if (Parent == null)
                    return null;
                else return (Parent as CompilationExpressionContext).ConstantsManager;
            }
            set
            {
                _cmanager = value;
            }
        }

        Dictionary<LambdaFunctionDesignator, StaticTypeResolver> lambdas;
        internal bool ContainsLambda(LambdaFunctionDesignator lambdaFunctionDesignator)
        {
            if (Parent != null)
                return (Parent as CompilationExpressionContext).ContainsLambda(lambdaFunctionDesignator);

            if (lambdas == null)
            {
                lambdas = new Dictionary<LambdaFunctionDesignator, StaticTypeResolver>();
                return false;
            }

            return lambdas.ContainsKey(lambdaFunctionDesignator);

        }

        internal StaticTypeResolver GetLambdaType(LambdaFunctionDesignator lambdaFunctionDesignator)
        {
            if (Parent != null)
                return (Parent as CompilationExpressionContext).GetLambdaType(lambdaFunctionDesignator);

            if (lambdas == null)
            {
                lambdas = new Dictionary<LambdaFunctionDesignator, StaticTypeResolver>();
                throw new InvalidOperationException();
            }

            return lambdas[lambdaFunctionDesignator];
        }

        internal void AddLambdaType(LambdaFunctionDesignator lambdaFunctionDesignator, StaticTypeResolver resolver)
        {
            if (Parent != null)
                (Parent as CompilationExpressionContext).AddLambdaType(lambdaFunctionDesignator, resolver);

            if (lambdas == null)
            {
                lambdas = new Dictionary<LambdaFunctionDesignator, StaticTypeResolver>();
            }

            lambdas.Add(lambdaFunctionDesignator, resolver);
        }

        ClosuresManager _closuresManager;

        internal ClosuresManager ClosuresManager
        {
            get
            {
                if (_closuresManager != null)
                    return _closuresManager;
                else if (Parent != null)
                    return (Parent as CompilationExpressionContext).ClosuresManager;
                else return null;
            }
            set { _closuresManager = value; }
        }

        internal ClosureClientContext GetClosureClientContext(IBarrier lambda)
        {
            var cmanager = ClosuresManager;
            if (cmanager == null) return null;
            else if (cmanager.Clients.Contains(lambda))
                return cmanager.Clients[lambda];
            else return null;
        }

        Dictionary<ClosureContainerContext, Slot> closureContainerSlots = new Dictionary<ClosureContainerContext, Slot>();

        internal Slot GetSlotForClosureContainer(ClosureContainerContext closureContainerContext)
        {
            if (closureContainerSlots.ContainsKey(closureContainerContext))
                return closureContainerSlots[closureContainerContext];
            else if (Parent != null)
                return (Parent as CompilationExpressionContext).GetSlotForClosureContainer(closureContainerContext);
            else return null;
        }

        internal void SetSlotForClosureContainer(ClosureContainerContext closureContainerContext, Slot fieldSlot)
        {
            closureContainerSlots.Add(closureContainerContext, fieldSlot);
        }

        #region IDeclarationsClient Members

        public new void SetSpecial(Symbol symbol)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

