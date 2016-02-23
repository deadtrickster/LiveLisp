using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST;
using LiveLisp.Core.Types;
using System.Collections.ObjectModel;

namespace LiveLisp.Core.Compiler
{
    /*
     * этот код используется для отлова лексических замыканий.
     * 
     * каждый раз когда специальная форма создаёт новые связи появляется возможность для лексических замыканий
     * при этом не исключается вложенность следующих видов
     * 
     * 1) форма не содержит дочерних форм вводящих новые связи
     * 2) форма содержит дочерние формы вводящие связи(возможно с лексическми замыканиями) но не
     *      создающие лексических замыканий вокруг связей формы
     * 3) форма содержит дочерние формы вводящие одноимённые связи - тем самым перекрываеющие лексичесую область видимости
     *     при этот внутри дочерних форм возможны лексические замыкания не содержащие ссылок на связи формы
     * 4)  форма содержит дочерние формы вводящие одноимённые связи - тем самым перекрываеющие лексичесую область видимости
     *     при этот внутри дочерних форм возможны лексические замыкания частияно ссылаяющиеся на связи формы
     * 5) форма содержит дочерние формы которые создают лексические замыкания одного уровня
     * 
     * Под уровнем лексического замыкания понимается вложенность связей
     * 
     * Let(var1 var2 var3)
     *  ..... //level1
     *      lambda(var3 var4)
     *     (setq var1 <value> setq var4 <value>) // level2
     */


    class ClosureSlotDesctiptor
    {
        public Symbol Name;
        public StaticFieldResolver Field;
        public ClosureContainerContext Container;

        public ClosureSlotDesctiptor(StaticFieldResolver field, Symbol name, ClosureContainerContext container)
        {
            Field = field;
            Name = name;
            Container = container;
        }
    }

    class ClosureSlotDesctiptorsCollection : KeyedCollection<Symbol, ClosureSlotDesctiptor>
    {
        protected override Symbol GetKeyForItem(ClosureSlotDesctiptor item)
        {
            return item.Name;
        }
    }

    /// <summary>
    /// возвращается методу вызвавшему предпросмотр
    /// 
    /// после получения этого объекта нужно довести до сведения дочерних 
    /// выражений что они используют ClosureContainerContext
    /// </summary>
    class ClosuresManager
    {
        ClosureClientContextCollection barriers;

        internal ClosureClientContextCollection Clients
        {
            get { return barriers; }
            set { barriers = value; }
        }

        ClosureContainerContextCollection _ClosedOverVarsProvider;

        internal ClosureContainerContextCollection Containers
        { 
            get { return _ClosedOverVarsProvider; }
            set { _ClosedOverVarsProvider = value; }
        }

        public ClosuresManager(ClosureClientContextCollection barriers, ClosureContainerContextCollection containers)
        {
            this.barriers = barriers;
            this._ClosedOverVarsProvider = containers;
        }

        internal ClosureContainerContext GetContainer(INewLexicalBindingsProvider contaner_exp)
        {
            if (_ClosedOverVarsProvider.Contains(contaner_exp))
                return _ClosedOverVarsProvider[contaner_exp];
            else return null;
        }
    }

    class ClosureClientContext
    {
        IBarrier expression;

        internal IBarrier Expression
        {
            get { return expression; }
            set { expression = value; }
        }
        ClosureSlotDesctiptorsCollection closedOverVars;

        internal ClosureSlotDesctiptorsCollection ClosedOverVars
        {
            get { return closedOverVars; }
            set { closedOverVars = value; }
        }

        ClosureContainerContextCollection usedContainers;

        public ClosureContainerContextCollection UsedContainers
        {
            get { return usedContainers; }
            set { usedContainers = value; }
        }

        public ClosureClientContext(IBarrier expression, ClosureSlotDesctiptorsCollection closedOverVars, ClosureContainerContextCollection usedContainers)
        {
            this.expression = expression;
            this.closedOverVars = closedOverVars;
            this.usedContainers = usedContainers;
        }
    }

    class ClosureClientContextCollection : KeyedCollection<IBarrier, ClosureClientContext>
    {
        protected override IBarrier GetKeyForItem(ClosureClientContext item)
        {
            return item.Expression;
        }
    }


    class ClosureContainerContext
    {
        INewLexicalBindingsProvider expression;

        internal INewLexicalBindingsProvider Expression
        {
            get { return expression; }
            set { expression = value; }
        }
        ClosureSlotDesctiptorsCollection closedOverVars;

        internal ClosureSlotDesctiptorsCollection ClosedOverVars
        {
            get { return closedOverVars; }
            set { closedOverVars = value; }
        }
        StaticTypeResolver envType;

        public StaticTypeResolver EnvType
        {
            get { return envType; }
            set { envType = value; }
        }

        public ClosureContainerContext(INewLexicalBindingsProvider expression, ClosureSlotDesctiptorsCollection closedOverVars, StaticTypeResolver envType)
        {
            this.expression = expression;
            this.closedOverVars = closedOverVars;
            this.envType = envType;
        }
    }

    class ClosureContainerContextCollection : KeyedCollection<INewLexicalBindingsProvider, ClosureContainerContext>
    {
        public ClosureContainerContextCollection()
        {
        }
        public ClosureContainerContextCollection(List<LiveLisp.Core.Compiler.ClosureContainerContext> param1)
        {
        		for (int i = 0; i < param1.Count; i++)
			{
                    Add(param1[i]);
			}
        }
        protected override INewLexicalBindingsProvider GetKeyForItem(ClosureContainerContext item)
        {
            return item.Expression;
        }
    }


    internal class ClosureWalkerExpressionContext : ExpressionContext
    {
        public ClosureWalkerExpressionContext ParentWalkerContext;

        public ClosureWalkerExpressionContext(ClosureWalkerExpressionContext parent)
        {
            ParentWalkerContext = parent;
        }

        internal ClosuresWalker GetWalker(Symbol assign_symbol)
        {
            if(localVars.ContainsKey(assign_symbol))
                return localVars[assign_symbol];
            else if(ParentWalkerContext != null)
                return ParentWalkerContext.GetWalker(assign_symbol);
            else
                return null;
        }

        Dictionary<Symbol, ClosuresWalker> localVars = new Dictionary<Symbol, ClosuresWalker>();
        internal void AddLocalVar(Symbol var, ClosuresWalker walker)
        {
            localVars.Add(var, walker);
        }

        internal bool IsLocalVar(Symbol symbol)
        {
            if (localVars.ContainsKey(symbol))
                return true;
            else if (ParentWalkerContext != null)
                return ParentWalkerContext.IsLocalVar(symbol);
            else
                return false;
        }
        public ClosuresWalker _currentWalker;

        public ClosuresWalker CurrentWalker
        {
            get
            {
                if (_currentWalker != null)
                    return _currentWalker;
                else if (ParentWalkerContext != null)
                    return ParentWalkerContext.CurrentWalker;
                else return null;
            }
            set
            {
                _currentWalker = value;
            }
        }
        public IBarrier _barrier;

        public IBarrier Barrier
        {
            get
            {
                if (_barrier != null)
                    return _barrier;
                else if (ParentWalkerContext != null)
                    return ParentWalkerContext.Barrier;
                else
                    return null;
            }
            set
            {
                _barrier = value;
            }
        }

        /// <summary>
        /// переменные вызвавшие лексическое замыкание 
        /// это переменные 
        /// 1) не специальные
        /// 2) введённые локально выше чем текущий барьер
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        internal bool IsClosuredOverVar(Symbol symbol)
        {
            if (Barrier == null)
                return false;
            else
                return ICIOVworker(symbol, Barrier);
        }

        private bool ICIOVworker(Symbol symbol, IBarrier barrier)
        {
            if (localVars.ContainsKey(symbol))
            {
                if (Barrier != barrier)
                    return true;
                else
                    return false;
            }
            else if (ParentWalkerContext != null)
                return ParentWalkerContext.ICIOVworker(symbol, barrier);
            else
                return false;
        }
    }

    internal class ClosuresWalker : IASTWalker
    {
        INewLexicalBindingsProvider closureContainer;
        IImplicitProgn formsProvider;
        Expression expression;
        ExpressionContext cec;
        public ClosuresWalker(Expression closureContainer, ExpressionContext cec)
        {
            this.closureContainer = closureContainer as INewLexicalBindingsProvider;
            if (closureContainer == null)
                throw new ArgumentException("Closure container must implement INewLexicalBindingsProvider");
            formsProvider = closureContainer as IImplicitProgn;
            if (formsProvider == null)
                throw new ArgumentException("Closure container must implement IImplicitProgn");
            this.expression = closureContainer;
            this.cec = cec;
        }

        public ClassDeclaration EnvType;

        Dictionary<IBarrier, ClosuresManager> barriers;

        public ClosuresManager Walk()
        {
            createdTypes = new List<ClassDeclaration>();
            ClosureWalkerExpressionContext root = new ClosureWalkerExpressionContext(null);
            Walk(root);

            // для каждого walker сконструировать тип объекта окружения если надо
            CreateEnvType(this);
            //заполнить словарь barrier

            return FillContext();
        }

        private ClosuresManager FillContext()
        {
            ClosureClientContextCollection clients = new ClosureClientContextCollection();
            ClosureContainerContextCollection containers = new ClosureContainerContextCollection();
            ClosuresManager ret = new ClosuresManager(clients, containers);
            
            _fillContainersWorker(this, containers);
            _fillClientsWorker(this, clients);

            return ret;
        }



        private ClosureContainerContext _Ctx;
        private void _fillContainersWorker(ClosuresWalker closuresWalker, ClosureContainerContextCollection containers)
        {
            ClosureSlotDesctiptorsCollection slots = new ClosureSlotDesctiptorsCollection();
            closuresWalker._Ctx = new ClosureContainerContext(closuresWalker.closureContainer, null, StaticTypeResolver.Create(closuresWalker.EnvType));

            for (int i = 0; i < closuresWalker.closedOverSymbols.Count; i++)
            {
                slots.Add(new ClosureSlotDesctiptor(new FieldDeclarationBasedFieldResolver(fields[closuresWalker.closedOverSymbols[i]]), closuresWalker.closedOverSymbols[i], closuresWalker._Ctx));
            }

            closuresWalker._Ctx.ClosedOverVars = slots;
            containers.Add(closuresWalker._Ctx);

            if (closuresWalker.KnownWalkers != null)
            {
                for (int i = 0; i < closuresWalker.KnownWalkers.Count; i++)
                {
                    _fillContainersWorker(closuresWalker.KnownWalkers[i], containers);
                }
            }
        }

        private void _fillClientsWorker(ClosuresWalker walker, ClosureClientContextCollection clients)
        {
            foreach (var item in walker.usedBySymbols.Keys)
            {
                ClosureClientContext ctx;
                if (clients.Contains(walker.usedBySymbols[item]))
                    ctx = clients[walker.usedBySymbols[item]];
                else
                    ctx = new ClosureClientContext(walker.usedBySymbols[item], new ClosureSlotDesctiptorsCollection(), new ClosureContainerContextCollection());

                if (ctx.ClosedOverVars.Contains(item))
                    continue;

                ClosureSlotDesctiptor csd = new ClosureSlotDesctiptor(fields[item], item, walker._Ctx);
                ctx.ClosedOverVars.Add(csd);
                ctx.UsedContainers.Add(walker._Ctx);
                clients.Add(ctx);
            }

            if (walker.KnownWalkers != null)
                for (int j = 0; j < walker.KnownWalkers.Count; j++)
                {
                    _fillClientsWorker(walker.KnownWalkers[j], clients);
                }
        }

        List<ClassDeclaration> createdTypes;

        void CreateEnvType(ClosuresWalker walker)
        {
            if (walker.closedOverSymbols.Count > 0)
            {
                if (createdTypes.Count > walker.closedOverSymbols.Count)
                {
                    if (createdTypes[walker.closedOverSymbols.Count] != null)
                        walker.EnvType = createdTypes[walker.closedOverSymbols.Count];
                    else
                    {
                        CreateNewEnvType(walker);
                    }
                }
                else
                {
                    for (int j = createdTypes.Count; j <= walker.closedOverSymbols.Count; j++)
                    {
                        createdTypes.Add(null);
                    }

                    CreateNewEnvType(walker);
                }
            }

            if (walker.KnownWalkers != null)
            {
                for (int i = 0; i < walker.KnownWalkers.Count; i++)
                {
                    CreateEnvType(walker.KnownWalkers[i]);
                }
            }
        }

        Dictionary<Symbol, FieldDeclaration> fields;

        void CreateNewEnvType(ClosuresWalker walker)
        {
            fields = new Dictionary<Symbol, FieldDeclaration>(); ;
            ClassDeclaration new_env = new ClassDeclaration();
            for (int j = 0; j < walker.closedOverSymbols.Count; j++)
            {
                FieldDeclaration new_envNewGeneratedField = new_env.NewGeneratedField(walker.closedOverSymbols[j].ToString(true), typeof(object));
                new_envNewGeneratedField.Attributes = System.Reflection.FieldAttributes.Public;
                fields.Add(walker.closedOverSymbols[j], new_envNewGeneratedField);
            }
            new_env.Name = Helpers.new_ClassName("ClosureEnv");
            new_env.Attributes = System.Reflection.TypeAttributes.Public;
            createdTypes[walker.closedOverSymbols.Count] = new_env;
            walker.EnvType = new_env;

        }

        private void Walk(ClosureWalkerExpressionContext root)
        {
            barriers = new Dictionary<IBarrier, ClosuresManager>();
            if (root.CurrentWalker == null)
                root.CurrentWalker = this;
            else
            {
                root.CurrentWalker.Add(this);
                root.CurrentWalker = this;
            }
            RemoveSpecialVars(root);
            ClosureWalkerExpressionContext ctx;
            for (int i = 0; i < formsProvider.Count - 1; i++)
            {
                ctx = new ClosureWalkerExpressionContext(root);
                ctx.NonVoidReturn = false;
                formsProvider[i].Visit(this, ctx);
            }

            ctx = new ClosureWalkerExpressionContext(root);
            ctx.NonVoidReturn = true;
            formsProvider[formsProvider.Count - 1].Visit(this, ctx);
        }

        List<Symbol> localVariables;

        List<Symbol> closedOverSymbols = new List<Symbol>();

        private void RemoveSpecialVars(ClosureWalkerExpressionContext context)
        {
            localVariables = new List<Symbol>();
            for (int i = 0; i < closureContainer.IntroducesLexicalVariables.Count; i++)
            {
                Symbol var = closureContainer.IntroducesLexicalVariables[i];
                if (!cec.IsSpecial(var))
                {
                    localVariables.Add(var);
                    context.AddLocalVar(var, this);
                }
            }
        }

        List<ClosuresWalker> KnownWalkers;

        internal void Add(ClosuresWalker new_walker)
        {

            if (KnownWalkers == null) KnownWalkers = new List<ClosuresWalker>();
            KnownWalkers.Add(new_walker);

        }

        Dictionary<Symbol, IBarrier> usedBySymbols = new Dictionary<Symbol, IBarrier>();

        private void AddUsedBySymbol(Symbol symbol, IBarrier barrier)
        {
            if (!usedBySymbols.ContainsKey(symbol))
                usedBySymbols.Add(symbol, barrier);
        }

        Dictionary<Symbol, ClosuresWalker> usedFromSymbols = new Dictionary<Symbol, ClosuresWalker>();

        private void AddUsedFromSymbol(Symbol assign_symbol, ClosuresWalker walker)
        {
            if (!usedFromSymbols.ContainsKey(assign_symbol))
                usedFromSymbols.Add(assign_symbol, walker);
        }

        #region IASTWalker Members

        public void BlockExpression(LiveLisp.Core.AST.Expressions.BlockExpression blockExpression, ExpressionContext context)
        {
            bool void_backup = context.NonVoidReturn;

            context.NonVoidReturn = false;
            for (int i = 0; i < blockExpression.Count - 1; i++)
            {
                blockExpression[i].Visit(this, context);
            }

            context.NonVoidReturn = void_backup;

            if (blockExpression.Count > 0)
                blockExpression[blockExpression.Count - 1].Visit(this, context);
        }

        public void CallExpression(LiveLisp.Core.AST.Expressions.CallExpression callExpression, ExpressionContext context)
        {
            ClosureWalkerExpressionContext ccc;
            if (callExpression.Function.DesignatorType == FunctionNameDesignatorType.Lambda)
            {
                ccc = new ClosureWalkerExpressionContext(context as ClosureWalkerExpressionContext);
                ccc.NonVoidReturn = true;
                callExpression.Function.Visit(this, ccc);
            }

            for (int i = 0; i < callExpression.Parameters.Count; i++)
            {
                ccc = new ClosureWalkerExpressionContext(context as ClosureWalkerExpressionContext);
                ccc.NonVoidReturn = true;
                callExpression.Parameters[i].Visit(this, ccc);
            }
        }

        public void CatchExpression(LiveLisp.Core.AST.Expressions.CatchExpression catchExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrClassExpression(LiveLisp.Core.AST.Expressions.ClrClassExpression clrClassExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrDelegateExpression(LiveLisp.Core.AST.Expressions.ClrDelegateExpression clrDelegateExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrEnumExpression(LiveLisp.Core.AST.Expressions.ClrEnumExpression clrEnumExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrMethodExpression(LiveLisp.Core.AST.Expressions.ClrMethodExpression clrMethodExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ConstantExpression(LiveLisp.Core.AST.Expressions.ConstantExpression constantExpression, ExpressionContext context)
        {
           
        }

        public void EvalWhenExpression(LiveLisp.Core.AST.Expressions.EvalWhenExpression evalWhenExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void FletExpression(LiveLisp.Core.AST.Expressions.FletExpression fletExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void FunctionExpression(LiveLisp.Core.AST.Expressions.FunctionExpression functionExpression, ExpressionContext context)
        {
            if (functionExpression.DesignatorType == FunctionNameDesignatorType.Lambda)
            {
                ClosuresWalker new_walker = new ClosuresWalker(functionExpression.Designator as LambdaFunctionDesignator, context);

                ClosureWalkerExpressionContext new_ctx = new ClosureWalkerExpressionContext(context as ClosureWalkerExpressionContext);
                new_ctx.Barrier = functionExpression.Designator as LambdaFunctionDesignator;
                new_walker.Walk(new_ctx);
            }
        }

        public void GoExpression(LiveLisp.Core.AST.Expressions.GoExpression goExpression, ExpressionContext context)
        {
        }

        public void IfExpression(LiveLisp.Core.AST.Expressions.IfExpression ifExpression, ExpressionContext context)
        {
            ClosureWalkerExpressionContext ccc = new ClosureWalkerExpressionContext(context as ClosureWalkerExpressionContext);

            ccc.NonVoidReturn = true;
            ifExpression.Condition.Visit(this, ccc);

            ccc = new ClosureWalkerExpressionContext(context as ClosureWalkerExpressionContext);
            ccc.NonVoidReturn = context.NonVoidReturn;
            ifExpression.Then.Visit(this, ccc);
            ccc = new ClosureWalkerExpressionContext(context as ClosureWalkerExpressionContext);
            ccc.NonVoidReturn = context.NonVoidReturn;
            ifExpression.Else.Visit(this, ccc);
        }

        public void ILCodeExpression(LiveLisp.Core.AST.Expressions.CLR.ILCodeExpression iLCodeExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LabelsExpression(LiveLisp.Core.AST.Expressions.LabelsExpression labelsExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LetExpression(LiveLisp.Core.AST.Expressions.LetExpression letExpression, ExpressionContext context)
        {
            bool void_backup = context.NonVoidReturn;

            context.NonVoidReturn = false;
            for (int i = 0; i < letExpression.Count - 1; i++)
            {
                letExpression[i].Visit(this, context);
            }

            context.NonVoidReturn = void_backup;

            if (letExpression.Count > 0)
                letExpression[letExpression.Count - 1].Visit(this, context);
        }

        public void LetStarExpression(LiveLisp.Core.AST.Expressions.LetStarExpression letStarExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LoadTimeValueExpression(LiveLisp.Core.AST.Expressions.LoadTimeValue loadTimeValue, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LocallyExpression(LiveLisp.Core.AST.Expressions.LocallyExpression locallyExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void MacroletExpression(LiveLisp.Core.AST.Expressions.MacroletExpression macroletExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void MultipleValueCallExpression(LiveLisp.Core.AST.Expressions.MultipleValueCallExpression multipleValueCallExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void MultipleValueProg1Expression(LiveLisp.Core.AST.Expressions.MultipleValueProg1Expression multipleValueProg1Expression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void PrognExpression(LiveLisp.Core.AST.Expressions.PrognExpression prognExpression, ExpressionContext context)
        {
            bool void_backup = context.NonVoidReturn;

            context.NonVoidReturn = false;
            for (int i = 0; i < prognExpression.Count - 1; i++)
            {
                prognExpression[i].Visit(this, context);
            }

            context.NonVoidReturn = void_backup;

            if (prognExpression.Count > 0)
                prognExpression[prognExpression.Count - 1].Visit(this, context);
        }

        public void ProgvExpression(LiveLisp.Core.AST.Expressions.ProgvExpression progvExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ReturnFromExpression(LiveLisp.Core.AST.Expressions.ReturnFromExpression returnFromExpression, ExpressionContext context)
        {
            returnFromExpression.Form.Visit(this, context);
        }

        public void SetqExpression(LiveLisp.Core.AST.Expressions.SetqExpression setqExpression, ExpressionContext context)
        {
            ClosureWalkerExpressionContext cwec = context as ClosureWalkerExpressionContext;

            for (int i = 0; i < setqExpression.Assings.Count; i++)
            {
                Symbol assign_symbol = setqExpression.Assings[i].Symbol;
                if (cwec.IsClosuredOverVar(assign_symbol))
                {
                    // reference to closed over variable

                    ClosuresWalker walker = cwec.GetWalker(assign_symbol);
                    if (!walker.closedOverSymbols.Contains(assign_symbol))
                        walker.closedOverSymbols.Add(assign_symbol);

                    walker.AddUsedBySymbol(assign_symbol, cwec.Barrier);
                    this.AddUsedFromSymbol(assign_symbol, walker);
                }

                ClosureWalkerExpressionContext ctx = new ClosureWalkerExpressionContext(context as ClosureWalkerExpressionContext);
                ctx.NonVoidReturn = true;
                setqExpression.Assings[i].Value.Visit(this, ctx);
            }
        }

        public void SymbolFunctionDesignator(SymbolFunctionDesignator symbolFunctionDesignator, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void SymbolMacroletExpression(LiveLisp.Core.AST.Expressions.SymbolMacroletExpression symbolMacroletExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void TagbodyExpression(LiveLisp.Core.AST.Expressions.TagBodyExpression tagBodyExpression, ExpressionContext context)
        {
            bool void_backup = context.NonVoidReturn;

            context.NonVoidReturn = false;
            for (int i = 0; i < tagBodyExpression.NontaggedProlog.Count; i++)
            {
                tagBodyExpression.NontaggedProlog[i].Visit(this, context);
            }

            for (int i = 0; i < tagBodyExpression.TaggedStatements.Count; i++)
            {
                List<Expression> stms = tagBodyExpression.TaggedStatements[i].Statements;
                for (int j = 0; j < stms.Count; j++)
                {
                    stms[j].Visit(this, context);
                }
            }
            context.NonVoidReturn = void_backup;
        }

        public void TheExpression(LiveLisp.Core.AST.Expressions.TheExpression theExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ThrowExpression(LiveLisp.Core.AST.Expressions.ThrowExpression throwExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void UnwindProtectExpression(LiveLisp.Core.AST.Expressions.UnwindProtectExpression unwindProtectExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void VariableExpression(LiveLisp.Core.AST.Expressions.VariableExpression variableExpression, ExpressionContext context)
        {
            ClosureWalkerExpressionContext cwec = context as ClosureWalkerExpressionContext;

            if (context.NonVoidReturn)
            {
                if (cwec.IsClosuredOverVar(variableExpression.VariableName))
                {

                    // reference to closed over variable

                    ClosuresWalker walker = cwec.GetWalker(variableExpression.VariableName);
                    if (!walker.closedOverSymbols.Contains(variableExpression.VariableName))
                        walker.closedOverSymbols.Add(variableExpression.VariableName);

                    walker.AddUsedBySymbol(variableExpression.VariableName, cwec.Barrier);
                    this.AddUsedFromSymbol(variableExpression.VariableName, walker);
                }
            }
        }

        public void MethodInfoFunctionDesignator(MethodInfoFunctionDesignator methodInfoFunctionDesignator, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void EmitMethodInfoDesignatorCall(MethodInfoFunctionDesignator methodInfoFunctionDesignator, LiveLisp.Core.AST.Expressions.CallExpression call, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void EmitMethodInfoDesignatorCall(SymbolFunctionDesignator symbolFunctionDesignator, LiveLisp.Core.AST.Expressions.CallExpression call, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrNewExpression(LiveLisp.Core.AST.Expressions.ClrNewExpression clrNewExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LambdaOverloadsExpression(LambdaOverloads lambdaOverloads, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LambdaOverload(LambdaOverload lambdaOverload, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrIfExpression(LiveLisp.Core.AST.Expressions.ClrIfExpression clrIfExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrConstantExpression(LiveLisp.Core.AST.Expressions.ClrConstantExpression clrConstantExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void EmitFunctionDesignatorCall(LiveLisp.Core.AST.Expressions.FunctionExpression functionExpression, LiveLisp.Core.AST.Expressions.CallExpression call, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrTryExpression(LiveLisp.Core.AST.Expressions.ClrTryExpression clrTryExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
