using LiveLisp.Core.BuiltIns.Characters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using LiveLisp.Core.BuiltIns.Strings;
using LiveLisp.Core.Types;
using LiveLisp.Core.BuiltIns.Numbers;
namespace LiveLisp.Core.Tests.Builtins
{
    
    
    /// <summary>
    ///This is a test class for CharactersDictionaryTest and is intended
    ///to contain all CharactersDictionaryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CharactersDictionaryTest
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
        public static void MyClassInitialize(TestContext testContext)
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

        /*
        /// <summary>
        ///A test for UpperCaseP
        ///</summary>
        [TestMethod()]
        public void UpperCasePTest()
        {
            char character = 'T';
            object expected = DefinedSymbols.T;
            object actual;
            actual = Package.LookupInPackages("UPPER-CASE-P").Invoke(character);
            Assert.AreEqual(expected, actual);

            character = '@';
            expected = DefinedSymbols.NIL;
            actual = CharactersDictionary.UpperCaseP(character);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for StandardCharP
        ///</summary>
        [TestMethod()]
        public void StandardCharPTest()
        {
            char character = 'T'; 
            object expected = DefinedSymbols.T; 
            object actual;
            actual = Package.LookupInPackages("STANDARD-CHAR-P").Invoke(character);
            Assert.AreEqual(expected, actual);

            character = 'ф';
            expected = DefinedSymbols.NIL;
            actual = CharactersDictionary.StandardCharP(character);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for set_CharName
        ///</summary>
        [TestMethod()]
        public void set_CharNameTest()
        {
            object character = null; // TODO: Initialize to an appropriate value
            object name = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.set_CharName(character, name);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NameChar
        ///</summary>
        [TestMethod()]
        public void NameCharTest()
        {
            object name = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.NameChar(name);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LowerCaseP
        ///</summary>
        [TestMethod()]
        public void LowerCasePTest()
        {
            char character = 't';
            object expected = DefinedSymbols.T;
            object actual;
            actual = Package.LookupInPackages("LOWER-CASE-P").Invoke(character);
            Assert.AreEqual(expected, actual);

            character = '@';
            expected = DefinedSymbols.NIL;
            actual = CharactersDictionary.LowerCaseP(character);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GraphicCharP
        ///</summary>
        [TestMethod()]
        public void GraphicCharPTest()
        {
            char character = 't';
            object expected = DefinedSymbols.T;
            object actual;
            actual = Package.LookupInPackages("GRAPHIC-CHAR-P").Invoke(character);
            Assert.AreEqual(expected, actual);

            character = ' ';
            expected = DefinedSymbols.T;
            actual = Package.LookupInPackages("GRAPHIC-CHAR-P").Invoke(character);
            Assert.AreEqual(expected, actual);

            character = (char)9; // note not all character considered to be whitespace
            expected = DefinedSymbols.NIL;
            actual = Package.LookupInPackages("GRAPHIC-CHAR-P").Invoke(character);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DigitCharP
        ///</summary>
        [TestMethod()]
        public void DigitCharPTest()
        {
            char character = 't';
            LispInteger base_ = 30;
            object expected = (LispInteger)29;
            object actual;
            actual = Package.LookupInPackages("DIGIT-CHAR-P").Invoke(character, base_);
            Assert.AreEqual(expected, actual);

            character = ' ';
            expected = DefinedSymbols.NIL;
            actual = Package.LookupInPackages("DIGIT-CHAR-P").Invoke(character,base_);
            Assert.AreEqual(expected, actual);

            character = 'T';
            expected = DefinedSymbols.NIL;
            actual = Package.LookupInPackages("DIGIT-CHAR-P").Invoke(character);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DigitChar
        ///</summary>
        [TestMethod()]
        public void DigitCharTest()
        {
            LispInteger character = 10;
            LispInteger base_ = 11;
            object expected = (char)'A';
            object actual;
            actual = Package.LookupInPackages("DIGIT-CHAR").Invoke(character, base_);
            Assert.AreEqual(expected, actual);

            character = 10;
            expected = DefinedSymbols.NIL;
            actual = Package.LookupInPackages("DIGIT-CHAR").Invoke(character);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CodeChar
        ///</summary>
        [TestMethod()]
        public void CodeCharTest()
        {
            LispInteger code = 41;
            object expected = (char)(char)41;
            object actual;
            actual = Package.LookupInPackages("CODE-CHAR").Invoke(code);
            Assert.AreEqual(expected, actual);

            expected = DefinedSymbols.NIL;
            actual = Package.LookupInPackages("CODE-CHAR").Invoke((LispInteger)(-666));
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CharUpcase
        ///</summary>
        [TestMethod()]
        public void CharUpcaseTest()
        {
            char character = 't';
            object expected = (char)'T';
            object actual;
            actual = Package.LookupInPackages("CHAR-UPCASE").Invoke(character);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CharNotLessp
        ///</summary>
        [TestMethod()]
        public void CharNotLesspTest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharNotLessp(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharNotGreaterP
        ///</summary>
        [TestMethod()]
        public void CharNotGreaterPTest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharNotGreaterP(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharNotequal
        ///</summary>
        [TestMethod()]
        public void CharNotequalTest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharNotequal(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharNE
        ///</summary>
        [TestMethod()]
        public void CharNETest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharNE(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharName
        ///</summary>
        [TestMethod()]
        public void CharNameTest()
        {
            object character = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharName(character);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharLessp
        ///</summary>
        [TestMethod()]
        public void CharLesspTest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharLessp(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharLE
        ///</summary>
        [TestMethod()]
        public void CharLETest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharLE(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharL
        ///</summary>
        [TestMethod()]
        public void CharLTest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharL(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharInt
        ///</summary>
        [TestMethod()]
        public void CharIntTest()
        {
            char character = 'a';
            LispInteger expected = new LispInteger('a');
            LispInteger actual;
            actual = Package.LookupInPackages("CHAR-INT").Invoke(character) as LispInteger;
            Assert.AreEqual<LispInteger>(expected, actual);
        }

        /// <summary>
        ///A test for Chargreaterp
        ///</summary>
        [TestMethod()]
        public void ChargreaterpTest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.Chargreaterp(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharGE
        ///</summary>
        [TestMethod()]
        public void CharGETest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharGE(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharG
        ///</summary>
        [TestMethod()]
        public void CharGTest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharG(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharEqual
        ///</summary>
        [TestMethod()]
        public void CharEqualTest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharEqual(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharE
        ///</summary>
        [TestMethod()]
        public void CharETest()
        {
            object[] characters = null; // TODO: Initialize to an appropriate value
            object expected = null; // TODO: Initialize to an appropriate value
            object actual;
            actual = CharactersDictionary.CharE(characters);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CharDowncase
        ///</summary>
        [TestMethod()]
        public void CharDowncaseTest()
        {
            char character = 'T';
            object expected = (char)'t';
            object actual;
            actual = Package.LookupInPackages("CHAR-DOWNCASE").Invoke(character);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CharCode
        ///</summary>
        [TestMethod()]
        public void CharCodeTest()
        {
            char character = 'a';
            LispInteger expected = new LispInteger('a');
            LispInteger actual;
            actual = Package.LookupInPackages("CHAR-CODE").Invoke(character) as LispInteger;
            Assert.AreEqual<LispInteger>(expected, actual);
        }

        /// <summary>
        ///A test for Characterp
        ///</summary>
        [TestMethod()]
        public void CharacterpTest()
        {
            object obj = (char)'q'; 
            object expected = DefinedSymbols.T;
            object actual;
            actual = Package.LookupInPackages("CHARACTERP").Invoke(obj);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Character
        ///</summary>
        [TestMethod()]
        public void CharacterTest()
        {
            char ch = 'q';
            Assert.AreEqual(ch, Package.LookupInPackages("CHARACTER").Invoke(ch));

            object obj = (string)"q";
            Assert.AreEqual(ch, Package.LookupInPackages("CHARACTER").Invoke(obj));

            obj = Package.Current.Intern("q");
            Assert.AreEqual(ch, Package.LookupInPackages("CHARACTER").Invoke(obj));

            obj = 'q';
            Assert.AreEqual(ch, Package.LookupInPackages("CHARACTER").Invoke(obj));
        }

        /// <summary>
        ///A test for BothCaseP
        ///</summary>
        [TestMethod()]
        public void BothCasePTest()
        {
            char character = 'T';
            object expected = DefinedSymbols.T;
            object actual;
            actual = Package.LookupInPackages("BOTH-CASE-P").Invoke(character);
            Assert.AreEqual(expected, actual);

            character = 't';
            expected = DefinedSymbols.T;
            actual = CharactersDictionary.BothCaseP(character);
            Assert.AreEqual(expected, actual);

            character = '@';
            expected = DefinedSymbols.NIL;
            actual = CharactersDictionary.BothCaseP(character);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for AlphaCharP
        ///</summary>
        [TestMethod()]
        public void AlphaCharPTest()
        {
            char character = 'T';
            object expected = DefinedSymbols.T;
            object actual;
            actual = Package.LookupInPackages("ALPHA-CHAR-P").Invoke(character);
            Assert.AreEqual(expected, actual);

            character = 'ы';
            expected = DefinedSymbols.T;
            actual = Package.LookupInPackages("ALPHA-CHAR-P").Invoke(character);
            Assert.AreEqual(expected, actual);

            character = '5';
            expected = DefinedSymbols.NIL;
            actual = Package.LookupInPackages("ALPHA-CHAR-P").Invoke(character);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Alhanumericp
        ///</summary>
        [TestMethod()]
        public void AlphanumericpTest()
        {
            char character = 'T';
            object expected = DefinedSymbols.T;
            object actual;
            actual = Package.LookupInPackages("ALPHANUMERICP").Invoke(character);
            Assert.AreEqual(expected, actual);

            character = 't';
            expected = DefinedSymbols.T;
            actual = Package.LookupInPackages("ALPHANUMERICP").Invoke(character);
            Assert.AreEqual(expected, actual);

            character = 'ф';
            expected = DefinedSymbols.NIL;
            actual = Package.LookupInPackages("ALPHANUMERICP").Invoke(character);
            Assert.AreEqual(expected, actual);
        }
         */
    }
}
