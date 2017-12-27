using System.CodeDom;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    /// <summary>
    /// Implement radio sensor,return if signals and mean direction to them (
    /// </summary>
    public class RadioSensor : CircleEntity, ISensor
    {
        //MEMBERS 
        /// <summary>
        /// Dimension of sensor 
        /// </summary>
        [JsonProperty]
        public int Dimension { get; protected set; }
        /// <summary>
        /// Bounds of specific dimension of out put
        /// </summary>
        [JsonProperty]
        public Bounds[] LocalBounds { get; protected set; }
        /// <summary>
        /// Normalization funcs, from read values to output(robot suitable)
        /// </summary>
        [JsonProperty]
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        /// <summary>
        /// Bounds of output 
        /// </summary>
        public static Bounds CoordinateBounds = new Bounds {Max = 1, Min = -1};
        /// <summary>
        /// Create new Sensor & connect it to Robot 
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="sensorRange"></param>
        public RadioSensor(RobotEntity robot,float sensorRange):base(robot.Middle,sensorRange,"RadioSensor")
        {
            //No need for orientation
            FPoint = Middle;
            RotationMiddle = Middle; 
            //Set bounds 
           
            Dimension = 3;
            LocalBounds = new Bounds[Dimension];
            LocalBounds[0] = new Bounds() {Min = 0, Max = 1};
            LocalBounds[1] = new Bounds() { Min = 0, Max = 1 };
            LocalBounds[2] = new Bounds() { Min = 0, Max = 1 };
            //Create normalization funcs
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);

        }
        /// <summary>
        /// Json COnvertor
        /// </summary>
        [JsonConstructor]
        protected RadioSensor() : base("RadioSensor")
        { }
        /// <summary>
        /// Make clone of radiosensor 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }
        public float[] Count(RobotEntity robot, Map.Map map)
        {
            //Update location 
            if (robot.Middle != Middle)
            {
                this.MoveTo(robot.Middle);
                this.RotationMiddle = robot.Middle;
            }
            //Find intersections 
            var intertesections = map.CollisionRadio(this);
            //Prepare ouput
            float[] o = new float[Dimension];
            //find if any intersection with radio signals 
            for (int i = 0; i < (int) RadioEntity.SignalValueBounds.Max; i++)
            {
                if (intertesections.ContainsKey(i))
                {
                    o[i] = 1;
                    //Vector direction 
                    Vector2 dir = intertesections[i].MeanDirection();
                }
                else
                {
                    o[i] = 0;
                }
            }
            LastReadValues = o;
            return o.Normalize(NormalizeFuncs);
        }
        /// <summary>
        /// Set middle of 
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
            this.RotationMiddle = robot.Middle;
            this.Middle = robot.Middle;
            this.FPoint = this.Middle;
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }
        /// <summary>
        /// Make clone of current sensor 
        /// </summary>
        /// <returns></returns>
        public ISensor Clone()
        {
            return (ISensor) DeepClone();
        }
        /// <summary>
        /// Last read values 
        /// </summary>
        [JsonProperty]
        public float[] LastReadValues = new float[3];
        /// <summary>
        /// Log local info
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s =new StringBuilder("Radio Sensor: ");
            s.AppendLine("\t" + base.Log());
            s.AppendLine("Radio signal 0: " + LastReadValues[0]);
            s.AppendLine("Radio signal 1: " + LastReadValues[1]);
            s.AppendLine("Radio signal 2: " + LastReadValues[2]);
            return s; 
        }
    }
}