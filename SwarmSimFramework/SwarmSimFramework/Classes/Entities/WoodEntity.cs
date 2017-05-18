using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    /// <summary>
    /// WoodEntity
    /// </summary>
    public class WoodEntity : CircleEntity
    {
        /// <summary>
        /// Create new wood entity with given amount
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="amountOfWood"></param>
        public WoodEntity(Vector2 middle, float radius, float amountOfWood) : base(middle, radius,"Wood entity" , middle, 0)
        {
            AmountOfWood = amountOfWood;
        }
        /// <summary>
        /// Get actual amount of wood 
        /// </summary>
        public float AmountOfWood { get; protected set; }
        /// <summary>
        /// Return deep clone of wood entity 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }
    }
}