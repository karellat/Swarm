using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Xml.Schema;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    /// <summary>
    /// Return position and direction vector 
    /// </summary>
    public class LocatorSensor : CircleEntity,ISensor
    {
        /// <summary>
        /// Dimension of the locator sensor
        /// </summary>
        public int Dimension { get; }
        /// <summary>
        /// Intern value bounds
        /// </summary>
        public Bounds[] LocalBounds { get; protected set; }
        /// <summary>
        /// Normalization functions to robot values
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        /// <summary>
        /// Locator sensor constructor
        /// </summary>
        /// <param name="robot"></param>
        public LocatorSensor(RobotEntity robot) : base(robot.Middle, 0, "LocatorSensor")
        { 
           //Return robot Middle X,Y and then direction Vector X,Y
            Dimension = 4;
            LocalBounds = new Bounds[4];
            LocalBounds[0] = new Bounds() { Max = 0, Min = 0 };
            LocalBounds[1] = new Bounds() { Max = 0, Min = 0 };
            LocalBounds[2] = new Bounds() { Max = robot.Radius, Min = -robot.Radius };
            LocalBounds[3] = new Bounds() { Max = robot.Radius, Min = -robot.Radius };
        }
        /// <summary>
        /// Make deep clone of sensor
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }
        /// <summary>
        /// Count output from robot position, if map size change bounds and normalization fnc
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public float[] Count(RobotEntity robot, Map.Map map)
        {
            Middle = robot.Middle;
            FPoint = Middle;
            //if map borders change, change normalization func
            if (map.MaxWidth != LocalBounds[0].Max || map.MaxHeight != LocalBounds[1].Max)
            {
                LocalBounds[0].Max = map.MaxWidth;
                LocalBounds[1].Max = map.MaxHeight;
                NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
            }
           
            Vector2 dirV = Vector2.Normalize(robot.FPoint - robot.Middle);

            float[] o = new[] {robot.Middle.X, robot.Middle.Y, dirV.X, dirV.Y};
            LastReadValues = o;
            return o.Normalize(NormalizeFuncs);

        }
        /// <summary>
        /// Change to robot bounds
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
            Middle = robot.Middle;
            FPoint = Middle;
            LocalBounds[0] = new Bounds() { Max = 0, Min = 0 };
            LocalBounds[1] = new Bounds() { Max = 0, Min = 0 };
            LocalBounds[2] = new Bounds() { Max = robot.Radius, Min = -robot.Radius };
            LocalBounds[3] = new Bounds() { Max = robot.Radius, Min = -robot.Radius };
        }
        /// <summary>
        /// Return clone of sensor
        /// </summary>
        /// <returns></returns>
        public ISensor Clone()
        {
            var s = (LocatorSensor) DeepClone();
            s.LocalBounds = new Bounds[this.LocalBounds.Length];
            this.LocalBounds.CopyTo(s.LocalBounds,0);
            return s;
        }
        /// <summary>
        /// Last read values
        /// </summary>
        public float[] LastReadValues = new float[4];
        /// <summary>
        /// Log locator
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s = new StringBuilder("LocatorSensor :");
            s.AppendLine("\t" + base.Log());
            s.AppendLine("Last located middle X = " + LastReadValues[0] + ", Y = " + LastReadValues[1] +
                         ", Direction : X = " + LastReadValues[2] + " Y = " + LastReadValues[3]);
            return base.Log();
        }
    }
}