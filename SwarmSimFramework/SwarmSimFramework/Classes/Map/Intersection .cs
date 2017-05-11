using SwarmSimFramework.Classes.Entities;
using System.Numerics;

namespace SwarmSimFramework.Classes.Map
{
    public struct Intersection
    {
        public Vector2 IntersectionPoint;
        public Entity CollidingEntity;
        public float Distance; 
    }
}