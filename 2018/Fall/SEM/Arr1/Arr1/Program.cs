using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arr1
{
    class Program
    {
        static void Main(string[] args)
        {
            var array = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10 };
            int k = 4;
            /*Array.Reverse(array, 0, array.Length);
            Array.Reverse(array, 0, k);
            Array.Reverse(array, k, array.Length - k);*/
            /*for (int i = 0; i < array.Length; i++)
            {
                int buffer = array[i];
                array[i] = array[(i + k) % array.Length];
                array[(i + k) % array.Length] = buffer;
            }*/
            for (int i = 0; i < array.Length; i++)
                Console.WriteLine(array[i]);
            
        }
    }
}
