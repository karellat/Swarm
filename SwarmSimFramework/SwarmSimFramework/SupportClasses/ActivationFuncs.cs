using System;

namespace SwarmSimFramework.SupportClasses
{

    /// <summary>
    /// Implementation of activation funcs 
    /// </summary>
    public static class ActivationFuncs
    {
        /// <summary>
        /// Float version of e 
        /// </summary>
        public const float FE = (float) Math.E;
        /// <summary>
        /// Parameteric rectified linear unit
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float PReLU(float a, float x)
        {
            return x < 0 ? a * x : x;
        }
        /// <summary>
        /// TanH activation func, [-1,1] 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float TanH(float x)
        {
            return 2 / (1 + (float) Math.Pow(FE, -(2 * x))) - 1;
        }
        /// <summary>
        /// TanH activation func, [-resize,resize] 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float ResieTanh(float resize, float x)
        {
            return resize * TanH(x);
        }

    }
}