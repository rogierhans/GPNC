using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    static class FindFragments
    {
        public static Graph FastContraction(Graph G, HashSet<Edge> cuts, int U) {
            G.GetAllArcs().ForEach(x => { if (!cuts.Contains(x)) G.ContractionAlt(x.From,x.To); });
            return G;
        }


        public static List<List<int>> FindPartions(Graph G, HashSet<Edge> cuts, int U)
        {
            List<List<int>> ps = new List<List<int>>();
            HashSet<int> allNodes = new HashSet<int>();
            G.nodes.ToList().ForEach(x => allNodes.Add(x));
            Random rnd = new Random();
            while (allNodes.Count > 0)
            {
                int rID = RandomID(allNodes,rnd);
                List<int> p = partitioning(G, cuts, rID, U);
                p.ForEach(x => allNodes.Remove(x));
                ps.Add(p);
            }
            return ps;
        }
        private static List<int> partitioning(Graph G, HashSet<Edge> cuts, int startNode, int U)
        {
            List<int> p = new List<int>();
            Queue<int> queue = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();
            queue.Enqueue(startNode);
            visited.Add(startNode);
            p.Add(startNode);
            while (queue.Count > 0)
            {
                int currentNode = queue.Dequeue();
                var neighbours = G.GetNeighbours(currentNode);
                foreach (int id in neighbours)
                {
                    if (!visited.Contains(id) && !cuts.Contains(new Edge(currentNode, id)))
                    {
                        p.Add(id);
                        visited.Add(id);
                        queue.Enqueue(id);
                    }
                }
            }
            int size = 0;
            p.ForEach(x => size += G.Size[x]);
            if (size > U)
                Console.WriteLine(size + "Size");


            return p;
        }
        private static int RandomID(HashSet<int> allNodes,Random rnd)
        {

            int index = rnd.Next(allNodes.Count);
            return allNodes.ElementAt(index);
        }
        public static void exportIds(List<int> ids, string nameFile)
        {
            String[] strings = new String[ids.Count];

            for (int i = 0; i < ids.Count; i++)
            {
                strings[i] = ids[i].ToString();
            }
            System.IO.File.WriteAllLines("F:\\Users\\Rogier\\Desktop\\" + nameFile + ".csv", strings);
        }
    }
}
