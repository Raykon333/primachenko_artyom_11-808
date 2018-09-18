using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expr10
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            string[] vars = input.Split(' ');
            int limit = int.Parse(vars[0]) - 1;
            int x = int.Parse(vars[1]);
            int y = int.Parse(vars[2]);
            int nx = (limit - x) / x + 1;
            int sumx = (2 * x + (nx - 1) * x) * nx / 2;
            int ny = (limit - y) / y + 1;
            int sumy = (2 * y + (ny - 1) * y) * ny / 2;
            int nxy = (limit - x * y) / (x * y) + 1;
            int sumxy = (2 * x * y + (nxy - 1) * x * y) * nxy / 2;
            Console.WriteLine(sumx + sumy - sumxy);
        }
    }
}
