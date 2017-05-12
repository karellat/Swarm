using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class TypeCircleSensor : CircleEntity,ISensor
    {
        //TODO: Implement

        public TypeCircleSensor(string name) : base(name)
        {
        }

        public TypeCircleSensor(Vector2 middle, float radius, string name, float orientation = 0) : base(middle, radius, name, orientation)
        {
        }

        public TypeCircleSensor(Vector2 middle, float radius, string name, Vector2 rotationMiddle, float orientation = 0) : base(middle, radius, name, rotationMiddle, orientation)
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