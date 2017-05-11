using System.Data;
using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class LineSensor : LineEntity,ISensor
    {
        //PUBLIC MEMBERS 

        /// <summary>
        /// Dimension of the ouput
        /// </summary>
        public int Dimension { get; protected set;  }
        /// <summary>
        /// Maximum value of intern values 
        /// </summary>
        public float MaxOuputValue { get; protected set; }

        /// <summary>
        /// Minimum value of intern values 
        /// </summary>
        public float MinOutputValue { get; protected set; }
        //PRIVATE MEMBERS 
        /// <summary>
        /// Rescale of the output to the normalize value of robot body 
        /// </summary>
        protected float RescaleOutput;
        /// <summary>
        /// Shift of the output to the normalize value of the robot body
        /// </summary>
        protected float ShiftOutput;

        /// <summary>
        /// Orientation to robot FPoint, adds to the robot orientation to rotate to correct possition 
        /// </summary>
        protected float OrientationToRobotFPoint;
        /// <summary>
        /// Return clone of this LineSensor
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
        public LineSensor(RobotEntity robot,float lenght,float orientation) : base("Line Sensors")
        {
            //Make sensor of give length 
            this.RotationMiddle = robot.Middle;
            this.A = robot.FPoint;
            this.B = Entity.MovePoint(robot.FPoint, Vector2.Normalize(robot.FPoint - robot.Middle) * lenght);
            this.Length = Vector2.Distance(A, B);
            //rotate sensor to its possition
            OrientationToRobotFPoint = orientation;
            this.RotateRadians(orientation+robot.Orientation);

            //OutputSize, returning LengthSqrt 
            MinOutputValue = 0;
            MaxOuputValue = Length * Length;
            Dimension = 1;
            //Normalize output
            RescaleOutput = Entity.RescaleInterval(MinOutputValue, MaxOuputValue, robot.NormalizeMin, robot.NormalizeMax);
            ShiftOutput = Entity.RescaleInterval(MinOutputValue, MaxOuputValue, robot.NormalizeMin, robot.NormalizeMax);
        }


        public virtual float[] Count(RobotEntity robot, Map.Map map)
            {
                //Update possition 
                if(robot.Middle != this.RotationMiddle)
                    this.MoveTo(robot.Middle);
                if(Orientation != robot.Orientation + OrientationToRobotFPoint)
                    this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
                //Count from the map 
            float Distance = map.Collision(this, robot).Distance;
            //Normalize output
            return new  [] {Distance * RescaleOutput + ShiftOutput};
        }

        public void ConnectToRobot(RobotEntity robot)
        {
            //Connect to the middle of the robot
            this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
            this.MoveTo(robot.Middle);
            //Count normalization 
            RescaleOutput = Entity.RescaleInterval(MinOutputValue, MaxOuputValue, robot.NormalizeMin,
                robot.NormalizeMax);
            ShiftOutput = Entity.ShiftInterval(MinOutputValue, MaxOuputValue, robot.NormalizeMin,
                robot.NormalizeMax);
        }

        /// <summary>
        /// Return clone of this sensor
        /// </summary>
        /// <returns></returns>
        public ISensor Clone()
        {
            return (LineSensor) this.DeepClone();
        }

            }
}