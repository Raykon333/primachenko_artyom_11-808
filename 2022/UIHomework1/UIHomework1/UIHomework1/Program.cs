using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace UIHomework1
{
    public enum DigitType
    {
        Arabic,
        Pictogram
    }

    public struct TestOptions
    {
        public DigitType DigitType;
        public int DigitsAmount;
        public (int, int, int) DigitRGB;
        public (int, int, int) BackgroundRGB;
    }

    public class MemoryTest
    {
        //Хранит текущий тестовый набор
        public int[] TestArray;

        public void GenerateArray(int length)
        {
            if (length < 1 || length > 9)
                throw new ArgumentException();

            var allDigits = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var result = new HashSet<int>();
            Random rng = new Random();

            for (int i = 0; i < length; i++)
            {
                int number = rng.Next(allDigits.Count);
                result.Add(allDigits[number]);
                allDigits.RemoveAt(number);
            }
            TestArray = result.ToArray();
        }

        public float GetAnswersAccuracy(int[] answers)
        {
            int rightAnswers = 0;
            foreach (int number in answers)
            {
                if (TestArray.Contains(number))
                {
                    rightAnswers++;
                }
            }

            return (float)rightAnswers / TestArray.Length;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var testOptions = GetTestOptions();
            var test = new MemoryTest();

            test.GenerateArray(testOptions.DigitsAmount);
            var array = test.TestArray;
            var timeInterval = 1000;

            for (int i = 0; i < array.Length; i++)
            {
                DisplayDigit(array[i], testOptions.DigitType, testOptions.DigitRGB, testOptions.BackgroundRGB);
                Thread.Sleep(timeInterval);
            }

            var answers = GetAnswers();
            var accuracy = test.GetAnswersAccuracy(answers);

            Console.WriteLine(accuracy * 100);

            SaveResult(testOptions, accuracy);

            Image img = Bitmap.FromFile("1.png");
            Bitmap bmp = new Bitmap(img);
            CreateImageWithColors(bmp, testOptions.DigitRGB, testOptions.BackgroundRGB);
        }

        //ЗАГЛУШКА
        static TestOptions GetTestOptions()
        {
            return new TestOptions
            {
                DigitType = DigitType.Arabic,
                DigitsAmount = 5,
                DigitRGB = (0, 0, 255),
                BackgroundRGB = (255, 0, 0)
            };
        }

        //ЗАГЛУШКА
        static void DisplayDigit(int digit, DigitType type, (int, int, int) digitRGB, (int, int, int) backgroundRGB)
        {
            Console.WriteLine(digit);
        }

        //ЗАГЛУШКА
        static int[] GetAnswers()
        {
            return new int[] { 1, 3, 5, 7, 9 };
        }

        static void SaveResult(TestOptions options, float accuracy)
        {
            string filePath = "results.txt";

            var digitType = options.DigitType.ToString();
            var digitsAmount = options.DigitsAmount;
            var digitRGB = options.DigitRGB.ToString();
            var backgroundRGB = options.BackgroundRGB.ToString();
            var dateTime = DateTime.Now;

            string newEntry = $"{dateTime}: Digit type - {digitType}, digits amount - {digitsAmount}, digit RGB - {digitRGB}, background RGB - {backgroundRGB}, accuracy - {accuracy}\n";
            File.AppendAllText(filePath, newEntry);
        }

        static Bitmap CreateImageWithColors(Bitmap bmp, (int r, int g, int b) digitRGB, (int r, int g, int b) backgroundRGB)
        {

            int x, y;

            // Loop through the images pixels to reset color.
            for (x = 0; x < bmp.Width; x++)
            {
                for (y = 0; y < bmp.Height; y++)
                {
                    Color pixelColor = bmp.GetPixel(x, y);
                    Color newColor = new Color();
                    if (pixelColor.R + pixelColor.G + pixelColor.B == 0)
                        newColor = Color.FromArgb(digitRGB.r, digitRGB.g, digitRGB.b);
                    if (pixelColor.R + pixelColor.G + pixelColor.B == 255 * 3)
                        newColor = Color.FromArgb(backgroundRGB.r, backgroundRGB.g, backgroundRGB.b);
                    bmp.SetPixel(x, y, newColor);
                }
            }

            bmp.Save("newColors.png", ImageFormat.Png);
            return bmp;
        }
    }
}
