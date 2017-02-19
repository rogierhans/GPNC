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
            HashSet<Edge> arcs = new HashSet<Edge>();
            G.GetAllArcs(U).ForEach(id => arcs.Add(id));

            while (arcs.Count > 0)
            {
                Edge bestEdge = arcs.First();
                double bestScore = 0;
                foreach (Edge e in arcs) {
                    int u = e.To;
                    int v = e.From;
                    double score = randomNumber() * ( (G.getWeight(u,v) / Math.Sqrt(G.Size[u])) + (G.getWeight(u, v) / Math.Sqrt(G.Size[v])));
                    if (score > bestScore) {
                        bestScore = score;
                        bestEdge = e;
                    }
                }
                //Console.WriteLine("u:{0}::{1} - v:{2}::{3}", bestEdge.From, G.Size[bestEdge.From], bestEdge.To, G.Size[bestEdge.To]);
                int newID = G.Contraction(bestEdge.From, bestEdge.To);
                //Console.WriteLine("new:{0} old{1}", newID, bestEdge.To);
                if(!arcs.Remove(bestEdge)) Console.WriteLine("HDNW");
                foreach(Edge arc in arcs.ToList())
                {


                    int v = arc.From;
                    int w = arc.To;
                    Edge te = new Edge(v, w);
                    if (G.Size[te.To] + G.Size[te.From] >= U) { arcs.Remove(te); }
                    if (v == bestEdge.To) {
                        Edge e = new Edge(w, newID);
                        if (G.Size[e.To] + G.Size[e.From] < U)
                        {
                            arcs.Add(e);
                        }
                        else { arcs.Remove(e); }
                        arcs.Remove(arc);
                    }
                    if (w == bestEdge.To)
                    {
                        Edge e = new Edge(v, newID);
                        if (G.Size[e.To] + G.Size[e.From] < U)
                        {
                            arcs.Add(e);
                        }
                        else { arcs.Remove(e); }
                        arcs.Remove(arc);
                    }

                }
            }

        }
        

        private static double randomNumber()
        {
            double a = 0.03;
            double b = 0.6;
            Random r = new Random();
            double randomNumber = r.NextDouble();
            if (randomNumber < a)
            {
                return r.NextDouble() * b;
            }
            else
            {
                return (r.NextDouble() * (1-b)) + b;
            }
        }


    }
}
