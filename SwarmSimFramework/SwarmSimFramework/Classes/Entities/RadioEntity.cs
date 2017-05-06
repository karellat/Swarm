using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SwarmSimFramework.Classes.Entities
{
    public class RadioEntity : CircleEntity
    {
        public RadioEntity(Vector2 middle, float radius, float orientation = 0) : base(middle, radius, orientation)
        {
        }

        public override Entity DeepClone()
        {
            throw new NotImplementedException();
        }
    }
}
