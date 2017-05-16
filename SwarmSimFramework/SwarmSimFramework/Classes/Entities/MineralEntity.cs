using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class MineralEntity:CircleEntity
    {
        public int CycleToRefactor { get; }
        public float FuelToRefactor { get; }
        public MineralEntity(string name) : base(name)
        {
        }

        public MineralEntity(Vector2 middle, float radius, string name, float orientation = 0) : base(middle, radius, name, orientation)
        {
        }

        public MineralEntity(Vector2 middle, float radius, string name, Vector2 rotationMiddle, float orientation = 0) : base(middle, radius, name, rotationMiddle, orientation)
        {
        }

        public override Entity DeepClone()
        {
            throw new System.NotImplementedException();
        }
    }
}