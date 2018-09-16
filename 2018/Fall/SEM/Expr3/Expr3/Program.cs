using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            string hourline = Console.ReadLine();
            int hour = int.Parse(hourline);
            int angle = 180 - Math.Abs(30 * (6 - hour % 12));
            Console.WriteLine(angle);
        }
    }
}