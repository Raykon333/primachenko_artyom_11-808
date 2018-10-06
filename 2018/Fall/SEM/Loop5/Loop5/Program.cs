using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loop5
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            int depth = 0;
            int maxDepth = 0;
            while (input.Contains("(") && input.Contains(")")
                && input.IndexOf('(') < input.IndexOf(')'))
            {
                for (int i = 0; i < input.Length - 1; i++)
                {
                    if (input[i] == '(' && input[i + 1] == ')')
                    {
                        depth++;
                        if (depth > maxDepth)
                            maxDepth = depth;
                        input = input.Remove(i, 2);
                        depth = 0;
                        break;
                    }
                    else
                        depth++;
                }
            }
            if (input.Length == 0)
            {
                Console.WriteLine("True");
                Console.WriteLine(maxDepth);
            }
            else
                Console.WriteLine("False");
            Console.WriteLine(input);
        }
    }
}
