using System;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.RobotBrains
{
    /// <summary>
    /// Constant brains returning given output
    /// </summary>
    public class FixedBrain : IRobotBrain
    {
        public FixedBrain(IODimension dimensions, Bounds inOutBounds)
        {
            IoDimension = dimensions;
            InOutBounds = inOutBounds;
            output = new float[dimensions.Output];
            for (int i = 0; i < output.Length; i++)
                output[i] = InOutBounds.Max;
        }
        public double Fitness { get; set; }
        public Func<float, float> ActivationFnc { get; }
        public IODimension IoDimension { get; }
        public Bounds InOutBounds { get; }
        public float[] output { get; set; }
        public float[] Decide(float[] readValues)
        {
            return output;
        }

        public IRobotBrain GetCleanCopy()
        {
            return (IRobotBrain) this.MemberwiseClone();
        }
    }
}