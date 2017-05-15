using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    /// <summary>
    /// Return amount of types in radius: 
    /// </summary>
    public class TypeCircleSensor : CircleEntity, ISensor
    {
        /// <summary>
        /// Dimension of TypeSensor 
        /// </summary>
        public int Dimension { get; }
        /// <summary>
        /// 
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// 
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; }
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="name"></param>
        /// <param name="rotationMiddle"></param>
        /// <param name="orientation"></param>
        public TypeCircleSensor(RobotEntity robot,float radius) : base(robot.Middle, radius, "Type Sensor")
        {

        }

        public override Entity DeepClone()
        {
            throw new System.NotImplementedException();
        }

        public float[] Count(RobotEntity robot, Map.Map map)
        {
            throw new System.NotImplementedException();
        }

        public void ConnectToRobot(RobotEntity robot)
        {
            throw new System.NotImplementedException();
        }


        public ISensor Clone()
        {
            throw new System.NotImplementedException();
        }
    }

}