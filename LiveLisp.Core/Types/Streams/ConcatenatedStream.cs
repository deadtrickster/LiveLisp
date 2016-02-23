using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.Types.Streams
{
    class ConcatenatedStream : IBinaryInputStream, ICharacterInputStream
    {
        public ConcatenatedStream(IEnumerable<IInputStream> streams)
        {
            this.streams = new Queue<IInputStream>(streams);
            if (this.streams.Count > 0)
            {
                currentStream = this.streams.Dequeue();
            }
        }

        Queue<IInputStream> streams;
        IInputStream currentStream;

        public IInputStream CurrentStream
        {
            get
            {
                if (CurrentStream == null)
                {
                    if (streams.Count > 0)
                        currentStream = streams.Dequeue();
                    else
                        return null;
                }

                if (CurrentStream.Eof)
                {
                    currentStream = null;
                    return CurrentStream;
                }

                return CurrentStream;
            }
        }

        public bool Eof
        {
            get
            {
                if ((CurrentStream == null || CurrentStream.Eof) && streams.Count == 0)
                    return true;
                return false;
            }
        }

        public object OnEof(bool eofErrorP, object eofValue)
        {
            if (eofErrorP)
            {
                ConditionsDictionary.Error("Concatenated-stream eof");
            }

            return eofValue;
        }

        void CheckIsClosed()
        {

        }

        public object ReadByte(bool eofErrorP, object eofValue)
        {
            CheckIsClosed();
            if(Eof)
            {
                return OnEof(eofErrorP, eofValue);
            }

            IBinaryInputStream stream = CurrentStream as IBinaryInputStream;

            if (stream == null)
            {
                ConditionsDictionary.TypeError("Not a binary-input-stream " + CurrentStream);
            }

            return stream.ReadByte(eofErrorP, eofValue);
        }



        public BinaryStreamElementType ElementType
        {
            get
            {
                CheckIsClosed();

                IBinaryInputStream stream = CurrentStream as IBinaryInputStream;

                if (stream == null)
                {
                    ConditionsDictionary.TypeError("Not a binary-input-stream " + CurrentStream);
                }

                return stream.ElementType;
            }
        }

        public object ReadByte()
        {
            CheckIsClosed();
            return (byte)ReadByte(false, -1);
        }

        public void Clear()
        {
            CheckIsClosed();
            CurrentStream.Clear();
        }

        public bool Listen
        {
            get {
                CheckIsClosed();
                return CurrentStream.Listen; }
        }

        bool closed = false;
        public void Close()
        {
            lock (this)
            {
                /*CurrentStream.Close();
                while (streams.Count != 0)
                {
                    streams.Dequeue().Close();
                }
                streams.Clear();
                currentStream = null;*/
                closed = true;
            }
        }

        public bool OpenStreamP
        {
            get { return  !closed; }
        }

        public int CurrentLine
        {
            get
            {
                CheckIsClosed();
                ICharacterInputStream stream = CurrentStream as ICharacterInputStream;
                if (stream == null){
                    ConditionsDictionary.TypeError("Not a character stream");
                }
                return stream.CurrentLine;
            }
        }

        public int RawIndex
        {
            get
            {
                CheckIsClosed();
                ICharacterInputStream stream = CurrentStream as ICharacterInputStream;
                if (stream == null)
                {
                    ConditionsDictionary.TypeError("Not a character stream");
                }
                return stream.RawIndex;
            }
        }

        public int CurrentColumn
        {
            get
            {
                CheckIsClosed();
                ICharacterInputStream stream = CurrentStream as ICharacterInputStream;
                if (stream == null)
                {
                    ConditionsDictionary.TypeError("Not a character stream");
                }
                return stream.CurrentColumn;
            }
        }

        public string DocId
        {
            get
            {
                CheckIsClosed();
                ICharacterInputStream stream = CurrentStream as ICharacterInputStream;
                if (stream == null)
                {
                    ConditionsDictionary.TypeError("Not a character stream");
                }
                return stream.DocId;
            }
            set
            {
                CheckIsClosed();
                ICharacterInputStream stream = CurrentStream as ICharacterInputStream;
                if (stream == null)
                {
                    ConditionsDictionary.TypeError("Not a character stream");
                }
                stream.DocId = value;
            }
        }

        public void UnreadChar(char ch)
        {
            CheckIsClosed();
            ICharacterInputStream stream = CurrentStream as ICharacterInputStream;
            if (stream == null)
            {
                ConditionsDictionary.TypeError("Not a character stream");
            }
            stream.UnreadChar(ch);
        }

        public object Read(bool eofErrorP, object eofValue)
        {
            CheckIsClosed();
            ICharacterInputStream stream = CurrentStream as ICharacterInputStream;
            if (stream == null)
            {
                ConditionsDictionary.TypeError("Not a character stream");
            }
            return stream.Read(eofErrorP, eofValue);
        }

        public int Read()
        {
            CheckIsClosed();
            ICharacterInputStream stream = CurrentStream as ICharacterInputStream;
            if (stream == null)
            {
                ConditionsDictionary.TypeError("Not a character stream");
            }
            return stream.Read();
        }

        public int Peek()
        {
            CheckIsClosed();
            ICharacterInputStream stream = CurrentStream as ICharacterInputStream;
            if (stream == null)
            {
                ConditionsDictionary.TypeError("Not a character stream");
            }
            return stream.Peek();
        }

        public void Dispose()
        {
            Close();
        }


        public System.IO.Stream BaseStream
        {
            get
            {
                IBinaryInputStream bs = currentStream as IBinaryInputStream;
                if (bs == null)
                    ConditionsDictionary.TypeError(currentStream + " is not a binary input stream");
                return bs.BaseStream;
            }
        }
    }
}
