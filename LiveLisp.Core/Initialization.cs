using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Reader;
using System.Reflection;
using LiveLisp.Core.Utils;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.BuiltIns.Reader;
using LiveLisp.Core.BuiltIns.DataAndControlFlow;
using LiveLisp.Core.CLOS;
namespace LiveLisp.Core
{
    public partial class Initialization
    {
        static bool initialized = false;

        static Package SystemPackage;
        static Package CommonLispPackage;
        static Package CommonLispUserPackage;

        public static void Initialize()
        {
            if (initialized)
                return;

            InitializePackages();
            InitializeSymbols();


            Settings.UseCompilerInREPL = true;
            InitializeVariables();
     //       InitializeCurrentPackage();

            InitializeClasses();

            InitializeManuallyDefinedFunctions();

            InitializeReader();

            InitializeFunctions();

            Settings.UseCompilerInREPL = false;
        }

        private static void InitializeClasses()
        {
        }

        private static void InitializePackages()
        {
            SystemPackage = PackagesCollection.GetOrCreatePackage("SYSTEM", new string[] { "SYS" });
            if (CommonLispPackage == null)
            {
                CommonLispPackage = PackagesCollection.GetOrCreatePackage("COMMON-LISP", new string[] { "CL" });
            }
            else
            {
                CommonLispPackage = PackagesCollection.FindByNameOrNickname("COMMON-LISP");
            }
            CommonLispPackage.Use(SystemPackage);
            CommonLispUserPackage = PackagesCollection.GetOrCreatePackage("COMMON-LISP-USER", new string[] { "CLU", "CL-USER" }, new Package[] { SystemPackage, CommonLispPackage });
        }

        private static void InitializeSymbols()
        {
            DefinedSymbols.InitSymbols();
        }

        private static void InitializeVariables()
        {
            DefinedSymbols.NIL.IsConstant = true;
            DefinedSymbols.NIL.Value = DefinedSymbols.NIL;
            DefinedSymbols._Package_.IsDynamic = true;
            DefinedSymbols._Package_.Value = CommonLispPackage;

            DefinedSymbols._Readtable_.IsDynamic = true;

            DefinedSymbols.PrintSymbolWithPackage.Value = DefinedSymbols.NIL;

            DefinedSymbols.EmitImportedVoidAsEmptyValues.Value = DefinedSymbols.NIL;
        }

        private static void InitializeReader()
        {
            InitReadtable();
        }

        private static void InitReadtable()
        {
            Type[] ReaderMacroTypes = ReflectionUtils.GetTypesMarkedwithAttr(new Assembly[] { Assembly.GetExecutingAssembly() }, typeof(ReaderMacroAttribute));

            MethodInfo[] ReaderMacros = ReflectionUtils.GetMethodMarkedwithAttr(ReaderMacroTypes, typeof(ReaderMacroAttribute));

            Dictionary<char, LispFunction> macroTable = new Dictionary<char, LispFunction>();

            Dictionary<char, MacroDispatchLambda> dtable = new Dictionary<char, MacroDispatchLambda>();

            foreach (var method in ReaderMacros)
            {
                ReaderMacroAttribute attr = ReflectionUtils.GetFirstAttrInstance<ReaderMacroAttribute>(method);

                if (attr.Dispatch != 0)
                {
                    MacroDispatchLambda dmethod;
                    if (!dtable.ContainsKey(attr.Dispatch))
                    {
                        dmethod = new MacroDispatchLambda(attr.Dispatch);

                        macroTable.Add(attr.Dispatch, dmethod);

                        dtable.Add(attr.Dispatch, dmethod);
                    }
                    else
                    {
                        dmethod = dtable[attr.Dispatch];
                    }

                    dmethod.AddOrReplaceSubcharacter(attr.Char, ClrMethodImporter.Import(SystemPackage.Intern(method.Name), method));
                }
                else
                {
                    if (macroTable.ContainsKey(attr.Char))
                    {
                        throw new NotImplementedException("macro already defined. " + attr.Char + ".");
                    }

                    macroTable.Add(attr.Char, ClrMethodImporter.Import(SystemPackage.Intern(method.Name), method));
                }
            }

            Readtable.Current = Readtable.CreateDefault(macroTable);
        }

        private static void InitializeManuallyDefinedFunctions()
        {
            InitReaderFunctions();

            DefinedSymbols.Values.Function = new ValuesFunction();
            DefinedSymbols.ValuesList.Function = new ValuesListFunction();
        }

        private static void InitReaderFunctions()
        {
            Readtable.Current = Readtable.CreateDefault(new Dictionary<char, LispFunction>());

            DefinedSymbols.Read.Function = ClrMethodImporter.Import(DefinedSymbols.Read, typeof(ReaderDictionary).GetMethod("Read"));
        }

        private static void InitializeCurrentPackage()
        {
            DefinedSymbols._Package_.Value = CommonLispUserPackage;
        }

        private static void InitializeFunctions()
        {
            Type[] BuiltinsTypes = ReflectionUtils.GetTypesMarkedwithAttr(new Assembly[] { Assembly.GetCallingAssembly() }, typeof(BuiltinsContainerAttribute));

            foreach (var type in BuiltinsTypes)
            {
                BuiltinsContainerAttribute tattr = ReflectionUtils.GetFirstAttrInstance<BuiltinsContainerAttribute>(type);

                Package pack;

                if (tattr.Package != null)
                {
                    pack = PackagesCollection.GetOrCreatePackage(tattr.Package);
                }
                else
                {
                    pack = Package.Current;
                }

                MethodInfo[] Methods = ReflectionUtils.GetMethodMarkedwithAttr(new Type[] { type }, typeof(BuiltinAttribute));

                foreach (var method in Methods)
                {
                    BuiltinAttribute methodattr = ReflectionUtils.GetFirstAttrInstance<BuiltinAttribute>(method);
                    Symbol symbol;
                    string symbolName = (methodattr.Name ?? method.Name).ToUpper();
                    if (methodattr.OverridePackage == true)
                        symbol = ReaderDictionary.ParseSymbol(symbolName);
                    else
                        symbol = pack.Intern(symbolName, true);

                    if (method.ReturnType == typeof(void))
                    {
                        methodattr.ValuesReturnPolitics = ValuesReturnPolitics.Void;
                    }

                    LispFunction lm = ClrMethodImporter.Import(symbol, method);

                    symbol.Function = lm;
                }
            }
        }
    }
}
