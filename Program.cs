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
            //    Node node = new Node(i, 1);
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
            //G.ContractList(new List<int> { 4,5,6,7,8});

            //////Console.WriteLine();
            ////Node v = G.GetNode(3);
            ////Node w = G.GetNode(5);
            ////G.Contraction(v, w);
            ////G.print();
            ////Console.ReadLine();
            ////v = G.GetNode(4);
            ////w = G.GetNode(6);
            ////G.Contraction(v, w);
            //////G.checkDuplicate();
            G.print();
            List<int> test = G.nodes.Keys.Take(10000).ToList();
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
                Node v = new Node(id, 1);
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

            //super inefficient
            List<int> toNodesV = v.ToNodes.Where(x => x != idW).ToList();
            List<int> toNodesW = w.ToNodes.Where(x => x != idV).ToList();
            List<int> fromNodesV = v.FromNodes.Where(x => x != idW).ToList();
            List<int> fromNodesW = w.FromNodes.Where(x => x != idV).ToList();


            Dictionary<int, int> weightsV = v.Weights;
            Dictionary<int, int> weightsW = w.Weights;
            int newSize = v.Size + w.Size;

            //create new node
            Node contraction = new Node(v.Ids, w.Ids, newSize);

            //get id new node (this is one of the Id's from the contracted nodes)
            int newId = contraction.Id;

            //add ToNodes to the contracted Node
            toNodesV.ForEach(x => contraction.AddToEgde(x));
            toNodesW.ForEach(x => contraction.AddToEgde(x));



            //add FromNodes to the contracted Node;
            fromNodesV.ForEach(x => contraction.AddFromEgde(x, weightsV[x]));
            fromNodesW.ForEach(x => contraction.AddFromEgde(x, weightsW[x]));

            //remove the edges v and w go to
            fromNodesV.ForEach(x => nodes[x].RemoveToEdge(idV));
            fromNodesW.ForEach(x => nodes[x].RemoveToEdge(idW));
            fromNodesV.ForEach(x => nodes[x].AddToEgde(newId));
            fromNodesW.ForEach(x => nodes[x].AddToEgde(newId));

            //Remove FromNodes of the neighbours of v and w and add for the contracted Node
            toNodesV.ForEach(x =>
            {
                Node neighbour = nodes[x];
                int weight = neighbour.Weights[v.Id];
                neighbour.RemoveFromEdge(v.Id);
                neighbour.AddFromEgde(newId, weight);
            });
            toNodesW.ForEach(x =>
            {
                Node neighbour = nodes[x];
                int weight = neighbour.Weights[w.Id];
                neighbour.RemoveFromEdge(w.Id);
                neighbour.AddFromEgde(newId, weight);
            });

            RemoveNodeFromGraph(idV);
            RemoveNodeFromGraph(idW);
            AddNodeToGraph(newId, contraction);
            return contraction;
        }

        public void ContractList(List<int> contractionList)
        {
            
            if (nodes.Count <= 1)
            {
                throw new Exception("CONTRACTING EMPTY/SINGLTON LIST BTFO");
            }
            Node first = nodes[contractionList.First()];
            foreach (int id in contractionList.Skip(1)) {
                Node second = nodes[id];
                first = Contraction(first, second);
                //print();
                Console.WriteLine(id);
            }
        }




        public void print()
        {
            foreach (KeyValuePair<int, Node> kvp in nodes.Take(100))
            {
                List<int> fromNodes = kvp.Value.FromNodes;
                List<int> toNodes = kvp.Value.ToNodes;
                string from = "";
                fromNodes.ForEach(x => from += " " + x + "::" + kvp.Value.Weights[x]);
                string to = "";
                toNodes.ForEach(x => to += " " + x);
                Console.WriteLine("{0}: with fromNodes {1} and toNodes {2}", kvp.Key, from, to);

            }
        }

        public void checkDuplicate()
        {
            //int i = 0;
            //foreach (KeyValuePair<int, Node> kvp in nodes)
            //{
            //    int doup = 0;
            //    kvp.Value.Neighbours.ForEach(x => kvp.Value.Neighbours.ForEach(y => { if (x == y) doup++; }));
            //    if (doup - kvp.Value.Neighbours.Count > 0) {
            //        string neighbours = "";
            //        kvp.Value.Neighbours.ForEach(x => neighbours += " " + x.ToString() + "::" + kvp.Value.Weights[x]);
            //        Console.WriteLine("{0}: with neighbours {1}", kvp.Key, neighbours);
            //        Console.ReadLine();
            //    }
            //}
            //Console.WriteLine(i);
        }
    }
    class Node
    {
        public List<int> FromNodes { get; set; }
        public List<int> ToNodes { get; set; }
        public Dictionary<int, int> Weights { get; set; }
        public int Id { get; private set; }
        public int Size { get; private set; }
        public List<int> Ids { get; private set; }

        public Node(int id, int size)
        {
            Size = size;
            Id = id;
            FromNodes = new List<int>();
            ToNodes = new List<int>();
            Weights = new Dictionary<int, int>();
            Ids = new List<int>();
            Ids.Add(id);
        }
        public Node(List<int> ids1, List<int> ids2, int size)
        {
            Size = size;

            FromNodes = new List<int>();
            ToNodes = new List<int>();
            Weights = new Dictionary<int, int>();
            Ids = ids1.Union(ids2).ToList();
            Id = Ids.First();
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
