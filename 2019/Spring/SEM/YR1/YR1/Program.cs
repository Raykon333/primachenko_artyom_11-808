using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YR1
{
    interface IFormula
    {
        int Size { get; }
        string Write();
    }

    class Constant : IFormula
    {
        public char constant;
        public int Size { get { return 0; } }

        public string Write()
        {
            return constant.ToString();
        }
    }

    class Formula : IFormula
    {
        public IFormula Formula1;
        public IFormula Formula2;
        public char Operation;
        public bool IsBracketed;
        public int Size { get { return Formula1.Size + Formula2.Size + 1; } }

        public string Write()
        {
            if (IsBracketed)
                return '(' + Formula1.Write() + Operation + Formula2.Write() + ')';
            return Formula1.Write() + Operation + Formula2.Write();
        }
    }

    class Program
    {
        private static char[] constants = new char[] { '0', '1', '2', 'x' };
        private static char[] operations = new char[] { '+', '-', '*' };
        private static bool[] bools = new bool[] { false, true };

        static void Main(string[] args)
        {
            foreach (var x in GetAllOfSize(2))
                Console.WriteLine(x.Write());
            Console.ReadKey();
        }

        private static IEnumerable<IFormula> GetAllOfSize(int size)
        {
            if (size == 0)
                foreach (var constant in constants)
                    yield return new Constant { constant = constant };
            else
                foreach(var formula1 in GetAllOfSize(size - 1))
                    foreach(var formula2 in GetAllOfSize(0))
                        foreach(var operation in operations)
                            foreach(var boolx in bools)
                            {
                                Formula result = new Formula() { Formula1 = formula1, Formula2 = formula2, Operation = operation, IsBracketed = boolx };
                                yield return result;
                            }
        }
    }
}
