using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    class LocalSearch
    {
        public static Graph Search1(Graph G, Graph FG, int U)
        {

            Graph BestGraph = G;
            int BestWeight = int.MaxValue;
            int runs = 100000;
            Dictionary<int, HashSet<int>> partitions;
            Random rng = new Random();

            EdgeSelecter edgeSelecter = new EdgeSelecter(G.GetAllArcs(), rng);
            while (edgeSelecter.NotEmpty() && runs > 0)
            {

                int weight = 0;
                G.GetAllArcs().ForEach(x => weight += G.getWeight(x.To, x.From));
                Console.WriteLine("Weight:{0}", weight);
                if (calcAccept(weight, BestWeight, runs, rng))
                {
                    BestGraph = G.CreateSubGraph(G.nodes.ToList());
                    BestWeight = weight;
                    edgeSelecter = new EdgeSelecter(G.GetAllArcs(), rng);
                }

                //Get the partitions
                partitions = Uncontract.GetPartitions(BestGraph, FG);

                G = FG.CreateSubGraphWithoutParent(FG.nodes.ToList());

                Edge e = edgeSelecter.NextEdge();
                ContractOnGraph(G, partitions, e);

                //apply greedy algorithm
                G.ApplyGreedyAlgorithm(U);
                runs--;
            }

            //fix Parent optimazation
            foreach (var kvp in FG.Parent)
            {
                BestGraph.Parent[kvp.Key] = kvp.Value;
            }
            return BestGraph;
        }

        private class EdgeSelecter
        {
            private List<Edge> possibleEdges;
            private Dictionary<Edge, int> phiScores;
            private int maxPhi = 16;
            Random rng;
            public EdgeSelecter(List<Edge> edges, Random rng)
            {
                possibleEdges = edges;
                phiScores = new Dictionary<Edge, int>();
                edges.ForEach(e => { phiScores[e] = 0; });
                this.rng = rng;
            }
            public Edge NextEdge()
            {

                Edge e = possibleEdges[rng.Next(possibleEdges.Count)];
                phiScores[e] += 1;

                if (phiScores[e] >= maxPhi)
                {
                    possibleEdges.Remove(e);
                }
                return e;
            }

            public bool NotEmpty()
            {
                return (possibleEdges.Count > 0);
            }

        }




        private static void ContractOnGraph(Graph G, Dictionary<int, HashSet<int>> partitions, Edge e)
        {
            foreach (var kvp in partitions)
            {
                int key = kvp.Key;
                if (!(key == e.From || key == e.To))
                {
                    G.ContractList(kvp.Value.ToList());
                }
            }
        }

        private static bool calcAccept(int weight, int bestWeight, int runs, Random r)
        {

            if (weight < bestWeight)
                return true;
            else
            {
                return false;
            }
        }
    }
}
