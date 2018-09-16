using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {
            double k = int.Parse(Console.ReadLine());
            double b = int.Parse(Console.ReadLine());
            if (b != 0)
            {
                Console.WriteLine($"(1; {k})");
            }
            else
            {
                Console.WriteLine("No parallel vector");
            }
            Console.WriteLine($"(1; {-1 / k})");
        }
    }
}

