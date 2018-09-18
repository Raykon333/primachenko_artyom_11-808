using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expr13
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            string[] vars = input.Split(' ');
            double a = int.Parse(vars[0]);
            double r = int.Parse(vars[1]);
            double s;
            double alpha = Math.Acos(a / r / 2) * 2;
            if (r <= a/2) s = Math.PI * r * r;
            else
            {
                if (r >= a * a / 2) s = a * a;
                else
                {
                    s = (Math.PI * r * r) - 4 * ((r * r * alpha / 2) - (r * r * Math.Sin(alpha) / 2));
                }
            }
            Console.WriteLine(s);
        }
    }
}
