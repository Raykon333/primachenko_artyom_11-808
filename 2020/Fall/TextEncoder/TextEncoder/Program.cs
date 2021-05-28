using System;
using System.IO;

namespace TextEncoder
{
    class Program
    {
        //Примаченко Артём
        //Плотников Илья

        static void Main(string[] args)
        {
            //Перечисление аргументов
            Console.WriteLine("Параметры запуска:");
            foreach (var arg in args)
                Console.WriteLine(arg);
            Console.WriteLine();

            //Получение входных данных
            string inputName = "-i";
            string inputDescription = "путь до сжимаемого файла";
            string outputName = "-o";
            string outputDescription = "путь до выходного файла";
            string inputPath = GetFromArgsOrRequest(args, inputName, inputDescription);
            string outputPath = GetFromArgsOrRequest(args, outputName, outputDescription);

            if (!File.Exists(inputPath))
                throw new ArgumentException("Файл " + inputPath + " не найден.");

            //Кодирование
            using (BinaryWriter bw = new BinaryWriter(File.Open(outputPath, FileMode.Create)))
            {
                foreach (var b in TextEncoder.EncodeWithAutoDictionary(inputPath))
                    bw.Write(b);
            }
            Console.WriteLine("Программа завершила работу");
        }

        //Взять входные данные из аргументов, если их нет, то запросить в консоли
        static string GetFromArgsOrRequest(string[] args, string parameterName, string parameterDescription)
        {
            int inputIndex = Array.FindIndex(args, a => a == parameterName);
            if (inputIndex == -1 || args.Length <= inputIndex + 1)
            {
                Console.WriteLine("Параметр " + parameterName + " не найден. Введите " + parameterDescription + ":");
                return Console.ReadLine();
            }
            else
                return args[inputIndex + 1];
        }
    }
}
