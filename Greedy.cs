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

            Random r = new Random();
            Dictionary<Edge, double> scores = new Dictionary<Edge, double>();
            G.GetAllArcs(U).ForEach(e =>
            {
                int u = e.To;
                int v = e.From;
                scores[e] = randomNumber(r) * ((G.getWeight(u, v) / Math.Sqrt(G.Size[u])) + (G.getWeight(u, v) / Math.Sqrt(G.Size[v])));
            });

            while (scores.Count > 0)
            {
                //O(n) -> alleen neighbours
                Edge bestEdge = getBestEdge(scores);
                int newID = bestEdge.From;
                int deletedId = bestEdge.To;
                List<int> neighbours = G.GetNeighbours(deletedId);
                List<int> neighboursND = G.GetNeighbours(newID);
                //Console.WriteLine("u:{0}::{1} - v:{2}::{3}", bestEdge.From, G.Size[bestEdge.From], bestEdge.To, G.Size[bestEdge.To]);

                G.Contraction(newID, deletedId);
                if (!scores.Remove(bestEdge)) Console.WriteLine("HDNW");

                //Change edges
                foreach (int neighbour in neighbours)
                {
                    Edge oldEdge = new Edge(deletedId, neighbour);
                    Edge newEdge = new Edge(newID, neighbour);
                    if (scores.ContainsKey(oldEdge))
                    {
                        scores.Remove(oldEdge);
                        AddEdge(G, scores, U, newEdge, r);

                    }
                }
                //remove edges that have become to big  
                foreach (int neighbour in neighboursND)
                {
                    Edge newEdge = new Edge(newID, neighbour);
                    if (scores.ContainsKey(newEdge))
                    {
                        if (G.Size[newEdge.To] + G.Size[newEdge.From] >= U)
                            scores.Remove(newEdge);
                    }
                }

            }

        }



        private static void AddEdge(Graph G, Dictionary<Edge, double> scores, int U, Edge e,Random r)
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


    }
}
