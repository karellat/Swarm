using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
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
        //Output Bounds
        public float Max;
        public float Min;

        public float Normalize(float x)
        {
            var o = (x* Rescale) +Shift;

            if (o > Max)
            {
                //Float precision correction
                if (o - Max < 1)
                {
                    o = Max;
                }
                else
                    throw new ArgumentOutOfRangeException("Normalize Func gets bigger number than expected: " + x.ToString());
            }
            else if (o < Min)
            {
                if (Min - o < 1)
                {
                    o = Min;
                }
                else
                    throw new ArgumentOutOfRangeException("Normalize Func gets smaller number than expected: " + x.ToString());

            }

            return o;
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