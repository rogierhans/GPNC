using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;


namespace GPNC
{
    class Program
    {
        static void Main(string[] args)
        {

            Graph G = Parser.ParseCSVFile();
            Console.WriteLine(G.nodes.Count);
            G = Filter.ConnectedComponent(G);
            Graph OG = G.CreateSubGraph(G.nodes.ToList());
            Console.WriteLine(G.nodes.Count);
            int U = G.nodes.Count / 6;
            Console.WriteLine(U);
            Dictionary<int, NodePoint> nodes = Parser.ParseNodes(G);
            Filter.RemoveOneDegree(G);
            Console.WriteLine(G.nodes.Count);
            Filter.RemoveTwoDegree(G);
            Console.WriteLine(G.nodes.Count);

            HashSet<Edge> cuts = NaturalCut.MakeCuts(G, U);
            Console.WriteLine("lastTSTep");
            List<List<int>> ps = FindPartions(G, cuts, U);
            ps.ForEach(x => { int v = G.ContractList(x); });
            Console.WriteLine(G.nodes.Count);
            Greedy.initPar(G, U);
            G.print();
            Console.WriteLine(G.nodes.Count);
            print(G, nodes);

            //Console.ReadLine();




            //List<int> test = G.nodes.Skip(5).Take(100000).ToList();
            //Console.ReadLine();
            //G.ContractList(test);




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
            int weight = 0;
            G.GetAllArcs().ForEach(e => weight += G.getWeight(e.To, e.From));
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
            //Console.ReadLine();
            Console.WriteLine("letsgooooooo");
            List<Edge> realEdges = new List<Edge>();
            foreach (Edge e in G.GetAllArcs())
            {
                HashSet<int> par1 = realPS[e.To];
                HashSet<int> par2 = realPS[e.From];
                foreach (int pe in par1)
                {
                    foreach (int n in OG.GetNeighbours(pe))
                    {
                        if (par2.Contains(n))
                        {
                            realEdges.Add(new Edge(pe, n));
                        }
                    }
                }

            }

            foreach (Edge e in G.GetAllArcs())
            {
                HashSet<int> par1 = realPS[e.To];
                HashSet<int> par2 = realPS[e.From];
                smallPicturetest(OG, nodes, par1, par2, realEdges, e.To + "" + e.From);
            }
            Console.WriteLine("Weight:{0}", weight);
            testo(nodes, realEdges, "0000REALFUCK");
            Console.ReadLine();
            //nodes = Parser.ParseNodes(G);
            //test(nodes, G.nodes.ToList(), G.nodes.ToList(), G.nodes.ToList(), "00james");
            //printPS(nodes, ps, "000aaap");
        }
        public static void testo(Dictionary<int, NodePoint> nodes, List<Edge> cuts, String filename)
        {

            var bmp = new Bitmap(1000, 1000);
            var gr = Graphics.FromImage(bmp);


            foreach (NodePoint np in nodes.Values)
            {
                gr.FillRectangle(Brushes.Red, np.x, np.y, 1, 1);
            }

            foreach (Edge e in cuts)
            {
                int v = e.To;
                int w = e.From;
                NodePoint np1 = nodes[v];
                NodePoint np2 = nodes[w];
                gr.DrawLine(new Pen(Color.Blue, 5), np1.x, np1.y, np2.x, np2.y);

            }
            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\" +
                filename + "a.png";
            bmp.Save(path);


        }
        private static void print(Graph G, Dictionary<int, NodePoint> nodes)
        {
            var bmp = new Bitmap(1000, 1000);
            var gr = Graphics.FromImage(bmp);
            Random r = new Random();
            Dictionary<int, Brush> pens = new Dictionary<int, Brush>();
            foreach (int id in G.nodes)
            {

                Brush pen = randomBrush(r);
                pens[id] = pen;
            }
            foreach (var kvp in nodes)
            {
                NodePoint np = kvp.Value;
                int currentId = kvp.Key;
                while (!pens.ContainsKey(currentId))
                {
                    currentId = G.Parent[currentId];

                }
                Brush pen = pens[currentId];
                NodePoint cntnp = nodes[currentId];
                gr.FillRectangle(pen, np.x, np.y, 1, 1);
            }
            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\" +
            "RESULT" + "a.png";
            bmp.Save(path);
        }


        private static List<List<int>> FindPartions(Graph G, HashSet<Edge> cuts, int U)
        {
            List<List<int>> ps = new List<List<int>>();
            HashSet<int> allNodes = new HashSet<int>();
            G.nodes.ToList().ForEach(x => allNodes.Add(x));

            while (allNodes.Count > 0)
            {
                int rID = RandomID(allNodes);
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

        private static int RandomID(HashSet<int> allNodes)
        {
            Random rnd = new Random();
            return allNodes.ToList()[rnd.Next(allNodes.Count)];
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





        public static Color randomColor(Random r)
        {
            Color result;
            double rd = r.NextDouble();
            if (rd < 0.2) { result = Color.Red; }
            else if (rd < 0.4) { result = Color.Blue; }
            else if (rd < 0.6) { result = Color.Green; }
            else if (rd < 0.8) { result = Color.White; }
            else { result = Color.Yellow; }

            return result;
        }

        public static Brush randomBrush(Random r)
        {
            Brush result;
            double rd = r.NextDouble();
            if (rd < 0.2) { result = Brushes.Red; }
            else if (rd < 0.4) { result = Brushes.Blue; }
            else if (rd < 0.6) { result = Brushes.Green; }
            else if (rd < 0.8) { result = Brushes.White; }
            else { result = Brushes.Yellow; }

            return result;
        }

        public static void smallPicturetest(Graph OG,Dictionary<int, NodePoint> nodes, HashSet<int> par1, HashSet<int> par2, List<Edge> cuts, String filename)
        {
            var bmp = new Bitmap(1000, 1000);
            var gr = Graphics.FromImage(bmp);
            List<int> subGraph = par1.Union(par2).ToList();
            int maxLati = subGraph.Max(x => nodes[x].lati);
            int minLati = subGraph.Min(x => nodes[x].lati);
            int maxLongi = subGraph.Max(x => nodes[x].longi);
            int minLongi = subGraph.Min(x => nodes[x].longi);
            Graph subOG = OG.CreateSubGraph(subGraph);
            List<Edge> subEdges = subOG.GetAllArcs();
            subEdges.ForEach(e =>
            {
                int v = e.To;
                int w = e.From;
                NodePoint np1 = nodes[v];
                NodePoint np2 = nodes[w];
                gr.DrawLine(new Pen(Color.Yellow), calcX(np1, minLongi, maxLongi), calcY(np1, minLati, maxLati), calcX(np2, minLongi, maxLongi), calcY(np2, minLati, maxLati));
            });
            subOG = OG.CreateSubGraph(par1.ToList());
            subEdges = subOG.GetAllArcs();
            subEdges.ForEach(e =>
            {
                int v = e.To;
                int w = e.From;
                NodePoint np1 = nodes[v];
                NodePoint np2 = nodes[w];
                gr.DrawLine(new Pen(Color.Red), calcX(np1, minLongi, maxLongi), calcY(np1, minLati, maxLati), calcX(np2, minLongi, maxLongi), calcY(np2, minLati, maxLati));
            });
            subOG = OG.CreateSubGraph(par2.ToList());
            subEdges = subOG.GetAllArcs();
            subEdges.ForEach(e =>
            {
                int v = e.To;
                int w = e.From;
                NodePoint np1 = nodes[v];
                NodePoint np2 = nodes[w];
                gr.DrawLine(new Pen(Color.Green), calcX(np1, minLongi, maxLongi), calcY(np1, minLati, maxLati), calcX(np2, minLongi, maxLongi), calcY(np2, minLati, maxLati));
            });
            foreach (Edge e in cuts)
            {
                int v = e.To;
                int w = e.From;
                NodePoint np1 = nodes[v];
                NodePoint np2 = nodes[w];
                gr.DrawLine(new Pen(Color.Blue, 2), calcX(np1, minLongi, maxLongi), calcY(np1, minLati, maxLati), calcX(np2, minLongi, maxLongi), calcY(np2, minLati, maxLati));

            }
            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\01z" +
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


        private Dictionary<int, HashSet<int>> AllReverseNeighbours = new Dictionary<int, HashSet<int>>();
        private Dictionary<int, Dictionary<int,int>> AllWeights = new Dictionary<int, Dictionary<int, int>>();
        public Dictionary<int, int> Size = new Dictionary<int, int>();
        public Dictionary<int, int> Parent = new Dictionary<int, int>();


        public int AddNodeToGraph(int id)
        {
            if (!nodes.Contains(id))
            {
                nodes.Add(id);
                HashSet<int> reverseNeighbours = new HashSet<int>();
                Dictionary<int, int> weights = new Dictionary<int, int>();
                AllWeights[id] = weights;
                AllReverseNeighbours[id] = reverseNeighbours;
                Size[id] = 1;
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
            Size[v] += Size[w];
            Parent[w] = v;
            return v;

        }

        public int ContractList(List<int> contractionList)
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
            return first;
        }
        public List<int> GetNeighbours(int id)
        {
            return AllWeights[id].Keys.Concat(AllReverseNeighbours[id]).ToList();
        }

        public Tuple<Dictionary<int, int>.KeyCollection, HashSet<int>> GetNeighboursFast(int id)
        {
            var from = AllWeights[id].Keys;

            var to = AllReverseNeighbours[id];


            return new Tuple<Dictionary<int, int>.KeyCollection, HashSet<int>>(from, to);
        }

        public void AddEdge(int id, int otherId, int weight)
        {
            int v = id > otherId ? id : otherId;
            int w = id > otherId ? otherId : id;

            if (!AllWeights[v].ContainsKey(w))
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

        public bool IsEdge(int id, int otherId)
        {
            int v = id > otherId ? id : otherId;
            int w = id > otherId ? otherId : id;
            return AllWeights[v].ContainsKey(w);
        }

        public List<Edge> GetAllArcs()
        {
            List<Edge> arcs = new List<Edge>();
            foreach (var kvp in AllReverseNeighbours)
            {
                foreach (int id in kvp.Value)
                {
                    arcs.Add(new Edge(id, kvp.Key));
                }
            }
            return arcs;

        }
        public List<Edge> GetAllArcs(int U)
        {
            List<Edge> arcs = new List<Edge>();
            foreach (var kvp in AllReverseNeighbours)
            {
                foreach (int id in kvp.Value)
                {
                    int size = Size[kvp.Key] + Size[id];
                    if (size <= U)
                        arcs.Add(new Edge(id, kvp.Key));
                }
            }
            return arcs;

        }
        public int GetDegree(int id)
        {
            return GetNeighbours(id).Count;
        }
        public void print()
        {
            foreach (int id in nodes)
            {
                string s = id + "(" + Size[id] + ")" + ": ";
                foreach (int n in GetNeighbours(id))
                {
                    s += n + "(" + Size[n] + ")";


                }
                Console.WriteLine(s);
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
                Dictionary<int, int> hd = AllWeights[id];
                foreach (int otherId in hd.Keys)
                {
                    if (G2.nodes.Contains(otherId))
                    {
                        G2.AddEdge(id, otherId, (int)AllWeights[id][otherId]);
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
        int Size;
        public BFS(Graph g, int U, double alpha, double f, int startNode)
        {
            G = g;
            MaxTree = (int)(U * alpha);
            MaxCore = (int)((double)U / f);
            Size = 0;
            run(startNode);
        }

        public BFS(Graph g, int startNode)
        {
            G = g;
        }

        public void run(int startNode)
        {
            visitNode(startNode);

            while (queue.Count > 0 && Size < MaxTree)
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
                Size += G.Size[v];
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

    public struct Edge
    {
        public int From;
        public int To;

        public Edge(int from, int to)
        {
            this.From = from < to ? from : to;
            this.To = from < to ? to : from;

        }
        public override string ToString()
        {
            return ("(" + From.ToString() + "," + To.ToString() + ")");
        }
        public override int GetHashCode()
        {
            return ((23 * 31) + From * 31) + To;
        }


    }
    //class SubGraph {
    //    Graph G;
    //    public SubGraph(Graph g) {
    //        G = 
    //    }

    //}



}
