using System;

namespace SwarmSimFramework.SupportClasses
{
    public static class RandomNumber
    {
        private static Random randomNumber = new Random(13051995);

        public static int GetRandomInt(int min, int max)
        {
            return randomNumber.Next(min, max);
        }

        public static int GetRandomInt(int max)
        {
            return randomNumber.Next(max);
        }

        public static int GetRandomInt()
        {
            return randomNumber.Next();
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
        public static bool Between(float a, float b,float x)
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

            if (x > lower && x < upper)
                return true;
            return false;
        }
        public static bool QuadraticSolver(double A, double B, double C, out double[] x)
        {
            double D = B * B - 4 * A * C;
            if (D < 0)
            {
                x = null;
                return false;
            }
            else
            {
                double x_0 = (-B + Math.Sqrt(D)) / (2.0 * A);
                double x_1 = (-B - Math.Sqrt(D)) / (2.0 * A);

                if (D == 0)
                {
                    x = new double[1];
                    x[0] = x_0;
                    return true;
                }
                else
                {
                    x = new double[2];
                    x[0] = x_0;
                    x[1] = x_1;
                    return true;
                }
            }
        }
    }
}