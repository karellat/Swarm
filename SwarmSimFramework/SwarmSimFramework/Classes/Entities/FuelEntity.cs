using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class FuelEntity : CircleEntity
    {
        public bool Empty { get; protected set; }
        public FuelEntity(Vector2 middle, float radius, float orientation = 0) : base(middle, radius, orientation)
        {
        }

        public override Entity DeepClone()
        {
            throw new System.NotImplementedException();
        }
    }
}