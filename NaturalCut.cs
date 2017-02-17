using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    static class NaturalCut
    {
        public static HashSet<Edge> MakeCuts(Graph G)
        {
            HashSet<int> allNodes = new HashSet<int>();
            foreach (int id in G.nodes)
            {
                allNodes.Add(id);
            }
            
            HashSet<Edge> allCuts = new HashSet<Edge>();
            while (allNodes.Count > 0)
            {

                int rID = RandomID(allNodes);
                BFS bfs = new BFS(G, 250000, 1, 10, rID);
                Graph G2 = G.CreateSubGraph(bfs.SubGraph);
                MinCut minCut = new MinCut();
                List<int> p = minCut.partition(G2, bfs.Core, bfs.Ring);
                p.ForEach(x => allNodes.Remove(x));
                minCut.cut.ForEach(x => allCuts.Add(x));
                Console.WriteLine(allNodes.Count + "to GO");
            }
            return allCuts;
        }

        private static int RandomID(HashSet<int> allNodes) {
            Random rnd = new Random();
            return allNodes.ToList()[rnd.Next(allNodes.Count)];
        }
    }


}

