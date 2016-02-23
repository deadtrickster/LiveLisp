namespace LiveLisp.Core.AST
{
    using System;
    using System.Collections.Generic;
    using LiveLisp.Core.Types;
    using LiveLisp.Core.Compiler;

    public class SpecialDeclaration : Declaration
    {
        public List<Symbol> Vars;

        public SpecialDeclaration()
        {
            this.Vars = new List<Symbol>();
        }

        public SpecialDeclaration(List<Symbol> vars)
        {
            this.Vars = vars;
        }

        public override void ApplyForProclaim()
        {
            for (int i = 0; i < Vars.Count; i++)
            {
                Vars[i].IsDynamic = true;
            }
        }

        public override void ApplyToEnvironment(IDeclarationsClient cec)
        {
            for (int i = 0; i < Vars.Count; i++)
            {
                cec.SetSpecial(Vars[i]);
              //  cec.AddSlot(Vars[i], new GlobalSlot(Vars[i]));
            }
        }


        public override bool ValidForProclaim
        {
            get { return true; }
        }

        public override bool ValidForEval
        {
            get { return true; }
        }
    }
}

