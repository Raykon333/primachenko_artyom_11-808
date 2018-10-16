using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Col1
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] array = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, -1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11 };
            int[] sums = new int[array.Length];
            int maxsum = int.MinValue;
            sums[0] = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                sums[i] = sums[i - 1] + array[i];
                for (int j = 0; j < i; j++)
                    maxsum = Math.Max(maxsum, sums[i] - sums[j]);
            }
            Console.WriteLine(maxsum);
        }
    }
}
