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
            int[] array = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, -1, -2, -3, -4, -5 };
            int[] sums = new int[array.Length];
            sums[0] = array[0];
            for (int i = 1; i < array.Length; i++)
                sums[i] = sums[i - 1] + array[i];
            int r = int.Parse(Console.ReadLine());
            int l = int.Parse(Console.ReadLine());
            int x = Math.Max(r, l);
            l = Math.Min(r, l);
            r = x;
            if (l != 0)
                Console.WriteLine(sums[r] - sums[l - 1]);
            else
                Console.WriteLine(sums[r]);
        }
    }
}
