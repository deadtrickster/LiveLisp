using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST;
using System.Reflection.Emit;
using LiveLisp.Core.AST.Expressions;
using LiveLisp.Core.AST.Expressions.CLR;
using LiveLisp.Core.CLOS;
using LiveLisp.Core.Types;
using LiveLisp.Core.Runtime;
using System.Reflection;
using LiveLisp.Core.Reader;

namespace LiveLisp.Core.Compiler
{
    public static class Helpers
    {

        static Dictionary<string, int> counters = new Dictionary<string, int>();
        public static string new_ClassName(string name)
        {
            name = name.Replace("*", "_");
            StaticTypeResolver resolver = new StaticTypeResolver(name);

            string new_name = name;
            Type t;

            if (counters.ContainsKey(name))
            {
                return name + "$" + NewLambdaVersion(name);
            }
            else
            {
                counters.Add(name, 0);
            }

            if (resolver.TryResolve(out t))
            {
                do
                {
                    new_name = name + "$" + NewLambdaVersion(name);
                    resolver.TypeName = new_name;
                }
                while (resolver.TryResolve(out t));
            }


            return new_name;
        }

        private static int NewLambdaVersion(string str)
        {
            if (counters.ContainsKey(str))
            {
                return counters[str]++;
            }
            else
            {
                counters.Add(str, 1);
                return 1;
            }
        }

        public static Type BoxType(this object constant)
        {
            ILispObject lobj = constant as ILispObject;
            if (lobj != null)
            {
                switch (lobj.LispTypeCode)
                {
                    case LispObjectTypeCode.Symbol:
                        break;
                    case LispObjectTypeCode.Cons:
                        break;
                    case LispObjectTypeCode.Number:
                        ILispNumber lnum = constant as ILispNumber;
                        switch (lnum.NumberType)
                        {
                            case NumberType.Byte:
                                return typeof(byte);
                            case NumberType.Int:
                                return typeof(Int32);
                            case NumberType.BigInt:
                                return typeof(BigInteger);
                            case NumberType.Long:
                                return typeof(Int64);
                            case NumberType.Ratio:
                                return typeof(Ratio);
                            case NumberType.Single:
                                return typeof(Single);
                            case NumberType.Double:
                                return typeof(Double);
                            case NumberType.Complex:
                                return typeof(Complex);
                            case NumberType.Decimal:
                                return typeof(Decimal);
                            default:
                                break;
                        }

                        break;
                    case LispObjectTypeCode.CLOSClass:
                        break;
                    case LispObjectTypeCode.String:
                        return typeof(String);
                    case LispObjectTypeCode.Hashtable:
                        break;
                    default:
                        break;
                }
            }
            return constant.GetType();
        }
    }

    internal sealed class BlockExitPoint
    {
        public readonly InstructionsBlock InstructionsBlock;
        public readonly Guid Id;
        public readonly LabelDeclaration LocalExitPoint;
        public readonly Symbol Name;
        public readonly bool NonVoid;

        public bool NonLocal;

        public BlockExitPoint(Symbol name, InstructionsBlock InstructionsBlock, Guid Id, LabelDeclaration ExitPoint, bool nonVoid)
        {
            Name = name;
            this.InstructionsBlock = InstructionsBlock;
            this.Id = Id;
            this.LocalExitPoint = ExitPoint;
            NonVoid = nonVoid;
        }
    }

    internal sealed class NonLocalHook
    {
        public object Tag;

        public NonLocalHook(object Tag)
        {
            this.Tag = Tag;
        }
    }

    internal sealed class TagbodyExitPoint
    {
        public readonly InstructionsBlock InstructionsBlock;
        public readonly Guid TagbodyId;
        public readonly Int32 TagId;
        public readonly LabelDeclaration TagLabel;
        public readonly Object Tag;

        public bool NonLocal;

        public TagbodyExitPoint(InstructionsBlock InstructionsBlock, Guid TagbodyId, Int32 TagId, LabelDeclaration TagLabel, object Tag)
        {
            this.InstructionsBlock = InstructionsBlock;
            this.TagbodyId = TagbodyId;
            this.TagId = TagId;
            this.TagLabel = TagLabel;
            this.Tag = Tag;
        }

        public NonLocalHook NonLocalHook;
    }

    public delegate object InterpreterDelegate();
    public class DefaultASTCompiler : IASTWalker
    {
        internal static void JustWalk(Expression expression, ILGenerator ilgenerator)
        {
            var th = new DefaultASTCompiler();
            InstructionsBlock mainBlock = new InstructionsBlock();
            CompilationContext ccontext = new CompilationContext(mainBlock);
            CompilationExpressionContext econtext = new CompilationExpressionContext(null, ccontext);
            econtext.NonVoidReturn = true;
            expression.Visit(th, econtext);
            new CompilerImpl().Comple(ccontext, ilgenerator);
        }

        static int EvalCounter = 0;
        internal InterpreterDelegate Compile(Expression exp)
        {

            CompilationContext ccontext = CreateCompilationContext(exp);
            EvalCounter++;
            var Method = new DynamicMethod("EvalCall_" + EvalCounter, typeof(Object), Type.EmptyTypes);

            var ILGen = (Method as DynamicMethod).GetILGenerator();
            new CompilerImpl().Comple(ccontext, ILGen);
            ILGen.EmitRet();
            return (Method as DynamicMethod).CreateDelegate(typeof(InterpreterDelegate)) as InterpreterDelegate;
        }

        internal static void JustWalk(Expression exp, CompilationContext ccontext)
        {
            var th = new DefaultASTCompiler();
            CompilationExpressionContext econtext = new CompilationExpressionContext(null, ccontext);
            econtext.NonVoidReturn = true;
            exp.Visit(th, econtext);
            new CompilerImpl().Comple(ccontext);
        }

        private CompilationContext CreateCompilationContext(Expression exp)
        {
            InstructionsBlock mainBlock = new InstructionsBlock();
            CompilationContext ccontext = new CompilationContext(mainBlock);
            MethodDeclaration mdecl = new MethodDeclaration();
            mdecl.Instructions = ccontext.MainInstructionsBlock;
            CompilationExpressionContext econtext = new CompilationExpressionContext(null, ccontext);
            econtext.ConstantsManager = new MethodScopedConstantsManager(mdecl);
            econtext.NonVoidReturn = true;
            econtext.SaveValues = true;
            exp.Visit(this, econtext);

            ccontext.MainInstructionsBlock.InsertRange(0, mdecl.MethodProlog);
            return ccontext;
        }

        private void BuildArray(List<Expression> exps, CompilationExpressionContext cec)
        {
            VariableDeclaration arr = cec.InstructionsBlock.DefineLocal(typeof(object[]));

            cec.InstructionsBlock.Add(new Ldc_I4Instruction(exps.Count));
            cec.InstructionsBlock.Add(new NewarrInstruction(typeof(object)));

            cec.InstructionsBlock.Add(new StlocInstruction((short)arr.Id));

            for (int i = 0; i < exps.Count; i++)
            {
                cec.InstructionsBlock.Add(new LdlocInstruction((short)arr.Id));
                cec.InstructionsBlock.Add(new Ldc_I4Instruction(i));

                exps[i].Visit(this, cec);
                //        if (exps[i].ReturnType.IsValueType)
                //            ILGen.Emit(OpCodes.Box, exps[i].ReturnType);

                cec.InstructionsBlock.Add(new StelemInstruction(typeof(object)));
            }

            cec.InstructionsBlock.Add(new LdlocInstruction((short)arr.Id));
        }

        #region IASTWalker Members

        public void BlockExpression(LiveLisp.Core.AST.Expressions.BlockExpression blockExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = new CompilationExpressionContext(context as CompilationExpressionContext);

            bool void_backup = cec.NonVoidReturn;
            cec.NonVoidReturn = false;
            LabelDeclaration exit_label = cec.InstructionsBlock.DefineLabel();

            InstructionsBlock backup = cec.InstructionsBlock;

         //   InstructionsBlock current = cec.InstructionsBlock = new InstructionsBlock();
            int index = cec.InstructionsBlock.Count;
            BlockExitPoint exit_point = new BlockExitPoint(blockExpression.BlockName, cec.InstructionsBlock, Guid.NewGuid(), exit_label, void_backup);

            cec.AddBlockExitPoint(blockExpression.BlockName, exit_point);

            for (int i = 0; i < blockExpression.Count - 1; i++)
            {
                blockExpression.Forms[i].Visit(this, cec);
            }

            cec.NonVoidReturn = void_backup;
            blockExpression.Forms[blockExpression.Count - 1].Visit(this, cec);

            if (exit_point.NonLocal)
            {
                VariableDeclaration ret_slot = null;
                if (void_backup)
                    ret_slot = backup.DefineLocal();

                LabelDeclaration ExcExitLabel = new LabelDeclaration(backup.NewUniqueLabelName());
                backup.Insert(index, new BeginExceptionBlock(ExcExitLabel));

                cec.InstructionsBlock.Add(new MarkLabelInstruction(exit_label));
                if (void_backup)
                    backup.Add(new StlocInstruction(ret_slot));
                backup.Add(new LeaveInstruction(ExcExitLabel.Name));

                //catch the exception
                backup.Add(new BeginCatchBlock(typeof(BlockNonLocalTransfer)));
                VariableDeclaration exc_slot = backup.DefineLocal(typeof(BlockNonLocalTransfer));
                backup.Add(new StlocInstruction(exc_slot));

                #region tag check
                var temp = backup.DefineLocal();

                var elselabel = backup.DefineLabel();
                // var endlabel = instructionsBlock.DefineLabel();

                backup.Add(new LdlocInstruction(exc_slot));
                backup.Add(new LdfldaInstruction(BlockNonLocalTransfer.TagIdField));
                backup.Add(new PushInstruction(exit_point.Id));
                backup.Add(new CallInstruction(RuntimeHelpers.GuidEqualsMethod));
                backup.Add(new BrfalseInstruction(elselabel));
                if (void_backup)
                {
                    backup.Add(new LdlocInstruction(exc_slot));
                    backup.Add(new LdfldInstruction(BlockNonLocalTransfer.ResultField));
                    backup.Add(new StlocInstruction(ret_slot));
                }
                backup.Add(new LeaveInstruction(ExcExitLabel.Name));
                backup.Add((new MarkLabelInstruction(elselabel)));
                backup.Add(new RethrowInstruction());
                backup.Add(new EndExceptionBlock());

                if (void_backup)
                    backup.Add(new LdlocInstruction(ret_slot));
                #endregion
            }
            else
            {
                cec.InstructionsBlock.Add(new MarkLabelInstruction(exit_label));
            }
        }

        public void CallExpression(LiveLisp.Core.AST.Expressions.CallExpression callExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;
            switch (callExpression.Function.DesignatorType)
            {
                case FunctionNameDesignatorType.Symbol:
                    GenerateSymbolFunctionLoading(callExpression.Function as SymbolFunctionDesignator, context as CompilationExpressionContext);
                    EmitLambdaCall(callExpression, cec);
                    break;
                case FunctionNameDesignatorType.Lambda:
                    InstantiateNewLambda(callExpression.Function as LambdaFunctionDesignator, cec);
                    EmitLambdaCall(callExpression, cec);
                    break;
                case FunctionNameDesignatorType.MethodInfo:
                    EmitMethodInfoCall(callExpression, cec);
                    break;
                default:
                    break;
            }
        }

        private void EmitMethodInfoCall(CallExpression callExpression, CompilationExpressionContext cec)
        {
            bool void_backup = cec.NonVoidReturn;
            cec.NonVoidReturn = true;
            bool values_backup = cec.SaveValues;
            cec.SaveValues = false;

            var methoddes = callExpression.Function as MethodInfoFunctionDesignator;
            MethodInfo resolved_method = methoddes.Method.Method;
            var param_s = resolved_method.GetParameters();
            for (int i = 0; i < callExpression.Parameters.Count; i++)
            {
                callExpression.Parameters[i].Visit(this, cec);
#if SILVERLIGHT
                cec.InstructionsBlock.Add(new IsinstInstruction(param_s[i].ParameterType));
#endif
            }

            cec.NonVoidReturn = void_backup;
            switch (methoddes.Opcode)
            {
                case CallOpcode.Call:
                    cec.InstructionsBlock.Add(new CallInstruction(methoddes.Method));
                    break;
                case CallOpcode.Calli:
                    cec.InstructionsBlock.Add(new CalliInstruction(methoddes.Method));
                    break;
                case CallOpcode.Callvirt:
                    cec.InstructionsBlock.Add(new CallvirtInstruction(methoddes.Method));
                    break;
                default:
                    break;
            }

            if (resolved_method.ReturnType == typeof(void) && cec.NonVoidReturn)
            {
                cec.InstructionsBlock.Add(new PushInstruction(DefinedSymbols.NIL));
            }

            if (!cec.NonVoidReturn && resolved_method.ReturnType != typeof(void))
                cec.InstructionsBlock.Add(new PopInstruction());


            cec.NonVoidReturn = void_backup;
            cec.SaveValues = values_backup;
        }

        private void EmitLambdaCall(LiveLisp.Core.AST.Expressions.CallExpression callExpression, CompilationExpressionContext cec)
        {
            int argscount = callExpression.Parameters.Count;
            bool void_backup = cec.NonVoidReturn;
            cec.NonVoidReturn = true;
            bool values_backup = cec.SaveValues;
            cec.SaveValues = false;
            if (argscount < 14)
            {
                foreach (var item in callExpression.Parameters)
                {
                    item.Visit(this, cec);
                }
            }
            else
            {
                BuildArray(callExpression.Parameters, cec);
            }

            int methodid = argscount < 14 ? argscount : 15;

            cec.NonVoidReturn = void_backup;
            cec.SaveValues = values_backup;

            if (cec.NonVoidReturn)
            {
                if (cec.SaveValues)
                    cec.InstructionsBlock.Add(new CallvirtInstruction(LambdaCallEmitterHelper.ValuesInvokeMethods[methodid]));
                else
                    cec.InstructionsBlock.Add(new CallvirtInstruction(LambdaCallEmitterHelper.InvokeMethods[methodid]));
            }
            else
                cec.InstructionsBlock.Add(new CallvirtInstruction(LambdaCallEmitterHelper.VoidInvokeMethods[methodid]));
        }

        public void CatchExpression(LiveLisp.Core.AST.Expressions.CatchExpression catchExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;
            bool void_backup = cec.NonVoidReturn;

            VariableDeclaration ret_slot = null;

            InstructionsBlock instructionsBlock = cec.InstructionsBlock;
            if (void_backup)
                ret_slot = instructionsBlock.DefineLocal();

            //evalueate catch tag;
            VariableDeclaration catch_tag_slot = instructionsBlock.DefineLocal();
            cec.NonVoidReturn = true;
            catchExpression.Tag.Visit(this, cec);
            instructionsBlock.Add(new StlocInstruction(catch_tag_slot));

            //evaluate forms
            cec.NonVoidReturn = false;

            LabelDeclaration ExcExitLabel = new LabelDeclaration(instructionsBlock.NewUniqueLabelName());
            instructionsBlock.Add(new BeginExceptionBlock(ExcExitLabel));

            for (int i = 0; i < catchExpression.Count - 1; i++)
            {
                catchExpression.Forms[i].Visit(this, context);
            }

            cec.NonVoidReturn = void_backup;

            catchExpression.Forms[catchExpression.Count - 1].Visit(this, context);
            instructionsBlock.Add(new StlocInstruction(ret_slot));

            //catch the exception
            instructionsBlock.Add(new BeginCatchBlock(typeof(CatchThrowException)));
            VariableDeclaration exc_slot = instructionsBlock.DefineLocal(typeof(CatchThrowException));
            instructionsBlock.Add(new StlocInstruction(exc_slot));

            #region tag check
            var temp = instructionsBlock.DefineLocal();

            var elselabel = instructionsBlock.DefineLabel();
            // var endlabel = instructionsBlock.DefineLabel();

            instructionsBlock.Add(new LdlocInstruction(catch_tag_slot));
            instructionsBlock.Add(new LdlocInstruction(exc_slot));
            instructionsBlock.Add(new LdfldInstruction(CatchThrowException.TagField));
            instructionsBlock.Add(new CallInstruction(RuntimeHelpers.ReferenceEqualsMethod));
            instructionsBlock.Add(new BrfalseInstruction(elselabel));
            instructionsBlock.Add(new LdlocInstruction(exc_slot));
            instructionsBlock.Add(new LdfldInstruction(CatchThrowException.ResultField));
            instructionsBlock.Add(new StlocInstruction(ret_slot));
            instructionsBlock.Add(new LeaveInstruction(ExcExitLabel.Name));
            instructionsBlock.Add((new MarkLabelInstruction(elselabel)));
            instructionsBlock.Add(new RethrowInstruction());
            instructionsBlock.Add(new EndExceptionBlock());
            #endregion

            if (void_backup)
                instructionsBlock.Add(new LdlocInstruction(ret_slot));

            cec.NonVoidReturn = void_backup;
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

        public void ConstantExpression(ConstantExpression constantExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;
            if (cec.NonVoidReturn)
            {
                GenerateConstant(constantExpression.Constant, constantExpression.Constant.BoxType(), cec);
            }
        }

        private static void GenerateConstant(object constant, Type boxType, CompilationExpressionContext cec)
        {
            var cmanager = cec.ConstantsManager;

            if (cmanager == null || constant is int || constant == DefinedSymbols.T || constant == DefinedSymbols.NIL)
            {
                cec.InstructionsBlock.Add(new PushInstruction(constant));

                if (!cec.NoBoxing)
                {
                    cec.InstructionsBlock.Add(new LboxInstruction(StaticTypeResolver.Create(boxType)));
                }
            }
            else
            {
                Slot slot;
                if (!cec.NoBoxing)
                {
                    slot = cmanager.GetSlot(constant, boxType);
                }
                else
                {
                    slot = cmanager.GetSlot(constant, null);
                }

                slot.EmitGet(cec.InstructionsBlock);
            }
        }

        public void EvalWhenExpression(LiveLisp.Core.AST.Expressions.EvalWhenExpression evalWhenExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void FletExpression(LiveLisp.Core.AST.Expressions.FletExpression fletExpression, ExpressionContext context)
        {
            CompilationExpressionContext out_cec = context as CompilationExpressionContext;
            CompilationExpressionContext cec = new CompilationExpressionContext(out_cec);
            for (int i = 0; i < fletExpression.Lambdas.Count; i++)
            {
                LambdaFunctionDesignator lambda = fletExpression.Lambdas[i];

                ClassDeclBasedTypeResolver lambda_type = GenerateNewLambda(lambda, out_cec) as ClassDeclBasedTypeResolver;

                FieldDeclaration singletonField = new FieldDeclaration(FieldAttributes.Public | FieldAttributes.Static, lambda_type, lambda_type._class_decl.Name + "$instance");
                lambda_type._class_decl.AddField(singletonField);

                MethodDeclaration init_Method = new MethodDeclaration();
                init_Method.Name = "CreateInstance";
                init_Method.Attributes = MethodAttributes.Public | MethodAttributes.Static;
                init_Method.ReturnType = typeof(void);
                init_Method.Instructions.Add(new NewobjInstruction(lambda_type._class_decl.Constructors[0]));
                init_Method.Instructions.Add(new StsfldInstruction(singletonField));
                init_Method.Instructions.Add(new RetInstruction());
                lambda_type._class_decl.Methods.Add(init_Method);

                out_cec.InstructionsBlock.Add(new CallInstruction(init_Method));

                cec.AddLocalFunction(lambda.Name, new FieldSlot(null, singletonField));
            }

            cec.NonVoidReturn = false;
            for (int i = 0; i < fletExpression.Forms.Count - 1; i++)
            {
                fletExpression.Forms[i].Visit(this, cec);
            }

            cec.NonVoidReturn = out_cec.NonVoidReturn;
            fletExpression.Forms[fletExpression.Forms.Count - 1].Visit(this, cec);
        }

        public void FunctionExpression(LiveLisp.Core.AST.Expressions.FunctionExpression functionExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;
            switch (functionExpression.DesignatorType)
            {
                case FunctionNameDesignatorType.Symbol:
                    GenerateSymbolFunctionLoading(functionExpression.Designator as SymbolFunctionDesignator, context as CompilationExpressionContext);
                    break;
                case FunctionNameDesignatorType.Lambda:
                    InstantiateNewLambda(functionExpression.Designator as LambdaFunctionDesignator, cec);
                    break;
                case FunctionNameDesignatorType.MethodInfo:
                    var lambda_type = GenerateNewLambda(functionExpression.Designator as MethodInfoFunctionDesignator, context as CompilationExpressionContext);
                    cec.InstructionsBlock.Add(new NewobjInstruction(lambda_type));
                    break;
            }
        }

        private void InstantiateNewLambda(LambdaFunctionDesignator lambda, CompilationExpressionContext cec)
        {
            StaticTypeResolver lambda_type = GenerateNewLambda(lambda, cec);

            ClosureClientContext ccc = cec.GetClosureClientContext(lambda);

            StaticTypeResolver[] args = null;
            if (ccc != null)
            {
                args = new StaticTypeResolver[ccc.UsedContainers.Count];
                for (int i = 0; i < ccc.UsedContainers.Count; i++)
                {
                    cec.GetSlotForClosureContainer(ccc.UsedContainers[i]).EmitGet(cec.InstructionsBlock);
                    args[i] = ccc.UsedContainers[i].EnvType;
                }
            }

            cec.InstructionsBlock.Add(new NewobjInstruction(StaticConstructorResolver.Create(lambda_type, args)));
        }

        private void GenerateSymbolFunctionLoading(SymbolFunctionDesignator symbolFunctionDesignator, CompilationExpressionContext compilationExpressionContext)
        {
            Slot function_slot = compilationExpressionContext.GetLocalFunction(symbolFunctionDesignator.Name);
            if (function_slot == null)
            {
                GenerateConstant(symbolFunctionDesignator.Name, typeof(Symbol), compilationExpressionContext);
                compilationExpressionContext.InstructionsBlock.Add(new CallvirtInstruction(StaticMethodResolver.Create(typeof(Symbol).GetMethod("get_Function"))));
            }
            else
            {
                function_slot.EmitGet(compilationExpressionContext.InstructionsBlock);
            }
        }

        private StaticTypeResolver GenerateNewLambda(LambdaFunctionDesignator lambdaFunctionDesignator, CompilationExpressionContext compilationExpressionContext)
        {
            var cec = new CompilationExpressionContext(compilationExpressionContext);

            // ClosureProvider - конструкция создающая новые связи которые замыкаются
            //
            // ClosureClient - лексически вложенная в ClosureProvider конструкция захватившая одну или несколько
            // переменных
            //

            // в случае вложенных лямбд эта функция вызывается несколько раз (потому что эмитируются перегрузки)
            if (cec.ContainsLambda(lambdaFunctionDesignator))
                return cec.GetLambdaType(lambdaFunctionDesignator);

            ClassDeclaration lambda_holder = new ClassDeclaration();

            cec.ConstantsManager = new ClassScopedConstantManager(lambda_holder);

            lambda_holder.Name = Helpers.new_ClassName(lambdaFunctionDesignator.Name.ToString(true));

            lambda_holder.Attributes = TypeAttributes.Public;
            lambda_holder.BaseType = typeof(FullLispFunction);
            ApplyDeclarations(lambdaFunctionDesignator, cec);

            /*
             * ClosuresManager clcontext = outer_context.ClosuresManager;
            if (clcontext == null)
                clcontext = new ClosuresWalker(letStarExpression, forms_context).Walk();

            forms_context.ClosuresManager = clcontext;

            ClosureContainerContext currentCCC = clcontext.GetContainer(letStarExpression);

            bool isClosureContainer = false;
            Slot closureEnvInstanceSlot = null;
            Dictionary<Symbol, FieldSlot> closureSlots = null;

            if (currentCCC != null)
            {
                isClosureContainer = true;
                closureEnvInstanceSlot = new LocalSlot(forms_context.InstructionsBlock.DefineLocal(currentCCC.EnvType));

                closureSlots = new Dictionary<Symbol, FieldSlot>();

                for (int i = 0; i < currentCCC.ClosedOverVars.Count; i++)
                {
                    FieldSlot cs = new FieldSlot(closureEnvInstanceSlot, currentCCC.ClosedOverVars[i].Field);
                    closureSlots.Add(currentCCC.ClosedOverVars[i].Name, cs);
                }

                ClassDeclBasedTypeResolver entype = currentCCC.EnvType as ClassDeclBasedTypeResolver;
                if (entype._class_decl.CreatedType == null)
                {
                    bindings_context.CompilationContext.AddClassDeclaration(entype._class_decl);
                }

                instructionsBlock.Add(new NewobjInstruction(currentCCC.EnvType));
                closureEnvInstanceSlot.EmitSet(instructionsBlock);

                bindings_context.SetSlotForClosureContainer(currentCCC, closureEnvInstanceSlot);
            }

             */

            // мы клиент лексического замыкания то нужно создавать особоый конструктор - который в качестве 
            // параметра будет принимать экзмепляр захваченного лексического окружения

            ConstructorDeclaration lambda_ctor;
            ClosureClientContext ccc = cec.GetClosureClientContext(lambdaFunctionDesignator);;
            // if(ccc==null) isClosureClient = false;
            if (ccc!= null)
            {
                Dictionary<ClosureContainerContext, FieldDeclaration> envfields = new Dictionary<ClosureContainerContext, FieldDeclaration>();
                for (int i = 0; i < ccc.UsedContainers.Count; i++)
                {
                    FieldDeclaration envfield = lambda_holder.NewGeneratedField("Closure", ccc.UsedContainers[i].EnvType);
                    envfields.Add(ccc.UsedContainers[i], envfield);
                    cec.SetSlotForClosureContainer(ccc.UsedContainers[i], new FieldSlot(new MethodParameterSlot(0), envfield));
                }

                // setup Slots;

                for (int i = 0; i <ccc.ClosedOverVars.Count; i++)
                {
                    FieldSlot cvs = new FieldSlot(new FieldSlot(new MethodParameterSlot(0), envfields[ccc.ClosedOverVars[i].Container]), ccc.ClosedOverVars[i].Field);

                    cec.AddSlot(ccc.ClosedOverVars[i].Name, cvs);
                }

                // ctor
                lambda_ctor = GenerateClosureConstructor(envfields, lambdaFunctionDesignator);
            }
            else
            {
                lambda_ctor = GenerateConstructor(lambdaFunctionDesignator);
            }
            
            lambda_holder.Constructors.Add(lambda_ctor);

            lambda_holder.Methods.AddRange(GenerateValuesOverloads(lambdaFunctionDesignator, cec));

            lambda_holder.Methods.AddRange(GenerateSingleValueOverloads(lambdaFunctionDesignator, cec));

            lambda_holder.Methods.AddRange(GenerateVoidOverloads(lambdaFunctionDesignator, cec));

            compilationExpressionContext.CompilationContext.AddClassDeclaration(lambda_holder);

            var resolver = StaticTypeResolver.Create(lambda_holder);

            cec.AddLambdaType(lambdaFunctionDesignator, resolver);

            return resolver;
        }

        private static ConstructorDeclaration GenerateConstructor(LambdaFunctionDesignator lambdaFunctionDesignator)
        {
            ConstructorDeclaration default_constructor = new ConstructorDeclaration();
            default_constructor.Attributes = MethodAttributes.Public;
            InstructionsBlock instructionsBlock = default_constructor.Instructions;
            instructionsBlock.Add(new LdargInstruction(0));
            instructionsBlock.Add(new LdstrInstruction(lambdaFunctionDesignator.Name.ToString(true)));
            instructionsBlock.Add(new CallInstruction(typeof(FullLispFunction).GetConstructor(new Type[] { typeof(string) })));

            instructionsBlock.Add(new LdargInstruction(0));
            instructionsBlock.Add(new Ldc_I4Instruction(lambdaFunctionDesignator.LambdaList.MinParamsCount));
            instructionsBlock.Add(new StfldInstruction(LambdaCallEmitterHelper.MinArgsField));
            instructionsBlock.Add(new LdargInstruction(0));
            instructionsBlock.Add(new Ldc_I4Instruction(lambdaFunctionDesignator.LambdaList.MaxParamsCount));
            instructionsBlock.Add(new StfldInstruction(LambdaCallEmitterHelper.MaxArgsField));


            instructionsBlock.Add(new RetInstruction());
            return default_constructor;
        }

        private ConstructorDeclaration GenerateClosureConstructor(Dictionary<ClosureContainerContext, FieldDeclaration> envfields, LambdaFunctionDesignator lambdaFunctionDesignator)
        {
            ConstructorDeclaration default_constructor = new ConstructorDeclaration();
            foreach (var item in envfields)
            {
                default_constructor.Args.AddNewAutoGenerated("ClosureEnv", item.Key.EnvType);
            }
            default_constructor.Attributes = MethodAttributes.Public;
            InstructionsBlock instructioinsBlock = default_constructor.Instructions;
            instructioinsBlock.Add(new LdargInstruction(0));
            instructioinsBlock.Add(new LdstrInstruction(lambdaFunctionDesignator.Name.ToString(true)));
            instructioinsBlock.Add(new CallInstruction(typeof(FullLispFunction).GetConstructor(new Type[] { typeof(string) })));

            instructioinsBlock.Add(new LdargInstruction(0));
            instructioinsBlock.Add(new Ldc_I4Instruction(lambdaFunctionDesignator.LambdaList.MinParamsCount));
            instructioinsBlock.Add(new StfldInstruction(LambdaCallEmitterHelper.MinArgsField));
            instructioinsBlock.Add(new LdargInstruction(0));
            instructioinsBlock.Add(new Ldc_I4Instruction(lambdaFunctionDesignator.LambdaList.MaxParamsCount));
            instructioinsBlock.Add(new StfldInstruction(LambdaCallEmitterHelper.MaxArgsField));

            // setup closure env
            int i = 1;
            foreach (var item in envfields)
            {
                instructioinsBlock.Add(new LdargInstruction(0));
                instructioinsBlock.Add(new LdargInstruction(i));
                instructioinsBlock.Add(new StfldInstruction(item.Value));
                i++;
            }


            instructioinsBlock.Add(new RetInstruction());
            return default_constructor;
        }

        private IEnumerable<MethodDeclaration> GenerateValuesOverloads(LambdaFunctionDesignator lambdaFunctionDesignator, CompilationExpressionContext compilationExpressionContext)
        {
            List<MethodDeclaration> overrides = new List<MethodDeclaration>();

            LambdaList ll = lambdaFunctionDesignator.LambdaList;
            int maxExpandedArgs = LispFunction.MAX_NONPARAMS_ARGS < ll.MaxParamsCount ? LispFunction.MAX_NONPARAMS_ARGS : ll.MaxParamsCount;


            if (ll.MinParamsCount == ll.MaxParamsCount)
            {
                MethodDeclaration method_decl = new MethodDeclaration();
                method_decl.Name = "ValuesInvoke";
                method_decl.Attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;
                FillArgs(method_decl, ll, ll.MaxParamsCount);
                method_decl.ReturnType = typeof(object);

                CreateValuesOverload(method_decl.Instructions, lambdaFunctionDesignator, ll.MaxParamsCount, compilationExpressionContext);

                overrides.Add(method_decl);
            }
            else
            {
                for (int i = ll.MinParamsCount; i < maxExpandedArgs; i++)
                {
                    MethodDeclaration method_decl = new MethodDeclaration();
                    method_decl.Name = "ValuesInvoke";
                    method_decl.Attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;
                    FillArgs(method_decl, ll, i);
                    method_decl.ReturnType = typeof(object);

                    CreateValuesOverload(method_decl.Instructions, lambdaFunctionDesignator, i, compilationExpressionContext);

                    overrides.Add(method_decl);

                }
            }

            MethodDeclaration args_method_decl = new MethodDeclaration();
            args_method_decl.Name = "ValuesInvoke";
            args_method_decl.Attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;
            args_method_decl.Args.Add(new ParameterDeclaration("args", typeof(object[])));
            args_method_decl.ReturnType = typeof(object);
            CreateValuesArgsOverload(args_method_decl.Instructions, lambdaFunctionDesignator, compilationExpressionContext);

            overrides.Add(args_method_decl);
            return overrides;
        }

        private IEnumerable<MethodDeclaration> GenerateSingleValueOverloads(LambdaFunctionDesignator lambdaFunctionDesignator, CompilationExpressionContext compilationExpressionContext)
        {
            List<MethodDeclaration> overrides = new List<MethodDeclaration>();

            LambdaList ll = lambdaFunctionDesignator.LambdaList;
            int maxExpandedArgs = LispFunction.MAX_NONPARAMS_ARGS < ll.MaxParamsCount ? LispFunction.MAX_NONPARAMS_ARGS : ll.MaxParamsCount;


            if (ll.MinParamsCount == ll.MaxParamsCount)
            {
                MethodDeclaration method_decl = new MethodDeclaration();
                method_decl.Name = "Invoke";
                method_decl.Attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;
                FillArgs(method_decl, ll, ll.MaxParamsCount);
                method_decl.ReturnType = typeof(object);

                CreateSingleValueOverload(method_decl.Instructions, lambdaFunctionDesignator, ll.MaxParamsCount, compilationExpressionContext);

                overrides.Add(method_decl);
            }
            else
            {
                for (int i = ll.MinParamsCount; i < maxExpandedArgs; i++)
                {
                    MethodDeclaration method_decl = new MethodDeclaration();
                    method_decl.Name = "Invoke";
                    method_decl.Attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;
                    FillArgs(method_decl, ll, i);
                    method_decl.ReturnType = typeof(object);

                    CreateSingleValueOverload(method_decl.Instructions, lambdaFunctionDesignator, i, compilationExpressionContext);

                    overrides.Add(method_decl);

                }
            }

            MethodDeclaration args_method_decl = new MethodDeclaration();
            args_method_decl.Name = "Invoke";
            args_method_decl.Attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;
            args_method_decl.Args.Add(new ParameterDeclaration("args", typeof(object[])));
            args_method_decl.ReturnType = typeof(object);
            CreateSingleValueArgsOverload(args_method_decl.Instructions, lambdaFunctionDesignator, compilationExpressionContext);

            overrides.Add(args_method_decl);
            return overrides;
        }

        private IEnumerable<MethodDeclaration> GenerateVoidOverloads(LambdaFunctionDesignator lambdaFunctionDesignator, CompilationExpressionContext compilationExpressionContext)
        {
            List<MethodDeclaration> overrides = new List<MethodDeclaration>();

            LambdaList ll = lambdaFunctionDesignator.LambdaList;
            int maxExpandedArgs = LispFunction.MAX_NONPARAMS_ARGS < ll.MaxParamsCount ? LispFunction.MAX_NONPARAMS_ARGS : ll.MaxParamsCount;


            if (ll.MinParamsCount == ll.MaxParamsCount)
            {
                MethodDeclaration method_decl = new MethodDeclaration();
                method_decl.Name = "VoidInvoke";
                method_decl.Attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;
                FillArgs(method_decl, ll, ll.MaxParamsCount);
                method_decl.ReturnType = typeof(void);

                CreateVoidOverload(method_decl.Instructions, lambdaFunctionDesignator, ll.MaxParamsCount, compilationExpressionContext);

                overrides.Add(method_decl);
            }
            else
            {
                for (int i = ll.MinParamsCount; i < maxExpandedArgs; i++)
                {
                    MethodDeclaration method_decl = new MethodDeclaration();
                    method_decl.Name = "VoidInvoke";
                    method_decl.Attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;
                    FillArgs(method_decl, ll, i);
                    method_decl.ReturnType = typeof(void);

                    CreateVoidOverload(method_decl.Instructions, lambdaFunctionDesignator, i, compilationExpressionContext);

                    overrides.Add(method_decl);

                }
            }

            MethodDeclaration args_method_decl = new MethodDeclaration();
            args_method_decl.Name = "VoidInvoke";
            args_method_decl.Attributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final;
            args_method_decl.Args.Add(new ParameterDeclaration("args", typeof(object[])));
            args_method_decl.ReturnType = typeof(void);
            CreateVoidArgsOverload(args_method_decl.Instructions, lambdaFunctionDesignator, compilationExpressionContext);

            overrides.Add(args_method_decl);
            return overrides;
        }

        private void FillArgs(MethodDeclaration method_decl, LambdaList ll, int i)
        {
            int p = 0;
            for (; p < ll.Count && p < i; p++)
            {
                method_decl.Args.Add(new ParameterDeclaration(ll[p].Name.ToString(true)));
            }

            int params_or_rest_index = ll.ParamsIndex;

            if (params_or_rest_index != -1)
            {
                string base_name = ll[params_or_rest_index].Name.Name;

                if (i > ll.Count)
                {
                    int counter = 1;
                    for (; p < i; p++)
                    {
                        method_decl.Args.Add(new ParameterDeclaration(base_name + "$" + counter++));
                    }
                }
            }
        }

        private void CreateValuesOverload(InstructionsBlock instructionsBlock, LambdaFunctionDesignator lambdaFunctionDesignator, int i, CompilationExpressionContext compilationExpressionContext)
        {
            bool values_backup = compilationExpressionContext.SaveValues;
            compilationExpressionContext.SaveValues = true;
            bool void_backup = compilationExpressionContext.NonVoidReturn;
            compilationExpressionContext.NonVoidReturn = true;
            EmitOverload(instructionsBlock, lambdaFunctionDesignator, i, compilationExpressionContext);
            compilationExpressionContext.SaveValues = values_backup;
            compilationExpressionContext.NonVoidReturn = void_backup;
        }

        private void EmitOverload(InstructionsBlock instructionsBlock, LambdaFunctionDesignator lambdaFunctionDesignator, int i, CompilationExpressionContext compilationExpressionContext)
        {
            var cec = new CompilationExpressionContext(compilationExpressionContext);
            ApplyDeclarations(lambdaFunctionDesignator, cec);
            InstructionsBlock specials_init_instr = new InstructionsBlock();
            InstructionsBlock specials_rebind_instr = new InstructionsBlock();
            bool void_backup = cec.NonVoidReturn;
            cec.InstructionsBlock = instructionsBlock;
            cec.NonVoidReturn = true;
            Slot rest_slot = null;
            Slot sup_slot = null;
            OptionalParameter optional;
            Slot slot;
            int p = 0;
            List<KeyParameter> keys = new List<KeyParameter>();
            for (; p < i && p < lambdaFunctionDesignator.LambdaList.Count; p++)
            {
                var parameter = lambdaFunctionDesignator.LambdaList[p];
                int arg_slot = p + 1;

                bool special = false;
                switch (parameter.Kind)
                {
                    case ParameterKind.Required:
                        if (cec.IsSpecial(parameter.Name))
                        {
                            AddSpecialParameter(instructionsBlock, specials_init_instr, specials_rebind_instr, parameter, new LdargInstruction(arg_slot));
                        }
                        else
                        {
                            cec.AddSlot(parameter.Name, new MethodParameterSlot(arg_slot));
                        }
                        break;
                    case ParameterKind.Optional:
                        if (cec.IsSpecial(parameter.Name))
                        {
                            AddSpecialParameter(instructionsBlock, specials_init_instr, specials_rebind_instr, parameter, new LdargInstruction(arg_slot));
                        }
                        else
                        {
                            cec.AddSlot(parameter.Name, new MethodParameterSlot(arg_slot));
                        }
                        optional = parameter as OptionalParameter;
                        if (optional.Suppliedp)
                        {
                            if (cec.IsSpecial(optional.SuppliedpName))
                            {
                                AddSpecialParameter(instructionsBlock, specials_init_instr, specials_rebind_instr, parameter, new PushInstruction(DefinedSymbols.T));
                            }
                            else
                            {
                                sup_slot = cec.AddSlot(optional.SuppliedpName, new LocalSlot(instructionsBlock.DefineLocal().Id));
                                instructionsBlock.Add(new PushInstruction(DefinedSymbols.T));
                                sup_slot.EmitSet(instructionsBlock);
                            }
                        }
                        break;
                    case ParameterKind.Rest:
                        var currentinstructionsBlock = instructionsBlock;
                        if (cec.IsSpecial(parameter.Name))
                        {
                            rest_slot = new LocalSlot(instructionsBlock.DefineLocal());
                            special = true;
                            currentinstructionsBlock = specials_init_instr;
                        }
                        else
                        {
                            special = false;
                            rest_slot = cec.AddSlot(parameter.Name, new LocalSlot(instructionsBlock.DefineLocal().Id));
                        }
                        rest_slot.EmitSetProlog(currentinstructionsBlock);
                        currentinstructionsBlock.Add(new LdargInstruction(arg_slot));
                        currentinstructionsBlock.Add(new NewobjInstruction(StaticConstructorResolver.Create(typeof(Cons), typeof(Object))));
                        rest_slot.EmitSet(currentinstructionsBlock);

                        for (int r = p + 1; r < i; r++)
                        {
                            rest_slot.EmitGet(currentinstructionsBlock);

                    currentinstructionsBlock.Add(new IsinstInstruction(typeof(Cons)));

                            currentinstructionsBlock.Add(new LdargInstruction(r + 1));
                            currentinstructionsBlock.Add(new CallInstruction(typeof(Cons).GetMethod("Append")));
                        }

                        if (special)
                        {
                            AddSpecialParameter(instructionsBlock, specials_init_instr, specials_rebind_instr, parameter, new LdlocInstruction((rest_slot as LocalSlot).Slot_num));
                        }
                        continue;
                    case ParameterKind.Key:
                        keys.Add(parameter as KeyParameter);
                        if (rest_slot == null)
                        {
                            rest_slot = cec.AddSlot(new Symbol("rest_keys"), new LocalSlot(instructionsBlock.DefineLocal().Id));
                            rest_slot.EmitSetProlog(instructionsBlock);
                            instructionsBlock.Add(new LdargInstruction(arg_slot));
                            instructionsBlock.Add(new NewobjInstruction(StaticConstructorResolver.Create(typeof(Cons), typeof(Object))));
                            rest_slot.EmitSet(instructionsBlock);

                            for (int r = p + 1; r < i; r++)
                            {
                                rest_slot.EmitGet(instructionsBlock);

                    instructionsBlock.Add(new IsinstInstruction(typeof(Cons)));

                                instructionsBlock.Add(new LdargInstruction(r + 1));
                                instructionsBlock.Add(new CallInstruction(typeof(Cons).GetMethod("Append")));
                            }
                        }
                        break;
                    case ParameterKind.Params:
                        currentinstructionsBlock = instructionsBlock;
                        if (cec.IsSpecial(parameter.Name))
                        {
                            slot = new LocalSlot(instructionsBlock.DefineLocal(typeof(object[])));
                            special = true;
                            currentinstructionsBlock = specials_init_instr;
                        }
                        else
                        {
                            special = false;
                            slot = cec.AddSlot(parameter.Name, new LocalSlot(cec.InstructionsBlock.DefineLocal(typeof(object[])).Id));
                        }

                        slot.EmitSetProlog(currentinstructionsBlock);
                        currentinstructionsBlock.Add(new Ldc_I4Instruction(i - p));
                        currentinstructionsBlock.Add(new NewarrInstruction(typeof(object)));
                        slot.EmitSet(currentinstructionsBlock);

                        slot.EmitGet(currentinstructionsBlock);

                      //  currentinstructionsBlock.Add(new IsinstInstruction(typeof(object[])));

                        currentinstructionsBlock.Add(new Ldc_I4Instruction(0));
                        currentinstructionsBlock.Add(new LdargInstruction(arg_slot));
                        currentinstructionsBlock.Add(new StelemInstruction(typeof(object)));


                        for (int r = p + 1; r < i; r++)
                        {
                            slot.EmitGet(currentinstructionsBlock);

                        currentinstructionsBlock.Add(new IsinstInstruction(typeof(object[])));

                            currentinstructionsBlock.Add(new Ldc_I4Instruction(r - p));
                            currentinstructionsBlock.Add(new LdargInstruction(r + 1));
                            currentinstructionsBlock.Add(new StelemInstruction(typeof(object)));

                        }

                        if (special)
                        {
                            AddSpecialParameter(instructionsBlock, specials_init_instr, specials_rebind_instr, parameter, new LdlocInstruction((slot as LocalSlot).Slot_num));
                        }
                        break;
                    default:
                        break;
                }
            }

            if (i < lambdaFunctionDesignator.LambdaList.Count)
            {
                for (; p < lambdaFunctionDesignator.LambdaList.Count; p++)
                {
                    var parameter = lambdaFunctionDesignator.LambdaList[p];
                    int arg_slot = p + 1;
                    switch (parameter.Kind)
                    {
                        case ParameterKind.Required:
                            throw new InvalidOperationException(); // should never throw
                        //      break;
                        case ParameterKind.Optional:
                         //   if (cec.IsSpecial(parameter.Name))
                         //   {
                         //       AddSpecialParameter(instructionsBlock, specials_init_instr, specials_rebind_instr, parameter,)
                         //   }
                            slot = cec.AddSlot(parameter.Name, new LocalSlot(cec.InstructionsBlock.DefineLocal().Id));
                            slot.EmitSetProlog(instructionsBlock);
                            optional = parameter as OptionalParameter;
                            optional.DefaultValue.Visit(this, cec);
                            slot.EmitSet(instructionsBlock);

                            if (optional.Suppliedp)
                            {
                                sup_slot = cec.AddSlot(optional.SuppliedpName, new LocalSlot(instructionsBlock.DefineLocal().Id));
                                instructionsBlock.Add(new PushInstruction(DefinedSymbols.NIL));
                                sup_slot.EmitSet(instructionsBlock);
                            }
                            break;
                        case ParameterKind.Rest:
                            rest_slot = cec.AddSlot(parameter.Name, new LocalSlot(cec.InstructionsBlock.DefineLocal().Id));
                            rest_slot.EmitSetProlog(instructionsBlock);
                            instructionsBlock.Add(new PushInstruction(DefinedSymbols.NIL));
                            rest_slot.EmitSet(instructionsBlock);
                            break;
                        case ParameterKind.Key:
                            if (rest_slot == null)
                            {
                                slot = cec.AddSlot(parameter.Name, new LocalSlot(cec.InstructionsBlock.DefineLocal().Id));
                                slot.EmitSetProlog(instructionsBlock);
                                KeyParameter key = parameter as KeyParameter;
                                key.DefaultValue.Visit(this, cec);
                                slot.EmitSet(instructionsBlock);

                                if (key.Suppliedp)
                                {
                                    sup_slot = cec.AddSlot(key.SuppliedpName, new LocalSlot(instructionsBlock.DefineLocal().Id));
                                    instructionsBlock.Add(new PushInstruction(DefinedSymbols.NIL));
                                    sup_slot.EmitSet(instructionsBlock);
                                }
                            }
                            else
                            {
                                keys.Add(parameter as KeyParameter);
                            }
                            break;
                        case ParameterKind.Params:
                            slot = cec.AddSlot(parameter.Name, new LocalSlot(cec.InstructionsBlock.DefineLocal(typeof(object[])).Id));
                            slot.EmitSetProlog(instructionsBlock);
                            instructionsBlock.Add(new Ldc_I4Instruction(0));
                            instructionsBlock.Add(new NewarrInstruction(typeof(object)));
                            slot.EmitSet(instructionsBlock);
                            break;
                        default:
                            break;
                    }
                }
            }

            EmitKeyParams(lambdaFunctionDesignator, cec, rest_slot, keys);

            cec.NonVoidReturn = void_backup;

            for (int j = 0; j < lambdaFunctionDesignator.Forms.Count - 1; j++)
            {
                lambdaFunctionDesignator.Forms[j].Visit(this, cec);
            }

            lambdaFunctionDesignator.Forms[lambdaFunctionDesignator.Forms.Count - 1].Visit(this, cec);
            //  EmitMultipleValuesCheck(instructionsBlock);
            //    if (cec.NonVoidReturn)
            instructionsBlock.Add(new RetInstruction());
        }

        private static void AddSpecialParameter(InstructionsBlock instructionsBlock, InstructionsBlock specials_init_instr, InstructionsBlock specials_rebind_instr, LambdaParameter parameter, ILInstruction specialInitistraction)
        {
            // backup
            VariableDeclaration backup = instructionsBlock.DefineLocal();
            GlobalRawSlot gs = new GlobalRawSlot(parameter.Name);
            gs.EmitGet(instructionsBlock);
            instructionsBlock.Add(new StlocInstruction(backup));

            // set
            gs.EmitSetProlog(specials_init_instr);
            specials_init_instr.Add(specialInitistraction);
            gs.EmitSet(specials_init_instr);

            // restore
            gs.EmitSetProlog(specials_rebind_instr);
            specials_rebind_instr.Add(new LdlocInstruction(backup));
            gs.EmitSet(specials_rebind_instr);
        }

        private static void EmitMultipleValuesCheck(InstructionsBlock instructionsBlock)
        {
            var temp = instructionsBlock.DefineLocal();
            var vallocal = instructionsBlock.DefineLocal(typeof(MultipleValuesContainer));

            var nulllabel = instructionsBlock.DefineLabel();
            var endlabel = instructionsBlock.DefineLabel();

            instructionsBlock.Add(new StlocInstruction(temp));
            instructionsBlock.Add(new LdlocInstruction(temp));
            instructionsBlock.Add(new IsinstInstruction(typeof(MultipleValuesContainer)));
            instructionsBlock.Add(new StlocInstruction(vallocal));
            instructionsBlock.Add(new LdlocInstruction(vallocal));
            instructionsBlock.Add(new BrfalseInstruction(nulllabel));
            instructionsBlock.Add(new LdlocInstruction(vallocal));
            instructionsBlock.Add(new BrInstruction(endlabel));
            instructionsBlock.Add((new MarkLabelInstruction(nulllabel)));
            instructionsBlock.Add(new LdlocInstruction(temp));
            instructionsBlock.Add(new CallInstruction(MultipleValuesContainer.Values1Ctor));
            instructionsBlock.Add(new MarkLabelInstruction(endlabel));
        }

        private void EmitKeyParams(LambdaFunctionDesignator lambdaFunctionDesignator, CompilationExpressionContext cec, Slot rest_slot, List<KeyParameter> keys)
        {
            InstructionsBlock instructionsBlock = cec.InstructionsBlock;
            Slot sup_slot = null;
            Slot key_pairs_slot = null;
            Slot slot = null;
            KeyParameter key = null;
            if (keys.Count != 0)
            {
                instructionsBlock.Add(new PushInstruction(lambdaFunctionDesignator.Name));
                rest_slot.EmitGet(instructionsBlock);
                instructionsBlock.Add(new PushInstruction(lambdaFunctionDesignator.LambdaList.KeysList));
                instructionsBlock.Add(new PushInstruction(lambdaFunctionDesignator.LambdaList.AllowOtherKeys));
                key_pairs_slot = new LocalSlot(instructionsBlock.DefineLocal(typeof(Dictionary<int,object>)).Id);
                key_pairs_slot.EmitSetProlog(instructionsBlock);
                instructionsBlock.Add(new CallInstruction(typeof(RuntimeHelpers).GetMethod("ValidateListForKeyParameters")));
                key_pairs_slot.EmitSet(instructionsBlock);
            }

            for (int k = 0; k < keys.Count; k++)
            {
                key = keys[k];
                slot = cec.AddSlot(key.Name, new LocalSlot(instructionsBlock.DefineLocal().Id));

                if (key.Suppliedp)
                {
                    sup_slot = cec.AddSlot(key.Name, new LocalSlot(instructionsBlock.DefineLocal().Id));
                }

                key_pairs_slot.EmitGet(instructionsBlock);
                instructionsBlock.Add(new Ldc_I4Instruction(key.Keyword.Id));
                instructionsBlock.Add(new CallInstruction(typeof(Dictionary<int, object>).GetMethod("ContainsKey")));

                LabelDeclaration elseLabel = instructionsBlock.DefineLabel();
                LabelDeclaration endLabel = instructionsBlock.DefineLabel();
                instructionsBlock.Add(new BrfalseInstruction(elseLabel));

                if (key.Suppliedp)
                {
                    sup_slot.EmitSetProlog(instructionsBlock);
                    instructionsBlock.Add(new PushInstruction(DefinedSymbols.T));
                    sup_slot.EmitSet(instructionsBlock);
                }

                slot.EmitSetProlog(instructionsBlock);
                key_pairs_slot.EmitGet(instructionsBlock);
                instructionsBlock.Add(new Ldc_I4Instruction(key.Keyword.Id));
                instructionsBlock.Add(new CallInstruction(typeof(Dictionary<int, object>).GetMethod("get_Item")));
                slot.EmitSet(instructionsBlock);
                instructionsBlock.Add(new BrInstruction(endLabel));

                instructionsBlock.Add(new MarkLabelInstruction(elseLabel));

                if (key.Suppliedp)
                {
                    sup_slot.EmitSetProlog(instructionsBlock);
                    instructionsBlock.Add(new PushInstruction(DefinedSymbols.NIL));
                    sup_slot.EmitSet(instructionsBlock);
                }

                slot.EmitSetProlog(instructionsBlock);
                key.DefaultValue.Visit(this, cec);
                slot.EmitSet(instructionsBlock);
                instructionsBlock.Add(new MarkLabelInstruction(endLabel));
            }
        }

        private void CreateValuesArgsOverload(InstructionsBlock instructionsBlock, LambdaFunctionDesignator lambdaFunctionDesignator, CompilationExpressionContext compilationExpressionContext)
        {
            bool values_backup = compilationExpressionContext.SaveValues;
            compilationExpressionContext.SaveValues = true;
            bool void_backup = compilationExpressionContext.NonVoidReturn;
            compilationExpressionContext.NonVoidReturn = true;
            EmitArgsOverload(instructionsBlock, lambdaFunctionDesignator, compilationExpressionContext);
            compilationExpressionContext.SaveValues = values_backup;
            compilationExpressionContext.NonVoidReturn = void_backup;
        }

        private void EmitArgsOverload(InstructionsBlock instructionsBlock, LambdaFunctionDesignator lambdaFunctionDesignator, CompilationExpressionContext compilationExpressionContext)
        {
            var cec = new CompilationExpressionContext(compilationExpressionContext);
            cec.InstructionsBlock = instructionsBlock;
            bool void_backup = cec.NonVoidReturn;
            cec.NonVoidReturn = true;
            VariableDeclaration array_length = instructionsBlock.DefineLocal(typeof(int));
            instructionsBlock.Add(new LdargInstruction(1));
            instructionsBlock.Add(new LdlenInstruction());
            instructionsBlock.Add(new StlocInstruction(array_length));

            // check args count this.ChechArgsCount(args.Length)
            instructionsBlock.Add(new LdargInstruction(0));
            instructionsBlock.Add(new LdlocInstruction(array_length));
            instructionsBlock.Add(new CallInstruction(LambdaCallEmitterHelper.CheckArgsMethod));

            // generate args;
            LambdaList ll = lambdaFunctionDesignator.LambdaList;

            Slot slot = null;
            Slot supp = null;
            Slot rest_slot = null;
            Slot params_slot = null;
            LabelDeclaration elseLabel = null;
            LabelDeclaration endLabel = null;
            List<KeyParameter> keys = new List<KeyParameter>();
            for (int i = 0; i < ll.Count; i++)
            {
                LambdaParameter parameter = ll[i];
                OptionalParameter optional;

                switch (parameter.Kind)
                {
                    case ParameterKind.Required:
                        slot = cec.AddSlot(parameter.Name, new LocalSlot(instructionsBlock.DefineLocal().Id));
                        instructionsBlock.Add(new LdargInstruction(1));
                        instructionsBlock.Add(new Ldc_I4Instruction(parameter.Position));
                        instructionsBlock.Add(new LdelemInstruction(typeof(object)));
                        slot.EmitSet(instructionsBlock);
                        break;
                    case ParameterKind.Optional:
                        slot = cec.AddSlot(parameter.Name, new LocalSlot(instructionsBlock.DefineLocal().Id));
                        optional = parameter as OptionalParameter;

                        elseLabel = instructionsBlock.DefineLabel();
                        endLabel = instructionsBlock.DefineLabel();
                        /*
                         * if(args.Length > parameter.Position)
                         * {
                         *  parameterSlot<-args[parameter.Position];
                         *  parameterSuppliedp<-DefinedSymbols.T;
                         * }
                         * else
                         * {
                         * parameterSlot<-parameterDefaultValue;
                         * parameterSuppliedp<-DefinedSymbols.Nil;
                         * }
                         */
                        instructionsBlock.Add(new LdlocInstruction(array_length));
                        instructionsBlock.Add(new Ldc_I4Instruction(parameter.Position));
                        instructionsBlock.Add(new BleInstruction(elseLabel.Name));
                        instructionsBlock.Add(new LdargInstruction(1));
                        instructionsBlock.Add(new Ldc_I4Instruction(parameter.Position));
                        instructionsBlock.Add(new LdelemInstruction(typeof(object)));
                        slot.EmitSet(instructionsBlock);
                        if (optional.Suppliedp)
                        {
                            supp = cec.AddSlot(optional.SuppliedpName, new LocalSlot(instructionsBlock.DefineLocal().Id));
                            instructionsBlock.Add(new PushInstruction(DefinedSymbols.T));
                            supp.EmitSet(instructionsBlock);
                        }
                        instructionsBlock.Add(new BrInstruction(endLabel.Name));
                        instructionsBlock.Add(new MarkLabelInstruction(elseLabel));
                        optional.DefaultValue.Visit(this, cec);
                        slot.EmitSet(instructionsBlock);
                        if (optional.Suppliedp)
                        {
                            //    supp = cec.AddSlot(optional.SuppliedpName, new LocalSlot(instructionsBlock.DefineLocal().Id));
                            instructionsBlock.Add(new PushInstruction(DefinedSymbols.NIL));
                            supp.EmitSet(instructionsBlock);
                        }
                        instructionsBlock.Add(new MarkLabelInstruction(endLabel));
                        break;
                    case ParameterKind.Rest:
                        rest_slot = EmitArgsFillRestSlot(cec, array_length, parameter.Name, parameter.Position);
                        break;
                    case ParameterKind.Key:
                        keys.Add(parameter as KeyParameter);
                        if (rest_slot == null)
                            rest_slot = EmitArgsFillRestSlot(cec, array_length, new Symbol("rest_slot"), parameter.Position);
                        break;
                    case ParameterKind.Params:
                        params_slot = cec.AddSlot(parameter.Name, new LocalSlot(instructionsBlock.DefineLocal(typeof(object[])).Id));

                        elseLabel = instructionsBlock.DefineLabel();
                        endLabel = instructionsBlock.DefineLabel();

                        instructionsBlock.Add(new LdlocInstruction(array_length));
                        instructionsBlock.Add(new Ldc_I4Instruction(parameter.Position));
                        instructionsBlock.Add(new BleInstruction(elseLabel.Name));

                        VariableDeclaration length = instructionsBlock.DefineLocal(typeof(int));

                        // calculate length
                        instructionsBlock.Add(new LdlocInstruction(array_length));
                        instructionsBlock.Add(new Ldc_I4Instruction(parameter.Position));
                        instructionsBlock.Add(new SubInstruction());
                        instructionsBlock.Add(new StlocInstruction(length));

                        // load length and create params array
                        instructionsBlock.Add(new LdlocInstruction(length));
                        instructionsBlock.Add(new NewarrInstruction(typeof(object)));
                        params_slot.EmitSet(instructionsBlock);

                        instructionsBlock.Add(new LdargInstruction(1));
                        instructionsBlock.Add(new Ldc_I4Instruction(parameter.Position));
                        params_slot.EmitGet(instructionsBlock);
                        instructionsBlock.Add(new Ldc_I4Instruction(0));
                        instructionsBlock.Add(new LdlocInstruction(length));
                        instructionsBlock.Add(new CallInstruction(RuntimeHelpers.ArrayCopyMethod));

                        instructionsBlock.Add(new BrInstruction(endLabel.Name));
                        instructionsBlock.Add(new MarkLabelInstruction(elseLabel));

                        instructionsBlock.Add(new Ldc_I4Instruction(0));
                        instructionsBlock.Add(new NewarrInstruction(typeof(object)));
                        params_slot.EmitSet(instructionsBlock);

                        instructionsBlock.Add(new MarkLabelInstruction(endLabel));
                        break;
                    default:
                        break;
                }
            }

            EmitKeyParams(lambdaFunctionDesignator, cec, rest_slot, keys);

            cec.NonVoidReturn = void_backup;

            for (int j = 0; j < lambdaFunctionDesignator.Forms.Count - 1; j++)
            {
                lambdaFunctionDesignator.Forms[j].Visit(this, cec);
            }

            lambdaFunctionDesignator.Forms[lambdaFunctionDesignator.Forms.Count - 1].Visit(this, cec);

            //EmitMultipleValuesCheck(instructionsBlock);

          //  if (cec.NonVoidReturn)
                instructionsBlock.Add(new RetInstruction());
        }

        private static Slot EmitArgsFillRestSlot(CompilationExpressionContext cec, VariableDeclaration array_length, Symbol parameterName, int parameterPosition)
        {
            InstructionsBlock instructionsBlock = cec.InstructionsBlock;

            Slot rest_slot = cec.AddSlot(parameterName, new LocalSlot(instructionsBlock.DefineLocal().Id));

            LabelDeclaration elseLabel = instructionsBlock.DefineLabel();
            LabelDeclaration endLabel = instructionsBlock.DefineLabel();

            instructionsBlock.Add(new LdlocInstruction(array_length));
            instructionsBlock.Add(new Ldc_I4Instruction(parameterPosition));
            instructionsBlock.Add(new BleInstruction(elseLabel.Name));

            instructionsBlock.Add(new LdargInstruction(1));
            instructionsBlock.Add(new Ldc_I4Instruction(parameterPosition));
            instructionsBlock.Add(new LdelemInstruction(typeof(object)));
            instructionsBlock.Add(new NewobjInstruction(StaticConstructorResolver.Create(typeof(Cons), typeof(Object))));
            rest_slot.EmitSet(instructionsBlock);

            VariableDeclaration counter = instructionsBlock.DefineLocal(typeof(int));
            instructionsBlock.Add(new Ldc_I4Instruction(parameterPosition + 1));
            instructionsBlock.Add(new StlocInstruction(counter));

            //while(counter < args.Length)
            LabelDeclaration loop_label = instructionsBlock.DefineLabel();

            instructionsBlock.Add(new MarkLabelInstruction(loop_label));
            instructionsBlock.Add(new LdlocInstruction(array_length));
            instructionsBlock.Add(new LdlocInstruction(counter));
            instructionsBlock.Add(new BleInstruction(endLabel.Name));


            rest_slot.EmitGet(instructionsBlock);

                        instructionsBlock.Add(new IsinstInstruction(typeof(Cons)));

            instructionsBlock.Add(new LdargInstruction(1));
            instructionsBlock.Add(new LdlocInstruction(counter));
            instructionsBlock.Add(new LdelemInstruction(typeof(object)));
            instructionsBlock.Add(new CallInstruction(typeof(Cons).GetMethod("Append")));

            //increment counter 
            instructionsBlock.Add(new LdlocInstruction(counter));
            instructionsBlock.Add(new Ldc_I4Instruction(1));
            instructionsBlock.Add(new AddInstruction());
            instructionsBlock.Add(new StlocInstruction(counter));
            instructionsBlock.Add(new BrInstruction(loop_label));

            instructionsBlock.Add(new BrInstruction(endLabel.Name));

            instructionsBlock.Add(new MarkLabelInstruction(elseLabel));
            instructionsBlock.Add(new PushInstruction(DefinedSymbols.NIL));
            rest_slot.EmitSet(instructionsBlock);
            instructionsBlock.Add(new MarkLabelInstruction(endLabel));

            return rest_slot;
        }

        private void CreateSingleValueArgsOverload(InstructionsBlock instructionsBlock, LambdaFunctionDesignator lambdaFunctionDesignator, CompilationExpressionContext compilationExpressionContext)
        {
            bool values_backup = compilationExpressionContext.SaveValues;
            compilationExpressionContext.SaveValues = false;
            bool void_backup = compilationExpressionContext.NonVoidReturn;
            compilationExpressionContext.NonVoidReturn = true;
            EmitArgsOverload(instructionsBlock, lambdaFunctionDesignator, compilationExpressionContext);
            compilationExpressionContext.SaveValues = values_backup;
            compilationExpressionContext.NonVoidReturn = void_backup;
        }

        private void CreateSingleValueOverload(InstructionsBlock instructionsBlock, LambdaFunctionDesignator lambdaFunctionDesignator, int i, CompilationExpressionContext compilationExpressionContext)
        {
            bool values_backup = compilationExpressionContext.SaveValues;
            compilationExpressionContext.SaveValues = false;
            bool void_backup = compilationExpressionContext.NonVoidReturn;
            compilationExpressionContext.NonVoidReturn = true;
            EmitOverload(instructionsBlock, lambdaFunctionDesignator, i, compilationExpressionContext);
            compilationExpressionContext.SaveValues = values_backup;
            compilationExpressionContext.NonVoidReturn = void_backup;
        }

        private void CreateVoidOverload(InstructionsBlock instructionsBlock, LambdaFunctionDesignator lambdaFunctionDesignator, int i, CompilationExpressionContext compilationExpressionContext)
        {
            bool values_backup = compilationExpressionContext.SaveValues;
            compilationExpressionContext.SaveValues = false;
            bool void_backup = compilationExpressionContext.NonVoidReturn;
            compilationExpressionContext.NonVoidReturn = false;
            EmitOverload(instructionsBlock, lambdaFunctionDesignator, i, compilationExpressionContext);
            compilationExpressionContext.SaveValues = values_backup;
            compilationExpressionContext.NonVoidReturn = void_backup;
        }

        private void CreateVoidArgsOverload(InstructionsBlock instructionsBlock, LambdaFunctionDesignator lambdaFunctionDesignator, CompilationExpressionContext compilationExpressionContext)
        {
            bool values_backup = compilationExpressionContext.SaveValues;
            compilationExpressionContext.SaveValues = false;
            bool void_backup = compilationExpressionContext.NonVoidReturn;
            compilationExpressionContext.NonVoidReturn = false;
            EmitArgsOverload(instructionsBlock, lambdaFunctionDesignator, compilationExpressionContext);
            compilationExpressionContext.SaveValues = values_backup;
            compilationExpressionContext.NonVoidReturn = void_backup;
        }

        

        private StaticTypeResolver GenerateNewLambda(MethodInfoFunctionDesignator methodInfoFunctionDesignator, CompilationExpressionContext compilationExpressionContext)
        {
            throw new NotImplementedException();
        }

        public void GoExpression(LiveLisp.Core.AST.Expressions.GoExpression goExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;
            TagbodyExitPoint label = cec.GetTagExitPoint(goExpression.Tag);

            if (label == null)
                throw new SyntaxErrorException("Tag " + goExpression.Tag + " not found");

            if (label.InstructionsBlock != cec.InstructionsBlock)
            {
                label.NonLocal = true;

                GenerateConstant(label.TagbodyId, typeof(Guid), cec);
                //         cec.InstructionsBlock.Add(new BoxInstruction(typeof(Guid)));
                cec.InstructionsBlock.Add(new PushInstruction(label.Tag));
                cec.InstructionsBlock.Add(new PushInstruction(label.TagId));
                cec.InstructionsBlock.Add(new NewobjInstruction(TagBodyNonLocalTransfer.Constructor));

                cec.InstructionsBlock.Add(new ThrowInstruction());

                label.NonLocalHook.Tag = goExpression.Tag;

                if (cec.NonVoidReturn)
                    cec.InstructionsBlock.Add(new LdnilInstruction());
            }
            else
            {
                cec.InstructionsBlock.Add(new BrInstruction(label.TagLabel));
            }
        }

        public void IfExpression(LiveLisp.Core.AST.Expressions.IfExpression ifExpression, ExpressionContext context)
        {
            var cec = context as CompilationExpressionContext;
            bool void_backup = cec.NonVoidReturn;
            LabelDeclaration elseLabel = cec.InstructionsBlock.DefineLabel();

            cec.NonVoidReturn = true;
            ifExpression.Condition.Visit(this, cec);
            cec.InstructionsBlock.Add(new CallInstruction(RuntimeHelpers.LispIfMethod));
            cec.InstructionsBlock.Add(new BrfalseInstruction(elseLabel.Name));

            cec.NonVoidReturn = void_backup;
            ifExpression.Then.Visit(this, cec);

            LabelDeclaration endLabel = cec.InstructionsBlock.DefineLabel();
            cec.InstructionsBlock.Add(new BrInstruction(endLabel.Name));
            cec.InstructionsBlock.Add(new MarkLabelInstruction(elseLabel));
            ifExpression.Else.Visit(this, cec);
            cec.InstructionsBlock.Add(new MarkLabelInstruction(endLabel));
        }

        public void ILCodeExpression(LiveLisp.Core.AST.Expressions.CLR.ILCodeExpression iLCodeExpression, ExpressionContext context)
        {
            var cec = context as CompilationExpressionContext;
            cec.InstructionsBlock.Add(iLCodeExpression.Instruction);

            MarkLabelInstruction mli = iLCodeExpression.Instruction as MarkLabelInstruction;
            if (mli != null)
                cec.InstructionsBlock.Labels.Add(mli.Label);
        }

        public void LabelsExpression(LiveLisp.Core.AST.Expressions.LabelsExpression labelsExpression, ExpressionContext context)
        {
            CompilationExpressionContext out_cec = context as CompilationExpressionContext;
            CompilationExpressionContext cec = new CompilationExpressionContext(out_cec);
            for (int i = 0; i < labelsExpression.Lambdas.Count; i++)
            {
                LambdaFunctionDesignator lambda = labelsExpression.Lambdas[i];
                FieldDeclarationBasedFieldResolver resolver = new FieldDeclarationBasedFieldResolver(null);
                cec.AddLocalFunction(lambda.Name, new FieldSlot(null, resolver));
                ClassDeclBasedTypeResolver lambda_type = GenerateNewLambda(lambda, cec) as ClassDeclBasedTypeResolver;

                FieldDeclaration singletonField = new FieldDeclaration(FieldAttributes.Public | FieldAttributes.Static, lambda_type, lambda_type._class_decl.Name + "$instance");

                resolver.field = singletonField;
                lambda_type._class_decl.AddField(singletonField);

                MethodDeclaration init_Method = new MethodDeclaration();
                init_Method.Name = "CreateInstance";
                init_Method.Attributes = MethodAttributes.Public | MethodAttributes.Static;
                init_Method.ReturnType = typeof(void);
                init_Method.Instructions.Add(new NewobjInstruction(lambda_type._class_decl.Constructors[0]));
                init_Method.Instructions.Add(new StsfldInstruction(singletonField));
                init_Method.Instructions.Add(new RetInstruction());
                lambda_type._class_decl.Methods.Add(init_Method);

                out_cec.InstructionsBlock.Add(new CallInstruction(init_Method));


            }

            cec.NonVoidReturn = false;
            for (int i = 0; i < labelsExpression.Forms.Count - 1; i++)
            {
                labelsExpression.Forms[i].Visit(this, cec);
            }

            cec.NonVoidReturn = out_cec.NonVoidReturn;
            labelsExpression.Forms[labelsExpression.Forms.Count - 1].Visit(this, cec);
        }

        public void LetExpression(LiveLisp.Core.AST.Expressions.LetExpression letExpression, ExpressionContext context)
        {
            var outer_context = context as CompilationExpressionContext;
            var instructionsBlock = outer_context.InstructionsBlock;
            var bindings_context = new CompilationExpressionContext(outer_context);


            var forms_context = new CompilationExpressionContext(bindings_context);

            ApplyDeclarations(letExpression, forms_context);


            // после этого станет известно какие переменнные входят в замыкание 
            // публиковать их теперь надо будет как слоты использующие поля
            //
            // кроме того если текущее выражение выступает контейнером замыканий то нужно инициализировать
            // экзмепляр окружения
            
            ClosuresManager clcontext = outer_context.ClosuresManager;
            if(clcontext == null)
            clcontext = new ClosuresWalker(letExpression, forms_context).Walk();

            forms_context.ClosuresManager = clcontext;

            ClosureContainerContext currentCCC = clcontext.GetContainer(letExpression);

            bool isClosureContainer = false;
            Slot closureEnvInstanceSlot = null;
            Dictionary<Symbol, FieldSlot> closureSlots = null;
            if (currentCCC != null && currentCCC.ClosedOverVars.Count != 0)
            {
                isClosureContainer = true;
                closureEnvInstanceSlot = new LocalSlot(forms_context.InstructionsBlock.DefineLocal(currentCCC.EnvType));

                closureSlots = new Dictionary<Symbol, FieldSlot>();

                for (int i = 0; i < currentCCC.ClosedOverVars.Count; i++)
                {
                    FieldSlot cs = new FieldSlot(closureEnvInstanceSlot, currentCCC.ClosedOverVars[i].Field);
                    closureSlots.Add(currentCCC.ClosedOverVars[i].Name, cs);
                }

                ClassDeclBasedTypeResolver entype = currentCCC.EnvType as ClassDeclBasedTypeResolver;
                if(entype._class_decl.CreatedType == null)
                {
                    forms_context.CompilationContext.AddClassDeclaration(entype._class_decl);
                }

                instructionsBlock.Add(new NewobjInstruction(currentCCC.EnvType));
                closureEnvInstanceSlot.EmitSet(instructionsBlock);

                forms_context.SetSlotForClosureContainer(currentCCC, closureEnvInstanceSlot);
            }

            InstructionsBlock specials_rebind_instr = new InstructionsBlock();
            int instructions_start = bindings_context.InstructionsBlock.Count;

            bool void_backup = outer_context.NonVoidReturn;
            bool values_backup = outer_context.SaveValues;

            bindings_context.NonVoidReturn = true;
            bindings_context.SaveValues = false;
            for (int i = 0; i < letExpression.NewBindings.Count; i++)
            {
                SyntaxBinding binding = letExpression.NewBindings[i];
                if (forms_context.IsSpecial(binding.Symbol))
                {
                    VariableDeclaration backup = instructionsBlock.DefineLocal();
                    GlobalRawSlot gs = new GlobalRawSlot(binding.Symbol);
                    gs.EmitGet(instructionsBlock);
                    instructionsBlock.Add(new StlocInstruction(backup));
                    gs.EmitSetProlog(instructionsBlock);
                    binding.Value.Visit(this, bindings_context);
                    gs.EmitSet(instructionsBlock);

                    gs.EmitSetProlog(specials_rebind_instr);
                    specials_rebind_instr.Add(new LdlocInstruction(backup));
                    gs.EmitSet(specials_rebind_instr);
                }
                else if (isClosureContainer && closureSlots.ContainsKey(binding.Symbol))
                {
                    Slot slot = forms_context.AddSlot(binding.Symbol, closureSlots[binding.Symbol]);
                    slot.EmitSetProlog(instructionsBlock);
                    binding.Value.Visit(this, bindings_context);
                    slot.EmitSet(instructionsBlock);
                }
                else
                {
                    VariableDeclaration local = instructionsBlock.DefineLocal(typeof(object));
                    Slot slot = forms_context.AddSlot(binding.Symbol, new LocalSlot(local.Id));
                    slot.EmitSetProlog(instructionsBlock);
                    binding.Value.Visit(this, bindings_context);
                    slot.EmitSet(instructionsBlock);
                }
            }
            forms_context.NonVoidReturn = false;
            forms_context.SaveValues = false;
            for (int i = 0; i < letExpression.Forms.Count - 1; i++)
            {
                letExpression.Forms[i].Visit(this, forms_context);
            }


            forms_context.NonVoidReturn = void_backup;
            forms_context.SaveValues = values_backup;
            letExpression.Forms[letExpression.Forms.Count - 1].Visit(this, forms_context);

            if (specials_rebind_instr.Count != 0)
            {
                instructionsBlock.Insert(instructions_start, new BeginExceptionBlock());

                VariableDeclaration var = null;
                if (void_backup)
                {
                    var = instructionsBlock.DefineLocal();
                    instructionsBlock.Add(new StlocInstruction(var));
                }

                instructionsBlock.Add(new BeginFinallyBlock());

                instructionsBlock.AddRange(specials_rebind_instr);

                instructionsBlock.Add(new EndExceptionBlock());

                if (void_backup)
                    instructionsBlock.Add(new LdlocInstruction(var));
            }
        }

        public void LetStarExpression(LiveLisp.Core.AST.Expressions.LetStarExpression letStarExpression, ExpressionContext context)
        {
            var outer_context = context as CompilationExpressionContext;
            var instructionsBlock = outer_context.InstructionsBlock;
            var bindings_context = new CompilationExpressionContext(outer_context);
            var forms_context = new CompilationExpressionContext(bindings_context);

            ApplyDeclarations(letStarExpression, bindings_context);

            InstructionsBlock specials_rebind_instr = new InstructionsBlock();
            int instructions_start = bindings_context.InstructionsBlock.Count;

            ClosuresManager clcontext = outer_context.ClosuresManager;
            if (clcontext == null)
                clcontext = new ClosuresWalker(letStarExpression, forms_context).Walk();

            forms_context.ClosuresManager = clcontext;

            ClosureContainerContext currentCCC = clcontext.GetContainer(letStarExpression);

            bool isClosureContainer = false;
            Slot closureEnvInstanceSlot = null;
            Dictionary<Symbol, FieldSlot> closureSlots = null;

            if (currentCCC != null)
            {
                isClosureContainer = true;
                closureEnvInstanceSlot = new LocalSlot(forms_context.InstructionsBlock.DefineLocal(currentCCC.EnvType));

                closureSlots = new Dictionary<Symbol, FieldSlot>();

                for (int i = 0; i < currentCCC.ClosedOverVars.Count; i++)
                {
                    FieldSlot cs = new FieldSlot(closureEnvInstanceSlot, currentCCC.ClosedOverVars[i].Field);
                    closureSlots.Add(currentCCC.ClosedOverVars[i].Name, cs);
                }

                ClassDeclBasedTypeResolver entype = currentCCC.EnvType as ClassDeclBasedTypeResolver;
                if (entype._class_decl.CreatedType == null)
                {
                    bindings_context.CompilationContext.AddClassDeclaration(entype._class_decl);
                }

                instructionsBlock.Add(new NewobjInstruction(currentCCC.EnvType));
                closureEnvInstanceSlot.EmitSet(instructionsBlock);

                bindings_context.SetSlotForClosureContainer(currentCCC, closureEnvInstanceSlot);
            }



            bool void_backup = outer_context.NonVoidReturn;
            bool values_backup = outer_context.SaveValues;

            outer_context.NonVoidReturn = true;
            outer_context.SaveValues = false;

            for (int i = 0; i < letStarExpression.NewBindings.Count; i++)
            {

                SyntaxBinding binding = letStarExpression.NewBindings[i];
                if (bindings_context.IsSpecial(binding.Symbol))
                {
                    VariableDeclaration backup = instructionsBlock.DefineLocal();
                    GlobalRawSlot gs = new GlobalRawSlot(binding.Symbol);
                    gs.EmitGet(instructionsBlock);
                    instructionsBlock.Add(new StlocInstruction(backup));
                    gs.EmitSetProlog(instructionsBlock);
                    binding.Value.Visit(this, bindings_context);
                    gs.EmitSet(instructionsBlock);

                    gs.EmitSetProlog(specials_rebind_instr);
                    specials_rebind_instr.Add(new LdlocInstruction(backup));
                    gs.EmitSet(specials_rebind_instr);
                }
                else if (isClosureContainer && closureSlots.ContainsKey(binding.Symbol))
                {
                    Slot slot = bindings_context.AddSlot(binding.Symbol, closureSlots[binding.Symbol]);
                    slot.EmitSetProlog(instructionsBlock);
                    binding.Value.Visit(this, bindings_context);
                    slot.EmitSet(instructionsBlock);
                }
                else
                {
                    VariableDeclaration local = bindings_context.InstructionsBlock.DefineLocal(typeof(object));
                    Slot slot = bindings_context.AddSlot(letStarExpression.NewBindings[i].Symbol, new LocalSlot(local.Id));
                    slot.EmitSetProlog(bindings_context.InstructionsBlock);
                    letStarExpression.NewBindings[i].Value.Visit(this, bindings_context);
                    slot.EmitSet(bindings_context.InstructionsBlock);
                }
            }
            forms_context.NonVoidReturn = false;
            forms_context.SaveValues = false;
            for (int i = 0; i < letStarExpression.Forms.Count - 1; i++)
            {
                letStarExpression.Forms[i].Visit(this, forms_context);
            }


            forms_context.NonVoidReturn = void_backup;
            forms_context.SaveValues = values_backup;
            letStarExpression.Forms[letStarExpression.Forms.Count - 1].Visit(this, forms_context);

            if (specials_rebind_instr.Count != 0)
            {
                instructionsBlock.Insert(instructions_start, new BeginExceptionBlock());

                VariableDeclaration var = null;
                if (void_backup)
                {
                    var = instructionsBlock.DefineLocal();
                    instructionsBlock.Add(new StlocInstruction(var));
                }

                instructionsBlock.Add(new BeginFinallyBlock());

                instructionsBlock.AddRange(specials_rebind_instr);

                instructionsBlock.Add(new EndExceptionBlock());

                if (void_backup)
                    instructionsBlock.Add(new LdlocInstruction(var));
            }
        }


        private void ApplyDeclarations(IDeclarationsContainer declarationsContainer, CompilationExpressionContext cec)
        {
            for (int i = 0; i < declarationsContainer.Declarations.Count; i++)
            {
                declarationsContainer.Declarations[i].ApplyToEnvironment(cec);
            }
        }


        public void LoadTimeValueExpression(LiveLisp.Core.AST.Expressions.LoadTimeValue loadTimeValue, ExpressionContext context)
        {
            var cec = context as CompilationExpressionContext;
            var null_env = new CompilationExpressionContext(cec.CompilationContext);
            null_env.InstructionsBlock = cec.InstructionsBlock;

            null_env.SaveValues = false;
            null_env.NonVoidReturn = cec.NonVoidReturn;

            loadTimeValue.Form.Visit(this, null_env);
        }

        public void LocallyExpression(LiveLisp.Core.AST.Expressions.LocallyExpression locallyExpression, ExpressionContext context)
        {
            var cec = context as CompilationExpressionContext;
            var inner = new CompilationExpressionContext(cec);
            ApplyDeclarations(locallyExpression, inner);


            bool void_backup = cec.NonVoidReturn;
            bool values_backup = cec.SaveValues;

            cec.NonVoidReturn = false;
            cec.SaveValues = false;

           
            for (int i = 0; i < locallyExpression.Count-1; i++)
            {
                locallyExpression[i].Visit(this, inner);
            }

            cec.NonVoidReturn = void_backup;
            cec.SaveValues = values_backup;
            if (locallyExpression.Count != 0)
            {
                locallyExpression[locallyExpression.Count - 1].Visit(this, inner);
            }
            else if(void_backup)
            {
                GenerateConstant(DefinedSymbols.NIL, null, inner);
            }
        }

        public void MacroletExpression(LiveLisp.Core.AST.Expressions.MacroletExpression macroletExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void MultipleValueCallExpression(LiveLisp.Core.AST.Expressions.MultipleValueCallExpression multipleValueCallExpression, ExpressionContext context)
        {
            var cec = context as CompilationExpressionContext;
            // first of all evaluate function
            InstructionsBlock instructionsBlock = cec.InstructionsBlock;
            VariableDeclaration func_holder = instructionsBlock.DefineLocal(typeof(LispFunction));
            multipleValueCallExpression.Function.Visit(this, cec);
            instructionsBlock.Add(new PushInstruction("PROGN"));
            instructionsBlock.Add(new PushInstruction("function-form"));
            instructionsBlock.Add(new CallInstruction(RuntimeHelpers.CoerceToFunctionMethod));
            instructionsBlock.Add(new StlocInstruction(func_holder));

            VariableDeclaration values_holder = instructionsBlock.DefineLocal(typeof(MultipleValuesContainer));
            instructionsBlock.Add(new NewobjInstruction(MultipleValuesContainer.Values0Ctor));
            instructionsBlock.Add(new StlocInstruction(values_holder));

            bool void_backup = cec.NonVoidReturn;
            bool values_backup = cec.SaveValues;

            cec.NonVoidReturn = true;
            cec.SaveValues = true;

            for (int i = 0; i < multipleValueCallExpression.ValuesProducers.Count; i++)
            {
                cec.InstructionsBlock.Add(new LdlocInstruction(values_holder));
                multipleValueCallExpression.ValuesProducers[i].Visit(this, cec);
                cec.InstructionsBlock.Add(new CallInstruction(MultipleValuesContainer.CombineMethod));
            }

            instructionsBlock.Add(new LdlocInstruction(func_holder));
            instructionsBlock.Add(new LdlocInstruction(values_holder));
            instructionsBlock.Add(new CallInstruction(MultipleValuesContainer.ToArrayMethod));

            if (values_backup)
            {
                if (values_backup)
                {
                    instructionsBlock.Add(new CallvirtInstruction(LambdaCallEmitterHelper.ValuesInvokeMethods[15]));
                }
                else
                {
                    instructionsBlock.Add(new CallvirtInstruction(LambdaCallEmitterHelper.InvokeMethods[15]));
                }
            }
            else
            {
                instructionsBlock.Add(new CallvirtInstruction(LambdaCallEmitterHelper.VoidInvokeMethods[15]));
            }

            cec.NonVoidReturn = void_backup;
            cec.SaveValues = values_backup;
        }

        public void MultipleValueProg1Expression(LiveLisp.Core.AST.Expressions.MultipleValueProg1Expression multipleValueProg1Expression, ExpressionContext context)
        {
            var cec = context as CompilationExpressionContext;
            bool void_backup = cec.NonVoidReturn;
            bool values_backup = cec.SaveValues;

            VariableDeclaration temp = null;

            if (void_backup)
            {
                temp = cec.InstructionsBlock.DefineLocal();
            }

            multipleValueProg1Expression.FirstForm.Visit(this, cec);
            if (void_backup)
                cec.InstructionsBlock.Add(new StlocInstruction(temp));

            cec.NonVoidReturn = false;
            cec.SaveValues = false;
            for (int i = 0; i < multipleValueProg1Expression.DiscarededForms.Count; i++)
            {
                multipleValueProg1Expression.DiscarededForms[i].Visit(this, cec);
            }

            cec.NonVoidReturn = void_backup;
            cec.SaveValues = values_backup;

            if (void_backup)
                cec.InstructionsBlock.Add(new LdlocInstruction(temp));
        }

        public void PrognExpression(LiveLisp.Core.AST.Expressions.PrognExpression prognExpression, ExpressionContext context)
        {
            var cec = context as CompilationExpressionContext;
            bool void_backup = cec.NonVoidReturn;
            cec.NonVoidReturn = false;
            for (int i = 0; i < prognExpression.Count - 1; i++)
            {
                prognExpression[i].Visit(this, cec);
            }

            cec.NonVoidReturn = void_backup;
            prognExpression[prognExpression.Count - 1].Visit(this, cec);
        }

        public void ProgvExpression(LiveLisp.Core.AST.Expressions.ProgvExpression progvExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;
            bool void_backup = cec.NonVoidReturn;
            bool values_backup = cec.SaveValues;

            cec.NonVoidReturn = true;
            cec.SaveValues = false;

            InstructionsBlock instructionsBlock = cec.InstructionsBlock;
            VariableDeclaration symbols_list_var = instructionsBlock.DefineLocal();
            VariableDeclaration backup_bindings_var = instructionsBlock.DefineLocal(typeof(List<object>));
            progvExpression.Symbols.Visit(this, cec);
            instructionsBlock.Add(new StlocInstruction(symbols_list_var));
            instructionsBlock.Add(new LdlocInstruction(symbols_list_var));
            progvExpression.Values.Visit(this, cec);
            instructionsBlock.Add(new LdlocaInstruction(backup_bindings_var));
            instructionsBlock.Add(new CallInstruction(RuntimeHelpers.ProgvBindingHelperMethod));

            instructionsBlock.Add(new BeginExceptionBlock());

            cec.NonVoidReturn = false;

            for (int i = 0; i < progvExpression.Forms.Count - 1; i++)
            {
                progvExpression.Forms[i].Visit(this, cec);
            }

            cec.NonVoidReturn = void_backup;
            cec.SaveValues = values_backup;

            progvExpression.Forms[progvExpression.Forms.Count - 1].Visit(this, cec);

            VariableDeclaration ret_var = null;
            if (void_backup)
            {
                ret_var = instructionsBlock.DefineLocal();
                instructionsBlock.Add(new StlocInstruction(ret_var));
            }

            instructionsBlock.Add(new BeginFinallyBlock());
            instructionsBlock.Add(new LdlocInstruction(symbols_list_var));
            instructionsBlock.Add(new LdlocInstruction(backup_bindings_var));
            instructionsBlock.Add(new CallInstruction(RuntimeHelpers.RestoreProgvHelperMethod));
            instructionsBlock.Add(new EndExceptionBlock());

            if (void_backup)
                instructionsBlock.Add(new LdlocInstruction(ret_var));


        }

        public void ReturnFromExpression(LiveLisp.Core.AST.Expressions.ReturnFromExpression returnFromExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;
            BlockExitPoint label = cec.GetBlockExitPoint(returnFromExpression.Tag);

            if (label == null)
                throw new SyntaxErrorException("Block " + returnFromExpression.Tag + " not found");

            if (label.InstructionsBlock != cec.InstructionsBlock)
            {
                label.NonLocal = true;
                cec.InstructionsBlock.Add(new PushInstruction(label.Id));
                //         cec.InstructionsBlock.Add(new BoxInstruction(typeof(Guid)));
                cec.InstructionsBlock.Add(new PushInstruction(label.Name));
                bool void_backup = cec.NonVoidReturn;
                cec.NonVoidReturn = label.NonVoid;
                returnFromExpression.Form.Visit(this, cec);
                cec.NonVoidReturn = void_backup;
                if (label.NonVoid)
                {
                    cec.InstructionsBlock.Add(new NewobjInstruction(BlockNonLocalTransfer.NonVoidBlockNonLocalTransferConstructor));
                }
                else
                {
                    cec.InstructionsBlock.Add(new NewobjInstruction(BlockNonLocalTransfer.VoidBlockNonLocalTransferConstructor));
                }
                cec.InstructionsBlock.Add(new ThrowInstruction());
            }
            else
            {

                if (cec.NonVoidReturn)
                    returnFromExpression.Form.Visit(this, cec);
                cec.InstructionsBlock.Add(new BrInstruction(label.LocalExitPoint));
            }
        }

        public void SetqExpression(LiveLisp.Core.AST.Expressions.SetqExpression setqExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;
            bool void_backup = cec.NonVoidReturn;
            cec.NonVoidReturn = true;
            Slot slot;
            List<SyntaxBinding> setqExpressionAssings = setqExpression.Assings;
            for (int i = 0; i < setqExpressionAssings.Count - 1; i++)
            {
                slot = cec.GetSlot(setqExpressionAssings[i].Symbol);
                if (slot == null)
                {
                    //TODO: put warning about using global value here
                    slot = new GlobalSlot(setqExpressionAssings[i].Symbol);
                }
                slot.EmitSetProlog(cec.InstructionsBlock);
                setqExpressionAssings[i].Value.Visit(this, cec);
                slot.EmitSet(cec.InstructionsBlock);
            }

            slot = cec.GetSlot(setqExpressionAssings[setqExpressionAssings.Count - 1].Symbol);
            if (slot == null)
            {
                //TODO: put warning about using global value here
                slot = new GlobalSlot(setqExpressionAssings[setqExpressionAssings.Count - 1].Symbol);
            }
            slot.EmitSetProlog(cec.InstructionsBlock);
            setqExpressionAssings[setqExpressionAssings.Count - 1].Value.Visit(this, cec);

            slot.EmitSet(cec.InstructionsBlock);


            if (void_backup)
            {
                slot.EmitGet(cec.InstructionsBlock);
            }

            cec.NonVoidReturn = void_backup;
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
            CompilationExpressionContext cec = new CompilationExpressionContext(context as CompilationExpressionContext);

            bool void_backup = cec.NonVoidReturn;
            cec.NonVoidReturn = false;

            Guid tagbodyId = Guid.NewGuid();
            NonLocalHook nlhook = new NonLocalHook(null);
            for (int i = 0; i < tagBodyExpression.tags.Count; i++)
            {
                TagbodyExitPoint exitPoint = new TagbodyExitPoint(cec.InstructionsBlock, tagbodyId, i, cec.InstructionsBlock.DefineLabel(), tagBodyExpression.tags[i]);
                exitPoint.NonLocalHook = nlhook;
                cec.AddTag(tagBodyExpression.tags[i], exitPoint);
            }

            List<int> currentSubblockExits = new List<int>();
            int subblockstartindex = cec.InstructionsBlock.Count;
            for (int i = 0; i < tagBodyExpression.NontaggedProlog.Count; i++)
            {
                nlhook.Tag = null;
                tagBodyExpression.NontaggedProlog[i].Visit(this, cec);

                if (nlhook.Tag != null) // в этом "подблоке" будет нелокальный переход к этому тегу
                {
                    int tagid = tagBodyExpression.tags.IndexOf(nlhook.Tag);

                    if (!currentSubblockExits.Contains(tagid))
                    {
                        currentSubblockExits.Add(tagid);
                    }
                }
            }

            CheckForNonLocalTagbodyExits(tagBodyExpression, cec, currentSubblockExits, subblockstartindex, tagbodyId);

            for (int i = 0; i < tagBodyExpression.TaggedStatements.Count; i++)
            {
                currentSubblockExits = new List<int>();
                subblockstartindex = cec.InstructionsBlock.Count;

                cec.InstructionsBlock.Add(new MarkLabelInstruction(cec.GetTagExitPoint(tagBodyExpression.TaggedStatements[i].Tag).TagLabel));

                for (int j = 0; j < tagBodyExpression.TaggedStatements[i].Statements.Count; j++)
                {
                    nlhook.Tag = null;
                    tagBodyExpression.TaggedStatements[i].Statements[j].Visit(this, cec);

                    if (nlhook.Tag != null) // в этом "подблоке" будет нелокальный переход к этому тегу
                    {
                        int tagid = tagBodyExpression.tags.IndexOf(nlhook.Tag);

                        if (!currentSubblockExits.Contains(tagid))
                        {
                            currentSubblockExits.Add(tagid);
                        }
                    }
                }

                CheckForNonLocalTagbodyExits(tagBodyExpression, cec, currentSubblockExits, subblockstartindex, tagbodyId);
            }

            cec.NonVoidReturn = void_backup;
            if (void_backup)
                cec.InstructionsBlock.Add(new PushInstruction(DefinedSymbols.NIL));
        }

        private void CheckForNonLocalTagbodyExits(LiveLisp.Core.AST.Expressions.TagBodyExpression tagBodyExpression, CompilationExpressionContext cec, List<int> currentSubblockExits, int subblockstartindex, Guid tagbodyid)
        {

            if (currentSubblockExits.Count != 0)
            {
                List<LabelDeclaration> TagRefs;
                int currentindex = cec.InstructionsBlock.Count;
                TagRefs = new List<LabelDeclaration>(tagBodyExpression.tags.Count);
                for (int i = 0; i < tagBodyExpression.tags.Count; i++)
                {
                    TagRefs.Add(cec.GetTagExitPoint(tagBodyExpression.tags[currentSubblockExits[i]]).TagLabel);
                }
                WrapInTagCatchBlock(cec, subblockstartindex, currentindex, TagRefs, tagbodyid );
                // текущий подблок содержит нелокальные переходы на теги, т.е. нужно "обрамить" его в перехватчик исключения
            }
        }

        private void WrapInTagCatchBlock(CompilationExpressionContext cec, int subblockstartindex, int currentindex, List<LabelDeclaration> TagRefs, Guid tagbodyId)
        {
            var instructionsBlock = cec.InstructionsBlock;

            LabelDeclaration ExcExitLabel = new LabelDeclaration(instructionsBlock.NewUniqueLabelName());
            instructionsBlock.Insert(subblockstartindex, new BeginExceptionBlock(ExcExitLabel));
            instructionsBlock.Add(new BeginCatchBlock(typeof(TagBodyNonLocalTransfer)));
            VariableDeclaration exc_slot = instructionsBlock.DefineLocal(typeof(TagBodyNonLocalTransfer));
            instructionsBlock.Add(new StlocInstruction(exc_slot));

            var temp = instructionsBlock.DefineLocal();

            var elselabel = instructionsBlock.DefineLabel();
            // var endlabel = instructionsBlock.DefineLabel();

            instructionsBlock.Add(new LdlocInstruction(exc_slot));
            instructionsBlock.Add(new LdfldaInstruction(TagBodyNonLocalTransfer.IdField));
            GenerateConstant(tagbodyId, typeof(Guid), cec);
            instructionsBlock.Add(new CallInstruction(RuntimeHelpers.GuidEqualsMethod));
            instructionsBlock.Add(new BrfalseInstruction(elselabel));
            // если тут. то tagbody совпал

            LabelDeclaration[] switchLabels = new LabelDeclaration[TagRefs.Count];


            instructionsBlock.Add(new LdlocInstruction(exc_slot));
            instructionsBlock.Add(new LdfldInstruction(TagBodyNonLocalTransfer.TagIdField));
            instructionsBlock.Add(new SwitchInstruction(switchLabels));

            for (int i = 0; i < TagRefs.Count; i++)
            {
                switchLabels[i] = instructionsBlock.DefineLabel();
                instructionsBlock.Add(new MarkLabelInstruction(switchLabels[i]));
                instructionsBlock.Add(new LeaveInstruction(TagRefs[i].Name)); 
            }

            instructionsBlock.Add((new MarkLabelInstruction(elselabel)));
            instructionsBlock.Add(new RethrowInstruction());
            instructionsBlock.Add(new EndExceptionBlock());
        }

        public void TheExpression(LiveLisp.Core.AST.Expressions.TheExpression theExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ThrowExpression(LiveLisp.Core.AST.Expressions.ThrowExpression throwExpression, ExpressionContext context)
        {

            CompilationExpressionContext cec = context as CompilationExpressionContext;

            bool void_backup = cec.NonVoidReturn;
            cec.NonVoidReturn = true;


            throwExpression.CatchTag.Visit(this, context);
            throwExpression.ResultForm.Visit(this, context);
            cec.InstructionsBlock.Add(new NewobjInstruction(CatchThrowException.CatchThrowExceptionConstructor));
            cec.InstructionsBlock.Add(new ThrowInstruction());

            cec.NonVoidReturn = void_backup;
        }

        public void UnwindProtectExpression(LiveLisp.Core.AST.Expressions.UnwindProtectExpression unwindProtectExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;
            var instructionsBlock = cec.InstructionsBlock;

            LabelDeclaration endlabel = new LabelDeclaration(instructionsBlock.NewUniqueLabelName());
            instructionsBlock.Add(new BeginExceptionBlock(endlabel));

            VariableDeclaration ret_var = null;

            if (cec.NonVoidReturn)
                ret_var = instructionsBlock.DefineLocal();

            unwindProtectExpression.ProtectedForm.Visit(this, cec);

            if (cec.NonVoidReturn)
                instructionsBlock.Add(new StlocInstruction(ret_var));

            instructionsBlock.Add(new BeginFinallyBlock());

            bool void_backup = cec.NonVoidReturn;
            bool values_backup = cec.SaveValues;

            cec.NonVoidReturn = false;
            cec.SaveValues = false;

            for (int i = 0; i < unwindProtectExpression.CleanupForms.Count; i++)
            {
                unwindProtectExpression.CleanupForms[i].Visit(this, cec);
            }

            instructionsBlock.Add(new EndExceptionBlock());

            cec.SaveValues = values_backup;
            cec.NonVoidReturn = void_backup;

            if (void_backup)
                instructionsBlock.Add(new LdlocInstruction(ret_var));
        }

        public void VariableExpression(LiveLisp.Core.AST.Expressions.VariableExpression variableExpression, ExpressionContext context)
        {
            CompilationExpressionContext cec = context as CompilationExpressionContext;
            if (cec.NonVoidReturn)
            {
                Slot slot = cec.GetSlot(variableExpression.VariableName);
                if (slot == null)
                {
                    //TODO: put warning about using global value here
                    slot = new GlobalSlot(variableExpression.VariableName);
                }
                slot.EmitGet(cec.InstructionsBlock);
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
