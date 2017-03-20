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
        public static readonly string location = "F:\\Users\\Rogier\\Desktop\\Pictures\\";
        //public static readonly string location = "C:\\Users\\Roosje\\OneDrive\\Roos\\Downloads\\";
        public static void makePrints(Graph G, Graph OG, Dictionary<int, GeoPoint> nodes, string filename)
        {
            var realPS = Uncontract.GetPartitions(G, OG);
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

            PrintCutsOnGraph(nodes, OG, G, realEdges, filename);
        }


        public static Color heatColor(int low, int high, int current)
        {
            int max = 255;
            int colorInt = (int)(max * ((double)(current - low) / (high - low)));
            return Color.FromArgb(colorInt, 0, max - colorInt);
        }

        public static void PrintHeatMap(Graph OG, Dictionary<int, GeoPoint> nodes, Dictionary<int, int> neighbourCount)
        {
            int size = 20000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);
            int minCount = neighbourCount.Values.Min(x => x);
            int maxCount = neighbourCount.Values.Max(x => x);

            int maxLati = OG.nodes.Max(x => nodes[x].Y);
            int minLati = OG.nodes.Min(x => nodes[x].Y);
            int maxLongi = OG.nodes.Max(x => nodes[x].X);
            int minLongi = OG.nodes.Min(x => nodes[x].X);
            foreach (int id in OG.nodes)
            {
                GeoPoint gp = nodes[id];
                Color c = heatColor(minCount, maxCount, neighbourCount[id]);
                gr.DrawRectangle(new Pen(c, 20), calcX(gp, minLongi, maxLongi, size), calcY(gp, minLati, maxLati, size), 1, 1);
            }
            var path = location + "heatmap.bmp";
            bmp.Save(path);
        }

        public static void PrintGrid(Graph OG, Dictionary<int, GeoPoint> nodes, List<int> GridNode, int gridSize)
        {
            int size = 5000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);

            int maxLati = OG.nodes.Max(x => nodes[x].Y);
            int minLati = OG.nodes.Min(x => nodes[x].Y);
            int maxLongi = OG.nodes.Max(x => nodes[x].X);
            int minLongi = OG.nodes.Min(x => nodes[x].X);

            //for (int x = 0; x < gridSize; x++)
            //{
            //    int longitude = minLongi + (x * ((maxLongi - minLongi) / gridSize));
            //    GeoPoint np1 = new GeoPoint(longitude, 0);
            //    GeoPoint npYmin = new GeoPoint(0, minLati);
            //    GeoPoint npYmax = new GeoPoint(0, maxLati);
            //    int XV = calcX(np1, minLongi, maxLongi, size);
            //    Console.WriteLine(XV);
            //    gr.DrawLine(new Pen(Color.Yellow, 10), XV, calcY(npYmin, minLati, maxLati, size), XV, calcY(npYmax, minLati, maxLati, size));
            //}
            //for (int y = 0; y < gridSize; y++)
            //{
            //    int latitude = minLati + (y * ((maxLati - minLati) / gridSize));
            //    GeoPoint np1 = new GeoPoint(0, latitude);
            //    GeoPoint npYmin = new GeoPoint(minLongi, 0);
            //    GeoPoint npYmax = new GeoPoint(maxLongi, 0);
            //    int YV = calcY(np1, minLati, maxLati, size);
            //    Console.WriteLine(YV);
            //    gr.DrawLine(new Pen(Color.Yellow, 10), calcX(npYmin, minLongi, maxLongi, size), YV, calcX(npYmax, minLongi, maxLongi, size), YV);
            //}
            //Console.ReadLine();
            foreach (int id in OG.nodes)
            {
                GeoPoint gp = nodes[id];
                gr.DrawRectangle(new Pen(Color.Red, 1), calcX(gp, minLongi, maxLongi, size), calcY(gp, minLati, maxLati, size), 1, 1);
            }
            foreach (int id in GridNode)
            {
                GeoPoint gp = nodes[id];
                gr.DrawRectangle(new Pen(Color.Blue, 20), calcX(gp, minLongi, maxLongi, size), calcY(gp, minLati, maxLati, size), 5, 5);
            }
            var path = location + "gridMap.bmp";
            bmp.Save(path);
        }

        public static void PrintGridSquare(Graph OG, Dictionary<int, GeoPoint> nodes,List<int> InS)
        {
            int size = 5000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);

            int maxLati = OG.nodes.Max(x => nodes[x].Y);
            int minLati = OG.nodes.Min(x => nodes[x].Y);
            int maxLongi = OG.nodes.Max(x => nodes[x].X);
            int minLongi = OG.nodes.Min(x => nodes[x].X);

            for (int x = 0; x < 20; x++)
            {
                int longitude = minLongi + (x * ((maxLongi - minLongi) / 20));
                GeoPoint np1 = new GeoPoint(longitude, 0);
                GeoPoint npYmin = new GeoPoint(0, minLati);
                GeoPoint npYmax = new GeoPoint(0, maxLati);
                int XV = calcX(np1, minLongi, maxLongi, size);
                Console.WriteLine(XV);
                gr.DrawLine(new Pen(Color.Yellow, 10), XV, calcY(npYmin, minLati, maxLati, size), XV, calcY(npYmax, minLati, maxLati, size));
            }
            for (int y = 0; y < 20; y++)
            {
                int latitude = minLati + (y * ((maxLati - minLati) / 20));
                GeoPoint np1 = new GeoPoint(0, latitude);
                GeoPoint npYmin = new GeoPoint(minLongi, 0);
                GeoPoint npYmax = new GeoPoint(maxLongi, 0);
                int YV = calcY(np1, minLati, maxLati, size);
                Console.WriteLine(YV);
                gr.DrawLine(new Pen(Color.Yellow, 10), calcX(npYmin, minLongi, maxLongi, size), YV, calcX(npYmax, minLongi, maxLongi, size), YV);
            }
            foreach (int id in OG.nodes)
            {
                GeoPoint gp = nodes[id];
                gr.DrawRectangle(new Pen(Color.Red, 1), calcX(gp, minLongi, maxLongi, size), calcY(gp, minLati, maxLati, size), 1, 1);
            }
            foreach (int id in InS)
            {
                GeoPoint gp = nodes[id];
                gr.DrawRectangle(new Pen(Color.Blue, 20), calcX(gp, minLongi, maxLongi, size), calcY(gp, minLati, maxLati, size), 5, 5);
            }

            var path = location + "gridSMap.bmp";
            bmp.Save(path);
        }

        public static Color randomColor(Random r)
        {
            Color result;
            double rd = r.NextDouble();
            if (rd < 0.2) { result = Color.Red; }
            else if (rd < 0.4) { result = Color.Orange; }
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

        public static Brush randomBrush2(Random r)
        {
            Brush result = Brushes.Transparent;


            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = r.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);
            return result;
        }

        public static int calcX(GeoPoint p, int minLongi, int maxLongi, int size)
        {
            int mx = maxLongi - minLongi;
            int hx = p.X - minLongi;
            return (int)(((float)hx / mx) * size);
        }
        public static int calcY(GeoPoint p, int minLati, int maxLati, int size)
        {
            int my = maxLati - minLati;
            int hy = p.Y - minLati;
            return (int)(size - (((float)hy / my) * size));

        }

        public static void PrintCutsOnGraph(Dictionary<int, GeoPoint> nodes, Graph OG, Graph G, List<Edge> cuts, String filename)
        {
            int size = 5000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);
            int maxLati = OG.nodes.Max(x => nodes[x].Y);
            int minLati = OG.nodes.Min(x => nodes[x].Y);
            int maxLongi = OG.nodes.Max(x => nodes[x].X);
            int minLongi = OG.nodes.Min(x => nodes[x].X);
            Random r = new Random();
            Dictionary<int, Pen> pens = new Dictionary<int, Pen>();
            foreach (int id in G.nodes)
            {

                Pen pen = new Pen(randomColor(r));
                pens[id] = pen;
            }
            foreach (Edge e in OG.GetAllArcs())
            {
                int v = e.To;
                int w = e.From;
                int currentId = e.To;
                while (!pens.ContainsKey(currentId))
                {
                    currentId = G.Parent[currentId];
                }
                Pen pen = pens[currentId];
                GeoPoint np1 = nodes[v];
                GeoPoint np2 = nodes[w];
                gr.DrawLine(pen, calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));
            }
            foreach (Edge e in cuts)
            {
                int v = e.To;
                int w = e.From;
                GeoPoint np1 = nodes[v];
                GeoPoint np2 = nodes[w];
                gr.DrawEllipse(new Pen(Color.Blue, 15), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), 3, 3);
                gr.DrawEllipse(new Pen(Color.Blue, 15), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size), 3, 3);
                gr.DrawLine(new Pen(Color.Blue, 9), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));
            }
            RectangleF rectf = new RectangleF(size - 100 * (size / 1000), size - 100 * (size / 1000), size, size);
            gr.DrawString(cuts.Count.ToString(), new Font("Tahoma", 8 * (size / 1000)), Brushes.White, rectf);
            var path = location +
                filename + cuts.Count.ToString() + ".bmp";
            bmp.Save(path);

        }
        public static void PrintCutFound(Dictionary<int, GeoPoint> nodes, Graph G, List<int> subGraph, List<int> partition, HashSet<int> par, List<int> core, String filename)
        {
            int size = 2000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);

            int maxLati = G.nodes.Max(x => nodes[x].Y);
            int minLati = G.nodes.Min(x => nodes[x].Y);
            int maxLongi = G.nodes.Max(x => nodes[x].X);
            int minLongi = G.nodes.Min(x => nodes[x].X);
            foreach (Edge e in G.GetAllArcs())
            {
                if ((par.Contains(e.To) && !par.Contains(e.From)) || (!par.Contains(e.To) && par.Contains(e.From)))
                {
                    GeoPoint np1 = nodes[e.To];
                    GeoPoint np2 = nodes[e.From];
                    gr.DrawEllipse(new Pen(Color.Blue, 5), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), 3, 3);
                    gr.DrawEllipse(new Pen(Color.Blue, 5), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size), 3, 3);
                    gr.DrawLine(new Pen(Color.Blue, 3), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));

                }
            }

            subGraph.ForEach(n =>
            {
                GeoPoint np = nodes[n];
                gr.FillRectangle(Brushes.Red, calcX(np, minLongi, maxLongi, size), calcY(np, minLati, maxLati, size), 3, 3);
            });
            partition.ForEach(n =>
            {
                GeoPoint np = nodes[n];
                gr.FillRectangle(Brushes.Yellow, calcX(np, minLongi, maxLongi, size), calcY(np, minLati, maxLati, size), 3, 3);
            });
            core.ForEach(n =>
            {
                GeoPoint np = nodes[n];
                gr.FillRectangle(Brushes.Green, calcX(np, minLongi, maxLongi, size), calcY(np, minLati, maxLati, size), 3, 3);
            });
            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\" +
                filename + "Cut.png";
            bmp.Save(path);

        }

        public static void DrawChagedMap(Graph OG, Graph G, Dictionary<int, GeoPoint> nodes, string name)
        {
            int size = 20000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);
            int maxLati = G.nodes.Max(x => nodes[x].Y);
            int minLati = G.nodes.Min(x => nodes[x].Y);
            int maxLongi = G.nodes.Max(x => nodes[x].X);
            int minLongi = G.nodes.Min(x => nodes[x].X);
            Random r = new Random();
            foreach (Edge e in OG.GetAllArcs())
            {

                GeoPoint np1 = nodes[e.To];
                GeoPoint np2 = nodes[e.From];
                gr.DrawEllipse(new Pen(Color.Yellow, 5), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), 3, 3);
                gr.DrawEllipse(new Pen(Color.Yellow, 5), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size), 3, 3);
                gr.DrawLine(new Pen(Color.Yellow, 3), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));

            }
            foreach (Edge e in G.GetAllArcs())
            {

                GeoPoint np1 = nodes[e.To];
                GeoPoint np2 = nodes[e.From];
                gr.DrawEllipse(new Pen(Color.Blue, 5), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), 3, 3);
                gr.DrawEllipse(new Pen(Color.Blue, 5), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size), 3, 3);
                gr.DrawLine(new Pen(Color.Blue, 3), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));

            }

            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\FliterMap" + name + ".png";
            bmp.Save(path);
        }
        public static void DrawMap(Graph G, Dictionary<int, int> dict, Dictionary<int, GeoPoint> nodes, string name)
        {
            int size = 20000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);
            int maxLati = G.nodes.Max(x => nodes[x].Y);
            int minLati = G.nodes.Min(x => nodes[x].Y);
            int maxLongi = G.nodes.Max(x => nodes[x].X);
            int minLongi = G.nodes.Min(x => nodes[x].X);
            Random r = new Random();
            foreach (Edge e in G.GetAllArcs())
            {

                GeoPoint np1 = nodes[e.To];
                GeoPoint np2 = nodes[e.From];
                gr.DrawEllipse(new Pen(Color.Blue, 5), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), 3, 3);
                gr.DrawEllipse(new Pen(Color.Blue, 5), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size), 3, 3);

                gr.DrawLine(new Pen(Color.Blue, 3), calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), calcX(np2, minLongi, maxLongi, size), calcY(np2, minLati, maxLati, size));

            }
            foreach (var kvp in dict)
            {
                GeoPoint np1 = nodes[kvp.Key];
                RectangleF rectf = new RectangleF(calcX(np1, minLongi, maxLongi, size), calcY(np1, minLati, maxLati, size), 100, 100);
                gr.DrawString(kvp.Value.ToString(), new Font("Tahoma", 16), Brushes.White, rectf);
            }

            var path = "F:\\Users\\Rogier\\Desktop\\ROOS\\TheMap" + name + ".png";
            bmp.Save(path);
        }
    }
}

