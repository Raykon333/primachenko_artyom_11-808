using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kosaraju
{
    class Kosaraju
    {
        public static List<HashSet<int>> FindStronglyConnectedComponents(Graph graph)
        {
            List<HashSet<int>> result = new List<HashSet<int>>();
            bool[] visited = new bool[graph.VertexCount];
            VertexInfo[] firstSearch = new VertexInfo[graph.VertexCount];

            //первый поиск в глубину, для нахождения времени выхода из каждой вершины
            foreach (var vertexInfo in graph.DepthFirstSearch(new Predicate<int>(vertex => visited[vertex] == false)))
            {
                visited[vertexInfo.Vertex] = true;
                firstSearch[vertexInfo.Vertex] = vertexInfo;
            }

            Graph inversedGraph = graph.GetInversedGraph();
            visited = new bool[graph.VertexCount];

            //второй поиск в глубину, приоритет выбора вершин - наибольшее время выхода
            foreach (var vertexInfo in inversedGraph.DepthFirstSearch(new Predicate<int>(vertex =>
            firstSearch.OrderByDescending(info => info.ExitTime).First(info => visited[info.Vertex] == false).Vertex == vertex)))
            {
                visited[vertexInfo.Vertex] = true;
                while (vertexInfo.Tree >= result.Count)
                    result.Add(new HashSet<int>());
                result[vertexInfo.Tree].Add(vertexInfo.Vertex);
            }

            return result;
        }

        private static Graph TestGraph()
        {
            HashSet<int>[] graph = new HashSet<int>[8];
            graph[0] = new HashSet<int> { 1 };
            graph[1] = new HashSet<int> { 2, 3 };
            graph[2] = new HashSet<int> { 0 };
            graph[3] = new HashSet<int> { 4 };
            graph[4] = new HashSet<int> { 5 };
            graph[5] = new HashSet<int> { 3 };
            graph[6] = new HashSet<int> { 7 };
            graph[7] = new HashSet<int> { 6 };
            return new Graph(graph);
        }

        static void Main()
        {
            var components = FindStronglyConnectedComponents(TestGraph());
            foreach (var component in components)
            {
                StringBuilder line = new StringBuilder();
                foreach (var vertex in component)
                    line.Append(vertex + " ");
                Console.WriteLine(line);
            }
        }
    }
}
