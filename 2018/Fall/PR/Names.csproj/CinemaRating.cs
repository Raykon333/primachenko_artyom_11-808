using System;

namespace Names
{
    public class CinemaRating
    {
        //Найти возраст
        public static int FindAge(NameData name)
        {
            DateTime date = DateTime.Today;
            int age = date.Year - name.BirthDate.Year;
            if (date.DayOfYear < name.BirthDate.DayOfYear)
                age--;
            return age;
        }

        //Создать гистограмму распределения людей по возрасту
        public static HistogramData GetHistogramPerRating(NameData[] names)
        {
            int[] aud = new int[] { 20, 30, 40, 50, 60, 70, 80 }; //настраиваемый список возрастных рейтингов
            string[] labels = CreateLabels(aud);
            double[] values = CreateValues(aud, names);
            return new HistogramData("Распределение по возрастным ограничениям", labels, values);
        }

        //Создать подписи к гистограмме
        public static string[] CreateLabels(int[] aud)
        {
            string[] labels = new string[aud.Length];
            labels[0] = String.Format("<{0}", aud[0].ToString());
            labels[labels.Length - 1] = String.Format("{0}+", aud[aud.Length - 1].ToString());
            for (int i = 1; i < aud.Length - 1; i++)
            {
                labels[i] = String.Format("{0}-{1}", aud[i], aud[i + 1] - 1);
            }
            return labels;
        }

        //Заполнить значения гистограммы
        public static double[] CreateValues(int[] aud, NameData[] names)
        {
            double[] values = new double[aud.Length];
            foreach (var name in names)
            {
                int age = FindAge(name);
                if (age < aud[0])
                    values[0]++;
                else if (age > aud[aud.Length- 1])
                    values[aud.Length- 1]++;
                else for (int i = 1; i < aud.Length; i++)
                    {
                        if (age >= aud[i - 1] && age < aud[i])
                            values[i]++;
                    }
            }
            return values;
        }
    }
}