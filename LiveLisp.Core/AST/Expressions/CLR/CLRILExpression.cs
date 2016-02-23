using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;
using System.Reflection.Emit;
using LiveLisp.Core.Types;
using LiveLisp.Core.AST;
using LiveLisp.Core.CLOS;

namespace LiveLisp.Core.AST.Expressions.CLR
{
    /// <summary>
    /// represent one msil instruction
    /// or declaration
    /// </summary>
    public class CLRILExpression : Expression
    {
        public CLRILExpression(ExpressionContext context)
            : base(context)
        {

        }




        public override object Eval(LiveLisp.Core.Interpreter.IEvalWalker evaluator, LiveLisp.Core.Interpreter.EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }

    /* syntax
     * (push <constant>)
     * (il :bge <label>)
     * 
     * например
     * 
     * (il :define_label label1)
     * (il :push 1)
     * (il :push 2)
     * (il :bge label1)
     * (il :define_label label2)
     * (il :push "one greater then two")
     * (il :calli (SystemConsole WriteLine String))
     * (il :br label2)
     * (il :mark_label label1)
     * (il :push "one greater then two")
     * (il :calli (SystemConsole WriteLine String))
     * (il :mark_label label2)
     */
 }
