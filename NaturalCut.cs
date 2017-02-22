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
using Google.OrTools.Graph;

namespace GPNC
{
    static class NaturalCut
    {
        public static HashSet<Edge> MakeCuts(Graph G, int U,double alpha, double f)
        {
            HashSet<int> allNodes = new HashSet<int>();
            foreach (int id in G.nodes)
            {
                allNodes.Add(id);
            }
            
            HashSet<Edge> allCuts = new HashSet<Edge>();
            Random rnd = new Random();
            while (allNodes.Count > 0)
            {

                int rID = RandomID(allNodes,rnd);
                BFS bfs = new BFS(G, U, alpha, f, rID);
                Graph G2 = G.CreateSubGraph(bfs.SubGraph);
                Tuple<List<int>, List<Edge>> partitionAndCut = MinCut.partition(G,bfs);

                //removes all nodes that are inside the cut
                partitionAndCut.Item1.ForEach(x => allNodes.Remove(x));

                //adds all cutEdges
                partitionAndCut.Item2.ForEach(x => allCuts.Add(x));
            }

            return allCuts;
        }
       
        private static int RandomID(HashSet<int> allNodes,Random rnd) {

            return allNodes.ToList()[rnd.Next(allNodes.Count)];
        }
    }


}
