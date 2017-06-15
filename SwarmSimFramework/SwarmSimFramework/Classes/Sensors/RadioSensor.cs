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
        public int Dimension { get; }
        /// <summary>
        /// Bounds of specific dimension of out put
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// Normalization funcs, from read values to output(robot suitable)
        /// </summary>
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
            Dimension = 9;
            LocalBounds = new Bounds[Dimension];
            LocalBounds[0] = new Bounds() {Min = 0, Max = 1};
            LocalBounds[1] = CoordinateBounds;
            LocalBounds[2] = CoordinateBounds;
            LocalBounds[3] = new Bounds() { Min = 0, Max = 1 };
            LocalBounds[4] = CoordinateBounds;
            LocalBounds[5] = CoordinateBounds;
            LocalBounds[6] = new Bounds() { Min = 0, Max = 1 };
            LocalBounds[7] = CoordinateBounds;
            LocalBounds[8] = CoordinateBounds;
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
                    o[i * 3] = 1;
                    //Vector direction 
                    Vector2 dir = intertesections[i].MeanDirection();
                    o[i * 3 + 1] = dir.X;
                    o[i * 3 + 2] = dir.Y;
                }
                else
                {
                    o[i * 3] = 0;
                    o[i * 3 + 1] = 0;
                    o[i * 3 + 2] = 0; 

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
        public float[] LastReadValues = new float[9];
        /// <summary>
        /// Log local info
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s =new StringBuilder("Radio Sensor: ");
            s.AppendLine("\t" + base.Log());
            s.AppendLine("Radio signal 0: " + LastReadValues[0] + " Mean Vector:(" + LastReadValues[1] + "," +
                         LastReadValues[2] + ")");
            s.AppendLine("Radio signal 1: " + LastReadValues[3] + " Mean Vector:(" + LastReadValues[4] + "," +
                         LastReadValues[5] + ")");
            s.AppendLine("Radio signal 2: " + LastReadValues[6] + " Mean Vector:(" + LastReadValues[7] + "," +
                         LastReadValues[8]);

            return s; 
        }
    }
}