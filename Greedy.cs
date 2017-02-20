using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPNC
{
    static class Greedy
    {

        public static void initPar(Graph G, int U)
        {
            mergeGraph(G, U);
        }

        public static void mergeGraph(Graph G, int U)
        {

            EdgeStructure ES = new EdgeStructure(U);
            G.GetAllArcs(U).ForEach(e =>
            {
                ES.AddEdge(G, e);
            });

            while (ES.NonEmpty())
            {
                //O(n) -> alleen neighbours
                Edge bestEdge = ES.BestEdge();
                int newID = bestEdge.From;
                int deletedId = bestEdge.To;
                List<int> neighbours = G.GetNeighbours(deletedId);
                List<int> neighboursND = G.GetNeighbours(newID);
                //Console.WriteLine("u:{0}::{1} - v:{2}::{3}", bestEdge.From, G.Size[bestEdge.From], bestEdge.To, G.Size[bestEdge.To]);

                G.Contraction(newID, deletedId);
                //ES.RemoveEdge(bestEdge);

                //Change edges
                foreach (int neighbour in neighbours)
                {
                    Edge oldEdge = new Edge(deletedId, neighbour);
                    Edge newEdge = new Edge(newID, neighbour);

                    if (ES.Contains(oldEdge))
                    {
                        ES.RemoveEdge(oldEdge);
                        ES.AddEdge(G, newEdge);
                    }
                }
                //remove edges that have become to big  
                foreach (int neighbour in neighboursND)
                {
                    Edge newEdge = new Edge(newID, neighbour);
                    if (ES.Contains(newEdge))
                    {
                        if (G.Size[newEdge.To] + G.Size[newEdge.From] >= U)
                            ES.RemoveEdge(newEdge);
                    }
                }

            }

        }



        private static void AddEdge(Graph G, Dictionary<Edge, double> scores, int U, Edge e, Random r)
        {
            if (G.Size[e.To] + G.Size[e.From] < U)
            {
                scores[e] = randomNumber(r) * ((G.getWeight(e.To, e.From) / Math.Sqrt(G.Size[e.To])) + (G.getWeight(e.To, e.From) / Math.Sqrt(G.Size[e.From])));
            }
        }
        private static Edge getBestEdge(Dictionary<Edge, double> scores)
        {
            Edge bestEdge = scores.Keys.First();
            double bestScore = 0;
            foreach (Edge e in scores.Keys)
            {
                int u = e.To;
                int v = e.From;
                double score = scores[e];
                if (score > bestScore)
                {
                    bestScore = score;
                    bestEdge = e;
                }
            }
            return bestEdge;
        }



        private static double randomNumber(Random r)
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
        class EdgeStructure
        {

            Dictionary<Edge, double> scores = new Dictionary<Edge, double>();
            private SortedSet<Edge> sortedEdges;
            Random r = new Random();
            int U;
            public EdgeStructure(int u)
            {
                U = u;
                sortedEdges = new SortedSet<Edge>(new EdgeCompare(scores));
            }
            public void RemoveEdge(Edge e)
            {
                sortedEdges.Remove(e);
                scores.Remove(e);
                //Console.WriteLine("Remove {0} score:{1} set{2}", e, scores.Count, sortedEdges.Count);
            }
            public void AddEdge(Graph G, Edge e)
            {
                if (Contains(e)) RemoveEdge(e);
                if (G.Size[e.To] + G.Size[e.From] < U)
                {
                    int u = e.To;
                    int v = e.From;
                    scores[e] = randomNumber(r) * ((G.getWeight(u, v) / Math.Sqrt(G.Size[u])) + (G.getWeight(u, v) / Math.Sqrt(G.Size[v])));
                    sortedEdges.Add(e);
                }
                //Console.WriteLine("Add {0} score:{1} set{2}", e, scores.Count, sortedEdges.Count);
            }
            public bool Contains(Edge e) { return scores.ContainsKey(e); }
            public Edge BestEdge()
            {
                Edge e = sortedEdges.Max;
                RemoveEdge(e);
                return e;
            }
            public bool NonEmpty() { return scores.Count > 0; }
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
