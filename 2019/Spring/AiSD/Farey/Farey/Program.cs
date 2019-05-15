using System;

namespace Farey
{
    class List
    {
        public Fraction Head { get; set; }

        public void Insert(Fraction fraction, Fraction newFraction)
        {
            newFraction.Next = fraction.Next;
            fraction.Next = newFraction;
        }
    }

    class Fraction
    {
        public readonly int Num;
        public readonly int Den;
        public Fraction Next { get; set; }

        public Fraction(int num, int den)
        {
            Num = num;
            Den = den;
        }

        public void Write()
        {
            Console.WriteLine(Num + "/" + Den);
        }
    }

    class Program
    {
        static void Main()
        {
            int n = int.Parse(Console.ReadLine());
            List list = new List { Head = new Fraction(0, 1) { Next = new Fraction(1, 1) } };
            for (int i = 2; i <= n; i++)
            {
                Fraction fraction = list.Head;
                while(fraction.Next != null)
                {
                    Fraction buffer = fraction.Next;
                    if (fraction.Den + fraction.Next.Den == i)
                        list.Insert(fraction,
                            new Fraction(fraction.Num + fraction.Next.Num,
                            fraction.Den + fraction.Next.Den));
                    fraction = buffer;
                }
            }
            Fraction writable = list.Head;
            while (writable != null)
            {
                writable.Write();
                writable = writable.Next;
            }
        }
    }
}
