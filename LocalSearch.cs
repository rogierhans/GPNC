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
            int runs = 1000;
            Dictionary<int, HashSet<int>> partitions;
            Random rnd = new Random();
            int index = 0;
            int round = 5;
            while (runs > 0)
            {

                //G.print();
                Console.WriteLine(G.nodes.Count);
                int weight = 0;
                G.GetAllArcs().ForEach(x => weight += G.getWeight(x.To, x.From));
                Console.WriteLine("Weight:{0}", weight);

                if (calcAccept(weight, BestWeight, runs, rnd))
                {
                    BestGraph = G.CreateSubGraph(G.nodes.ToList());
                    BestWeight = weight;
                    index = 0;
                }

                //Get the partitions
                partitions = Uncontract.GetPartitions(BestGraph, FG);

                G = FG.CreateSubGraphWithoutParent(FG.nodes.ToList());

                List<Edge> edges = BestGraph.GetAllArcs();
                if (index >= edges.Count)
                {
                    if (round <= 0)
                        break;
                    else
                    {
                        round--;
                        index = 0;
                    }
                }
                //Edge e = edges[rnd.Next(edges.Count)];
                Edge e = edges.OrderBy(x => BestGraph.getWeight(x.To, x.From)).ToList()[(edges.Count - 1) - index];
                ContractOnGraph(G, partitions, e);

                //apply greedy algorithm
                G.ApplyGreedyAlgorithm(U);
                runs--; index++;
            }
            foreach (var kvp in FG.Parent) {
                BestGraph.Parent[kvp.Key] = kvp.Value;
            }
            return BestGraph;
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
                //double c = runs / 10.0;
                //double score = bestWeight - weight;
                //double p = Math.Pow(Math.E, score/c);
                //Console.WriteLine(p);
                //bool boo= (p > r.NextDouble());
                //Console.WriteLine(boo);
                //return boo;
                return false;
            }
        }
    }
}
