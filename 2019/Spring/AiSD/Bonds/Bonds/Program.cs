using System;
using System.Diagnostics;

namespace Bonds
{

    class TreeItem
    {
        public int Next;
        public int Weight = 1;

        public TreeItem(int value)
        {
            Next = value;
        }
    }

    static class Bonds
    {
        public static TreeItem[] treeFastest;
        public static TreeItem[] treeFast;

        static Bonds()
        {
            int size = 100000;
            treeFastest = new TreeItem[size];
            treeFast = new TreeItem[size];
            for (int i = 0; i < size; i++)
            {
                treeFastest[i] = new TreeItem(i);
                treeFast[i] = new TreeItem(i);
            }
        }

        public static string ConnectFastest(int point1, int point2)
        {
            point1 = FindStarostaFastest(point1);
            point2 = FindStarostaFastest(point2);
            if (point1 == point2)
                return "Failed";
            if (treeFastest[point1].Weight > treeFastest[point2].Weight)
            {
                treeFastest[point2].Next = point1;
                treeFastest[point2].Weight++;
            }
            else
            {
                treeFastest[point1].Next = point2;
                treeFastest[point1].Weight++;
            }
            return "Succeeded";
        }

        public static string ConnectFast(int point1, int point2)
        {
            point1 = FindStarostaFast(point1);
            point2 = FindStarostaFast(point2);
            if (point1 == point2)
                return "Failed";
            if (treeFast[point1].Weight > treeFast[point2].Weight)
            {
                treeFast[point2].Next = point1;
                treeFast[point2].Weight++;
            }
            else
            {
                treeFast[point1].Next = point2;
                treeFast[point1].Weight++;
            }
            return "Succeeded";
        }

        public static int FindStarostaFastest(int point)
        {
            while (treeFastest[point].Next != point)
            {
                int buffer = point;
                point = treeFastest[point].Next;
                treeFastest[buffer].Next = treeFastest[point].Next;
            }
            return point;
        }
        
        public static int FindStarostaFast(int point)
        {
            while (treeFast[point].Next != point)
            {
                point = treeFast[point].Next;
            }
            return point;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            for(int i = 1; i <= 99999; i++)
            {
                stopwatch.Restart();
                Bonds.ConnectFast(0, i);
                long fast = stopwatch.ElapsedTicks;
                stopwatch.Restart();
                Bonds.ConnectFastest(0, i);
                long fastest = stopwatch.ElapsedTicks;
                stopwatch.Stop();
                Console.WriteLine("Fast: " + fast + "\tFastest: " + fastest);
            }
        }
    }
}
