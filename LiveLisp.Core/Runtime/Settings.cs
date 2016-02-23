using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.Runtime
{

    public enum MetadataCachingModeInClos
    {
        /// <summary>
        /// without caching
        /// </summary>
        None, 

        /// <summary>
        /// Caching while wrapper constructor
        /// </summary>
        WhenCreated,

        /// <summary>
        /// Cache memberinfos on first retrive
        /// </summary>
        WhenReflected
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ReflectionMode
    {
        NameAndPackage,
        Name
    }


    public class Settings
    {

       [ThreadStatic]
        static bool _readWithInfo;
        static bool _readOverrided;
        static bool _useCompilerInRepl;

        public static bool ReadOverrided
        {
            get { return Settings._readOverrided; }
            set
            {
                Settings._readOverrided = value;
                _readWithInfo = !value;
            }
        }
        public static bool ReadWithInfo
        {
            get
            {
                return _readWithInfo;
            }
            set
            {
                _readWithInfo = value;
            }
        }
        public static bool UseCompilerInREPL
        {
            get { return _useCompilerInRepl; }
            set { _useCompilerInRepl = value; }
        }

        public static ReflectionMode ReflectionMode
        {
            get;
            set;
        }

        static MetadataCachingModeInClos metadataCachingModeInClos;

        /// <summary>
        /// Если true  то в момент создания CLOSCLRClass считываются все метаданные оборачиваемого типа
        /// 
        /// </summary>
        public static MetadataCachingModeInClos MetadataCachingModeInClos
        {
            get { return Settings.metadataCachingModeInClos; }
            set { Settings.metadataCachingModeInClos = value; }
        }

        public static Package PackageForReflectedNames
        {
            get { throw new NotImplementedException(); }
        }

        static Settings()
        {
           // _readWithInfo = true;
            _readOverrided = false;
            DefinedSymbols.Read.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Read_PropertyChanged);
            ReflectionMode = ReflectionMode.Name;
        }

        static void Read_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Function")
            {
                _readOverrided = true;
            }

        }


        [ThreadStatic]
        public static bool NotInternSymbols;
    }
}
