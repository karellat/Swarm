using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    //TODO: Implement
    public class LineDistanceSensor : LineEntity,ISensor
    {
        public LineDistanceSensor(Vector2 a, Vector2 b, Vector2 rotationMiddle, string name, float orientation = 0) : base(a, b, rotationMiddle, name, orientation)
        {
        }

        public LineDistanceSensor(string name) : base(name)
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

        public int Dimension { get; }
        public float MaxOuputValue { get; }
        public float MinOutputValue { get; }
        public ISensor Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}