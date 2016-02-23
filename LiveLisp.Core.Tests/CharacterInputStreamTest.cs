using LiveLisp.Core.Types.Streams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace LiveLisp.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for CharacterInputStreamTest and is intended
    ///to contain all CharacterInputStreamTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CharacterInputStreamTest
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
        ///A test for Real
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LiveLisp.Core.dll")]
        public void RealTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            CharacterInputStream_Accessor target = new CharacterInputStream_Accessor(param0); // TODO: Initialize to an appropriate value
            TextReader expected = null; // TODO: Initialize to an appropriate value
            TextReader actual;
            target.Real = expected;
            actual = target.Real;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RawIndex
        ///</summary>
        [TestMethod()]
        public void RawIndexTest()
        {
            TextReader real = null; // TODO: Initialize to an appropriate value
            CharacterInputStream target = new CharacterInputStream(real); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.RawIndex;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OpenStreamP
        ///</summary>
        [TestMethod()]
        public void OpenStreamPTest()
        {
            TextReader real = null; // TODO: Initialize to an appropriate value
            CharacterInputStream target = new CharacterInputStream(real); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.OpenStreamP;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Listen
        ///</summary>
        [TestMethod()]
        public void ListenTest()
        {
            TextReader real = null; // TODO: Initialize to an appropriate value
            CharacterInputStream target = new CharacterInputStream(real); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Listen;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DocId
        ///</summary>
        [TestMethod()]
        public void DocIdTest()
        {
            TextReader real = null; // TODO: Initialize to an appropriate value
            CharacterInputStream target = new CharacterInputStream(real); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.DocId = expected;
            actual = target.DocId;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CurrentLine
        ///</summary>
        [TestMethod()]
        public void CurrentLineTest()
        {
            TextReader real = null; // TODO: Initialize to an appropriate value
            CharacterInputStream target = new CharacterInputStream(real); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.CurrentLine;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CurrentColumn
        ///</summary>
        [TestMethod()]
        public void CurrentColumnTest()
        {
            TextReader real = null; // TODO: Initialize to an appropriate value
            CharacterInputStream target = new CharacterInputStream(real); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.CurrentColumn;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }/*

        /// <summary>
        ///A test for UnreadChar
        ///</summary>
        [TestMethod()]
        public void UnreadCharTest()
        {
            CharacterInputStream cis = new CharacterInputStream("0123");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                char c = (char)cis.Read();

                if (i % 2 == 0)
                    sb.AppendLine(i + " : " + c);
                else
                    cis.UnreadChar(c);
            }

            Assert.AreEqual("0 : 0\r\n2 : 1\r\n4 : 2\r\n", sb.ToString());
        }
/*
        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            TextReader real = null; // TODO: Initialize to an appropriate value
            CharacterInputStream target = new CharacterInputStream(real); // TODO: Initialize to an appropriate value
            object obj = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            TextReader real = null; // TODO: Initialize to an appropriate value
            CharacterInputStream target = new CharacterInputStream(real); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Close
        ///</summary>
        [TestMethod()]
        public void CloseTest()
        {
            TextReader real = null; // TODO: Initialize to an appropriate value
            CharacterInputStream target = new CharacterInputStream(real); // TODO: Initialize to an appropriate value
            target.Close();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Clear
        ///</summary>
        [TestMethod()]
        public void ClearTest()
        {
            TextReader real = null; // TODO: Initialize to an appropriate value
            CharacterInputStream target = new CharacterInputStream(real); // TODO: Initialize to an appropriate value
            target.Clear();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }*/
    }
}
