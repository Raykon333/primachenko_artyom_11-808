using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expr4
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = int.Parse(Console.ReadLine());
            int x = int.Parse(Console.ReadLine());
            int y = int.Parse(Console.ReadLine());
            int c = 0;
            for (int i = 1; i < n; i++)
            {
                int a = i % x;
                int b = i % y;
                if ((a == 0) || (b == 0))
                {
                    c++;
                }
            }
            Console.WriteLine(c);
        }
    }
}