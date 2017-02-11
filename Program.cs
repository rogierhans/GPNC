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
            //    Node node = new Node(i);
            //    G.AddNodeToGraph(i, node);
            //}
            //for (int i = 0; i < 9; i++)
            //{
            //    Node n = G.GetNode(i);
            //    Node npe = G.GetNode(i + 1);

            //    //misschien hier iets aan doen 
            //    n.AddFromEgde(i + 1, 20 - i);
            //    npe.AddToEgde(i);
            //}
            //G.print();
            //Console.ReadLine();
            //G.ContractList(new List<int> { 4, 5, 8 });
            //G.ContractList(new List<int> { 9,6 });

            List<uint> test = G.nodes.Take(100000).ToList();
            G.ContractList(test);
            G.print();
            Console.ReadLine();
        }
    }

    class Graph
    {
        public HashSet<uint> nodes = new HashSet<uint>();
        public Dictionary<uint, HashSet<uint>> AllFromNodes = new Dictionary<uint, HashSet<uint>>();
        public Dictionary<uint, HashSet<uint>> AllToNodes = new Dictionary<uint, HashSet<uint>>();
        public Dictionary<uint, Dictionary<uint, int>> AllWeights = new Dictionary<uint, Dictionary<uint, int>>();



        public uint AddNodeToGraph(uint id)
        {
            if (!nodes.Contains(id))
            {
                nodes.Add(id);
                HashSet<uint> fromNodes = new HashSet<uint>();
                HashSet<uint> toNodes = new HashSet<uint>();
                Dictionary<uint, int> weights = new Dictionary<uint, int>();
                AllFromNodes[id] = fromNodes;
                AllToNodes[id] = toNodes;
                AllWeights[id] = weights;
            }
            return id;
        }
        public void RemoveNodeFromGraph(uint id)
        {
            nodes.Remove(id);
        }

        public uint Contraction(uint v, uint w)
        {
            HashSet<uint> toNodesW = AllToNodes[w];
            HashSet<uint> fromNodesW = AllFromNodes[w];

            //Dictionary<int, int> weightsW = w.Weights;



            foreach (uint x in fromNodesW)
            {
                if (x != v)
                {
                    RemoveToEdge(x, w);
                    AddFromEgde(v, x, AllWeights[w][x]);
                    AddToEgde(x, v);
                }
            }
            foreach (uint x in toNodesW)
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

        public void ContractList(List<uint> contractionList)
        {

            if (nodes.Count <= 1)
            {
                throw new Exception("CONTRACTING EMPTY/SINGLTON LIST BTFO");
            }
            uint first = contractionList.First();
            foreach (uint id in contractionList.Skip(1))
            {
                first = Contraction(first, id);
                //print();
                Console.WriteLine(id);
            }
        }

        public void AddFromEgde(uint id, uint idOtherNode, int weight)
        {

            if (!AllFromNodes[id].Contains(idOtherNode))
            {
                AllFromNodes[id].Add(idOtherNode);
                AllWeights[id][idOtherNode] = weight;
            }
            else
                AllWeights[id][idOtherNode] += weight;
        }
        public void AddToEgde(uint id, uint idOtherNode)
        {
            if (!AllToNodes[id].Contains(idOtherNode))
                AllToNodes[id].Add(idOtherNode);
        }
        public void RemoveFromEdge(uint id, uint idOtherNode)
        {
            AllFromNodes[id].Remove(idOtherNode);
        }
        public void RemoveToEdge(uint id, uint idOtherNode)
        {
            AllToNodes[id].Remove(idOtherNode);
        }


        public void print()
        {
            foreach (uint id in nodes)
            {
                HashSet<uint> fromNodes = AllFromNodes[id];
                HashSet<uint> toNodes = AllToNodes[id];
                string from = "";
                foreach (uint x in fromNodes)
                    from += " " + x + "::" + AllWeights[id][x];
                string to = "";
                foreach (uint x in toNodes) to += " " + x;
                Console.WriteLine("{0}: with fromNodes {1} and toNodes {2}", id, from, to);

            }
        }
        public Graph CreateSubGraph(List<uint> ids)
        {
            Graph G2 = new Graph();

            foreach (uint id in ids)
            {
                if (!nodes.Contains(id))
                { throw new Exception("Node not in original graph"); }
                else
                {
                    G2.AddNodeToGraph(id);
                }
            }
            var allToNodes = new Dictionary<uint, HashSet<uint>>();
            var allFromNodes = new Dictionary<uint, HashSet<uint>>();
            G2.AllWeights = AllWeights;

            return G2;
        }
    }


}
