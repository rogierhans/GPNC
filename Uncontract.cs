using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    static class Uncontract
    {
        public static Dictionary<int, HashSet<int>>  GetPartitions( Graph G, Graph OG) {
            Dictionary<int, HashSet<int>> realPS = new Dictionary<int, HashSet<int>>();
            foreach (int id in OG.nodes)
            {
                int currentNode = id;
                while (!G.nodes.Contains(currentNode))
                {
                    currentNode = G.Parent[currentNode];
                }
                if (!realPS.ContainsKey(currentNode))
                {
                    HashSet<int> par = new HashSet<int>();
                    realPS[currentNode] = par;
                }
                realPS[currentNode].Add(id);
            }
            foreach (var kvp in realPS)
            {
                Console.WriteLine(kvp.Key + " " + kvp.Value.Count);
            }
            return realPS;
        }
    }
}
