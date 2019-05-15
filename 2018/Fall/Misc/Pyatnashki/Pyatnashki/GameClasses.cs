using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyatnashki
{
    class Command
    {
        public void Swap(Dice dice1, Dice dice2)
        {
            var buffer = dice1.Value;
            dice1.Value = dice2.Value;
            dice2.Value = buffer;
        }
    }

    class GameClasses
    {
        static void Main(string[] args)
        {
            Game.Map = new int[,] {
                { 0, 1, 2, 3},
                { 4, 5, 6, 7},
                { 8, 9, 10, 11},
                { 12, 13, 14, 15}
            };
            ConsoleField.WriteField();
        }
    }

    class ConsoleField
    {
        public static void WriteField()
        {
            Console.WriteLine("------------");
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                    Console.Write("|" + Game.Map[i, j] + "|");
                Console.WriteLine("\n------------");
            }
        }
    }

    class PlayerMoves
    {

    }

    public class Dice
    {
        public int Value;
        int X;
        int Y;
    }
}
