using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    class Graph
    {
        public HashSet<int> nodes = new HashSet<int>();


        private Dictionary<int, HashSet<int>> AllReverseNeighbours = new Dictionary<int, HashSet<int>>();
        public Dictionary<int, Dictionary<int, int>> AllWeights = new Dictionary<int, Dictionary<int, int>>();
        public Dictionary<int, int> Size = new Dictionary<int, int>();
        public Dictionary<int, int> Parent = new Dictionary<int, int>();


        public int AddNodeToGraph(int id, int size)
        {
            if (!nodes.Contains(id))
            {
                nodes.Add(id);
                HashSet<int> reverseNeighbours = new HashSet<int>();
                Dictionary<int, int> weights = new Dictionary<int, int>();
                AllWeights[id] = weights;
                AllReverseNeighbours[id] = reverseNeighbours;
                Size[id] = size;
            }
            return id;
        }

        public void RemoveNodeFromGraph(int id)
        {
            nodes.Remove(id);
        }

        public int Contraction(int v, int w)
        {

            foreach (int x in GetNeighbours(w))
            {
                if (x != v)
                {

                    int weight = getWeight(w, x);
                    RemoveEdge(x, w);
                    AddEdge(v, x, weight);
                }
            }

            RemoveEdge(v, w);
            RemoveNodeFromGraph(w);
            Size[v] += Size[w];
            Parent[w] = v;
            return v;

        }

        public int ContractList(List<int> contractionList)
        {

            if (nodes.Count <= 1)
            {
                throw new Exception("CONTRACTING EMPTY/SINGLTON LIST BTFO");
            }
            int first = contractionList.First();
            foreach (int id in contractionList.Skip(1))
            {
                first = Contraction(first, id);
            }
            return first;
        }

        public List<int> GetNeighbours(int id)
        {
            return AllWeights[id].Keys.Concat(AllReverseNeighbours[id]).ToList();
        }

        public Tuple<Dictionary<int, int>.KeyCollection, HashSet<int>> GetNeighboursFast(int id)
        {
            var from = AllWeights[id].Keys;

            var to = AllReverseNeighbours[id];


            return new Tuple<Dictionary<int, int>.KeyCollection, HashSet<int>>(from, to);
        }

        public void AddEdge(int id, int otherId, int weight)
        {
            int v = id > otherId ? id : otherId;
            int w = id > otherId ? otherId : id;

            if (!AllWeights[v].ContainsKey(w))
            {
                AllWeights[v].Add(w, weight);
                AllReverseNeighbours[w].Add(v);
            }
            else
            {
                AllWeights[v][w] = (int)AllWeights[v][w] + weight;
            }
        }

        public void RemoveEdge(int id, int otherId)
        {
            int v = id > otherId ? id : otherId;
            int w = id > otherId ? otherId : id;
            AllWeights[v].Remove(w);
            AllReverseNeighbours[w].Remove(v);
        }

        public int getWeight(int id, int otherId)
        {
            int v = id > otherId ? id : otherId;
            int w = id > otherId ? otherId : id;
            return (int)AllWeights[v][w];
        }

        public bool IsEdge(int id, int otherId)
        {
            int v = id > otherId ? id : otherId;
            int w = id > otherId ? otherId : id;
            return AllWeights[v].ContainsKey(w);
        }

        public void ContractionAlt(int v1, int w1) {
            int v2 = v1;
            while (!nodes.Contains(v2)) { v2 = Parent[v2];

            }
            int w2 = w1;
            while (!nodes.Contains(w2)) { w2 = Parent[w2];
            }

            if (v2 != w2)
            {
                Contraction(v2, w2);
            }
        }

        public void ApplyGreedyAlgorithm(int U)
        {
            EdgeStructure ES = new EdgeStructure(this, U);

            while (ES.NonEmpty())
            {
                Edge bestEdge = ES.BestEdgeToContract();
                int newID = bestEdge.From;
                int deletedId = bestEdge.To;
                Contraction(newID, deletedId);
                ES.UpdateStructure(newID, deletedId);
            }
        }

        public List<Edge> GetAllArcs()
        {
            List<Edge> arcs = new List<Edge>();
            foreach (var kvp in AllReverseNeighbours)
            {
                foreach (int id in kvp.Value)
                {
                    arcs.Add(new Edge(id, kvp.Key));
                }
            }
            return arcs;

        }

        public List<Edge> GetAllArcs(int U)
        {
            List<Edge> arcs = new List<Edge>();
            foreach (var kvp in AllReverseNeighbours)
            {
                foreach (int id in kvp.Value)
                {
                    int size = Size[kvp.Key] + Size[id];
                    if (size <= U)
                        arcs.Add(new Edge(id, kvp.Key));
                }
            }
            return arcs;

        }

        public int GetDegree(int id)
        {
            return GetNeighbours(id).Count;
        }

        public void print()
        {
            foreach (int id in nodes)
            {
                string s = id + "(" + Size[id] + ")" + ": ";
                foreach (int n in GetNeighbours(id))
                {
                    s += n + "(" + Size[n] + ")";


                }
                Console.WriteLine(s);
            }
        }
        //does not copy parent
        public Graph CreateSubGraph(List<int> ids)
        {
            Graph G2 = new Graph();

            foreach (int id in ids)
            {
                if (!nodes.Contains(id))
                { throw new Exception("Node not in original graph"); }
                else
                {
                    G2.AddNodeToGraph(id,Size[id]);
                }
            }

            foreach (int id in G2.nodes)
            {
                Dictionary<int, int> hd = AllWeights[id];
                foreach (int otherId in hd.Keys)
                {
                    if (G2.nodes.Contains(otherId))
                    {
                        G2.AddEdge(id, otherId, AllWeights[id][otherId]);
                    }
                }
            }
            G2.Parent = new Dictionary<int, int>(Parent);
            return G2;
        }
        public Graph CreateSubGraphWithoutParent(List<int> ids)
        {
            Graph G2 = new Graph();

            foreach (int id in ids)
            {
                if (!nodes.Contains(id))
                { throw new Exception("Node not in original graph"); }
                else
                {
                    G2.AddNodeToGraph(id, Size[id]);
                }
            }

            foreach (int id in G2.nodes)
            {
                Dictionary<int, int> hd = AllWeights[id];
                foreach (int otherId in hd.Keys)
                {
                    if (G2.nodes.Contains(otherId))
                    {
                        G2.AddEdge(id, otherId, AllWeights[id][otherId]);
                    }
                }
            }

            return G2;
        }
    }
}
