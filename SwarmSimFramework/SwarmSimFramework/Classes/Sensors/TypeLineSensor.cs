using System;

namespace SwarmSimFramework.Classes.Entities
{
    public class TypeLineSensor : LineSensor
    {
        public TypeLineSensor(RobotEntity robot, float lenght, float orientation) : base(robot, lenght, orientation)
        {
            Name = "TypeLineSensor";
        }

        public override float[] Count(RobotEntity robot, Map.Map map)
        {
            //Update possition 
            if (robot.Middle != this.RotationMiddle)
                this.MoveTo(robot.Middle);
            if (Orientation != robot.Orientation + OrientationToRobotFPoint)
                this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
            //Count from the map 
            throw new NotImplementedException();
        }
    }
}