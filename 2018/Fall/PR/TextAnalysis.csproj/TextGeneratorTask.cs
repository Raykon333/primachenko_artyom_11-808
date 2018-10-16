using System.Collections.Generic;

namespace TextAnalysis
{
    static class TextGeneratorTask
    {
        public static string ContinuePhrase(
            Dictionary<string, string> nextWords,
            string phraseBeginning,
            int wordsCount)
        {
            List<string> result = new List<string>(phraseBeginning.Split(' '));
            wordsCount += result.Count;
            while(wordsCount > result.Count)
            {
                string lastTwo = "";
                if (result.Count >= 2) //если слов два или больше, попробовать найти продолжение последних двух слов
                    lastTwo = result[result.Count - 2] + " " + result[result.Count - 1];
                if (nextWords.ContainsKey(lastTwo))
                    foreach (var word in nextWords[lastTwo].Split(' '))
                        result.Add(word);
                else if (nextWords.ContainsKey(result[result.Count - 1])) //попробовать найти продолжение последнего слова
                    result.Add(nextWords[result[result.Count - 1]]);
                else break; //если продолжений нет, закончить
            }
            phraseBeginning = result[0];
            for (int i = 1; i < result.Count; i++) //превратить список слов в строку
                phraseBeginning += " " + result[i];
            return phraseBeginning;
        }
    }
}