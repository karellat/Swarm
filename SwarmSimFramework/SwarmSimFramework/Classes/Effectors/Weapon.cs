using System.Numerics;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Effectors
{
    public class Weapon : LineEntity,IEffector
    {
        protected float OrientationToRobotFPoint; 
        public Weapon(RobotEntity robot, float length, float damage, float orientationToFPoint) :
            base(robot.FPoint, Entity.MovePoint(robot.FPoint, Vector2.Normalize(robot.FPoint - robot.Middle) * length),
                robot.Middle, "Weapon Effector")
        {
            //rotate sensor to its possition
            OrientationToRobotFPoint = orientationToFPoint;
            this.RotateRadians(orientationToFPoint + robot.Orientation);
        }

        public override Entity DeepClone()
        {
            throw new System.NotImplementedException();
        }

        public int Dimension { get; }
        public void ConnectToRobot(RobotEntity robot)
        {
            //Connect to the middle of the robot
            this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
            this.MoveTo(robot.Middle);
        }

        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            //Update possition 
            if (robot.Middle != this.RotationMiddle)
                this.MoveTo(robot.Middle);
            if (Orientation != robot.Orientation + OrientationToRobotFPoint)
                this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
        }

        public Bounds[] LocalBounds { get; }
        public NormalizeFunc[] NormalizeFuncs { get; }
        public IEffector Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}