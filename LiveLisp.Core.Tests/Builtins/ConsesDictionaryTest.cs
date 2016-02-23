using LiveLisp.Core.BuiltIns.Conses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiveLisp.Core.Types;
using LiveLisp.Core.Reader;

namespace LiveLisp.Core.Tests.Builtins
{
    
    
    /// <summary>
    ///This is a test class for ConsesDictionaryTest and is intended
    ///to contain all ConsesDictionaryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConsesDictionaryTest
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
        public static void MyClassInitialize(TestContext testContext)
        {
            cons = Cons.FromCollection(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });
            Initialization.Initialize();
        }
        static Cons cons;

        /// <summary>
        ///A test for UNION
        ///</summary>
        [TestMethod()]
        public void UNIONTest()
        {
            object list1 = null; // TODO: Initialize to an appropriate value
            object list2 = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.UNION(list1, list2, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Tree_EQUAL
        ///</summary>
        [TestMethod()]
        public void Tree_EQUALTest()
        {
            object tree1 = null; // TODO: Initialize to an appropriate value
            object tree2 = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Tree_EQUAL(tree1, tree2, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for THIRD
        ///</summary>
        [TestMethod()]
        public void THIRDTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.THIRD(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TENTH
        ///</summary>
        [TestMethod()]
        public void TENTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.TENTH(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TAILP
        ///</summary>
        [TestMethod()]
        public void TAILPTest()
        {
            object obj = null; // TODO: Initialize to an appropriate value
            object list = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.TAILP(obj, list);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SUBST_IF_NOT
        ///</summary>
        [TestMethod()]
        public void SUBST_IF_NOTTest()
        {
            object _new = null; // TODO: Initialize to an appropriate value
            object predicate = null; // TODO: Initialize to an appropriate value
            object tree = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.SUBST_IF_NOT(_new, predicate, tree, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SUBST_IF
        ///</summary>
        [TestMethod()]
        public void SUBST_IFTest()
        {
            object _new = null; // TODO: Initialize to an appropriate value
            object predicate = null; // TODO: Initialize to an appropriate value
            object tree = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.SUBST_IF(_new, predicate, tree, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SUBST
        ///</summary>
        [TestMethod()]
        public void SUBSTTest()
        {
            object _new = null; // TODO: Initialize to an appropriate value
            object old = null; // TODO: Initialize to an appropriate value
            object tree = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.SUBST(_new, old, tree, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SUBSETP
        ///</summary>
        [TestMethod()]
        public void SUBSETPTest()
        {
            object list1 = null; // TODO: Initialize to an appropriate value
            object list2 = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.SUBSETP(list1, list2, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SUBLIS
        ///</summary>
        [TestMethod()]
        public void SUBLISTest()
        {
            object alist = null; // TODO: Initialize to an appropriate value
            object tree = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.SUBLIS(alist, tree, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SIXTH
        ///</summary>
        [TestMethod()]
        public void SIXTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.SIXTH(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SEVENTH
        ///</summary>
        [TestMethod()]
        public void SEVENTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.SEVENTH(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_THIRD
        ///</summary>
        [TestMethod()]
        public void set_THIRDTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_THIRD(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_TENTH
        ///</summary>
        [TestMethod()]
        public void set_TENTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_TENTH(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_SIXTH
        ///</summary>
        [TestMethod()]
        public void set_SIXTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_SIXTH(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_SEVENTH
        ///</summary>
        [TestMethod()]
        public void set_SEVENTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_SEVENTH(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_SECOND
        ///</summary>
        [TestMethod()]
        public void set_SECONDTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_SECOND(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_REST
        ///</summary>
        [TestMethod()]
        public void set_RESTTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_REST(list);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_NTH
        ///</summary>
        [TestMethod()]
        public void set_NTHTest()
        {
            object n = null; // TODO: Initialize to an appropriate value
            object list = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_NTH(n, list, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_NINTH
        ///</summary>
        [TestMethod()]
        public void set_NINTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_NINTH(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_GETF
        ///</summary>
        [TestMethod()]
        public void set_GETFTest()
        {
            object plist = null; // TODO: Initialize to an appropriate value
            object indicator = null; // TODO: Initialize to an appropriate value
            object store = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_GETF(plist, indicator, store);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_FOURTH
        ///</summary>
        [TestMethod()]
        public void set_FOURTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_FOURTH(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_FIRST
        ///</summary>
        [TestMethod()]
        public void set_FIRSTTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_FIRST(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_FIFTH
        ///</summary>
        [TestMethod()]
        public void set_FIFTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_FIFTH(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SET_EXCLUSIVE_OR
        ///</summary>
        [TestMethod()]
        public void SET_EXCLUSIVE_ORTest()
        {
            object list1 = null; // TODO: Initialize to an appropriate value
            object list2 = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.SET_EXCLUSIVE_OR(list1, list2, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_EIGHTH
        ///</summary>
        [TestMethod()]
        public void set_EIGHTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_EIGHTH(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SET_DIFFERENCE
        ///</summary>
        [TestMethod()]
        public void SET_DIFFERENCETest()
        {
            object list1 = null; // TODO: Initialize to an appropriate value
            object list2 = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.SET_DIFFERENCE(list1, list2, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cdr
        ///</summary>
        [TestMethod()]
        public void set_CdrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cdr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cddr
        ///</summary>
        [TestMethod()]
        public void set_CddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cddr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cdddr
        ///</summary>
        [TestMethod()]
        public void set_CdddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cdddr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cddddr
        ///</summary>
        [TestMethod()]
        public void set_CddddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cddddr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cdddar
        ///</summary>
        [TestMethod()]
        public void set_CdddarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cdddar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cddar
        ///</summary>
        [TestMethod()]
        public void set_CddarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cddar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cddadr
        ///</summary>
        [TestMethod()]
        public void set_CddadrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cddadr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cddaar
        ///</summary>
        [TestMethod()]
        public void set_CddaarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cddaar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cdar
        ///</summary>
        [TestMethod()]
        public void set_CdarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cdar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cdadr
        ///</summary>
        [TestMethod()]
        public void set_CdadrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cdadr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cdaddr
        ///</summary>
        [TestMethod()]
        public void set_CdaddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cdaddr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cdadar
        ///</summary>
        [TestMethod()]
        public void set_CdadarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cdadar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cdaar
        ///</summary>
        [TestMethod()]
        public void set_CdaarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cdaar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cdaadr
        ///</summary>
        [TestMethod()]
        public void set_CdaadrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cdaadr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cdaaar
        ///</summary>
        [TestMethod()]
        public void set_CdaaarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cdaaar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Car
        ///</summary>
        [TestMethod()]
        public void set_CarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Car(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cadr
        ///</summary>
        [TestMethod()]
        public void set_CadrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cadr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Caddr
        ///</summary>
        [TestMethod()]
        public void set_CaddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Caddr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cadddr
        ///</summary>
        [TestMethod()]
        public void set_CadddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cadddr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Caddar
        ///</summary>
        [TestMethod()]
        public void set_CaddarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Caddar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cadar
        ///</summary>
        [TestMethod()]
        public void set_CadarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cadar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cadadr
        ///</summary>
        [TestMethod()]
        public void set_CadadrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cadadr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Cadaar
        ///</summary>
        [TestMethod()]
        public void set_CadaarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Cadaar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Caar
        ///</summary>
        [TestMethod()]
        public void set_CaarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Caar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Caadr
        ///</summary>
        [TestMethod()]
        public void set_CaadrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Caadr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Caaddr
        ///</summary>
        [TestMethod()]
        public void set_CaaddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Caaddr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Caadar
        ///</summary>
        [TestMethod()]
        public void set_CaadarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Caadar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Caaar
        ///</summary>
        [TestMethod()]
        public void set_CaaarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Caaar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Caaadr
        ///</summary>
        [TestMethod()]
        public void set_CaaadrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Caaadr(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for set_Caaaar
        ///</summary>
        [TestMethod()]
        public void set_CaaaarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.set_Caaaar(cons, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SECOND
        ///</summary>
        [TestMethod()]
        public void SECONDTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.SECOND(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Rplacd
        ///</summary>
        [TestMethod()]
        public void RplacdTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object new_cdr = null; // TODO: Initialize to an appropriate value
            Cons expected = null; // TODO: Initialize to an appropriate value
            Cons actual;
            actual = ConsesDictionary.Rplacd(cons, new_cdr);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Rplaca
        ///</summary>
        [TestMethod()]
        public void RplacaTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object new_car = null; // TODO: Initialize to an appropriate value
            Cons expected = null; // TODO: Initialize to an appropriate value
            Cons actual;
            actual = ConsesDictionary.Rplaca(cons, new_car);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for REVAPPEND
        ///</summary>
        [TestMethod()]
        public void REVAPPENDTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object tail = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.REVAPPEND(list, tail);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for REST
        ///</summary>
        [TestMethod()]
        public void RESTTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.REST(list);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RASSOC_IF_NOT
        ///</summary>
        [TestMethod()]
        public void RASSOC_IF_NOTTest()
        {
            object predicate = null; // TODO: Initialize to an appropriate value
            object alist = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.RASSOC_IF_NOT(predicate, alist, key);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RASSOC_IF
        ///</summary>
        [TestMethod()]
        public void RASSOC_IFTest()
        {
            object predicate = null; // TODO: Initialize to an appropriate value
            object alist = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.RASSOC_IF(predicate, alist, key);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RASSOC
        ///</summary>
        [TestMethod()]
        public void RASSOCTest()
        {
            object item = null; // TODO: Initialize to an appropriate value
            object alist = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.RASSOC(item, alist, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PAIRLIS
        ///</summary>
        [TestMethod()]
        public void PAIRLISTest()
        {
            object key = null; // TODO: Initialize to an appropriate value
            object data = null; // TODO: Initialize to an appropriate value
            object alist = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.PAIRLIS(key, data, alist);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NULL
        ///</summary>
        [TestMethod()]
        public void NULLTest()
        {
            object obj = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NULL(obj);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NUINON
        ///</summary>
        [TestMethod()]
        public void NUINONTest()
        {
            object list1 = null; // TODO: Initialize to an appropriate value
            object list2 = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NUINON(list1, list2, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NTHCDR
        ///</summary>
        [TestMethod()]
        public void NTHCDRTest()
        {
            object n = null; // TODO: Initialize to an appropriate value
            object list = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NTHCDR(n, list);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NTH
        ///</summary>
        [TestMethod()]
        public void NTHTest()
        {
            object n = null; // TODO: Initialize to an appropriate value
            object list = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NTH(n, list);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NSUBST_IF_NOT
        ///</summary>
        [TestMethod()]
        public void NSUBST_IF_NOTTest()
        {
            object _new = null; // TODO: Initialize to an appropriate value
            object predicate = null; // TODO: Initialize to an appropriate value
            object tree = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NSUBST_IF_NOT(_new, predicate, tree, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NSUBST_IF
        ///</summary>
        [TestMethod()]
        public void NSUBST_IFTest()
        {
            object _new = null; // TODO: Initialize to an appropriate value
            object predicate = null; // TODO: Initialize to an appropriate value
            object tree = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NSUBST_IF(_new, predicate, tree, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NSUBST
        ///</summary>
        [TestMethod()]
        public void NSUBSTTest()
        {
            object _new = null; // TODO: Initialize to an appropriate value
            object old = null; // TODO: Initialize to an appropriate value
            object tree = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NSUBST(_new, old, tree, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NSUBLIS
        ///</summary>
        [TestMethod()]
        public void NSUBLISTest()
        {
            object alist = null; // TODO: Initialize to an appropriate value
            object tree = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NSUBLIS(alist, tree, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NSET_EXCLUSIVE_OR
        ///</summary>
        [TestMethod()]
        public void NSET_EXCLUSIVE_ORTest()
        {
            object list1 = null; // TODO: Initialize to an appropriate value
            object list2 = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NSET_EXCLUSIVE_OR(list1, list2, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NSET_DIFFERENCE
        ///</summary>
        [TestMethod()]
        public void NSET_DIFFERENCETest()
        {
            object list1 = null; // TODO: Initialize to an appropriate value
            object list2 = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NSET_DIFFERENCE(list1, list2, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NRECONC
        ///</summary>
        [TestMethod()]
        public void NRECONCTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object tail = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NRECONC(list, tail);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NINTH
        ///</summary>
        [TestMethod()]
        public void NINTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NINTH(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NINTERSECTION
        ///</summary>
        [TestMethod()]
        public void NINTERSECTIONTest()
        {
            object list1 = null; // TODO: Initialize to an appropriate value
            object list2 = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NINTERSECTION(list1, list2, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NCONC
        ///</summary>
        [TestMethod()]
        public void NCONCTest()
        {
            object lists = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NCONC(lists);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NBUTLAST
        ///</summary>
        [TestMethod()]
        public void NBUTLASTTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object n = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.NBUTLAST(list, n);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MEMBER_IF_NOT
        ///</summary>
        [TestMethod()]
        public void MEMBER_IF_NOTTest()
        {
            object predicate = null; // TODO: Initialize to an appropriate value
            object list = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.MEMBER_IF_NOT(predicate, list, key);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MEMBER_IF
        ///</summary>
        [TestMethod()]
        public void MEMBER_IFTest()
        {
            object predicate = null; // TODO: Initialize to an appropriate value
            object list = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.MEMBER_IF(predicate, list, key);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MEMBER
        ///</summary>
        [TestMethod()]
        public void MEMBERTest()
        {
            object item = null; // TODO: Initialize to an appropriate value
            object list = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.MEMBER(item, list, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MAPLIST
        ///</summary>
        [TestMethod()]
        public void MAPLISTTest()
        {
            object function = null; // TODO: Initialize to an appropriate value
            object lists = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.MAPLIST(function, lists);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MAPL
        ///</summary>
        [TestMethod()]
        public void MAPLTest()
        {
            object function = null; // TODO: Initialize to an appropriate value
            object lists = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.MAPL(function, lists);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MAPCON
        ///</summary>
        [TestMethod()]
        public void MAPCONTest()
        {
            object function = null; // TODO: Initialize to an appropriate value
            object lists = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.MAPCON(function, lists);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MAPCAR
        ///</summary>
        [TestMethod()]
        public void MAPCARTest()
        {
            object function = null; // TODO: Initialize to an appropriate value
            object lists = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.MAPCAR(function, lists);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MAPCAN
        ///</summary>
        [TestMethod()]
        public void MAPCANTest()
        {
            object function = null; // TODO: Initialize to an appropriate value
            object lists = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.MAPCAN(function, lists);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MAPC
        ///</summary>
        [TestMethod()]
        public void MAPCTest()
        {
            object function = null; // TODO: Initialize to an appropriate value
            object lists = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.MAPC(function, lists);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MAKE_LIST
        ///</summary>
        [TestMethod()]
        public void MAKE_LISTTest()
        {
            object size = null; // TODO: Initialize to an appropriate value
            object initial_element = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.MAKE_LIST(size, initial_element);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LISTStar
        ///</summary>
        [TestMethod()]
        public void LISTStarTest()
        {
            object args = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.LISTStar(args);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LIST_LENGTH
        ///</summary>
        [TestMethod()]
        public void LIST_LENGTHTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.LIST_LENGTH(list);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LIST
        ///</summary>
        [TestMethod()]
        public void LISTTest()
        {
            object args = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.LIST(args);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LDIFF
        ///</summary>
        [TestMethod()]
        public void LDIFFTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object obj = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.LDIFF(list, obj);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LAST
        ///</summary>
        [TestMethod()]
        public void LASTTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object n = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.LAST(list, n);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for INTERSECTION
        ///</summary>
        [TestMethod()]
        public void INTERSECTIONTest()
        {
            object list1 = null; // TODO: Initialize to an appropriate value
            object list2 = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.INTERSECTION(list1, list2, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GETF
        ///</summary>
        [TestMethod()]
        public void GETFTest()
        {
            object plist = null; // TODO: Initialize to an appropriate value
            object indicator = null; // TODO: Initialize to an appropriate value
            object def = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.GETF(plist, indicator, def);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GET_PROPERTIES
        ///</summary>
        [TestMethod()]
        public void GET_PROPERTIESTest()
        {
            object plist = null; // TODO: Initialize to an appropriate value
            object indicator_list = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.GET_PROPERTIES(plist, indicator_list);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FOURTH
        ///</summary>
        [TestMethod()]
        public void FOURTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.FOURTH(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FIRST
        ///</summary>
        [TestMethod()]
        public void FIRSTTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.FIRST(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FIFTH
        ///</summary>
        [TestMethod()]
        public void FIFTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.FIFTH(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ENDP
        ///</summary>
        [TestMethod()]
        public void ENDPTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.ENDP(list);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EIGHTH
        ///</summary>
        [TestMethod()]
        public void EIGHTHTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.EIGHTH(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CopyTree
        ///</summary>
        [TestMethod()]
        public void CopyTreeTest()
        {
            object tree = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.CopyTree(tree);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for COPY_LIST
        ///</summary>
        [TestMethod()]
        public void COPY_LISTTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.COPY_LIST(list);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for COPY_ALIST
        ///</summary>
        [TestMethod()]
        public void COPY_ALISTTest()
        {
            object alist = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.COPY_ALIST(alist);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Consp
        ///</summary>
        [TestMethod()]
        public void ConspTest()
        {
            object obj = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Consp(obj);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cons
        ///</summary>
        [TestMethod()]
        public void ConsTest()
        {
            object car = null; // TODO: Initialize to an appropriate value
            object cdr = null; // TODO: Initialize to an appropriate value
            Cons expected = null; // TODO: Initialize to an appropriate value
            Cons actual;
            actual = ConsesDictionary.Cons(car, cdr);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cdr
        ///</summary>
        [TestMethod()]
        public void CdrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cdr(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cddr
        ///</summary>
        [TestMethod()]
        public void CddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cddr(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cdddr
        ///</summary>
        [TestMethod()]
        public void CdddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cdddr(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cddddr
        ///</summary>
        [TestMethod()]
        public void CddddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cddddr(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cdddar
        ///</summary>
        [TestMethod()]
        public void CdddarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cdddar(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cddar
        ///</summary>
        [TestMethod()]
        public void CddarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cddar(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cddadr
        ///</summary>
        [TestMethod()]
        public void CddadrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cddadr(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cddaar
        ///</summary>
        [TestMethod()]
        public void CddaarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cddaar(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cdar
        ///</summary>
        [TestMethod()]
        public void CdarTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cdar(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cdadr
        ///</summary>
        [TestMethod()]
        public void CdadrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cdadr(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cdaddr
        ///</summary>
        [TestMethod()]
        public void CdaddrTest()
        {
            object cons = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Cdaddr(cons);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Cdadar
        ///</summary>
        [TestMethod()]
        public void CdadarTest()
        {
            object cons = DefinedSymbols.Read.Invoke("((1 (2 . 3) 4))");
            object expected = ConsesDictionary.Cdr(ConsesDictionary.Car(ConsesDictionary.Cdr(ConsesDictionary.Car(cons))));
            object actual;
            actual = ConsesDictionary.Cdadar(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Cdadar(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Cdaar
        ///</summary>
        [TestMethod()]
        public void CdaarTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(((1 . 2) 3) 4)");
            object expected = ConsesDictionary.Cdr(ConsesDictionary.Car(ConsesDictionary.Car(cons)));
            object actual;
            actual = ConsesDictionary.Cdaar(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Cdaar(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Cdaadr
        ///</summary>
        [TestMethod()]
        public void CdaadrTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(1 ((2 . 3) 4))");
            object expected = ConsesDictionary.Cdr(ConsesDictionary.Car(ConsesDictionary.Car(ConsesDictionary.Cdr(cons))));
            object actual;
            actual = ConsesDictionary.Cdaadr(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Cdaadr(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Cdaaar
        ///</summary>
        [TestMethod()]
        public void CdaaarTest()
        {
            object cons = DefinedSymbols.Read.Invoke("((((1 . 2) 3) 4))");
            object expected = ConsesDictionary.Cdr(ConsesDictionary.Car(ConsesDictionary.Car(ConsesDictionary.Car(cons))));
            object actual;
            actual = ConsesDictionary.Cdaaar(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Cdaaar(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Car
        ///</summary>
        [TestMethod()]
        public void CarTest()
        {
            Cons cons = new Cons(1, 2);
            object expected = 1; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Car(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Car(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Cadr
        ///</summary>
        [TestMethod()]
        public void CadrTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(1 2 3 4)");
            object expected = ConsesDictionary.Car(ConsesDictionary.Cdr(cons));
            object actual;
            actual = ConsesDictionary.Cadr(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Cadr(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Caddr
        ///</summary>
        [TestMethod()]
        public void CaddrTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(1 2 3 4)");
            object expected = ConsesDictionary.Car(ConsesDictionary.Cdr(ConsesDictionary.Cdr(cons)));
            object actual;
            actual = ConsesDictionary.Caddr(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Caddr(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Cadddr
        ///</summary>
        [TestMethod()]
        public void CadddrTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(1 2 3 4)");
            object expected = ConsesDictionary.Car(ConsesDictionary.Cdr(ConsesDictionary.Cdr(ConsesDictionary.Cdr(cons))));
            object actual;
            actual = ConsesDictionary.Cadddr(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Cadddr(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Caddar
        ///</summary>
        [TestMethod()]
        public void CaddarTest()
        {
            object cons = DefinedSymbols.Read.Invoke("((1 2 3 4))");
            object expected = ConsesDictionary.Car(ConsesDictionary.Cdr(ConsesDictionary.Cdr(ConsesDictionary.Car(cons))));
            object actual;
            actual = ConsesDictionary.Caddar(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Caddar(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Cadar
        ///</summary>
        [TestMethod()]
        public void CadarTest()
        {
            object cons = DefinedSymbols.Read.Invoke("((1 2 3 4))");
            object expected = ConsesDictionary.Car(ConsesDictionary.Cdr(ConsesDictionary.Car(cons)));
            object actual;
            actual = ConsesDictionary.Cadar(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Cadar(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Cadadr
        ///</summary>
        [TestMethod()]
        public void CadadrTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(1 (2 3 4))");
            object expected = ConsesDictionary.Car(ConsesDictionary.Cdr(ConsesDictionary.Car(ConsesDictionary.Cdr(cons))));
            object actual;
            actual = ConsesDictionary.Cadadr(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Cadadr(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Cadaar
        ///</summary>
        [TestMethod()]
        public void CadaarTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(((1 2 3 4)))");
            object expected = ConsesDictionary.Car(ConsesDictionary.Cdr(ConsesDictionary.Car(ConsesDictionary.Car(cons))));
            object actual;
            actual = ConsesDictionary.Cadaar(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Cadaar(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Caar
        ///</summary>
        [TestMethod()]
        public void CaarTest()
        {
            object cons = DefinedSymbols.Read.Invoke("((1 2) 3 4)");
            object expected = ConsesDictionary.Car(ConsesDictionary.Car(cons)); 
            object actual;
            actual = ConsesDictionary.Caar(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Caar(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Caadr
        ///</summary>
        [TestMethod()]
        public void CaadrTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(1 (2 3) 4)");
            object expected = ConsesDictionary.Car(ConsesDictionary.Car(ConsesDictionary.Cdr(cons))); 
            object actual;
            actual = ConsesDictionary.Caadr(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Caadr(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Caaddr
        ///</summary>
        [TestMethod()]
        public void CaaddrTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(1 2 (3 4))");
            object expected = ConsesDictionary.Car(ConsesDictionary.Car(ConsesDictionary.Cdr(ConsesDictionary.Cdr(cons))));
            object actual;
            actual = ConsesDictionary.Caaddr(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Caaddr(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Caadar
        ///</summary>
        [TestMethod()]
        public void CaadarTest()
        {
            object cons =  DefinedSymbols.Read.Invoke("((1 (2 3) 4))");
            object expected = ConsesDictionary.Car(ConsesDictionary.Car(ConsesDictionary.Cdr(ConsesDictionary.Car(cons))));
            object actual;
            actual = ConsesDictionary.Caadar(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Caadar(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Caaar
        ///</summary>
        [TestMethod()]
        public void CaaarTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(((1 2) 3 4))");
            object expected = ConsesDictionary.Car(ConsesDictionary.Car(ConsesDictionary.Car(cons)));
            object actual;
            actual = ConsesDictionary.Caaar(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Caaar(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Caaadr
        ///</summary>
        [TestMethod()]
        public void CaaadrTest()
        {
            object cons = DefinedSymbols.Read.Invoke("(1 ((2 3 4)))");
            object expected = ConsesDictionary.Car(ConsesDictionary.Car(ConsesDictionary.Car(ConsesDictionary.Cdr(cons))));
            object actual;
            actual = ConsesDictionary.Caaadr(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Caaadr(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Caaaar
        ///</summary>
        [TestMethod()]
        public void CaaaarTest()
        {
            object cons = DefinedSymbols.Read.Invoke("((((1 2) 3 4)))");
            object expected = ConsesDictionary.Car(ConsesDictionary.Car(ConsesDictionary.Car(ConsesDictionary.Car(cons))));
            object actual;
            actual = ConsesDictionary.Caaaar(cons);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = ConsesDictionary.Caaaar(DefinedSymbols.NIL);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BUTLAST
        ///</summary>
        [TestMethod()]
        public void BUTLASTTest()
        {
            object list = null; // TODO: Initialize to an appropriate value
            object n = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.BUTLAST(list, n);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Atomp
        ///</summary>
        [TestMethod()]
        public void AtompTest()
        {
            object obj = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.Atomp(obj);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ASSOC_IF_NOT
        ///</summary>
        [TestMethod()]
        public void ASSOC_IF_NOTTest()
        {
            object predicate = null; // TODO: Initialize to an appropriate value
            object alist = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.ASSOC_IF_NOT(predicate, alist, key);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ASSOC_IF
        ///</summary>
        [TestMethod()]
        public void ASSOC_IFTest()
        {
            object predicate = null; // TODO: Initialize to an appropriate value
            object alist = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.ASSOC_IF(predicate, alist, key);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ASSOC
        ///</summary>
        [TestMethod()]
        public void ASSOCTest()
        {
            object item = null; // TODO: Initialize to an appropriate value
            object alist = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.ASSOC(item, alist, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for APPEND
        ///</summary>
        [TestMethod()]
        public void APPENDTest()
        {
            object lists = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.APPEND(lists);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ADJOIN
        ///</summary>
        [TestMethod()]
        public void ADJOINTest()
        {
            object item = null; // TODO: Initialize to an appropriate value
            object list = null; // TODO: Initialize to an appropriate value
            object key = null; // TODO: Initialize to an appropriate value
            object test = null; // TODO: Initialize to an appropriate value
            object test_not = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.ADJOIN(item, list, key, test, test_not);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ACONS
        ///</summary>
        [TestMethod()]
        public void ACONSTest()
        {
            object key = null; // TODO: Initialize to an appropriate value
            object datum = null; // TODO: Initialize to an appropriate value
            object alist = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = ConsesDictionary.ACONS(key, datum, alist);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
