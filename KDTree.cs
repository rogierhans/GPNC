﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;

namespace GPNC
{
    public class KDTree
    {
        private KDTree left;
        private KDTree right;
        private GeoPoint Point;
        private bool evenDepth;

        private GeoPoint min;
        private GeoPoint max;

        private int TreeCount;
        public KDTree(List<GeoPoint> points) : this(points, 0) { }

        public KDTree(List<GeoPoint> points, int depth)
        {
            TreeCount = points.Count;
            evenDepth = isEven(depth);
            if (points.Count == 1)
            {
                Point = points[0];
            }
            else
            {
                //sort on x or y coordinate
                points = points.OrderBy(x => isEven(depth) ? x.X : x.Y).ToList();

                //devide on median
                int median = points.Count / 2;
                Point = points[median];

                //
                List<GeoPoint> leftPoints = points.Take(median).ToList();
                List<GeoPoint> rightPoints = points.Skip(median).ToList();
                if (leftPoints.Count > 0) left = new KDTree(leftPoints, depth + 1);
                if (rightPoints.Count > 0) right = new KDTree(rightPoints, depth + 1);
                min = new GeoPoint(points.Min(gp => gp.X), points.Min(gp => gp.Y));
                max = new GeoPoint(points.Max(gp => gp.X), points.Max(gp => gp.Y));
            }
        }

        internal string ToString(int v)
        {
            if (isLeaf())
            {
                return new String(' ', v) + "|" + Point.ToString();
            }
            else
            {
                return new String(' ', v) + "|" + "*" + Point.ToString() + "*" + "\n" + left.ToString(v + 1) + "\n" + right.ToString(v + 1);
            }
        }

        public override string ToString()
        {
            if (isLeaf())
            {
                return $"{{ { Point} }}";
            }
            else
            {
                return $"{{{left}|||{right}}}";
            }
        }

        public List<GeoPoint> GetRange(Range range)
        {
            var allPoints = new List<GeoPoint>();
            if (isLeaf())
            {
                //mischien not correct ff testen
                Console.WriteLine("leaf");
                if (IsFullyContained(range)) { allPoints.Add(Point); }
            }
            else
            {
                if (left.IsFullyContained(range))
                {
                    //Console.WriteLine("leftBranchF");
                    allPoints.AddRange(left.ReportPoints());
                }
                else if (left.Intersects(range))
                {

                    //Console.WriteLine("leftInter");
                    allPoints.AddRange(left.GetRange(range));
                }
                if (right.IsFullyContained(range))
                {
                    //Console.WriteLine("yolo");
                    allPoints.AddRange(right.ReportPoints());
                }
                else if (right.Intersects(range))
                {

                    //Console.WriteLine("rightInter");
                    allPoints.AddRange(right.GetRange(range));
                }
            }
            return allPoints;
        }
        public double CountUnits(GeoPoint gp, int gridsize)
        {
            int XUnit = (max.X - min.X) / gridsize;
            int YUnit = (max.Y - min.Y) / gridsize;
            GeoPoint startPoint = new GeoPoint(gp.X - XUnit, gp.Y - YUnit);
            Range r = new Range(startPoint, XUnit * 2, YUnit * 2);
            return Count(r);
        }

        public List<int> GetNodesOnGrid(int gridsize)
        {
            Random rng = new Random();
            int XUnit = (max.X - min.X) / gridsize;
            int YUnit = (max.Y - min.Y) / gridsize;
            List<int> NodesOnGrid = new List<int>();
            double xDisturbance = rng.NextDouble();
            double yDisturbance = rng.NextDouble();
            for (int x = 0; x < gridsize; x++)
            {
                for (int y = 0; y < gridsize; y++)
                {
                    int longitude = min.X + (int)((xDisturbance + x) * XUnit);
                    int latitude = min.Y + (int)((yDisturbance + y) * YUnit);
                    GeoPoint startPoint = new GeoPoint(longitude - XUnit, latitude - YUnit);
                    Range r = new Range(startPoint, XUnit * 2, YUnit * 2);
                    var nodesInRange = GetRange(r);
                    if (nodesInRange.Count > 0)
                    {
                        var ClosestGP = nodesInRange.OrderBy(gp =>
                            Math.Pow((longitude - gp.X), 2) + Math.Pow((latitude - gp.Y), 2)
                        ).First();
                        NodesOnGrid.Add(ClosestGP.Id);
                    }
                    Console.WriteLine(gridsize * x + y);
                }
            }

            return NodesOnGrid;
        }



        public Dictionary<int, double> GetScores(int gridSize)
        {
            Dictionary<int, double> scores = new Dictionary<int, double>();
            foreach (GeoPoint gp in ReportPoints())
            {
                double score = CountUnits(gp, gridSize);
                scores[gp.Id] = score;
            }
            return scores;
        }

        public List<int> GetOrder(Dictionary<int, double> scores)
        {
            Random rng = new Random();
            double disturbance;
            Dictionary<int, double> newScores = new Dictionary<int, double>();
            foreach (var kvp in scores)
            {
                disturbance = rng.NextDouble() * 0.01 + 0.995;
                newScores[kvp.Key] = scores[kvp.Key] * disturbance;
            }
            var orded = from pair in newScores
                        orderby pair.Value descending
                        select pair.Key;
            return orded.ToList();
        }


        public int Count(Range range)
        {
            var count = 0;
            if (isLeaf())
            {
                if (IsFullyContained(range)) { count += 1; }
            }
            else
            {
                if (left.IsFullyContained(range))
                {
                    count += left.TreeCount;
                }
                else if (left.Intersects(range))
                {

                    count += (left.Count(range));
                }
                if (right.IsFullyContained(range))
                {
                    //Console.WriteLine("yolo");
                    count += right.TreeCount;
                }
                else if (right.Intersects(range))
                {

                    //Console.WriteLine("rightInter");
                    count += (right.Count(range));
                }
            }
            return count;
        }


        public List<GeoPoint> ReportPoints()
        {
            if (isLeaf())
            {
                return new List<GeoPoint>() { Point };
            }
            else
            {
                List<GeoPoint> leftPoints = left.ReportPoints();
                leftPoints.AddRange(right.ReportPoints());
                return leftPoints;
            }
        }

        public bool IsFullyContained(Range range)
        {
            if (isLeaf())
            {
                return (range.Low.X <= Point.X && range.Low.Y <= Point.Y) && (range.High.X >= Point.X && range.High.Y >= Point.Y);
            }
            else
            {
                return (range.Low.X <= min.X && range.Low.Y <= min.Y) && (range.High.X >= max.X && range.High.Y >= max.Y);
            }
        }

        public bool Intersects(Range range)
        {
            if (isLeaf())
            {
                return (Point.X <= range.High.X && Point.X >= range.Low.X && Point.Y <= range.High.Y && Point.Y >= range.Low.Y);

            }
            else
            {
                return (min.X <= range.High.X && max.X >= range.Low.X && min.Y <= range.High.Y && max.Y >= range.Low.Y);
            }
        }


        private bool isLeaf()
        {
            return left == null && right == null;
        }

        private bool isEven(int depth)
        {
            return depth % 2 == 0;
        }


        public List<Range> GetAllRange()
        {
            Range ownRange;
            if (isLeaf())
            {
                return new List<Range>();
            }
            else
            {

                ownRange = new Range();
                ownRange.Low = min;
                ownRange.High = max;
                var leftList = left.GetAllRange();
                var rightList = right.GetAllRange();
                leftList.AddRange(rightList);
                leftList.Add(ownRange);
                return leftList;
            }
        }
        public void makePicture(Range realRange)
        {
            var location = "F:\\Users\\Rogier\\Desktop\\Pictures\\";

            int size = 20000;
            var bmp = new Bitmap(size, size);
            var gr = Graphics.FromImage(bmp);
            var ranges = GetAllRange();
            int minX = ranges.Min(r => r.Low.X);
            int maxX = ranges.Max(r => r.High.X);
            int minY = ranges.Min(r => r.Low.Y);
            int maxY = ranges.Max(r => r.High.Y);
            foreach (Range r in ranges)
            {
                GeoPoint gp1 = r.Low;
                GeoPoint gp2 = r.High;
                int oneX = (int)(gp1.relativeX(minX, maxX) * size);
                int oneY = (int)(gp1.relativeY(minY, maxY) * size);

                int twoX = (int)(gp2.relativeX(minX, maxX) * size);
                int twoY = (int)(gp2.relativeY(minY, maxY) * size);
                gr.DrawRectangle(new Pen(Color.Red), oneX, oneY, twoX - oneX, twoY - oneY);
                Console.WriteLine($"(({oneX},{oneY}),({twoX},{twoY}))");
            }
            foreach (GeoPoint gp in ReportPoints())
            {
                int oneX = (int)(gp.relativeX(minX, maxX) * size);
                int oneY = (int)(gp.relativeY(minY, maxY) * size);
                gr.DrawRectangle(new Pen(Color.Blue, 5), oneX, oneY, 1, 1);
            }
            {
                GeoPoint gp1 = realRange.Low;
                GeoPoint gp2 = realRange.High;
                int oneX = (int)(gp1.relativeX(minX, maxX) * size);
                int oneY = (int)(gp1.relativeY(minY, maxY) * size);

                int twoX = (int)(gp2.relativeX(minX, maxX) * size);
                int twoY = (int)(gp2.relativeY(minY, maxY) * size);
                gr.DrawRectangle(new Pen(Color.Brown, 100), oneX, oneY, twoX - oneX, twoY - oneY);

            }
            var path = location + "KDTREE.bmp";
            bmp.Save(path);
        }
    }

    public struct GeoPoint
    {
        public int X;
        public int Y;
        public int Id;
        public GeoPoint(int x, int y)
        {
            X = x;
            Y = y;
            Id = -1;
        }

        public GeoPoint(int x, int y, int id)
        {
            X = x;
            Y = y;
            Id = id;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public double relativeX(int minX, int maxX)
        {
            return (double)(X - minX) / (maxX - minX);
        }
        public double relativeY(int minY, int maxY)
        {
            return (double)(Y - minY) / (maxY - minY);
        }
    }

    public struct Range
    {
        public GeoPoint Low;
        public GeoPoint High;
        public Range(GeoPoint start, int width, int height)
        {
            Low = start;
            High = new GeoPoint(start.X + width, start.Y + height);
        }
        public override string ToString()
        {
            return $"[{Low} to {High}]";
        }

    }
}
