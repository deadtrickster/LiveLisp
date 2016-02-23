using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.Interpreter
{
    public interface IEvalWalker
    {
        object ConstantExpression(LiveLisp.Core.AST.Expressions.ConstantExpression constantExpression, EvaluationContext context);

        object CallExpression(LiveLisp.Core.AST.Expressions.CallExpression callExpression, EvaluationContext context);

        object SymbolFunctionDesignator(LiveLisp.Core.AST.SymbolFunctionDesignator symbolFunctionDesignator, EvaluationContext context);

        object VariableExpression(LiveLisp.Core.AST.Expressions.VariableExpression variableExpression, EvaluationContext context);

        object SetqExpression(LiveLisp.Core.AST.Expressions.SetqExpression setqExpression, EvaluationContext context);

        object LambdaFunctionDesignator(LiveLisp.Core.AST.LambdaFunctionDesignator lambdaFunctionDesignator, EvaluationContext context);

        object PrognExpression(LiveLisp.Core.AST.Expressions.PrognExpression prognExpression, EvaluationContext context);

        object BlockExpression(LiveLisp.Core.AST.Expressions.BlockExpression blockExpression, EvaluationContext context);

        object ReturnFromExpression(LiveLisp.Core.AST.Expressions.ReturnFromExpression returnFromExpression, EvaluationContext context);

        object LetExpression(LiveLisp.Core.AST.Expressions.LetExpression letExpression, EvaluationContext context);

        object TagbodyExpression(LiveLisp.Core.AST.Expressions.TagBodyExpression tagBodyExpression, EvaluationContext context);

        object CatchExpression(LiveLisp.Core.AST.Expressions.CatchExpression catchExpression, EvaluationContext context);

        object EvalWhenExpression(LiveLisp.Core.AST.Expressions.EvalWhenExpression evalWhenExpression, EvaluationContext context);

        object FletExpression(LiveLisp.Core.AST.Expressions.FletExpression fletExpression, EvaluationContext context);

        object GoExpression(LiveLisp.Core.AST.Expressions.GoExpression goExpression, EvaluationContext context);

        object IfExpression(LiveLisp.Core.AST.Expressions.IfExpression ifExpression, EvaluationContext context);

        object LabelsExpression(LiveLisp.Core.AST.Expressions.LabelsExpression labelsExpression, EvaluationContext context);

        object LetStarExpression(LiveLisp.Core.AST.Expressions.LetStarExpression letStarExpression, EvaluationContext context);

        object LoadTimeValue(LiveLisp.Core.AST.Expressions.LoadTimeValue loadTimeValue, EvaluationContext context);

        object LocallyExpression(LiveLisp.Core.AST.Expressions.LocallyExpression locallyExpression, EvaluationContext context);

        object MultipleValueCallExpression(LiveLisp.Core.AST.Expressions.MultipleValueCallExpression multipleValueCallExpression, EvaluationContext context);

        object MultipleValueProg1Expression(LiveLisp.Core.AST.Expressions.MultipleValueProg1Expression multipleValueProg1Expression, EvaluationContext context);

        object ProgvExpression(LiveLisp.Core.AST.Expressions.ProgvExpression progvExpression, EvaluationContext context);

        object SymbolMacroletExpression(LiveLisp.Core.AST.Expressions.SymbolMacroletExpression symbolMacroletExpression, EvaluationContext context);

        object TheExpression(LiveLisp.Core.AST.Expressions.TheExpression theExpression, EvaluationContext context);

        object ThrowExpression(LiveLisp.Core.AST.Expressions.ThrowExpression throwExpression, EvaluationContext context);

        object UnwindProtectExpression(LiveLisp.Core.AST.Expressions.UnwindProtectExpression unwindProtectExpression, EvaluationContext context);
    }
}
