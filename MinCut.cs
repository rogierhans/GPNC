using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;
using Google.OrTools.Graph;

namespace GPNC
{
    static class MinCut
    {

        static public Tuple<List<int>, List<Edge>> partition(Graph OriginalG, BFS bfs)
        {
            Graph G = OriginalG.CreateSubGraph(bfs.SubGraph);
            G.ContractList(bfs.Core);
            G.ContractList(bfs.Ring);
            Tuple<List<int>, List<Edge>> result = SolveOnGraph(OriginalG, G, bfs);
            return new Tuple<List<int>, List<Edge>>(bfs.Core.Union(result.Item1).ToList(), result.Item2);
        }

        static private Tuple<List<int>, List<Edge>> SolveOnGraph(Graph OriginalG, Graph G,BFS bfs)
        {
            List<Edge> cut = new List<Edge>();
            Dictionary<int, Dictionary<int, int>> arcToIndex = new Dictionary<int, Dictionary<int, int>>();
            int index = 0;
            MaxFlow maxFlow = new MaxFlow();

            int s = bfs.Core.First();
            int t = bfs.Ring.First();
            //add arcs to Google OrTools
            foreach (int n in G.nodes)
            {
                var tuple = G.GetNeighboursFast(n);
                foreach (int fn in tuple.Item1)
                {
                    maxFlow.AddArcWithCapacity(n, fn, G.getWeight(n, fn));
                    if (!arcToIndex.ContainsKey(n))
                    {
                        arcToIndex[n] = new Dictionary<int, int>();
                    }
                    arcToIndex[n][fn] = index;
                    index++;
                }
                foreach (int fn in tuple.Item2)
                {
                    maxFlow.AddArcWithCapacity(n, fn, G.getWeight(n, fn));
                    if (!arcToIndex.ContainsKey(n))
                    {
                        arcToIndex[n] = new Dictionary<int, int>();
                    }
                    arcToIndex[n][fn] = index;
                    index++;
                }
            }
            int solveStatus = maxFlow.Solve(s, t);
            Queue<int> queue = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();

            List<int> result = new List<int>();
            queue.Enqueue(s);
            visited.Add(s);
            result.Add(s);
            while (queue.Count > 0)
            {
                int id = queue.Dequeue();
                List<int> neighbours = G.GetNeighbours(id);

                foreach (int fn in neighbours)
                {
                    if (!visited.Contains(fn))
                    {
                        int i = arcToIndex[id][fn];
                        long flow = maxFlow.Flow(i);
                        long capacity = maxFlow.Capacity(i);
                        if (capacity - flow > 0)
                        {
                            queue.Enqueue(fn);
                            visited.Add(fn);
                            result.Add(fn);
                        }
                        else if (maxFlow.Flow(arcToIndex[fn][id]) > 0)
                        {
                            queue.Enqueue(fn);
                            visited.Add(fn);
                            result.Add(fn);
                        }
                        else
                        {
                            AddCut(id, fn, OriginalG, G,bfs,cut);
                        }
                    }
                }


            }

            return new Tuple<List<int>, List<Edge>> (result,cut);
        }

        static private List<Edge> AddCut(int id, int fn, Graph OriginalG, Graph G, BFS bfs , List<Edge> cut)
        {
            int s = bfs.Core.First();
            int t = bfs.Ring.First();

            if (id == s)
            {
                foreach (int coreElement in bfs.Core)
                {
                    if (OriginalG.IsEdge(coreElement, fn))
                    {
                        Edge e = new Edge(coreElement, fn);
                        cut.Add(e);
                    }
                }
            }

            else if (fn == t)
            {
                foreach (int ringElement in bfs.Ring)
                {
                    if (OriginalG.IsEdge(ringElement, fn))
                    {
                        Edge e = new Edge(ringElement, fn);
                        cut.Add(e);
                    }
                }
            }
            else
            {
                Edge e = new Edge(id, fn);
                cut.Add(e);
            }
            return cut;
        }
    }
}