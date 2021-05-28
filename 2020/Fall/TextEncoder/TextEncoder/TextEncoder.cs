using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextEncoder
{
    class TextEncoder
    {
        //Типы неотрицательных целых чисел
        private enum UnsignedType
        {
            Byte,
            Ushort,
            Uint
        }

        //Основной метод для кодирования
        internal static IEnumerable<byte> EncodeWithAutoDictionary(string SourceFilePath)
        {
            //Словарь фраз длиной в 1 символ
            Dictionary<char, uint> chars = new Dictionary<char, uint>();

            //Заполнение словаря всеми уникальными символами
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

            //Кодирование словаря
            //Перед словарём четырьми битами передаётся количество символов в нём
            //Затем код каждого символа длиной по 2 бита
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

            //Определение того, сколько байтов необходимо выделить на одну запись
            //в словаре, в зависимости от максимальной предполагаемой длины словаря
            long textLength = new FileInfo(SourceFilePath).Length;
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
                //Инициализация словаря фраз длиной от двух символов
                Dictionary<string, uint> words = new Dictionary<string, uint>();

                //Номер следующей записи в словаре
                uint i = (uint)charsCount;
                //
                string w = ((char)sr.Read()).ToString();

                //Пока не достигнут конец потока
                while (sr.BaseStream.Position != sr.BaseStream.Length)
                {
                    //Прочитать следующий символ
                    char entry = (char)sr.Read();
                    //Если фраза с новым символом содержится в словаре, 
                    //перейти к следующему символу
                    if (words.ContainsKey(w + entry))
                    {
                        w = w + entry;
                    }
                    //Иначе добавить в словарь фразу с новым символом 
                    //и принять за фразу новый символ, отправив фразу в поток
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
                //Кодирование последней фразы
                if (w.Length == 1)
                    foreach (var b in UintToUtype(chars[w.First()], uType))
                        yield return b;
                else
                    foreach (var b in UintToUtype(words[w], uType))
                        yield return b;
            }
        }

        //Перевод неотрицательного целого числа в байты
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
    }
}
