using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class LineSensor : LineEntity,ISensor
    {
        public LineSensor(Vector2 a, Vector2 b, Vector2 rotationMiddle) : base(a, b, rotationMiddle)
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

        public int Dimension { get; }
        public float MaxOuputValue { get; }
        public float MinOutputValue { get; }
    }
}