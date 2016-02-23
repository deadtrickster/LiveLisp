using LiveLisp.Core.Reader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiveLisp.Core.Types;
using LiveLisp.Core.Types.Streams;
using LiveLisp.Core.Compiler;
namespace LiveLisp.Core.Tests.Reader
{


    /// <summary>
    ///This is a test class for ReaderDictionaryTest and is intended
    ///to contain all ReaderDictionaryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReaderDictionaryTest
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

        [ClassInitialize()]
        public static void ReaderDictionaryTestInitialize(TestContext testContext)
        {
            Initialization.Initialize();
        }

        /// <summary>
        ///A test for Read
        ///</summary>
        [TestMethod()]
        public void ReadTest()
        {
            CompilerImpl.SaveAssembly();
            object expected = Cons.FromCollection(new int[] { 1, 2, 3 });
            object actual;
            actual = DefinedSymbols.Read.Invoke(new CharacterInputStream("(1 2 3)"));
            Assert.IsInstanceOfType(actual, typeof(Cons));
            Assert.AreEqual(expected.ToString(), actual.ToString());
            //   Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod]
        public void SingleQuoteTest()
        {
           // CLRMethodImporter.SaveLambdas();
            Cons expected = new Cons(DefinedSymbols.Quote);
            expected.Cdr = new Cons(new Cons(DefinedSymbols.Quote, new Cons(1)));
            object actual;
            actual = DefinedSymbols.Read.Invoke(new CharacterInputStream("''1"));
            Assert.IsInstanceOfType(actual, typeof(Cons));
            Assert.AreEqual(expected.ToString(), actual.ToString());
            //   Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod]
        public void DblQuoteTest()
        {
            string expected = "\"APL\\360?\" he cried.";
            object actual = DefinedSymbols.Read.Invoke(new CharacterInputStream("\"\\\"APL\\\\360?\\\" he cried.\""));
            Assert.IsInstanceOfType(actual, typeof(string));
            Assert.AreEqual(expected, actual as string);
        }

        [TestMethod]
        public void SemicolonTest()
        {
            object expected = DefinedSymbols.NIL;
            object actual = DefinedSymbols.Read.Invoke(new CharacterInputStream(";comment"));
            
            Assert.AreEqual(expected, actual);
        }

        #region errors driven 
        [TestMethod]
        public void TestNilAsRegularMember()
        {
            object expectesraw = DefinedSymbols.Read.Invoke(new CharacterInputStream("(cons nil nil)"));
            
            Cons cons = expectesraw as Cons;

            Assert.AreEqual(3, cons.Count);
        }
        #endregion
    }
}
