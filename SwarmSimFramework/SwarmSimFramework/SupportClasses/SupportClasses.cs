using System;
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

        public override string ToString()
        {
            return "IN: " + Input + "; OUT: " + Output;
        }
    }
  
}