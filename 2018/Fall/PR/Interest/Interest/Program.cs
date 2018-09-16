using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interest
{
    class Program
    {
        static void Main(string[] args)
        {
            double sum = double.Parse(Console.ReadLine());
            double interest = double.Parse(Console.ReadLine());
            int term = int.Parse(Console.ReadLine());
            Console.WriteLine(Calculate(sum, interest, term).ToString());
        }
        public static double Calculate(double sum, double interest, int term)
        {
            sum = sum * Math.Pow(((1 + interest / 12 * 0.01)), term);
            return sum;
        }
    }
}
