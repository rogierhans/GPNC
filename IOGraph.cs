using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GPNC
{
    class IOGraph
    {
        public static readonly string location = "F:\\Users\\Rogier\\Desktop\\Graphs\\";

        public static bool DoesGraphsExists(string filename) {
            var pathNodes = location + filename + "nodes.csv";
            var pathArcs = location + filename + "arcs.csv";
            var pathParents = location + filename + "parents.csv";
            return File.Exists(pathNodes) && File.Exists(pathArcs) && File.Exists(pathParents);
        }
        public static void WriteGraph(Graph G, string filename)
        {
            List<string> AllLinesNodes = new List<string>();

            foreach (int id in G.nodes)
            {
                string[] line = new string[2];
                line[0] = id.ToString();
                line[1] = G.Size[id].ToString();
                AllLinesNodes.Add(String.Join(",", line));
            }
            var path = location + filename + "nodes.csv";
            File.WriteAllLines(path, AllLinesNodes.ToArray());

            List<string> AllLinesArcs = new List<string>();

            foreach (var kvp in G.AllWeights)
            {
                int from = kvp.Key;
                foreach (int to in kvp.Value.Keys)
                {
                    string[] line = new string[3];
                    line[0] = from.ToString();

                    line[1] = to.ToString();
                    line[2] = G.AllWeights[from][to].ToString();
                    AllLinesArcs.Add(String.Join(",", line));
                }
            }
            path = location + filename + "arcs.csv";
            File.WriteAllLines(path, AllLinesArcs.ToArray());


            List<string> AllParents = new List<string>();

            foreach (var kvp in G.Parent)
            {

                string[] line = new string[2];
                line[0] = kvp.Key.ToString();
                line[1] = kvp.Value.ToString();
                AllParents.Add(String.Join(",", line));

            }
            path = location + filename + "parents.csv";
            File.WriteAllLines(path, AllParents.ToArray());

        }

        internal static void MakeMap(string map, int countryNumber)
        {
            string Unfiltered = "UF";
            Graph G = Parser.ParseCSVFileWithNumber(countryNumber);
            IOGraph.WriteGraph(G, map + Unfiltered);
        }

        public static Graph ReadGraph(string filename) {
            var pathNodes = location + filename + "nodes.csv";
            var pathArcs = location + filename + "arcs.csv";
            var pathParents = location + filename + "parents.csv";
            Graph G = new Graph();
            Stream stream = File.Open(pathNodes, FileMode.Open);
            using (StreamReader sr = new StreamReader(stream))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(new char[] { ',' });
                    int fromnode = Int32.Parse(values[0]);
                    int size = Int32.Parse(values[1]);
                    int v = G.AddNodeToGraph(fromnode, size);
                }
            }
            stream = File.Open(pathArcs, FileMode.Open);
            using (StreamReader sr = new StreamReader(stream))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(new char[] { ',' });
                    int from = Int32.Parse(values[0]);
                    int to = Int32.Parse(values[1]);
                    int weight = Int32.Parse(values[2]);
                    G.AddEdge(from,to,weight);
                }
            }
            stream = File.Open(pathParents, FileMode.Open);
            using (StreamReader sr = new StreamReader(stream))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(new char[] { ',' });
                    int id = Int32.Parse(values[0]);
                    int parent= Int32.Parse(values[1]);
                    G.Parent[id] = parent;
                }
            }


            return G;

        }
        public static Graph GetFilteredGraph(string map)
        {
            string Filter2 = "F2";
            string Filter1 = "F1";
            string OriginalGraph = "OG";
            string Unfiltered = "UF";

            if (IOGraph.DoesGraphsExists(map + Filter2))
            {
                return IOGraph.ReadGraph(map + Filter2);
            }
            else if (IOGraph.DoesGraphsExists(map + Filter1))
            {
                Graph G = IOGraph.ReadGraph(map + Filter1);
                Filter.RemoveTwoDegree(G);
                IOGraph.WriteGraph(G, map + Filter2);
                return G;
            }
            else if (IOGraph.DoesGraphsExists(map + OriginalGraph))
            {
                Graph G = IOGraph.ReadGraph(map + OriginalGraph);
                Filter.RemoveOneDegree(G);
                IOGraph.WriteGraph(G, map + Filter1);
                Filter.RemoveTwoDegree(G);
                IOGraph.WriteGraph(G, map + Filter2);
                return G;
            }
            else if (IOGraph.DoesGraphsExists(map + Unfiltered))
            {
                Graph G = IOGraph.ReadGraph(map + Unfiltered);
                G = Filter.ConnectedComponent(G);
                IOGraph.WriteGraph(G, map + OriginalGraph);
                Filter.RemoveOneDegree(G);
                IOGraph.WriteGraph(G, map + Filter1);
                Filter.RemoveTwoDegree(G);
                IOGraph.WriteGraph(G, map + Filter2);
                return G;
            }
            else
            {

                Graph G = Parser.ParseCSVFile(map);
                IOGraph.WriteGraph(G, map + Unfiltered);
                G = Filter.ConnectedComponent(G);
                IOGraph.WriteGraph(G, map + OriginalGraph);
                Filter.RemoveOneDegree(G);
                IOGraph.WriteGraph(G, map + Filter1);
                Filter.RemoveTwoDegree(G);
                IOGraph.WriteGraph(G, map + Filter2);
                return G;
            }
        }
        public static Graph GetOriginalGraph(string map)
        {
            string OriginalGraph = "OG";
            return IOGraph.ReadGraph(map + OriginalGraph);
        }
    }
}
