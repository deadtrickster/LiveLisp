using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.Compiler
{
    public class Scope
    {
        public static Scope Global = new Scope();

        Dictionary<Symbol, Slot> slots = new Dictionary<Symbol, Slot>();

        internal bool GetSlot(LiveLisp.Core.Types.Symbol symbol, out Slot slot)
        {
            if (slots.ContainsKey(symbol))
            {
                slot = slots[symbol];
                return true;
            }
            slot = null;
            return false;
        }

        internal void SetSlot(LiveLisp.Core.Types.Symbol symbol, Slot slot)
        {
            if (slots.ContainsKey(symbol))
                slots[symbol] = slot;
            else
                slots.Add(symbol, slot);
        }
    }
}
