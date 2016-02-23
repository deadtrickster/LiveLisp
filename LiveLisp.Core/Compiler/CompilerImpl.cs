using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using LiveLisp.Core.Runtime;

namespace LiveLisp.Core.Compiler
{
    public class CompilerImpl
    {

        static CompilerImpl()
        {
            #if !SILVERLIGHT
            assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("LambdasHolder"), AssemblyBuilderAccess.RunAndSave);
#else
            assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("LambdasHolder"), AssemblyBuilderAccess.Run);
#endif
            module = assembly.DefineDynamicModule("LambdasHolder.dll");

            AssemblyCache.AddAssembly(assembly);
        }
        #if !SILVERLIGHT
        static public void SaveAssembly()
        {
            assembly.Save("LambdasHolder.dll");
        }
#endif
        //TODO: make private
        private static AssemblyBuilder assembly;

        //TODO: make private
        private static ModuleBuilder module;

        /// <summary>
        /// Compiles context, if mainGenerator is null 'global' instructions not allowed.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mainGenerator"></param>
        /// <returns>Newly created type, if existed</returns>
        public Type[] Comple(CompilationContext context, ILGenerator mainGenerator)
         {
            Type[] types = new Type[context.new_classes.Count]; 
            if (context.new_classes.Count != 0)
            {
                BuildNewTypes(context);
            }

            context.MainInstructionsBlock.Create(mainGenerator);

            for (int i = 0; i < context.new_classes.Count; i++)
            {
                types[i] = context.new_classes[i].CreatedType;
            }

            return types;
        }

        internal Type[] Comple(CompilationContext ccontext)
        {
            if (ccontext.MainInstructionsBlock.Count > 0)
                throw new FormattedException("top level code not allowed");

            Type[] types = new Type[ccontext.new_classes.Count];
            if (ccontext.new_classes.Count != 0)
            {
                BuildNewTypes(ccontext);
            }

            for (int i = 0; i < ccontext.new_classes.Count; i++)
            {
                types[i] = ccontext.new_classes[i].CreatedType;
            }

            return types;
        }

        private void BuildNewTypes(CompilationContext context)
        {
            for (int i = 0; i < context.new_classes.Count; i++)
            {
                context.new_classes[i].CreateBuilders(module);
            }

            for (int i = 0; i < context.new_classes.Count; i++)
            {
                context.new_classes[i].Create();
            }
        }
    }
}
