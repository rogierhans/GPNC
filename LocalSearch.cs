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
            int runs = 10000;
            Dictionary<int, HashSet<int>> realPS;
            Dictionary<int, int> BestGraphParents = null;
            Random rnd = new Random();
            int i = 0;
            int meta = 0;
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
                    BestGraphParents = new Dictionary<int, int>(G.Parent);
                    i = 0;
                }

                //Get the partitions
                realPS = Uncontract.getPartitions(BestGraph, FG, BestGraphParents);

                G = FG.CreateSubGraph(FG.nodes.ToList());

                List<Edge> edges = BestGraph.GetAllArcs();
                if (i >= edges.Count)
                {
                    if (meta > 5)
                        break;
                    else
                    { meta++;
                        i = 0;
                    }
                }
                //Edge e = edges[rnd.Next(edges.Count)];
                Edge e = edges.OrderBy(x => BestGraph.getWeight(x.To, x.From)).ToList()[(edges.Count -1)-i];
                Contracted1(G, realPS, e);

                //apply greedy algorithm
                Greedy.initPar(G, U);
                runs--; i++;
            }
            BestGraph.Parent = BestGraphParents;
            return BestGraph;
        }
        private static void Contracted1(Graph G, Dictionary<int, HashSet<int>> realPS, Edge e)
        {
            HashSet<int> tabo = new HashSet<int>();
            //foreach (int n in G.GetNeighbours(e.To)) { tabo.Add(n); }
            //foreach (int n in G.GetNeighbours(e.From)) { tabo.Add(n); }
            tabo.Add(e.To); tabo.Add(e.From);
            foreach (var kvp in realPS)
            {
                int key = kvp.Key;
                if (!tabo.Contains(key))
                {
                    G.ContractList(kvp.Value.ToList());
                }
            }
        }
        private static void Contracted2(Graph G, Dictionary<int, HashSet<int>> realPS, Edge e)
        {
            foreach (var kvp in realPS)
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
