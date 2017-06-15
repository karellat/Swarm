using System.Data.SqlTypes;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Effectors
{
    /// <summary>
    /// Two state effector implemented as  line, if  coliding with RawMaterialEntity refactor to wood 
    /// if more than zero try to refactor 
    /// </summary>
    public class WoodRefactor: LineEntity, IEffector
    {
        /// <summary>
        /// Orientation to robot FPoint
        /// </summary>
        [JsonProperty]
        public float OrientationToRobotFPoint { get; protected set; }
        /// <summary>
        /// Intern values bounds
        /// </summary>
        [JsonProperty]
        public Bounds[] LocalBounds { get; protected set; }
        /// <summary>
        /// Normalization fncs 
        /// </summary>
        [JsonProperty]
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        /// <summary>
        /// Dimension of effector 
        /// </summary>
        [JsonProperty]
        public int Dimension { get; protected set; }
        /// <summary>
        /// Create wood refactor 
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="length"></param>
        /// <param name="orientation"></param>
        public WoodRefactor(RobotEntity robot, float length, float orientation) :
            base(robot.FPoint, Entity.MovePoint(robot.FPoint, Vector2.Normalize(robot.FPoint - robot.Middle) * length),
                robot.Middle, "Wood refactor")
        {
            //Rotate sensor to its possition
            OrientationToRobotFPoint = orientation;
            this.RotateRadians(OrientationToRobotFPoint + robot.Orientation);

            Dimension = 1; 
            LocalBounds = new [] {new Bounds(){Min=-1,Max = 1}, };

            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound,LocalBounds);
        }
        /// <summary>
        /// Json Constructor
        /// </summary>
        [JsonConstructor]
        protected WoodRefactor() : base("Wood refactor")
        {
            
        }
        /// <summary>
        /// Make clone woodrefactor 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }

        /// <summary>
        /// Connect to  robot
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
        /// [0] - if more than zero make refactor wood if posible
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            var s = settings.Normalize(NormalizeFuncs)[0];
            //Update possition 
            if (robot.Middle != this.RotationMiddle)
                this.MoveTo(robot.Middle);
            if (Orientation != robot.Orientation + OrientationToRobotFPoint)
                this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);

            //Decide if refactor 
            if (settings[0] > 0)
            {
                var intersection = map.Collision(this, robot);
                //if material to refactor 
                if (!float.IsPositiveInfinity(intersection.Distance) && intersection.CollidingEntity != null && intersection.CollidingEntity.Color == EntityColor.RawMaterialColor)
                {
                    var c = (RawMaterialEntity) intersection.CollidingEntity;
                    map.PasiveEntities.Remove(c);
                    map.PasiveEntities.Add(new WoodEntity(c.Middle,c.Radius,c.MaterialToRefactor));
                }
                else
                {
                    robot.InvalidRefactorOperation++;
                    return;
                }
            }
            else
            {
                //IDLE 
            }
        }

        /// <summary>
        /// Created clone of effector
        /// </summary>
        /// <returns></returns>
        public IEffector Clone()
        {
            return (IEffector) DeepClone();
            
        }

        public float[] lastSettings = new float[1];
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
           StringBuilder s = new StringBuilder("WoodRefactor: ");
            s.AppendLine("\t " + base.Log());
            s.AppendLine("Refactor settings: " + lastSettings[0]);
            return s;
        }
    }
}