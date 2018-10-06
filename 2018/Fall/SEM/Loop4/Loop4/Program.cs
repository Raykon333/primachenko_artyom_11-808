using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loop4
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            int max = 1;
            int counter = 1;
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == input[i - 1])
                    counter++;
                else
                {
                    if (max < counter)
                    {
                        max = counter;
                    }
                    counter = 1;
                }
            }
            if (max < counter)
            {
                max = counter;
            }
            Console.WriteLine(max);
        }
    }
}
