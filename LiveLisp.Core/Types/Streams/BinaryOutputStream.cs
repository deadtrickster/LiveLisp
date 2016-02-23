using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.Types.Streams
{
    public class BinaryOutputStream : IBinaryOutputStream, IDisposable
    {

        BinaryStreamElementType elementType;
        Stream stream;

        public Stream Stream
        {
          get { return stream; }
          set { stream = value; }
        }

        public BinaryOutputStream(Stream stream, BinaryStreamElementType elementType)
        {
            this.stream = stream;
            this.elementType = elementType;
            ComputeMask();
        }

        private void ComputeMask()
        {
            int mask = 0;
            for (int i = 0; i < elementType.Bits; i++)
            {
                mask = mask | (1 << i);
            }

            _mask = mask;
        }

        public BinaryStreamElementType ElementType
        {
            get { return elementType; }
        }

        byte last_byte;
        int last_bit_in_byte = 8;

        public object WriteByte(object bte)
        {
            GetBytesToWrite(bte);

            return bte;
        }

        private void GetBytesToWrite(object bte)
        {
            /*Type t = bte.GetType();
                        switch (Type.GetTypeCode(bte.GetType()))
                        {
                            case TypeCode.Boolean:
                            case TypeCode.Byte:
                            case TypeCode.Char:
                                return GetBytesFromUInt32((UInt32)(dynamic)bte);
                            case TypeCode.DBNull:
                            case TypeCode.DateTime:
                            case TypeCode.Decimal:
                            case TypeCode.Double:
                            case TypeCode.Empty:
                                ConditionsDictionary.TypeError(bte + " is not a byte nor unsigend byte");
                                throw new Exception("never reached");
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                                return GetBytesFromUInt32((UInt32)(dynamic)bte);
                            case TypeCode.Int64: 
                                return GetBytesFromUInt64((UInt32)(dynamic)bte);
                            case TypeCode.Object:
                                ConditionsDictionary.TypeError(bte + " is not a byte nor unsigend byte");
                                throw new Exception("never reached");
                            case TypeCode.SByte:
                                return GetBytesFromUInt32((UInt32)(dynamic)bte);
                            case TypeCode.Single:
                            case TypeCode.String:
                                ConditionsDictionary.TypeError(bte + " is not a byte nor unsigend byte");
                                throw new Exception("never reached");
                            case TypeCode.UInt16:
                            case TypeCode.UInt32:
                                return GetBytesFromUInt32((UInt32)(dynamic)bte);
                            case TypeCode.UInt64:
                                return GetBytesFromUInt64((UInt32)(dynamic)bte);
                            default:
                                if (bte is BigInteger)
                                {
                                    return GetBytesFromBigInteger((BigInteger)bte);
                                }
                                break;
                        }*/

            WriteBytesFrom((dynamic)bte);

        }
        private void WriteBytesFrom(int ui)
        {
            long rem = (ui & _mask);

            if ((ui & ~_mask) != 0)
            {
                ConditionsDictionary.TypeError(ui + " is not a type " + elementType);
            }

            int tail = 8 - last_bit_in_byte;

            int tail_mask = CreateTailMask(tail);

            uint tail_value = (uint)(rem & tail_mask);

            if (tail != 0)
            {
                FillAndWriteLastByte(tail_value, tail);
            }

            uint body = (uint)(rem & ~tail_mask) >> tail;
            int body_bytes = (elementType.Bits - tail) / 8;
            int ad_bits = (elementType.Bits - tail) % 8;
            if (ad_bits != 0)
            {
                last_byte = (byte)((body & ~(CreateTailMask((body_bytes-1) * 8 + 8))) >> (body_bytes * 8));
                last_bit_in_byte = ad_bits;
            }
            else
            {
                last_bit_in_byte = 8;
            }


            for (int i = 0; i < body_bytes; i++)
            {
                stream.WriteByte((byte)((body & (CreateTailMask(i * 8 + 8))) >> (i * 8)));
            }
        }

        private void WriteBytesFrom(UInt32 ui)
        {
            long rem = (ui & _mask);

            if ((ui & ~_mask) != 0)
            {
                ConditionsDictionary.TypeError(ui + " is not a type " + elementType);
            }

            int tail = 8 - last_bit_in_byte;

            int tail_mask = CreateTailMask(tail);

            uint tail_value = (uint)(rem & tail_mask);

            if (tail != 0)
            {
                FillAndWriteLastByte(tail_value, tail);
            }

            uint body = (uint)(rem & ~tail_mask) >> tail;
            int body_bytes = (elementType.Bits - tail) / 8;
            int ad_bits = (elementType.Bits - tail) % 8;
            if (ad_bits != 0)
            {
                last_byte = (byte)((body_bytes & ~(CreateTailMask(body_bytes * 8 + 8))) >> body_bytes * 8);
                last_bit_in_byte = ad_bits;
            }
            else
            {
                last_bit_in_byte = 8;
            }


            for (int i = 0; i < body_bytes; i++)
            {
                stream.WriteByte((byte)((i & ~(CreateTailMask(i * 8 + 8))) >> i * 8));
            }
        }

        private int CreateTailMask(int tail)
        {
            int mask = 0;
            for (int i = 0; i < tail; i++)
            {
                mask = mask | (1 << i);
            }

            return mask;
        }

        private void FillAndWriteLastByte(uint tail_value, int tail)
        {
            last_byte = (byte)(last_byte | (tail_value << (8 - tail)));
            stream.WriteByte(last_byte);
        }

        private byte[] GetBytesFrom(UInt64 ui)
        {
            throw new NotImplementedException();
        }

        private byte[] GetBytesFrom(BigInteger bigInteger)
        {
            throw new NotImplementedException();
        }

        int _mask;

        public object FinishOutput()
        {
            stream.Flush();
            return DefinedSymbols.NIL;
        }

        public object ForceOutput()
        {
            stream.Flush();
            return DefinedSymbols.NIL;
        }

        public object ClearOutput()
        {
            return DefinedSymbols.NIL;
        }

        bool closed;

        void IsCLosedCheck()
        {
        
        }

        public void Close()
        {
            closed = true;
            stream.WriteByte(last_byte);
            stream.Flush();
            stream.Close();
            stream.Dispose();
        }

        public bool OpenStreamP
        {
            get { return !closed; }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
