using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
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
        [JsonProperty]
        public int Dimension { get; protected set;  }
        /// <summary>
        /// Local bounds 
        /// </summary>
        [JsonProperty]
        public Bounds[] LocalBounds { get; protected set; }
        /// <summary>
        /// Normalize Funcs 
        /// </summary>
        [JsonProperty]
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        /// <summary>
        /// Orientation to robot FPoint, adds to the robot orientationToRobotFPoint to rotate to correct possition 
        /// </summary>
        [JsonProperty]
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
        public LineTypeSensor(RobotEntity robot,float lenght,float orientationToRobotFPoint,bool sameOriented = false) : base(robot.FPoint, Entity.MovePoint(robot.FPoint, Vector2.Normalize(robot.FPoint - robot.Middle) * lenght),robot.Middle, "Line Sensors",0, sameOriented)
        {
            //rotate sensor to its possition
            OrientationToRobotFPoint = orientationToRobotFPoint;
            this.RotateRadians(orientationToRobotFPoint+robot.Orientation);

            //OutputSize, returning LengthSqrt & Type of colliding entity 
            Dimension = 1 + EntityColorCount;
            LastReadValues = new float[Dimension];
            //Normalize output
            LocalBounds = new Bounds[Dimension];
            // sqrt distance bounds 
            LocalBounds[0] = new Bounds() {Min = 0, Max = lenght};
            // amount of types, 1- for given type 0 for none
            for (int i = 0; i < EntityColorCount; i++)
            {
                LocalBounds[i+1] = new Bounds() {Min=0,Max=1};
            }
            //Create normalization func to robot normal values
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
            //Check orientation 
#if DEBUG
            if(sameOriented)
                Debug.Assert(robot.FPoint == this.A);
#endif
        }

        /// <summary>
        /// Json Constructor
        /// </summary>
        [JsonConstructor]
        protected LineTypeSensor() : base("Line Sensors")
        {
        }

        public virtual float[] Count(RobotEntity robot, Map.Map map)
            {
                //Update possition 
                if(robot.Middle != this.RotationMiddle)
                    this.MoveTo(robot.Middle);
                if(Orientation != robot.Orientation + OrientationToRobotFPoint)
                    this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
#if DEBUG
                if (this.orientedSameAsFRobot)
                {
                    Debug.Assert(Math.Abs(robot.FPoint.X - A.X) < 0.05 && Math.Abs(robot.FPoint.Y - A.Y) < 0.05); 
                }
#endif
            //Count from the map 
            Intersection intersection = map.Collision(this, robot,true);
                float[] output = new float[Dimension];
                if (intersection.Distance == Double.PositiveInfinity)
                {
                    output[0] = LocalBounds[0].Max;
                }
                else
                {
                    //Coliding with border
                    if (intersection.CollidingEntity != null)
                    {
                    output[(int)intersection.CollidingEntity.Color + 1] =
                        LocalBounds[(int)intersection.CollidingEntity.Color + 1].Max;
                    }
                    else
                    {
                    //Coliding with the border
                        output[(int) EntityColor.ObstacleColor +1] = LocalBounds[(int)EntityColor.ObstacleColor + 1].Max;
                    }
                    if (intersection.Distance >= Length * Length)
                        output[0] = Length;
                    else
                        output[0] = (float) Math.Sqrt(intersection.Distance);
                }
            //Normalize output & mark old values 
                LastReadValues = output;
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
        /// <summary>
        /// Last read values 
        /// </summary>
        public float[] LastReadValues; 
        /// <summary>
        /// Log current sensor
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s = new StringBuilder("LineTypeSensor :");
            s.AppendLine( "\t" + base.Log());
            s.AppendLine("\t Length: " + LastReadValues[0].ToString("##.###"));
            for (int i = 0; i < EntityColorCount; i++)
            {
                EntityColor e = 0;
                e += i;
                s.AppendLine("\t " + e + " : "  + LastReadValues[i+1].ToString("##.###") );
            }
            return s;
        }
    }
}