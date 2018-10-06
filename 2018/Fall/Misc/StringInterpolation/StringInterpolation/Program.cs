using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringInterpolation
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine(GetGreetingMessage("Student", 10.01));
            Console.WriteLine(GetGreetingMessage("Bill Gates", 10000000.5));
            Console.WriteLine(GetGreetingMessage("Steve Jobs", 1));
        }

        private static string GetGreetingMessage(string name, double salary)
        {
            // возвращает "Hello, <name>, your salary is <salary>"
            return $"Hello, {name}, your salary is {Math.Ceiling(salary)}";
        }
    }
}
