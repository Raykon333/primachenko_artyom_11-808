using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextCompression
{
    class TextEncoder
    {
        public readonly BaseDictionaryType BaseDictionaryType;

        private enum UnsignedType
        {
            Byte,
            Ushort,
            Uint
        }

        public TextEncoder(BaseDictionaryType baseDictionaryType)
        {
            BaseDictionaryType = baseDictionaryType;
        }

        public IEnumerable<byte> Encode(string SourceFilePath)
        {
            switch (BaseDictionaryType)
            {
                case BaseDictionaryType.Auto:
                    yield return 0;
                    foreach (var b in EncodeWithAutoDictionary(SourceFilePath))
                        yield return b;
                    break;

                case BaseDictionaryType.Char:
                    yield return 1;
                    foreach (var b in EncodeUTF8Dictionary(SourceFilePath))
                        yield return b;
                    break;
            }
        }

        private static IEnumerable<byte> EncodeWithAutoDictionary(string SourceFilePath)
        {
            Dictionary<char, uint> chars = new Dictionary<char, uint>();

            using (StreamReader sr = new StreamReader(SourceFilePath))
            {
                HashSet<char> hs = new HashSet<char>();
                while (sr.Peek() >= 0)
                {
                    hs.Add((char)sr.Read());
                }

                uint i = 0;
                chars = hs.ToDictionary(c => c, c => i++);
            }

            long textLength = new FileInfo(SourceFilePath).Length;
            int charsCount = chars.Count();

            char[] returnCharsArray = new char[charsCount];
            foreach (var entry in chars)
            {
                returnCharsArray[entry.Value] = entry.Key;
            }
            foreach (var b in BitConverter.GetBytes(charsCount))
                yield return b;
            foreach (var c in returnCharsArray)
                foreach (var b in BitConverter.GetBytes(c))
                    yield return b;

            UnsignedType uType = UnsignedType.Uint;
            if (charsCount + textLength < 256)
                uType = UnsignedType.Byte;
            else if (charsCount + textLength < 65536)
                uType = UnsignedType.Ushort;
            else
                uType = UnsignedType.Uint;
            yield return (byte)uType;

            using (StreamReader sr = new StreamReader(SourceFilePath))
            {
                Dictionary<string, uint> words = new Dictionary<string, uint>();
                uint i = (uint)charsCount;
                string w = ((char)sr.Read()).ToString();
                while (sr.Peek() >= 0)
                {
                    char entry = (char)sr.Read();
                    if (words.ContainsKey(w + entry))
                    {
                        w = w + entry;
                    }
                    else
                    {
                        if (w.Length == 1)
                            foreach (var b in UintToUtype(chars[w.First()], uType))
                                yield return b;
                        else
                            foreach (var b in UintToUtype(words[w], uType))
                                yield return b;
                        words.Add(w + entry, i++);
                        w = entry.ToString();
                    }
                }
                if (w.Length == 1)
                    foreach (var b in UintToUtype(chars[w.First()], uType))
                        yield return b;
                else
                    foreach (var b in UintToUtype(words[w], uType))
                        yield return b;
            }
        }

        private static IEnumerable<byte> UintToUtype(uint number, UnsignedType type)
        {
            switch (type)
            {
                case UnsignedType.Byte:
                    yield return (byte)number;
                    break;

                case UnsignedType.Ushort:
                    foreach (var b in BitConverter.GetBytes((ushort)number))
                        yield return b;
                    break;

                case UnsignedType.Uint:
                    foreach (var b in BitConverter.GetBytes(number))
                        yield return b;
                    break;
            }
        }

        private static IEnumerable<byte> EncodeUTF8Dictionary(string SourceFilePath)
        {
            long textLength = new FileInfo(SourceFilePath).Length;
            int charsCount = 256;

            UnsignedType uType = UnsignedType.Uint;
            if (charsCount + textLength < 256)
                uType = UnsignedType.Byte;
            else if (charsCount + textLength < 65536)
                uType = UnsignedType.Ushort;
            else
                uType = UnsignedType.Uint;
            yield return (byte)uType;

            using (StreamReader sr = new StreamReader(SourceFilePath))
            {
                Dictionary<string, uint> words = new Dictionary<string, uint>();
                uint i = (uint)charsCount;
                string w = ((char)sr.Read()).ToString();
                while (sr.Peek() >= 0)
                {
                    char entry = (char)sr.Read();
                    if (words.ContainsKey(w + entry))
                    {
                        w = w + entry;
                    }
                    else
                    {
                        if (w.Length == 1)
                            foreach (var b in UintToUtype((uint)Encoding.UTF8.GetBytes(w).First(), uType))
                                yield return b;
                        else
                            foreach (var b in UintToUtype(words[w], uType))
                                yield return b;
                        words.Add(w + entry, i++);
                        w = entry.ToString();
                    }
                }
                if (w.Length == 1)
                    foreach (var b in UintToUtype((uint)Encoding.UTF8.GetBytes(w).First(), uType))
                        yield return b;
                else
                    foreach (var b in UintToUtype(words[w], uType))
                        yield return b;
            }
        }
    }
}
