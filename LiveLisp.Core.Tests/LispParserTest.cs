using LiveLisp.Core.Eval;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiveLisp.Core.Types;
using LiveLisp.Core.AST;

namespace LiveLisp.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for LispParserTest and is intended
    ///to contain all LispParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LispParserTest
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
        ///A test for ParseSymbol
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseSymbolTest()
        {
            Symbol symbol = null; // TODO: Initialize to an appropriate value
            Expression expected = null; // TODO: Initialize to an appropriate value
            Expression actual;
            actual = LispParser_Accessor.ParseSymbol(symbol);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseSpecialOp
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseSpecialOpTest()
        {
            Symbol _operator = null; // TODO: Initialize to an appropriate value
            object p = null; // TODO: Initialize to an appropriate value
            Expression ret = null; // TODO: Initialize to an appropriate value
            Expression retExpected = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = LispParser_Accessor.ParseSpecialOp(_operator, p, out ret);
            Assert.AreEqual(retExpected, ret);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseFunctionalForm
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseFunctionalFormTest()
        {
            Cons _functionalForm = null; // TODO: Initialize to an appropriate value
            FunctionalForm expected = null; // TODO: Initialize to an appropriate value
            FunctionalForm actual;
            actual = LispParser_Accessor.ParseFunctionalForm(_functionalForm);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseExpression
        ///</summary>
        [TestMethod()]
        public void ParseExpressionTest()
        {
            object form = null; // TODO: Initialize to an appropriate value
            Expression expected = null; // TODO: Initialize to an appropriate value
            Expression actual;
            actual = LispParser.ParseExpression(form);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseConst
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseConstTest()
        {
            object value = null; // TODO: Initialize to an appropriate value
            Expression expected = null; // TODO: Initialize to an appropriate value
            Expression actual;
            actual = LispParser_Accessor.ParseConst(value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseCons
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseConsTest()
        {
            Cons cons = null; // TODO: Initialize to an appropriate value
            Expression expected = null; // TODO: Initialize to an appropriate value
            Expression actual;
            actual = LispParser_Accessor.ParseCons(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseCompoundCall
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void ParseCompoundCallTest()
        {
            Cons cons = null; // TODO: Initialize to an appropriate value
            Cons _functionalForm = null; // TODO: Initialize to an appropriate value
            Expression expected = null; // TODO: Initialize to an appropriate value
            Expression actual;
            actual = LispParser_Accessor.ParseCompoundCall(cons, _functionalForm);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LispParser Constructor
        ///</summary>
        [TestMethod()]
        public void LispParserConstructorTest()
        {
            LispParser target = new LispParser();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }*/
    }
}
