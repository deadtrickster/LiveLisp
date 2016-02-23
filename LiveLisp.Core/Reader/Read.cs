using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Reader;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.Reader
{
    public class CommentPlaceholder
    {
        public String Comment
        {
            get;
            set;
        }

        public CommentPlaceholder(string Comment)
        {
            this.Comment = Comment;
        }
    }

    public class WhitespacePlaceholder
    {

    }

    public class ValueTypePlaceholder
    {
        public object Value
        {
            get;
            set;
        }

        public ValueTypePlaceholder(object Value)
        {
            this.Value = Value;
        }

        public Type TypeOfValue
        {
            get { return Value.GetType(); }
        }
    }
}
