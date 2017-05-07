using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class FuelEntity : CircleEntity
    {
        public bool Empty { get; protected set; }


        public override Entity DeepClone()
        {
            throw new System.NotImplementedException();
        }

        public FuelEntity(Vector2 middle, float radius, string name, float orientation = 0) : base(middle, radius, name, orientation)
        {
        }
    }
}