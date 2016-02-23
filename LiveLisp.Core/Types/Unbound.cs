using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;

namespace LiveLisp.Core.Types
{
    public sealed class UnboundValue
    {
        private UnboundValue()
        {

        }

        public static UnboundValue Unbound = new UnboundValue();
    }
}
