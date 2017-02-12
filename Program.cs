using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Google.OrTools.Graph;

namespace GPNC
{
    class Program
    {
        static void Main(string[] args)
        {


            Graph G = Parser.ParseCSVFile();
            //Graph G = new Graph();
            //for (int i = 0; i < 10; i++)
            //{
            //    G.AddNodeToGraph(i);
            //}
            //for (int i = 0; i < 9; i++)
            //{

            //    //misschien hier iets aan doen 
            //    G.AddFromEgde(i, i + 1, 20 - (int)i);
            //    G.AddToEgde(i + 1, i);
            //}
            //G.print();
            //Console.ReadLine();
            //G.ContractList(new List<int> { 4, 5, 8 });
            //G.print();
            //Console.ReadLine();
            //G.ContractList(new List<int> { 9, 6 });
            //G.print();
            //Console.ReadLine();
            //Graph G2 = G.CreateSubGraph(new List<int> { 1,2,3});

            //List<int> test = G.nodes.Take(100000).ToList();
            //G.ContractList(test);
            //Graph G2 = G.CreateSubGraph(G.nodes.Take(10000).ToList());
            //G2.print();
            BFS bfs = new BFS(G, 10000, 1, 10, 1);
            //Dictionary<int, NodePoint> nodes = Parser.ParseNodes();
            //test(nodes, bfs.T);
            Graph G2 = G.CreateSubGraph(bfs.SubGraph);

            Console.WriteLine("Oki");
            G2.ContractList(bfs.Core);
            G2.ContractList(bfs.Ring);
            SolveOnGraph(G2, bfs.Core.First(), bfs.Ring.First());
            Console.ReadLine();
        }

        public static void exportIds(List<int> ids, string nameFile)
        {
            String[] strings = new String[ids.Count];

            for (int i = 0; i < ids.Count; i++)
            {
                strings[i] = ids[i].ToString();
            }
            System.IO.File.WriteAllLines("F:\\Users\\Rogier\\Desktop\\" + nameFile + ".csv", strings);
        }
        public static void test(Dictionary<int, NodePoint> nodes, List<int> specialNodes)
        {

            using (var bmp = new Bitmap(1000, 1000))
            using (var gr = Graphics.FromImage(bmp))
            {
                //gr.FillRectangle(Brushes.Orange, new Rectangle(0, 0, bmp.Width, bmp.Height));
                foreach (NodePoint np in nodes.Values)
                {
                    gr.FillRectangle(Brushes.Red, np.x, np.y, 1, 1);
                }
                specialNodes.ForEach(n =>
                {
                    NodePoint np = nodes[n];
                    gr.FillRectangle(Brushes.Yellow, np.x, np.y, 1, 1);
                });

                var path = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "Example.png");
                bmp.Save(path);
            }
        }
        private static void SolveOnGraph(Graph G, int s, int t)
        {
            List<Arc> arcs = new List<Arc>();
            int numNodes = 0;
            int numArcs = 0;
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

            SolveMaxFlow(s, t,numNodes,numArcs,start_nodes,end_nodes,capacities);
        }

        public struct Arc
        {
            public int from;
            public int to;
            public int capacity;
        }





        private static void SolveMaxFlow(int s, int t, int numNodes, int numArcs, int[] start_nodes, int[] end_nodes, int[] capacities)
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

            Console.WriteLine("Solving max flow with " + numNodes + " nodes, and " +
                              numArcs + " arcs, source=" + source + ", sink=" + sink);
            Console.ReadLine();
            // Find the maximum flow between node 0 and node 4.
            int solveStatus = maxFlow.Solve(source, sink);
            if (solveStatus == MaxFlow.OPTIMAL)
            {
                //Console.WriteLine("Max. flow: " + totalFlow);
                Console.WriteLine("");
                Console.WriteLine(" Edge     Flow / Capacity");
                for (int i = 0; i < numArcs; ++i)
                {
                    Console.WriteLine(maxFlow.Tail(i) + " -> " +
                                      maxFlow.Head(i) + "    " +
                                      string.Format("{0,3}", maxFlow.Flow(i)) + "  /  " +
                                      string.Format("{0,3}", maxFlow.Capacity(i)));
                    
                }
            }
            else
            {
                Console.WriteLine("Solving the max flow problem failed. Solver status: " +
                                  solveStatus);
            }
            Console.ReadLine();
            SWIGTYPE_p_std__vectorT_int_t lolWatDekanker = null;
            maxFlow.GetSinkSideMinCut(lolWatDekanker);
            lolWatDekanker.ToString();
        }

    }

    class Graph
    {
        public HashSet<int> nodes = new HashSet<int>();
        public Dictionary<int, HashSet<int>> AllFromNodes = new Dictionary<int, HashSet<int>>();
        public Dictionary<int, HashSet<int>> AllToNodes = new Dictionary<int, HashSet<int>>();
        public Dictionary<int, HybridDictionary> AllWeights = new Dictionary<int, HybridDictionary>();

        //TODO add size and trace contractions

        public int AddNodeToGraph(int id)
        {
            if (!nodes.Contains(id))
            {
                nodes.Add(id);
                HashSet<int> fromNodes = new HashSet<int>();
                HashSet<int> toNodes = new HashSet<int>();
                HybridDictionary weights = new HybridDictionary();
                AllFromNodes[id] = fromNodes;
                AllToNodes[id] = toNodes;
                AllWeights[id] = weights;
            }
            return id;
        }
        public void RemoveNodeFromGraph(int id)
        {
            nodes.Remove(id);
        }

        public int Contraction(int v, int w)
        {
            HashSet<int> toNodesW = AllToNodes[w];
            HashSet<int> fromNodesW = AllFromNodes[w];

            //Dictionary<int, int> weightsW = w.Weights;



            foreach (int x in fromNodesW)
            {
                if (x != v)
                {
                    RemoveToEdge(x, w);
                    AddFromEgde(v, x, (int)AllWeights[w][x]);
                    AddToEgde(x, v);
                }
            }
            foreach (int x in toNodesW)
            {
                if (x != v)
                {
                    AddToEgde(v, x);
                    int weight = (int)AllWeights[x][w];
                    RemoveFromEdge(x, w);
                    AddFromEgde(x, v, weight);
                }
            }

            RemoveFromEdge(v, w);
            RemoveToEdge(v, w);
            RemoveNodeFromGraph(w);

            return v;
        }

        public void ContractList(List<int> contractionList)
        {

            if (nodes.Count <= 1)
            {
                throw new Exception("CONTRACTING EMPTY/SINGLTON LIST BTFO");
            }
            int first = contractionList.First();
            foreach (int id in contractionList.Skip(1))
            {
                first = Contraction(first, id);
                //print();
                //Console.WriteLine(id);
            }
        }

        public void AddFromEgde(int id, int idOtherNode, int weight)
        {
            HybridDictionary weights = AllWeights[id];
            if (!AllFromNodes[id].Contains(idOtherNode))
            {
                AllFromNodes[id].Add(idOtherNode);
                weights[idOtherNode] = weight;
            }
            else
                weights[idOtherNode] = (int)weights[idOtherNode] + weight;
        }
        public void AddToEgde(int id, int idOtherNode)
        {
            if (!AllToNodes[id].Contains(idOtherNode))
                AllToNodes[id].Add(idOtherNode);
        }
        public void RemoveFromEdge(int id, int idOtherNode)
        {
            AllFromNodes[id].Remove(idOtherNode);
        }
        public void RemoveToEdge(int id, int idOtherNode)
        {
            AllToNodes[id].Remove(idOtherNode);
        }


        public void print()
        {
            foreach (int id in nodes)
            {
                HashSet<int> fromNodes = AllFromNodes[id];
                HashSet<int> toNodes = AllToNodes[id];
                string from = "";
                foreach (int x in fromNodes)
                    from += " " + x + "::" + AllWeights[id][x];
                string to = "";
                foreach (int x in toNodes) to += " " + x;
                Console.WriteLine("{0}: with fromNodes {1} and toNodes {2}", id, from, to);

            }
        }
        public Graph CreateSubGraph(List<int> ids)
        {
            Graph G2 = new Graph();

            foreach (int id in ids)
            {
                if (!nodes.Contains(id))
                { throw new Exception("Node not in original graph"); }
                else
                {
                    G2.AddNodeToGraph(id);
                }
            }

            foreach (int id in G2.nodes)
            {
                HashSet<int> fromNodes = AllFromNodes[id];
                foreach (int nId in fromNodes)
                {
                    if (G2.nodes.Contains(nId))
                    {
                        G2.AddFromEgde(id, nId, (int)AllWeights[id][nId]);
                    }
                }
                HashSet<int> toNodes = AllToNodes[id];
                foreach (int nId in toNodes)
                {
                    if (G2.nodes.Contains(nId))
                    {
                        G2.AddToEgde(id, nId);
                    }
                }
            }
            return G2;
        }

    }

    class BFS
    {

        public List<int> T = new List<int>();
        public List<int> Core = new List<int>();
        public List<int> Ring;
        public List<int> SubGraph;
        Queue<int> queue = new Queue<int>();
        HashSet<int> visited = new HashSet<int>();
        Graph G;
        int MaxTree;
        int MaxCore;
        public BFS(Graph g, int U, double alpha, double f, int startNode)
        {
            G = g;
            MaxTree = (int)(U * alpha);
            MaxCore = (int)((double)U / f);
            run(startNode);
        }

        public void run(int startNode)
        {

            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0 && T.Count < MaxTree)
            {
                int currentNode = queue.Dequeue();
                HashSet<int> fromNodes = G.AllFromNodes[currentNode];
                foreach (int nId in fromNodes)
                {
                    visitNode(nId);
                }
                HashSet<int> toNodes = G.AllToNodes[currentNode];
                foreach (int nId in toNodes)
                {
                    visitNode(nId);
                }

            }
            Ring = queue.ToList();
            SubGraph = T.Union(Ring).ToList();

        }
        private void visitNode(int v)
        {
            if (!visited.Contains(v))
            {
                queue.Enqueue(v);
                if (Core.Count < MaxCore)
                {
                    Core.Add(v);
                }
                T.Add(v);
                visited.Add(v);
            }
        }

        private int RandomVertex()
        {
            throw new NotImplementedException();
        }


    }
    public struct NodePoint
    {
        public int x;
        public int y;
    }

}
