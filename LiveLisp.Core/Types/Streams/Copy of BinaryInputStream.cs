using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.Types.Streams
{

    public abstract class BinaryInputStream : IBinaryInputStream
    {

        bool closed;
        private Stream baseStream;

        public Stream BaseStream
        {
            get { return baseStream; }
        }

        protected BinaryStreamElementType elementType;


        public BinaryInputStream(Stream baseStream, BinaryStreamElementType elementType)
        {
            this.baseStream = baseStream;
            this.elementType = elementType;
        }

        public BinaryStreamElementType ElementType
        {
            get { return elementType; }
        }

        protected void CheckIsClosed()
        {

        }

        protected void ThrowEof()
        {

        }

        public virtual object ReadByte(bool eofErrorP, object eofValue)
        {
            CheckIsClosed();

            if (Eof)
            {
                if (eofErrorP)
                    ThrowEof();
                return eofErrorP;
            }

            return ReadByteImpl();
        }

        public abstract object ReadByteImpl();

        public virtual object ReadByte()
        {
            return ReadByte(true, -1);
        }

        public virtual void Clear()
        {
            
        }

        public bool Listen
        {
            get { throw new NotImplementedException(); }
        }

        public abstract bool Eof
        {
            get;
        }
    
        public void Close()
        {
            closed = true;
            baseStream.Close();
            baseStream.Dispose();
        }

        public bool OpenStreamP
        {
            get { return !closed; }
        }

        public virtual void Dispose()
        {
            Close();
        }
    }

    class SignedByte8InputStream : BinaryInputStream
    {
        public SignedByte8InputStream(Stream baseStream, BinaryStreamElementType elementType)
            : base(baseStream, elementType)
        {
        }

        public override object ReadByteImpl()
        {
            return BaseStream.ReadByte();
        }

        public override bool Eof
        {
            get { return BaseStream.Position == BaseStream.Length; }
        }
    }

    class SignedByte16InputStream : BinaryInputStream
    {
        public SignedByte16InputStream(Stream baseStream, BinaryStreamElementType elementType)
            : base(baseStream, elementType)
        {
        }

        public override object ReadByteImpl()
        {
            return (UInt16)BaseStream.ReadByte() | ((UInt16)BaseStream.ReadByte() << 8);
        }

        public override bool Eof
        {
            get { return (BaseStream.Length - BaseStream.Position) < 2 ? true : false; }
        }
    }

    class SignedByte32InputStream : BinaryInputStream
    {
        public SignedByte32InputStream(Stream baseStream, BinaryStreamElementType elementType)
            : base(baseStream, elementType)
        {
        }

        public override object ReadByteImpl()
        {
            return (Int32)BaseStream.ReadByte() | ((Int32)BaseStream.ReadByte() << 8) | ((Int32)BaseStream.ReadByte() << 16) | ((Int32)BaseStream.ReadByte() << 24);
        }

        public override bool Eof
        {
            get { return (BaseStream.Length - BaseStream.Position) < 4 ? true : false; }
        }
    }

    class SignedByte64InputStream : BinaryInputStream
    {
        public SignedByte64InputStream(Stream baseStream, BinaryStreamElementType elementType)
            : base(baseStream, elementType)
        {
        }

        public override object ReadByteImpl()
        {
            return (Int64)BaseStream.ReadByte() | (Int64)(BaseStream.ReadByte() << 8) | ((Int64)BaseStream.ReadByte() << 16) | ((Int64)BaseStream.ReadByte() << 24)
                | ((Int64)BaseStream.ReadByte() << 32) | ((Int64)BaseStream.ReadByte() << 40) | ((Int64)BaseStream.ReadByte() << 48) | ((Int64)BaseStream.ReadByte() << 56);
        }

        public override bool Eof
        {
            get { return (BaseStream.Length - BaseStream.Position) < 8 ? true : false; }
        }
    }

    class GeneralSignedBinaryInputStream : BinaryInputStream
    {
        public GeneralSignedBinaryInputStream(Stream stream, BinaryStreamElementType elementType)
            : base(stream, elementType)
        {
        }

        // содержит номер последнего прочитанного байта
        int last_byte_num = -1;
        // содержит номер последнего прочитанного бита в последнем прочитанном байте
        int last_bit_num = 8;
        int shift = 0;
        byte last_byte;

        public override object ReadByteImpl()
        {
            /*                   int byte_count = CalculateBytesCount();
              int actually_readed;
              byte[] bytes = ReadBytes(byte_count, out actually_readed);

              if (byte_count > actually_readed)
              {
                  if (eofErrorP)
                  {
                      ConditionsDictionary.Error("oef on stream {0}", this);
                      throw new Exception("never reached");
                  }
                  else
                  {
                      return eofValue;
                  }
              }*/


            int bytes_count_to_read = CalculateBytesCountToRead();

            int actually_readed;
            byte[] bytes = ReadBytes(bytes_count_to_read, out actually_readed);

            int tail;

            bytes = Align(bytes, out tail);

            dynamic ret = (dynamic)TransformToSignedInteger(bytes);

            ret = ret >> shift;

            dynamic mask = CreateMask(elementType.Bits, 0);

            return ret & mask;
        }

        private int CalculateBytesCountToRead()
        {
            int bits_to_read = elementType.Bits - (8 - last_bit_num);
            int ret = bits_to_read / 8;
            if ((bits_to_read % 8) != 0)
                ret++;
            return ret;
        }

        /// <summary>
        /// start | 0 | 1 | 2 | ... | tail-byte (b0 b1 b2 b3  b4 b5 b6 b7 |
        ///         |            readed_bytes         (tail)          |
        /// </summary>
        /// <param name="readed_bytes"></param>
        /// <returns></returns>
        private byte[] Align(byte[] readed_bytes, out int tail)
        {

            if (last_byte_num == -1)
            {
                last_byte = readed_bytes[0];
                last_byte_num = 1;
            }
            else
            {
                this.shift = last_bit_num;
            }
            int shift = (8 - this.last_bit_num);
            int bits_to_read = elementType.Bits - shift;
            tail = bits_to_read % 8;
            int new_array_size = readed_bytes.Length;
            byte[] aligned;
            if (shift == 0)
            {
                aligned = new byte[new_array_size];
                Array.Copy(readed_bytes, aligned, readed_bytes.Length);
            }
            else
            {
                aligned = new byte[new_array_size + 1];
                aligned[0] = this.last_byte;

                Array.Copy(readed_bytes, 0, aligned, 1, readed_bytes.Length);
            }

            last_byte = readed_bytes[readed_bytes.Length - 1];
            last_bit_num = tail;

            return aligned;
        }

        private void AlignWithoutShftt(byte[] temp, byte[] aligned, int tail)
        {
            if (tail == 0)
            {
                Array.Copy(temp, aligned, temp.Length);
            }
        }

        private void AlignWithShift(byte[] temp, byte[] aligned, int shift, int tail)
        {
            int shift_left = MakeMaskForLastByte(shift);

            aligned[0] = (byte)((last_byte & shift_left) << last_byte_num);
        }

        private int MakeMaskForLastByte(int start)
        {
            int mask = 0;
            for (int i = 0; i < start; i++)
            {
                mask = mask | (1 << i);
            }

            return mask;
        }

        private int CalculateBytesCount()
        {
            int ret = elementType.Bits / 8;
            if ((elementType.Bits % 8) != 0)
                ret++;
            return ret;
        }

        private object CreateMask(int bits_count, int tail)
        {
            if (bits_count <= 32)
            {
                return CreateInt32Mask(bits_count, tail);
            }
            else if (bits_count <= 64)
            {
                return CreateInt64Mask(bits_count, tail);
            }
            else
            {
                return CreateBigIntegerMask(bits_count, tail);
            }
        }

        private Int32 CreateInt32Mask(int bits_count, int tail)
        {
            Int32 mask = 0;
            for (int i = 0; i < bits_count; i++)
            {
                mask = (mask | (Int32)(1 << i));
            }

            return mask;
        }

        private Int64 CreateInt64Mask(int bits_count, int tail)
        {
            Int64 mask = 0;
            for (int i = 0; i < bits_count; i++)
            {
                mask = (mask | ((Int64)1 << i));
            }

            return mask;
        }

        private BigInteger CreateBigIntegerMask(int bits_count, int tail)
        {
            throw new NotImplementedException();
        }

        private byte[] ReadBytes(int byte_count, out int readed)
        {
            byte[] bytes = new byte[byte_count];

            readed = BaseStream.Read(bytes, 0, byte_count);

            return bytes;
        }

        private object TransformToSignedInteger(byte[] bytes)
        {
            switch (bytes.Length)
            {
                case 1:
                    return bytes[0];
                case 2:
                    return ToInt16(bytes);
                case 3:
                case 4:
                    return ToInt32(bytes);
                case 5:
                case 6:
                case 7:
                case 8:
                    return ToInt64(bytes);
                default:
                    return ToSignedBigInteger(bytes);
            }
        }

        private object ToInt16(byte[] bytes)
        {
            // little endian
            return (Int16)(bytes[0] | (bytes[1] << 8));
        }

        private object ToInt32(byte[] bytes)
        {
            var ret = (Int32)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16));
            if (bytes.Length > 3)
                ret = ret | (Int32)(bytes[3] << 24);
            return ret;
        }

        private object ToInt64(byte[] bytes)
        {
            return (Int64)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24)
                | (bytes[3] << 32) | (bytes[3] << 40) | (bytes[3] << 48) | (bytes[3] << 56));
        }

        private object ToSignedBigInteger(byte[] bytes)
        {
            return BigInteger.Create(bytes);
        }

        public override bool Eof
        {
            get
            {
                if (CalculateBytesCountToRead() > (BaseStream.Length - BaseStream.Position))
                {
                    return true;
                }
                return false;
            }
        }
    }

    class UnsignedByte8InputStream : BinaryInputStream
    {
        public UnsignedByte8InputStream(Stream baseStream, BinaryStreamElementType elementType)
            :base(baseStream, elementType)
        {
        }

        public override object ReadByteImpl()
        {
            return BaseStream.ReadByte();
        }

        public override bool Eof
        {
            get { return BaseStream.Position == BaseStream.Length; }
        }
    }

    class UnsignedByte16InputStream : BinaryInputStream
    {
        public UnsignedByte16InputStream(Stream baseStream, BinaryStreamElementType elementType)
            : base(baseStream, elementType)
        {
        }

        public override object ReadByteImpl()
        {
            return (UInt16)BaseStream.ReadByte() | ((UInt16)BaseStream.ReadByte() << 8);
        }

        public override bool Eof
        {
            get { return (BaseStream.Length - BaseStream.Position) < 2?true:false; }
        }
    }

    class UnsignedByte32InputStream : BinaryInputStream
    {
        public UnsignedByte32InputStream(Stream baseStream, BinaryStreamElementType elementType)
            : base(baseStream, elementType)
        {
        }

        public override object ReadByteImpl()
        {
            UInt32 ret = (UInt32)BaseStream.ReadByte() | ((UInt32)BaseStream.ReadByte() << 8) | ((UInt32)BaseStream.ReadByte() << 16) | ((UInt32)BaseStream.ReadByte() << 24);

            if (ret <= Int32.MaxValue)
            {
                return (Int32)ret;
            }
            else
                return ret;
        }

        public override bool Eof
        {
            get { return (BaseStream.Length - BaseStream.Position) < 4 ? true : false; }
        }
    }

    class UnsignedByte64InputStream : BinaryInputStream
    {
        public UnsignedByte64InputStream(Stream baseStream, BinaryStreamElementType elementType)
            : base(baseStream, elementType)
        {
        }

        public override object ReadByteImpl()
        {
            UInt64 ret = (UInt64)BaseStream.ReadByte() | (UInt64)(BaseStream.ReadByte() << 8) | ((UInt64)BaseStream.ReadByte() << 16) | ((UInt64)BaseStream.ReadByte() << 24)
                | ((UInt64)BaseStream.ReadByte() << 32) | ((UInt64)BaseStream.ReadByte() << 40) | ((UInt64)BaseStream.ReadByte() << 48) | ((UInt64)BaseStream.ReadByte() << 56);

            if (ret <= Int64.MaxValue)
            {
                return (Int64)ret;
            }
            else
                return ret;
        }

        public override bool Eof
        {
            get { return (BaseStream.Length - BaseStream.Position) < 8 ? true : false; }
        }
    }

    class GeneralUnsignedBinaryInputStream : BinaryInputStream
    {
        public GeneralUnsignedBinaryInputStream(Stream stream, BinaryStreamElementType elementType)
            : base(stream, elementType)
        {
        }

        // содержит номер последнего прочитанного байта
        int last_byte_num = -1;
        // содержит номер последнего прочитанного бита в последнем прочитанном байте
        int last_bit_num = 8;
        int shift = 0;
        byte last_byte;

        public override object ReadByteImpl()
        {
            /*                   int byte_count = CalculateBytesCount();
              int actually_readed;
              byte[] bytes = ReadBytes(byte_count, out actually_readed);

              if (byte_count > actually_readed)
              {
                  if (eofErrorP)
                  {
                      ConditionsDictionary.Error("oef on stream {0}", this);
                      throw new Exception("never reached");
                  }
                  else
                  {
                      return eofValue;
                  }
              }*/


            int bytes_count_to_read = CalculateBytesCountToRead();

            int actually_readed;
            byte[] bytes = ReadBytes(bytes_count_to_read, out actually_readed);

            int tail;

            bytes = Align(bytes, out tail);

            dynamic ret = (dynamic)TransformToUnsignedInteger(bytes);

            ret = ret >> shift;

            dynamic mask = CreateMask(elementType.Bits, 0);

            return ret & mask;
        }

        private int CalculateBytesCountToRead()
        {
            int bits_to_read = elementType.Bits - (8 - last_bit_num);
            int ret = bits_to_read / 8;
            if ((bits_to_read % 8) != 0)
                ret++;
            return ret;
        }

        /// <summary>
        /// start | 0 | 1 | 2 | ... | tail-byte (b0 b1 b2 b3  b4 b5 b6 b7 |
        ///         |            readed_bytes         (tail)          |
        /// </summary>
        /// <param name="readed_bytes"></param>
        /// <returns></returns>
        private byte[] Align(byte[] readed_bytes, out int tail)
        {

            if (last_byte_num == -1)
            {
                last_byte = readed_bytes[0];
                last_byte_num = 1;
            }
            else
            {
                this.shift = last_bit_num;
            }
            int shift = (8 - this.last_bit_num);
            int bits_to_read = elementType.Bits - shift;
            tail = bits_to_read % 8;
            int new_array_size = readed_bytes.Length;
            byte[] aligned;
            if (shift == 0)
            {
                aligned = new byte[new_array_size];
                Array.Copy(readed_bytes, aligned, readed_bytes.Length);
            }
            else
            {
                aligned = new byte[new_array_size + 1];
                aligned[0] = this.last_byte;

                Array.Copy(readed_bytes, 0, aligned, 1, readed_bytes.Length);
            }

            last_byte = readed_bytes[readed_bytes.Length - 1];
            last_bit_num = tail;

            return aligned;
        }

        private void AlignWithoutShftt(byte[] temp, byte[] aligned, int tail)
        {
            if (tail == 0)
            {
                Array.Copy(temp, aligned, temp.Length);
            }


        }

        private void AlignWithShift(byte[] temp, byte[] aligned, int shift, int tail)
        {
            int shift_left = MakeMaskForLastByte(shift);

            aligned[0] = (byte)((last_byte & shift_left) << last_byte_num);
        }

        private int MakeMaskForLastByte(int start)
        {
            int mask = 0;
            for (int i = 0; i < start; i++)
            {
                mask = mask | (1 << i);
            }

            return mask;
        }

        private int CalculateBytesCount()
        {
            int ret = elementType.Bits / 8;
            if ((elementType.Bits % 8) != 0)
                ret++;
            return ret;
        }

        private object CreateMask(int bits_count, int tail)
        {
            if (bits_count <= 32)
            {
                return CreateInt32Mask(bits_count, tail);
            }
            else if (bits_count <= 64)
            {
                return CreateInt64Mask(bits_count, tail);
            }
            else
            {
                return CreateBigIntegerMask(bits_count, tail);
            }
        }

        private Int32 CreateInt32Mask(int bits_count, int tail)
        {
            Int32 mask = 0;
            for (int i = 0; i < bits_count; i++)
            {
                mask = (mask | (1 << i));
            }

            return mask;
        }

        private Int64 CreateInt64Mask(int bits_count, int tail)
        {
            Int64 mask = 0;
            for (int i = 0; i < bits_count; i++)
            {
                mask = (mask | ((Int64)1 << i));
            }

            return mask;
        }

        private BigInteger CreateBigIntegerMask(int bits_count, int tail)
        {
            throw new NotImplementedException();
        }

        private byte[] ReadBytes(int byte_count, out int readed)
        {
            byte[] bytes = new byte[byte_count];

            readed = BaseStream.Read(bytes, 0, byte_count);

            return bytes;
        }

        private object TransformToUnsignedInteger(byte[] bytes)
        {
            switch (bytes.Length)
            {
                case 1:
                    return bytes[0];
                case 2:
                    return ToUInt16(bytes);
                case 3:
                case 4:
                    return ToUInt32(bytes);
                case 5:
                case 6:
                case 7:
                case 8:
                    return ToUInt64(bytes);
                default:
                    return ToUnsignedBigInteger(bytes);
            }
        }

        private object ToUInt16(byte[] bytes)
        {
            // little endian
            UInt16 ret = (UInt16)(bytes[0] | ((UInt16)bytes[1] << 8));
            if (ret <= Int16.MaxValue)
            {
                return (Int16)ret;
            }
            else
                return ret;
        }

        private object ToUInt32(byte[] bytes)
        {
            var ret = (UInt32)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16));
            if (bytes.Length > 3)
                ret = ret | (UInt32)(bytes[3] << 24);

            if (ret <= Int32.MaxValue)
                return (int)ret;
            else
                return ret;
        }

        private object ToUInt64(byte[] bytes)
        {
            UInt64 ret = (UInt64)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24)
                | (bytes[3] << 32) | (bytes[3] << 40) | (bytes[3] << 48) | (bytes[3] << 56));

            if (ret <= Int64.MaxValue)
            {
                return (Int64)ret;
            }
            else
            {
                return ret;
            }
        }

        private object ToUnsignedBigInteger(byte[] bytes)
        {
            return BigInteger.CreateUnsigned(bytes);
        }

        public override bool Eof
        {
            get
            {
                if (CalculateBytesCountToRead() > (BaseStream.Length - BaseStream.Position))
                {
                    return true;
                }
                return false;
            }
        }
    }

    public class TypedBinaryInputStream<T> : IBinaryInputStream
    {

        private IBinaryInputStream actualStream;

        internal TypedBinaryInputStream(IBinaryInputStream actualStream)
        {
            this.actualStream = actualStream;
        }

        public BinaryStreamElementType ElementType
        {
            get { return actualStream.ElementType; }
        }

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            return actualStream.ReadByte(eofErrorP, eofValue);
        }

        public T ReadByte(bool eofErrorP, T eofValue)
        {
            return (T)actualStream.ReadByte(eofErrorP, eofValue);
        }

        public object ReadByte()
        {
            return actualStream.ReadByte();
        }

        public T Read()
        {
            return (T)actualStream.ReadByte();
        }

        public Stream BaseStream
        {
            get { return actualStream.BaseStream; }
        }

        public void Clear()
        {
            actualStream.Clear();
        }

        public bool Listen
        {
            get { return actualStream.Listen; }
        }

        public bool Eof
        {
            get { return actualStream.Eof; }
        }

        public void Close()
        {
            actualStream.Close();
        }

        public bool OpenStreamP
        {
            get { return actualStream.OpenStreamP; }
        }

        public void Dispose()
        {
            actualStream.Dispose();
        }
    }

 
    public static class BinaryStreamsfactory
    {
        public static IBinaryInputStream CreateInputStream(Stream baseStream, BinaryStreamElementType elementType)
        {
            if (elementType.Signed)
            {
                return CreateSignedInputStream(baseStream, elementType);
            }
            else
            {
                return CreateUnsignedInputStream(baseStream, elementType);
            }
        }

        private static IBinaryInputStream CreateSignedInputStream(Stream baseStream, int bitsCount)
        {
            if (bitsCount <= 0)
            {
                throw new ArgumentOutOfRangeException("bitsCount");
            }
            var elementType = new BinaryStreamElementType(true, bitsCount);

            return CreateSignedInputStream(baseStream, elementType);
        }

        private static IBinaryInputStream CreateSignedInputStream(Stream baseStream, BinaryStreamElementType elementType)
        {
            switch (elementType.Bits)
            {
                case 8:
                    return new SignedByte8InputStream(baseStream, elementType);
                case 16:
                    return new SignedByte16InputStream(baseStream, elementType);
                case 32:
                    return new SignedByte32InputStream(baseStream, elementType);
                case 64:
                    return new SignedByte64InputStream(baseStream, elementType);
                default:
                    return new GeneralSignedBinaryInputStream(baseStream, elementType);
            }
        }

        private static IBinaryInputStream CreateUnsignedInputStream(Stream baseStream, int bitsCount)
        {
            if (bitsCount <= 0)
            {
                throw new ArgumentOutOfRangeException("bitsCount");
            }

            var elementType = new BinaryStreamElementType(false, bitsCount);

            return CreateUnsignedInputStream(baseStream, elementType);
        }

        private static IBinaryInputStream CreateUnsignedInputStream(Stream baseStream, BinaryStreamElementType elementType)
        {
            switch (elementType.Bits)
            {
                case 8:
                    return new UnsignedByte8InputStream(baseStream, elementType);
                case 16:
                    return new UnsignedByte16InputStream(baseStream, elementType);
                case 32:
                    return new UnsignedByte32InputStream(baseStream, elementType);
                case 64:
                    return new UnsignedByte64InputStream(baseStream, elementType);
                default:
                    return new GeneralUnsignedBinaryInputStream(baseStream, elementType);
            }
        }

        public static TypedBinaryInputStream<T> CreateInputStream<T>(Stream baseStream, int bitsCount)
        {
            if(bitsCount <= 0)
            {
                throw new ArgumentOutOfRangeException("bitsCount");
            }

            IBinaryInputStream actualStream = null;
            bool signed = false;

            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    if (bitsCount > 1)
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                    signed = false;
                    break;
                case TypeCode.Byte:
                    if (bitsCount > 8)
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                     signed = false;
                    break;
                case TypeCode.Char:
                case TypeCode.DBNull:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Empty:
                    throw new ArgumentException("Invalid Type " + typeof(Char));
                case TypeCode.Int16:
                     if (bitsCount > 16)
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                     signed = true;
                    break;
                case TypeCode.Int32:
                     if (bitsCount > 32)
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                     signed = true;
                    break;
                case TypeCode.Int64:
                     if (bitsCount > 64)
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                     signed = true;
                    break;
                case TypeCode.Object:
                    throw new ArgumentException("Invalid Type " + typeof(Char));
                case TypeCode.SByte:
                    if (bitsCount > 8)
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                    signed = true;
                    break;
                case TypeCode.Single:
                case TypeCode.String:
                    throw new ArgumentException("Invalid Type " + typeof(Char));
                case TypeCode.UInt16:
                    if (bitsCount > 16)
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                    signed = false;
                    break;
                case TypeCode.UInt32:
                    if (bitsCount > 32)
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                    signed = false;
                    break;
                case TypeCode.UInt64:
                    if (bitsCount > 64)
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                    signed = false;
                    break;
                default:
                    if (typeof(T) == typeof(BigInteger))
                    {
                        signed = true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                    break;
            }

            actualStream = CreateUnsignedInputStream(baseStream, new BinaryStreamElementType(signed, bitsCount));
            return new TypedBinaryInputStream<T>(actualStream);
        }

        public static TypedBinaryInputStream<T> CreateInputStream<T>(Stream baseStream)
        {

            IBinaryInputStream actualStream = null;
            bool signed = false;
            int bitsCount = 8;
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    bitsCount = 1;
                    signed = false;
                    break;
                case TypeCode.Byte:
                    signed = false;
                    break;
                case TypeCode.Char:
                case TypeCode.DBNull:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Empty:
                    throw new ArgumentException("Invalid Type " + typeof(Char));
                case TypeCode.Int16:
                    bitsCount = 16;
                    signed = true;
                    break;
                case TypeCode.Int32:
                    bitsCount = 32;
                    signed = true;
                    break;
                case TypeCode.Int64:
                    bitsCount = 64;
                    signed = true;
                    break;
                case TypeCode.Object:
                    throw new ArgumentException("Invalid Type " + typeof(Char));
                case TypeCode.SByte:
                    signed = true;
                    break;
                case TypeCode.Single:
                case TypeCode.String:
                    throw new ArgumentException("Invalid Type " + typeof(Char));
                case TypeCode.UInt16:
                    bitsCount = 16;
                    signed = false;
                    break;
                case TypeCode.UInt32:
                    bitsCount = 32;
                    signed = false;
                    break;
                case TypeCode.UInt64:
                    bitsCount = 64;
                    signed = false;
                    break;
                default:
                    if (typeof(T) == typeof(BigInteger))
                    {
                        signed = true;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("bitsCount");
                    }
                    break;
            }

            actualStream = CreateUnsignedInputStream(baseStream, new BinaryStreamElementType(signed, bitsCount));
            return new TypedBinaryInputStream<T>(actualStream);
        }
    }
}
