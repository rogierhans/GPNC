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
using Google.OrTools.Graph;

namespace GPNC
{
    static class NaturalCut
    {
        public static HashSet<Edge> MakeCuts(Dictionary<int, NodePoint> nodes, Graph G, int U, double alpha, double f)
        {
            HashSet<int> allNodes = new HashSet<int>();
            foreach (int id in G.nodes)
            {
                allNodes.Add(id);
            }

            HashSet<Edge> allCuts = new HashSet<Edge>();
            Random rnd = new Random();
            CutFinder cutFinder = new CutFinder();
            while (allNodes.Count > 0)
            {

                int rID = RandomID(allNodes, rnd);
                BFS bfs = new BFS(G, U, alpha, f, rID);
                cutFinder.SolveOnGraph(G, bfs);
                //removes all nodes that are inside the cut
                cutFinder.Partition.ForEach(x => allNodes.Remove(x));
                //bfs.Core.ForEach(x => allNodes.Remove(x));

                //adds all cutEdges
                cutFinder.Cut.ForEach(x => allCuts.Add(x));


                //remove latr
                //HashSet<int> parpar = new HashSet<int>();
                //partitionAndCut.Item1.ForEach(e => parpar.Add(e));
                //Print.PrintCutFound(nodes, G.CreateSubGraphWithoutParent(bfs.SubGraph), bfs.SubGraph, partitionAndCut.Item1, parpar, bfs.Core, allNodes.Count.ToString());
                Console.WriteLine(allNodes.Count + "left");

            }

            return allCuts;
        }

        private static int RandomID(HashSet<int> allNodes, Random rnd)
        {

            return allNodes.ToList()[rnd.Next(allNodes.Count)];
        }

        
    }


}

