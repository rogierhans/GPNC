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
        public List<int> partition;
        public MinCut(Graph G, List<int> core, List<int> ring) {
            G.ContractList(core);
            G.ContractList(ring);
            List<int> result = SolveOnGraph(G, core.First(), ring.First());
            partition = result.Union(core).ToList();
        }

        private List<int> SolveOnGraph(Graph G, int s, int t)
        {
            List<Arc> arcs = new List<Arc>();
            Dictionary<int, Dictionary<int, int>> arcToIndex = new Dictionary<int, Dictionary<int, int>>();
            int numNodes = 0;
            int numArcs = 0;
            int index = 0;
            foreach (int n in G.nodes)
            {
                HashSet<int> fromNodes = G.AllFromNodes[n];
                if (fromNodes.Count > 0) numNodes++;
                foreach (int fn in fromNodes)
                {
                    Arc arc = new Arc();
                    arc.from = n;
                    arc.to = fn;
                    arc.capacity = (int)G.AllWeights[n][fn];
                    numArcs++;
                    arcs.Add(arc);

                    if (!arcToIndex.ContainsKey(n))
                    {
                        arcToIndex[n] = new Dictionary<int, int>();
                    }
                    arcToIndex[n][fn] = index;
                    index++;
                }
            }
            Console.WriteLine("done with arcs");
            int[] start_nodes = new int[arcs.Count];
            int[] end_nodes = new int[arcs.Count];
            int[] capacities = new int[arcs.Count];
            for (int i = 0; i < arcs.Count; i++)
            {
                start_nodes[i] = arcs[i].from;
                end_nodes[i] = arcs[i].to;
                capacities[i] = arcs[i].capacity;

            }

            MaxFlow maxflow = SolveMaxFlow(s, t, numNodes, numArcs, start_nodes, end_nodes, capacities);
            Queue<int> queue = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();

            List<int> result = new List<int>();
            queue.Enqueue(s);
            visited.Add(s);
            result.Add(s);
            while (queue.Count > 0)
            {
                int id = queue.Dequeue();
                HashSet<int> fromNode = G.AllFromNodes[id];

                foreach (int fn in fromNode)
                {
                    if (!visited.Contains(fn))
                    {
                        int i = arcToIndex[id][fn];
                        long flow = maxflow.Flow(i);
                        long capacity = maxflow.Capacity(i);
                        if (capacity - flow > 0)
                        {
                            queue.Enqueue(fn);
                            visited.Add(fn);
                            result.Add(fn);
                        }
                    }
                }
                HashSet<int> toNode = G.AllToNodes[id];
                foreach (int tn in toNode)
                {
                    if (!visited.Contains(tn))
                    {
                        int i = arcToIndex[tn][id];
                        long flow = maxflow.Flow(i);
                        if (flow > 0)
                        {
                            queue.Enqueue(tn);
                            visited.Add(tn);
                            result.Add(tn);
                        }
                    }
                }


            }
            return result;
        }


        private static MaxFlow SolveMaxFlow(int s, int t, int numNodes, int numArcs, int[] start_nodes, int[] end_nodes, int[] capacities)
        {
            // Define three parallel arrays: start_nodes, end_nodes, and the capacities
            // between each pair. For instance, the arc from node 0 to node 1 has a
            // capacity of 20.
            // From Taha's 'Introduction to Operations Research',
            // example 6.4-2.

            // Instantiate a SimpleMaxFlow solver.
            MaxFlow maxFlow = new MaxFlow();

            // Add each arc.
            for (int i = 0; i < numArcs; ++i)
            {
                int arc = maxFlow.AddArcWithCapacity(start_nodes[i], end_nodes[i],
                                                     capacities[i]);
                if (arc != i) throw new Exception("Internal error");
            }
            int source = s;
            int sink = t;


            // Find the maximum flow between node 0 and node 4.
            int solveStatus = maxFlow.Solve(source, sink);
            if (solveStatus == MaxFlow.OPTIMAL)
            {
            }
            else
            {
                Console.WriteLine("Solving the max flow problem failed. Solver status: " +
                                  solveStatus);
            }
            return maxFlow;
        }


    }
}
