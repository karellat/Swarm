using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Intersection2D
{
    public static class Intersections
    {
        // Intersection methods
        /// <summary>
        /// Return true if a, b circles intersects
        /// </summary>
        /// <param name="aMiddle"></param>
        /// <param name="aRadius"></param>
        /// <param name="bMiddle"></param>
        /// <param name="bRadius"></param>
        /// <returns></returns>
        public static bool CircleCircleIntersection(Vector2 aMiddle, float aRadius, Vector2 bMiddle, float bRadius)
        {
            float squaredDistanceBetweenAB = (bMiddle - aMiddle).LengthSquared();

            if (squaredDistanceBetweenAB <= ((aRadius + bRadius) * (aRadius + bRadius)))
                return true;
            return false;
        }
        /// <summary>
        /// Return intersection point of Line a and b
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns> Return NaN vector if no intersection, Infity if one line is part of the second </returns>
        public static Vector2 LineLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            //Transform points to equation form 
            //line a 
            float A1 = a1.Y - a2.Y;
            float B1 = a2.X - a1.X;
            float C1 = A1 * a2.X + B1 * a2.Y;

            //line b
            float A2 = b1.Y - b2.Y;
            float B2 = b2.X - b1.X;
            float C2 = A2 * b2.X + B2 * b2.Y;

            //assuming Ax + By = C form 
            float delta = A1 * B2 - A2 * B1;
            //Check parallelity of lines 
            if (delta == 0)
            {
                //Check equality of lines 
                if (A2 * b1.X + B2 * b1.Y == -C2)
                    return new Vector2(float.PositiveInfinity, float.PositiveInfinity);

                return new Vector2(float.NaN, float.NaN);
            }
            //If not parellel return point of intersection
            return new Vector2((B2 * C1 - B1 * C2) / delta, (A1 * C2 - A2 * C1) / delta);


        }
        /// <summary>
        /// Return intersection point of two line segment a & b 
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static Vector2 LinesegmentLinesegmentIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            //If not line throw exception
            //Debug.Assert(a1 - a2 == Vector2.Zero);
            //Debug.Assert(b1 - b2 == Vector2.Zero);
            //Find line & line intersection 
            Vector2 i = LineLineIntersection(a1, a2, b1, b2);
            //No intersection 
            if (float.IsNaN(i.X))
                return i;

            if (!float.IsPositiveInfinity(i.X))
            {
                //Check line a contains intersection 
                //if  perpendicular with axis 
                if (a1.X != a2.X)
                {
                    if (!MyExtensions.Between(a1.X, a2.X, i.X))
                        return new Vector2(float.NaN, float.NaN);
                }
                else
                {
                    if (!MyExtensions.Between(a1.Y, a2.Y, i.Y))
                        return new Vector2(float.NaN, float.NaN);
                }

                //Check line b contains intersection 
                if (b1.X != b2.X)
                {
                    if (!MyExtensions.Between(b1.X, b2.X, i.X))
                        return new Vector2(float.NaN, float.NaN);
                }
                else
                {
                    if (!MyExtensions.Between(b1.Y, b2.Y, i.Y))
                        return new Vector2(float.NaN, float.NaN);
                }

                return i;
            }
            //Same line 
            else
            {
                Vector2 aMiddle = Vector2.Divide((a1 + a2), 2.0f);
                float aRadius = (a1 - aMiddle).Length();
                Vector2 bMiddle = Vector2.Divide((b1 + b2), 2.0f);
                float bRadius = (b1 - bMiddle).Length();

                //if circle intersects 
                if (CircleCircleIntersection(aMiddle, aRadius, bMiddle, bRadius))
                {
                    return i;
                }
                else
                {
                    return new Vector2(float.NaN, float.NaN);
                }

            }
        }
        /// <summary>
        /// Return all point of intersections of circle & line 
        /// </summary>
        /// <param name="aMiddle"></param>
        /// <param name="aRadius"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static Vector2[] CircleLineSegmentIntersection(Vector2 aMiddle, float aRadius, Vector2 b1, Vector2 b2)
        {

            

            float A;
            float B;
            float C;

            float x0 = aMiddle.X;
            float y0 = aMiddle.Y;

            Vector2 d = b1 - b2;
            Vector2 n = new Vector2(d.Y*-1,d.X);
            float a = -d.Y;
            float b = d.X;
            float c = -a * b1.X - b * b1.Y;
            float r2 = aRadius * aRadius;


            Vector2 X_1 = Vector2.Zero;
            bool X_1isRoot = false; 
            Vector2 X_2 = Vector2.Zero;
            bool X_2isRoot = false; 

            if (a != 0)
            {
                A = 1 + (b * b) / (a * a);
                B = (c * b) / a + 2 * x0 * (b / a) + 2 * y0;
                C = (x0 * x0) + y0 * y0 - r2 + c * c / (a * a) + 2 * x0 * (c / a);

                var xs = MyExtensions.QuadraticSolver(A, B, C);
                if (xs != null)
                {
                    if (!float.IsNaN(xs.Item2))
                    {
                        X_2.Y = xs.Item2;
                        X_2.X = (-b * X_2.Y - c) / a;
                        X_2isRoot = true;
                    }

                    X_1.Y = xs.Item1;
                    X_1.X = (-b * X_1.Y - c) / a;
                    X_1isRoot = true; 

                }
            }
            else if(b != 0)
            {
                A = 1 + (a * a) / (b * b);
                B = (c * a) / b + 2 * y0 * (a / b) + 2 * x0;
                C = x0 * x0 + y0 * y0 - r2 + c * c / (b * b) + 2 * y0 * (c / b);

                var xs = MyExtensions.QuadraticSolver(A, B, C);
                if (xs != null)
                {
                    if (!float.IsNaN(xs.Item2))
                    {
                        X_2.X = xs.Item2;
                        X_2.Y = (-a * X_2.X - c) / b;
                        X_2isRoot = true;
                    }

                    X_1.X = xs.Item1;
                    X_1.Y = (-a * X_1.X - c) / b;
                    X_1isRoot = true;
                }
            }
            else
            {
                throw new ArgumentException("Not a line in input! " + b1.ToString() + ' ' +b2.ToString());
            }
            

            //Intersection points 

           
            Debug.Assert(!X_1isRoot || MyExtensions.PointOnLine(b1,b2,X_1));
            Debug.Assert(!X_1isRoot || MyExtensions.PointOnCircle(aMiddle,aRadius,X_1));
            Debug.Assert(!X_2isRoot || MyExtensions.PointOnLine(b1, b2, X_2));
            Debug.Assert(!X_2isRoot || MyExtensions.PointOnCircle(aMiddle, aRadius, X_2));


            if (X_1isRoot && MyExtensions.PointOnLineSegment(b1, b2, X_1))
            {
                if (X_2isRoot&& MyExtensions.PointOnLineSegment(b1, b2, X_2))
                    return new[] {X_1, X_2};
                else
                    return new[] {X_1};
            }
            else
            {
                if (X_2isRoot && MyExtensions.PointOnLineSegment(b1, b2, X_2))
                    return new[] { X_2 };
                else
                    return new Vector2[0];
            }
           
        }

    }
    public static class MyExtensions
    {
        /// <summary>
        /// Test if x lies between a & b 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool Between(float a, float b, float x)
        {
            float upper;
            float lower;

            if (a > b)
            {
                upper = a;
                lower = b;
            }
            else
            {
                upper = b;
                lower = a;
            }

            if (x >= lower && x <= upper)
                return true;
            return false;
        }
        /// <summary>
        /// Returns true if P lies on line segment given by A,B point
        /// It has to lie on the Line 
        /// </summary>
        public static bool PointOnLineSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            Debug.Assert(PointOnLine(a,b,p));
            return MyExtensions.Between(a.X, b.X, p.X) && MyExtensions.Between(a.Y, b.Y, p.Y);

        }

        public static bool PointOnLine(Vector2 l1, Vector2 l2, Vector2 p)
        {
            //Perpendicular vector to line
            Vector2 n = new Vector2(-1*(l1.Y - l2.Y), l1.X - l2.X);
            float c = -1 * (n.X * l1.X + n.Y * l1.Y);


            return (Math.Abs(n.X * p.X + n.Y * p.Y + c) < 0.0001);

        }

        public static bool PointOnCircle(Vector2 m, float radius, Vector2 p)
        {
            return (Math.Abs(Vector2.Distance(m, p) - radius) < 0.0001);
        }

        public static Tuple<float,float> QuadraticSolver(float a, float b, float c)
        {
            Debug.Assert(a != 0);

            var d = b * b - 4 * a * c;
            if (d < 0)
                return null; 
            else if (d == 0)
                return new Tuple<float, float>(-b / (2 * a), float.NaN);
            else
            {
                var sqrtD = (float) Math.Sqrt(d);
                return new Tuple<float, float>((-b+sqrtD)/(2*a),(-b-sqrtD)/(2*a));
            }
        }
    }
}

