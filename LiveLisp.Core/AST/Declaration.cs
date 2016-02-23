namespace LiveLisp.Core.AST
{
    using System;
using LiveLisp.Core.Types;
using System.Collections.Generic;
    using LiveLisp.Core.BuiltIns.Conditions;

    public delegate Declaration DeclarationParser(Cons decl_body);

    public abstract class Declaration
    {
        public abstract void ApplyForProclaim();

        public abstract void ApplyToEnvironment(IDeclarationsClient cec);

        public abstract bool ValidForProclaim
        {
            get;
        }

        public abstract bool ValidForEval
        {
            get;
        }

        internal static bool IsSupportedDeclaration(Symbol car, out DeclarationParser parser)
        {
            return supportedDecls.TryGetValue(car.Id, out parser);
        }

        internal static bool IsForeignDeclaration(Symbol car)
        {
            return false;
        }

        static Dictionary<int, DeclarationParser> supportedDecls;

        static Declaration()
        {
            supportedDecls = new Dictionary<int, DeclarationParser>();
            supportedDecls.Add(DefinedSymbols.Special.Id, ParseSpecialDeclarations);
        }

        private static Declaration ParseSpecialDeclarations(Cons cons)
        {
            if (cons == null)
                ConditionsDictionary.Error("Special declaration too short");

            List<Symbol> variables = new List<Symbol>(cons.Count);

            foreach (var item in cons)
            {
                Symbol varname = item as Symbol;

                if (varname == null)
                {
                    throw new ReaderErrorException("special declaration: {0} is invalid varname", item);
                }
                variables.Add(varname);
            }

            return new SpecialDeclaration(variables);
        }
    }

    public interface IDeclarationsClient
    {

        void SetSpecial(LiveLisp.Core.Types.Symbol symbol);
    }

    public class ForeignDeclarationStub : Declaration
    {
        Cons DeclForm; 
        
        public ForeignDeclarationStub(Cons decl_form)
        {
            DeclForm = decl_form;
        }

        public override void ApplyForProclaim()
        {

        }

        public override void ApplyToEnvironment(IDeclarationsClient cec)
        {

        }

        public override bool ValidForProclaim
        {
            get { return true; }
        }

        public override bool ValidForEval
        {
            get { return true; }
        }

        public override string ToString()
        {
            return "Foreign declaration stub: " + DeclForm;
        }
    }

}

