using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.Reader
{
    public class SourceSpan
    {
        public Int32 StartLine
        {
            get;
            set;
        }

        public Int32 StartColumn
        {
            get;
            set;
        }

        public Int32 EndLine
        {
            get;
            set;
        }

        public Int32 EndColumn
        {
            get;
            set;
        }

        public Int32 RawStartIndex
        {
            get;
            set;
        }

        public Int32 RawEndIndex
        {
            get;
            set;
        }

        public SourceSpan(int startindex, int startline, int startcol, int rawIndex, int currentLine, int currentColumn)
        {
            RawStartIndex = startindex;
            StartLine = startline;
            StartColumn = startcol;

            RawEndIndex = rawIndex;
            EndLine = currentLine;
            EndColumn = currentColumn;
        }

        /// <summary>
        /// Determines if the specified position is contained within the text region spaned by this Span.
        /// </summary>
        /// <param name="i">raw index</param>
        /// <returns></returns>
        public bool Containts(int i)
        {
            return (i >= RawStartIndex) && (i <= RawEndIndex);
        }

        /// <summary>
        /// Determines if the specified position is contained within the text region spaned by this Span.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool Containts(int line, int column)
        {
            bool ret = (line>=StartLine) && (line<=EndLine);
            if(!ret)
                return false;

            if (line == StartLine)
            {
                return column>= StartColumn;
            }

            if (line == EndLine)
            {
                return column <= EndColumn;
            }

            return true;
        }


        public override string ToString()
        {
            return string.Format(pattern, RawStartIndex, StartLine, StartColumn, RawEndIndex, EndLine, EndColumn);
        }

        public string String
        {
            get { return ToString(); }
        }

        static string pattern = @"Start index: " + '\t' + @"{0}." + System.Environment.NewLine +
                                @"Start Line: " + '\t' + @"{1}." + System.Environment.NewLine +
                                @"Start Column: " + '\t' + @"{2}." + System.Environment.NewLine +
                                System.Environment.NewLine +
                                @"End index: " + '\t' + @"{3}." + System.Environment.NewLine +
                                @"End Line: " + '\t' + @"{4}." + System.Environment.NewLine +
                                @"End Column: " + '\t' + @"{5}.";
    }
}


