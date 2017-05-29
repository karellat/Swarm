using System;
using System.Text;
using SwarmSimFramework.Classes;

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

        public static int GetRandomFloat()
        {
            return (float) randomNumber.NextDouble();
        }

    }

    public static class MyExtensions
    {
        /// <summary>
        /// Return normalized array of float
        /// </summary>
        /// <param name="value"></param>
        /// <param name="normalizeFuncs"></param>
        /// <returns></returns>
        public static float[] Normalize(this float[] value, NormalizeFunc[] normalizeFuncs)
        {
            for (int i = 0; i < value.Length; i++)
            {
                value[i] = normalizeFuncs[i].Normalize(value[i]);
            }
            return value;
        }
        /// <summary>
        /// Return string of float values (2.0,2.5....)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStringList(this float[] values)
        {
            StringBuilder s = new StringBuilder("(");
            for (int i = 0; i < values.Length-1; i++)
            {
                s.Append(values[i] + ", ");
            }
            if (values.Length >= 1)
                s.Append(values[values.Length - 1]);
            s.Append(")");
            return s.ToString();

        }

    }
}