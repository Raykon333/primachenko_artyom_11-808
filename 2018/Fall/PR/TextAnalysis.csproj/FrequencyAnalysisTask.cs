using System.Collections.Generic;

namespace TextAnalysis
{
    static class FrequencyAnalysisTask
    {
        public static Dictionary<string, string> GetMostFrequentNextWords(List<List<string>> text)
        {
            var result = new Dictionary<string, Dictionary<string, int>>();
            int[] ns = new int[] { 2, 3 }; //размерности N-грамм
            foreach (var sent in text)
            {
                foreach (var n in ns)
                {
                    var sentNgramms = GetNgrammsOfSentence(sent, n);
                    result = UniteResult(result, sentNgramms);
                }
            }
            return ChooseNgrammsEndings(result);
        }

        //найти все N-граммы предложения sent
        public static Dictionary<string, Dictionary<string, int>> GetNgrammsOfSentence(List<string> sent, int n)
        {
            var allNgramms = new Dictionary<string, Dictionary<string, int>>(); //словарь с ключами - началами N-грамм и значениями-словарями с ключами - концами N-грамм и их частотой
            for (int i = 0; i < sent.Count - n + 1; i++)
            {
                var ngrammStart = sent[i];
                for (int j = 1; j < n - 1; j++) //если N-грамма состоит из трёх или больше слов
                    ngrammStart += " " + sent[i + j];
                if (!allNgramms.ContainsKey(ngrammStart)) //если в словаре нет начала N-граммы
                    allNgramms[ngrammStart] = new Dictionary<string, int>();
                if (!allNgramms[ngrammStart].ContainsKey(sent[i + n - 1])) //если в словаре есть начало, но нет конца N-граммы
                    allNgramms[ngrammStart][sent[i + n - 1]] = 0;
                allNgramms[ngrammStart][sent[i + n - 1]]++;
            }
            return allNgramms;
        }

        //из словаря всех окончаний N-грамм выбрать наиболее подходящий по условию
        public static Dictionary<string, string> ChooseNgrammsEndings(Dictionary<string, Dictionary<string, int>> allNgrams)
        {
            int maxCount;
            string fitEnd;
            var result = new Dictionary<string, string>();
            foreach (var nStart in allNgrams)
            {
                maxCount = 0;
                fitEnd = "";
                foreach (var nEnd in nStart.Value)
                {
                    if (nEnd.Value > maxCount //если конец Н-граммы встречается чаще
                        || (nEnd.Value == maxCount //или также часто, но он лексиграфически меньше
                        && string.CompareOrdinal(nEnd.Key, fitEnd) < 0))
                    {
                        maxCount = nEnd.Value;
                        fitEnd = nEnd.Key;
                    }
                }
                result[nStart.Key] = fitEnd; //каждому началу приравнивается соответствующий конец
            }
            return result;
        }

        //объединить результат с новополученным словарём
        public static Dictionary<string, Dictionary<string, int>> UniteResult(Dictionary<string, Dictionary<string, int>> result,
            Dictionary<string, Dictionary<string, int>> sentNgramms)
        {
            foreach (var sentNgramm in sentNgramms)
            {
                if (!result.ContainsKey(sentNgramm.Key)) //если в словаре нет начала N-граммы
                    result[sentNgramm.Key] = sentNgramm.Value;
                else foreach (var sentNgrammEnd in sentNgramm.Value)
                    {
                        if (!result[sentNgramm.Key].ContainsKey(sentNgrammEnd.Key)) //если в словаре есть начало, но нет конца N-граммы
                            result[sentNgramm.Key][sentNgrammEnd.Key] = 0;
                        result[sentNgramm.Key][sentNgrammEnd.Key] += sentNgrammEnd.Value;
                    }

            }
            return result;
        }
    }
}