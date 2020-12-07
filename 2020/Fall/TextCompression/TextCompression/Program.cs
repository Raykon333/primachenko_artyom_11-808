using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Убедитесь, что исходный текст находится в папке с программой и называется 'Text.txt'. " +
                "Результат кодирования будет в файле 'EncodedText', результат декодирования - в файле 'DecodedText.txt'");
            string input = null;
            while (input == null)
            {
                Console.WriteLine("Введите 0 для кодирования с автоматическим базовым словарём, 1 - для кодирования со словарём UTF-8");
                input = Console.ReadLine();
                if (input != "0" && input != "1")
                    input = null;
            }
            BaseDictionaryType dictType = input == "0" ? BaseDictionaryType.Auto : BaseDictionaryType.Char;
            using (BinaryWriter bw = new BinaryWriter(File.Open("EncodedText", FileMode.Create)))
            {
                TextEncoder te = new TextEncoder(dictType);
                foreach (var b in te.Encode("Text.txt"))
                    bw.Write(b);
            }
            TextDecoder.Decode("EncodedText", "DecodedText.txt");
        }
    }
}
