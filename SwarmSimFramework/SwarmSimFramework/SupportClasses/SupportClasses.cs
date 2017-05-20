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
    }
}