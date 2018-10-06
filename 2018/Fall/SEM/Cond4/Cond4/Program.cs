using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cond4
{
    class Program
    {
        static void FindIntersection(int A, int B, int C, int D)
        {
            if (A > D)
            Console.WriteLine("No intersection");
            else if (C > B)
            Console.WriteLine("No intersection");
            else if (C <= A && B <= D)
            Console.WriteLine(String.Format("{0} {1}", A, B));
            else if (A <= C && D <= B)
            Console.WriteLine(String.Format("{0} {1}", C, D));
            else if (A <= C && B <= D)
            Console.WriteLine(String.Format("{0} {1}", C, B));
            else if (C <= A && D <= B)
            Console.WriteLine(String.Format("{0} {1}", A, D));
        }

        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            string[] vars = input.Split(' ');
            int A = int.Parse(vars[0]);
            int B = int.Parse(vars[1]);
            int C = int.Parse(vars[2]);
            int D = int.Parse(vars[3]);
            FindIntersection(A, B, C, D);
        }
    }
}
