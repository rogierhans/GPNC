using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.Graph;

namespace GPNC
{
    class MinCut
    {
        public List<Edge> cut = new List<Edge>();
        public List<int> partition(Graph G, List<int> core, List<int> ring)
        {
            G.ContractList(core);
            G.ContractList(ring);
            List<int> result = SolveOnGraph(G, core.First(), ring.First());
            return result.Union(core).ToList();
        }

        private List<int> SolveOnGraph(Graph G, int s, int t)
        {
            Dictionary<int, Dictionary<int, int>> arcToIndex = new Dictionary<int, Dictionary<int, int>>();
            int index = 0;
            MaxFlow maxFlow = new MaxFlow();
            foreach (int n in G.nodes)
            {
                var fromNodes = G.AllWeights[n].Keys;
                foreach (int fn in fromNodes)
                {
                    maxFlow.AddArcWithCapacity(n, fn, (int)G.AllWeights[n][fn]);
                    maxFlow.AddArcWithCapacity(fn, n, (int)G.AllWeights[n][fn]);
                    if (!arcToIndex.ContainsKey(n))
                    {
                        arcToIndex[n] = new Dictionary<int, int>();
                    }
                    arcToIndex[n][fn] = index;
                    index++;
                    if (!arcToIndex.ContainsKey(fn))
                    {
                        arcToIndex[fn] = new Dictionary<int, int>();
                    }
                    arcToIndex[fn][n] = index;
                    index++;
                }
            }
            int solveStatus = maxFlow.Solve(s, t);
            Console.WriteLine("done with arcs");
            Console.WriteLine("done with flow");
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
                        else {
                            Edge e = new Edge(id,fn);
                            cut.Add(e);
                        }
                    }
                }


            }
            Console.WriteLine("done with partitioning");
            return result;
        }


    }
}
