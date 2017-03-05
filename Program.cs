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
            double alpha = 1;
            int U = 2500;
            double f = 20;
            string map = "LUX";
            string Fragmented = "FG";
            string Solution = "SG";
            //Find natural cuts and contract them

            Graph G = GetFilteredGraph(map);
            HashSet<Edge> cuts = NaturalCut.MakeCuts(null, G, U, alpha, f);
            List<List<int>> ps = FindFragments.FindPartions(G, cuts, U);
            ps.ForEach(x => { int v = G.ContractList(x); });
            IOGraph.WriteGraph(G,  map + Fragmented + U / 1000);
            Graph FG = G.CreateSubGraph(G.nodes.ToList());
            G.ApplyGreedyAlgorithm(U);
            G = LocalSearch.Search1(G, FG, U);
            IOGraph.WriteGraph(G, map + Solution + U / 1000);
            Graph OG = GetOriginalGraph(map);
            var nodes = Parser.ParseNodes(OG, map);
            Print.makePrints(G, OG, nodes, U / 1000 + "f" + f + map);

            //Dictionary<int, NodePoint> nodes = Parser.ParseNodes(OG);
            //foreach (Edge e in G.GetAllArcs())
            //{
            //    cut += G.getWeight(e.To, e.From);
            //}
            //Print.makePrints(G, OG, nodes, "firstMap");


            //Graph OG = IOGraph.ReadGraph("OG");
            //Dictionary<int, NodePoint> nodes = Parser.ParseNodes(OG);



            //for (double f = 10; f <= 20; f = f + 5)
            //{
            //    int allCuts = 0;
            //    for (int i = 0; i < 10; i++)
            //    {
            //        //Graph G = IOGraph.ReadGraph("RG");
            //        HashSet<Edge> cuts = NaturalCut.MakeCuts(null, G, U, alpha, f);
            //        List<List<int>> ps = FindFragments.FindPartions(G, cuts, U);
            //        ps.ForEach(x => G.ContractList(x));
            //        Graph FG = G.CreateSubGraph(G.nodes.ToList());
            //        Console.WriteLine(G.nodes.Count);
            //        G.ApplyGreedyAlgorithm(U);
            //        G = LocalSearch.Search1(G, FG, U);
            //        int cut = 0;
            //        foreach (Edge e in G.GetAllArcs())
            //        {
            //            cut += G.getWeight(e.To, e.From);
            //        }
            //        allCuts += cut;
            //        //Print.makePrints(G, OG, nodes, "RealCoref" + f + "ac" + (double)allCuts / (i + 1) + "");

            //    }
            //}



            //Tree T = new Tree(OG, RG, U, 5000, alpha, f,new HashSet<int>(),nodes,0.ToString());

            //Console.WriteLine(T.Quality());
            //Console.ReadLine();
            //T.Print(0);
            //Graph G = IOGraph.ReadGraph("FG");


            //Graph FG = G.CreateSubGraph(G.nodes.ToList());
            //Console.WriteLine(G.nodes.Count);
            //G.ApplyGreedyAlgorithm(U);
            //G = LocalSearch.Search1(G, FG, U);
            //Dictionary<int ,HashSet<int>> partitions = Uncontract.GetPartitions(G, OG);


            //Print.makePrints(G, OG, nodes);


        }
        public static Graph GetFilteredGraph(string map)
        {
            string Filter2 = "F2";
            string Filter1 = "F1";
            string OriginalGraph = "OG";
            string Unfiltered = "UF";

            if (IOGraph.DoesGraphsExists(map + Filter2))
            {
                return IOGraph.ReadGraph(map + Filter2);
            }
            else if (IOGraph.DoesGraphsExists(map + Filter1))
            {
                Graph G = IOGraph.ReadGraph(map + Filter1);
                Filter.RemoveTwoDegree(G);
                IOGraph.WriteGraph(G, map + Filter2);
                return G;
            }
            else if (IOGraph.DoesGraphsExists(map + OriginalGraph))
            {
                Graph G = IOGraph.ReadGraph(map + OriginalGraph);
                Filter.RemoveOneDegree(G);
                IOGraph.WriteGraph(G, map + Filter1);
                Filter.RemoveTwoDegree(G);
                IOGraph.WriteGraph(G, map + Filter2);
                return G;
            }
            else if (IOGraph.DoesGraphsExists(map + Unfiltered))
            {
                Graph G = IOGraph.ReadGraph(map + Unfiltered);
                G = Filter.ConnectedComponent(G);
                IOGraph.WriteGraph(G, map + OriginalGraph);
                Filter.RemoveOneDegree(G);
                IOGraph.WriteGraph(G, map + Filter1);
                Filter.RemoveTwoDegree(G);
                IOGraph.WriteGraph(G, map + Filter2);
                return G;
            }
            else
            {

                Graph G = Parser.ParseCSVFile(map);
                IOGraph.WriteGraph(G, map + Unfiltered);
                G = Filter.ConnectedComponent(G);
                IOGraph.WriteGraph(G, map + OriginalGraph);
                Filter.RemoveOneDegree(G);
                IOGraph.WriteGraph(G, map + Filter1);
                Filter.RemoveTwoDegree(G);
                IOGraph.WriteGraph(G, map + Filter2);
                return G;
            }
        }

        public static Graph GetOriginalGraph(string map) {
            string OriginalGraph = "OG";
            return IOGraph.ReadGraph(map + OriginalGraph);
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
