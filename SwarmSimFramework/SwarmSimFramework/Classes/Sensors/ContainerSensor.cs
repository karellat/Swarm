using System;
using System.CodeDom;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    public class ContainerSensor : CircleEntity, ISensor
    {
        private int _maxSizeOfContainer;
        /// <summary>
        /// Last read values
        /// </summary>
        public float[] LastReadValues = new float[1];

        public float[] Count(RobotEntity robot, Map.Map map)
        {
            Middle = robot.Middle;
            FPoint = Middle;

            if (robot.ActualContainerCount > _maxSizeOfContainer)
                throw new ArgumentException("Wrong container to count");

            float[] o = new [] {(float) robot.ActualContainerCount};

            LastReadValues = o;
            o[0] = NormalizeFuncs[0].Normalize(o[0]);
            Debug.Assert(robot.ActualContainerCount != 0 || Math.Abs(o[0] - (-100.0f)) < 0.1);
            Debug.Assert(robot.ActualContainerCount != robot.ContainerMaxCapacity || Math.Abs(o[0] - 100.0f) < 0.1);
            return o; 
        }

        public void ConnectToRobot(RobotEntity robot)
        {
            Middle = robot.Middle;
            FPoint = Middle;
            _maxSizeOfContainer = robot.ContainerMaxCapacity;
            LocalBounds[0] = new Bounds() { Max = _maxSizeOfContainer, Min = 0 };
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }

        public int Dimension { get; }
        public Bounds[] LocalBounds { get; protected set; }
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        public ISensor Clone()
        {
            var s = (ContainerSensor)DeepClone();
            s.LocalBounds = new Bounds[this.LocalBounds.Length];
            this.LocalBounds.CopyTo(s.LocalBounds, 0);
            return s;
        }

        public override Entity DeepClone()
        {
            return (Entity)this.MemberwiseClone();
        }

        public override StringBuilder Log()
        {
           return new StringBuilder("Container Sensor - max: " + _maxSizeOfContainer + "actual size: " + LastReadValues[0]);
        }

        public ContainerSensor(RobotEntity robot) : base(robot.Middle, 0, "ContainerSensor")
        {
            _maxSizeOfContainer = robot.ContainerMaxCapacity;
            if(_maxSizeOfContainer <= 0)
                throw new ArgumentException("Not suitable size of container: " + _maxSizeOfContainer);
            Dimension = 1;
            LocalBounds = new Bounds[1];
            LocalBounds[0] = new Bounds() { Max = _maxSizeOfContainer, Min = 0 };
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }
        /// <summary>
        /// Json Constructor
        /// </summary>
        [JsonConstructor]
        protected ContainerSensor() : base("ContainerSensor") { }
    }
}