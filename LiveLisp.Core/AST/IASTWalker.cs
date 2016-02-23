namespace LiveLisp.Core.AST
{
    using LiveLisp.Core.AST.Expressions;
    using System;

    public interface IASTWalker
    {
        void BlockExpression(LiveLisp.Core.AST.Expressions.BlockExpression blockExpression, ExpressionContext context);
        void CallExpression(LiveLisp.Core.AST.Expressions.CallExpression callExpression, ExpressionContext context);
        void CatchExpression(LiveLisp.Core.AST.Expressions.CatchExpression catchExpression, ExpressionContext context);
        void ClrClassExpression(LiveLisp.Core.AST.Expressions.ClrClassExpression clrClassExpression, ExpressionContext context);
        void ClrDelegateExpression(LiveLisp.Core.AST.Expressions.ClrDelegateExpression clrDelegateExpression, ExpressionContext context);
        void ClrEnumExpression(LiveLisp.Core.AST.Expressions.ClrEnumExpression clrEnumExpression, ExpressionContext context);
        void ClrMethodExpression(LiveLisp.Core.AST.Expressions.ClrMethodExpression clrMethodExpression, ExpressionContext context);
        void ConstantExpression(LiveLisp.Core.AST.Expressions.ConstantExpression constantExpression, ExpressionContext context);
        void EvalWhenExpression(LiveLisp.Core.AST.Expressions.EvalWhenExpression evalWhenExpression, ExpressionContext context);
        void FletExpression(LiveLisp.Core.AST.Expressions.FletExpression fletExpression, ExpressionContext context);
        void FunctionExpression(LiveLisp.Core.AST.Expressions.FunctionExpression functionExpression, ExpressionContext context);
        void GoExpression(LiveLisp.Core.AST.Expressions.GoExpression goExpression, ExpressionContext context);
        void IfExpression(LiveLisp.Core.AST.Expressions.IfExpression ifExpression, ExpressionContext context);
        void ILCodeExpression(LiveLisp.Core.AST.Expressions.CLR.ILCodeExpression iLCodeExpression, ExpressionContext context);
        void LabelsExpression(LiveLisp.Core.AST.Expressions.LabelsExpression labelsExpression, ExpressionContext context);
        void LetExpression(LiveLisp.Core.AST.Expressions.LetExpression letExpression, ExpressionContext context);
        void LetStarExpression(LiveLisp.Core.AST.Expressions.LetStarExpression letStarExpression, ExpressionContext context);
        void LoadTimeValueExpression(LoadTimeValue loadTimeValue, ExpressionContext context);
        void LocallyExpression(LiveLisp.Core.AST.Expressions.LocallyExpression locallyExpression, ExpressionContext context);
        void MacroletExpression(LiveLisp.Core.AST.Expressions.MacroletExpression macroletExpression, ExpressionContext context);
        void MultipleValueCallExpression(LiveLisp.Core.AST.Expressions.MultipleValueCallExpression multipleValueCallExpression, ExpressionContext context);
        void MultipleValueProg1Expression(LiveLisp.Core.AST.Expressions.MultipleValueProg1Expression multipleValueProg1Expression, ExpressionContext context);
        void PrognExpression(LiveLisp.Core.AST.Expressions.PrognExpression prognExpression, ExpressionContext context);
        void ProgvExpression(LiveLisp.Core.AST.Expressions.ProgvExpression progvExpression, ExpressionContext context);
        void ReturnFromExpression(LiveLisp.Core.AST.Expressions.ReturnFromExpression returnFromExpression, ExpressionContext context);
        void SetqExpression(LiveLisp.Core.AST.Expressions.SetqExpression setqExpression, ExpressionContext context);
        void SymbolFunctionDesignator(LiveLisp.Core.AST.SymbolFunctionDesignator symbolFunctionDesignator, ExpressionContext context);
        void SymbolMacroletExpression(LiveLisp.Core.AST.Expressions.SymbolMacroletExpression symbolMacroletExpression, ExpressionContext context);
        void TagbodyExpression(TagBodyExpression tagBodyExpression, ExpressionContext context);
        void TheExpression(LiveLisp.Core.AST.Expressions.TheExpression theExpression, ExpressionContext context);
        void ThrowExpression(LiveLisp.Core.AST.Expressions.ThrowExpression throwExpression, ExpressionContext context);
        void UnwindProtectExpression(LiveLisp.Core.AST.Expressions.UnwindProtectExpression unwindProtectExpression, ExpressionContext context);
        void VariableExpression(LiveLisp.Core.AST.Expressions.VariableExpression variableExpression, ExpressionContext context);

        void MethodInfoFunctionDesignator(MethodInfoFunctionDesignator methodInfoFunctionDesignator, ExpressionContext context);

        void EmitMethodInfoDesignatorCall(MethodInfoFunctionDesignator methodInfoFunctionDesignator, CallExpression call, ExpressionContext context);

        void EmitMethodInfoDesignatorCall(SymbolFunctionDesignator symbolFunctionDesignator, CallExpression call, ExpressionContext context);

        void ClrNewExpression(ClrNewExpression clrNewExpression, ExpressionContext context);

        void LambdaOverloadsExpression(LambdaOverloads lambdaOverloads, ExpressionContext context);

        void LambdaOverload(LambdaOverload lambdaOverload, ExpressionContext context);

        void ClrIfExpression(LiveLisp.Core.AST.Expressions.ClrIfExpression clrIfExpression, ExpressionContext context);

        void ClrConstantExpression(ClrConstantExpression clrConstantExpression, ExpressionContext context);

        void EmitFunctionDesignatorCall(FunctionExpression functionExpression, CallExpression call, ExpressionContext context);

        void ClrTryExpression(ClrTryExpression clrTryExpression, ExpressionContext context);
    }
}

