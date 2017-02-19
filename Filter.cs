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
        public static void RemoveOneDegree(Graph G)
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
            //Console.WriteLine(G.nodes.Count);
            //Console.WriteLine(possibleOneEdge.Count);
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
                //Console.WriteLine(G.nodes.Count);
                //Console.WriteLine(possibleOneEdge.Count);
            }
        }
        public static void RemoveTwoDegree(Graph G)
        {
            List<Edge> arcs = G.GetAllArcs();
            Dictionary<int, List<int>> ContractionList = new Dictionary<int, List<int>>();
            int test = 0;
            foreach (Edge e in arcs)
            {
                int v = e.To;
                int w = e.From;
                int degreeV = G.GetDegree(v);
                int degreeW = G.GetDegree(w);

                if (degreeV == 2 && degreeW == 2)
                {
                    test++;
                    if (!ContractionList.ContainsKey(v) && !ContractionList.ContainsKey(w))
                    {
                        List<int> CL = new List<int> { v, w };
                        ContractionList[v] = CL;
                        ContractionList[w] = CL;
                    }
                    else if (ContractionList.ContainsKey(v) && !ContractionList.ContainsKey(w))
                    {
                        List<int> CL = ContractionList[v];
                        CL.Add(w);
                        ContractionList[w] = CL;
                    }
                    else if (!ContractionList.ContainsKey(v) && ContractionList.ContainsKey(w))
                    {
                        List<int> CL = ContractionList[w];
                        CL.Add(v);
                        ContractionList[v] = CL;
                    }
                    else if (ContractionList.ContainsKey(v) && ContractionList.ContainsKey(w))
                    {
                        List<int> CL1 = ContractionList[v];
                        List<int> CL2 = ContractionList[v];
                        CL1 = CL1.Union(CL2).ToList();
                        foreach (int id in CL2)
                        {
                            ContractionList[id] = CL1;
                        }
                    }

                }

            }
            List<List<int>> contractions = ContractionList.Values.Distinct().ToList();
            //int dubbelTest = 0;
            //contractions.ForEach(x => { dubbelTest-- ; x.ForEach(y => dubbelTest++); });
            //Console.WriteLine(dubbelTest);
            //Console.ReadLine();
            contractions.ForEach(x => G.ContractList(x));
        }
    }
}
