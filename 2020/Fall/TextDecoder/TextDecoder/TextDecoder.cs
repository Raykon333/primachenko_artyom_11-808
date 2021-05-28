using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextDecoder
{
    class TextDecoder
    {
        //Основной метод декодирования
        public static IEnumerable<string> DecodeAutoDict(string inputPath)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(inputPath)))
            {
                //Восстановка словаря фраз длиной в 1 символ
                int dictLength = BitConverter.ToInt32(br.ReadBytes(4));
                List<string> words = new List<string>();
                for (int j = 0; j < dictLength; j++)
                {
                    words.Add(BitConverter.ToChar(br.ReadBytes(2)).ToString());
                }

                //Прочитать закодированную длину одной записи в словаре
                UintType uType = ByteToUintType(br.ReadByte());

                //Декодировать первую фразу, отправить в поток
                int entry = ReadAndConvert(br, uType);
                string entryWord = words[entry];
                string newWordPart = entryWord;
                yield return entryWord;

                //Номер следующей записи в словаре
                int i = dictLength;
                //Пока не достигнут конец потока
                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    //Декодировать и отправить в поток следующую фразу, занести в словарь
                    //конкатенацию прошлой фразы и первого символа новой фразы
                    entry = ReadAndConvert(br, uType);
                    if (entry == i)
                        words.Add(newWordPart + newWordPart.First());
                    else
                        words.Add(newWordPart + words[entry].First());
                    entryWord = words[entry];
                    newWordPart = entryWord;
                    yield return entryWord;
                    i++;
                }
            }
        }

        //Типы неотрицательных целых чисел
        enum UintType
        {
            Byte,
            Ushort,
            Uint
        }

        //Перевод байта с закодированным типом неотрицательных целых чисел
        private static UintType ByteToUintType(byte convertedByte)
        {
            UintType uType;
            switch (convertedByte)
            {
                case 0:
                    uType = UintType.Byte;
                    break;

                case 1:
                    uType = UintType.Ushort;
                    break;

                case 2:
                    uType = UintType.Uint;
                    break;

                default:
                    throw new Exception("Encoded text is in wrong format");
            }
            return uType;
        }

        //В зависимости от используемого целочисленного типа,
        //прочитать один или несколько байтов и преобразовать их в число
        private static int ReadAndConvert(BinaryReader br, UintType uintType)
        {
            int entry;
            switch (uintType)
            {
                case UintType.Byte:
                    entry = br.ReadByte();
                    break;

                case UintType.Uint:
                    entry = BitConverter.ToInt16(br.ReadBytes(2));
                    break;

                case UintType.Ushort:
                    entry = BitConverter.ToInt32(br.ReadBytes(4));
                    break;
                default:
                    throw new ArgumentException();
            }
            return entry;
        }
    }
}
