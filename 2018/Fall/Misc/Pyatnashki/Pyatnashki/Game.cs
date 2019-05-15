//using System.Windows.Forms;

namespace Pyatnashki
{
    public static class Game
    {
        private const string mapWithPlayerTerrain = @"
05 02 15 09
01 00 13 10
06 12 07 04
14 03 08 11";

        public static int[,] Map;
        public static int Scores;
        public static bool IsOver;

        //public static Keys KeyPressed;
        public static int MapWidth => Map.GetLength(0);
        public static int MapHeight => Map.GetLength(1);

        public static void CreateMap()
        {
            //Map = CreatureMapCreator.CreateMap(mapWithPlayerTerrain);
        }
    }
}
