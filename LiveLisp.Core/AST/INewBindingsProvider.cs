using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.AST
{
    interface INewLexicalBindingsProvider
    {
       /* /// <summary>
        /// Note to implementors - don't include special var (declared either globally or locally) 
        * 
        * obsolete - to do this we need to pass an environment objcet
        /// </summary>*/

        /// <summary>
        /// List of all newly introduced local variables.
        /// 
        /// NOTE to client this list migth include special variables proclaimed or declared in outer scope
        /// 
        /// however this list doesn't include variables locally declared as special
        /// </summary>
        List<Symbol> IntroducesLexicalVariables
        {
            get;
        }
    }
}
