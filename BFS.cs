using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    class BFS
    {

        public List<int> T = new List<int>();
        public List<int> Core = new List<int>();
        public List<int> AltCore = new List<int>();
        public List<int> Ring;
        public List<int> SubGraph;
        Queue<int> queue = new Queue<int>();
        HashSet<int> visited = new HashSet<int>();
        Graph G;
        private int maxTree;
        private int maxCore;
        private int size;

        public BFS(Graph g, int U, double alpha, double f, int startNode)
        {
            G = g;
            maxTree = (int)(U * alpha);
            maxCore = (int)((double)U / f);
            size = 0;
            run(startNode);
        }

        public BFS(Graph g, int U, double alpha, double f, int startNode, bool alternativeCoreDefinition)
        {
            G = g;
            maxTree = (int)(U * alpha);
            maxCore = (int)((double)U / f);
            size = 0;
            run(startNode);
        }

        // Runs the BFS and stores the Core and Ring
        private void run(int startNode)
        {
            visitNode(startNode);

            while (queue.Count > 0 && size < maxTree)
            {
                int currentNode = queue.Dequeue();

                foreach (int nId in G.GetNeighbours(currentNode))
                {
                        visitNode(nId);
                }

            }
            Ring = queue.ToList();
            SubGraph = T.Union(Ring).ToList();

        }

        private void visitNode(int v)
        {
            if (!visited.Contains(v))
            {
                size += G.Size[v];
                queue.Enqueue(v);
                if (Core.Count < maxCore)
                {
                    Core.Add(v);
                }
                T.Add(v);
                visited.Add(v);
            }
        }
    }
}
