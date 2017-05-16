using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class MineralEntity:CircleEntity
    {
        /// <summary>
        /// Amount of cycles to refactor to fuel 
        /// </summary>
        public int CycleToRefactor { get; }
        /// <summary>
        /// Amount fuel created by refactoring 
        /// </summary>
        public float FuelToRefactor { get; }
        /// <summary>
        /// Create new mineral entity
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="fuelToRefactor"></param>
        /// <param name="cycleRefactor"></param>
        public MineralEntity(Vector2 middle, float radius, float fuelToRefactor, int cycleRefactor) : base(middle,radius,"Mineral Entity")
        {
            CycleToRefactor = cycleRefactor;
            FuelToRefactor = fuelToRefactor;

        }
        /// <summary>
        /// Make Clone of Mineral Entity 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
           return (Entity) this.MemberwiseClone();
        }
    }
}