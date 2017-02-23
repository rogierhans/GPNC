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
    static class Print
    {
        public static void makePrints(Graph G, Graph OG, Dictionary<int,NodePoint> nodes,Dictionary<int,int> Parents) {
            var realPS = Uncontract.GetPartitions(G, OG);
            print(G, nodes,Parents);
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
                PrintCutOfTwoFragments(OG, nodes, par1, par2, realEdges, "1"+ e.To + "" + e.From);
            }
            PrintCutsOnGraph(nodes,OG, G.GetAllArcs(), realEdges, "0GraphWithCuts");

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

        public static Brush randomBrush2(Random r) {
            Brush result = Brushes.Transparent;


            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = r.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);
            return result;
        }

        public static void PrintCutOfTwoFragments(Graph OG, Dictionary<int, NodePoint> nodes, HashSet<int> par1, HashSet<int> par2, List<Edge> cuts, String filename)
        {
            int size = 4000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);
            List<int> subGraph = par1.Union(par2).ToList();
            int maxLati = subGraph.Max(x => nodes[x].lati);
            int minLati = subGraph.Min(x => nodes[x].lati);
            int maxLongi = subGraph.Max(x => nodes[x].longi);
            int minLongi = subGraph.Min(x => nodes[x].longi);
            Graph subOG = OG.CreateSubGraphWithoutParent(subGraph);
            List<Edge> subEdges = subOG.GetAllArcs();
            subEdges.ForEach(e =>
            {
                int v = e.To;
                int w = e.From;
                NodePoint np1 = nodes[v];
                NodePoint np2 = nodes[w];
                gr.DrawLine(new Pen(Color.Yellow), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));
            });
            subOG = OG.CreateSubGraphWithoutParent(par1.ToList());
            subEdges = subOG.GetAllArcs();
            subEdges.ForEach(e =>
            {
                int v = e.To;
                int w = e.From;
                NodePoint np1 = nodes[v];
                NodePoint np2 = nodes[w];
                gr.DrawLine(new Pen(Color.Red), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));
            });
            subOG = OG.CreateSubGraphWithoutParent(par2.ToList());
            subEdges = subOG.GetAllArcs();
            subEdges.ForEach(e =>
            {
                int v = e.To;
                int w = e.From;
                NodePoint np1 = nodes[v];
                NodePoint np2 = nodes[w];
                gr.DrawLine(new Pen(Color.Green), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));
            });
            int cutsCount = 0;
            foreach (Edge e in cuts)
            {
                if ((par2.Contains(e.To) || par2.Contains(e.From)) && (par1.Contains(e.To) || par1.Contains(e.From)))
                {
                    cutsCount++;
                    int v = e.To;
                    int w = e.From;
                    NodePoint np1 = nodes[v];
                    NodePoint np2 = nodes[w];
                    gr.FillRectangle(Brushes.Blue, calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), 1, 1);
                    gr.FillRectangle(Brushes.Blue, calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size), 1, 1);
                    gr.DrawLine(new Pen(Color.Blue, 2), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));
                }
            }
            RectangleF rectf = new RectangleF(size - 100, size-100, size, size);
            gr.DrawString(cutsCount.ToString(), new Font("Tahoma", 8), Brushes.White, rectf);
            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\" +
    filename + ".png";
            bmp.Save(path);
        }
        public static int calcX(NodePoint p, int minLongi, int maxLongi,int size)
        {
            int mx = maxLongi - minLongi;
            int hx = p.longi - minLongi;
            return (int)(((float)hx / mx) * size);
        }
        public static int calcY(NodePoint p, int minLati, int maxLati, int size)
        {
            int my = maxLati - minLati;
            int hy = p.lati - minLati;
            return (int)(size - (((float)hy / my) * size));

        }

        public static void print(Graph G, Dictionary<int, NodePoint> nodes, Dictionary<int,int> Parent)
        {
            int size = 4000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);
            Random r = new Random();
            Dictionary<int, Brush> pens = new Dictionary<int, Brush>();
            foreach (int id in G.nodes)
            {

                Brush pen = randomBrush2(r);
                pens[id] = pen;
            }
            foreach (var kvp in nodes)
            {
                NodePoint np = kvp.Value;
                int currentId = kvp.Key;
                while (!pens.ContainsKey(currentId))
                {
                    currentId = Parent[currentId];

                }
                Brush pen = pens[currentId];
                NodePoint cntnp = nodes[currentId];
                gr.FillRectangle(pen, np.x, np.y, 1, 1);
            }
            RectangleF rectf = new RectangleF(size - 100, size-100, size, size);
            int count = 0;
            G.GetAllArcs().ForEach(x => count += G.getWeight(x.To, x.From));
            gr.DrawString(count.ToString(), new Font("Tahoma", 8), Brushes.White, rectf);
            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\" +
            "0RESULT" + ".png";
            bmp.Save(path);
        }
        public static void PrintCutsOnGraph(Dictionary<int, NodePoint> nodes, Graph OG, List<Edge> allCuts, List<Edge> cuts, String filename)
        {
            int size = 4000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);
            int maxLati = OG.nodes.Max(x => nodes[x].lati);
            int minLati = OG.nodes.Min(x => nodes[x].lati);
            int maxLongi = OG.nodes.Max(x => nodes[x].longi);
            int minLongi = OG.nodes.Min(x => nodes[x].longi);

            //foreach (NodePoint np in nodes.Values)
            //{
            //    gr.FillRectangle(Brushes.Red, calcX(np, minLongi, maxLongi, size), calcY(np, minLati, maxLati, size), 1, 1);
            //}
            foreach (Edge e in allCuts)
            {
                int v = e.To;
                int w = e.From;
                NodePoint np1 = nodes[v];
                NodePoint np2 = nodes[w];
                gr.DrawLine(new Pen(Color.Red), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));
            }
            foreach (Edge e in cuts)
            {
                int v = e.To;
                int w = e.From;
                NodePoint np1 = nodes[v];
                NodePoint np2 = nodes[w];
                gr.DrawEllipse(new Pen(Color.Blue, 5), np1.x, np1.y, 3, 3);
                gr.DrawEllipse(new Pen(Color.Blue, 5), np2.x, np2.y, 3, 3);
                gr.DrawLine(new Pen(Color.Blue,3), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));
            }
            RectangleF rectf = new RectangleF(size-100, size - 100, size, size);
            gr.DrawString(cuts.Count.ToString(), new Font("Tahoma", 8), Brushes.White, rectf);
            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\" +
                filename + ".png";
            bmp.Save(path);

        }
        public static void PrintCutFound(Dictionary<int, NodePoint> nodes, List<int> subGraph, List<int> partition, List<int> core, String filename)
        {

            var bmp = new Bitmap(4000, 4000);
            var gr = Graphics.FromImage(bmp);


            foreach (NodePoint np in nodes.Values)
            {
                gr.FillRectangle(Brushes.Red, np.x, np.y, 1, 1);
            }

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
                filename + "Cut.png";
            bmp.Save(path);

        }
    }
}

