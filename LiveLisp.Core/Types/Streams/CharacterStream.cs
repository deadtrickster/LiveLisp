using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types.Streams;
using System.IO;
using LiveLisp.Core.BuiltIns.Conditions;

namespace LiveLisp.Core.Types.Streams
{
    public class CharacterInputStream : TextReader, ICharacterInputStream
    {
        /// <summary>
        /// Gets or sets the Real used TextReader
        /// </summary>
        /// <value>The Real TextReader</value>
#if DEBUG
        public TextReader Real
        {
            get;
            set;
        }
#else
        TextReader Real
        {
            get;
            set;
        }
#endif

        Int32 _CurrentLine = 1;

        /// <summary>
        /// Gets the line number of last character
        /// </summary>
        /// <value>The current line.</value>
        public int CurrentLine
        {
            get { return _CurrentLine; }
        }

        Int32 _CurrentIndex;


        /// <summary>
        /// Gets the index of the last character.
        /// </summary>
        /// <value>The index of the current character.</value>
        public int RawIndex
        {
            get { return _CurrentIndex; }
        }

        Int32 _CurrentColumn;

        /// <summary>
        /// Gets the column number of the last character
        /// </summary>
        /// <value>The current column.</value>
        public int CurrentColumn
        {
            get { return _CurrentColumn; }
        }

        public string DocId
        {
            get;
            set;
        }

        #region TextReader overrides
        bool opened;
        public CharacterInputStream(TextReader real)
            : this(real, "Console")
        {

        }

        public CharacterInputStream(TextReader real, string DocId)
        {
            this.DocId = DocId;
            this.Real = real;
            NewLine = System.Environment.NewLine.ToCharArray();
            opened = true;
            _CurrentIndex = -1;
        }

        public CharacterInputStream(string text, string DocId)
            : this(new StringReader(text), DocId)
        {

        }

        public CharacterInputStream(string text, string DocId, int rawIndex, int column, int row)
            : this(new StringReader(text), DocId)
        {
            _CurrentIndex = rawIndex;
            _CurrentColumn = column;
            _CurrentLine = row;
        }

        public CharacterInputStream(string text)
            : this(new StringReader(text), "Console")
        {

        }

        public override int Peek()
        {
            if (readnext)
            {
                return Real.Peek();
            }
            else
            {
                return unreaded;
            }
        }

        public bool Eof
        {
            get { return Peek() == -1; }
        }

        public override int Read()
        {
            if (readnext)
            {
                // increase character counter
                unreaded = Real.Read();
                if (unreaded != -1)
                {
                    _CurrentIndex++;
                    UpdateLineAndColumn(unreaded);
                }

                return unreaded;
            }
            else
            {
                _CurrentIndex++;
                readnext = true;
                return unreaded;
            }

        }

        public object Read(bool eofErrorP, object eofValue)
        {
            int ret = Read();
            if (ret == -1)
            {
                if (eofErrorP)
                {
                    ConditionsDictionary.Error("eof-error-p true");
                }

                return eofValue;
            }

            return ret;
        }

        char[] NewLine;

        bool makenewline;
        bool newlinestartreached;
        private void UpdateLineAndColumn(int ret)
        {
            if (ret != -1)
            {
                if (makenewline)
                {
                    _CurrentLine++;
                    _CurrentColumn = 0;
                    makenewline = false;
                }

                char ch = (char)ret;
                if (NewLine.Length == 1 && NewLine[0] == ch)
                {
                    // last character in current line
                    makenewline = true;

                    return;
                }
                if (NewLine.Length == 2 && NewLine[0] == ch)
                {
                    newlinestartreached = true;

                    return;
                }
                if (NewLine.Length == 2 && newlinestartreached && NewLine[1] == ch)
                {
                    newlinestartreached = false;
                    makenewline = true;

                    return;
                }

                _CurrentColumn++;
            }
            else
            {
                // end of file
            }
        }

        public override void Close()
        {
            opened = false;
            Real.Close();
        }

        public new void Dispose()
        {
            Real.Dispose();
        }

        public override bool Equals(object obj)
        {
            return Real.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Real.GetHashCode();
        }
#if !SILVERLIGHT
        public override object InitializeLifetimeService()
        {
            throw new Exception("this must not be called");
        }
#endif
        public override int Read(char[] buffer, int index, int count)
        {
            return Real.Read(buffer, index, count);
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            return Real.ReadBlock(buffer, index, count);
        }

        public override string ReadLine()
        {
            return Real.ReadLine();
        }

        public override string ReadToEnd()
        {
            return Real.ReadToEnd();
        }

        public override string ToString()
        {
            return Real.ToString();
        }

        #endregion

        public void Clear()
        {
            Real.ReadToEnd();
        }

        public bool Listen
        {
            get
            {
                return Real.Peek() != -1 ? true : false;
            }
        }

        public bool OpenStreamP
        {
            get { return opened; }
        }

        bool readnext = true;
        int unreaded;
        public void UnreadChar(char ch)
        {
            if (unreaded == ch)
            {
                unreaded = ch;
                readnext = false;
                _CurrentIndex--;
            }
        }
    }


    public class CharacterOutputStream : TextWriter, ICharacterOutputStream
    {
        TextWriter Real;

        public CharacterOutputStream(TextWriter tw)
        {
            Real = tw;
        }
        
        #region IOutputStream Members

        public object FinishOutput()
        {
            throw new NotImplementedException();
        }

        public object ForceOutput()
        {
            throw new NotImplementedException();
        }

        public object ClearOutput()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ILispStream Members


        public bool OpenStreamP
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public override Encoding Encoding
        {
            get { return Real.Encoding; }
        }

        public override void Close()
        {
            Real.Close();
        }

        public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
        {
            return Real.CreateObjRef(requestedType);
        }

        protected override void Dispose(bool disposing)
        {
            Real.Dispose();
        }

        public override void Flush()
        {
            Real.Flush();
        }

        public override IFormatProvider FormatProvider
        {
            get
            {
                return Real.FormatProvider;
            }
        }

        public override object InitializeLifetimeService()
        {
            return Real.InitializeLifetimeService();
        }

        public override string NewLine
        {
            get
            {
                return Real.NewLine;
            }
            set
            {
                Real.NewLine = value;
            }
       }

        public override void Write(bool value)
        {
            Real.Write(value);
        }

        public override void Write(char value)
        {
            Real.Write(value);
        }

        public override void Write(char[] buffer)
        {
            Real.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Real.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            Real.Write(value);
        }

        public override void Write(double value)
        {
            Real.Write(value);
        }

        public override void Write(float value)
        {
            Real.Write(value);
        }

        public override void Write(int value)
        {
            Real.Write(value);
        }

        public override void Write(long value)
        {
            Real.Write(value);
        }

        public override void Write(object value)
        {
            Real.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            Real.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            Real.Write(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Real.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            Real.Write(format, arg);
        }

        public override void Write(string value)
        {
            Real.Write(value);
        }

        public override void Write(uint value)
        {
            Real.Write(value);
        }

        public override void Write(ulong value)
        {
            Real.Write(value);
        }

        public override void WriteLine()
        {
            Real.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            Real.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            Real.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            Real.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            Real.WriteLine(buffer, index, count);
        }

        public override void WriteLine(decimal value)
        {
            Real.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            Real.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            Real.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            Real.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            Real.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            Real.WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            Real.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            Real.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Real.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            Real.WriteLine(format, arg);
        }

        public override void WriteLine(string value)
        {
            Real.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            Real.WriteLine(value);
        }

        public override void WriteLine(uint value)
        {
            Real.WriteLine(value);
        }
    }

}
