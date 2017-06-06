using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public class RawMaterialEntity:CircleEntity
    {
        /// <summary>
        /// Discovered by any robot 
        /// </summary>
        public bool Discovered = false;
        /// <summary>
        /// Amount of cycles to refactor to fuel 
        /// </summary>
        public int CycleToRefactor { get; }
        /// <summary>
        /// Amount fuel created by refactoring 
        /// </summary>
        public float MaterialToRefactor { get; }
        /// <summary>
        /// Create new mineral entity
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="materialToRefactor"></param>
        /// <param name="cycleRefactor"></param>
        public RawMaterialEntity(Vector2 middle, float radius, float materialToRefactor, int cycleRefactor) : base(middle,radius,"Mineral Entity")
        {
            Color = EntityColor.RawMaterialColor;
            CycleToRefactor = cycleRefactor;
            MaterialToRefactor = materialToRefactor;

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