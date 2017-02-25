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
            int U = 100000;

            ////read graph
            //Graph G = Parser.ParseCSVFile();
            //Console.WriteLine(G.nodes.Count);

            ////Filter unconnected nodes
            //G = Filter.ConnectedComponent(G);

            ////copy original graph
            //Graph OG = G.CreateSubGraph(G.nodes.ToList());
            //Console.WriteLine(G.nodes.Count);
            //IOGraph.WriteGraph(OG, "OG");

            ////Contract One degree
            //Filter.RemoveOneDegree(G);
            //Console.WriteLine(G.nodes.Count);

            ////Contract two degree
            //Filter.RemoveTwoDegree(G);
            //Console.WriteLine(G.nodes.Count);
            //IOGraph.WriteGraph(G, "RG");
            ////Graph G = IOGraph.ReadGraph("RG");

            ////Find natural cuts and contract them
            //HashSet<Edge> cuts = NaturalCut.MakeCuts(G, U, alpha, f);
            //List<List<int>> ps = FindFragments.FindPartions(G, cuts, U);
            //ps.ForEach(x => { int v = G.ContractList(x); });
            //IOGraph.WriteGraph(G, "FG");
            Graph OG = IOGraph.ReadGraph("OG");
            Graph RG = IOGraph.ReadGraph("RG");
            Dictionary<int, NodePoint> nodes = Parser.ParseNodes(OG);
            Tree T = new Tree(OG, RG, U, 10000, alpha, f,new HashSet<int>(),nodes,0.ToString());

            Console.WriteLine(T.Quality());
            T.Print(0);
            //Graph G = IOGraph.ReadGraph("FG");


            //Graph FG = G.CreateSubGraph(G.nodes.ToList());
            //Console.WriteLine(G.nodes.Count);
            //G.ApplyGreedyAlgorithm(U);
            //G = LocalSearch.Search1(G, FG, U);
            //Dictionary<int ,HashSet<int>> partitions = Uncontract.GetPartitions(G, OG);


            //Print.makePrints(G, OG, nodes);

            Console.ReadLine();
        }
    }

    //Graph test = G.CreateSubGraph(G.nodes.ToList());
    //var now = DateTime.Now.Second;
    //test = FindFragments.FastContraction(test,cuts,U);
    //        Console.WriteLine(DateTime.Now.Second -  now);
    //        Console.ReadLine();
    //        now = DateTime.Now.Second;

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
