using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cond3
{
    class Program
    {
        static void Main(string[] args)
        {
            string number = Console.ReadLine();
            Console.WriteLine(((number[0] + number[1] + number[2]) == (number[3] + number[4] + number[5] - 1))
                || ((number[0] + number[1] + number[2]) == (number[3] + number[4] + number[5] + 1)));
        }
    }
}
