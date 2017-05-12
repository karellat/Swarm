using System;
using System.Diagnostics.Tracing;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace SwarmSimFramework.Classes.Entities
{
    public class TouchSensor:CircleEntity,ISensor
    {
        //MEMBERs

        /// <summary>
        /// Dimension of touch  sensors 
        /// </summary>
        public int Dimension { get; }
        /// <summary>
        /// Maxoutput of touch sensor 
        /// </summary>
        public float MaxOuputValue { get; }
        /// <summary>
        /// Minoutput if touch sensor
        /// </summary>
        public float MinOutputValue { get; }
        //PRIVATE MEMBERs
        /// <summary>
        /// Rescale to normalized value of body 
        /// </summary>
        protected float RescaleOutput;
        /// <summary>
        /// Shift to normalized value of body 
        /// </summary>
        protected float ShiftOutput;
        /// <summary>
        /// Orientation to robot FPoint, adds to the robot orientationToFPoint to rotate to correct possition 
        /// </summary>
        protected float OrientationToRobotFPoint;
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="name"></param>
        /// <param name="orientationToFPoint"></param>
        public TouchSensor(RobotEntity robot,float size, float orientationToFPoint) : base(Entity.RotatePoint(orientationToFPoint, robot.FPoint, robot.Middle),size, "TouchSensor",robot.Middle)
        {
            //ISensor values
            MaxOuputValue = Radius;
            MinOutputValue = 0;
            Dimension = 1; 
            RescaleOutput = RescaleInterval(MinOutputValue, MaxOuputValue, robot.NormalizeMin, robot.NormalizeMax);
            ShiftOutput = ShiftInterval(MinOutputValue, MaxOuputValue, robot.NormalizeMin, robot.NormalizeMax);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (TouchSensor) this.MemberwiseClone();
        }

        public float[] Count(RobotEntity robot, Map.Map map)
        {
             //Update position
            if (robot.Middle != RotationMiddle)
            {
                this.MoveTo(robot.Middle);
            }
            if(Orientation != robot.Orientation + OrientationToRobotFPoint)
                this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
            //Count collision 
            if (map.Collision(this, robot))
                return new[] {MaxOuputValue * RescaleOutput + ShiftOutput};
            else
                return new[] {MinOutputValue * RescaleOutput + ShiftOutput}; 
        }

        public void ConnectToRobot(RobotEntity robot)
        {
            //find the location of beginning of touch sensor
            Middle = Entity.RotatePoint(OrientationToRobotFPoint, robot.FPoint, robot.Middle);
            RotationMiddle = robot.Middle;
            FPoint = Middle;
            Orientation = robot.Orientation;
            //Normalize
            RescaleOutput = RescaleInterval(MinOutputValue, MaxOuputValue, robot.NormalizeMin, robot.NormalizeMax);
            ShiftOutput = ShiftInterval(MinOutputValue, MaxOuputValue, robot.NormalizeMin, robot.NormalizeMax);
        }

        public ISensor Clone()
        {
            return (ISensor) this.DeepClone(); 
        }
    }
}