﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;


namespace GPNC
{
    class Program
    {
        static void Main(string[] args)
        {


            Graph G = Parser.ParseCSVFile();
            G.print();
            //List<int> test = G.nodes.Skip(5).Take(100000).ToList();
            //Console.ReadLine();
            //G.ContractList(test);


            BFS bfs = new BFS(G, G.nodes.Count, 1, 10, 1);

            G = G.CreateSubGraph(bfs.SubGraph);

            //int dedend = 0;
            //G.nodes.ToList().ForEach(x => { if (G.AllFromNodes[x].Count + G.AllToNodes[x].Count == 1) dedend++; });
            //int path = 0;
            //G.nodes.ToList().ForEach(x => { if (G.AllFromNodes[x].Count + G.AllToNodes[x].Count == 2) path++; });
            //Console.WriteLine("deadend{0}   path{1}", dedend, path);
            //Graph G = new Graph();
            //for (int i = 0; i < 10; i++)
            //{
            //    G.AddNodeToGraph(i);
            //}
            //for (int i = 0; i < 9; i++)
            //{

            //    //misschien hier iets aan doen 
            //    G.AddFromEgde(i, i + 1, 20 - (int)i);
            //    G.AddToEgde(i + 1, i);
            //}
            //G.print();
            //Console.ReadLine();
            //G.ContractList(new List<int> { 4, 5, 8 });
            //G.print();
            //Console.ReadLine();
            //G.ContractList(new List<int> { 9, 6 });
            //G.print();
            //Console.ReadLine();
            //Graph G2 = G.CreateSubGraph(new List<int> { 1,2,3});


            //Graph G2 = G.CreateSubGraph(G.nodes.Take(10000).ToList());
            //G2.print();

            Dictionary<int, NodePoint> nodes = Parser.ParseNodes(G);

            HashSet<int> allNodes = new HashSet<int>();
            foreach (int id in G.nodes)
            {
                allNodes.Add(id);
            }
            Random rnd = new Random();
            while (allNodes.Count > 0)
            {
                int rInt = allNodes.ToList()[rnd.Next(allNodes.Count)];
                Console.WriteLine("random node" + rInt);
                List<int> p = createPartition(nodes, G, rInt);
                p.ForEach(x => allNodes.Remove(x));
                Console.WriteLine(allNodes.Count + "to GO");
                Console.ReadLine();
            }



            Console.ReadLine();
        }

        static int i = 0;
        public static List<int> createPartition(Dictionary<int, NodePoint> nodes, Graph G, int startNode)
        {
            BFS bfs = new BFS(G, 250000, 1, 10, startNode);
            Console.WriteLine("Core size:" + bfs.Core.Count);
            Console.WriteLine("Contains random node: " + bfs.Core.Contains(startNode));
            Graph G2 = G.CreateSubGraph(bfs.SubGraph);
            MinCut minCut = new MinCut(G2, bfs.Core, bfs.Ring);

            test(nodes, bfs.T, minCut.partition, bfs.Core, i++ + "");
            smallPicturetest(nodes, bfs.T, minCut.partition,bfs.Core, i++ + "");
            Console.WriteLine("created Picture");
            return minCut.partition;
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

        static Graphics grap;
        public static void test(Dictionary<int, NodePoint> nodes, List<int> subGraph, List<int> partition, List<int> core, String filename)
        {

            var bmp = new Bitmap(1000, 1000);
            var gr = Graphics.FromImage(bmp);


            foreach (NodePoint np in nodes.Values)
            {
                gr.FillRectangle(Brushes.Red, np.x, np.y, 1, 1);
            }
            grap = gr;

            subGraph.ForEach(n =>
            {
                NodePoint np = nodes[n];
                gr.FillRectangle(Brushes.Yellow, np.x, np.y, 1, 1);
            });
            partition.ForEach(n =>
            {
                NodePoint np = nodes[n];
                gr.FillRectangle(Brushes.Blue, np.x, np.y, 1, 1);
            });
            core.ForEach(n =>
            {
                NodePoint np = nodes[n];
                gr.FillRectangle(Brushes.Green, np.x, np.y, 1, 1);
            });
            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\" +
                filename + "a.png";
            bmp.Save(path);

        }

        public static void smallPicturetest(Dictionary<int, NodePoint> nodes, List<int> subGraph, List<int> partition, List<int> core, String filename)
        {
            var bmp = new Bitmap(1000, 1000);
            var gr = Graphics.FromImage(bmp);
            int maxLati = subGraph.Max(x => nodes[x].lati);
            int minLati = subGraph.Min(x => nodes[x].lati);
            int maxLongi = subGraph.Max(x => nodes[x].longi);
            int minLongi = subGraph.Min(x => nodes[x].longi);
            subGraph.ForEach(n =>
            {
                NodePoint np = nodes[n];
                gr.FillRectangle(Brushes.Yellow, calcX(np, minLongi, maxLongi), calcY(np, minLati, maxLati), 1, 1);
            });
            partition.ForEach(n =>
            {
                NodePoint np = nodes[n];
                gr.FillRectangle(Brushes.Blue, calcX(np, minLongi, maxLongi), calcY(np, minLati, maxLati), 1, 1);
            });
            core.ForEach(n =>
            {
                NodePoint np = nodes[n];
                gr.FillRectangle(Brushes.Green, calcX(np, minLongi, maxLongi), calcY(np, minLati, maxLati), 1, 1);
            });
            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\" +
    filename + "b.png";
            bmp.Save(path);
        }
        public static int calcX(NodePoint p, int minLongi, int maxLongi)
        {
            int mx = maxLongi - minLongi;
            int hx = p.longi - minLongi;
            return (int)(((float)hx / mx) * 1000);
        }
        public static int calcY(NodePoint p, int minLati, int maxLati)
        {
            int my = maxLati - minLati;
            int hy = p.lati - minLati;
            return (int)(1000 - (((float)hy / my) * 1000));

        }

    }

    class Graph
    {
        public HashSet<int> nodes = new HashSet<int>();


        public Dictionary<int, HashSet<int>> AllReverseNeighbours = new Dictionary<int, HashSet<int>>();
        public Dictionary<int, HybridDictionary> AllWeights = new Dictionary<int, HybridDictionary>();



        //TODO add size and trace contractions

        public int AddNodeToGraph(int id)
        {
            if (!nodes.Contains(id))
            {
                nodes.Add(id);
                HashSet<int> reverseNeighbours = new HashSet<int>();
                HybridDictionary weights = new HybridDictionary();
                AllWeights[id] = weights;
                AllReverseNeighbours[id] = reverseNeighbours;
            }
            return id;
        }
        public void RemoveNodeFromGraph(int id)
        {
            nodes.Remove(id);
        }

        public int Contraction(int v, int w)
        {

            foreach (int x in GetNeighbours(w))
            {
                if (x != v)
                {

                    int weight = getWeight(w, x);
                    RemoveEdge(x, w);
                    AddEdge(v, x, weight);
                }
            }

            RemoveEdge(v, w);
            RemoveNodeFromGraph(w);
            return v;

        }

        public void ContractList(List<int> contractionList)
        {

            if (nodes.Count <= 1)
            {
                throw new Exception("CONTRACTING EMPTY/SINGLTON LIST BTFO");
            }
            int first = contractionList.First();
            foreach (int id in contractionList.Skip(1))
            {
                first = Contraction(first, id);
                //print();
                //Console.WriteLine(id);
            }
        }
        public List<int> GetNeighbours(int id)
        {
            List<int> neighbours = new List<int>();
            var fromNodes = AllWeights[id].Keys;
            var toNodes = AllReverseNeighbours[id];
            foreach (int v in fromNodes) neighbours.Add(v);
            foreach (int w in toNodes) neighbours.Add(w);

            return neighbours;
        }

        public void AddEdge(int id, int otherId, int weight)
        {
            int v = id > otherId ? id : otherId;
            int w = id > otherId ? otherId : id;

            if (!AllWeights[v].Contains(w))
            {
                AllWeights[v].Add(w, weight);
                AllReverseNeighbours[w].Add(v);
            }
            else
            {
                AllWeights[v][w] = (int)AllWeights[v][w] + weight;
            }
        }
        public void RemoveEdge(int id, int otherId)
        {
            int v = id > otherId ? id : otherId;
            int w = id > otherId ? otherId : id;
            AllWeights[v].Remove(w);
            AllReverseNeighbours[w].Remove(v);
        }

        public int getWeight(int id, int otherId)
        {
            int v = id > otherId ? id : otherId;
            int w = id > otherId ? otherId : id;
            return (int)AllWeights[v][w];
        }


        public void print()
        {
            foreach (int id in nodes.Take(15))
            {
                string ws = "";
                HybridDictionary hd = AllWeights[id];
                foreach (int w in hd.Keys)
                {
                    ws += w + "::" + AllWeights[id][w] + ",";
                }
                string vs = "";
                foreach (int v in AllReverseNeighbours[id])
                {
                    vs += v + ",";
                }
                Console.WriteLine("{0}  {1}  {2}", id, ws, vs);

            }
        }
        public Graph CreateSubGraph(List<int> ids)
        {
            Graph G2 = new Graph();

            foreach (int id in ids)
            {
                if (!nodes.Contains(id))
                { throw new Exception("Node not in original graph"); }
                else
                {
                    G2.AddNodeToGraph(id);
                }
            }

            foreach (int id in G2.nodes)
            {
                HybridDictionary hd = AllWeights[id];
                foreach(int otherId in hd.Keys) {
                    if (G2.nodes.Contains(otherId)) {
                        G2.AddEdge(id, otherId, (int) AllWeights[id][otherId]);
                    }
                }
            }
            return G2;
        }

    }

    class BFS
    {

        public List<int> T = new List<int>();
        public List<int> Core = new List<int>();
        public List<int> Ring;
        public List<int> SubGraph;
        Queue<int> queue = new Queue<int>();
        HashSet<int> visited = new HashSet<int>();
        Graph G;
        int MaxTree;
        int MaxCore;
        public BFS(Graph g, int U, double alpha, double f, int startNode)
        {
            G = g;
            MaxTree = (int)(U * alpha);
            MaxCore = (int)((double)U / f);
            run(startNode);
        }

        public BFS(Graph g, int startNode)
        {
            G = g;
        }

        public void run(int startNode)
        {
            visitNode(startNode);

            while (queue.Count > 0 && T.Count < MaxTree)
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
                queue.Enqueue(v);
                if (Core.Count < MaxCore)
                {
                    Core.Add(v);
                }
                T.Add(v);
                visited.Add(v);
            }
        }

        private int RandomVertex()
        {
            throw new NotImplementedException();
        }


    }
    public struct NodePoint
    {
        public int x;
        public int y;
        public int lati;
        public int longi;
    }

}
