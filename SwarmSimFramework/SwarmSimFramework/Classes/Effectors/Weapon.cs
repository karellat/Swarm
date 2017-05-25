using System.Numerics;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Effectors
{
    /// <summary>
    /// Weapon effector cause damage to friendly Robots or to enemy Robots
    /// need of: 
    /// [0]  => attack friend
    /// [1] => idle
    /// [2] => attack enemy 
    /// </summary>
    public class Weapon : LineEntity,IEffector
    {
        /// <summary>
        /// Intern values 
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// Normalization fncs from robot brain
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        /// <summary>
        /// Dimension of settings
        /// </summary>
        public int Dimension { get; }
        /// <summary>
        /// Rotation angle to FPoint of robot
        /// </summary>
        protected float OrientationToRobotFPoint;
        /// <summary>
        /// Damage of weapon
        /// </summary>
        public float Damage { get; protected set; }
        /// <summary>
        /// Create a weapon effector 
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="length"></param>
        /// <param name="damage"></param>
        /// <param name="orientationToFPoint"></param>
        public Weapon(RobotEntity robot, float length, float damage, float orientationToFPoint) :
            base(robot.FPoint, Entity.MovePoint(robot.FPoint, Vector2.Normalize(robot.FPoint - robot.Middle) * length),
                robot.Middle, "Weapon Effector")
        {
            //rotate sensor to its possition
            OrientationToRobotFPoint = orientationToFPoint;
            this.RotateRadians(orientationToFPoint + robot.Orientation);

            Damage = damage;
            Dimension = 3;

            LocalBounds = new [] {new Bounds(){Max=1, Min=0}, new Bounds() { Max = 1, Min = 0 }, new Bounds() { Max = 1, Min = 0 } };
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);
        }
        /// <summary>
        /// Make clone of weapon
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }
        /// <summary>
        /// Connect to robot
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
            //Connect to the middle of the robot
            this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
            this.MoveTo(robot.Middle);
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);
        }
        /// <summary>
        /// Attack robot, if colliding with any
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            //Update possition 
            if (robot.Middle != this.RotationMiddle)
                this.MoveTo(robot.Middle);
            if (Orientation != robot.Orientation + OrientationToRobotFPoint)
                this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);


            //Attack friend if possible 
            if (settings[0] >= settings[1] && settings[0] >= settings[2])
            {
                var i = map.Collision(this, robot);
                if (i.CollidingEntity is RobotEntity)
                {
                    RobotEntity e = (RobotEntity) i.CollidingEntity;
                    if (e.TeamNumber == robot.TeamNumber)
                    {
                        e.AcceptDamage(Damage,map);
                        return;
                    }
                }
                robot.InvalidWeaponOperation++;
                return;
            }
            //Attack enemy if possible
            else  if (settings[2] >= settings[0] && settings[2] >= settings[1])
                {
                var i = map.Collision(this, robot);
                if (i.CollidingEntity is RobotEntity)
                {
                    RobotEntity e = (RobotEntity) i.CollidingEntity;
                    if (e.TeamNumber != robot.TeamNumber)
                    {
                        e.AcceptDamage(Damage, map);
                        return;
                    }
                }
                robot.InvalidWeaponOperation++;
                return;
            }
            //Else idle
        }
        //Create clone of effector
        public IEffector Clone()
        {
            return (IEffector) DeepClone();
        }
        /// <summary>
        /// Last settings of weapon
        /// </summary>
        public float[] lastSettings = new float[3];
        /// <summary>
        /// Log weapon
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s = new StringBuilder("Weapon: ");
            s.AppendLine("\t" + base.Log());
            s.AppendLine("Attack friend: " + lastSettings[0] + " Idle: " + lastSettings[1] + "Attack enemy: " +
                         lastSettings[2]);

            return base.Log();
        }
    }
}