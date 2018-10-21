using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Col24
{
    class Program
    {
        static public List<int> prime = new List<int>();

        static void Main(string[] args)
        {
            prime.Add(2);
            prime.Add(3);
            int k = int.Parse(Console.ReadLine());
            int[] array = new int[k];
            for (int i = 0; i < k; i++)
                array[i] = int.Parse(Console.ReadLine());
            int max = 0;
            for (int i = 0; i < k; i++)
                if (max < array[i])
                    max = array[i];
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 1; i < max; i++)
            {
                TrialDivision();
            }
            stopwatch.Stop();
            foreach (var number in array)
                Console.WriteLine(prime[number - 1]);
            //Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
        }

        static void TrialDivision()
        {
            for (int j = prime[prime.Count - 1] + 2; ; j = j + 2)
            {
                bool check = true;
                foreach (var x in prime)
                {
                    if (j % x == 0)
                    {
                        check = false;
                        break;
                    };
                }
                if (check)
                {
                    prime.Add(j);
                    break;
                }
            }
        }
    }
}
