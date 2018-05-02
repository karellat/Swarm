using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace SwarmSimFramework.SupportClasses
{
    /// Represent bounds of interval 
    public struct Bounds
    {
        /// <summary>
        /// Maximum bound 
        /// </summary>
        public float Max;

        /// <summary>
        /// Minimum bound
        /// </summary>
        public float Min;

        public bool In(float x)
        {
            if (float.IsNaN(x)) return false;
            if (float.IsInfinity(x)) return false;
            if (x >= Min && x <= Max)
                return true;
            return false;
        }
    }

    /// Normalize functions   
    public struct NormalizeFunc
    {
        public float Rescale;
        public float Shift;

        public float Normalize(float x)
        {
            return (x * Rescale) + Shift;
        }
    }

    /// <summary>
    /// Input output dimension
    /// </summary>
    public struct IODimension
    {
        public int Input;
        public int Output;

        public static IODimension Get(int input, int output)
        {
            return new IODimension()
            {
                Input = input,
                Output = output
            };
        }
        public override string ToString()
        {
            return "IN: " + Input + "; OUT: " + Output;
        }
    }


    public static class DictionaryExtension
    {
        public static void SafeIncrementBy(this Dictionary<string, double> dic, string Key, double value)
        {
            if (value == 0)
                return;
            if (!dic.ContainsKey(Key))
                dic.Add(Key, 0);

            dic[Key] += value;
        }
        public static void SafeIncrement(this Dictionary<string, double> dic, string Key)
        {
            dic.SafeIncrementBy(Key,1);
        }
    }
}