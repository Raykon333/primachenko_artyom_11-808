using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loop1
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            int n = input.Length;
            string output = "";
            for (int i = n - 1; i >= 0; i--)
            {
                output += input[i];
            }
            Console.WriteLine(output);
        }
    }
}
