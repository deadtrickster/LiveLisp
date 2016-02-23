namespace LiveLisp.Core.AST
{
    using System;
    using System.Collections.Generic;
using System.Collections.ObjectModel;

    public class TaggedStatements
    {
        private List<Expression> _statements;
        private object _tag;

        public TaggedStatements(object tag, List<Expression> statements)
        {
            this._tag = tag;
            this._statements = statements;
        }

        public List<Expression> Statements
        {
            get
            {
                return this._statements;
            }
        }

        public object Tag
        {
            get
            {
                return this._tag;
            }
            set
            {
                this._tag = value;
            }
        }
    }

    public class TaggedStatementscollection : KeyedCollection<object, TaggedStatements>
    {
        protected override object GetKeyForItem(TaggedStatements item)
        {
            return item.Tag;
        }
    }

}

