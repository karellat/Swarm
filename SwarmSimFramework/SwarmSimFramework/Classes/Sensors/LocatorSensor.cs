using System.Numerics;
using System.Xml.Schema;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    /// <summary>
    /// Return position and direction vector 
    /// </summary>
    public class LocatorSensor : CircleEntity,ISensor
    {

        public int Dimension { get; }
        public Bounds[] LocalBounds { get; }
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
            LocalBounds[2] = new Bounds() { Max = 0, Min = 0 };
            LocalBounds[3] = new Bounds() { Max = 0, Min = 0 };
        }

        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }

        public float[] Count(RobotEntity robot, Map.Map map)
        {
            Middle = robot.Middle;
            FPoint = Middle;
            //if map borders change, change normalization func
            if (map.MaxWidth != LocalBounds[0].Max || map.MaxHeight != LocalBounds[1].Max)
            {
                LocalBounds[0].Max = map.MaxWidth;
                LocalBounds[2].Max = map.MaxWidth;
                LocalBounds[1].Max = map.MaxHeight;
                LocalBounds[3].Max = map.MaxHeight;
                NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
            }

            Vector2 dirV = Vector2.Normalize(robot.FPoint - robot.Middle);

            float[] o = new[] {robot.Middle.X, robot.Middle.Y, dirV.X, dirV.Y};
            return o.Normalize(NormalizeFuncs);

        }

        public void ConnectToRobot(RobotEntity robot)
        {
            Middle = robot.Middle;
            FPoint = Middle;
            LocalBounds[0] = new Bounds() { Max = 0, Min = 0 };
            LocalBounds[1] = new Bounds() { Max = 0, Min = 0 };
            LocalBounds[2] = new Bounds() { Max = 0, Min = 0 };
            LocalBounds[3] = new Bounds() { Max = 0, Min = 0 };
        }


        public ISensor Clone()
        {
            return (ISensor) DeepClone();
        }
    }
}