using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using LiveLisp.Core.Types.Streams;

namespace LiveLisp.Core.Tests.Types.Streams
{
    [TestClass]
    public class BinaryInputStreamTests
    {

        Stream stream;

        [TestInitialize()]
        public void TestInitialize()
        {
            stream = new MemoryStream(new byte[]{0x46, 0x49, 0x4c,
                0x45,0x53,0x3d,0x34,0x30,0x0d,0x0a}, false);
        }
        /*[TestCleanup()]
        public void TestCleanup()
        {
        }*/
        [TestMethod]
        public void TestSignedByte8()
        {
           IBinaryInputStream bstream = BinaryStreamsfactory.CreateInputStream(stream, new BinaryStreamElementType (true, 8));
          
            Assert.AreEqual(true, bstream is SignedByte8InputStream);

            Assert.AreEqual(0x46, (int)bstream.ReadByte());
        }


        [TestMethod]
        public void TestSignedByte16()
        {
            IBinaryInputStream bstream = BinaryStreamsfactory.CreateInputStream(stream, new BinaryStreamElementType ( true,  16 ));

            Assert.AreEqual(true, bstream is SignedByte16InputStream);

            Assert.AreEqual(18758, (int)bstream.ReadByte());
        }


        [TestMethod]
        public void TestSignedByte32()
        {
            IBinaryInputStream bstream = BinaryStreamsfactory.CreateInputStream(stream, new BinaryStreamElementType { Signed = true, Bits = 32 });

            Assert.AreEqual(true, bstream is SignedByte32InputStream);

            Assert.AreEqual(1162627398, (int)bstream.ReadByte());
        }


        [TestMethod]
        public void TestSignedByte64()
        {
            IBinaryInputStream bstream = BinaryStreamsfactory.CreateInputStream(stream, new BinaryStreamElementType { Signed = true, Bits = 64 });

            Assert.AreEqual(true, bstream is SignedByte64InputStream);

            Assert.AreEqual(3473468640463702342, (Int64)bstream.ReadByte());
        }


        //326 393 1301
        [TestMethod]
        public void TestSignedGeneral()
        {
            IBinaryInputStream bstream = BinaryStreamsfactory.CreateInputStream(stream, new BinaryStreamElementType { Signed = true, Bits = 11 });

            Assert.AreEqual(true, bstream is GeneralSignedBinaryInputStream);

            Assert.AreEqual(326, (Int32)bstream.ReadByte());

            Assert.AreEqual(393, (Int32)bstream.ReadByte());

            Assert.AreEqual(1301, (Int32)bstream.ReadByte());
        }

        [TestMethod]
        public void TestUnsignedByte8()
        {
            IBinaryInputStream bstream = BinaryStreamsfactory.CreateInputStream(stream, new BinaryStreamElementType { Signed = false, Bits = 8 });

            Assert.AreEqual(true, bstream is UnsignedByte8InputStream);

            Assert.AreEqual(0x46, (int)bstream.ReadByte());
        }


        [TestMethod]
        public void TestUnsignedByte16()
        {
            IBinaryInputStream bstream = BinaryStreamsfactory.CreateInputStream(stream, new BinaryStreamElementType { Signed = false, Bits = 16 });

            Assert.AreEqual(true, bstream is UnsignedByte16InputStream);

            Assert.AreEqual(18758, (int)bstream.ReadByte());
        }


        [TestMethod]
        public void TestUnsignedByte32()
        {
            IBinaryInputStream bstream = BinaryStreamsfactory.CreateInputStream(stream, new BinaryStreamElementType { Signed = false, Bits = 32 });

            Assert.AreEqual(true, bstream is UnsignedByte32InputStream);

            Assert.AreEqual(1162627398, (int)bstream.ReadByte());
        }


        [TestMethod]
        public void TestUnsignedByte64()
        {
            IBinaryInputStream bstream = BinaryStreamsfactory.CreateInputStream(stream, new BinaryStreamElementType { Signed = false, Bits = 64 });

            Assert.AreEqual(true, bstream is UnsignedByte64InputStream);

            Assert.AreEqual(3473468640463702342, (Int64)bstream.ReadByte());
        }


        //326 393 1301
        [TestMethod]
        public void TestUnsignedGeneral()
        {
            IBinaryInputStream bstream = BinaryStreamsfactory.CreateInputStream(stream, new BinaryStreamElementType { Signed = false, Bits = 11 });

            Assert.AreEqual(true, bstream is GeneralUnsignedBinaryInputStream);

            Assert.AreEqual(326, (Int32)bstream.ReadByte());

            Assert.AreEqual(393, (Int32)bstream.ReadByte());

            Assert.AreEqual(1301, (Int32)bstream.ReadByte());
        }

        [TestMethod]
        public void TypedTests()
        {
            TypedBinaryInputStream<Int32> bstream = BinaryStreamsfactory.CreateInputStream<Int32>(stream);
        }
    }
}
