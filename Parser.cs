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

        internal static Graph ParseCSVFileWithNumber(int countryNumber)
        {
            String path = location + "EA" + "arc.csv";
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
                    int country = Int32.Parse(values[5]);
                    if (country == countryNumber &&tonode != fromnode)
                    {
                        int v = G.AddNodeToGraph(fromnode, 1);
                        int w = G.AddNodeToGraph(tonode, 1);
                        G.AddEdge(v, w, 1);
                    }

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

        public static Dictionary<int, GeoPoint> ParseNodes(Graph G, string map)
        {
            Dictionary<int, GeoPoint> dict = new Dictionary<int, GeoPoint>();
            String path = location + map + "node.csv";
            Stream stream = File.Open(path, FileMode.Open);
            using (StreamReader sr = new StreamReader(stream))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(new char[] { ',' });

                    var id = Int32.Parse(values[1]);
                    if (G.nodes.Contains(id))
                    {
                        var lati = Int32.Parse(values[2]);
                        var longi = Int32.Parse(values[3]);
                        dict[id] = new GeoPoint(longi,lati,id);
                    }
                }
            }
            Console.WriteLine("done");
            return dict;
        }
    }
}
