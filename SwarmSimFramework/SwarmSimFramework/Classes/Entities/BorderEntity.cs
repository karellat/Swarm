using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class BorderEntity : LineEntity
    {
        public BorderEntity(Vector2 a, Vector2 b, Vector2 rotationMiddle) : base(a, b, rotationMiddle)
        {
        }

        public override Entity DeepClone()
        {
            throw new System.NotImplementedException();
        }
    }
}