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
    static class NaturalCut
    {
        public static HashSet<Edge> MakeCuts(Graph G, int U,double alpha, double f)
        {
            HashSet<int> allNodes = new HashSet<int>();
            foreach (int id in G.nodes)
            {
                allNodes.Add(id);
            }
            
            HashSet<Edge> allCuts = new HashSet<Edge>();
            Random rnd = new Random();
            while (allNodes.Count > 0)
            {

                int rID = RandomID(allNodes,rnd);
                BFS bfs = new BFS(G, U, alpha, f, rID);
                Tuple<List<int>, List<Edge>> partitionAndCut = GetPartitionAndCut(G,bfs);

                //removes all nodes that are inside the cut
                partitionAndCut.Item1.ForEach(x => allNodes.Remove(x));

                //adds all cutEdges
                partitionAndCut.Item2.ForEach(x => allCuts.Add(x));
            }

            return allCuts;
        }
       
        private static int RandomID(HashSet<int> allNodes,Random rnd) {

            return allNodes.ToList()[rnd.Next(allNodes.Count)];
        }

        static public Tuple<List<int>, List<Edge>> GetPartitionAndCut(Graph OriginalG, BFS bfs)
        {
            Graph SubGraph = OriginalG.CreateSubGraphWithoutParent(bfs.SubGraph);
            SubGraph.ContractList(bfs.Core);
            SubGraph.ContractList(bfs.Ring);
            Tuple<List<int>, List<Edge>> result = SolveOnGraph(OriginalG, SubGraph, bfs);
            return new Tuple<List<int>, List<Edge>>(bfs.Core.Union(result.Item1).ToList(), result.Item2);
        }

        static private Tuple<List<int>, List<Edge>> SolveOnGraph(Graph OriginalG, Graph SubGraph, BFS bfs)
        {
            List<Edge> cut = new List<Edge>();
            Dictionary<int, Dictionary<int, int>> arcToIndex = new Dictionary<int, Dictionary<int, int>>();
            int index = 0;
            MaxFlow maxFlow = new MaxFlow();

            int s = bfs.Core.First();
            int t = bfs.Ring.First();
            //add arcs to Google OrTools
            foreach (int n in SubGraph.nodes)
            {
                var tuple = SubGraph.GetNeighboursFast(n);
                foreach (int fn in tuple.Item1)
                {
                    maxFlow.AddArcWithCapacity(n, fn, SubGraph.getWeight(n, fn));
                    if (!arcToIndex.ContainsKey(n))
                    {
                        arcToIndex[n] = new Dictionary<int, int>();
                    }
                    arcToIndex[n][fn] = index;
                    index++;
                }
                foreach (int fn in tuple.Item2)
                {
                    maxFlow.AddArcWithCapacity(n, fn, SubGraph.getWeight(n, fn));
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
                List<int> neighbours = SubGraph.GetNeighbours(id);

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
                            AddCut(id, fn, OriginalG, SubGraph, bfs, cut);
                        }
                    }
                }


            }

            return new Tuple<List<int>, List<Edge>>(result, cut);
        }

        static private List<Edge> AddCut(int id, int fn, Graph OriginalG, Graph G, BFS bfs, List<Edge> cut)
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

