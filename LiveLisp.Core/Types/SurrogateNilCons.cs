using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.Types
{
    public class SurrogateNilCons : Cons
    {
        public SurrogateNilCons()
        {
            _properlist = true;
            cdr = car = DefinedSymbols.NIL;
        }

        public override Cons MakeCopy()
        {
            return new SurrogateNilCons();
        }
    }

    
}
