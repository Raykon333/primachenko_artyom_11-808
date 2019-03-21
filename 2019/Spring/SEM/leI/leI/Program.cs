using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leI
{
    class Program
    {
        static IEnumerable<string> Task1(IEnumerable<string> lines)
        {
            return lines
                .TakeWhile(x => x.Length <= lines.Count())
                .Where(x => Char.IsLetter(x[x.Length - 1]))
                .OrderByDescending(x => x.Length)
                .ThenBy(x => x);
        }

        static IEnumerable<int> Task2(int k, IEnumerable<int> numbers)
        {
            return numbers
                .Take(k + 1)
                .Where(x => x % 2 == 0)
                .Distinct()
                .Reverse();
        }

        static IEnumerable<char> Task3(IEnumerable<string> lines)
        {
            return lines
                .Select(x => x[0])
                .Reverse();
        }

        static IEnumerable<string> Task4(IEnumerable<int> numbers)
        {
            return numbers
                .Where(x => x % 2 != 0)
                .Select(x => x.ToString())
                .OrderBy(x => x);
        }

        static void Main(string[] args)
        {
        }
    }
}
