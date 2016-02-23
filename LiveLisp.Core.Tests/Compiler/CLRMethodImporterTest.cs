using LiveLisp.Core.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using LiveLisp.Core.Types;
using System.Runtime.InteropServices;
using System.Text;
using LiveLisp.Core;
using LiveLisp.Core.CLOS;
//using LiveLisp.Core.BuiltIns.Strings;
namespace LiveLisp.Core.Tests.Compiler
{
    
    
    /// <summary>
    ///This is a test class for CLRMethodImporterTest and is intended
    ///to contain all CLRMethodImporterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CLRMethodImporterTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        static Symbol WriteSymbol;
        static Symbol WriteLineSymbol;

        [ClassInitialize()]
        public static void CLRMethodImporterTestInitialize(TestContext testContext)
        {
            Initialization.Initialize();
            // нужно создать пакет и символы
            Package TestClassPackage = PackagesCollection.CreatePackage("TestClass");
            WriteSymbol = TestClassPackage.Intern("Write", true);
            WriteLineSymbol = TestClassPackage.Intern("WriteLine", true);
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Import
        ///</summary>
        [TestMethod()]
        public void ImportStaticTest()
        {
            Type t = typeof(TestClass);
            var method = t.GetMethod("Write", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(object), typeof(object), typeof(object) }, null);
            LambdaList ll = OrdinaryLambdaList.CreateFromMethod(method);
            Assert.AreEqual(1, ll.MinParamsCount,"min");
            Assert.AreEqual(2, ll.MaxParamsCount,"max");
            WriteSymbol.Function = ClrMethodImporter.Import(WriteSymbol, method);
            Assert.AreEqual(false, WriteSymbol.Function.IsUnbound);
            CompilerImpl.SaveAssembly();
            WriteSymbol.Invoke("string");

            Assert.AreEqual("Write(object string, [Optional] object default)", TestClass.StateField);

            WriteSymbol.Invoke("string", "another_string");

            Assert.AreEqual("Write(object string, [Optional] object another_string)", TestClass.StateField);


            WriteSymbol.Invoke(new object[] { "string" });

            Assert.AreEqual("Write(object string, [Optional] object default)", TestClass.StateField);

            WriteSymbol.Invoke(new object[] { "string", "another_string" });

            Assert.AreEqual("Write(object string, [Optional] object another_string)", TestClass.StateField);
        }

        /// <summary>
        ///A test for Import
        ///</summary>
        [TestMethod()]
        public void ImportStaticRestTest()
        {
            Type t = typeof(TestClass);
            var method = t.GetMethod("Write", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(object), typeof(Cons) }, null);
            LambdaList ll = OrdinaryLambdaList.CreateFromMethod(method);
            Assert.AreEqual(1, ll.MinParamsCount, "min");
            Assert.AreEqual(int.MaxValue, ll.MaxParamsCount, "max");
            WriteSymbol.Function = ClrMethodImporter.Import(WriteSymbol, method);
            Assert.AreEqual(false, WriteSymbol.Function.IsUnbound);
            CompilerImpl.SaveAssembly();
            WriteSymbol.Invoke("string");

            Assert.AreEqual("string ", TestClass.StateField);

            WriteSymbol.Invoke("string", "another_string");

            Assert.AreEqual("string another_string ", TestClass.StateField);

            WriteSymbol.Invoke("string", "another_string", "yet_another_string");

            Assert.AreEqual("string another_string yet_another_string ", TestClass.StateField);

            WriteSymbol.Invoke(new object[] { "string" });

            Assert.AreEqual("string ", TestClass.StateField);

            WriteSymbol.Invoke(new object[] { "string", "another_string" });

            Assert.AreEqual("string another_string ", TestClass.StateField);

            WriteSymbol.Invoke(new object[] { "string", "another_string", "yet_another_string" });

            Assert.AreEqual("string another_string yet_another_string ", TestClass.StateField);
            
        }

        [TestMethod()]
        public void ImportStaticOptionalRestTest()
        {
            Type t = typeof(TestClass);
            var method = t.GetMethod("Write", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(object), typeof(object), typeof(Cons) }, null);
            LambdaList ll = OrdinaryLambdaList.CreateFromMethod(method);
            Assert.AreEqual(1, ll.MinParamsCount, "min");
            Assert.AreEqual(int.MaxValue, ll.MaxParamsCount, "max");
            WriteSymbol.Function = ClrMethodImporter.Import(WriteSymbol, method);
            Assert.AreEqual(false, WriteSymbol.Function.IsUnbound);
            CompilerImpl.SaveAssembly();
            WriteSymbol.Invoke("string");

            Assert.AreEqual("string default ", TestClass.StateField);

            WriteSymbol.Invoke("string", "another_string");

            Assert.AreEqual("string another_string ", TestClass.StateField);

            WriteSymbol.Invoke("string", "another_string", "yet_another_string");

            Assert.AreEqual("string another_string yet_another_string ", TestClass.StateField);

            /*WriteSymbol.Invoke(new object[] { "string" });

            Assert.AreEqual("string ", TestClass.StateField);

            WriteSymbol.Invoke(new object[] { "string", "another_string" });

            Assert.AreEqual("string another_string ", TestClass.StateField);

            WriteSymbol.Invoke(new object[] { "string", "another_string", "yet_another_string" });

            Assert.AreEqual("string another_string yet_another_string ", TestClass.StateField);*/

        }

        [TestMethod()]
        public void ImportInstanceTest()
        {
            Type t = null; // TODO: Initialize to an appropriate value
            BindingFlags flags = new BindingFlags(); // TODO: Initialize to an appropriate value
            string name_pattern_with_wildcards = string.Empty; // TODO: Initialize to an appropriate value
         //   CLRMethodImporter.ImportAll(t, flags, name_pattern_with_wildcards);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        [TestMethod]
        public void SimpleKeywordsTest()
        {
            TestLambda tl = new TestLambda();
            object expected  = tl;
            Symbol keywordx = KeywordPackage.Instance.Intern("x");
            object actual = tl.Invoke(keywordx, expected);

            Assert.AreEqual(expected, actual);

            actual = tl.Invoke(DefinedSymbols.LambdaAllowOtherKeys, DefinedSymbols.T);
            Assert.AreEqual("default", actual);
        }

        [TestMethod]
        public void MediumKeywordsTest()
        {
            TestLambda tl = new TestLambda();
            object expected = "$1:2$";
            Symbol keywordx = KeywordPackage.Instance.Intern("x");
            Symbol keywordy = KeywordPackage.Instance.Intern("y");
            object actual = tl.Invoke(keywordy, 2, keywordx, 1, DefinedSymbols.LambdaAllowOtherKeys, DefinedSymbols.NIL);

            Assert.AreEqual(expected, actual);

            actual = tl.Invoke(KeywordPackage.Instance.Intern("z"), "l", KeywordPackage.Instance.Intern("o"), "p", DefinedSymbols.LambdaAllowOtherKeys, DefinedSymbols.T);
            Assert.AreEqual("$NIL:NIL$", actual);
        }

        [TestMethod]
        public void HardKeywordsTest()
        {
            //  тестируем следующую лямбду: (x y &optional a s &rest rest &key r t)
            object x = 1;
            object y = 2;
            object a = 3;
            object s = 4;
            object rest = 5;
            object r = 6;
            object t = 7;
            object some_for_rest = 8;
            object expected = "1:2:3:4:(5 :r 6 :t 7):6:7";
            TestLambda tl = new TestLambda();
            Symbol keywordr = KeywordPackage.Instance.Intern("r");
            Symbol keywordt = KeywordPackage.Instance.Intern("t");
            object actual = tl.Invoke(1, 2, 3, 4, 5, keywordr, 6, keywordt, 7);
            Assert.AreEqual(expected, actual);

            expected = "1:2:3:4:(:ALLOW-OTHER-KEYS T :r 5 6):5:NIL";
            actual = tl.Invoke(1, 2, 3, 4, DefinedSymbols.LambdaAllowOtherKeys, DefinedSymbols.T, keywordr, 5, 6);
            Assert.AreEqual(expected, actual);
        }
    }
}


/* В этом тесте используется класс-симулятор System.Console 
 чтобы упростить процесс импортирования вначале теста создаётся нужный пакет
 и все нужные символы. после импортирования проводятся следующие тесты
 1. проверяется что слот функции не пуст у каждого символа
 2. вызываются нужные перегрузки LambdaMethod - эти вызывовы должны в конечном счёте 
 *  изменить значение статического поля тестового класса
 3. операции Assert проводятся над ожидаемым и актульаным значениями тестового поля
 */

public class TestClass
{
    public static string StateField;


    public static void Write(object arg1, [LiveLisp.Core.Compiler.Optional("default",  NextParamIsPresentedIndicator=true)] object arg2, object presented)
    {
        StateField = String.Format("Write(object {0}, [Optional] object {1})", arg1, arg2);
    }

    public static void WriteLine(int[] arg1, [LiveLisp.Core.Compiler.Optional] Symbol  arg2)
    {
        StateField = "Write(object arg1, [Optional] Symbol  arg2)";
    }

    public static void WriteLine(Cons arg1, [DefaultParameterValueAttribute(3)] int arg2)
    {
        StateField = "Write(Cons arg1, [DefaultParameterValueAttribute(\"default\")] int arg2)";
    }

    public static void WriteLine(object arg1, params object[]  args)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(arg1 + " ");

        foreach (var item in args)
        {
            builder.Append(item + " ");
        }

        StateField = builder.ToString();
    }

    public static void Write(object arg1, [LiveLisp.Core.Compiler.Optional("default")] object arg2, [Rest] Cons args)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(arg1 + " ");
        builder.Append(arg2 + " ");

        foreach (var item in args)
        {
            builder.Append(item + " ");
        }

        StateField = builder.ToString();
    }

    static void _Write(string pattern, object arg)
    {
        StateField = string.Format(pattern,arg);
    }
    
    public static void WriteLine()
    {
        StateField = "NewLine was written by WriteLine";
    }

    public static void WriteLine(bool value)
    {
        _Write("bool value {0} was written by WriteLine", value);
    }

    public static void WriteLine(char value)
    {
        _Write("char value {0} was written by WriteLine", value);
    }

    public static void WriteLine(char[] buffer)
    {
        _Write("char[] value {0} was written by WriteLine", buffer);
    }

    public static void WriteLine(decimal value)
    {
        _Write("decimal value {0} was written by WriteLine", value);
    }

    public static void WriteLine(double value)
    {
        _Write("double value {0} was written by WriteLine", value);
    }

    public static void WriteLine(float value)
    {
        _Write("float value {0} was written by WriteLine", value);
    }

    public static void WriteLine(int value)
    {
        _Write("int value {0} was written by WriteLine", value);
    }

    public static void WriteLine(long value)
    {
        _Write("long value {0} was written by WriteLine", value);
    }

    public static void WriteLine(object value)
    {
        _Write("object value {0} was written by WriteLine", value);
    }

    public static void WriteLine(string value)
    {
        _Write("string value {0} was written by WriteLine", value);
    }
    
    [CLSCompliant(false)]
    public static void WriteLine(uint value)
    {
        _Write("uint value {0} was written by WriteLine", value);
    }
    
    [CLSCompliant(false)]
    public static void WriteLine(ulong value)
    {
        _Write("bool value {0} was written by WriteLine", value);
    }
    
    public static void WriteLine(string format, object arg0)
    {
        _Write("object value {0} was written by WriteLine(1) with format", arg0);
    }

    public static void WriteLine(string format, params object[] arg)
    {
        _Write("object value {0} was written by WriteLine(params) with format", arg);
    }

    public static void WriteLine(char[] buffer, int index, int count)
    {
        StateField = String.Format("WriteLine with copying form buffer invoked with index {0} and count {1}", index, count);
    }

    public static void WriteLine(string format, object arg0, object arg1)
    {
        StateField = String.Format("object values {0} {1} was written by WriteLine(2) with format", arg0,arg1);
    }

    public static void WriteLine(string format, object arg0, object arg1, object arg2)
    {
        StateField = String.Format("object value {0} {1} {2} was written by WriteLine(3) with format", arg0, arg1,arg2);
    }
    
    [CLSCompliant(false)]
    public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
    {
        StateField = String.Format("object value {0} {1} {2} {3} was written by WriteLine(4) with format", arg0, arg1, arg2, arg3);
    }

}

/// <summary>
/// все импортированные методы c именем Write из класса TestClass
/// </summary>
/*class ImportWriteTemplate : SingleValueLambdaMethod
{
    #region single return

    public override object Invoke()
    {
        TestClass.WriteLine();
        return DefinedSymbols.NIL;
    }

    /// <summary>
    /// int[] (0)
    /// Cons  (1)
    /// bool  (2)
    /// char  (3)
    /// char[]  (4)
    /// decimal (5)
    /// double   (6)
    /// int     (7)
    /// long    (8)
    /// object  (9)
    /// string  (10)
    /// uint    (11)
    /// ulong (12)
    /// single (13)
    /// 1. if-else test
    /// 2. becouse of chars and ints can use TypeCode
    /// 
    /// Dont forget about ILispNumber
    /// 
    /// для каждого параметра вводим переменную состояние - номер выбора
    /// или иначе - номер ветки(листа) в дереве диспетчера
    /// </summary>
    /// <param name="arg1"></param>
    /// <returns></returns>
    public override object Invoke(object arg1)
    {
        int level_0_case = -1;

        // load to stack and fill level_0_case

        if (arg1 is ILispNumber) // we MUST handle this in any way
        {
            ILispNumber lisp_num = arg1 as ILispNumber;

            // for now because of we have such leaves as int, long and etc dispatch it
            switch (lisp_num.NumberType)
            {
                case NumberType.Byte:
                    //load to stack as int
                    level_0_case = 7;
                    goto level1;
                case NumberType.Int:
                    //load to stack as int
                    level_0_case = 7;
                    goto level1;
                case NumberType.BigInt:
                    //load to stack as int
                    level_0_case = 7;
                    goto level1;
                case NumberType.Long:
                    //load to stack as Int64
                    level_0_case = 8;
                    goto level1;
                case NumberType.Ratio:
                    //load to stack as double
                    level_0_case = 6;
                    goto level1;
                case NumberType.Single:
                    //load to stack as single
                    level_0_case = 13;
                    goto level1;
                case NumberType.Float:
                    //load to stack as double
                    level_0_case = 6;
                    goto level1;
                case NumberType.Complex:
                    //load to stack as int32
                    level_0_case = 7;
                    goto level1;
                case NumberType.Decimal:
                    //load to stack as decimal
                    level_0_case = 5;
                    goto level1;
                case NumberType.BigDecimal:
                    //try to load as decimal else load as object
                    level_0_case = 5;
                    goto level1; 
                default:
                    break;
            }
        }
        else if (arg1 is string)
        {
            // load as string 
            level_0_case = 10;
            goto level1; 
        }

        switch (Type.GetTypeCode(arg1.GetType()))
        {
            case TypeCode.Boolean:
                goto level1;
            case TypeCode.Byte:
                goto level1;
            case TypeCode.Char:
                goto level1;
            case TypeCode.DBNull:
                goto level1;
            case TypeCode.DateTime:
                goto level1;
            case TypeCode.Decimal:
                goto level1;
            case TypeCode.Double:
                goto level1;
            case TypeCode.Empty:
                goto level1;
            case TypeCode.Int16:
                goto level1;
            case TypeCode.Int32:
                goto level1;
            case TypeCode.Int64:
                goto level1;
            case TypeCode.Object:
                goto level1;
            case TypeCode.SByte:
                goto level1;
            case TypeCode.Single:
                goto level1;
            case TypeCode.String:
                goto level1;
            case TypeCode.UInt16:
                goto level1;
            case TypeCode.UInt32:
                goto level1;
            case TypeCode.UInt64:
                goto level1;
            default:
                break;
        }
        level1:
        switch (level_0_case) // process next levels in respective cases 
        {
            default:
                break;
        }
    }

    public override object Invoke(object arg1, object arg2)
    {
        CheckArgsCount(2);

        throw new InvalidOperationException("if this exception was thrown then lambda(2) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3)
    {
        CheckArgsCount(3);

        throw new InvalidOperationException("if this exception was thrown then lambda(3) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4)
    {
        CheckArgsCount(4);

        throw new InvalidOperationException("if this exception was thrown then lambda(4) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5)
    {
        CheckArgsCount(5);

        throw new InvalidOperationException("if this exception was thrown then lambda(5) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
    {
        CheckArgsCount(6);

        throw new InvalidOperationException("if this exception was thrown then lambda(6) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
    {
        CheckArgsCount(7);

        throw new InvalidOperationException("if this exception was thrown then lambda(7) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
    {
        CheckArgsCount(8);

        throw new InvalidOperationException("if this exception was thrown then lambda(8) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)
    {
        CheckArgsCount(9);

        throw new InvalidOperationException("if this exception was thrown then lambda(9) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)
    {
        CheckArgsCount(10);

        throw new InvalidOperationException("if this exception was thrown then lambda(10) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)
    {
        CheckArgsCount(11);

        throw new InvalidOperationException("if this exception was thrown then lambda(11) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)
    {
        CheckArgsCount(12);

        throw new InvalidOperationException("if this exception was thrown then lambda(12) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)
    {
        CheckArgsCount(13);

        throw new InvalidOperationException("if this exception was thrown then lambda(13) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, params object[] args)
    {
        CheckArgsCount(13 + args.Length);

        throw new InvalidOperationException("if this exception was thrown then lambda(13 + args.Length) generator totally fail when it generated Invokes");
    }

    public override object Invoke(object[] args)
    {
        CheckArgsCount(args.Length);

        throw new InvalidOperationException("if this exception was thrown then lambda(args.Length) generator totally fail when it generated Invokes");
    }
    #endregion 
}*/

class TestLambda : FullLispFunction
{
    bool allowotherkeys = false;

    public TestLambda()
        : base("qwe")
    {

    }
    public override object Invoke(object arg1, object arg2)
    {
        bool backup = allowotherkeys;
        // first of all search for :allow-other-keys
        if (arg1 == DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (arg2 == DefinedSymbols.NIL)
            {
                allowotherkeys = false;
            }
            else
            {
                allowotherkeys = true;
            }
        }

        object x;
        if (!KeywordPackage.CheckKeyword(arg1, "x"))
        {
            if (allowotherkeys)
            {
                x = "default";
            }
            else
            {
                throw new SimpleErrorException("unknown keyword " + arg1);
            }

        }
        else
        {
            x = arg2;
        }

        allowotherkeys = backup;
        return x;
    }

    public override object Invoke(object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
    {
        bool backup = allowotherkeys;
        bool allowotherkeyspresented = false;
        // first of all search for :allow-other-keys
        if (arg1 == DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (!allowotherkeyspresented)
            {
                if (arg2 == DefinedSymbols.NIL)
                {
                    allowotherkeys = false;
                }
                else
                {
                    allowotherkeys = true;
                }
                allowotherkeyspresented = true;
            }
        }
        else if (arg3 == DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (!allowotherkeyspresented)
            {
                if (arg4 == DefinedSymbols.NIL)
                {
                    allowotherkeys = false;
                }
                else
                {
                    allowotherkeys = true;
                }
                allowotherkeyspresented = true;
            }
        }
        else if (arg5 == DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (!allowotherkeyspresented)
            {
                if (arg6 == DefinedSymbols.NIL)
                {
                    allowotherkeys = false;
                }
                else
                {
                    allowotherkeys = true;
                }
                allowotherkeyspresented = true;
            }
        }

        object x = DefinedSymbols.NIL;
        object y = DefinedSymbols.NIL;
        bool xpresented = false;
        bool ypresented = false;
        object currentkey = arg1;
        object currentvalue = arg2;
        if (KeywordPackage.CheckKeyword(currentkey, "x"))
        {
            if (!xpresented)
            {
                x = currentvalue;
                xpresented = true;
            }
        }
        else if (KeywordPackage.CheckKeyword(currentkey, "y"))
        {
            if (!ypresented)
            {
                y = currentvalue;
                ypresented = true;
            }
        }
        else if (currentkey != DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (!allowotherkeys)
            {
                throw new SimpleErrorException("unknown keyword " + currentkey);
            }
        }
        currentkey = arg3;
        currentvalue = arg4;
        if (KeywordPackage.CheckKeyword(currentkey, "x"))
        {
            if (!xpresented)
            {
                x = currentvalue;
                xpresented = true;
            }

        }
        else if (KeywordPackage.CheckKeyword(currentkey, "y"))
        {
            if (!ypresented)
            {
                y = currentvalue;
                ypresented = true;
            }
        }
        else if (currentkey != DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (!allowotherkeys)
            {
                throw new SimpleErrorException("unknown keyword " + currentkey);
            }
        }
        currentkey = arg5;
        currentvalue = arg6;
        if (KeywordPackage.CheckKeyword(currentkey, "x"))
        {
            if (!xpresented)
            {
                x = currentvalue;
                xpresented = true;
            }

        }
        else if (KeywordPackage.CheckKeyword(currentkey, "y"))
        {
            if (!ypresented)
            {
                y = currentvalue;
                ypresented = true;
            }
        }
        else if (currentkey != DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (!allowotherkeys)
            {
                throw new SimpleErrorException("unknown keyword " + currentkey);
            }
        }
        allowotherkeys = backup;
        return "$" + x + ":" + y + "$";
    }
    //(x y &optional a s &rest rest &key r t)
    public override object Invoke(object x, object y, object a, object s, object rest, object arg6, object arg7, object arg8, object arg9)
    {
        // load x
        // load y
        // load a
        // load s
        Cons rest_slot = new Cons(rest);

        bool backup = allowotherkeys;
        #region look for allow-other-key
        bool allowotherkeyspresented = false;
        // first of all search for :allow-other-keys
        if (arg6 == DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (!allowotherkeyspresented)
            {
                if (arg7 == DefinedSymbols.NIL)
                {
                    allowotherkeys = false;
                }
                else
                {
                    allowotherkeys = true;
                }
                allowotherkeyspresented = true;
            }
        }
        else if (arg7 == DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (!allowotherkeyspresented)
            {
                if (arg8 == DefinedSymbols.NIL)
                {
                    allowotherkeys = false;
                }
                else
                {
                    allowotherkeys = true;
                }
                allowotherkeyspresented = true;
            }
        }
        else if (arg8 == DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (!allowotherkeyspresented)
            {
                if (arg9 == DefinedSymbols.NIL)
                {
                    allowotherkeys = false;
                }
                else
                {
                    allowotherkeys = true;
                }
                allowotherkeyspresented = true;
            }
        }
        #endregion

        object r = DefinedSymbols.NIL;
        object t = DefinedSymbols.NIL;
        bool rpresented = false;
        bool tpresented = false;
        object currentkey = arg6;
        object currentvalue = arg7;
        bool skip = false;
        if (KeywordPackage.CheckKeyword(currentkey, "r"))
        {
            if (!rpresented)
            {
                r = currentvalue;
                rpresented = true;
                skip = true;
            }
        }
        else if (KeywordPackage.CheckKeyword(currentkey, "t"))
        {
            if (!tpresented)
            {
                t = currentvalue;
                tpresented = true;
                skip = true;
            }
        }
        else if (currentkey != DefinedSymbols.LambdaAllowOtherKeys)
        {
            if (!allowotherkeys)
            {
                throw new SimpleErrorException("unknown keyword " + currentkey);
            }
        }
        rest_slot.Add(arg6);
        if (!skip)
        {
        currentkey = arg7;
        currentvalue = arg8;
        
            if (KeywordPackage.CheckKeyword(currentkey, "x"))
            {
                if (!rpresented)
                {
                    r = currentvalue;
                    rpresented = true;
                    skip = true;
                }

            }
            else if (KeywordPackage.CheckKeyword(currentkey, "y"))
            {
                if (!tpresented)
                {
                    t = currentvalue;
                    tpresented = true;
                    skip = true;
                }
            }
            else if (currentkey != DefinedSymbols.LambdaAllowOtherKeys)
            {
                if (!allowotherkeys)
                {
                    throw new SimpleErrorException("unknown keyword " + currentkey);
                }
            }
        }
        else
        {
            skip = false;
        }
        rest_slot.Add(arg7);
        if (!skip)
        {
            currentkey = arg8;
            currentvalue = arg9;
            if (KeywordPackage.CheckKeyword(currentkey, "r"))
            {
                if (!rpresented)
                {
                    r = currentvalue;
                    rpresented = true;
                    skip = true;
                }

            }
            else if (KeywordPackage.CheckKeyword(currentkey, "t"))
            {
                if (!tpresented)
                {
                    t = currentvalue;
                    tpresented = true;
                    skip = true;
                }
            }
            else if (currentkey != DefinedSymbols.LambdaAllowOtherKeys)
            {
                if (!allowotherkeys)
                {
                    throw new SimpleErrorException("unknown keyword " + currentkey);
                }
            }
        }
        else
        {
            skip = false;
        }
        rest_slot.Append(arg8);
        if (!skip && !allowotherkeys)
        {
            throw new SimpleErrorException("non pair-keyword " + arg9);
        }
        rest_slot.Append(arg9);
        allowotherkeys = backup;
        return x + ":" + y + ":" + a + ":" + s + ":" + rest_slot + ":" + r + ":" + t;
    }
}
