using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    class EdgeStructure
    {

        Dictionary<Edge, double> scores = new Dictionary<Edge, double>();
        private SortedSet<Edge> sortedEdges;
        Random r = new Random();
        int U;
        Graph G;
        public EdgeStructure(Graph g, int u)
        {
            G = g;
            U = u;
            sortedEdges = new SortedSet<Edge>(new EdgeCompare(scores));
            G.GetAllArcs(U).ForEach(e =>
            {
                AddEdge(e);
            });
        }
        public bool NonEmpty()
        {
            return scores.Count > 0;
        }
        public Edge BestEdgeToContract()
        {
            Edge e = sortedEdges.Max;
            RemoveEdge(e);
            return e;
        }
        internal void UpdateStructure(int newID, int deletedId)
        {
            List<int> neighbours = G.GetNeighbours(newID);
            //Change edges
            foreach (int neighbour in neighbours)
            {
                Edge oldEdge = new Edge(deletedId, neighbour);
                Edge newEdge = new Edge(newID, neighbour);

                if (Contains(oldEdge))
                {
                    RemoveEdge(oldEdge);
                    AddEdge(newEdge);
                }
                if (Contains(newEdge))
                {
                    if (G.Size[newEdge.To] + G.Size[newEdge.From] >= U)
                        RemoveEdge(newEdge);
                }
            }
        }

        private void RemoveEdge(Edge e)
        {
            sortedEdges.Remove(e);
            scores.Remove(e);
            //Console.WriteLine("Remove {0} score:{1} set{2}", e, scores.Count, sortedEdges.Count);
        }
        private void AddEdge(Edge e)
        {
            if (Contains(e)) RemoveEdge(e);
            if (G.Size[e.To] + G.Size[e.From] < U)
            {
                int u = e.To;
                int v = e.From;
                scores[e] = randomNumber() * ((G.getWeight(u, v) / Math.Sqrt(G.Size[u])) + (G.getWeight(u, v) / Math.Sqrt(G.Size[v])));
                sortedEdges.Add(e);
            }
            //Console.WriteLine("Add {0} score:{1} set{2}", e, scores.Count, sortedEdges.Count);
        }
        private bool Contains(Edge e)
        {
            return scores.ContainsKey(e);
        }

        private double randomNumber()
        {
            double a = 0.03;
            double b = 0.6;

            double randomNumber = r.NextDouble();
            if (randomNumber < a)
            {
                return r.NextDouble() * b;
            }
            else
            {
                return (r.NextDouble() * (1 - b)) + b;
            }
        }


        class EdgeCompare : IComparer<Edge>
        {
            Dictionary<Edge, double> scores;
            public EdgeCompare(Dictionary<Edge, double> scores) { this.scores = scores; }
            public int Compare(Edge x, Edge y)
            {
                return scores[x].CompareTo(scores[y]);
            }
        }
    }
}
