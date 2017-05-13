using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    /// <summary>
    /// Implement radio sensor, return two most frequent signals and mean direction to them (
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
        public static Bounds CoordinateBounds = new Bounds {Max = 100, Min = -100};
        /// <summary>
        /// Create new Sensor & connect it to Robot 
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="sensorRange"></param>
        public RadioSensor(RobotEntity robot,float sensorRange):base(robot.Middle,sensorRange,"RadioSensor")
        {
            //No need for orientation
            FPoint = Middle;
            //Set bounds 
            Dimension = 6;
            LocalBounds = new Bounds[6];
            LocalBounds[0] = RadioEntity.SignalValueBounds;
            LocalBounds[1] = CoordinateBounds;
            LocalBounds[2] = CoordinateBounds;
            LocalBounds[3] = RadioEntity.SignalValueBounds;
            LocalBounds[4] = CoordinateBounds;
            LocalBounds[5] = CoordinateBounds;
            //Create normalization funcs
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);

        }
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
            if(robot.Middle != Middle)
                this.MoveTo(robot.Middle);
           
           //Find intersections 
           var intertesections = map.CollisionRadio(this);

           //Find two most frequent signals
            RadioIntersection max = null;
            RadioIntersection max2 = null;
            foreach (var i in intertesections)
            {
                if (max == null || max.AmountOfSignal < i.Value.AmountOfSignal)
                {
                    max2 = max;
                    max = i.Value; 
                }
                else
                {
                    if (max2 == null || max2.AmountOfSignal < i.Value.AmountOfSignal)
                    {
                        max2 = i.Value;
                    }
                }
                
            }
            float[] o = new float[Dimension];
            var maxMeanDir = max != null ? KeepInBounds(CoordinateBounds,max.MeanDirection()) : Vector2.Zero;
            Vector2 max2MeanDir = max2 != null ? KeepInBounds(CoordinateBounds,max2.MeanDirection()) : Vector2.Zero;
            //check overflow 
            //value of the most frequent signal; 
            o[0] = max != null ? max.ValueOfSignal : RadioEntity.SignalValueBounds.Min;
            o[1] = maxMeanDir.X;
            o[2] = maxMeanDir.Y;

            o[3] = max2 != null ? max2.ValueOfSignal : RadioEntity.SignalValueBounds.Min;
            o[4] = max2MeanDir.X;
            o[5] = max2MeanDir.Y;
            return o.Normalize(NormalizeFuncs);
        }
        /// <summary>
        /// Set middle of 
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
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
    }
}