using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.Types.Streams
{
    class SignedByte8InputStream: IBinaryInputStream
    {

            public BinaryStreamElementType  ElementType
        {
        	get { throw new NotImplementedException(); }
        }

        public object  ReadByte(bool eofErrorP, object eofValue)
        {
         	throw new NotImplementedException();
        }

        public byte  ReadByte()
        {
         	throw new NotImplementedException();
        }

        public void  Clear()
        {
         	throw new NotImplementedException();
        }

        public bool  Listen
        {
        	get { throw new NotImplementedException(); }
        }

        public bool  Eof
        {
        	get { throw new NotImplementedException(); }
        }

        public void  Close()
        {
         	throw new NotImplementedException();
        }

        public bool  OpenStreamP
        {
        	get { throw new NotImplementedException(); }
        }
}

    class SignedByte16InputStream : IBinaryInputStream
    {

        public BinaryStreamElementType ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Listen
        {
            get { throw new NotImplementedException(); }
        }

        public bool Eof
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool OpenStreamP
        {
            get { throw new NotImplementedException(); }
        }
    }

    class SignedByte32InputStream : IBinaryInputStream
    {

        public BinaryStreamElementType ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Listen
        {
            get { throw new NotImplementedException(); }
        }

        public bool Eof
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool OpenStreamP
        {
            get { throw new NotImplementedException(); }
        }
    }

    class SignedByte64InputStream : IBinaryInputStream
    {

        public BinaryStreamElementType ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Listen
        {
            get { throw new NotImplementedException(); }
        }

        public bool Eof
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool OpenStreamP
        {
            get { throw new NotImplementedException(); }
        }
    }

    class GeneralSignedBinaryInputStream : IBinaryInputStream
    {
        public GeneralSignedBinaryInputStream(Stream stream, BinaryStreamElementType elementType)
            :base(stream, elementType)
        {
        }

        void CheckIsClosed()
        {

        }

        // содержит номер последнего прочитанного байта
        int last_byte_num = -1;
        // содержит номер последнего прочитанного бита в последнем прочитанном байте
        int last_bit_num = 8;
        int shift = 0;
        byte last_byte;

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            CheckIsClosed();
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

            if (bytes_count_to_read > actually_readed)
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
            }

            int tail;

            bytes = Align(bytes, out tail);

            dynamic ret = (dynamic)TransformToInteger(bytes);

            ret = ret >> shift;

            dynamic mask = CreateMask(elementType.Bits, 0);

            /*00001001 01000110*/
            /*00000111 11111111*/
            /*2047*/
            /*00000001 01000110*/

            /*01001100  01001001*/
            //00001001 10001001
            /*00000001 10001001*/

            //      3               2               1           порядок в котором были считаны байты               
            //01010011 01000101 01001100
            //00000001 01001101 00010101   001100  это кусок предыдущего element-size byte'a  
            //00000101   00010101

            /*
             * т.к. мы считывем little endian то нужен только один сдвиг
             */

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

            /*if ((tail + shift) > 8)
            {
                new_array_size++;
            }


            byte[] temp;


            if (tail != 0)
            {
                temp = new byte[readed_bytes.Length + 1];
                Array.Copy(readed_bytes, temp, readed_bytes.Length);
                AlignWithoutShftt(temp, aligned, tail);
            }
            else
            {
                temp = new byte[readed_bytes.Length];
                temp[0] = this.last_byte;
                Array.Copy(readed_bytes, 0, temp, 1, readed_bytes.Length);
                AlignWithShift(temp, aligned, shift, tail);
            }



            this.last_byte = last_byte;
            this.last_bit_num = last_bit_num;*/

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
            //int working_set = bits_count + tail;

            if (bits_count <= 32)
            {
                return CreateUInt32Mask(bits_count, tail);
            }
            else if (bits_count <= 64)
            {
                return CreateUInt64Mask(bits_count, tail);
            }
            else
            {
                return CreateBigIntegerMask(bits_count, tail);
            }
        }

        private UInt32 CreateUInt32Mask(int bits_count, int tail)
        {
            UInt32 mask = 0;
            for (int i = 0; i < bits_count; i++)
            {
                mask = (mask | (UInt32)(1 << i));
            }

            return mask;
        }

        private UInt64 CreateUInt64Mask(int bits_count, int tail)
        {
            /* Int64 mask = 0;
             for (int i = 0; i < bits_count; i++)
             {
                 mask = mask | (1 << i);
             }

             return (UInt64)mask;*/

            throw new NotImplementedException();
        }

        private BigInteger CreateBigIntegerMask(int bits_count, int tail)
        {
            throw new NotImplementedException();
        }

        private byte[] ReadBytes(int byte_count, out int readed)
        {
            byte[] bytes = new byte[byte_count];

            readed = stream.Read(bytes, 0, byte_count);

            return bytes;
        }

        private object TransformToInteger(byte[] bytes)
        {
            if (elementType.Signed)
            {
                return TransformToSignedInteger(bytes);
            }
            else
            {
                return TransformToUnsignedInteger(bytes);
            }
        }

        private object TransformToSignedInteger(byte[] bytes)
        {
            switch (bytes.Length)
            {
                case 1:
                    byte b = bytes[0];
                    return (sbyte)(b - ((b & 0x80) << 1));
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
                    return ToBigInteger(bytes);
            }
        }

        private object ToInt16(byte[] bytes)
        {
            return (Int16)(bytes[0] | (bytes[1] << 8));
        }

        private object ToInt32(byte[] bytes)
        {
            return (Int32)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24));
        }

        private object ToInt64(byte[] bytes)
        {
            return (Int64)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24)
                | (bytes[3] << 32) | (bytes[3] << 40) | (bytes[3] << 48) | (bytes[3] << 56));
        }

        private object ToBigInteger(byte[] bytes)
        {
            return BigInteger.Create(bytes);
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
            return (UInt16)(bytes[0] | (bytes[1] << 8));
        }

        private object ToUInt32(byte[] bytes)
        {
            var ret = (UInt32)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16));
            if (bytes.Length > 3)
                ret = ret | (UInt32)(bytes[3] << 24);
            return ret;
        }

        private object ToUInt64(byte[] bytes)
        {
            return (UInt64)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16) | (bytes[3] << 24)
                | (bytes[3] << 32) | (bytes[3] << 40) | (bytes[3] << 48) | (bytes[3] << 56));
        }

        private object ToUnsignedBigInteger(byte[] bytes)
        {
            return BigInteger.CreateUnsigned(bytes);
        }

        public byte ReadByte()
        {
            CheckIsClosed();
            int b = stream.ReadByte();
            if (b != -1)
                return (byte)b;
            else
            {
                ConditionsDictionary.Error("eof on stream " + this);
                throw new Exception("never reached");
            }
        }

        public void Clear()
        {
            CheckIsClosed();
        }

        public bool Listen
        {
            get { CheckIsClosed(); return false; }
        }

        public bool Eof
        {
            get { CheckIsClosed(); return stream.Length == stream.Position; }
        }

        bool closed = false;
        public void Close()
        {
            closed = true;
            stream.Close();
        }

        public bool OpenStreamP
        {
            get { return !closed; }
        }

        BinaryStreamElementType elementType;

        public BinaryStreamElementType ElementType
        {
            get { CheckIsClosed(); return elementType; }
        }


        #region Stream Implementation
        /*
        public override bool CanRead
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
*/
        #endregion
    }

    class UnsignedByte8InputStream : IBinaryInputStream
    {

        public BinaryStreamElementType ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Listen
        {
            get { throw new NotImplementedException(); }
        }

        public bool Eof
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool OpenStreamP
        {
            get { throw new NotImplementedException(); }
        }
    }

    class UnsignedByte16InputStream : IBinaryInputStream
    {

        public BinaryStreamElementType ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Listen
        {
            get { throw new NotImplementedException(); }
        }

        public bool Eof
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool OpenStreamP
        {
            get { throw new NotImplementedException(); }
        }
    }

    class UnsignedByte32InputStream : IBinaryInputStream
    {

        public BinaryStreamElementType ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Listen
        {
            get { throw new NotImplementedException(); }
        }

        public bool Eof
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool OpenStreamP
        {
            get { throw new NotImplementedException(); }
        }
    }

    class UnsignedByte64InputStream : IBinaryInputStream
    {

        public BinaryStreamElementType ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Listen
        {
            get { throw new NotImplementedException(); }
        }

        public bool Eof
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool OpenStreamP
        {
            get { throw new NotImplementedException(); }
        }
    }

    class GeneralUnsignedBinaryInputStream : IBinaryInputStream
    {
        public BinaryStreamElementType ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            throw new NotImplementedException();
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Listen
        {
            get { throw new NotImplementedException(); }
        }

        public bool Eof
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool OpenStreamP
        {
            get { throw new NotImplementedException(); }
        }
    }
 
    public static class BinaryStreamsfactory
    {
        public static IBinaryInputStream CreateInputStream(BinaryStreamElementType elementType)
        {
            if (elementType.Signed)
            {
                return CreateSignedInputStream(elementType);
            }
            else
            {
                return CreateUnsignedInputStream(elementType);
            }
        }

        private static IBinaryInputStream CreateSignedInputStream(BinaryStreamElementType elementType)
        {
         	throw new NotImplementedException();
        }

        private static IBinaryInputStream CreateUnsignedInputStream(BinaryStreamElementType elementType)
        {
         	throw new NotImplementedException();
        }
    }
}
