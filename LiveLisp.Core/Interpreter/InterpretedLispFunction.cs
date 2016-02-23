using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;
using LiveLisp.Core.AST;
using LiveLisp.Core.Eval;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Reader;
using LiveLisp.Core.CLOS;
using System.Collections;
using LiveLisp.Core.BuiltIns.TypesAndClasses;

namespace LiveLisp.Core.Interpreter
{
    public class InterpretedLispFunction : SingleFunction
    {
        LambdaList lambdaList; Expression body; EvaluationContext ctx;
        public InterpretedLispFunction(Symbol name, LambdaList lambdaList, Expression body, EvaluationContext ctx)
            : base(name.ToString(true))
        {
            this.lambdaList = lambdaList;
            this.body = body;
            this.ctx = ctx;

            MinArgsCount = lambdaList.MinParamsCount;
            MaxArgsCount = lambdaList.MaxParamsCount;
        }

        public override bool IsCompiled
        {
            get
            {
                return false;
            }

            set
            {
                throw new NotImplementedException();
                //things like dynamic compilation / decompilation ^-)
            }
        }

        public override object ValuesInvoke(object[] args)
        {
            Stack<Pair<Symbol, object>> backups = null;
            try
            {
                // bind args to lambdalist provided params
                EvaluationContext new_ctx = BindParameters(args, out backups);
                // eval in new environment
                new_ctx.SaveValues = true;
                return body.Eval(new Interpreter(), new_ctx);
            }
            finally
            {
                while (backups.Count != 0)
                {
                    var pair = backups.Pop();

                    pair.First.RawValue = pair.Second;
                }
            }
        }

        private EvaluationContext BindParameters(object[] args, out Stack<Pair<Symbol, object>> backups)
        {
            CheckArgsCount(args.Length);

            backups = new Stack<Pair<Symbol, object>>();
            EvaluationContext new_ctx = new EvaluationContext(ctx);

            object rest = null;

            for (int i = 0; i < lambdaList.Count; i++)
            {
                LambdaParameter parameter = lambdaList[i];
                bool value_supplied = false;
                Dictionary<int, object> pairs = null;
                ParameterWithInitForm optional_param;
                switch (parameter.Kind)
                {
                    case ParameterKind.Required:
                        Bind(new_ctx, parameter.Name, args[i], ref backups);
                        break;
                    case ParameterKind.Optional:
                        optional_param = parameter as ParameterWithInitForm;
                        if (i < args.Length)
                        {
                            value_supplied = true;
                            Bind(new_ctx, parameter.Name, args[i], ref backups);
                        }
                        else
                        {
                            Bind(new_ctx, parameter.Name, optional_param.DefaultValue.Eval(new Interpreter(), new_ctx), ref backups);
                        }

                        if (optional_param.Suppliedp)
                        {
                            Bind(new_ctx, optional_param.SuppliedpName, value_supplied ? DefinedSymbols.T : DefinedSymbols.NIL, ref backups);
                        }
                        break;
                    case ParameterKind.Rest:
                        SetupRest(args, ref rest, i);

                        Bind(new_ctx, parameter.Name, rest, ref backups);
                        break;
                    case ParameterKind.Key:
                        if (rest == null)
                        {
                            SetupRest(args, ref rest, i);
                        }
                        if (pairs == null)
                        {
                            pairs = RuntimeHelpers.ValidateListForKeyParameters(parameter.Name, rest, lambdaList.KeysList, lambdaList.AllowOtherKeys);
                        }


                        optional_param = parameter as ParameterWithInitForm;

                        if (pairs.ContainsKey(parameter.Name.Id))
                        {
                            Bind(new_ctx, parameter.Name, pairs[parameter.Name.Id], ref backups);
                            value_supplied = true;
                        }
                        else
                        {
                            Bind(new_ctx, parameter.Name, optional_param.DefaultValue.Eval(new Interpreter(), new_ctx), ref backups);
                        }
                        if (optional_param.Suppliedp)
                            Bind(new_ctx, optional_param.SuppliedpName, value_supplied ? DefinedSymbols.T : DefinedSymbols.NIL, ref backups);
                        break;
                    case ParameterKind.Params:

                        object[] _params;
                        if (i < args.Length)
                        {
                            _params = new object[args.Length - i];
                            Array.Copy(args, i, _params, i, _params.Length);
                            Bind(new_ctx, parameter.Name, _params, ref backups);
                        }
                        else
                        {
                            Bind(new_ctx, parameter.Name, new object[0], ref backups);
                        }
                        break;
                    case ParameterKind.Environment:
                        break;
                    case ParameterKind.Whole:
                        break;
                    case ParameterKind.Aux:
                        Bind(new_ctx, parameter.Name, (parameter as AuxParameter).InitForm.Eval(new Interpreter(), new_ctx), ref backups);
                        break;
                    default:
                        break;
                }
            }

            return new_ctx;
        }

        private static void SetupRest(object[] args, ref object rest, int i)
        {
            object[] _rest;
            if (i < args.Length)
            {
                _rest = new object[args.Length - i];
                Array.Copy(args, i, _rest, i, _rest.Length);
                rest = Cons.FromCollection(_rest);
            }
            else
                rest = DefinedSymbols.NIL;
        }

        private void Bind(EvaluationContext new_ctx, Symbol symbol, object value, ref Stack<Pair<Symbol, object>> backups)
        {
            if (new_ctx.IsSpecial(symbol))
            {
                backups.Push(new Pair<Symbol, object>(symbol, symbol.RawValue));
                symbol.RawValue = value;
            }
            else
                new_ctx.Bind(symbol, value);
        }

        public override object this[Symbol name]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool Eq(object obj)
        {
            throw new NotImplementedException();
        }
    }

    public class GenericFunction : SingleFunction
    {
        public List<GenericMethod> PrimaryMethods = new List<GenericMethod>();
        public List<GenericMethod> BeforeMethods = new List<GenericMethod>();
        public List<GenericMethod> AfterMethods = new List<GenericMethod>();
        public List<GenericMethod> AroundMethods = new List<GenericMethod>();

        public GenericFunction(Symbol name)
            : base(name.ToString())
        {
            this.Name = name;
        }

        public void AddMethod(GenericMethod method)
        {
            GenericSignatureComparer comparer = new GenericSignatureComparer();
            List<GenericMethod> list;
            int sign = 1;

            switch (method.Type)
            {
                case GenericMethodType.Before:
                    {
                        list = BeforeMethods;
                        break;
                    }
                case GenericMethodType.After:
                    {
                        sign = -1;
                        list = AfterMethods;
                        break;
                    }
                case GenericMethodType.Around:
                    {
                        list = AroundMethods;
                        break;
                    }
                case GenericMethodType.Primary:
                default:
                    {
                        list = PrimaryMethods;
                        break;
                    }
            }
            for (int i = 0; i < list.Count; ++i)
            {
                int result = comparer.Compare(method, list[i]);
                if (sign * result == -1)
                {
                    list.Insert(i, method);
                    return;
                }
                else if (result == 0)
                {
                    list[i] = method;
                    return;
                }
            }

            list.Add(method);
        }

        public override object ValuesInvoke(object[] args)
        {
            NextMethodState saved_state = NextMethodState.CurrentState;

            try
            {
                NextMethodState.CurrentState = new NextMethodState(args, this);
                return CallNextMethod(null);
            }
            finally
            {
                NextMethodState.CurrentState = saved_state;
            }
        }

        bool Matching(Cons args, LambdaList llist)
        {
            for (int i = 0; i < llist.MinParamsCount; ++i, args = args.Child)
            {
                if (args == null)
                {
                    return false;
                }

                object type = llist[i].Class;
                if (type != null)
                {
                    object expr = args.Car;
                    object result = TypesAndClassesDictionary.Typep(expr, type, DefinedSymbols.NIL);
                    if (result == DefinedSymbols.NIL)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        object CallNextMethod(object[] newArgs)
        {
            if (NextMethodState.CurrentState == null)
            {
                return null;
            }

            NextMethodState state = NextMethodState.CurrentState;

            object[] args;

            if (newArgs == null)
            {
                args = state.Args;
            }
            else
            {
                args = newArgs;
            }

            while (state.Iter.MoveNext())
            {
                GenericMethod method = (GenericMethod)state.Iter.Current;
                if (Matching(state.Args, method.LambdaList))
                {
                    return method.ValuesInvoke(args);
                }
            }

            if (!state.Around)
            {
                return null;
            }

            state.Around = false;
            state.Iter = state.Generic.PrimaryMethods.GetEnumerator();

            // switch to before, primary, after
            foreach (GenericMethod method in state.Generic.BeforeMethods)
            {
                if (Matching(state.Args, method.LambdaList))
                {
                    method.ValuesInvoke(args);
                }
            }

            object result = UnboundValue.Unbound;

            while (state.Iter.MoveNext())
            {
                GenericMethod method = state.Iter.Current;
                if (Matching(state.Args, method.LambdaList))
                {
                    result = state.Iter.Current.ValuesInvoke(args);
                    break;
                }
            }

            if (result == UnboundValue.Unbound)
            {
                throw new SimpleErrorException("error", "no matching generic method found");
            }

            foreach (GenericMethod method in state.Generic.AfterMethods)
            {
                if (Matching(state.Args, method.LambdaList))
                {
                    method.VoidInvoke(args);
                }
            }

            return result;
        }

        public override object this[Symbol name]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool Eq(object obj)
        {
            throw new NotImplementedException();
        }
    }

    enum CompareClassResult
    {
        NoResult,
        Equal,
        SubClass,
        SuperClass
    }

    class NextMethodState
    {
        public GenericFunction Generic;
        public object[] Args;
        public bool Around;
        public IEnumerator<GenericMethod> Iter;

        public NextMethodState(object[] args, GenericFunction generic)
        {
            Args = args;
            Generic = generic;
            Around = true;
            Iter = generic.AroundMethods.GetEnumerator();
        }

        [ThreadStatic]
        public static NextMethodState CurrentState;
    }

    class GenericSignatureComparer : IComparer<GenericMethod>
    {
        // if x more specific than y then return -1
        public int Compare(GenericMethod x, GenericMethod y)
        {
            if (x.Type < y.Type)
            {
                return -1;
            }

            if (x.Type > y.Type)
            {
                return 1;
            }

            if (x.LambdaList.MinParamsCount > y.LambdaList.MinParamsCount)
            {
                // more arguments is more specific
                return -1;
            }

            if (x.LambdaList.MinParamsCount < y.LambdaList.MinParamsCount)
            {
                // more arguments is more specific
                return 1;
            }

            for (int i = 0; i < x.LambdaList.MinParamsCount; ++i)
            {
                Symbol class1 = x.LambdaList[i].Class;
                Symbol class2 = y.LambdaList[i].Class;
                CompareClassResult result = CompareClass(class1, class2);
                switch (result)
                {
                    case CompareClassResult.NoResult:
                        {
                            return String.Compare(class1.Name, class2.Name);
                        }
                    case CompareClassResult.SubClass:
                        {
                            // more specific must be on top
                            return -1;
                        }
                    case CompareClassResult.SuperClass:
                        {
                            // less specific must be on bottom
                            return 1;
                        }
                    case CompareClassResult.Equal:
                    default:
                        {
                            // next slot
                            break;
                        }
                }
            }

            return 0;

        }

        CompareClassResult CompareClass(Symbol class1, Symbol class2)
        {
            if (class1 == DefinedSymbols.T)
            {
                if (class2 == DefinedSymbols.T)
                {
                    return CompareClassResult.Equal;
                }
                else
                {
                    return CompareClassResult.SuperClass;
                }
            }
            else if (class2 == DefinedSymbols.T)
            {
                return CompareClassResult.SubClass;
            }
            else
            {
                CLOSClass k1 = CLOSTypeTable.Instance[class1.Id];
                CLOSClass k2 = CLOSTypeTable.Instance[class2.Id];


                if (k1 is CLOSCLRClass && k2 is CLOSCLRClass)
                {
                    CLOSCLRClass c1 = (CLOSCLRClass)k1;
                    CLOSCLRClass c2 = (CLOSCLRClass)k2;
                    if (c1 == c2)
                    {
                        return CompareClassResult.Equal;
                    }
                    else if (c1.Type.IsSubclassOf(c2.Type))
                    {
                        return CompareClassResult.SubClass;
                    }
                    else if (c2.Type.IsSubclassOf(c2.Type))
                    {
                        return CompareClassResult.SuperClass;
                    }
                    else
                    {
                        return CompareClassResult.NoResult;
                    }
                }
                else
                {
                    CLOSClass c1 = (CLOSClass)k1;
                    CLOSClass c2 = (CLOSClass)k2;
                    if (c1 == c2)
                    {
                        return CompareClassResult.Equal;
                    }
                    else if (c1.IsSubTypeOf(c2))
                    {
                        return CompareClassResult.SubClass;
                    }
                    else if (c2.IsSubTypeOf(c2))
                    {
                        return CompareClassResult.SuperClass;
                    }
                    else
                    {
                        return CompareClassResult.NoResult;
                    }

                }
            }

        }
    }

    public enum GenericMethodType
    {
        Primary,
        Before,
        After,
        Around
    }

    public class GenericMethod : SingleFunction
    {
        public GenericMethod(Symbol name)
            : base("GenericMethod_" + name)
        {

        }

        public GenericMethodType Type;
        public LambdaList LambdaList;
    }
}
