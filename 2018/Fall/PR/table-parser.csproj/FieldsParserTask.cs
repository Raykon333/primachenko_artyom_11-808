using System;
using System.Collections.Generic;

namespace TableParser
{
	public class FieldsParserTask
	{
		// При решении этой задаче постарайтесь избежать создания методов, длиннее 10 строк.
		// Ниже есть метод ReadField — это подсказка. Найдите класс Token и изучите его.
		// Подумайте как можно использовать ReadField и Token в этой задаче.
		public static List<string> ParseLine(string line)
		{
            int startIndex = 0;
            List<string> output = new List<string>();
            while (startIndex < line.Length - 1)
            {
                Token token = ReadField(line, startIndex);
                output.Add(token.Value);
                startIndex = token.GetIndexNextToToken();
            }
			return output; // сокращенный синтаксис для инициализации коллекции.
		}

		
		private static Token ReadField(string line, int startIndex)
		{
            line = line.Remove(0, startIndex);
            line = line.Trim(' ');
            Tuple<int, int> indexes = GetIndexes(line);
            int min = Math.Min(indexes.Item1, indexes.Item2);
            if (min < line.Length && indexes.Item2 > 0)
                line = line.Remove(min);
            else if (min < line.Length)
            return new Token(line, startIndex, line.Length);
		}

        private static Tuple<int, int> GetIndexes(string line)
        {
            int indexOfSpace;
            int indexOfBracket;
            if (!line.Contains(" "))
                indexOfSpace = line.Length;
            else
                indexOfSpace = line.IndexOf(' ');
            if (!line.Contains("\""))
                indexOfBracket = line.Length;
            else
                indexOfBracket = line.IndexOfAny(new char[] { '"', '\'' });
            return new Tuple<int, int>(indexOfSpace, indexOfBracket);
        }

        /*private static string GetBracketedField(string line)
        {
            char mainBracket;
            if (line.Contains("\"") && line.Contains("'"))
                if (line)
        }*/
	}
}