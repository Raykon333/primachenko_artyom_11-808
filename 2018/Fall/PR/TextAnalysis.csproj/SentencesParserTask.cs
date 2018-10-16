using System.Collections.Generic;

namespace TextAnalysis
{
    static class SentencesParserTask
    {
        public static List<List<string>> SentencesList = new List<List<string>>();
        public static int Y = 0;
        public static string Word = "";
        public static char[] PunctMarks = new char[] { '.', '!', '?', ';', ':', '(', ')' };

        public static List<List<string>> ParseSentences(string text)
        {
            SentencesList = new List<List<string>>();
            Y = 0;
            Word = "";
            SentencesList.Add(new List<string>());
            for (int i = 0; i < text.Length; i++)
                AnalyzeChar(text[i]);
            AddWordIfNotEmpty(); //добавить последнее слово, если в конце предложения не было знака препинания
            if (SentencesList.Count == 1 && SentencesList[0].Count == 0) 
                //очистить список, если он содержит один пустой элемент
                SentencesList.RemoveAt(0);
            return SentencesList;
        }

        //если слово непусто, добавляется в список и обнуляется
        public static void AddWordIfNotEmpty()
        {
            if (Word != "")
            {
                SentencesList[Y].Add(Word.ToLower());
                Word = "";
            }
        }

        //проверить символ в тексте
        public static void AnalyzeChar(char x)
        {
            if (char.IsLetter(x) || x == '\'') //строится слово, если символ - буква или апостроф
                Word += x;
            else
            {
                AddWordIfNotEmpty();
                for (int k = 0; k < PunctMarks.Length; k++) //если символ - знак конца предложения, создаётся новый список слов и начинается работа с ним
                    if (x == PunctMarks[k] && SentencesList[Y].Count > 0)
                    {
                        Y++;
                        SentencesList.Add(new List<string>());
                    }
            }
        }
    }
}