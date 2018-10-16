// Вставьте сюда финальное содержимое файла HistogramTask.cs
using System;
using System.Linq;

namespace Names//Количество участников в ВОВ по поколениям
{
    internal static class HistogramTask2
    {
        public static HistogramData GetBirthsPerDayHistogram2(NameData[] names, string name)
        {
            var veteranName = new string[4]; //три живых поколения
            veteranName[0] = /*.. - 1899*/ "Lost";
            veteranName[1] = /*1900 - 1919 */"Greatest";
            veteranName[2] = /*1920 - 1939*/ "Silent";
            veteranName[3] = /*1940 - 1945*/"Baby Boom";
            var countVeteran = new double[4];//массив количеств для каждого поколения
            int year = 0;//год, каждый ход изменяемый
            for (var i = 0; i < names.Length; i++)//проход по всем именам
            {
                //if (names[i].ToString().Substring(names[i].ToString().Length - name.Length, name.Length) == name)
                if (names[i].Name == name)
                {//если имя совпадает, увеличиваем количество соответствующего дня на 1
                    //year = int.Parse(names[i].ToString().Substring(6, 4));
                    year = names[i].BirthDate.Year;
                    if (year < 1900)//первое поколение
                        countVeteran[0]++;
                    else if (year < 1920)//второе поколение
                        countVeteran[1]++;
                    else if (year < 1940)//третье поколение
                        countVeteran[2]++;
                    else if (year < 1946)//четвертое поколение
                        countVeteran[3]++;
                }
            }
            return new HistogramData(string.Format("Рождаемость людей-участников в ВОВ '{0}'", name), veteranName , countVeteran);
        }
    }
}