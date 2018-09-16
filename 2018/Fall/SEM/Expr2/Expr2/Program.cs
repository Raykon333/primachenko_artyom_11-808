using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expr2
{
    class Program
    {
        static void Main(string[] args)
        {
            string line = Console.ReadLine();
            int a = int.Parse(line);
            int x = (a / 100);
            int y = (a % 100) / 10;
            int z = (a % 10);
            a = (z * 100 + y * 10 + x);
            line = a.ToString();
            Console.WriteLine(line);
        }
    }
}

