﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;
using System.Diagnostics;


namespace GPNC
{
    class Program
    {
        static void Main(string[] args)
        {

            //List<GeoPoint> points = new List<GeoPoint>();
            //Random rng = new Random();
            //for (int i = 0; i < 10; i++)
            //{
            //    int x = rng.Next() / 10000;
            //    int y = rng.Next() / 10000;
            //    points.Add(new GeoPoint(x, y));
            //}
            //points.ForEach(x => Console.WriteLine(x));
            //KDTree testTree = new KDTree(points, 0);
            //Console.WriteLine(testTree.ToString(0));
            //int minx = points.Min(gp => gp.X);
            //int miny = points.Min(gp => gp.Y);
            //GeoPoint GPMin = new GeoPoint(minx, miny);  
            //GeoPoint GPMax = new GeoPoint(points.Max(x => x.X), points.Max(x => x.Y));
            //Range allRange = new Range();
            //allRange.Low = GPMin;
            //allRange.High = GPMax;
            //Console.WriteLine("ok");
            //Console.WriteLine($"Deze man {allRange}");
            //testTree.ReportPoints().ForEach(x => Console.WriteLine(x));
            //Console.WriteLine("ok");

            //int distance = 500;
            //GeoPoint first = points.First();
            //Range specific  = new Range(new GeoPoint(first.X - distance, first.Y - distance), distance *2, distance*2);
            ////testTree.GetRange(new Range(new GeoPoint(minx-100, miny-100), 200, 200)).ForEach(x => Console.WriteLine(x));
            //testTree.GetRange(specific).ForEach(x => Console.WriteLine(" xD"+x));
            //Console.WriteLine(testTree.test(allRange));
            ////testTree.makePicture(specific);
            ////testTree.GetAllRange().ForEach(x => Console.WriteLine(x));

            //IOGraph.MakeMap("DU", 6);


            // //Console.ReadLine();
            // //return;
            double alpha = 1;
            int U = 100000;
            string map = "NL";
            //double f = 20;
            //string Fragmented = "FG";
            //string Solution = "SG";
            //bool RemoveCore = false;
            //// Graph G = IOGraph.GetFilteredGraph(map);
            Graph OG = IOGraph.GetOriginalGraph(map);


            //Graph OG = IOGraph.GetOriginalGraph(map);
            //int min = OG.nodes.Min(x => x);
            //int max = OG.nodes.Max(x => x);
            //Console.WriteLine($"{min} en {max}");


            //return;
            //Graph OG = IOGraph.GetOriginalGraph(map);
            //var nodes = Parser.ParseNodes(OG, map);

            //KDTree tree = new KDTree(nodes.Values.ToList());


            //Console.WriteLine(odfake.Count);
            //Report report = new Report(map, 25, U, alpha, 60, false, odfake, "yolotest");
            //var OD100 = tree.GetNodesMostDense(100);
            //var OD200 = tree.GetNodesMostDense(200);
            //var OD400 = tree.GetNodesMostDense(400);
            //var OD800 = tree.GetNodesMostDense(800);
            //var OD1600 = tree.GetNodesMostDense(1600);
            //var sw = new Stopwatch();
            //sw.Start();

            //for (int f = 20; f <=100; f =  f+20)
            //{
            //    for (int i = 0; i < 5; i++)
            //    {
            //        Report r = new Report(map,25,U,alpha,f,false, (int)Math.Pow(2, i) * 100,i.ToString());
            //        r.WriteLogToFile();
            //    }

            //}
            //for (int i = 0; i < 5; i++)
            //{
            //    Report r = new Report(map, 25, U, alpha, 10, true, (int)Math.Pow(2, i) * 100, i.ToString()); 
            //    r.WriteLogToFile();
            //}
            //var G = IOGraph.GetOriginalGraph(map);
            //var nodes = Parser.ParseNodes(G, map);
            //KDTree kdtree = new KDTree(nodes.Values.ToList());

            Report report = new Report(map, 25, U, alpha, 100, false, 35, 35.ToString());
            report.WriteLogToFile();

            //Print.PrintOrder(G, nodes, od, ((int)Math.Pow(2, i) * 100).ToString());

            //}
            //return;
            //Print.PrintGrid(OG, nodes, tree.GetNodesOnGrid(40),40);
            //Print.PrintGridSquare(OG, nodes, tree.test(20));
            //return;
            //int minx = nodes.Values.Min(gp => gp.X);
            //int miny = nodes.Values.Min(gp => gp.Y);
            //int maxx = nodes.Values.Max(gp => gp.X);
            //int maxy = nodes.Values.Max(gp => gp.Y);
            //int XUnits = (maxx - minx) / 200;
            //int YUnits = (maxy - miny) / 200;
            //Dictionary<int, int> scores = new Dictionary<int, int>();
            //foreach (int id in OG.nodes)
            //{
            //    GeoPoint gp = nodes[id];
            //    int score = tree.CountUnits(gp,1);
            //    scores[id] = score;
            //}
            //var orded = from pair in scores
            //         orderby pair.Value descending
            //         select pair.Key;
            //List<int> OrdedNodes = orded.ToList();
            //var watch = System.Diagnostics.Stopwatch.StartNew();

            ////make cuts and get the fragments

            //// report the time




            //foreach (int U in new int[] { 100000, 250000 })
            //{
            //    for (double f = 30; f <= 100; f = f + 10)
            //    {

            //        Report report = new Report(map, 25, U, alpha, f, false, null);
            //        report.WriteLogToFile();
            //    }
            //}
            //foreach (int U in new int[] {250000 })
            //{
            //    for (double f = 5; f <= 20; f = f + 5)
            //    {
            //        foreach (bool rc in new bool[] { false, true })
            //        {
            //            Report report = new Report(map, 25, U, alpha, f, rc, null);
            //            report.WriteLogToFile();
            //        }
            //    }
            //}

            //Graph G = IOGraph.GetFilteredGraph(map);
            //G = FindFragments.GetFragmentedGraph(G, U, alpha, f, RemoveCore, null,nodes);
            //Graph FG = G.CreateSubGraph(G.nodes.ToList());
            //G.ApplyGreedyAlgorithm(U);
            //G = LocalSearch.Search1(G, FG, U);
            ////watch.Stop();
            ////Console.WriteLine(watch.ElapsedMilliseconds);
            ////Console.ReadLine();
            //Print.makePrints(G, OG, nodes, U / 1000 + "f" + f + map + (RemoveCore ? "T" : "F"));
            return;
            //Print.PrintHeatMap(OG,nodes,scores);
            // int distance = 100000;
            // Dictionary<int, int> scores = new Dictionary<int, int>();
            // foreach (int id in OG.nodes)
            // {
            //     GeoPoint gp = nodes[id];
            //     Range range = new Range(new GeoPoint(gp.X - distance, gp.Y - distance), distance * 2, distance * 2);
            //     int score = tree.Count(range);
            //     scores[id] = score;
            // }

            // tree.makePicture(new Range(new GeoPoint(nodes[OG.nodes.ToList()[10000]].X - distance, nodes[OG.nodes.ToList()[10000]].Y - distance), distance * 2, distance * 2));
            // scores.OrderBy(x => x.Value);
            // var kvp = scores.First();
            // var kvpl = scores.Last();
            // Console.WriteLine($"{kvp.Key} met {kvp.Value}");
            // Console.WriteLine($"{kvpl.Key} met {kvpl.Value}");
            // Print.PrintHeatMap(OG, nodes, scores);

            // Console.ReadLine();
            // return;
            //Graph G = IOGraph.GetFilteredGraph(map);
            //HashSet<Edge> cuts = NaturalCut.MakeCuts(null, G, U, alpha, f, RemoveCore);
            //List<List<int>> ps = FindFragments.FindPartions(G, cuts, U);
            //ps.ForEach(x => { int v = G.ContractList(x); });
            //Graph FG = G.CreateSubGraph(G.nodes.ToList());
            //G.ApplyGreedyAlgorithm(U);
            //G = LocalSearch.Search1(G, FG, U);
            //Graph OG = IOGraph.GetOriginalGraph(map);
            //var nodes = Parser.ParseNodes(OG, "EA");
            //Print.makePrints(G, OG, nodes, U / 1000 + "f" + f + map + (RemoveCore ? "T" : "F"));


            ////Find natural cuts and contract them
            //foreach (int U in new int[] { 250000, 1000000 })
            //{
            //    for (double f = 20; f <= 60; f = f + 20)
            //    {
            //        Report report = new Report(map, 5, U, alpha, f, RemoveCore);
            //        report.WriteLogToFile();
            //    }
            //}
            //RemoveCore = true;
            ////Find natural cuts and contract them
            //foreach (int U in new int[] { 250000, 1000000 })
            //{
            //    for (double f = 5; f <= 25; f = f + 5)
            //    {
            //        Report report = new Report(map, 10, U, alpha, f, RemoveCore);
            //        report.WriteLogToFile();
            //    }
            //}
            //for (int i = 0; i < 10; i++)
            //{
            //    Graph G = GetFilteredGraph(map);
            //    HashSet<Edge> cuts = NaturalCut.MakeCuts(null, G, U, alpha, f, RemoveCore);
            //    List<List<int>> ps = FindFragments.FindPartions(G, cuts, U);
            //    ps.ForEach(x => { int v = G.ContractList(x); });
            //    IOGraph.WriteGraph(G, map + Fragmented + U / 1000);
            //    Graph FG = G.CreateSubGraph(G.nodes.ToList());
            //    G.ApplyGreedyAlgorithm(U);
            //    G = LocalSearch.Search1(G, FG, U);
            //    IOGraph.WriteGraph(G, map + Solution + U / 1000);
            //    Graph OG = GetOriginalGraph(map);
            //    var nodes = Parser.ParseNodes(OG, map);
            //    Print.makePrints(G, OG, nodes, U / 1000 + "f" + f + map + (RemoveCore ? "T" : "F"));

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



    }

    public class Report
    {
        List<SingleRun> Runs = new List<SingleRun>();
        string Map;
        int U;
        int N;
        double Alpha;
        double F;
        bool RemoveCore;
        string path = "F:\\Users\\Rogier\\Google Drive\\Log\\";
        string ExtraName;
        public Report(string map, int n, int u, double alpha, double f, bool removeCore, int gridsize, string extraName)
        {
            Map = map;
            ExtraName = extraName;
            N = n;
            U = u;
            Alpha = alpha;
            F = f;
            RemoveCore = removeCore;


            Graph OG = IOGraph.GetOriginalGraph(map);
            var nodes = Parser.ParseNodes(OG, map);



            string Solution = "SG";
            var pathLog = path + "log.csv";


            Graph GG = IOGraph.GetFilteredGraph(map);
            var TreeNodes = Parser.ParseNodes(GG, map);
            KDTree tree = new KDTree(TreeNodes.Values.ToList());
            //var scores = tree.GetScores(gridsize);
            for (int i = 0; i < n; i++)
            {


                // Create object to store information about the run
                SingleRun run = new SingleRun();

                //get the (filtered) graph
                Graph G = IOGraph.GetFilteredGraph(map);

                //create stopwatch to measure time
                var watch = System.Diagnostics.Stopwatch.StartNew();

                //make cuts and get the fragments
                var OD = tree.GetNodesOnGrid(gridsize);
                G = FindFragments.GetFragmentedGraph(G, U, alpha, f, RemoveCore, OD, null);

                // report the time
                watch.Stop();
                var timeCutFinding = watch.ElapsedMilliseconds;
                run.TimeCutFinding = timeCutFinding;

                // report the amount of fragments created
                run.Fragments = G.nodes.Count;

                //create a copy for later use
                //TODO this could also be done in the Local Search
                Graph FG = G.CreateSubGraph(G.nodes.ToList());

                // restart the stopwatch
                watch = System.Diagnostics.Stopwatch.StartNew();

                // Do the intial greedy algorithm
                G.ApplyGreedyAlgorithm(U);

                //report the time
                watch.Stop();
                var timeGreedy = watch.ElapsedMilliseconds;
                run.TimeGreedy = timeGreedy;


                //count the intial cutweight
                //int weight = G.GetAllArcs().Sum(e => G.getWeight(e.To, e.From));
                //run.GreedyCut = weight;

                // restart the stopwatch
                watch = System.Diagnostics.Stopwatch.StartNew();

                // do local search on Graph
                G = LocalSearch.Search1(G, FG, U);

                //report the time
                watch.Stop();
                var timeLocalSearch = watch.ElapsedMilliseconds;
                run.TimeLocalSearch = timeLocalSearch;


                //write the solution found to file
                IOGraph.WriteGraph(G, map + Solution + U / 1000 + i + "q" + f + (RemoveCore ? "T" : "F"));

                //Graph OG = IOGraph.GetOriginalGraph(map);
                run.SetValues(G, OG);
                run.WriteToFile(pathLog);


                Print.makePrints(G, OG, nodes, U / 1000 + "f" + f + map + i + (RemoveCore ? "T" : "F"));
                Runs.Add(run);
            }
        }

        public void WriteLogToFile()
        {
            List<string> lines = new List<string>();
            lines.Add(Runs.First().GetFirstLine());
            lines.Add(GetAverages());

            foreach (SingleRun run in Runs)
            {
                lines.Add(run.GetString());
            }
            File.WriteAllLines(path + Map + U / 1000 + F + N + (RemoveCore ? "T" : "F") + ExtraName + "report.csv", lines.ToArray());
        }

        private string GetAverages()
        {
            List<string> allAvgs = new List<string>();
            double totalTimeCutFinding = 0;
            double totalTimeGreedy = 0;
            double totalTimeLocalSearch = 0;
            //int totalCQMMeasure = 0;
            int totalBoundaryNodes = 0;
            //int totalGreedyCut = 0;
            int totalCut = 0;
            int totalFragments = 0;
            foreach (SingleRun run in Runs)
            {
                totalTimeCutFinding += run.TimeCutFinding;
                totalTimeGreedy += run.TimeGreedy;
                totalTimeLocalSearch += run.TimeLocalSearch;
                //totalCQMMeasure += run.CQMMeasure;
                totalBoundaryNodes += run.BoundaryNodes;
                //totalGreedyCut += run.GreedyCut;
                totalCut += run.Cut;
                totalFragments += run.Fragments;
            }
            foreach (SingleRun run in Runs)
            {
                allAvgs.Add(((int)totalTimeCutFinding / Runs.Count).ToString());
                allAvgs.Add(((int)totalTimeGreedy / Runs.Count).ToString());
                allAvgs.Add(((int)totalTimeLocalSearch / Runs.Count).ToString());
                //allAvgs.Add((totalCQMMeasure / Runs.Count).ToString());
                allAvgs.Add((totalBoundaryNodes / Runs.Count).ToString());
                //allAvgs.Add((totalGreedyCut / Runs.Count).ToString());
                allAvgs.Add((totalCut / Runs.Count).ToString());
                allAvgs.Add((totalFragments / Runs.Count).ToString());
            }
            return String.Join(",", allAvgs);
        }
    }

    public class SingleRun
    {
        public double TimeCutFinding;
        public double TimeGreedy;
        public double TimeLocalSearch;
        //public int CQMMeasure;
        public int BoundaryNodes;
        //public int GreedyCut;
        public int Cut;
        public int Fragments;

        public void WriteToFile(string filename)
        {
            string line = GetString();
            if (!File.Exists(filename))
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine(GetFirstLine());
                    sw.WriteLine(line);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(line);
                }
            }
        }

        public string GetFirstLine()
        {
            return String.Join(",", new List<string>() { "Finding Cuts", "Greedy", "Local Search", "Fragments", "BoundaryNodes", "Cut" });
        }

        public string GetString()
        {
            List<string> info = new List<string>();
            info.Add(TimeCutFinding.ToString());
            info.Add(TimeGreedy.ToString());
            info.Add(TimeLocalSearch.ToString());
            //info.Add(CQMMeasure.ToString());
            info.Add(Fragments.ToString());
            info.Add(BoundaryNodes.ToString());
            //info.Add(GreedyCut.ToString());
            info.Add(Cut.ToString());

            return String.Join(",", info);
        }

        private Dictionary<int, HashSet<int>> partitionToBoundaryNodes = new Dictionary<int, HashSet<int>>();
        public void SetValues(Graph G, Graph OG)
        {
            var partitions = Uncontract.GetPartitions(G, OG);
            foreach (int ids in partitions.Keys)
            {
                partitionToBoundaryNodes[ids] = new HashSet<int>();
            }
            List<Edge> cutEdges = new List<Edge>();
            foreach (Edge e in G.GetAllArcs())
            {
                HashSet<int> par1 = partitions[e.From];
                HashSet<int> par2 = partitions[e.To];
                foreach (int from in par1)
                {
                    foreach (int to in OG.GetNeighbours(from))
                    {
                        if (par2.Contains(to))
                        {
                            partitionToBoundaryNodes[e.From].Add(from);
                            partitionToBoundaryNodes[e.To].Add(to);
                            cutEdges.Add(new Edge(from, to));
                        }
                    }
                }
            }
            Cut = cutEdges.Count;

            BoundaryNodes = 0;
            partitionToBoundaryNodes.Values.ToList().ForEach(x => BoundaryNodes += x.Count);

            //CQMMeasure = -1;
            //(int)QualityNode(G, OG, partitions);
        }
        public double QualityNode(Graph G, Graph OG, Dictionary<int, HashSet<int>> partitions)
        {
            double sum = 0;
            double totalSize = G.nodes.Sum(x => G.Size[x]);
            foreach (int i in G.nodes)
            {
                foreach (int j in G.nodes)
                {

                    if (i != j)
                    {
                        double pi = G.Size[i] / totalSize;
                        double pj = G.Size[j] / totalSize;
                        double value = pi * pj * (CountEdge(OG, partitions[i]) + CountEdge(OG, partitions[j]));
                        sum += value;
                    }
                }
            }
            return SplitQuality(G) + sum;
        }
        public int CountEdge(Graph OG, HashSet<int> partition)
        {
            return OG.CreateSubGraph(partition.ToList()).GetAllArcs().Count;
        }

        public double SplitQuality(Graph G)
        {
            double sum = 0;
            foreach (int id in G.nodes)
            {
                sum += Math.Pow(partitionToBoundaryNodes[id].Count, 1.5);
            }
            return sum + Cut;
        }
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
