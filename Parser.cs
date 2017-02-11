﻿using System;
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
                        G.AddFromEgde(v,tonode, length);
                        G.AddToEgde(w,fromnode);
                    }

                    //v.AddEgde(tonode, length);
                    //w.AddEgde(fromnode, length);
                }
            }
            return G;
        }
    }
}
