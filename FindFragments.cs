using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    static class FindFragments
    {

        public static Graph GetFragmentedGraph(Graph G, int U, double alpha, double f, bool RemoveCore, List<int> OD)
        {
            NaturalCut NC = new NaturalCut(G, U, alpha, f, RemoveCore, OD);
            HashSet<Edge> cuts = NC.MakeCuts(null);
            List<List<int>> ps = FindFragments.FindPartitions(G, cuts, U);
            ps.ForEach(x => { int v = G.ContractList(x); });
            return G;
        }

        private static List<List<int>> FindPartitions(Graph G, HashSet<Edge> cuts, int U)
        {
            List<List<int>> partitions = new List<List<int>>();
            HashSet<int> allNodes = new HashSet<int>();
            G.nodes.ToList().ForEach(x => allNodes.Add(x));
            Random rnd = new Random();
            while (allNodes.Count > 0)
            {
                int rID = RandomID(allNodes, rnd);
                List<int> partition = FindSinglePartition(G, cuts, rID, U);
                partition.ForEach(x => allNodes.Remove(x));
                partitions.Add(partition);
            }
            return partitions;
        }
        private static List<int> FindSinglePartition(Graph G, HashSet<Edge> cuts, int startNode, int U)
        {
            List<int> partition = new List<int>();
            Queue<int> queue = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();
            queue.Enqueue(startNode);
            visited.Add(startNode);
            partition.Add(startNode);
            while (queue.Count > 0)
            {
                int currentNode = queue.Dequeue();
                var neighbours = G.GetNeighbours(currentNode);
                foreach (int id in neighbours)
                {
                    if (!visited.Contains(id) && !cuts.Contains(new Edge(currentNode, id)))
                    {
                        partition.Add(id);
                        visited.Add(id);
                        queue.Enqueue(id);
                    }
                }
            }

            return partition;
        }
        private static int RandomID(HashSet<int> allNodes, Random rnd)
        {
            int index = rnd.Next(allNodes.Count);
            return allNodes.ElementAt(index);
        }


    }
}
