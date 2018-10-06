//wip
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 1  2  3  4  5  6  7  8  9  1  0  1  1  1  2  1  3  1  4  ... 2  0  ...  1   0   0   1   0   1   1   0   2   1   0
// 1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 ... 30 31 ... 190 191 192 193 194 195 196 197 198 199 200

namespace Loop3
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = int.Parse(Console.ReadLine());
            int digitAmount = FindDigitAmount(n);
            int answer = 0;
            //поиск номера n в подмножестве цифр, состоящих в числах из такого же кол-ва цифр
            for (int i = 1; i < digitAmount; i++)
                n -= i * 9 * (int)Math.Pow(10, i - 1);
            for (int i = 0; i < digitAmount; i++)
                if (n % digitAmount == i)
                    answer = n / ((i + 1) * (int)Math.Pow(10, i));
            //проверка на старший разряд
            if (n % digitAmount == digitAmount - 1)
                answer++;
            Console.WriteLine(answer);
        }

        static int FindDigitAmount(int n)
        {
            double s = 0;
            int i;
            for(i = 1; s < n; i++)
                s += 9 * i * Math.Pow(10, i - 1);
            return i - 1;
        }
    }
}
