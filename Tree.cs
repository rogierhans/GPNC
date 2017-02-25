using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    class Tree
    {
        public List<Tree> Childeren;
        public HashSet<int> LeafNodes;

        HashSet<Edge> CutEdges = new HashSet<Edge>();
        HashSet<int> BoundaryNodes;
        public int Nodes;
        public int Edges;

        public Tree(Graph OG, Graph G, int U, int MinSize, double alpha, double f, HashSet<int> BoundaryNodes, Dictionary<int, NodePoint> printNodes, string name)
        {
            Console.WriteLine("Tree1");
            Graph SubGraph = G.CreateSubGraph(G.nodes.ToList());
            this.BoundaryNodes = BoundaryNodes;
            Childeren = new List<Tree>();

            Nodes = G.nodes.Sum(x => G.Size[x]);
            Edges = G.GetAllArcs().Sum(e => G.getWeight(e.To, e.From));

            Console.WriteLine("NC:{0} with {1}", G.nodes.Count, MinSize);
            if (G.nodes.Count > MinSize)
            {
                HashSet<Edge> cuts = NaturalCut.MakeCuts(G, U, alpha, f);
                List<List<int>> ps = FindFragments.FindPartions(G, cuts, U);
                ps.ForEach(x => G.ContractList(x));
                Graph FG = G.CreateSubGraph(G.nodes.ToList());
                Console.WriteLine(G.nodes.Count);
                G.ApplyGreedyAlgorithm(U);
                G = LocalSearch.Search1(G, FG, U);
                Dictionary<int, HashSet<int>> partitions = Uncontract.GetPartitions(G, SubGraph);
                int i = 0;
                foreach (var kvp in partitions)
                {
                    int cId = kvp.Key;

                    Graph newSubGraph = SubGraph.CreateSubGraph(kvp.Value.ToList());
                    int newU = U / partitions.Count;

                    List<int> neighbourPartitions = G.GetNeighbours(cId);
                    int cutEdges = 0;
                    neighbourPartitions.ForEach(neighbour => cutEdges += G.getWeight(cId, neighbour));


                    //todo
                    HashSet<int> originalIDs = GetOrignalID(OG, G,kvp.Value);
                    HashSet<int> parBoundaryNodes = new HashSet<int>();
                    foreach (Edge e in OG.GetAllArcs())
                    {
                        if (originalIDs.Contains(e.To) && !originalIDs.Contains(e.From))
                        {
                            parBoundaryNodes.Add(e.To);
                            CutEdges.Add(e);
                        }
                        if (!originalIDs.Contains(e.To) && originalIDs.Contains(e.From))
                        {
                            parBoundaryNodes.Add(e.From);
                            CutEdges.Add(e);
                        }
                    }
                    Childeren.Add(new Tree(OG, newSubGraph, newU, MinSize, alpha, f, parBoundaryNodes, printNodes, name + i++));
                }
            }
            else
            {
                LeafNodes = G.nodes;
            }

        }

        //inefficient
        private HashSet<int> GetOrignalID(Graph OG, Graph G, HashSet<int> par)
        {
            HashSet<int> set = new HashSet<int>();
            foreach (int id in OG.nodes)
            {
                int currentNode = id;
                while (G.Parent.ContainsKey(currentNode)) currentNode = G.Parent[currentNode];
                if (par.Contains(currentNode)) set.Add(id);
            }
            return set;
        }

        public double Quality()
        {
            if (Leaf())
            {
                return Edges;
            }
            else
            {
                double splitQ = SplitQualtity();
                double qualityChildNodes = 0;
                foreach (Tree ti in Childeren)
                {
                    double pi = (double)ti.Nodes / Nodes;
                    foreach (Tree tj in Childeren)
                    {
                        if (ti != tj)
                        {
                            double pj = (double)tj.Nodes / Nodes;
                            qualityChildNodes += pi * pj * (ti.Quality() + tj.Quality());
                        }
                    }
                }
                return splitQ + qualityChildNodes;
            }
        }


        //splitQuality word nu op null aangeroepen
        private double SplitQualtity()
        {
            double boundary = 0;
            foreach (Tree ti in Childeren)
            {
                boundary += Math.Pow(ti.BoundaryNodes.Count, 3.0 / 2);
            }
            double crossComponentEdges = CutEdges.Count;
            return boundary + crossComponentEdges;
        }
        public void Print(int n)
        {
            string s = "";
            for (int i = 0; i < n; i++)
            {
                s += "\t";
            }
            Console.Write(s);
            Console.WriteLine(Nodes+ " " + BoundaryNodes.Count + " " + CutEdges.Count);
            Childeren.ForEach(x => x.Print(n + 1));
        }

        public bool Leaf()
        {
            return Childeren.Count == 0;
        }

    }
}
