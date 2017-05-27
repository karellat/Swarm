using System;
using System.Text;
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
            Output = new float[dimensions.Output];
            for (int i = 0; i < Output.Length; i++)
                Output[i] = InOutBounds.Max;
        }
        public double Fitness { get; set; }
        public Func<float, float> ActivationFnc { get; }
        public IODimension IoDimension { get; }
        public Bounds InOutBounds { get; }
        public float[] Output { get; set; }
        public float[] Decide(float[] readValues)
        {
            return Output;
        }

        public IRobotBrain GetCleanCopy()
        {
            return (IRobotBrain) this.MemberwiseClone();
        }

        public StringBuilder Log()
        {
            var s = new StringBuilder("Fixed brain\n");
            s.AppendLine("\t"+Output.ToStringList() + "\n");
            return s;
        }

        public IRobotBrain DeserializeBrain(string jsonString)
        {
            throw new NotImplementedException();
        }

        public string SerializeBrain()
        {
            throw new NotImplementedException();
        }
    }
}