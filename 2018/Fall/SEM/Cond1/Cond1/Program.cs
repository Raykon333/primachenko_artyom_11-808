using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cond1
{
    class Program
    {
        static bool Rook(int x1, int y1, int x2, int y2)
        {
            return (x1 == x2) ^ (y1 == y2);
        }

        static bool Bishop(int x1, int y1, int x2, int y2)
        {
            return ((Math.Abs(x1 - x2) == Math.Abs(y1 - y2)) && (x1 != x2));
        }

        static bool Knight(int x1, int y1, int x2, int y2)
        {
            return (Math.Abs(x1 - x2) == 2) && (Math.Abs(y1 - y2) == 1) || (Math.Abs(x1 - x2) == 1) && (Math.Abs(y1 - y2) == 2);
        }

        static bool King(int x1, int y1, int x2, int y2)
        {
            return (Math.Abs(x1 - x2) <= 1) && (Math.Abs(y1 - y2) <= 1) && ((x1 != x2) || (y1 != y2));
        }

        static void Main()
        {
            string input = Console.ReadLine();
            string[] vars = input.Split(' ');
            int x1 = int.Parse(vars[0]);
            int y1 = int.Parse(vars[1]);
            int x2 = int.Parse(vars[2]);
            int y2 = int.Parse(vars[3]);
            Console.WriteLine("Rook: " + Rook(x1, y1, x2, y2));
            Console.WriteLine("Bishop: " + Bishop(x1, y1, x2, y2));
            Console.WriteLine("Knight: " + Knight(x1, y1, x2, y2));
            Console.WriteLine("Queen: " + (Rook(x1, y1, x2, y2) || Bishop(x1, y1, x2, y2)));
            Console.WriteLine("King: " + King(x1, y1, x2, y2));
        }
    }
}
