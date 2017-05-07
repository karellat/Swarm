using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class BorderEntity : LineEntity
    {


        public override Entity DeepClone()
        {
            throw new System.NotImplementedException();
        }

        public BorderEntity(Vector2 a, Vector2 b, Vector2 rotationMiddle, string name, float orientation = 0) : base(a, b, rotationMiddle, name, orientation)
        {
        }
    }
}