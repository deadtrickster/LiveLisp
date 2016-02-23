namespace LiveLisp.Core.AST
{
    using LiveLisp.Core.Types;
    using System;
using LiveLisp.Core.Compiler;

    public class SyntaxBinding
    {
        private LiveLisp.Core.Types.Symbol symbol;
        private Expression value;

        public SyntaxBinding(LiveLisp.Core.Types.Symbol symbol, Expression value)
        {
            this.symbol = symbol;
            this.value = value;
        }

        public SyntaxBinding(Slot slot, Expression value)
        {
            this.Slot = slot;
            this.value = value;
        }

        public LiveLisp.Core.Types.Symbol Symbol
        {
            get
            {
                return this.symbol;
            }
            set
            {
                this.symbol = value;
            }
        }

        public Expression Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public Slot Slot
        {
            get;
            set;
        }
    }
}

