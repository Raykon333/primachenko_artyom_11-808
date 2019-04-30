using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Kosaraju
{
    class VertexInfo
    {
        public readonly int Vertex;
        public readonly long ExitTime;
        public readonly int Tree;

        public VertexInfo(int vertex, long exitTime, int tree)
        {
            Vertex = vertex;
            ExitTime = exitTime;
            Tree = tree;
        }
    }

    class Graph
    {
        private readonly HashSet<int>[] AdjacencyList;
        public int VertexCount => AdjacencyList.Length;

        public Graph(HashSet<int>[] adjacencyList)
        {
            AdjacencyList = adjacencyList;
        }

        //возвращает все вершины, в которые существует дуга из vertex
        public IEnumerable<int> GetAllAdjacentTo(int vertex)
        {
            foreach (var endVertex in AdjacencyList[vertex])
                yield return endVertex;
        }

        //возвращает инвертированный граф
        public Graph GetInversedGraph()
        {
            HashSet<int>[] inversedAdjacency = new HashSet<int>[VertexCount];
            for (int i = 0; i < VertexCount; i++)
                inversedAdjacency[i] = new HashSet<int>();

            for (int startV = 0; startV < VertexCount; startV++)
                foreach (var endV in AdjacencyList[startV])
                    inversedAdjacency[endV].Add(startV);
            return new Graph(inversedAdjacency);
        }

        //выполняет поиск в глубину, выбирая вершину каждого нового дерева по правилу vertexSelection
        public IEnumerable<VertexInfo> DepthFirstSearch(Predicate<int> vertexSelection)
        {
            bool[] visited = new bool[VertexCount];
            int[] vertexes = new int[VertexCount];
            for (int i = 0; i < VertexCount; i++)
                vertexes[i] = i;
            int tree = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (visited.Contains(false))
            {
                foreach (var info in RecursiveDFS(vertexes.First(x => vertexSelection(x)), visited, stopwatch))
                    yield return new VertexInfo(info.Item1, info.Item2, tree);
                tree++;
            }
        }

        //рекурсивный алгоритм поиска дерева DFS
        private IEnumerable<Tuple<int, long>> RecursiveDFS(int vertex, bool[] visited, Stopwatch stopwatch)
        {
            visited[vertex] = true;
            foreach (var newVertex in GetAllAdjacentTo(vertex))
                if (!visited[newVertex])
                    foreach (var nextVertex in RecursiveDFS(newVertex, visited, stopwatch))
                        yield return nextVertex;
            yield return Tuple.Create(vertex, stopwatch.ElapsedTicks);
        }
    }
}
