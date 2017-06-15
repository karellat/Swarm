using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SwarmSimFramework.Classes.Entities
{
    /// <summary>
    /// Represent Circle obstacle 
    /// </summary>
   public  class ObstacleEntity : CircleEntity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        public ObstacleEntity(Vector2 middle,float radius) : base(middle,radius,"Obstacle Entity")
        {
            Color = EntityColor.ObstacleColor;
            
        }
        /// <summary>
        /// Create a clone of entity
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
            
        }
    }
}
