using SwarmSimFramework.Classes.Entities;
using System.Numerics;

namespace SwarmSimFramework.Classes.Map
{
    /// <summary>
    /// Represent Intersection with the Entity 
    /// </summary>
    public struct Intersection
    {
        public Vector2 IntersectionPoint;
        public Entity CollidingEntity;
        public float Distance; 
    }

    /// <summary>
    /// Represent intersections with radio entities 
    /// </summary>
    public class RadioIntersection
    {
        public Vector2 SumOfDirections;
        public int ValueOfSignal { get; }
        public int AmountOfSignal;

        public RadioIntersection(int valueOfSignal)
        {
            ValueOfSignal = valueOfSignal;
            AmountOfSignal = 0;
            SumOfDirections = Vector2.Zero;
        }

        public Vector2 MeanDirection()
        {
            if(AmountOfSignal == 0)
                return Vector2.Zero;
            return new Vector2(SumOfDirections.X / AmountOfSignal, SumOfDirections.Y / AmountOfSignal);
        }
    }
    /// <summary>
    /// Represent intersection with colors
    /// </summary>
    public class ColorIntersection
    {
        public Entity.EntityColor Color;
        public int AmountOfSignal;

        public ColorIntersection(Entity.EntityColor color)
        {
            Color = color;
            AmountOfSignal = 1; 
        }
    }
}