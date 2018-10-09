using System;

namespace Names
{
    internal static class HeatmapTask
    {
        public static HeatmapData GetBirthsPerDateHeatmap(NameData[] names)
        {
            string[] days = new string[30];
            for (int i = 0; i < days.Length; i++)
                days[i] = (i + 2).ToString();
            string[] months = new string[12];
            for (int i = 0; i < months.Length; i++)
                months[i] = (i + 1).ToString();
            double[,] map = new double[days.Length, months.Length];
            foreach (var name in names)
                if (name.BirthDate.Day != 1)
                    map[name.BirthDate.Day - 2, name.BirthDate.Month - 1]++;
            return new HeatmapData("Карта интенсивностей", map, days, months);
        }
    }
}