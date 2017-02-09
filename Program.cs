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

            List<int> test = G.nodes.Keys.Take(100000).ToList();
            G.ContractList(test);
            G.print();
            Console.ReadLine();
        }
    }

    class Graph
    {
        public Dictionary<int, Node> nodes = new Dictionary<int, Node>();

        public void AddNodeToGraph(int id, Node n)
        {
            nodes[id] = n;
        }

        public Node MakeNodeOrReturnNode(int id)
        {
            if (!nodes.ContainsKey(id))
            {
                Node v = new Node(id);
                nodes[id] = v;
            }
            return nodes[id];
        }
        public void RemoveNodeFromGraph(int id)
        {
            nodes.Remove(id);
        }
        public Node GetNode(int id)
        {
            return nodes[id];
        }

        public Node Contraction(Node v, Node w)
        {
            int idV = v.Id;
            int idW = w.Id;

            HashSet<int> toNodesW = w.ToNodes;
            HashSet<int> fromNodesW = w.FromNodes;

            Dictionary<int, int> weightsW = w.Weights;
            


            foreach (int x in fromNodesW)
            {
                if (x != idV)
                {
                    nodes[x].RemoveToEdge(idW);
                    v.AddFromEgde(x, weightsW[x]);
                    nodes[x].AddToEgde(idV);
                }
            }
            foreach (int x in toNodesW)
            {
                if (x != idV)
                {
                    v.AddToEgde(x);
                    Node neighbour = nodes[x];
                    int weight = neighbour.Weights[idW];
                    neighbour.RemoveFromEdge(idW);
                    neighbour.AddFromEgde(idV, weight);
                }
            }

            v.RemoveFromEdge(idW);
            v.RemoveToEdge(idW);
            RemoveNodeFromGraph(idW);

            v.AddIds(w.Ids);
            return v;
        }

        public void ContractList(List<int> contractionList)
        {

            if (nodes.Count <= 1)
            {
                throw new Exception("CONTRACTING EMPTY/SINGLTON LIST BTFO");
            }
            Node first = nodes[contractionList.First()];
            foreach (int id in contractionList.Skip(1))
            {
                Node second = nodes[id];
                first = Contraction(first, second);
                //print();
                Console.WriteLine(id);
            }
        }




        public void print()
        {
            foreach (KeyValuePair<int, Node> kvp in nodes)
            {
                HashSet<int> fromNodes = kvp.Value.FromNodes;
                HashSet<int> toNodes = kvp.Value.ToNodes;
                string from = "";
                foreach (int x in fromNodes)
                    from += " " + x + "::" + kvp.Value.Weights[x];
                string to = "";
                foreach (int x in toNodes) to += " " + x;
                Console.WriteLine("{0}: with fromNodes {1} and toNodes {2}", kvp.Key, from, to);

            }
        }
    }
    class Node
    {
        public HashSet<int> FromNodes { get; private set; }
        public HashSet<int> ToNodes { get; private set; }
        public Dictionary<int, int> Weights { get; private set; }
        public int Id { get; private set; }
        public int Size { get; private set; }
        public List<int> Ids { get; private set; }

        public Node(int id)
        {
            Id = id;
            FromNodes = new HashSet<int>();
            ToNodes = new HashSet<int>();
            Weights = new Dictionary<int, int>();
            Ids = new List<int>();
            Ids.Add(id);
        }
        public void AddIds(List<int> ids){
            ids.ForEach(x => Ids.Add(x));
        }
        public void AddFromEgde(int idNode, int weight)
        {
            if (!FromNodes.Contains(idNode))
            {
                FromNodes.Add(idNode);
                Weights[idNode] = weight;
            }
            else
                Weights[idNode] += weight;
        }
        public void AddToEgde(int idNode)
        {
            if (!ToNodes.Contains(idNode))
                ToNodes.Add(idNode);
        }
        public void RemoveFromEdge(int idNode)
        {
            FromNodes.Remove(idNode);

            //optionele stap denk ik
            Weights.Remove(idNode);
        }
        public void RemoveToEdge(int idNode)
        {
            ToNodes.Remove(idNode);
        }
    }

}
