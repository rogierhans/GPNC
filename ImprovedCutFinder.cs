using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.Graph;
namespace GPNC
{
    class ImprovedCutFinder
    {
        Dictionary<int, NodePoint> Nodes;
        Graph G;
        int U;
        double Alpha;
        double F;

        public ImprovedCutFinder(Dictionary<int, NodePoint> nodes, Graph g, int u, double alpha, double f)
        {
            Nodes = nodes;
            G = g;
            U = u;
            Alpha = alpha;
            F = f;
        }

        Dictionary<int, Dictionary<int, int>> arcToIndex = new Dictionary<int, Dictionary<int, int>>();
        MaxFlow maxFlow;
        List<int> partition;
        List<Edge> cut;
        int index;
        HashSet<int> visited;
        Queue<int> queue;
        int size;
        readonly int source = int.MaxValue;
        readonly int sink = int.MinValue;
        public Tuple<List<int>, List<Edge>> FindCut(int startNode)
        {
            partition = new List<int>();
            cut = new List<Edge>();
            arcToIndex = new Dictionary<int, Dictionary<int, int>>();
            maxFlow = new MaxFlow();
            index = 0;
            visited = new HashSet<int>();
            queue = new Queue<int>();
            size = 0;


            //phase 1 grow core until size U/f
            //CoreRing = edges that needs to be added extra edge 
            Visit(startNode);
            GrowCore();
            AddCoreRing();

            //phase 2 grow and add to maxflow all neigbours allready in Subgraph unti size U * alpha
            GrowToRing();

            //phase 3 add ring
            AddRing();

            maxFlow.Solve(source, sink);

            GetPartionAndCut();

            return new Tuple<List<int>, List<Edge>>(partition, cut);
        }

        private void GetPartionAndCut()
        {
            Queue<int> queue = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();

            queue.Enqueue(source);
            visited.Add(source);
            partition.Add(source);
            while (queue.Count > 0)
            {
                int currentNode = queue.Dequeue();
                var tuple = G.GetNeighboursFast(currentNode);

                foreach (int neighbour in tuple.Item1.Concat(tuple.Item2))
                {
                    if (!visited.Contains(neighbour))
                    {
                        int i = arcToIndex[currentNode][neighbour];
                        long flow = maxFlow.Flow(i);
                        long capacity = maxFlow.Capacity(i);
                        if (capacity - flow > 0)
                        {
                            queue.Enqueue(neighbour);
                            visited.Add(neighbour);
                            partition.Add(neighbour);
                        }
                        else if (maxFlow.Flow(arcToIndex[neighbour][currentNode]) > 0)
                        {
                            queue.Enqueue(neighbour);
                            visited.Add(neighbour);
                            partition.Add(neighbour);
                        }
                        else
                        {
                            cut.Add(new Edge(neighbour, currentNode));
                        }
                    }
                }
            }
        }

        private void AddRing()
        {
            foreach (int ringElement in queue)
            {
                var tuple = G.GetNeighboursFast(ringElement);
                foreach (int neighbour in tuple.Item1.Concat(tuple.Item2))
                {
                    if (visited.Contains(neighbour))
                    {
                        AddArc(neighbour,sink, int.MaxValue);
                    }
                }
            }
        }
        private void AddCoreRing()
        {
            foreach (int coreRingElement in queue)
            {
                var tuple = G.GetNeighboursFast(coreRingElement);
                foreach (int neighbour in tuple.Item1.Concat(tuple.Item2))
                {
                    if (visited.Contains(neighbour))
                    {
                        //AddArc(neighbour,coreRingElement,G.getWeight(neighbour,coreRingElement));
                        AddArc(source, neighbour, int.MaxValue);
                    }
                }
            }
        }

        private void Visit(int node)
        {
            queue.Enqueue(node);
            visited.Add(node);
            size += G.Size[node];
        }

        private void GrowCore()
        {
            double maxSizeCore = U / F;
            while (size < maxSizeCore)
            {
                int currentNode = queue.Dequeue();
                var tuple = G.GetNeighboursFast(currentNode);
                foreach (int neighbour in tuple.Item1.Concat(tuple.Item2))
                {
                    if (!visited.Contains(neighbour))
                    {
                        Visit(neighbour);
                    }

                }
            }
        }

        private void GrowToRing()
        {
            double maxSizeSubGraph = U * Alpha;
            while (size < maxSizeSubGraph)
            {
                int currentNode = queue.Dequeue();
                var tuple = G.GetNeighboursFast(currentNode);
                foreach (int neighbour in tuple.Item1.Concat(tuple.Item2))
                {
                    if (!visited.Contains(neighbour))
                    {
                        Visit(neighbour);
                    }
                    else
                    {
                        AddArc(neighbour, currentNode, G.getWeight(neighbour, currentNode));
                        AddArc(currentNode, neighbour, G.getWeight(neighbour, currentNode));
                    }
                }
            }
        }

        public void AddArc(int n, int fn, int weight)
        {
            maxFlow.AddArcWithCapacity(n, fn, weight);
            if (!arcToIndex.ContainsKey(n))
            {
                arcToIndex[n] = new Dictionary<int, int>();
            }
            arcToIndex[n][fn] = index++;
        }
    }
}
