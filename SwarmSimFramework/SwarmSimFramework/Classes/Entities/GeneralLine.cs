using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class GeneralLine : LineEntity
    {
       
        public GeneralLine(Vector2 a, Vector2 b, string name ="GeneralLine", float orientation = 0) : base(a, b, Vector2.Zero, name, orientation)
        {
        }

        public override Entity DeepClone()
        {
            throw new System.NotImplementedException();
        }
    }
}