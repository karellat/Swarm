using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class RadioSensor : CircleEntity,ISensor
    {
        //TODO: Implement 
        public RadioSensor(string name) : base(name)
        {
        }

        public RadioSensor(Vector2 middle, float radius, string name, float orientation = 0) : base(middle, radius, name, orientation)
        {
        }

        public RadioSensor(Vector2 middle, float radius, string name, Vector2 rotationMiddle, float orientation = 0) : base(middle, radius, name, rotationMiddle, orientation)
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