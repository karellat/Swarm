using System;
using System.Numerics;
using Newtonsoft.Json;

namespace SwarmSimFramework.Classes.Entities
{
    public class FuelEntity : CircleEntity
    {
        public static float FuelRadius = 5.0f;
        /// <summary>
        /// True if the tank was consumed
        /// </summary>
        [JsonProperty]
        public bool Empty { get; protected set; }
        /// <summary>
        /// Return the amount of fuel in this tank
        /// </summary>
        [JsonProperty]
        public float Capacity { get; protected set; }
        /// <summary>
        /// Give clone of this fuel tank
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) MemberwiseClone();
        }
        /// <summary>
        /// Creates new tank with given capacity
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="name"></param>
        /// <param name="capacity"></param>
        /// <param name="orientation"></param>
        public FuelEntity(Vector2 middle, float radius, float capacity, float orientation = 0) : base(middle, radius, "FuelEntity", orientation)
        {
            Color = EntityColor.FuelColor;
            Capacity = capacity;
        }
        /// <summary>
        /// Json constructor
        /// </summary>
        [JsonConstructor]
        protected FuelEntity() : base("FuelEntity")
        {
            
        }
        /// <summary>
        /// Empty this fuel tank and return amount of fuel 
        ///  </summary>
        /// <returns></returns>
        public float Suck()
        {
            Empty = true;
            Capacity = 0;
            return Capacity;
        }
    }
}