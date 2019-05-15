using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rec4
{
    class Program
    {
        static void Main(string[] args)
        {
            var puzzle = new char[,] { { 'a', 'b', 'c' }, { 'd', 'e', 'f' }, { 'g', 'h', 'i' } };
            var word = new Tuple<int, int>[puzzle.Length * puzzle.Length];
            for (int i = 0; i < word.Length; i++)
                word[i] = new Tuple<int, int>(-1, -1);
            var dictionary = new string[] { "abcfj", "abcfe", "abedghjfc", "abefg" };
        }

        static void GetWord(Tuple<int, int>[] word, int position)
        {

        }

        static void CheckWord(Tuple<int, int>[] word, string[] dictionary)
        {
            for (int i = 0; i < word.Length && word[i].Item1 != -1; i++)
            {

            }
        }
    }
}
