using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Compiler;
using System.IO;

namespace LiveLisp.Core.Types.Streams
{
    public interface ILispStream : IDisposable
    {
        void Close();
        //[LispBool]
        bool OpenStreamP
        {
            get;
        }
    }

    public interface IInputStream : ILispStream
    {
        void Clear();

        bool Listen
        {
            get;
        }

        bool Eof
        {
            get;
        }
    }

    public interface IOutputStream : ILispStream
    {
        //finish-output &optional output-stream => nil
        object FinishOutput();

        //force-output &optional output-stream => nil
        object ForceOutput();

        //clear-output &optional output-stream => nil
        object ClearOutput();

    }

    public interface ICharacterInputStream : IInputStream, IDisposable
    {
         int CurrentLine
        {
            get;
        }

         int RawIndex
        {
            get;
        }

         int CurrentColumn
        {
            get;
        }
         string DocId
        {
            get;
            set;
        }

         void UnreadChar(char ch);

         object Read(bool eofErrorP, object eofValue);

         int Read();

         int Peek();
    }

    public interface ICharacterOutputStream : IOutputStream, IDisposable
    {
        void Write(char ch);
        void Write(string str);
        void WriteLine(string str);

         Encoding Encoding
        {
            get;
        }
    }

    public class BinaryStreamElementType
    {
        public bool Signed = true;
        public int Bits = 8;


        public BinaryStreamElementType()
        {

        }

        public BinaryStreamElementType(bool signed, int bitsCount)
        {
            // TODO: Complete member initialization
            Signed = signed;
            Bits = bitsCount;
        }
    }

    public interface IBinaryInputStream : IInputStream
    {
        BinaryStreamElementType ElementType
        {
            get;
        }
        //read-byte stream &optional eof-error-p eof-value
        object ReadByte(bool eofErrorP, object eofValue);
        object ReadByte();

        Stream BaseStream
        {
            get;
        }
    }

    public interface IBinaryOutputStream : IOutputStream
    {
        BinaryStreamElementType ElementType
        {
            get;
        }
        object WriteByte(object bte);
        
    }

    public interface IBidirectionalStream : ICharacterInputStream, ICharacterOutputStream
    {

    }
    ///////////////////////////////////////////////////////////////////////
/*
    public interface IStream<T>
    {
        void Close();

        bool Opened
        {
            get;
        }
    }

    public interface IInputStream<T> : IStream<T>
    {
        T Peek();
        T Read();
        void Unread(T item);
    }

    public class CharacterStream : TextReader, IInputStream<int>
    {
        #region IInputStream<char> Members

        public int Peek()
        {
            throw new NotImplementedException();
        }

        public int Read()
        {
            throw new NotImplementedException();
        }

        public void Unread(char item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IStream<char> Members


        public bool Opened
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class StreamFromCons : IInputStream<object>
    {
        #region IInputStream<object> Members

        public object Peek()
        {
            throw new NotImplementedException();
        }

        public object Read()
        {
            throw new NotImplementedException();
        }

        public void Unread(object item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IStream<object> Members

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool Opened
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
    */

}