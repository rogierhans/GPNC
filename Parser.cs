using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GPNC
{
    static class Parser
    {
        public static Graph ParseCSVFile()
        {

            //String path = "C:\\Users\\Roosje\\OneDrive\\Roos\\Downloads\\arcNL.csv";

            String path = "F:\\Users\\Rogier\\Desktop\\CQM\\arcNL.csv";
            Graph G = new Graph();
            Stream stream = File.Open(path, FileMode.Open);
            using (StreamReader sr = new StreamReader(stream))
            {
                String line;

                //bye first line
                sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(new char[] { ',' });
                    int id = Int32.Parse(values[1]);
                    int fromnode = Int32.Parse(values[2]);
                    int tonode = Int32.Parse(values[3]);
                    int length = Int32.Parse(values[4]);
                    //Console.WriteLine(fromnode);
                    //remove cycles
                    if (tonode != fromnode)
                    {
                        int v = G.AddNodeToGraph(fromnode);
                        int w = G.AddNodeToGraph(tonode);
                        G.AddFromEgde(v, tonode, length);
                        G.AddToEgde(w, fromnode);
                    }

                    //v.AddEgde(tonode, length);
                    //w.AddEgde(fromnode, length);
                }
            }
            return G;
        }
        public static Dictionary<int, NodePoint> ParseNodes()
        {
            Dictionary<int, NodePoint> dict = new Dictionary<int, NodePoint>();
            String path = "F:\\Users\\Rogier\\Desktop\\CQM\\nodeNL.csv";

            List<Node> nodes = new List<Node>();
            Stream stream = File.Open(path, FileMode.Open);
            using (StreamReader sr = new StreamReader(stream))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(new char[] { ',' });
                    Node n = new Node();

                    n.id = Int32.Parse(values[1]);
                    n.lati = Int32.Parse(values[2]);
                    n.longi = Int32.Parse(values[3]);
                    nodes.Add(n);
                }
            }
            int maxLati = nodes.Max(x => x.lati);
            int minLati = nodes.Min(x => x.lati);
            int maxLongi = nodes.Max(x => x.longi);
            int minLongi = nodes.Min(x => x.longi);
            int mx = maxLongi - minLongi;
            int my = maxLati - minLati;
            int hx;
            int hy;
            nodes.ForEach(n =>
            {
                hx = n.longi - minLongi;
                hy = n.lati - minLati;
                NodePoint p = new NodePoint();
                p.x = (int)(((float)hx / mx) * 1000);
                p.y = (int)(1000 - (((float)hy / my) * 1000));
                dict[n.id] = p;
            });
            Console.WriteLine("done");
            return dict;
        }


        public struct Node
        {
            public int id;
            public int lati;
            public int longi;
        }

    }
}
