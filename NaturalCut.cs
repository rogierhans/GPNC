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
using System.Threading;

namespace GPNC
{
    class NaturalCut
    {
        Graph G;
        int U;
        double Alpha;
        double F;
        bool RemoveCore;
        List<int> OrdedNodes;
        public NaturalCut(Graph g, int u, double alpha, double f, bool removeCore)
        {
            G = g;
            U = u;
            Alpha = alpha;
            F = f;
            RemoveCore = removeCore;
        }
        public NaturalCut(Graph g, int u, double alpha, double f, bool removeCore, List<int> ordedNodes)
        {
            G = g;
            U = u;
            Alpha = alpha;
            F = f;
            RemoveCore = removeCore;
            OrdedNodes = ordedNodes;
        }


        private BFS bfs;
        private List<Edge> cut;
        private List<int> result;
        private int s;
        private int t;
        public HashSet<Edge> MakeCuts(Dictionary<int, GeoPoint> nodes)
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
                int startNodeForBFS;
                if (OrdedNodes == null)
                {
                    startNodeForBFS = RandomID(allNodes, rnd);
                }
                else {
                    startNodeForBFS = GetMostDenseNode(allNodes);
                }


                bfs = new BFS(G, U, Alpha, F, startNodeForBFS);
                SolveOnGraph();
                bfs.Core.ForEach(y => allNodes.Remove(y));
                if (!RemoveCore)
                {
                    result.ForEach(y => allNodes.Remove(y));
                }

                cut.ForEach(y => allCuts.Add(y));
                //HashSet<int> parpar = new HashSet<int>();
                //partitionAndCut.Item1.ForEach(e => parpar.Add(e));
                //Print.PrintCutFound(nodes, G.CreateSubGraphWithoutParent(bfs.SubGraph), bfs.SubGraph, partitionAndCut.Item1, parpar, bfs.Core, allNodes.Count.ToString());
                //Console.WriteLine(allNodes.Count + "left");
            }

            return allCuts;
        }

        private int index = 0;
        private int GetMostDenseNode(HashSet<int> allNodes) {
            int rID = -1;
            while (rID == -1)
            {
                if (allNodes.Contains(OrdedNodes[index]))
                {
                    rID = OrdedNodes[index];
                }
                else
                {
                    index++;
                }
            }
            return rID;
        }

        private int RandomID(HashSet<int> allNodes, Random rnd)
        {

            return allNodes.ToList()[rnd.Next(allNodes.Count)];
        }


        private void SolveOnGraph()
        {
            Graph SubGraph = G.CreateSubGraphWithoutParent(bfs.SubGraph);
            SubGraph.ContractList(bfs.Core);
            SubGraph.ContractList(bfs.Ring);



            MaxFlow maxFlow = new MaxFlow();


            s = bfs.Core.First();
            t = bfs.Ring.First();
            //add arcs to Google OrTools
            var arcToIndex = AddArcsToMaxFlow(SubGraph, maxFlow);
            int solveStatus = maxFlow.Solve(s, t);

            ExtractCutAndPartition(SubGraph, maxFlow, arcToIndex);
        }


        private Dictionary<int, Dictionary<int, int>> AddArcsToMaxFlow(Graph SubGraph, MaxFlow maxFlow)
        {
            int index = 0;
            Dictionary<int, Dictionary<int, int>> arcToIndex = new Dictionary<int, Dictionary<int, int>>();
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
            return arcToIndex;
        }


        private void ExtractCutAndPartition(Graph SubGraph, MaxFlow maxFlow, Dictionary<int, Dictionary<int, int>> arcToIndex)
        {
            Queue<int> queue = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();
            cut = new List<Edge>();
            result = new List<int>();
            queue.Enqueue(s);
            visited.Add(s);
            result.Add(s);

            HashSet<int> hashCore = new HashSet<int>(bfs.Core);
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
                            AddCut(id, fn, hashCore);
                        }
                    }
                }


            }

        }

        private void AddCut(int id, int fn, HashSet<int> hashCore)
        {
            int s = bfs.Core.First();
            int t = bfs.Ring.First();

            if (id == s)
            {
                foreach (int potentialNeighbour in G.GetNeighbours(fn))
                {
                    if (hashCore.Contains(potentialNeighbour))
                    {
                        Edge e = new Edge(potentialNeighbour, fn);
                        cut.Add(e);
                    }
                }
            }

            else if (fn == t)
            {
                foreach (int ringElement in bfs.Ring)
                {
                    if (G.IsEdge(ringElement, fn))
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
        }
    }


}

