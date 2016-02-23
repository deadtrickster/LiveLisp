using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types;

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        #region special_ops
        public static Symbol Block;
        public static Symbol Catch;
        public static Symbol EvalWhen;
        public static Symbol Flet;
        public static Symbol Function;
        public static Symbol Go;
        public static Symbol If;
        public static Symbol Labels;
        public static Symbol Let;
        public static Symbol LetStar;
        public static Symbol LoadTimeValue;
        public static Symbol Locally;
        public static Symbol Macrolet;
        public static Symbol MultipleValueCall;
        public static Symbol MultipleValueProg1;
        public static Symbol Progn;
        public static Symbol Progv;
        public static Symbol Quote;
        public static Symbol ReturnFrom;
        public static Symbol Setq;
        public static Symbol SymbolMacrolet;
        public static Symbol Tagbody;
        public static Symbol The;
        public static Symbol Throw;
        public static Symbol UnwindProtect;
        public static Symbol ILCode;
        public static Symbol ClrTry;
        public static Symbol ClrClass;
        public static Symbol ClrDelegate;
        public static Symbol ClrEnum;
        public static Symbol ClrTypeExpression;
        #endregion

        #region il opcodes
        
        // standard
        public const int IL_OPCODES_START = 33;
        public static Symbol Add;
        public static Symbol Add_Ovf;
        public static Symbol Add_Ovf_Un;
        public static Symbol And;
        public static Symbol Arglist;
        public static Symbol Beq;
        public static Symbol Bge;
        public static Symbol Bge_Un;
        public static Symbol Bgt;
        public static Symbol Bgt_Un;
        public static Symbol Ble;
        public static Symbol Ble_Un;
        public static Symbol Blt;
        public static Symbol Blt_Un;
        public static Symbol Bne_Un;
        public static Symbol Box;
        public static Symbol Br;
        public static Symbol Break;
        public static Symbol Brfalse;
        public static Symbol Brtrue;
        public static Symbol Call;
        public static Symbol Calli;
        public static Symbol Callvirt;
        public static Symbol Castclass;
        public static Symbol Ceq;
        public static Symbol Cgt;
        public static Symbol Cgt_Un;
        public static Symbol Ckfinite;
        public static Symbol Clt;
        public static Symbol Clt_Un;
        public static Symbol Constrained;
        public static Symbol Conv_I;
        public static Symbol Conv_I1;
        public static Symbol Conv_I2;
        public static Symbol Conv_I4;
        public static Symbol Conv_I8;
        public static Symbol Conv_Ovf_I;
        public static Symbol Conv_Ovf_I_Un;
        public static Symbol Conv_Ovf_I1;
        public static Symbol Conv_Ovf_I1_Un;
        public static Symbol Conv_Ovf_I2;
        public static Symbol Conv_Ovf_I2_Un;
        public static Symbol Conv_Ovf_I4;
        public static Symbol Conv_Ovf_I4_Un;
        public static Symbol Conv_Ovf_I8;
        public static Symbol Conv_Ovf_I8_Un;
        public static Symbol Conv_Ovf_U;
        public static Symbol Conv_Ovf_U_Un;
        public static Symbol Conv_Ovf_U1;
        public static Symbol Conv_Ovf_U1_Un;
        public static Symbol Conv_Ovf_U2;
        public static Symbol Conv_Ovf_U2_Un;
        public static Symbol Conv_Ovf_U4;
        public static Symbol Conv_Ovf_U4_Un;
        public static Symbol Conv_Ovf_U8;
        public static Symbol Conv_Ovf_U8_Un;
        public static Symbol Conv_R_Un;
        public static Symbol Conv_R4;
        public static Symbol Conv_R8;
        public static Symbol Conv_U;
        public static Symbol Conv_U1;
        public static Symbol Conv_U2;
        public static Symbol Conv_U4;
        public static Symbol Conv_U8;
        public static Symbol Cpblk;
        public static Symbol Cpobj;
        public static Symbol Div;
        public static Symbol Div_Un;
        public static Symbol Dup;
        public static Symbol Endfilter;
        public static Symbol Endfinally;
        public static Symbol Initblk;
        public static Symbol Initobj;
        public static Symbol Isint;
        public static Symbol Jmp;
        public static Symbol Ldarg;
        public static Symbol Ldarga;
        public static Symbol Ldc_I4;
        public static Symbol Ldc_I8;
        public static Symbol Ldc_R4;
        public static Symbol Ldc_R8;
        public static Symbol Ldelem;
        public static Symbol Ldelem_I;
        public static Symbol Ldelem_I1;
        public static Symbol Ldelem_I2;
        public static Symbol Ldelem_I4;
        public static Symbol Ldelem_I8;
        public static Symbol Ldelem_R4;
        public static Symbol Ldelem_R8;
        public static Symbol Ldelem_Ref;
        public static Symbol Ldelem_U1;
        public static Symbol Ldelem_U2;
        public static Symbol Ldelem_U4;
        public static Symbol Ldelema;
        public static Symbol Ldfld;
        public static Symbol Ldflda;
        public static Symbol Ldftn;
        public static Symbol Ldind_I;
        public static Symbol Ldind_I1;
        public static Symbol Ldind_I2;
        public static Symbol Ldind_I4;
        public static Symbol Ldind_I8;
        public static Symbol Ldind_R4;
        public static Symbol Ldind_R8;
        public static Symbol Ldind_Ref;
        public static Symbol Ldind_U1;
        public static Symbol Ldind_U2;
        public static Symbol Ldind_U4;
        public static Symbol Ldlen;
        public static Symbol Ldloc;
        public static Symbol Ldloca;
        public static Symbol Ldnull;
        public static Symbol Ldobj;
        public static Symbol Ldsfld;
        public static Symbol Ldsflda;
        public static Symbol Ldstr;
        public static Symbol Ldtoken;
        public static Symbol Ldvirtftn;
        public static Symbol Leave;
        public static Symbol Localloc;
        public static Symbol Mkrefany;
        public static Symbol Mul;
        public static Symbol Mul_Ovf;
        public static Symbol Mul_Ovf_Un;
        public static Symbol Neg;
        public static Symbol Newarr;
        public static Symbol Newobj;
        public static Symbol Nop;
        public static Symbol Not;
        public static Symbol Or;
        public static Symbol Pop;
        public static Symbol Readonly;
        public static Symbol Refanytype;
        public static Symbol Refanyval;
        public static Symbol Rem;
        public static Symbol Rem_Un;
        public static Symbol Ret;
        public static Symbol Rethrow;
        public static Symbol Shl;
        public static Symbol Shr;
        public static Symbol Shr_Un;
        public static Symbol Sizeof;
        public static Symbol Starg;
        public static Symbol Stelem;
        public static Symbol Stelem_I;
        public static Symbol Stelem_I1;
        public static Symbol Stelem_I2;
        public static Symbol Stelem_I4;
        public static Symbol Stelem_I8;
        public static Symbol Stelem_R4;
        public static Symbol Stelem_R8;
        public static Symbol Stelem_Ref;
        public static Symbol Stfld;
        public static Symbol Stind_I;
        public static Symbol Stind_I1;
        public static Symbol Stind_I2;
        public static Symbol Stind_I4;
        public static Symbol Stind_I8;
        public static Symbol Stind_R4;
        public static Symbol Stind_R8;
        public static Symbol Stind_Ref;
        public static Symbol Stloc;
        public static Symbol Stobj;
        public static Symbol Stsfld;
        public static Symbol Sub;
        public static Symbol Sub_Ovf;
        public static Symbol Sub_Ovf_Un;
        public static Symbol Switch;
        public static Symbol Tailcall;
        public static Symbol op_Throw;
        public static Symbol Unaligned;
        public static Symbol Unbox;
        public static Symbol Unbox_Any;
        public static Symbol Volatile;
        public static Symbol Xor;



        // extensions
        public static Symbol Lbox;
        public static Symbol Lunbox;
        public static Symbol Ldnil;
        public static Symbol Ldt;
        public static Symbol Push; // pushes constant of any type into stack;
        public static Symbol Set; // set value to lexcally visible slot
        public static Symbol Ld; // load value from lexically visible slot
        public static Symbol DeclareLocal;
        public static Symbol DefineLabel;
        public static Symbol MarkLabel;
        public static Symbol Begin_scope; // creates new lexical bindings for names e.g. (il :begin_scope x y)
        public static Symbol End_scope;

        public static Symbol Begin_catch_block;
        public static Symbol Begin_ExceptFilter_block;
        public static Symbol Begin_exception_block;
        public static Symbol Begin_fault_block;
        public static Symbol Begin_finally_block;
        public static Symbol Write_Line;
        public static Symbol End_exception_block;
        public static Symbol Mark_sequence_point;


        #region ilasm

        public const int ILASM_START = 228;
        // il file level
        public static Symbol Assembly;
        public static Symbol Class;
        public static Symbol Corflags;
        public static Symbol Custom;
        public static Symbol Data;
        public static Symbol GlobalField;
        public static Symbol File;
        public static Symbol GlobalMethod;
        public static Symbol Module;
        public static Symbol Mresource;
        public static Symbol Subsystem;
        public static Symbol VtFixup;
        public static Symbol Line;
        public static Symbol Permissionset;
        public static Symbol Permission;

        // asmdecl/asmrefdecl level
        public static Symbol Extern;
        public static Symbol Hash;
        //Custom
        public static Symbol Culture;
        public static Symbol PublicKey;
        public static Symbol Ver;
        public static Symbol PublicKeyToken;  

        #region Class
        public const int ATTRIBUTES_START = 249;
        // ClassHeader
        public static Symbol Abstract;
        public static Symbol Ansi;
        public static Symbol Auto;
        public static Symbol Autochar;
        public static Symbol Beforefieldinit;
        public static Symbol Explicit;
        public static Symbol Interface;
        public static Symbol Nested_assembly;
        public static Symbol Nested_famandassem;
        public static Symbol Nested_family;
        public static Symbol Nested_famorassem;
        public static Symbol Nested_private;
        public static Symbol Nested_public;
        public static Symbol Private;
        public static Symbol Public;
        public static Symbol Rtspecialname;
        public static Symbol Sealed;
        public static Symbol Sequential;
        public static Symbol Serializable;
        public static Symbol SpecialName;
        public static Symbol Unicode;

        public static Symbol Extends;
        public static Symbol Implements;

        public const int CLASS_MEMBERS_START = 272;
        public static Symbol Ctor;
        public static Symbol CCtor;
        public static Symbol Field;
        public static Symbol Method;
        public static Symbol Property;
        public static Symbol Event;
        public static Symbol Nested_Class;

        public static Symbol MarshalAs;
        public static Symbol Default;

        public const int MEMBER_COMMON = 281;
        public static Symbol PrivateScope;
        public static Symbol FamAndAssem;
        public const int _Assembly = ILASM_START;
        public static Symbol Family;
        public static Symbol FamOrAssem;
        public static Symbol PInvokeImpl;
        public const int ACCESS_PRIVATE = ATTRIBUTES_START + 13;
        public const int ACCESS_PUBLIC = ATTRIBUTES_START + 14;
        public static Symbol Static;
        public const int _RTSPECIALNAME = ATTRIBUTES_START + 15;


        public const int FIELD_ATTRIBUTES = 288;
        public static Symbol InitOnly;
        public static Symbol Literal;
        public static Symbol NotSerialized;
        public static Symbol HasFieldMarshal;
        public static Symbol HasDefault;
        public static Symbol HasFieldRVA;

       

        public const int METHOD_ATTRIBUTES = 295;
        const int METHOD_ATTRS_COUNT = 11;
        public static Symbol CompilerControlled;
        public static Symbol Final;
        public static Symbol HideBySig;
        public static Symbol NewSlot;
        public static Symbol Virtual;
        public static Symbol CheckAccessOnOverride;
        public static Symbol ReuseSlot;
        public static Symbol UnmanagedExport;
        public static Symbol HasSecurity;
        public static Symbol RequireSecObject;
        public static Symbol Strict;

        public const int PROPERTY_ATTRIBUTES = METHOD_ATTRIBUTES + METHOD_ATTRS_COUNT;

        #region explicit ids
        public const int FIELD = CLASS_MEMBERS_START + 2;
        public const int FIELD_PRIVATE_SCOPE = MEMBER_COMMON;
        public const int FIELD_FAMANDASSEM = FIELD_PRIVATE_SCOPE + 1;
        public const int FIELD_ASSEMBLY = ILASM_START;
        public const int FIELD_FAMILY = FIELD_PRIVATE_SCOPE + 2;
        public const int FIELD_FAMORASSEM = FIELD_PRIVATE_SCOPE + 3;
        public const int FIELD_SPECIALNAME = ATTRIBUTES_START + 19;
        public const int FIELD_PINVOKEIMPL = FIELD_PRIVATE_SCOPE + 4;
        public const int FIELD_ACCESS_PRIVATE = ACCESS_PRIVATE;
        public const int FIELD_ACCESSS_PUBLIC = ACCESS_PUBLIC;
        public const int FIELD_ACCESS_STATIC = FIELD_PRIVATE_SCOPE + 5;
        public const int FIELD_RTSPECIALNAME = _RTSPECIALNAME;
        public const int FIELD_INITONLY = FIELD_PRIVATE_SCOPE + 6;
        public const int FIELD_LITERAL = FIELD_PRIVATE_SCOPE + 7;
        public const int FIELD_NOTSERIALIZED = FIELD_PRIVATE_SCOPE + 8;
        public const int FIELD_HASFIELDMARSHAL = FIELD_PRIVATE_SCOPE + 9;
        public const int FIELD_HASDEFAULT = FIELD_PRIVATE_SCOPE + 10;
        public const int FIELD_HASFIELDRVA = FIELD_PRIVATE_SCOPE + 11;

        public const int METHOD = CLASS_MEMBERS_START + 3;
        public const int METHOD_ABSTRACT = ATTRIBUTES_START;
        public const int METHOD_ASSEMBLY = ILASM_START;
        #endregion
        #endregion



        #region unmanaged types
        
        const int UNMANAGED_TYPES_START = FIELD_HASFIELDRVA;
        public const int UNMANAGED_BOOL = UNMANAGED_TYPES_START;
        public const int UNMANAGED_I1 = UNMANAGED_TYPES_START + 1;
        public const int UNMANAGED_U1 = UNMANAGED_TYPES_START + 2;
        public const int UNMANAGED_I2 = UNMANAGED_TYPES_START + 3;
        public const int UNMANAGED_U2 = UNMANAGED_TYPES_START + 4;
        public const int UNMANAGED_I4 = UNMANAGED_TYPES_START + 5;
        public const int UNMANAGED_U4 = UNMANAGED_TYPES_START + 6;
        public const int UNMANAGED_I8 = UNMANAGED_TYPES_START + 7;
        public const int UNMANAGED_U8 = UNMANAGED_TYPES_START + 8;
        public const int UNMANAGED_R4 = UNMANAGED_TYPES_START + 9;
        public const int UNMANAGED_R8 = UNMANAGED_TYPES_START + 10;
        public const int UNMANAGED_CURRENCY = UNMANAGED_TYPES_START + 11;
        public const int UNMANAGED_BSTR = UNMANAGED_TYPES_START + 12;
        public const int UNMANAGED_LPSTR = UNMANAGED_TYPES_START + 13;
        public const int UNMANAGED_LPWSTR = UNMANAGED_TYPES_START + 14;
        public const int UNMANAGED_LPTSTR = UNMANAGED_TYPES_START + 15;
        public const int UNMANAGED_BYVALTSTR = UNMANAGED_TYPES_START + 16;
        public const int UNMANAGED_IUNKNOWN = UNMANAGED_TYPES_START + 17;
        public const int UNMANAGED_IDISPATCH = UNMANAGED_TYPES_START + 18;
        public const int UNMANAGED_STRUCT = UNMANAGED_TYPES_START + 19;
        public const int UNMANAGED_INTERFACE = UNMANAGED_TYPES_START + 20;
        public const int UNMANAGED_SAFEARRAY = UNMANAGED_TYPES_START + 21;
        public const int UNMANAGED_BYVALARRAY = UNMANAGED_TYPES_START + 22;
        public const int UNMANAGED_SYSINT = UNMANAGED_TYPES_START + 23;
        public const int UNMANAGED_SYSUINT = UNMANAGED_TYPES_START + 24;
        public const int UNMANAGED_VBBYREFSTR = UNMANAGED_TYPES_START + 25;
        public const int UNMANAGED_ANSIBSTR = UNMANAGED_TYPES_START + 26;
        public const int UNMANAGED_TBSTR = UNMANAGED_TYPES_START + 27;
        public const int UNMANAGED_VARIANTBOOL = UNMANAGED_TYPES_START + 28;
        public const int UNMANAGED_FUNCTIONPTR = UNMANAGED_TYPES_START + 29;
        public const int UNMANAGED_ASANY = UNMANAGED_TYPES_START + 30;
        public const int UNMANAGED_LPARRAY = UNMANAGED_TYPES_START + 31;
        public const int UNMANAGED_LPSTRUCT = UNMANAGED_TYPES_START + 32;
        public const int UNMANAGED_CUSTOMMARSHALER = UNMANAGED_TYPES_START + 33;
        #endregion
        #endregion
        #endregion
        static Package cl;

        public static Symbol NIL;
        public static Symbol T;
         static DefinedSymbols()
        {
        }

         internal static void InitSymbols()
         {
             cl = PackagesCollection.GetOrCreatePackage("COMMON-LISP", new string[] { "CL" });
             Block = cl.Intern("BLOCK", true);
             Catch = cl.Intern("CATCH", true);
             EvalWhen = cl.Intern("EVAL-WHEN", true);
             Flet = cl.Intern("FLET", true);
             Function = cl.Intern("FUNCTION", true);
             Go = cl.Intern("GO", true);
             If = cl.Intern("IF", true);
             Labels = cl.Intern("LABELS", true);
             Let = cl.Intern("LET", true);
             LetStar = cl.Intern("LET*", true);
             LoadTimeValue = cl.Intern("LOAD-TIME-VALUE", true);
             Locally = cl.Intern("LOCALLY", true);
             Macrolet = cl.Intern("MACROLET", true);
             MultipleValueCall = cl.Intern("MULTIPLE-VALUE-CALL", true);
             MultipleValueProg1 = cl.Intern("MULTIPLE-VALUE-PROG1", true);
             Progn = cl.Intern("PROGN", true);
             Progv = cl.Intern("PROGV", true);
             Quote = cl.Intern("QUOTE", true);
             ReturnFrom = cl.Intern("RETURN-FROM", true);
             Setq = cl.Intern("SETQ", true);
             SymbolMacrolet = cl.Intern("SYMBOL-MACROLET", true);
             Tagbody = cl.Intern("TAGBODY", true);
             The = cl.Intern("THE", true);
             Throw = cl.Intern("THROW", true);
             UnwindProtect = cl.Intern("UNWIND-PROTECT", true);
             ILCode = cl.Intern("IL", true);
             ClrTry = cl.Intern("CLRTRY", true);
             ClrClass = cl.Intern("CLRCLASS", true);
             ClrDelegate = cl.Intern("CLRDELEGATE", true);
             ClrEnum = cl.Intern("CLRENUM", true);
             ClrTypeExpression = cl.Intern("CLR-TYPE", true);

             NIL = cl.Intern("NIL", true);
             T = cl.Intern("T", true);

             IniIlopcodes();
             InitILAsm();
             InitCLOSSymbols(cl);
             DefinedSymbols.NIL = cl.Intern("NIL", true);
             DefinedSymbols._Package_ = cl.Intern("*PACKAGE*", true);

             DefinedSymbols.Read = cl.Intern("READ", true);
             DefinedSymbols._Readtable_ = cl.Intern("*READTABLE*", true);
             DefinedSymbols._Read_Base_ = cl.Intern("*READ-BASE*", true);

             DefinedSymbols.Eval = cl.Intern("EVAL", true);

             DefinedSymbols.Print = cl.Intern("PRINT", true);

             DefinedSymbols.PrintSymbolWithPackage = cl.Intern("PRINT-SYMBOL-WITH-PACKAGE");

             DefinedSymbols.EmitImportedVoidAsEmptyValues = cl.Intern("EMIT-IMPORTED-VOIDS-AS-EMPTY-VALUES");
             DefinedSymbols.KAllowOtherKeys = KeywordPackage.Instance.Intern("ALLOW-OTHER-KEYS") as KeywordSymbol;
             DefinedSymbols.LambdaAllowOtherKeys = cl.Intern("&ALLOW-OTHER-KEYS") as KeywordSymbol;

             DefinedSymbols.Declare = cl.Intern("DECLARE", true);

             DefinedSymbols.Lambda = cl.Intern("LAMBDA", true);

             DefinedSymbols.InitializeConsesDictiononarySymbols();
             DefinedSymbols.InitializeStreamsSymbols();
             DefinedSymbols.InitializeDataAndCotnrolFlowSymbols();
             DefinedSymbols.InitializeNumbersDictionary();
             DefinedSymbols.InitializeSymbolsDictionarySymbols();

             LambdaAux = cl.Intern("&AUX", true);
             LambdaBody = cl.Intern("&BODY", true);
             LambdaEnvironment = cl.Intern("&ENVIRONMENT", true);
             LambdaKey = cl.Intern("&KEY", true);
             LambdaOptional = cl.Intern("&OPTIONAL", true);
             LambdaRest = cl.Intern("&REST", true);
             LambdaWhole = cl.Intern("&WHOLE", true);
             LambdaParams = cl.Intern("&PARAMS", true);

             Special = cl.Intern("SPECIAL", true);

             SETF = cl.Intern("SETF", true);
         }

        private static void InitILAsm()
        {
            // il file level
            Assembly = KeywordPackage.Instance.Intern("ASSEMBLY");
            Class = KeywordPackage.Instance.Intern("CLASS");
            Corflags = KeywordPackage.Instance.Intern("CORFLAGS");
            Custom = KeywordPackage.Instance.Intern("CUSTOM");
            Data = KeywordPackage.Instance.Intern("DATA");
            GlobalField = KeywordPackage.Instance.Intern("GLOBAL-FIELD");
            File = KeywordPackage.Instance.Intern("FILE");
            GlobalMethod = KeywordPackage.Instance.Intern("GLOBAL-METHOD");
            Module = KeywordPackage.Instance.Intern("MODULE");
            Mresource = KeywordPackage.Instance.Intern("MRESOURCE");
            Subsystem = KeywordPackage.Instance.Intern("SUBSYSTEM");
            VtFixup = KeywordPackage.Instance.Intern("VTFIXUP");
            Line = KeywordPackage.Instance.Intern("LINE");
            Permissionset = KeywordPackage.Instance.Intern("PERMISSIONSET");
            Permission = KeywordPackage.Instance.Intern("PERMISSION");

            // asmdecl/asmrefdecl level
            Extern = KeywordPackage.Instance.Intern("EXTERN");
            Hash = KeywordPackage.Instance.Intern("HASH");
            //Custom
            Culture = KeywordPackage.Instance.Intern("CULTURE");
            PublicKey = KeywordPackage.Instance.Intern("PUBLICKEY");
            Ver = KeywordPackage.Instance.Intern("VER");
            PublicKeyToken = KeywordPackage.Instance.Intern("PUBLICKEYTOKEN");

            // ClassHeader
            Abstract = KeywordPackage.Instance.Intern("ABSTRACT");
            Ansi = KeywordPackage.Instance.Intern("ANSI");
            Auto = KeywordPackage.Instance.Intern("AUTO");
            Autochar = KeywordPackage.Instance.Intern("AUTOCHAR");
            Beforefieldinit = KeywordPackage.Instance.Intern("BEFOREFIELDINIT");
            Explicit = KeywordPackage.Instance.Intern("EXPLICIT");
            Interface = KeywordPackage.Instance.Intern("INTERFACE");
            Nested_assembly = KeywordPackage.Instance.Intern("NESTED-ASSEMBLY");
            Nested_famandassem = KeywordPackage.Instance.Intern("NESTED-FAMANDASSEM");
            Nested_family = KeywordPackage.Instance.Intern("NESTED-FAMILY");
            Nested_famorassem = KeywordPackage.Instance.Intern("NESTED-FAMORASSEM");
            Nested_private = KeywordPackage.Instance.Intern("NESTED-PRIVATE");
            Nested_public = KeywordPackage.Instance.Intern("NESTED-PUBLIC");
            Private = KeywordPackage.Instance.Intern("PRIVATE");
            Public = KeywordPackage.Instance.Intern("PUBLIC");
            Rtspecialname = KeywordPackage.Instance.Intern("RTSPECIALNAME");
            Sealed = KeywordPackage.Instance.Intern("SEALED");
            Sequential = KeywordPackage.Instance.Intern("SEQUENTIAL");
            Serializable = KeywordPackage.Instance.Intern("SERIALIZABLE");
            SpecialName = KeywordPackage.Instance.Intern("SPECIALNAME");
            Unicode = KeywordPackage.Instance.Intern("UNICODE");

            Extends = KeywordPackage.Instance.Intern("EXTENDS");
            Implements = KeywordPackage.Instance.Intern("IMPLEMENTS");

            Ctor = KeywordPackage.Instance.Intern("CTOR");
            CCtor = KeywordPackage.Instance.Intern("CCTOR");
            Field = KeywordPackage.Instance.Intern("FIELD");
            Method = KeywordPackage.Instance.Intern("METHOD");
            Property = KeywordPackage.Instance.Intern("PROPERTY");
            Event = KeywordPackage.Instance.Intern("EVENT");
            Nested_Class = KeywordPackage.Instance.Intern("NESTED-CLASS");

            MarshalAs = KeywordPackage.Instance.Intern("MARSHAL-AS");
            Default = KeywordPackage.Instance.Intern("DEFAULT");

            PrivateScope = KeywordPackage.Instance.Intern("PRIVATE-SCOPE");
            FamAndAssem = KeywordPackage.Instance.Intern("FAMANDASSEM");
            Family = KeywordPackage.Instance.Intern("FAMILY");
            FamOrAssem = KeywordPackage.Instance.Intern("FAMORASSSEM");
            SpecialName = KeywordPackage.Instance.Intern("SPECIALNAME");
            PInvokeImpl = KeywordPackage.Instance.Intern("PINVOKEIMPL");
            Static = KeywordPackage.Instance.Intern("STATIC");


            InitOnly = KeywordPackage.Instance.Intern("INITONLY");
            Literal = KeywordPackage.Instance.Intern("LITERAL");
            NotSerialized = KeywordPackage.Instance.Intern("NOTSERIALIZED");
            HasFieldMarshal = KeywordPackage.Instance.Intern("HASFIELDMARSHAL");
            HasDefault = KeywordPackage.Instance.Intern("HASDEFAULT");
            HasFieldRVA = KeywordPackage.Instance.Intern("HASFIELDRVA");

            CompilerControlled = KeywordPackage.Instance.Intern("COMPILERCONTROLLER");
            Final = KeywordPackage.Instance.Intern("FINAL");
            Virtual = KeywordPackage.Instance.Intern("VIRTUAL");
            HideBySig = KeywordPackage.Instance.Intern("HIDEBYSIG");
            CheckAccessOnOverride = KeywordPackage.Instance.Intern("CHECKACCESONOVERRIDE");
            ReuseSlot = KeywordPackage.Instance.Intern("REUSE-SLOT");
            NewSlot = KeywordPackage.Instance.Intern("NEW-SLOT");
            UnmanagedExport = KeywordPackage.Instance.Intern("UNMANAGED-EXPORT");
            HasSecurity = KeywordPackage.Instance.Intern("HASSECURITY");
            RequireSecObject = KeywordPackage.Instance.Intern("REQUIRESECOBJECT");
            Strict = KeywordPackage.Instance.Intern("STRICT");
        }

        private static void IniIlopcodes()
        {
            Add = KeywordPackage.Instance.Intern("ADD");
            Add_Ovf = KeywordPackage.Instance.Intern("ADD_OVF");
            Add_Ovf_Un = KeywordPackage.Instance.Intern("ADD_OVF_UN");
            And = KeywordPackage.Instance.Intern("AND");
            Arglist = KeywordPackage.Instance.Intern("ARGLIST");
            Beq = KeywordPackage.Instance.Intern("BEQ");
            Bge = KeywordPackage.Instance.Intern("BGE");
            Bge_Un = KeywordPackage.Instance.Intern("BGE_UN");
            Bgt = KeywordPackage.Instance.Intern("BGT");
            Bgt_Un = KeywordPackage.Instance.Intern("BGT_UN");
            Ble = KeywordPackage.Instance.Intern("BLE");
            Ble_Un = KeywordPackage.Instance.Intern("BLE_UN");
            Blt = KeywordPackage.Instance.Intern("BLT");
            Blt_Un = KeywordPackage.Instance.Intern("BLT_UN");
            Bne_Un = KeywordPackage.Instance.Intern("BNE_UN");
            Box = KeywordPackage.Instance.Intern("BOX");
            Br = KeywordPackage.Instance.Intern("BR");
            Break = KeywordPackage.Instance.Intern("BREAK");
            Brfalse = KeywordPackage.Instance.Intern("BRFALSE");
            Brtrue = KeywordPackage.Instance.Intern("BRTRUE");
            Call = KeywordPackage.Instance.Intern("CALL");
            Calli = KeywordPackage.Instance.Intern("CALLI");
            Callvirt = KeywordPackage.Instance.Intern("CALLVIRT");
            Castclass = KeywordPackage.Instance.Intern("CASTCLASS");
            Ceq = KeywordPackage.Instance.Intern("CEQ");
            Cgt = KeywordPackage.Instance.Intern("CGT");
            Cgt_Un = KeywordPackage.Instance.Intern("CGT_UN");
            Ckfinite = KeywordPackage.Instance.Intern("CKFINITE");
            Clt = KeywordPackage.Instance.Intern("CLT");
            Clt_Un = KeywordPackage.Instance.Intern("CLT_UN");
            Constrained = KeywordPackage.Instance.Intern("CONSTRAINED");
            Conv_I = KeywordPackage.Instance.Intern("CONV_I");
            Conv_I1 = KeywordPackage.Instance.Intern("CONV_I1");
            Conv_I2 = KeywordPackage.Instance.Intern("CONV_I2");
            Conv_I4 = KeywordPackage.Instance.Intern("CONV_I4");
            Conv_I8 = KeywordPackage.Instance.Intern("CONV_I8");
            Conv_Ovf_I = KeywordPackage.Instance.Intern("CONV_OVF_I");
            Conv_Ovf_I_Un = KeywordPackage.Instance.Intern("CONV_OVF_I_UN");
            Conv_Ovf_I1 = KeywordPackage.Instance.Intern("CONV_OVF_I1");
            Conv_Ovf_I1_Un = KeywordPackage.Instance.Intern("CONV_OVF_I1_UN");
            Conv_Ovf_I2 = KeywordPackage.Instance.Intern("CONV_OVF_I2");
            Conv_Ovf_I2_Un = KeywordPackage.Instance.Intern("CONV_OVF_I2_UN");
            Conv_Ovf_I4 = KeywordPackage.Instance.Intern("CONV_OVF_I4");
            Conv_Ovf_I4_Un = KeywordPackage.Instance.Intern("CONV_OVF_I4_UN");
            Conv_Ovf_I8 = KeywordPackage.Instance.Intern("CONV_OVF_I8");
            Conv_Ovf_I8_Un = KeywordPackage.Instance.Intern("CONV_OVF_I81_UN");
            Conv_Ovf_U = KeywordPackage.Instance.Intern("CONV_OVF_U");
            Conv_Ovf_U_Un = KeywordPackage.Instance.Intern("CONV_U_UN");
            Conv_Ovf_U1 = KeywordPackage.Instance.Intern("CONV_OVF_U1");
            Conv_Ovf_U1_Un = KeywordPackage.Instance.Intern("CONV_OVF_U1_UN");
            Conv_Ovf_U2 = KeywordPackage.Instance.Intern("CONV_OVF_U2");
            Conv_Ovf_U2_Un = KeywordPackage.Instance.Intern("CONV_OVF_U2_UN");
            Conv_Ovf_U4 = KeywordPackage.Instance.Intern("CONV_OVF_U4");
            Conv_Ovf_U4_Un = KeywordPackage.Instance.Intern("CONV_OVF_U4_UN");
            Conv_Ovf_U8 = KeywordPackage.Instance.Intern("CONV_OVF_U8");
            Conv_Ovf_U8_Un = KeywordPackage.Instance.Intern("CONV_OVF_U8_UN");
            Conv_R_Un = KeywordPackage.Instance.Intern("CONV_R_UN");
            Conv_R4 = KeywordPackage.Instance.Intern("CONV_R4");
            Conv_R8 = KeywordPackage.Instance.Intern("CONV_R8");
            Conv_U = KeywordPackage.Instance.Intern("CONV_U");
            Conv_U1 = KeywordPackage.Instance.Intern("CONV_U1");
            Conv_U2 = KeywordPackage.Instance.Intern("CONV_U2");
            Conv_U4 = KeywordPackage.Instance.Intern("CONV_U4");
            Conv_U8 = KeywordPackage.Instance.Intern("CONV_U8");
            Cpblk = KeywordPackage.Instance.Intern("CPBLK");
            Cpobj = KeywordPackage.Instance.Intern("CPOBJ");
            Div = KeywordPackage.Instance.Intern("DIV");
            Div_Un = KeywordPackage.Instance.Intern("DIV_UN");
            Dup = KeywordPackage.Instance.Intern("DUP");
            Endfilter = KeywordPackage.Instance.Intern("ENDFILTER");
            Endfinally = KeywordPackage.Instance.Intern("ENDFINALLY");
            Initblk = KeywordPackage.Instance.Intern("INITBLK");
            Initobj = KeywordPackage.Instance.Intern("INITOBJ");
            Isint = KeywordPackage.Instance.Intern("ISINT");
            Jmp = KeywordPackage.Instance.Intern("JMP");
            Ldarg = KeywordPackage.Instance.Intern("LDARG");
            Ldarga = KeywordPackage.Instance.Intern("LDARGA");
            Ldc_I4 = KeywordPackage.Instance.Intern("LDC_I4");
            Ldc_I8 = KeywordPackage.Instance.Intern("LDC_I8");
            Ldc_R4 = KeywordPackage.Instance.Intern("LDC_R4");
            Ldc_R8 = KeywordPackage.Instance.Intern("LDC_R8");
            Ldelem = KeywordPackage.Instance.Intern("LDELEM");
            Ldelem_I = KeywordPackage.Instance.Intern("LDELEM_I");
            Ldelem_I1 = KeywordPackage.Instance.Intern("LDELEM_I1");
            Ldelem_I2 = KeywordPackage.Instance.Intern("LDELEM_I2");
            Ldelem_I4 = KeywordPackage.Instance.Intern("LDELEM_I4");
            Ldelem_I8 = KeywordPackage.Instance.Intern("LDELEM_I8");
            Ldelem_R4 = KeywordPackage.Instance.Intern("LDELEM_R4");
            Ldelem_R8 = KeywordPackage.Instance.Intern("LDELEM_R8");
            Ldelem_Ref = KeywordPackage.Instance.Intern("LDELEM_REF");
            Ldelem_U1 = KeywordPackage.Instance.Intern("LDELEM_U1");
            Ldelem_U2 = KeywordPackage.Instance.Intern("LDELEM_U2");
            Ldelem_U4 = KeywordPackage.Instance.Intern("LDELEM_U4");
            Ldelema = KeywordPackage.Instance.Intern("LDELEMA");
            Ldfld = KeywordPackage.Instance.Intern("LDFLD");
            Ldflda = KeywordPackage.Instance.Intern("LDFLDA");
            Ldftn = KeywordPackage.Instance.Intern("LDFTN");
            Ldind_I = KeywordPackage.Instance.Intern("LDIND_I");
            Ldind_I1 = KeywordPackage.Instance.Intern("LDIND_I1");
            Ldind_I2 = KeywordPackage.Instance.Intern("LDIND_I2");
            Ldind_I4 = KeywordPackage.Instance.Intern("LDIND_I4");
            Ldind_I8 = KeywordPackage.Instance.Intern("LDIND_I8");
            Ldind_R4 = KeywordPackage.Instance.Intern("LDIND_R4");
            Ldind_R8 = KeywordPackage.Instance.Intern("LDIND_R8");
            Ldind_Ref = KeywordPackage.Instance.Intern("LDIND_REF");
            Ldind_U1 = KeywordPackage.Instance.Intern("LDIND_U1");
            Ldind_U2 = KeywordPackage.Instance.Intern("LDIND_U2");
            Ldind_U4 = KeywordPackage.Instance.Intern("LDIND_U4");
            Ldlen = KeywordPackage.Instance.Intern("LDLEN");
            Ldloc = KeywordPackage.Instance.Intern("LDLOC");
            Ldloca = KeywordPackage.Instance.Intern("LDLOCA");
            Ldnull = KeywordPackage.Instance.Intern("LDNULL");
            Ldobj = KeywordPackage.Instance.Intern("LDOBJ");
            Ldsfld = KeywordPackage.Instance.Intern("LDSFLD");
            Ldsflda = KeywordPackage.Instance.Intern("LDSFLDA");
            Ldstr = KeywordPackage.Instance.Intern("LDSTR");
            Ldtoken = KeywordPackage.Instance.Intern("LDTOKEN");
            Ldvirtftn = KeywordPackage.Instance.Intern("LDVIRTFTN");
            Leave = KeywordPackage.Instance.Intern("LEAVE");
            Localloc = KeywordPackage.Instance.Intern("LOCALLOC");
            Mkrefany = KeywordPackage.Instance.Intern("MKREFANY");
            Mul = KeywordPackage.Instance.Intern("MUL");
            Mul_Ovf = KeywordPackage.Instance.Intern("MUL_OVF");
            Mul_Ovf_Un = KeywordPackage.Instance.Intern("MUL_OVF_UN");
            Neg = KeywordPackage.Instance.Intern("NEG");
            Newarr = KeywordPackage.Instance.Intern("NEWARR");
            Newobj = KeywordPackage.Instance.Intern("NEWOBJ");
            Nop = KeywordPackage.Instance.Intern("NOP");
            Not = KeywordPackage.Instance.Intern("NOT");
            Or = KeywordPackage.Instance.Intern("OR");
            Pop = KeywordPackage.Instance.Intern("POP");
            Readonly = KeywordPackage.Instance.Intern("READONLY");
            Refanytype = KeywordPackage.Instance.Intern("REFANYTYPE");
            Refanyval = KeywordPackage.Instance.Intern("REFANYVAL");
            Rem = KeywordPackage.Instance.Intern("REM");
            Rem_Un = KeywordPackage.Instance.Intern("REM_UN");
            Ret = KeywordPackage.Instance.Intern("RET");
            Rethrow = KeywordPackage.Instance.Intern("RETHROW");
            Shl = KeywordPackage.Instance.Intern("SHL");
            Shr = KeywordPackage.Instance.Intern("SHR");
            Shr_Un = KeywordPackage.Instance.Intern("SHR_UN");
            Sizeof = KeywordPackage.Instance.Intern("SIZEOF");
            Starg = KeywordPackage.Instance.Intern("STARG");
            Stelem = KeywordPackage.Instance.Intern("STELEM");
            Stelem_I = KeywordPackage.Instance.Intern("STELEM_I");
            Stelem_I1 = KeywordPackage.Instance.Intern("STELEM_I1");
            Stelem_I2 = KeywordPackage.Instance.Intern("STELEM_I2");
            Stelem_I4 = KeywordPackage.Instance.Intern("STELEM_I4");
            Stelem_I8 = KeywordPackage.Instance.Intern("STELEM_I8");
            Stelem_R4 = KeywordPackage.Instance.Intern("STELEM_R4");
            Stelem_R8 = KeywordPackage.Instance.Intern("STELEM_R8");
            Stelem_Ref = KeywordPackage.Instance.Intern("STELEM_REF");
            Stfld = KeywordPackage.Instance.Intern("STFLD");
            Stind_I = KeywordPackage.Instance.Intern("STIND_I");
            Stind_I1 = KeywordPackage.Instance.Intern("STIND_I1");
            Stind_I2 = KeywordPackage.Instance.Intern("STIND_I2");
            Stind_I4 = KeywordPackage.Instance.Intern("STIND_I4");
            Stind_I8 = KeywordPackage.Instance.Intern("STIND_I8");
            Stind_R4 = KeywordPackage.Instance.Intern("STIND_R4");
            Stind_R8 = KeywordPackage.Instance.Intern("STIND_R8");
            Stind_Ref = KeywordPackage.Instance.Intern("STIND_REF");
            Stloc = KeywordPackage.Instance.Intern("STLOC");
            Stobj = KeywordPackage.Instance.Intern("STOBJ");
            Stsfld = KeywordPackage.Instance.Intern("STSFLD");
            Sub = KeywordPackage.Instance.Intern("SUB");
            Sub_Ovf = KeywordPackage.Instance.Intern("SUB_OVF");
            Sub_Ovf_Un = KeywordPackage.Instance.Intern("SUB_OVF_UN");
            Switch = KeywordPackage.Instance.Intern("SWITCH");
            Tailcall = KeywordPackage.Instance.Intern("TAILCALL");
            op_Throw = KeywordPackage.Instance.Intern("THROW");
            Unaligned = KeywordPackage.Instance.Intern("UNALIGNED");
            Unbox = KeywordPackage.Instance.Intern("UNBOX");
            Unbox_Any = KeywordPackage.Instance.Intern("UNBOX_ANY");
            Volatile = KeywordPackage.Instance.Intern("VOLATILE");
            Xor = KeywordPackage.Instance.Intern("XOR");



            // extensions
            Lbox = KeywordPackage.Instance.Intern("LBOX");
            Lunbox = KeywordPackage.Instance.Intern("LUNBOX");
            Ldnil = KeywordPackage.Instance.Intern("LDNIL");
            Ldt = KeywordPackage.Instance.Intern("LDT");
            Push = KeywordPackage.Instance.Intern("PUSH"); // pushes constant of any type into stack = KeywordPackage.Instance.Intern("");
            MarkLabel = KeywordPackage.Instance.Intern("MARK_LABEL");
            Set = KeywordPackage.Instance.Intern("SET");
            Ld = KeywordPackage.Instance.Intern("LD");
            DeclareLocal = KeywordPackage.Instance.Intern("DECLARE_LOCAL");
            DefineLabel = KeywordPackage.Instance.Intern("DEFINE_LABEL");
            Begin_scope = KeywordPackage.Instance.Intern("BEGIN_SCOPE");
            End_scope = KeywordPackage.Instance.Intern("END_SCOPE");

            Begin_catch_block = KeywordPackage.Instance.Intern("BEGIN_CATCH_BLOCK");
            Begin_ExceptFilter_block = KeywordPackage.Instance.Intern("BEGIN_EXCEPT_FILTER_BLOCK");
            Begin_exception_block = KeywordPackage.Instance.Intern("BEGIN_EXCEPTION_BLOCK");
            Begin_fault_block = KeywordPackage.Instance.Intern("BEGIN_FAULT_BLOCK");
            Begin_finally_block = KeywordPackage.Instance.Intern("BEGIN_FINALLY_BLOCK");
            Write_Line = KeywordPackage.Instance.Intern("WRITE_LINE");
            End_exception_block = KeywordPackage.Instance.Intern("END_EXCEPTION_BLOCK");
            Mark_sequence_point = KeywordPackage.Instance.Intern("MARK_SEQUENCE_POINT");
        }


        public static Symbol SETF;

        public static Symbol DynamicExtent;
        public static Symbol Ftype;
        public static Symbol Ignorable;
        public static Symbol Ignore;
        public static Symbol Inline;
        public static Symbol Notinline;
        public static Symbol Optimize;
        public static Symbol Special;
        public static Symbol Type;
        public static Symbol Attribute;
        public static Symbol ClrType;

        #region reader
        public static Symbol Termainal_IO;
        public static Symbol Error_Output;
        public static Symbol Query_IO;
        public static Symbol Trace_Output;
        public static Symbol Read;
        public static Symbol ReadtableCase;
        public static Symbol StandardReadtable;
        #endregion

        // lambda lists related &allow-other-keys, &aux, &body, &environment, &key, &optional, &rest, and &whole (i)&params
        public static Symbol Lambda;
        public static Symbol LambdaAux;
        public static Symbol LambdaBody;
        public static Symbol LambdaEnvironment;
        public static Symbol LambdaKey;
        public static Symbol LambdaOptional;
        public static Symbol LambdaRest;
        public static Symbol LambdaWhole;
        public static Symbol LambdaParams;
        public static Symbol LambdaAllowOtherKeys;
        public static KeywordSymbol KAllowOtherKeys;

        // lambda lists types
        public static KeywordSymbol Ordinary;
        public static KeywordSymbol Macros;

        public static Symbol Eval;
        public static Symbol Print;

        public static Symbol This;


        #region implementation related
        public static Symbol PrintSymbolWithPackage;
        public static Symbol EmitImportedVoidAsEmptyValues;
        #endregion
    }
}
