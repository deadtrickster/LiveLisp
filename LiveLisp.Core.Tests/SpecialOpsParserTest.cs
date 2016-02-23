using LiveLisp.Core.Eval;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiveLisp.Core.Types;
using LiveLisp.Core.AST;
using System.Collections.Generic;
using LiveLisp.Core.AST.Expressions;

namespace LiveLisp.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for SpecialOpsParserTest and is intended
    ///to contain all SpecialOpsParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpecialOpsParserTest
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
        /*

        /// <summary>
        ///A test for TryParseDeclaration
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void TryParseDeclarationTest()
        {
            object decl = null; // TODO: Initialize to an appropriate value
            List<Declaration> declarations = null; // TODO: Initialize to an appropriate value
            List<Declaration> declarationsExpected = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = SpecialOpsParser_Accessor.TryParseDeclaration(decl, out declarations);
            Assert.AreEqual(declarationsExpected, declarations);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseUnwindProtect
        ///</summary>
        [TestMethod()]
        public void ParseUnwindProtectTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            UnwindProtectExpression expected = null; // TODO: Initialize to an appropriate value
            UnwindProtectExpression actual;
            actual = SpecialOpsParser.ParseUnwindProtect(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseThrow
        ///</summary>
        [TestMethod()]
        public void ParseThrowTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            ThrowExpression expected = null; // TODO: Initialize to an appropriate value
            ThrowExpression actual;
            actual = SpecialOpsParser.ParseThrow(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseThe
        ///</summary>
        [TestMethod()]
        public void ParseTheTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            TheExpression expected = null; // TODO: Initialize to an appropriate value
            TheExpression actual;
            actual = SpecialOpsParser.ParseThe(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseTagbody
        ///</summary>
        [TestMethod()]
        public void ParseTagbodyTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            TagBodyExpression expected = null; // TODO: Initialize to an appropriate value
            TagBodyExpression actual;
            actual = SpecialOpsParser.ParseTagbody(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseSymbolMacrolet
        ///</summary>
        [TestMethod()]
        public void ParseSymbolMacroletTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            SymbolMacroletExpression expected = null; // TODO: Initialize to an appropriate value
            SymbolMacroletExpression actual;
            actual = SpecialOpsParser.ParseSymbolMacrolet(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseSpecialDeclarations
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseSpecialDeclarationsTest()
        {
            Cons cons = null; // TODO: Initialize to an appropriate value
            Declaration expected = null; // TODO: Initialize to an appropriate value
            Declaration actual;
            actual = SpecialOpsParser_Accessor.ParseSpecialDeclarations(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseSetq
        ///</summary>
        [TestMethod()]
        public void ParseSetqTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            SetqExpression expected = null; // TODO: Initialize to an appropriate value
            SetqExpression actual;
            actual = SpecialOpsParser.ParseSetq(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseReturnFrom
        ///</summary>
        [TestMethod()]
        public void ParseReturnFromTest()
        {
            Cons progv = null; // TODO: Initialize to an appropriate value
            ReturnFromExpression expected = null; // TODO: Initialize to an appropriate value
            ReturnFromExpression actual;
            actual = SpecialOpsParser.ParseReturnFrom(progv);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseQuote
        ///</summary>
        [TestMethod()]
        public void ParseQuoteTest()
        {
            Cons progv = null; // TODO: Initialize to an appropriate value
            ConstantExpression expected = null; // TODO: Initialize to an appropriate value
            ConstantExpression actual;
            actual = SpecialOpsParser.ParseQuote(progv);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseProgv
        ///</summary>
        [TestMethod()]
        public void ParseProgvTest()
        {
            Cons progv = null; // TODO: Initialize to an appropriate value
            ProgvExpression expected = null; // TODO: Initialize to an appropriate value
            ProgvExpression actual;
            actual = SpecialOpsParser.ParseProgv(progv);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseProgn
        ///</summary>
        [TestMethod()]
        public void ParsePrognTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            PrognExpression expected = null; // TODO: Initialize to an appropriate value
            PrognExpression actual;
            actual = SpecialOpsParser.ParseProgn(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseMultipleVAlueProg1
        ///</summary>
        [TestMethod()]
        public void ParseMultipleVAlueProg1Test()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            MultipleValueProg1Expression expected = null; // TODO: Initialize to an appropriate value
            MultipleValueProg1Expression actual;
            actual = SpecialOpsParser.ParseMultipleVAlueProg1(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseMultipleValueCall
        ///</summary>
        [TestMethod()]
        public void ParseMultipleValueCallTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            MultipleValueCallExpression expected = null; // TODO: Initialize to an appropriate value
            MultipleValueCallExpression actual;
            actual = SpecialOpsParser.ParseMultipleValueCall(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseMacrolet
        ///</summary>
        [TestMethod()]
        public void ParseMacroletTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            MacroletExpression expected = null; // TODO: Initialize to an appropriate value
            MacroletExpression actual;
            actual = SpecialOpsParser.ParseMacrolet(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseLocally
        ///</summary>
        [TestMethod()]
        public void ParseLocallyTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            LocallyExpression expected = null; // TODO: Initialize to an appropriate value
            LocallyExpression actual;
            actual = SpecialOpsParser.ParseLocally(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseLoadTimeValue
        ///</summary>
        [TestMethod()]
        public void ParseLoadTimeValueTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            LoadTimeValue expected = null; // TODO: Initialize to an appropriate value
            LoadTimeValue actual;
            actual = SpecialOpsParser.ParseLoadTimeValue(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseLet
        ///</summary>
        [TestMethod()]
        public void ParseLetTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            LetLetStartBase expected = null; // TODO: Initialize to an appropriate value
            LetLetStartBase actual;
            actual = SpecialOpsParser.ParseLet(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseLabels
        ///</summary>
        [TestMethod()]
        public void ParseLabelsTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            LabelsExpression expected = null; // TODO: Initialize to an appropriate value
            LabelsExpression actual;
            actual = SpecialOpsParser.ParseLabels(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseIgnoreDeclarations
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseIgnoreDeclarationsTest()
        {
            Cons cons = null; // TODO: Initialize to an appropriate value
            Declaration expected = null; // TODO: Initialize to an appropriate value
            Declaration actual;
            actual = SpecialOpsParser_Accessor.ParseIgnoreDeclarations(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseIf
        ///</summary>
        [TestMethod()]
        public void ParseIfTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            IfExpression expected = null; // TODO: Initialize to an appropriate value
            IfExpression actual;
            actual = SpecialOpsParser.ParseIf(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseGo
        ///</summary>
        [TestMethod()]
        public void ParseGoTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            GoExpression expected = null; // TODO: Initialize to an appropriate value
            GoExpression actual;
            actual = SpecialOpsParser.ParseGo(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseFunction
        ///</summary>
        [TestMethod()]
        public void ParseFunctionTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            FunctionExpression expected = null; // TODO: Initialize to an appropriate value
            FunctionExpression actual;
            actual = SpecialOpsParser.ParseFunction(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseForms
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseFormsTest()
        {
            Cons _forms = null; // TODO: Initialize to an appropriate value
            List<Expression> expected = null; // TODO: Initialize to an appropriate value
            List<Expression> actual;
            actual = SpecialOpsParser_Accessor.ParseForms(_forms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseFlet
        ///</summary>
        [TestMethod()]
        public void ParseFletTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            FletExpression expected = null; // TODO: Initialize to an appropriate value
            FletExpression actual;
            actual = SpecialOpsParser.ParseFlet(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseEvalWhen
        ///</summary>
        [TestMethod()]
        public void ParseEvalWhenTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            EvalWhenExpression expected = null; // TODO: Initialize to an appropriate value
            EvalWhenExpression actual;
            actual = SpecialOpsParser.ParseEvalWhen(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseClrTypeDeclarations
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseClrTypeDeclarationsTest()
        {
            Cons cons = null; // TODO: Initialize to an appropriate value
            Declaration expected = null; // TODO: Initialize to an appropriate value
            Declaration actual;
            actual = SpecialOpsParser_Accessor.ParseClrTypeDeclarations(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseClrProperty
        ///</summary>
        [TestMethod()]
        public void ParseClrPropertyTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            ClrPropertyExpression expected = null; // TODO: Initialize to an appropriate value
            ClrPropertyExpression actual;
            actual = SpecialOpsParser.ParseClrProperty(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseClrMethod
        ///</summary>
        [TestMethod()]
        public void ParseClrMethodTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            ClrMethodExpression expected = null; // TODO: Initialize to an appropriate value
            ClrMethodExpression actual;
            actual = SpecialOpsParser.ParseClrMethod(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseClrField
        ///</summary>
        [TestMethod()]
        public void ParseClrFieldTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            ClrFieldExpression expected = null; // TODO: Initialize to an appropriate value
            ClrFieldExpression actual;
            actual = SpecialOpsParser.ParseClrField(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseClrEnum
        ///</summary>
        [TestMethod()]
        public void ParseClrEnumTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            ClrEnumExpression expected = null; // TODO: Initialize to an appropriate value
            ClrEnumExpression actual;
            actual = SpecialOpsParser.ParseClrEnum(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseClrDelegate
        ///</summary>
        [TestMethod()]
        public void ParseClrDelegateTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            ClrDelegateExpression expected = null; // TODO: Initialize to an appropriate value
            ClrDelegateExpression actual;
            actual = SpecialOpsParser.ParseClrDelegate(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
        
        /// <summary>
        ///A test for ParseClrClass
        ///</summary>
        [TestMethod()]
        public void ParseClrClassTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            ClrClassExpression expected = null; // TODO: Initialize to an appropriate value
            ClrClassExpression actual;
            actual = SpecialOpsParser.ParseClrClass(parms);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
        
        /// <summary>
        ///A test for ParseCatch
        ///</summary>
        [TestMethod()]
        public void ParseCatchTest()
        {
            CatchExpression actual;
            bool _throwed = false;
            try
            {
                actual = SpecialOpsParser.ParseCatch(new Cons(DefinedSymbols.NIL));
            }
            catch (SyntaxException)
            {
                _throwed = true;
            }

            Assert.AreEqual(true, _throwed);

            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseBlock
        ///</summary>
        [TestMethod()]
        public void ParseBlockTest()
        {
            Symbol blockName = new Symbol("%tmp%");

            Cons parms = new Cons(new object[] { blockName });
            var actual = SpecialOpsParser.ParseBlock(parms);
            Assert.AreEqual(blockName, actual.BlockName);

            bool _throwed = false;
            try
            {
                actual = SpecialOpsParser.ParseBlock(new object[] { "invlaid" });
            }
            catch(SyntaxException)
            {
                _throwed = true;
            }

            Assert.AreEqual(true, _throwed);

            Assert.Inconclusive("Verify the correctness of this test method.");
        }
/*
        /// <summary>
        ///A test for ParseAttributeDeclarations
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseAttributeDeclarationsTest()
        {
            Cons cons = null; // TODO: Initialize to an appropriate value
            Declaration expected = null; // TODO: Initialize to an appropriate value
            Declaration actual;
            actual = SpecialOpsParser_Accessor.ParseAttributeDeclarations(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DispatchDeclarations
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void DispatchDeclarationsTest()
        {
            Cons _declars = null; // TODO: Initialize to an appropriate value
            List<Declaration> declarations = null; // TODO: Initialize to an appropriate value
            List<Declaration> declarationsExpected = null; // TODO: Initialize to an appropriate value
            SpecialOpsParser_Accessor.DispatchDeclarations(_declars, out declarations);
            Assert.AreEqual(declarationsExpected, declarations);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for BindingFromCons
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void BindingFromConsTest()
        {
            Cons bindpair = null; // TODO: Initialize to an appropriate value
            string op_name = string.Empty; // TODO: Initialize to an appropriate value
            Binding expected = null; // TODO: Initialize to an appropriate value
            Binding actual;
            actual = SpecialOpsParser_Accessor.BindingFromCons(bindpair, op_name);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AssertMaxArgs
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void AssertMaxArgsTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            int maxcount = 0; // TODO: Initialize to an appropriate value
            string opname = string.Empty; // TODO: Initialize to an appropriate value
            SpecialOpsParser_Accessor.AssertMaxArgs(parms, maxcount, opname);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AssertArgsExistence
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void AssertArgsExistenceTest1()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            string opname = string.Empty; // TODO: Initialize to an appropriate value
            SpecialOpsParser_Accessor.AssertArgsExistence(parms, opname);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AssertArgsExistence
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void AssertArgsExistenceTest()
        {
            Cons parms = null; // TODO: Initialize to an appropriate value
            int count = 0; // TODO: Initialize to an appropriate value
            string opname = string.Empty; // TODO: Initialize to an appropriate value
            SpecialOpsParser_Accessor.AssertArgsExistence(parms, count, opname);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SpecialOpsParser Constructor
        ///</summary>
        [TestMethod()]
        public void SpecialOpsParserConstructorTest()
        {
            SpecialOpsParser target = new SpecialOpsParser();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }*/
    }
}
