using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;
using System.Reflection;

namespace LiveLisp.Core.AST.Expressions
{
    public class ClrConstructorExpression : ClrMethodExpression
    {

        private ClrConstructorExpression(MethodAttributes mattrs, ClrMethodLambdaList llist, List<Expression> forms, ExpressionContext context)
            : base(mattrs, llist, forms, context, true)
        {

        }

        public static ClrConstructorExpression Create(StaticTypeResolver baseType, MethodAttributes mattrs, ClrMethodLambdaList llist, List<Expression> forms, ExpressionContext context)
        {
            forms.Insert(0, new ClrNewExpression(baseType, context));

            return new ClrConstructorExpression(mattrs, llist, forms, context);
        }

    }
}
