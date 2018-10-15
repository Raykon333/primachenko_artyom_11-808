using System.Collections.Generic;

namespace TextAnalysis
{
    static class SentencesParserTask
    {
        public static List<List<string>> ParseSentences(string text)
        {
            var sentencesList = new List<List<string>>();
            string word = "";
            char[] punctMarks = new char[] { '.', '!', '?', ';', ':', '(', ')' };
            int y = 0;
            sentencesList.Add(new List<string>());
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsLetter(text[i]) || text[i] == '\'')
                    word += text[i];
                else
                {
                    if (word != "")
                    {
                        sentencesList[y].Add(word.ToLower());
                        word = "";
                    }
                    for (int k = 0; k < punctMarks.Length; k++)
                        if (text[i] == punctMarks[k] && sentencesList[y].Count > 0)
                        {
                            y++;
                            sentencesList.Add(new List<string>());
                        }
                }
            }
            if (word != "")
            {
                sentencesList[y].Add(word.ToLower());
                word = "";
            }
            if (sentencesList.Count == 1 && sentencesList[0].Count == 0)
                sentencesList.RemoveAt(0);
            return sentencesList;
        }
    }
}