using LiveLisp.Core.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace LiveLisp.Core.Tests
{
    
    
    /// <summary>
    ///This is a test class for ConsTest and is intended
    ///to contain all ConsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConsTest
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
        [ClassInitialize()]
        public static void ConsTestInitialize(TestContext testContext)
        {
            Initialization.Initialize();
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

        [TestMethod()]
        public void ToStringSimpleTest()
       { 
            Cons target = Cons.FromCollection(new string[] { "first", "second", "third", "fourth", "fifth" });
            Assert.AreEqual<string>("(first second third fourth fifth)",target.ToString());
        }

        [TestMethod()]
        public void ToStringNilTest()
        {
            Cons target = new Cons(DefinedSymbols.NIL);
            Assert.AreEqual<string>("()", target.ToString());
        }

        [TestMethod()]
        public void ToStringNullTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", null, "fourth", "fifth" });
            Assert.AreEqual<string>("(first second clrnull fourth fifth)", target.ToString());
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod()]
        public void ItemTest0()
        {
            Cons target = Cons.FromCollection(new string[] { "clrnull", "second", "third", "fourth", "fifth" });
            int index = 0;
            object expected = "first";
            object actual;
            target[index] = expected;
            actual = target[index];
            Assert.AreEqual(expected, actual);
            Assert.AreEqual("first", actual);
        }

        [TestMethod()]
        public void ItemTest2()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "clrnull", "fourth", "fifth" });
            int index = 2;
            object expected = "third";
            object actual;
            target[index] = expected;
            actual = target[index];
            Assert.AreEqual(expected, actual);
            Assert.AreEqual("third", actual);
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod()]
        public void KeyItemImproperListTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "clrnull", "fourth", "fifth" });
            object key = "second"; // no matter
            object actual;
            bool _rex = false;
            try
            {
                actual = target[key];
            }
            catch (Exception ex)
            {
                _rex = true;
                Assert.IsInstanceOfType(ex, typeof(SimpleErrorException), "Cons.items[object] must generate SimpleErrorException as response to improper using");
            }
            if (!_rex)
                Assert.Fail("items[object] property of cons must fail on non property lists (in case of this test - odd items found");
        }

        [TestMethod()]
        public void KeyItemTest()
        {
            Cons target =Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object key = "key"; // no matter
            string expected = "value";
            object actual;
            target[key] = expected;
            actual = target[key];
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void KeyItemBrokenKeyTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object key = "broken_key"; // no matter
            string expected = "value";
            bool _rex = false;
            try
            {
                target[key] = expected;
            }
            catch(Exception ex)
            {
                _rex = true;
                Assert.IsInstanceOfType(ex, typeof(KeyNotFoundException));
            }
            if (!_rex)
                Assert.Fail("items[object] property of cons must fail if key not found");
        }

        [TestMethod()]
        public void KeyItemNewKeyTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object key = "broken_key"; // no matter
            string expected = "broken_value";
            object actual;
            target.AddKeyValue(key, expected);
            actual = target[key];
            Assert.AreEqual(expected, actual);
            int expected_len = target.Count;

            expected = "new_broken_value";
            target.AddKeyValue(key, expected);
            actual = target[key];
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected_len, target.Count);
        }
/*
        /// <summary>
        ///A test for IsSynchronized
        ///</summary>
        [TestMethod()]
        public void IsSynchronizedTest()
        {
            Cons target = new Cons(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsSynchronized;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsReadOnly
        ///</summary>
        [TestMethod()]
        public void IsReadOnlyTest()
        {
            Cons target = new Cons(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsReadOnly;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsFixedSize
        ///</summary>
        [TestMethod()]
        public void IsFixedSizeTest()
        {
            Cons target = new Cons(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsFixedSize;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
*/
        /// <summary>
        ///A test for Count
        ///</summary>
        [TestMethod()]
        public void CountTest()
        {
            Cons target = new Cons(DefinedSymbols.NIL);
            target.Cdr = new Cons(2, new Cons(3, new Cons(4)));
            int actual;
            actual = target.Count;
            Assert.AreEqual(4, actual);
        }

        

        /// <summary>
        ///A test for Count
        ///</summary>
        [TestMethod()]
        public void CountDelayedTest()
        {
            Cons target = Cons.FromCollection(new int[] { 1, 2, 3, 4 });
            int actual;
            actual = target.Count;
            Assert.AreEqual(4, actual);
        }

        /// <summary>
        ///A test for Count
        ///</summary>
        [TestMethod()]
        public void CountDelayedTest1()
        {
            Cons target = Cons.FromCollection(new int[] { 1, 2, 3, 4 });
            int actual;
            actual = target.Count;
            Assert.AreEqual(4, actual);
            
            Cons cons = new Cons(5);
            target = new Cons(1, new Cons(2, new Cons(3, new Cons(4, cons))));
            actual = target.Count;
            Assert.AreEqual(5, actual);

            cons.SetCdrDelayed(Cons.FromCollection(new int[] { 6, 7, 8, 9 }));
            actual = cons.Count;
            Assert.AreEqual(1, actual);

            actual = target.Count;
            Assert.AreEqual(5, actual);

            cons.CalculateDelayedLengthNoThrow();
            actual = cons.Count;
            Assert.AreEqual(5, actual);

            actual = target.Count;
            Assert.AreEqual(9, actual);
        }


        /// <summary>
        ///A test for Cdr
        ///</summary>
        [TestMethod()]
        public void CdrTest()
        {
            Cons target = new Cons(DefinedSymbols.NIL);

            Assert.AreEqual(0, target.Count);

            Assert.AreEqual(true, target.IsProperList);

            target.Cdr = new Cons(DefinedSymbols.NIL);

            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(true, target.IsProperList);

            target.Cdr = 5;
            Assert.AreEqual(false, target.IsProperList);
        }


        /// <summary>
        ///A test for RemoveAt
        ///</summary>
        [TestMethod()]
        public void RemoveAtHeadTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            int index = 0;
            target.RemoveAt(index);
            Assert.AreEqual(5, target.Count);
            Assert.AreEqual("second", target.Car);
        }

        [TestMethod()]
        public void RemoveAtEndTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            int index = 5;
            target.RemoveAt(index);
            Assert.AreEqual(5, target.Count);
            Assert.AreEqual("fourth", target[4]);
        }

        [TestMethod()]
        public void RemoveAtMedianTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            int index = 2;
            target.RemoveAt(index);
            Assert.AreEqual(5, target.Count);
            Assert.AreEqual("clrnull", target[2]);
        }

        [TestMethod()]
        public void RemoveHeadTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object index = "first";
            target.Remove(index);
            Assert.AreEqual(5, target.Count);
            Assert.AreEqual("second", target.Car);
        }

        [TestMethod()]
        public void RemoveEndTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object index = "fifth";
            target.Remove(index);
            Assert.AreEqual(5, target.Count);
            Assert.AreEqual("fourth", target[4]);
        }

        [TestMethod()]
        public void RemoveMedianTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object index = "key";
            target.Remove(index);
            Assert.AreEqual(5, target.Count);
            Assert.AreEqual("clrnull", target[2]);
        }

        /// <summary>
        ///A test for Insert
        ///</summary>
        [TestMethod()]
        public void InsertToHeadTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            int index = 0;
            object value = "head";
            target.Insert(index, value);
            Assert.AreEqual(7, target.Count);
            Assert.AreEqual(value, target[0]);
        }

        [TestMethod()]
        public void InsertToEndTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            int index = 6; // TODO: Initialize to an appropriate value
            object value = "end"; // TODO: Initialize to an appropriate value
            target.Insert(index, value);
            Assert.AreEqual(7, target.Count);
            Assert.AreEqual(value, target[6]);
        }

        [TestMethod()]
        public void InsertToMedianTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            int index = 2;
            object value = "median"; // TODO: Initialize to an appropriate value
            target.Insert(index, value);
            Assert.AreEqual(7, target.Count);
            Assert.AreEqual(value, target[2]);
        }

        /// <summary>
        ///A test for IndexOf
        ///</summary>
        [TestMethod()]
        public void IndexOfHeadTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object value = "first"; // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.IndexOf(value);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IndexOf
        ///</summary>
        [TestMethod()]
        public void IndexOfLastTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object value = "fifth"; // TODO: Initialize to an appropriate value
            int expected = 5; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.IndexOf(value);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IndexOf
        ///</summary>
        [TestMethod()]
        public void IndexOfMedianTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object value = "key"; // TODO: Initialize to an appropriate value
            int expected = 2; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.IndexOf(value);
            Assert.AreEqual(expected, actual);
        }

        
        /// <summary>
        ///A test for CopyTo
        ///</summary>
        [TestMethod()]
        public void CopyToTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            Array array = new string[6];
            int index = 0; 
            target.CopyTo(array, index);
            Assert.AreEqual(target.Count, array.Length);
        }

        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod()]
        public void ContainsTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object value = "clrnull";
            bool expected = true;
            bool actual;
            actual = target.Contains(value);
            Assert.AreEqual(expected, actual);
            expected = false;
            value = "qwertyu";
            actual = target.Contains(value);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Clear
        ///</summary>
        [TestMethod()]
        public void ClearTest()
        {
            Cons target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            target.Clear();
            Assert.AreEqual(0, target.Count);
            Assert.AreEqual(DefinedSymbols.NIL, target.Car);
            Assert.AreEqual(DefinedSymbols.NIL, target.Cdr);
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            Cons target = new Cons(DefinedSymbols.NIL);
            Assert.AreEqual(0, target.Count);
            target.Add("123");
            Assert.AreEqual(1, target.Count);

            target = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            object value = "end";
            int expected = 6;
            int actual;
            actual = target.Add(value);
            Assert.AreEqual(expected, actual);
        }
        
        

        /// <summary>
        ///A test for Cons Constructor
        ///</summary>
        [TestMethod()]
        public void ConsConstructorTest2()
        {
            Cons target = new Cons(DefinedSymbols.NIL);
            Assert.AreEqual(0, target.Count);
            Assert.AreEqual(DefinedSymbols.NIL, target.Car);
            Assert.AreEqual(DefinedSymbols.NIL, target.Cdr);
        }

        /// <summary>
        ///A test for Cons Constructor
        ///</summary>
        [TestMethod()]
        public void ConsConstructorTest1()
        {
            object car = 1;
            object cdr = null;
            Cons target = new Cons(car, cdr);

            Assert.AreEqual(false, target.IsProperList);
            target.Cdr = DefinedSymbols.NIL;
            Assert.AreEqual(true, target.IsProperList);
            Assert.AreEqual(1, target.Count);
        }

        /// <summary>
        ///A test for Cons Constructor
        ///</summary>
        [TestMethod()]
        public void ConsConstructorTest()
        {
            object car = null; // TODO: Initialize to an appropriate value
            Cons target = new Cons(car);

            Assert.AreEqual(true, target.IsProperList);
            Assert.AreEqual(1, target.Count);
        }

        [TestMethod]
        public void MakeCopyTest()
        {
            var cons = Cons.FromCollection(new string[] { "first", "second", "key", "clrnull", "fourth", "fifth" });
            var copy = cons.MakeCopy();
            Assert.AreEqual(6, copy.Count);
        }

        [TestMethod]
        public void LastTest()
        {
            var pair = new Cons(2, 100);
            var cons = new Cons(1, new Cons(3, new Cons(4, pair)));
            Assert.AreEqual(pair, cons.Last);
        }

        #region errors driven
        [TestMethod]
        public void TestNilAsRegularMember()
        {
            Cons cons = new Cons(DefinedSymbols.NIL);

            Assert.AreEqual(1, cons.Count);

            Assert.AreEqual("(NIL)", cons.ToString());

            int count = 0;

            foreach (var item in cons)
            {
                count++;
            }

            Assert.AreEqual(1, count);

            cons = new Cons(DefinedSymbols.NIL, cons);

            Assert.AreEqual(2, cons.Count);

            Assert.AreEqual("(NIL NIL)", cons.ToString());

            count = 0;

            foreach (var item in cons)
            {
                count++;
            }

            Assert.AreEqual(2, count);
        }
        #endregion
    }
}
