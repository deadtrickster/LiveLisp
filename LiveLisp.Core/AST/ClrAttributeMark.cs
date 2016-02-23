namespace LiveLisp.Core.AST
{
    using System;
    using System.Collections.Generic;

    public class ClrAttributeMark
    {
        private StaticTypeResolver attributeTypeName = new StaticTypeResolver();
        private List<object> constructorArgs = new List<object>();
        private Dictionary<string, object> namedArgs = new Dictionary<string, object>();

        public StaticTypeResolver AttributeTypeName
        {
            get
            {
                return this.attributeTypeName;
            }
            set
            {
                this.attributeTypeName = value;
            }
        }

        public List<object> ConstructorArgs
        {
            get
            {
                return this.constructorArgs;
            }
            set
            {
                this.constructorArgs = value;
            }
        }

        public Dictionary<string, object> NamedArgs
        {
            get
            {
                return this.namedArgs;
            }
            set
            {
                this.namedArgs = value;
            }
        }
    }
}

