using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rec3
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 5;
            int[] permutation = new int[n];
            GetPermutationsByLine(permutation, 0);
            Console.WriteLine("end");
            Console.ReadKey();
        }

        private static void GetPermutationsByLine(int[] permutation, int line)
        {
            if (line == permutation.Length)
            {
                foreach (var x in permutation)
                    Console.Write(x);
                Console.WriteLine("/n");
            }
            else
            {
                for (int i = 0; i < permutation.Length; i++)
                {
                    bool check = true;
                    for (int j = 0; j < line; j++)
                    {
                        check &= !(i == permutation[j]);
                        check &= !(i - line == permutation[j] - j);
                        check &= !(i + line == permutation[j] + j);
                    }
                    if (check)
                    {
                        permutation[line] = i;
                        GetPermutationsByLine(permutation, line + 1);
                    }
                }
            }
        }
    }
}
