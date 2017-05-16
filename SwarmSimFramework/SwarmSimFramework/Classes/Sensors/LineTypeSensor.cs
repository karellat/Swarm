using System;
using System.Data;
using System.Linq;
using System.Numerics;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    public class LineTypeSensor : LineEntity,ISensor
    {
        //PUBLIC MEMBERS 

        /// <summary>
        /// Dimension of the ouput
        /// </summary>
        public int Dimension { get; protected set;  }

        public Bounds[] LocalBounds { get; protected set; }
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        /// <summary>
        /// Orientation to robot FPoint, adds to the robot orientationToRobotFPoint to rotate to correct possition 
        /// </summary>
        protected float OrientationToRobotFPoint;
        /// <summary>
        /// Return clone of this LineTypeSensor
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (LineEntity) this.MemberwiseClone(); 
        }

        /// <summary>
        /// Return read value from map,connected to the robot 
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        //CONSTRUCTOR 
        public LineTypeSensor(RobotEntity robot,float lenght,float orientationToRobotFPoint) : base(robot.FPoint, Entity.MovePoint(robot.FPoint, Vector2.Normalize(robot.FPoint - robot.Middle) * lenght),robot.Middle, "Line Sensors")
        {
            //rotate sensor to its possition
            OrientationToRobotFPoint = orientationToRobotFPoint;
            this.RotateRadians(orientationToRobotFPoint+robot.Orientation);

            //OutputSize, returning LengthSqrt & Type of colliding entity 
            Dimension = 2; 
            //Normalize output
            LocalBounds = new Bounds[Dimension];
            // sqrt distance bounds 
            LocalBounds[0] = new Bounds() {Min = 0, Max = lenght};
            // amount of types, colors  + 1 for null 
            LocalBounds[1] = new Bounds() {Min = 0, Max = Entity.EntityColorCount}; 
            //Create normalization func to robot normal values
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }


        public virtual float[] Count(RobotEntity robot, Map.Map map)
            {
                //Update possition 
                if(robot.Middle != this.RotationMiddle)
                    this.MoveTo(robot.Middle);
                if(Orientation != robot.Orientation + OrientationToRobotFPoint)
                    this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
                //Count from the map 
                Intersection intersection = map.Collision(this, robot);
                float[] output;
                if (intersection.Distance == Double.PositiveInfinity)
                    output = new[] {LocalBounds[0].Max, LocalBounds[1].Max};
                else 
                    output = new[] {(float) Math.Sqrt(intersection.Distance), (float) intersection.CollidingEntity.Color};
            //Normalize output
                return output.Normalize(NormalizeFuncs);
            }

        public void ConnectToRobot(RobotEntity robot)
        {
            //Connect to the middle of the robot
            this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
            this.MoveTo(robot.Middle);
            //Count normalization 
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }

        /// <summary>
        /// Return clone of this sensor
        /// </summary>
        /// <returns></returns>
        public ISensor Clone()
        {
            return (LineTypeSensor) this.DeepClone();
        }

      }
}