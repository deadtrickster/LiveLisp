using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.AST;
using LiveLisp.Core.AST.Expressions;

namespace LiveLisp.Core.Eval
{
    /// <summary>
    /// Используя AST заполняет CompilationContext;
    /// </summary>
    public class Translator : IASTWalker
    {

        #region IASTWalker Members

        public void BlockExpression(BlockExpression blockExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void CallExpression(CallExpression callExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void CatchExpression(CatchExpression catchExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrClassExpression(ClrClassExpression clrClassExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrDelegateExpression(ClrDelegateExpression clrDelegateExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrEnumExpression(ClrEnumExpression clrEnumExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrMethodExpression(ClrMethodExpression clrMethodExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ConstantExpression(ConstantExpression constantExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void EvalWhenExpression(EvalWhenExpression evalWhenExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void FletExpression(FletExpression fletExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void FunctionExpression(FunctionExpression functionExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void GoExpression(GoExpression goExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void IfExpression(IfExpression ifExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ILCodeExpression(LiveLisp.Core.AST.Expressions.CLR.ILCodeExpression iLCodeExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LabelsExpression(LabelsExpression labelsExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LetExpression(LetExpression letExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LetStarExpression(LetStarExpression letStarExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LoadTimeValueExpression(LoadTimeValue loadTimeValue, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void LocallyExpression(LocallyExpression locallyExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void MacroletExpression(MacroletExpression macroletExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void MultipleValueCallExpression(MultipleValueCallExpression multipleValueCallExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void MultipleValueProg1Expression(MultipleValueProg1Expression multipleValueProg1Expression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void PrognExpression(PrognExpression prognExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ProgvExpression(ProgvExpression progvExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ReturnFromExpression(ReturnFromExpression returnFromExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void SetqExpression(SetqExpression setqExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void SymbolFunctionDesignator(SymbolFunctionDesignator symbolFunctionDesignator, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void SymbolMacroletExpression(SymbolMacroletExpression symbolMacroletExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void TagbodyExpression(TagBodyExpression tagBodyExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void TheExpression(TheExpression theExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ThrowExpression(ThrowExpression throwExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void UnwindProtectExpression(UnwindProtectExpression unwindProtectExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void VariableExpression(VariableExpression variableExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void MethodInfoFunctionDesignator(MethodInfoFunctionDesignator methodInfoFunctionDesignator, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void EmitMethodInfoDesignatorCall(MethodInfoFunctionDesignator methodInfoFunctionDesignator, CallExpression call, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void EmitMethodInfoDesignatorCall(SymbolFunctionDesignator symbolFunctionDesignator, CallExpression call, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrNewExpression(ClrNewExpression clrNewExpression, ExpressionContext context)
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

        public void ClrIfExpression(ClrIfExpression clrIfExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrConstantExpression(ClrConstantExpression clrConstantExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void EmitFunctionDesignatorCall(FunctionExpression functionExpression, CallExpression call, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        public void ClrTryExpression(ClrTryExpression clrTryExpression, ExpressionContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
