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
            string userInput = Console.ReadLine();
            Console.WriteLine(Calculate(userInput).ToString());
        }
        public static double Calculate(string userInput)
        {
            string[] vars = userInput.Split(' ');
            double sum = double.Parse(vars[0]);
            double interest = double.Parse(vars[1]);
            int term = int.Parse(vars[2]);
            return sum * Math.Pow((1 + interest / 12 * 0.01), term);
        }
    }
}
