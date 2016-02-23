namespace LiveLisp.Core.AST
{
    using System.Collections.Generic;

    public interface IImplicitProgn
    {
        List<Expression> Forms { get; }

        Expression this[int index]
        { get; }

        int Count
        {
            get;
        }

        Expression Last
        {
            get;
        }
    }
}

