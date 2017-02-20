using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;


namespace GPNC
{
    class Program
    {
        static void Main(string[] args)
        {

            ////read graph
            //Graph G = Parser.ParseCSVFile();
            //Console.WriteLine(G.nodes.Count);

            ////Filter unconnected nodes
            //G = Filter.ConnectedComponent(G);

            ////copy original graph
            //Graph OG = G.CreateSubGraph(G.nodes.ToList());
            //Console.WriteLine(G.nodes.Count);
            //IOGraph.WriteGraph(OG, "OG");

            ////calculate max size partition
            //int U = G.nodes.Count / 2;
            //Console.WriteLine(U);

            ////read Nodepoint to make graphs
            //Dictionary<int, NodePoint> nodes = Parser.ParseNodes(G);

            ////Contract One degree
            //Filter.RemoveOneDegree(G);
            //Console.WriteLine(G.nodes.Count);

            ////Contract two degree
            //Filter.RemoveTwoDegree(G);
            //Console.WriteLine(G.nodes.Count);

            ////Find natural cuts and contract them
            //HashSet<Edge> cuts = NaturalCut.MakeCuts(G, U);
            //List<List<int>> ps = FindFragments.FindPartions(G, cuts, U);
            //ps.ForEach(x => { int v = G.ContractList(x); });
            //IOGraph.WriteGraph(G, "FG");

            Graph OG = IOGraph.ReadGraph("OG");
            Dictionary<int, NodePoint> nodes = Parser.ParseNodes(OG);
            int U = OG.nodes.Count / 2;
            Graph G = IOGraph.ReadGraph("FG");


            Dictionary<int, int> Parents = new Dictionary<int, int>(G.Parent);
            Graph FG = G.CreateSubGraph(G.nodes.ToList());
            Console.WriteLine(G.nodes.Count);



            Graph BestGraph = G;
            int BestWeight = int.MaxValue;
            int runs = 100;
            Dictionary<int, HashSet<int>> realPS;
            Dictionary<int, int> BestGraphParents = null;
            Random rnd = new Random();
            while (runs > 0)
            {
                //apply greedy algorithm
                Greedy.initPar(G, U);
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
                }

                //Get the partitions
                realPS = Uncontract.getPartitions(BestGraph, FG, BestGraphParents);

                G = FG.CreateSubGraph(FG.nodes.ToList());

                List<Edge> edges = BestGraph.GetAllArcs();
                Edge e = edges[rnd.Next(edges.Count)];

                foreach (var kvp in realPS)
                {
                    int key = kvp.Key;
                    if (!(key == e.From || key == e.To))
                    {
                        G.ContractList(kvp.Value.ToList());
                    }
                }
                runs--;
            }
            IOGraph.WriteGraph(BestGraph, "BG");

            foreach (var kvp in BestGraphParents)
            {
                Parents[kvp.Key] = kvp.Value;
            }

            realPS = Uncontract.getPartitions(BestGraph, OG, Parents);
            //Print.makePrints(BestGraph, OG, nodes, realPS, Parents);

            Console.ReadLine();
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

    public struct NodePoint
    {
        public int x;
        public int y;
        public int lati;
        public int longi;
    }

    public struct Edge
    {
        public int From;
        public int To;

        public Edge(int from, int to)
        {
            this.From = from < to ? from : to;
            this.To = from < to ? to : from;

        }
        public override string ToString()
        {
            return ("(" + From.ToString() + "," + To.ToString() + ")");
        }
        public override int GetHashCode()
        {
            return ((23 * 31) + From * 31) + To;
        }


    }


}
