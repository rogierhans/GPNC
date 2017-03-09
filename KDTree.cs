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
                List<GeoPoint> rightPoints = points.Skip(median + 1).ToList();
                if (leftPoints.Count > 0) left = new KDTree(leftPoints, depth + 1);
                if (rightPoints.Count > 0) right = new KDTree(rightPoints, depth + 1);
                min = new GeoPoint(points.Min(gp => gp.X), points.Min(gp => gp.Y));
                max = new GeoPoint(points.Max(gp => gp.X), points.Max(gp => gp.Y));
            }
        }

        public List<GeoPoint> GetRange(GeoPoint low, GeoPoint high)
        {
            var allPoints = new List<GeoPoint>();
            if (!isLeaf())

                allPoints.Add(Point);

            {
                if (left.IsFullyContained(low, high))
                {
                    allPoints.AddRange(right.ReportPoints());
                }
                else if (left.Intersects(low, high))
                {
                    allPoints.AddRange(left.GetRange(low, high));
                }
                if (right != null)
                {
                    if (right.IsFullyContained(low, high))
                    {
                        allPoints.AddRange(right.ReportPoints());
                    }
                    else if (right.Intersects(low, high))
                    {
                        allPoints.AddRange(right.GetRange(low, high));

                    }
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

        public bool IsFullyContained(GeoPoint low, GeoPoint high)
        {
            return (low.X <= min.X && low.Y <= min.Y) && (high.X >= max.X && high.Y >= max.Y);
        }

        private bool Intersects(GeoPoint low, GeoPoint high)
        {
            return (low.X <= min.X && low.Y <= min.Y) && (high.X >= max.X && high.Y >= max.Y);
        }

        private bool inLeftChild(GeoPoint low, GeoPoint high)
        {
            if (evenDepth)
            {
                return low.X <= Point.X && high.X <= Point.X;
            }
            else
            {
                return low.Y <= Point.Y && high.Y <= Point.Y;
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
    }
}
