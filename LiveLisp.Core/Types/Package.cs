using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using LiveLisp.Core.BuiltIns.Conditions;
//using LiveLisp.Core.BuiltIns.Strings;

namespace LiveLisp.Core.Types
{
    public class Package
    {
        String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        SymbolsCollection internalSymbols;
        protected SymbolsCollection externalSymbols;

        List<Package> usings;
        List<string> nicknames;

        internal Package(string name)
            : this(name, new List<string>(), new List<Package>())
        {

        }

        internal Package(string name, IEnumerable<string> nicknames)
            : this(name, nicknames, new List<Package>())
        {

        }

        internal Package(string name, IEnumerable<string> nicknames, IEnumerable<Package> usings)
        {
            _name = name;
            this.nicknames = new List<string>(nicknames);
            this.usings = new List<Package>(usings);
            internalSymbols = new SymbolsCollection();
            externalSymbols = new SymbolsCollection();
        }

        public static Package Current
        {
            get { return DefinedSymbols._Package_.ValueAsPackage; }
            set { DefinedSymbols._Package_.Value = value; }
        }

        public virtual Symbol Intern(string name, bool makeExternal)
        {
            if (externalSymbols.Contains(name))
                return externalSymbols[name];

            if (internalSymbols.Contains(name))
                return internalSymbols[name];

            var newSymbol = SymbolTable.GetFreshSymbol(name, this);

            if (makeExternal)
                externalSymbols.Add(newSymbol);
            else
                internalSymbols.Add(newSymbol);

            return newSymbol;
        }

        // for regular symbols - not external by default
        public virtual Symbol Intern(string name)
        {
            return Intern(name, false);
        }

        internal bool IsInternal(Symbol symbol)
        {
            if (symbol.Package != this)
                throw new ArgumentException("Package.IsInternal: current package doesn't hold {0} symbol", symbol.Name);

            return internalSymbols.Contains(symbol);
        }

        internal static Symbol LookupInPackages(string name)
        {
            Package package = Package.Current;

            if (package.internalSymbols.Contains(name))
                return package.internalSymbols[name];

            return _lookinusings(package,name);
        }

        private static Symbol _lookinusings(Package _package,string name)
        {
            var package = _package;

            if (_package.externalSymbols.Contains(name))
                return _package.externalSymbols[name];
            Symbol symbol = null;
            foreach (var _using in _package.usings)
            {
                symbol = _lookinusings(_using, name);
                if (symbol != null)
                    return symbol;
            }

            return symbol;
        }

        public List<Symbol> GetAllAcessibleSymbols(Predicate<Symbol> pred)
        {
            List<Symbol> ret = new List<Symbol>();

            for (int i = 0; i < internalSymbols.Count; i++)
            {
                if (pred.Invoke(internalSymbols[i]))
                    ret.Add(internalSymbols[i]);
            }

            for (int i = 0; i < externalSymbols.Count; i++)
            {
                if (pred.Invoke(externalSymbols[i]))
                    ret.Add(externalSymbols[i]);
            }

            for (int i = 0; i < usings.Count; i++)
            {
                ret.AddRange(usings[i].GetAllExternalAndUsedSymbols(pred));
            }
            return ret;
        }
        
        public List<Symbol> GetAllExternalAndUsedSymbols(Predicate<Symbol> pred)
        {
            List<Symbol> ret = new List<Symbol>();

            for (int i = 0; i < externalSymbols.Count; i++)
            {
                if (pred.Invoke(externalSymbols[i]))
                    ret.Add(externalSymbols[i]);
            }

            for (int i = 0; i < usings.Count; i++)
            {
                ret.AddRange(usings[i].GetAllExternalAndUsedSymbols(pred));
            }

            return ret;
        }

        public List<Symbol> GetAllExternalSymbols(Predicate<Symbol> pred)
        {
            List<Symbol> ret = new List<Symbol>();

            for (int i = 0; i < externalSymbols.Count; i++)
            {
                if (pred.Invoke(externalSymbols[i]))
                    ret.Add(externalSymbols[i]);
            }
            return ret;
        }

        public List<Symbol> GetAllInternalSymbols(Predicate<Symbol> pred)
        {
            List<Symbol> ret = new List<Symbol>();

            for (int i = 0; i < internalSymbols.Count; i++)
            {
                if (pred.Invoke(internalSymbols[i]))
                    ret.Add(internalSymbols[i]);
            }
            return ret;
        }

        internal bool IsExternal(string sname, out Symbol symbol)
        {
            symbol = null;
            if (externalSymbols.Contains(sname))
            {
                symbol = externalSymbols[sname];
                return true;
            }
            return false;
        }

        internal bool IsInternal(string sname, out Symbol symbol)
        {
            symbol = null;
            if (internalSymbols.Contains(sname))
            {
                symbol = internalSymbols[sname];
                return true;
            }
            return false;
        }

        internal bool CheckName(string name)
        {
            return _name == name;
        }

        internal bool CheckNickname(string name)
        {
            return nicknames.Contains(name);
        }

        internal void Use(Package package)
        {
            if (usings.Contains(package))
                return;
            usings.Add(package);
        }

        internal void Export(Symbol sym)
        {
            if (sym.Package == this)
            {
                if (internalSymbols.Contains(sym.Name))
                {
                    internalSymbols.Remove(sym);
                }

                if (!externalSymbols.Contains(sym.Name))
                {
                    externalSymbols.Add(sym);
                }
            }
            else
            {
               ConditionsDictionary.Error("Symbol " + sym + " should be imported into #<PACKAGE " + Name + " + > before being exported.");
            }
        }

        internal bool TryGetSymbol(string name, out Symbol symbol)
        {
            throw new NotImplementedException();
        }

        internal bool TryGetAccessibleSymbol(string name, out Symbol symbol)
        {
             symbol = LookupInPackages(name);
            if(symbol == null)
                return false;

            else return true;
        }
    }

    public class KeywordPackage : Package
    {
        internal KeywordPackage()
            : base("")
        {

        }

        public override Symbol Intern(string name)
        {
            if (externalSymbols.Contains(name))
                return externalSymbols[name];


            Symbol symbol = SymbolTable.GetFreshKeywordSymbol(name);
            symbol.Package = this;
            externalSymbols.Add(symbol);
            return symbol;
        }

        public override Symbol Intern(string name, bool makeExternal)
        {
            if (externalSymbols.Contains(name))
                return externalSymbols[name];


            Symbol symbol = SymbolTable.GetFreshKeywordSymbol(name);
            symbol.Package = this;
            externalSymbols.Add(symbol);
            return symbol;
        }

        static KeywordPackage _instance = new KeywordPackage();

        public static KeywordPackage Instance
        {
            get { return _instance; }
        }

        internal Symbol getSymbol(string name)
        {
            if (externalSymbols.Contains(name))
                return externalSymbols[name];

            return null;
        }

        public KeywordSymbol NewKeyword(string name)
        {
            return SymbolTable.GetFreshKeywordSymbol(name) ;
        }

        internal static bool CheckKeyword(object arg1, string name)
        {
            KeywordSymbol k = arg1 as KeywordSymbol;
            if (k != null)
            {
                return k.Name == name;
            }

            return false;
        }
    }

    public class SymbolsCollection : KeyedCollection<string, Symbol>
    {
        protected override string GetKeyForItem(Symbol item)
        {
            return item.Name;
        }
    }

    public class SymbolTable
    {
        static List<Symbol> _store = new List<Symbol>();

        static int counter = 0;

        /// <summary>
        /// don't use with gensym
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Symbol GetFreshSymbol(string name)
        {
            var sym = new Symbol(name, counter++);
            _store.Add(sym);
            return sym;
        }

        /// <summary>
        /// don't use with gensym
        /// </summary>
        /// <param name="name"></param>
        /// <param name="package"></param>
        /// <returns></returns>
        public static Symbol GetFreshSymbol(string name, Package package)
        {
            var sym = new Symbol(name, counter++, package);
            _store.Add(sym);
            return sym;
        }

        public static KeywordSymbol GetFreshKeywordSymbol(string name)
        {
            var sym = new KeywordSymbol(name, counter++);
            _store.Add(sym);
            return sym;
        }

        public static Symbol GetSymbol(int id)
        {
            if (id >= _store.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _store[id];
        }
    }

    public class PackagesCollection : KeyedCollection<string, Package>
    {
        protected override string GetKeyForItem(Package item)
        {
            return item.Name;
        }

        internal static PackagesCollection Instance = new PackagesCollection();

        public static int PackagesCount
        {
            get { return Instance.Count; }
        }

        public static Package GetAtIndex(int index)
        {
            return Instance[index];
        }

        internal static Package FindByNameOrNickname(string name)
        {
            Package ret = null;
            foreach (var package in Instance)
            {
                if (package.CheckName(name) || package.CheckNickname(name))
                {
                    ret = package;
                    break;
                }
            }
            return ret;
        }

        public static Package CreatePackage(string name)
        {
            return CreatePackage(name, new string[] { }, new Package[] { });
        }

        private static Package CreatePackage(string name, string[] nicknames)
        {
            return CreatePackage(name, nicknames, new Package[] { });
        }

        private static Package CreatePackage(string name, string[] nicknames, Package[] usings)
        {
            var ret = new Package(name,nicknames, usings);
            Instance.Add(ret);
            return ret;
        }

        public static Package GetOrCreatePackage(string name)
        {
            return GetOrCreatePackage(name, new string[] { }, new Package[] { });
        }

        internal static Package GetOrCreatePackage(string name, string[] nicknames)
        {
            return GetOrCreatePackage(name, nicknames, new Package[] { });
        }

        internal static Package GetOrCreatePackage(string name, string[] nicknames, Package[] usings)
        {
            Package ret;
            ret = PackagesCollection.FindByNameOrNickname(name);

            if (ret == null)
            {
                return CreatePackage(name, nicknames, usings);
            }

            return ret;
        }

    }
}
