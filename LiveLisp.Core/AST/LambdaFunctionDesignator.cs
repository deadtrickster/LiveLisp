namespace LiveLisp.Core.AST
{
    using LiveLisp.Core.Compiler;
    using LiveLisp.Core.Types;
    using System;
    using System.Collections.Generic;

    public class LambdaFunctionDesignator : FunctionNameDesignator, IImplicitProgn, IDeclarationsContainer, IBarrier, INewLexicalBindingsProvider
    {
        private List<Declaration> _declarations;
        private List<Expression> _forms;
        private LiveLisp.Core.Compiler.LambdaList _lambdaList;
        private KeywordSymbol _lambdaType;
        private Symbol _name;

        public override FunctionNameDesignatorType DesignatorType
        {
            get
            {
                return FunctionNameDesignatorType.Lambda;
            }
        }
        public LambdaFunctionDesignator(Symbol name, LambdaList lambdaList, Expression body)
            : base(ExpressionContext.Root)
        {
            this._lambdaType = DefinedSymbols.Ordinary;
            this._name = name;
            this._lambdaList = lambdaList;
            this._declarations = new List<Declaration>();
            this._forms = new List<Expression>(new Expression[]{body});
        }

        public LambdaFunctionDesignator(KeywordSymbol lambdaType, Symbol lambdaName, LiveLisp.Core.Compiler.LambdaList lambdaList, List<Declaration> declarations, List<Expression> forms, ExpressionContext context) : base(context)
        {
            this._lambdaType = lambdaType;
            this._name = lambdaName;
            this._lambdaList = lambdaList;
            this._declarations = declarations;
            this._forms = forms;
        }

        public List<Declaration> Declarations
        {
            get
            {
                return this._declarations;
            }
        }

        public List<Expression> Forms
        {
            get
            {
                return this._forms;
            }
        }

        public LiveLisp.Core.Compiler.LambdaList LambdaList
        {
            get
            {
                return this._lambdaList;
            }
            set
            {
                this._lambdaList = value;
            }
        }

        public KeywordSymbol LambdaType
        {
            get
            {
                return this._lambdaType;
            }
            set
            {
                this._lambdaType = value;
            }
        }

        public Symbol Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public Expression this[int index]
        {
            get { return _forms[index]; }
        }

        public int Count
        {
            get { return _forms.Count; }
        }

        public Expression Last
        {
            get { return _forms[_forms.Count - 1]; }
        }

        #region INewLexicalBindingsProvider Members

        public List<Symbol> IntroducesLexicalVariables
        {
            get
            {
                List<Symbol> vars = new List<Symbol>();

                for (int i = 0; i < LambdaList.Count; i++)
                {
                    if (!vars.Contains(LambdaList[i].Name))
                        vars.Add(LambdaList[i].Name);
                }

                for (int i = 0; i < Declarations.Count; i++)
                {
                    SpecialDeclaration specialdecl = Declarations[i] as SpecialDeclaration;
                    if (specialdecl != null)
                    {
                        for (int j = 0; j < specialdecl.Vars.Count; j++)
                        {
                            vars.Remove(specialdecl.Vars[j]);
                        }
                    }
                }

                return vars;
            }
        }

        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            return evaluator.LambdaFunctionDesignator(this, context);
        }
        #endregion
    }
}

