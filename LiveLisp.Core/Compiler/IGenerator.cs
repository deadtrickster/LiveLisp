using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using LiveLisp.Core.AST;

namespace LiveLisp.Core.Compiler
{
    internal interface IGenerator
    {
        void Generate(ILGenerator gen);
    }
}
