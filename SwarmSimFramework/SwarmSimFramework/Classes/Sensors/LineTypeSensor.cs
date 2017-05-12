using System.Data;
using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class LineTypeSensor : LineEntity,ISensor
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
        /// Orientation to robot FPoint, adds to the robot orientationToFPoint to rotate to correct possition 
        /// </summary>
        protected float OrientationToFPointToRobotFPoint;
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
        public LineTypeSensor(RobotEntity robot,float lenght,float orientationToFPoint) : base(robot.FPoint, Entity.MovePoint(robot.FPoint, Vector2.Normalize(robot.FPoint - robot.Middle) * lenght),robot.Middle, "Line Sensors")
        {
            //rotate sensor to its possition
            OrientationToFPointToRobotFPoint = orientationToFPoint;
            this.RotateRadians(orientationToFPoint+robot.Orientation);

            //OutputSize, returning LengthSqrt 
            MinOutputValue = 0;
            MaxOuputValue = Length * Length;
            Dimension = 1;
            //TODO: Normalize OTHER TYPE VALUES 
            //Normalize output
            RescaleOutput = Entity.RescaleInterval(MinOutputValue, MaxOuputValue, robot.NormalizeMin, robot.NormalizeMax);
            ShiftOutput = Entity.RescaleInterval(MinOutputValue, MaxOuputValue, robot.NormalizeMin, robot.NormalizeMax);
        }


        public virtual float[] Count(RobotEntity robot, Map.Map map)
            {
                //Update possition 
                if(robot.Middle != this.RotationMiddle)
                    this.MoveTo(robot.Middle);
                if(Orientation != robot.Orientation + OrientationToFPointToRobotFPoint)
                    this.RotateRadians((robot.Orientation + OrientationToFPointToRobotFPoint) - Orientation);
                //Count from the map 
            float Distance = map.Collision(this, robot).Distance;
            
            //TODO: Return type info: 
            //Normalize output
            return new  [] {Distance * RescaleOutput + ShiftOutput};
        }

        public void ConnectToRobot(RobotEntity robot)
        {
            //Connect to the middle of the robot
            this.RotateRadians((robot.Orientation + OrientationToFPointToRobotFPoint) - Orientation);
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
            return (LineTypeSensor) this.DeepClone();
        }

      }
}