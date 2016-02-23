using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.AST.Expressions;
using LiveLisp.Core.AST;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.Compiler
{
    public static class ClrMethodImporter
    {
        public static LispFunction Import(MethodInfo method)
        {
            return Import(new Symbol(method.Name), method);
        }

        public static LispFunction Import(Symbol name, MethodInfo method)
        {
            return Import(name, OrdinaryLambdaList.CreateFromMethod(method), method);
        }

        public static LispFunction Import(Symbol name, LambdaList lambdaList, MethodInfo method)
        {
            List<Expression> vars = new List<Expression>(lambdaList.Count);

            for (int i = 0; i < lambdaList.Count; i++)
            {
                vars.Add(new VariableExpression(lambdaList[i].Name));
            }

            MethodInfoFunctionDesignator fname = new MethodInfoFunctionDesignator(method, ExpressionContext.Root);

            CallExpression call = new CallExpression(fname, vars, ExpressionContext.Root);

            LambdaFunctionDesignator lfd = new LambdaFunctionDesignator(name, lambdaList, call);

            FunctionExpression fexp = new FunctionExpression(lfd, ExpressionContext.Root);

            return new DefaultASTCompiler().Compile(fexp).DynamicInvoke() as LispFunction;
        }
    }
}
