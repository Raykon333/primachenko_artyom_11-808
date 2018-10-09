using System;
using System.Linq;

namespace Names
{
    internal static class HistogramTask
    {
        public static HistogramData GetBirthsPerDayHistogram(NameData[] names, string searchedName)
        {
            string[] labels = new string[31];
            for (int i = 0; i < labels.Length; i++)
                labels[i] = (i + 1).ToString();
            double[] values = new double[31];
            foreach (var name in names)
            {
                if (searchedName == name.Name && name.BirthDate.Day != 1)
                    values[name.BirthDate.Day - 1]++;
            }
            return new HistogramData(string.Format("Рождаемость людей с именем '{0}'", searchedName), labels, values);
        }
    }
}