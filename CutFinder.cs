using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.Graph;

namespace GPNC
{
    class CutFinder
    {
        public List<int> Partition;
        public List<Edge> Cut;



        private Graph OriginalG;
        private BFS bfs;
        private Dictionary<int, Dictionary<int, int>> arcToIndex;
        private int index;
        private MaxFlow maxFlow;
        private HashSet<int> hashCore;
        private HashSet<int> hashRing;
        public void SolveOnGraph(Graph G, BFS bfs)
        {
            OriginalG = G;
            this.bfs = bfs;
            hashCore = new HashSet<int>(bfs.Core);
            hashRing = new HashSet<int>(bfs.Ring);
            maxFlow = new MaxFlow();
            Graph SubGraph = OriginalG.CreateSubGraphWithoutParent(bfs.SubGraph);
            SubGraph.ContractList(bfs.Core);
            SubGraph.ContractList(bfs.Ring);
            Partition = new List<int>();
            Cut = new List<Edge>();
            index = 0;
            arcToIndex = new Dictionary<int, Dictionary<int, int>>();


            int s = bfs.Core.First();
            int t = bfs.Ring.First();
            //add arcs to Google OrTools
            foreach (int n in bfs.SubGraph)
            {
                var neighbours = SubGraph.GetNeighbours(n);
                foreach (int fn in neighbours)
                {
                    AddToMaxFlow(n, fn, SubGraph);
                }
            }
            int solveStatus = maxFlow.Solve(s, t);
            Queue<int> queue = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();

            queue.Enqueue(s);
            visited.Add(s);
            Partition.Add(s);


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
                            Partition.Add(fn);
                        }
                        else if (maxFlow.Flow(arcToIndex[fn][id]) > 0)
                        {
                            queue.Enqueue(fn);
                            visited.Add(fn);
                            Partition.Add(fn);
                        }
                        else
                        {
                            AddCut(id, fn);
                        }
                    }
                }


            }
        }
        private void AddToMaxFlow(int n, int fn, Graph SubGraph)
        {
            maxFlow.AddArcWithCapacity(n, fn, SubGraph.getWeight(n, fn));
            if (!arcToIndex.ContainsKey(n))
            {
                arcToIndex[n] = new Dictionary<int, int>();
            }
            arcToIndex[n][fn] = index;
            index++;
        }

        private void AddCut(int id, int fn)
        {
            int s = bfs.Core.First();
            int t = bfs.Ring.First();

            if (id == s)
            {
                foreach (int potentialNeighbour in OriginalG.GetNeighbours(fn))
                {
                    if (hashCore.Contains(potentialNeighbour))
                    {
                        Edge e = new Edge(potentialNeighbour, fn);
                        Cut.Add(e);
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
                        Cut.Add(e);
                    }
                }
            }
            else
            {
                Edge e = new Edge(id, fn);
                Cut.Add(e);
            }
        }
    }
}
