using System;
using System.Text;
using System.IO;

namespace sem1
{
    class FileGeneration
    {
        public static void GenerateFile()
        {
            Random random = new Random(1488);
            int dataAmount = random.Next(50, 100);
            using (StreamWriter sw = File.CreateText(@"data.txt"))
                for (int i = 1; i <= dataAmount; i++)
                {
                    int dataLength = (10 * i) * (10 * i + 1) / 2;
                    StringBuilder graph = new StringBuilder();
                    graph.Append(random.Next(0, 2) == 0 ? "null" : random.Next().ToString());
                    for (int j = 1; j < dataLength; j++)
                        graph.Append(" " + (random.Next(0, 2) == 0 ? "null" : random.Next(0, 1001).ToString()));
                        sw.WriteLine(graph);
                }
        }
    }
}
