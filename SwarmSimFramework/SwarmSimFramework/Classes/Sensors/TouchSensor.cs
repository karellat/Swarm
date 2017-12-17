using System;
using System.Diagnostics.Tracing;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    public class TouchSensor : CircleEntity, ISensor
    {
    //MEMBERs
        /// <summary>
        /// Dimension of touch  sensors 
        /// </summary>
        [JsonProperty]
        public int Dimension { get; protected set; }
        /// <summary>
        /// Local maximum and minimum for given dimension 
        /// </summary>
        [JsonProperty]
        public Bounds[] LocalBounds { get; protected set; }
        /// <summary>
        /// Normalization functions for given robot.
        /// </summary>
        [JsonProperty]
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        /// <summary>
        /// Maxoutput of touch sensor 
        /// </summary>
        //PRIVATE MEMBERs
        [JsonProperty]
        protected float OrientationToRobotFPoint;
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="name"></param>
        /// <param name="orientationToFPoint"></param>
        public TouchSensor(RobotEntity robot,float size, float orientationToFPoint) : base(Entity.RotatePoint(orientationToFPoint, robot.FPoint, robot.Middle),size, "TouchSensor",robot.Middle)
        {
            //ISensor values
            this.OrientationToRobotFPoint = orientationToFPoint;
            Dimension = 1; 
            LocalBounds = new Bounds[Dimension];
            //Make bounds of touch sensor 
            LocalBounds[0] = new Bounds() {Max = 1, Min = 0};
            NormalizeFuncs = Entity.MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound); 
        }
        /// <summary>
        /// Json Constructor
        /// </summary>
        [JsonConstructor]
        protected TouchSensor() : base("TouchSensor")
        {
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (TouchSensor) this.MemberwiseClone();
        }

        public float[] Count(RobotEntity robot, Map.Map map)
        {
            //Update possition
            ConnectToRobot(robot);
            float[] output;
            //Count collision 
            if (map.Collision(this, robot))
                output = (new[] {1.0f});
            else
                output = (new[] {0.0f});
            LastReadValue[0] = output[0];
            return output.Normalize(NormalizeFuncs);
        }

        public void ConnectToRobot(RobotEntity robot)
        {
            //find the location of beginning of touch sensor
            Middle = Entity.RotatePoint(OrientationToRobotFPoint, robot.FPoint, robot.Middle);
            RotationMiddle = robot.Middle;
            FPoint = Middle;
            Orientation = robot.Orientation;
            //Normalize
            NormalizeFuncs = Entity.MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }
        public float[] LastReadValue = new float[1];
        public ISensor Clone()
        {
            return (ISensor) this.DeepClone(); 
        }
        /// <summary>
        /// Log current TouchSensor 
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s = new StringBuilder("TouchSensor ");
            s.AppendLine("\t" + base.Log());
            s.Append("\t Last read value: " + LastReadValue[0] + " = ");
            if (LastReadValue[0] == 1.0f)
                s.AppendLine("Is Coliding");
            else
                s.AppendLine("Is not coliding");
            return s;
        }
    }
}