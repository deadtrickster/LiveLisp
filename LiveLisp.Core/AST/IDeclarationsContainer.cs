namespace LiveLisp.Core.AST
{
    using System.Collections.Generic;

    public interface IDeclarationsContainer
    {
        List<Declaration> Declarations { get; }
    }
}

