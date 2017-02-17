using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    static class Filter
    {
        public static Graph ConnectedComponent(Graph G)
        {
            BFS bfs = new BFS(G, G.nodes.Count, 1, 10, 1);
            return G.CreateSubGraph(bfs.SubGraph);
        }
        public static Graph RemoveOneDegree(Graph G)
        {
            HashSet<int> possibleOneEdge = new HashSet<int>();
            foreach (int id in G.nodes.ToList())
            {
                List<int> neighbours = G.GetNeighbours(id);
                if (neighbours.Count == 1)
                {
                    possibleOneEdge.Add(G.Contraction(neighbours[0], id));
                }
            }
            Console.WriteLine(G.nodes.Count);
            Console.WriteLine(possibleOneEdge.Count);
            while (possibleOneEdge.Count > 0)
            {
                List<int> loopList = possibleOneEdge.ToList();
                possibleOneEdge = new HashSet<int>();
                foreach (int id in loopList)
                {
                    List<int> neighbours = G.GetNeighbours(id);
                    if (neighbours.Count == 1)
                    {
                        possibleOneEdge.Add(G.Contraction(neighbours[0], id));
                    }
                }
                Console.WriteLine(G.nodes.Count);
                Console.WriteLine(possibleOneEdge.Count);
            }
            return G;
        }

    }
}
