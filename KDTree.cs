using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public KDTree(List<GeoPoint> points, int depth)
        {
            evenDepth = isEven(depth);
            if (points.Count == 1)
            {
                Point = points[0];
            }
            else
            {
                points = points.OrderBy(x => isEven(depth) ? x.X : x.Y).ToList();
                int median = points.Count / 2;
                Point = points[median];
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
                if (Intersects(range)) { allPoints.Add(Point); }
            }
            else
            {
                if (left.IsFullyContained(range))
                {
                    allPoints.AddRange(left.ReportPoints());
                }
                else if (left.Intersects(range))
                {
                    allPoints.AddRange(left.GetRange(range));
                }
                if (right.IsFullyContained(range))
                {
                    allPoints.AddRange(right.ReportPoints());
                }
                else if (right.Intersects(range))
                {
                    allPoints.AddRange(right.GetRange(range));
                }
            }
            return allPoints;
        }

        //        . if ν is a leaf
        //2. then Report the point stored at ν if it lies in R
        //3. else if region(lc(ν)) is fully contained in R
        //4.      then ReportSubtree(lc(ν))
        //5.      else if region(lc(ν)) intersects R
        //6.      then SearchKdTree(lc(ν),R)
        //7. if region(rc(ν)) is fully contained in R
        //8. then ReportSubtree(rc(ν))
        //9. else if region(rc(ν)) intersects R
        //10. then SearchKdTree(rc(ν),R)

        public List<GeoPoint> ReportPoints()
        {
            if (isLeaf())
            {
                return new List<GeoPoint>() { Point };
            }
            else
            {
                List<GeoPoint> leftPoints = left.ReportPoints();
                if (right != null)
                {
                    List<GeoPoint> rightPoints = right.ReportPoints();
                    leftPoints.AddRange(rightPoints);
                }
                leftPoints.Add(Point);
                return leftPoints;
            }
        }

        public bool IsFullyContained(Range range)
        {
            return (range.Low.X <= min.X && range.Low.Y <= min.Y) && (range.High.X >= max.X && range.High.Y >= max.Y);
        }

        public bool Intersects(Range range)
        {
            return (min.X < range.High.X && max.X > range.Low.X && min.Y < range.High.Y && max.Y > range.Low.Y);
        }

        private bool inLeftChild(Range range)
        {
            if (evenDepth)
            {
                return range.Low.X <= Point.X && range.High.X <= Point.X;
            }
            else
            {
                return range.Low.Y <= Point.Y && range.High.Y <= Point.Y;
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
    }

    public struct GeoPoint
    {
        public int X;
        public int Y;

        public GeoPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
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
