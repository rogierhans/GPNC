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
            double f = 20;
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
            //int U = G.nodes.Count / 10;
            //Console.WriteLine(U);

            ////Contract One degree
            //Filter.RemoveOneDegree(G);
            //Console.WriteLine(G.nodes.Count);

            ////Contract two degree
            //Filter.RemoveTwoDegree(G);
            //Console.WriteLine(G.nodes.Count);
            //IOGraph.WriteGraph(G, "RG");


            ////Find natural cuts and contract them
            //HashSet<Edge> cuts = NaturalCut.MakeCuts(G, U, alpha, f);
            //List<List<int>> ps = FindFragments.FindPartions(G, cuts, U);
            //ps.ForEach(x => { int v = G.ContractList(x); });
            //IOGraph.WriteGraph(G, "FG");


            Graph G = IOGraph.ReadGraph("FG");
            int combinedWeight = 0;
            G.nodes.ToList().ForEach(x => combinedWeight += G.Size[x]);
            int U = combinedWeight / 10;


            Dictionary<int, int> Parents = new Dictionary<int, int>(G.Parent);
            Graph FG = G.CreateSubGraph(G.nodes.ToList());
            Console.WriteLine(G.nodes.Count);

            //Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt"));
            Greedy.initPar(G, U);
            G = LocalSearch.Search1(G, FG, U);


            foreach (var kvp in G.Parent)
            {
                Parents[kvp.Key] = kvp.Value;
            }
            //IOGraph.WriteGraph(BestGraph, "BG");
            Graph OG = IOGraph.ReadGraph("OG");
            Dictionary<int, NodePoint> nodes = Parser.ParseNodes(OG);
            var realPS = Uncontract.getPartitions(G, OG, Parents);
            Print.makePrints(G, OG, nodes, realPS, Parents);

            Console.ReadLine();
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
