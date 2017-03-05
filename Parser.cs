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
        static bool nl = true;

        static readonly string location = "F:\\Users\\Rogier\\Desktop\\CQM\\";
        public static Graph ParseCSVFile(string map)
        {

            String path = location + map + "arc.csv";
            Graph G = new Graph();
            Stream stream = File.Open(path, FileMode.Open);
            using (StreamReader sr = new StreamReader(stream))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(new char[] { ',' });
                    int id = Int32.Parse(values[1]);
                    int fromnode = Int32.Parse(values[2]);
                    int tonode = Int32.Parse(values[3]);
                    if ( tonode != fromnode)
                    {
                        int v = G.AddNodeToGraph(fromnode, 1);
                        int w = G.AddNodeToGraph(tonode, 1);
                        G.AddEdge(v, w, 1);
                    }

                }
            }
            return G;
        }

        public static Graph ParseCSVFileAllowedCountries()
        {

            //String path = "C:\\Users\\Roosje\\OneDrive\\Roos\\Downloads\\arcNL.csv";
            String path;
            if (nl)
            {
                path = "F:\\Users\\Rogier\\Desktop\\CQM\\arcEurAz.csv";
            }
            else
            {
                path = "F:\\Users\\Rogier\\Desktop\\CQM\\arc.csv";
            }
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
                    int country = Int32.Parse(values[5]);
                    //Console.WriteLine(fromnode);
                    //remove cycles
                    HashSet<int>  allowedCountries = new HashSet<int>() { 151, 26, 6, 1, 115, 92, 86, 83, 164 };
                    if (allowedCountries.Contains(country) && tonode != fromnode)
                    {
                        int v = G.AddNodeToGraph(fromnode,1);
                        int w = G.AddNodeToGraph(tonode,1);
                        G.AddEdge(v, w, 1);
                    }

                    //v.AddEgde(tonode, length);
                    //w.AddEgde(fromnode, length);
                }
            }
            return G;
        }
        public static Tuple<Graph,Dictionary<int,int>> ParseCSVFileWithNumber()
        {

            //String path = "C:\\Users\\Roosje\\OneDrive\\Roos\\Downloads\\arcNL.csv";
            String path;
            Dictionary<int, int> dict = new Dictionary<int, int>();
            if (nl)
            {
                path = "F:\\Users\\Rogier\\Desktop\\CQM\\arcEurAz.csv";
            }
            else
            {
                path = "F:\\Users\\Rogier\\Desktop\\CQM\\arc.csv";
            }
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
                    int country = Int32.Parse(values[5]);
                    HashSet<int> allowedCountries = new HashSet<int>() { 151, 26, 6, 1, 28, 115, 92, 86, 83, 164 };
                    if (allowedCountries.Contains(country) && tonode != fromnode)
                    {
                        int v = G.AddNodeToGraph(fromnode, 1);
                        int w = G.AddNodeToGraph(tonode, 1);
                        if (!dict.Values.Contains(country))
                        {
                            dict[fromnode] = country;
                            dict[tonode] = country;
                        }
                        G.AddEdge(v, w, 1);
                    }

                    //v.AddEgde(tonode, length);
                    //w.AddEgde(fromnode, length);
                }
            }
            return new Tuple<Graph, Dictionary<int, int>>(G,dict);
        }

        public static Dictionary<int, NodePoint> ParseNodes(Graph G, string map)
        {
            Dictionary<int, NodePoint> dict = new Dictionary<int, NodePoint>();
            String path = location + map + "node.csv";
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
                    if (G.nodes.Contains(n.id))
                    {
                        n.lati = Int32.Parse(values[2]);
                        n.longi = Int32.Parse(values[3]);
                        nodes.Add(n);
                    }
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
                p.x = (int)(((float)hx / mx) * 4000);
                p.y = (int)(4000 - (((float)hy / my) * 4000));
                p.lati = n.lati;
                p.longi = n.longi;
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
