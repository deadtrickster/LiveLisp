using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Reader;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.BuiltIns.Characters;

namespace LiveLisp.Core.Types
{
    public enum ReadtableCases
    {
        Upcase,
        Downcase,
        Preserve,
        Invert
    }

    public partial class Readtable
    {
        List<char> WhiteSpaces;

        List<char> Termainals;

        List<char> InvalidChars;

        Dictionary<char, LispFunction> MacroTable;

        public ReadtableCases ReadtableCase;

        

        Mirror<string, char> CharNicknames;

        int _cachedNumBase;
        public int NumBase
        {
            get { return _cachedNumBase; }
            set { _cachedNumBase = value; DefinedSymbols._Read_Base_.Value = value; }
        }
        void _ReadBase__PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
                _cachedNumBase = (int)DefinedSymbols._Read_Base_.Value;
        }


        public string RationalSplit;

        public char SignleEscape;

        public char MultipleEscape;
    }
}
