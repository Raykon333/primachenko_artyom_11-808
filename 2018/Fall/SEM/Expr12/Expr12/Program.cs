using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expr12
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            string[] vars = input.Split(' ');
            int h = int.Parse(vars[0]);
            int t = int.Parse(vars[1]);
            int v = int.Parse(vars[2]);
            int x = int.Parse(vars[3]);
            double uncomfTimeMin;
            double uncomfTimeMax;
            if(h - t * x <= 0)
            {
                uncomfTimeMin = 0;
            }
            else
            {
                uncomfTimeMin = (h - t * x) / (v - x);
            }
            uncomfTimeMax = h / (x + 1);
            if (uncomfTimeMax > t) uncomfTimeMax = t;
            Console.WriteLine(uncomfTimeMin.ToString());
            Console.WriteLine(uncomfTimeMax.ToString());
        }
    }
}
