using System;

namespace Cons2
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class Fraction
    {
        private int denominator;
        int Numerator { get; set; }
        int Denominator
        {
            get { return denominator; }
            set
            {
                if (value > 0)
                    denominator = value;
                throw new ArgumentException();
            }
        }


    }
}
