using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.Types.Streams
{
    public class BidirectionalStream : IBidirectionalStream
    {
        ICharacterInputStream inputStream;
        ICharacterOutputStream outputStream;

        #region ICharacterInputStream Members

        public int CurrentLine
        {
            get { return inputStream.CurrentLine; }
        }

        public int RawIndex
        {
            get { return inputStream.RawIndex; }
        }

        public int CurrentColumn
        {
            get { return inputStream.CurrentColumn; }
        }

        public string DocId
        {
            get
            {
                return inputStream.DocId;
            }
            set
            {
                inputStream.DocId = value;
            }
        }

        public void UnreadChar(char ch)
        {
            inputStream.UnreadChar(ch);
        }

        public int Read()
        {
            return inputStream.Read();
        }

        public object Read(bool eofErrorP, object eofValue)
        {
            return inputStream.Read(eofErrorP, eofValue);
        }

        public int Peek()
        {
            return inputStream.Peek();
        }

        #endregion

        #region IInputStream Members

        public void Clear()
        {
            inputStream.Clear();
        }

        public bool Listen
        {
            get { return inputStream.Listen; }
        }

        public bool Eof
        {
            get { return inputStream.Eof; }
        }

        #endregion

        #region ILispStream Members

        public void Close()
        {
           inputStream.Close();
        }

        public bool OpenStreamP
        {
            get { return inputStream.OpenStreamP; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            inputStream.Dispose();
            outputStream.Dispose();
        }

        #endregion

        #region ICharacterOutputStream Members

        public void Write(char ch)
        {
            outputStream
                .Write(ch);
        }

        public void Write(string str)
        {
            outputStream.Write(str);
        }

        public void WriteLine(string str)
        {
            outputStream.WriteLine(str);
        }

        public Encoding Encoding
        {
            get { return outputStream.Encoding; }
        }

        #endregion

        #region IOutputStream Members

        public object FinishOutput()
        {
            return outputStream.FinishOutput();
        }

        public object ForceOutput()
        {
            return outputStream.ForceOutput();
        }

        public object ClearOutput()
        {
            return outputStream.ClearOutput();
        }

        #endregion

        public BidirectionalStream(ICharacterInputStream inputStream, ICharacterOutputStream outputStream)
        {
            this.inputStream = inputStream;
            this.outputStream = outputStream;
        }
    }
}
