using System;
using System.Numerics;
using System.Text;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    public class FuelLineSensor : LineEntity, ISensor
    {
        /// <summary>
        /// Dimension of output 
        /// </summary>
        public int Dimension { get; }
        /// <summary>
        /// Local bounds of read values 
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// Normalization funcs to  
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        /// <summary>
        /// Orientation to robot FPoint, adds to the robot orientationToFPoint to rotate to correct possition 
        /// </summary>
        protected float OrientationToFPointToRobotFPoint;

        public FuelLineSensor(RobotEntity robot, float lenght, float orientationToFPoint) : base(robot.FPoint, Entity.MovePoint(robot.FPoint, Vector2.Normalize(robot.FPoint - robot.Middle) * lenght), robot.Middle, "Line Fuel Sensors")
        {
            //rotate sensor to its possition
            OrientationToFPointToRobotFPoint = orientationToFPoint;
            this.RotateRadians(orientationToFPoint + robot.Orientation);

            //OutputSize, returning LengthSqrt & Type of colliding entity 
            Dimension = 1;
            //Normalize output
            LocalBounds = new Bounds[Dimension];
            // sqrt distance bounds 
            LocalBounds[0] = new Bounds() { Min = 0, Max = lenght };
            //Create normalization func to robot normal values
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }

        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }
        /// <summary>
        /// Count the intersection of nearest point
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public float[] Count(RobotEntity robot, Map.Map map)
        {
            //Update possition      
            if (robot.Middle != this.RotationMiddle)
            this.MoveTo(robot.Middle);
            if (Orientation != robot.Orientation + OrientationToFPointToRobotFPoint)
                this.RotateRadians((robot.Orientation + OrientationToFPointToRobotFPoint) - Orientation);
            //Count from the map 
            Intersection intersection = map.CollisionFuel(this);
            float[] output;
            if (intersection.Distance == Double.PositiveInfinity)
                output = new[] { LocalBounds[0].Max};
            else
                output = new[] { (float)Math.Sqrt(intersection.Distance)};
            //log & return normalized output 
            lastReadValues = output;
            return output.Normalize(NormalizeFuncs);
        }
        /// <summary>
        /// Connect to robot, prepare normalization fncs 
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
            //Connect to the middle of the robot
            this.RotateRadians((robot.Orientation + OrientationToFPointToRobotFPoint) - Orientation);
            this.MoveTo(robot.Middle);
            //Count normalization 
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }


        public ISensor Clone()
        {
            return (ISensor) DeepClone();
        }
        /// <summary>
        /// Cached last read values for Log method 
        /// </summary>
        private float[] lastReadValues;
        /// <summary>
        /// Log current state 
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s = new StringBuilder("FuelLineSensor : ");
            s.AppendLine("\t" + base.Log());
            if (lastReadValues[0] == LocalBounds[0].Max)
                s.AppendLine("\tMaximum read value; ");
            else
            {
                s.AppendLine("\t"+ lastReadValues[0].ToString("##.###") + " to nearest fuel");
            }

            return s;

        }
    }
}