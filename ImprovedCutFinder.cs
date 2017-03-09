using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.Graph;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;
namespace GPNC
{
    class ImprovedCutFinder
    {
        Dictionary<int, GeoPoint> Nodes;
        Graph G;
        int U;
        double Alpha;
        double F;

        public ImprovedCutFinder(Dictionary<int, GeoPoint> nodes, Graph g, int u, double alpha, double f)
        {
            Nodes = nodes;
            G = g;
            U = u;
            Alpha = alpha;
            F = f;
        }

        Dictionary<int, Dictionary<int, int>> arcToIndex = new Dictionary<int, Dictionary<int, int>>();
        MaxFlow maxFlow;
        List<int> partition;
        List<Edge> cut;
        int index;
        HashSet<int> visited;
        Queue<int> queue;
        int size;
        int source = 6661337;
        List<int> sources;
        int sink = 1337666;
        HashSet<int> core;
        public Tuple<List<int>, List<Edge>> FindCut(int startNode)
        {
            partition = new List<int>();
            cut = new List<Edge>();
            arcToIndex = new Dictionary<int, Dictionary<int, int>>();
            maxFlow = new MaxFlow();
            index = 0;
            visited = new HashSet<int>();
            queue = new Queue<int>();
            size = 0;
            sources = new List<int>();
            core = new HashSet<int>();

            //phase 1 grow core until size U/f
            //CoreRing = edges that needs to be added extra edge 
            Visit(startNode);
            core.Add(startNode);
            GrowCore();
            AddCoreRing();

            //phase 2 grow and add to maxflow all neigbours allready in Subgraph unti size U * alpha
            GrowToRing();

            //phase 3 add ring
            AddRing();


            int solveStatus = maxFlow.Solve(source, sink);
            //Console.WriteLine(solveStatus);
            //Console.ReadLine();

            GetPartionAndCut();
            partition.AddRange(core);
            //trrint(startNode);
            return new Tuple<List<int>, List<Edge>>(partition, cut);
        }

        private void GetPartionAndCut()
        {
            Queue<int> OtherQueue = new Queue<int>();
            HashSet<int> OtherVisited = new HashSet<int>();

            foreach (int realSource in sources)
            {
                OtherQueue.Enqueue(realSource);
                OtherVisited.Add(realSource);
                partition.Add(realSource);
            }
            while (OtherQueue.Count > 0)
            {
                int currentNode = OtherQueue.Dequeue();
                var tuple = G.GetNeighboursFast(currentNode);

                foreach (int neighbour in tuple.Item1.Concat(tuple.Item2))
                {
                    if (!core.Contains(neighbour) && !OtherVisited.Contains(neighbour)&& !(neighbour == source))
                    {
                        //to test
                        //Console.WriteLine(neighbour);
                        //Console.WriteLine(visited.Contains(neighbour));
                        //bool neighbourInvisit = visited.Contains(neighbour);
                        //Console.WriteLine(currentNode);
                        //Console.WriteLine(visited.Contains(currentNode));
                        //Console.WriteLine(core.Contains(currentNode));
                        //Console.WriteLine(maxFlow.OptimalFlow());
                        //bool currentInvisit = visited.Contains(currentNode);


                        int i = arcToIndex[currentNode][neighbour];
                        long flow = maxFlow.Flow(i);
                        long capacity = maxFlow.Capacity(i);
                        if (capacity - flow > 0)
                        {
                            OtherQueue.Enqueue(neighbour);
                            OtherVisited.Add(neighbour);
                            partition.Add(neighbour);
                        }
                        else if (maxFlow.Flow(arcToIndex[neighbour][currentNode]) > 0)
                        {
                            OtherQueue.Enqueue(neighbour);
                            OtherVisited.Add(neighbour);
                            partition.Add(neighbour);
                        }
                        else
                        {
                            cut.Add(new Edge(neighbour, currentNode));
                        }
                    }
                }
            }
        }

        private void AddRing()
        {
            foreach (int ringElement in queue)
            {
                var tuple = G.GetNeighboursFast(ringElement);
                foreach (int neighbour in tuple.Item1.Concat(tuple.Item2))
                {
                    if (visited.Contains(neighbour) && completed.Contains(neighbour))
                    {
                        //if (connected.Contains(neighbour)) {
                        //    Console.WriteLine("cocoloc");
                        //    Console.ReadLine();
                        //}
                        AddArc(neighbour, sink, 100000);
                        //AddArc(sink, neighbour, 10000);
                        //Console.WriteLine("oki");
                    }
                }
            }
            //Console.ReadLine();
        }
        private void AddCoreRing()
        {
            foreach (int coreRingElement in queue)
            {
                var tuple = G.GetNeighboursFast(coreRingElement);

                foreach (int neighbour in tuple.Item1.Concat(tuple.Item2))
                {
                    //is dit correct? 
                    if (visited.Contains(neighbour))
                    {
                        sources.Add(neighbour);
                        //AddArc(neighbour,coreRingElement,G.getWeight(neighbour,coreRingElement));
                        AddArc(source, neighbour, 100000);
                        //AddArc(neighbour, source, 1000);
                        //Console.WriteLine("oki");
                    }
                }
            }
            // Console.ReadLine();
        }

        private void Visit(int node)
        {
            queue.Enqueue(node);
            visited.Add(node);
            size += G.Size[node];
        }
        HashSet<int> completed = new HashSet<int>();
        private void GrowCore()
        {
            double maxSizeCore = U / F;
            while (size < maxSizeCore)
            {
                int currentNode = queue.Dequeue();
                var tuple = G.GetNeighboursFast(currentNode);
                foreach (int neighbour in tuple.Item1.Concat(tuple.Item2))
                {
                    if (!visited.Contains(neighbour))
                    {
                        Visit(neighbour);

                        //is dit wel correct?
                        core.Add(neighbour);
                    }

                }
            }
        }
        HashSet<int> connected = new HashSet<int>();
        private void GrowToRing()
        {
            double maxSizeSubGraph = U * Alpha;
            while (size < maxSizeSubGraph)
            {
                int currentNode = queue.Dequeue();
                completed.Add(currentNode);
                var tuple = G.GetNeighboursFast(currentNode);
                foreach (int neighbour in tuple.Item1.Concat(tuple.Item2))
                {
                    if (!visited.Contains(neighbour))
                    {
                        Visit(neighbour);
                    }
                    else
                    {
                        connected.Add(neighbour);
                        AddArc(neighbour, currentNode, G.getWeight(neighbour, currentNode));
                        AddArc(currentNode, neighbour, G.getWeight(neighbour, currentNode));
                    }

                }
            }
            //Console.ReadLine();
        }

        //HashSet<ARC> arcs = new HashSet<ARC>();
        public void AddArc(int n, int fn, int weight)
        {
           // arcs.Add(new ARC(n, fn));
            maxFlow.AddArcWithCapacity(n, fn, weight);
            //Console.WriteLine(maxFlow.Solve(n, fn));
            //Console.WriteLine(maxFlow.OptimalFlow());
            //Console.WriteLine(weight);
            //Console.ReadLine();

            if (!arcToIndex.ContainsKey(n))
            {
                arcToIndex[n] = new Dictionary<int, int>();
            }
            arcToIndex[n][fn] = index++;
        }
        public struct ARC
        {
            public int From;
            public int To;
            public ARC(int from, int to)
            {
                From = from;
                To = to;
            }
        }

        //public void trrint(int startNode)
        //{

        //    int size = 20000;
        //    var bmp = new Bitmap(size, size);
        //    var gr = Graphics.FromImage(bmp);
        //    var nodes = new List<int>();
        //    foreach (ARC arc in arcs)
        //    {
        //        if (arc.To != sink && arc.From != sink && arc.To != source && arc.From != source)
        //        {
        //            nodes.Add(arc.To);
        //            nodes.Add(arc.From);
        //        }
        //    }

        //    int maxLati = nodes.Max(x => Nodes[x].lati);
        //    int minLati = nodes.Min(x => Nodes[x].lati);
        //    int maxLongi = nodes.Max(x => Nodes[x].longi);
        //    int minLongi = nodes.Min(x => Nodes[x].longi);

        //    NodePoint npSource = new NodePoint();
        //    npSource.lati = Nodes[startNode].lati;
        //    npSource.longi = Nodes[startNode].longi;
        //    Nodes[source] = npSource;

        //    NodePoint npSink = new NodePoint();
        //    npSink.lati = minLati - 10000;
        //    npSink.longi = minLongi - 10000;
        //    Nodes[sink] = npSink;

        //    nodes = new List<int>();
        //    foreach (ARC arc in arcs)
        //    {
        //        nodes.Add(arc.To);
        //        nodes.Add(arc.From);
        //    }

        //    maxLati = nodes.Max(x => Nodes[x].lati);
        //    minLati = nodes.Min(x => Nodes[x].lati);
        //    maxLongi = nodes.Max(x => Nodes[x].longi);
        //    minLongi = nodes.Min(x => Nodes[x].longi);

        //    foreach (ARC arc in arcs)
        //    {
        //        NodePoint np1 = Nodes[arc.From];
        //        NodePoint np2 = Nodes[arc.To];
        //        if (arc.To == sink || arc.From == sink) {
        //            gr.DrawRectangle(new Pen(Color.Green, 30), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), 10, 10);
        //            gr.DrawRectangle(new Pen(Color.Green, 30), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size), 10, 10);
        //        }
        //        else
        //            gr.DrawRectangle(new Pen(Color.Yellow, 30), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size), 10, 10);
        //        gr.DrawLine(new Pen(Color.Red, 5), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));
        //    }
        //    foreach (Edge e in cut)
        //    {
        //        NodePoint np1 = Nodes[e.From];
        //        NodePoint np2 = Nodes[e.To];

        //        gr.DrawLine(new Pen(Color.Blue, 15), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));
        //    }

        //    var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\killmeb.png";
        //    bmp.Save(path);


        //}

        //public static int calcX(NodePoint p, int minLongi, int maxLongi, int size)
        //{
        //    int mx = maxLongi - minLongi;
        //    int hx = p.longi - minLongi;
        //    return (int)(((float)hx / mx) * size);
        //}
        //public static int calcY(NodePoint p, int minLati, int maxLati, int size)
        //{
        //    int my = maxLati - minLati;
        //    int hy = p.lati - minLati;
        //    return (int)(size - (((float)hy / my) * size));

        //}
    }
}
