using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace sem1
{
    class Axis
    {
        public readonly double[] Values;
        public readonly string Title;

        public Axis(double[] values, string title)
        {
            Values = values;
            Title = title;
        }
    }

    class LevitResult
    {
        public readonly int[] Distances;
        public readonly Stopwatch Time;
        public readonly int Iterations;

        public LevitResult(int[] distances, Stopwatch time, int iterations)
        {
            Distances = distances;
            Time = time;
            Iterations = iterations;
        }
    }

    class QueueByArray
    {
        private int[] Elements;
        private int Start = 0;
        private int AfterEnd = 0;
        public bool IsEmpty { get { return Start == AfterEnd; } }

        public QueueByArray(int length)
        {
            Elements = new int[length];
        }

        public void AddFirst(int item)
        {
            if (IsEmpty)
            {
                Elements[0] = item;
                AfterEnd++;
            }
            else
            {
                Start--;
                if (Start < 0)
                    Start = Elements.Length - 1;
                Elements[Start] = item;
            }
        }

        public void AddLast(int item)
        {
            Elements[AfterEnd] = item;
            AfterEnd++;
            if (AfterEnd >= Elements.Length)
                AfterEnd = 0;
        }

        public int Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException();
            int result = Elements[Start];
            Start++;
            if (Start >= Elements.Length)
                Start = 0;
            return result;
        }
    }

    class Program
    {
        static LevitResult Levit(int?[,] graph, int start)
        {
            #region
            if (graph.GetLength(0) != graph.GetLength(1) || start >= graph.GetLength(0) || start < 0)
                throw new InvalidDataException();
            int[] ids = new int[graph.GetLength(0)];
            int[] pathes = new int[graph.GetLength(0)];
            int iterations = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                if (i == start)
                {
                    ids[i] = 1;
                    pathes[i] = 0;
                }
                else
                {
                    ids[i] = 2;
                    pathes[i] = int.MaxValue;
                }
            }
            #endregion
            QueueByArray queue = new QueueByArray(graph.GetLength(0));
            QueueByArray urgentQueue = new QueueByArray(graph.GetLength(0));
            queue.AddFirst(start);
            while(!(queue.IsEmpty && urgentQueue.IsEmpty))
            {
                int vertex;
                if (urgentQueue.IsEmpty)
                    vertex = queue.Dequeue();
                else
                    vertex = urgentQueue.Dequeue();
                ids[vertex] = 0;
                for (int j = 0; j < graph.GetLength(0); j++)
                {
                    iterations++;
                    if (graph[vertex, j] == null || j == vertex)
                        continue;
                    if (pathes[j] > pathes[vertex] + (int)graph[vertex, j])
                    {
                        pathes[j] = pathes[vertex] + (int)graph[vertex, j];
                        if (ids[j] == 2)
                            queue.AddLast(j);
                        if (ids[j] == 0)
                            urgentQueue.AddLast(j);
                        ids[j] = 1;
                    }
                }
            }
            stopwatch.Stop();
            return new LevitResult(pathes, stopwatch, iterations);
        }

        static LevitResult SingleQueueLevit(int?[,] graph, int start)
        {
            #region
            if (graph.GetLength(0) != graph.GetLength(1) || start >= graph.GetLength(0) || start < 0)
                throw new InvalidDataException();
            int[] ids = new int[graph.GetLength(0)];
            int[] pathes = new int[graph.GetLength(0)];
            int iterations = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                if (i == start)
                {
                    ids[i] = 1;
                    pathes[i] = 0;
                }
                else
                {
                    ids[i] = 2;
                    pathes[i] = int.MaxValue;
                }
            }
            #endregion
            QueueByArray queue = new QueueByArray(graph.GetLength(0));
            queue.AddFirst(start);
            while (!queue.IsEmpty)
            {
                int vertex = queue.Dequeue();
                ids[vertex] = 0;
                for (int j = 0; j < graph.GetLength(0); j++)
                {
                    iterations++;
                    if (graph[vertex, j] == null || j == vertex)
                        continue;
                    if (pathes[j] > pathes[vertex] + (int)graph[vertex, j])
                    {
                        pathes[j] = pathes[vertex] + (int)graph[vertex, j];
                        if (ids[j] != 1)
                            queue.AddLast(j);
                        ids[j] = 1;
                    }
                }
            }
            stopwatch.Stop();
            return new LevitResult(pathes, stopwatch, iterations);
        }

        static List<int?[,]> GraphsByFile(string path)
        {
            List<int?[,]> graphs = new List<int?[,]>();
            using (StreamReader sr = File.OpenText(path))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    string[] data = s.Split(' ');
                    int graphLength = (1 + (int)Math.Sqrt(1 + 4 * (data.Length * 2 - 2))) / 2;
                    int?[,] graph = new int?[graphLength, graphLength];
                    for (int i = 0; i < graphLength; i++)
                    {
                        for (int j = 0; j <= i; j++)
                        {
                            int? rib = (data[i * (i + 1) / 2 + j] == "null") ? null : (int?)int.Parse(data[i * (i + 1) / 2 + j]);
                            graph[i, j] = rib;
                            if (i != j)
                                graph[j, i] = rib;
                        }
                    }
                    graphs.Add(graph);
                }
            }
            return graphs;
        }

        static void Main(string[] args)
        {
            FileGeneration.GenerateFile();
            List<int?[,]> graphs = GraphsByFile("data.txt");
            double[] xValues = new double[graphs.Count];
            double[] yTime = new double[graphs.Count];
            double[] ySQTime = new double[graphs.Count];
            double[] yIterations = new double[graphs.Count];
            double[] ySQIterations = new double[graphs.Count];
            double[] yExpected = new double[graphs.Count];
            double[] yReal = new double[graphs.Count];
            LevitResult[] results = new LevitResult[graphs.Count];
            LevitResult[] singleQueueResults = new LevitResult[graphs.Count];
            for (int i = 0; i < graphs.Count; i++)
            {
                xValues[i] = graphs[i].GetLength(0);
                results[i] = Levit(graphs[i], 0);
                singleQueueResults[i] = SingleQueueLevit(graphs[i], 0);
                yTime[i] = results[i].Time.ElapsedMilliseconds;
                ySQTime[i] = singleQueueResults[i].Time.ElapsedMilliseconds;
                yIterations[i] = results[i].Iterations;
                ySQIterations[i] = singleQueueResults[i].Iterations;
                yExpected[i] = graphs[i].GetLength(0) * (graphs[i].Length / 2);
                yReal[i] = Math.Log(graphs[i].GetLength(0)) * (graphs[i].Length / 2);
            }
            CreateChart("Expected Iterations", new Axis(xValues, "Vertex Amount"), new Axis(yIterations, "Levit Iterations"), new Axis(ySQIterations, "Single Queue Iterations"), new Axis(yExpected, "N * M"));
            CreateChart("Real Iterations", new Axis(xValues, "Vertex Amount"), new Axis(yIterations, "Iterations"), new Axis(ySQIterations, "Single Queue Iterations"), new Axis(yReal, "Log(N) * M"));
            CreateChart("Time in milliseconds", new Axis(xValues, "Vertex Amount"), new Axis(yTime, "Levit Time"), new Axis(ySQTime, "Single Queue Time"));
        }

        static void GraphToConsole(int?[,] graph)
        {
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < graph.GetLength(1); j++)
                    sb.Append((graph[i, j] == null ? "null" : graph[i, j].ToString()) + "\t");
                Console.WriteLine(sb);
            }
        }

        static void ArrayToConsole(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
                Console.Write(array[i] + " ");
        }

        static void CreateChart(string title, Axis xAxis, params Axis[] yAxes)
        {
            Random random = new Random(1);
            var chart = new ZedGraphControl
            {
                Dock = DockStyle.Fill
            };
            chart.GraphPane.Title.Text = title;
            chart.GraphPane.YAxis.Title.Text = "Y";
            chart.GraphPane.XAxis.Title.Text = xAxis.Title;
            foreach (var yAxis in yAxes)
            {
                chart.GraphPane.AddCurve(yAxis.Title, xAxis.Values, yAxis.Values, Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
            }
            chart.GraphPane.YAxis.Scale.MaxAuto = true;
            chart.GraphPane.YAxis.Scale.MinAuto = true;
            chart.GraphPane.XAxis.Type = AxisType.Text;
            string[] xLabels = Array.ConvertAll(xAxis.Values, x => x.ToString());
            chart.GraphPane.XAxis.Scale.TextLabels = xLabels;

            chart.AxisChange();
            var form = new Form
            {
                Text = title,
                Size = new Size(1600, 900)
            };
            form.Controls.Add(chart);
            form.ShowDialog();
        }
    }
}
