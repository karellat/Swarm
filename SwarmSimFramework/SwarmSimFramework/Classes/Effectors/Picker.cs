using System.Numerics;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Effectors
{
    /// <summary>
    /// Effector implementing picking up from map
    /// Three state control
    /// need of action
    /// [0] - PUT [1] - Pick up [2] - IDle
    /// </summary>
    public class Picker : LineEntity,IEffector
    {
        /// <summary>
        /// Dimension of picker 
        /// </summary>
        [JsonProperty]
        public int Dimension { get; protected set; }
        /// <summary>
        /// Local bounds 0-1 - put 1-2 - idle 2-3 pick
        /// </summary>
        [JsonProperty]
        public Bounds[] LocalBounds { get; protected set; }
        /// <summary>
        /// Normalization fnc from brain
        /// </summary>
        [JsonProperty]
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        /// <summary>
        /// Orientation shift to FPoint of robot
        /// </summary>
        [JsonProperty]
        protected float OrientationToRobotFPoint;
        /// <summary>
        /// Creates new picker with given  length & orientation to robot FPoint
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="length"></param>
        /// <param name="orientation"></param>
        public Picker(RobotEntity robot, float length, float orientation) :
            base(robot.FPoint, Entity.MovePoint(robot.FPoint, Vector2.Normalize(robot.FPoint - robot.Middle) * length),
                robot.Middle, "Picker Effector")
        {
            //Rotate sensor to its possition
            OrientationToRobotFPoint = orientation; 
            this.RotateRadians(OrientationToRobotFPoint + robot.Orientation);
            
            //Dimension
            Dimension = 3;
            LocalBounds = new Bounds[Dimension];
            for (int i = 0; i < 3; i++)
            {
                LocalBounds[i] = new Bounds() {Max = 1, Min = 0};
            }
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);
        }
        [JsonConstructor]
        protected Picker() : base("Picker Effector")
        {
            
        }
        /// <summary>
        /// Make deep clone of effector 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }

        /// <summary>
        /// Set up normalization fncs 
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
            //Connect to the middle of the robot
            this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
            this.MoveTo(robot.Middle);
            //Count normalization 
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }

        /// <summary>
        /// Based on settings pick up entity or put down from container
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            lastSettings = settings;
            //Update possition 
            if (robot.Middle != this.RotationMiddle)
                this.MoveTo(robot.Middle);
            if (Orientation != robot.Orientation + OrientationToRobotFPoint)
                this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
            //put entity from container
            if (settings[0] >= settings[1] && settings[0] >= settings[2])
            {
                //if empty return
                if (robot.ActualContainerCount == 0)
                {
                    robot.InvalidContainerOperation++;
                    return; 
                }
                var removingEntity = robot.PeekContainer();
                //Move to the end point of sensor 
                removingEntity.MoveTo(B);
                //Check collision
                if (map.Collision(removingEntity) || map.OutOfBorderTest(removingEntity))
                {
                    robot.InvalidContainerOperation++;
                    return;
                }
                //Add to map
                else
                {
#if DEBUG && POSCORRECT
                    map.CheckCorrectionOfPossition();
#endif 
                    if (removingEntity is FuelEntity)
                        map.FuelEntities.Add((FuelEntity) robot.PopContainer());
                    else
                        map.PasiveEntities.Add(robot.PopContainer());
#if DEBUG && POSCORRECT
                    map.CheckCorrectionOfPossition();
#endif 
                }
            }
            //pick from map
            else if (settings[1] >= settings[0] && settings[1] >= settings[2])
                {
                //Check, if picker collides with anything
                var intersections = map.Collision(this, robot);
                //if no intersection
                if (float.IsPositiveInfinity(intersections.Distance))
                {
                    robot.InvalidContainerOperation++;
                    return;
                }
                //Check if entity is Circle & size is suitable for container & its not RobotEntity
                if (intersections.CollidingEntity is CircleEntity &&
                    ((CircleEntity) intersections.CollidingEntity).Radius * 2 <= this.Length && (!(intersections.CollidingEntity is RobotEntity)))
                {
                    if (robot.PushContainer((CircleEntity) intersections.CollidingEntity))
                    {
                        if (intersections.CollidingEntity is FuelEntity)
                            map.FuelEntities.Remove((FuelEntity) intersections.CollidingEntity);
                        else
                            map.PasiveEntities.Remove((CircleEntity) intersections.CollidingEntity); 
                    }
                    else
                    {
                        robot.InvalidContainerOperation++;
                        return;
                    }
                }
                else
                {
                    robot.InvalidContainerOperation++;
                    return; 
                }

            }
            //idle 
            else
            {
                return;
            }
        }

        /// <summary>
        /// Make Clone of Picker
        /// </summary>
        /// <returns></returns>
        public IEffector Clone()
        {
            return (IEffector) DeepClone();
        }
        /// <summary>
        /// Last settings 
        /// </summary>
        public float[] lastSettings = new float[3];
        public override StringBuilder Log()
        {
            StringBuilder s = new StringBuilder("Picker: ");
            s.AppendLine("\t" + base.Log());
            s.AppendLine("Put: " +  lastSettings[0]  + ",Pick up: " + lastSettings[1] + ",Idle: " + lastSettings[2]);
            return s;
        }
    }
}