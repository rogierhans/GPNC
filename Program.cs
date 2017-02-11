using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            BFS bfs = new BFS(G,10000,1,10,1);
            bfs.Core.ForEach(x => Console.Write(x + " "));
            bfs.T.ForEach(x => Console.Write(x + " "));
            exportIds(bfs.Core, "core");
            exportIds(bfs.T, "t");
            Console.ReadLine();
        }

        public static void exportIds(List<int> ids, string nameFile) {
            String[] strings = new String[ids.Count];

            for (int i = 0; i < ids.Count; i++)
            {
                strings[i] = ids[i].ToString();
            }
            System.IO.File.WriteAllLines("F:\\Users\\Rogier\\Desktop\\"+nameFile+".csv", strings);
        }
    }

    class Graph
    {
        public HashSet<int> nodes = new HashSet<int>();
        public Dictionary<int, HashSet<int>> AllFromNodes = new Dictionary<int, HashSet<int>>();
        public Dictionary<int, HashSet<int>> AllToNodes = new Dictionary<int, HashSet<int>>();
        public Dictionary<int, Dictionary<int, int>> AllWeights = new Dictionary<int, Dictionary<int, int>>();

        //TODO add size and trace contractions

        public int AddNodeToGraph(int id)
        {
            if (!nodes.Contains(id))
            {
                nodes.Add(id);
                HashSet<int> fromNodes = new HashSet<int>();
                HashSet<int> toNodes = new HashSet<int>();
                Dictionary<int, int> weights = new Dictionary<int, int>();
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
                    AddFromEgde(v, x, AllWeights[w][x]);
                    AddToEgde(x, v);
                }
            }
            foreach (int x in toNodesW)
            {
                if (x != v)
                {
                    AddToEgde(v, x);
                    int weight = AllWeights[x][w];
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

            if (!AllFromNodes[id].Contains(idOtherNode))
            {
                AllFromNodes[id].Add(idOtherNode);
                AllWeights[id][idOtherNode] = weight;
            }
            else
                AllWeights[id][idOtherNode] += weight;
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
                        G2.AddFromEgde(id, nId, AllWeights[id][nId]);
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

        }
        private void visitNode(int v) {
            if (!visited.Contains(v))
            {
                queue.Enqueue(v);
                if (Core.Count < MaxCore) {
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


}
