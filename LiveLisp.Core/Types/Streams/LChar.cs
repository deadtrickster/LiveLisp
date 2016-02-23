using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.Types.Streams
{
    public class LChar
    {
        Int32 _code;
        public Int32 Code
        {
            get{return _code;}
        }

        char _nativeChar;

        public Char NativeChar
        {
            get { return _nativeChar; }
        }

        public LChar(Char ch)
        {

            _nativeChar = ch;
            _code = ch;
        }

        private LChar(Int32 code)
        {
            _nativeChar = (char)code;
            _code = code;
        }

        private LChar( Char ch, Int32 code)
        {
            _nativeChar = ch;
            _code = code;
        }

        public override string ToString()
        {
            return _nativeChar.ToString();
        }

        public string ToString(IFormatProvider provider)
        {
            return _nativeChar.ToString(provider);
        }

        public static implicit operator LChar(int code)
        {
            if (code < char.MinValue || code > char.MaxValue)
                throw new ArgumentOutOfRangeException("code");

            return GetFromCache(code);
        }

        public static implicit operator LChar(char ch)
        {
            return GetFromCache(ch);
        }

        public static implicit operator int(LChar ch)
        {
            return ch._code;
        }

        public static implicit operator char(LChar ch)
        {
            return ch._nativeChar;
        }

        static Dictionary<int, LChar> _cache = new Dictionary<int, LChar>();

        private static LChar GetFromCache(Int32 code)
        {
            if (!_cache.ContainsKey(code))
            {
                _cache.Add(code, new LChar(code));
            }
            // если int16 поддерживается clr использовать тут его
           // throw new NotImplementedException();

            return _cache[code];
        }

        private static LChar GetFromCache(char ch)
        {
            int code = ch;
            if (!_cache.ContainsKey(code))
            {
                _cache.Add(code, new LChar(ch, code));
            }
            // если int16 поддерживается clr использовать тут его
            return _cache[code];
        }
    }
}
