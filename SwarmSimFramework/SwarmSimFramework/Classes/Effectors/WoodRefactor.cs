using System.Numerics;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Effectors
{
    /// <summary>
    /// Two state 
    /// </summary>
    public class WoodRefactor: LineEntity, IEffector

    {
        public WoodRefactor(Vector2 a, Vector2 b, Vector2 rotationMiddle, string name, float orientation = 0) : base(a, b, rotationMiddle, name, orientation)
        {
        }

        public WoodRefactor(string name) : base(name)
        {
        }

        public override Entity DeepClone()
        {
            throw new System.NotImplementedException();
        }

        public int Dimension { get; }
        public void ConnectToRobot(RobotEntity robot)
        {
            throw new System.NotImplementedException();
        }

        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            throw new System.NotImplementedException();
        }

        public Bounds[] LocalBounds { get; }
        public NormalizeFunc[] NormalizeFuncs { get; }
        public IEffector Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}