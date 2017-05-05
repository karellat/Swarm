using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SwarmSimFramework.Classes.Entities
{
    public abstract class Entity
    {
        //MEMBERS 
        /// <summary>
        /// Enumarate of entity shape 
        /// </summary>
        public enum Shape
        {
            Circle,
            Line, 
            LineSegment
        }
        /// <summary>
        /// Name of entity
        /// </summary>
        public string Name { get; protected set; } = "Entity";
        /// <summary>
        /// Return Shape of entity
        /// </summary>
        public Shape GetShape { get; protected set; }
        //METHODS
        /// <summary>
        /// Return clone of actual entity
        /// </summary>
        /// <returns></returns>
        public abstract Entity DeepClone();
        /// <summary>
        /// Return rotation middle
        /// </summary>
        /// <returns></returns>
        public abstract Vector2 GetRotationMiddle();
        /// <summary>
        /// Rotate this for angleInRadians around rotation middle 
        /// </summary>
        /// <param name="angleInRadians"></param>
        public abstract void RotateRadians(float angleInRadians);
        /// <summary>
        /// Convert degrees into radians and call virtual method on Entity
        /// </summary>
        /// <param name="angleInDegrees"></param>
        public virtual void RotateDegrees(float angleInDegrees)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Rotate point arounf rotationMiddle
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angleInRadians"></param>
        /// <param name="rotationMiddle"></param>
        /// <returns></returns>
        public static Vector2 MakeRotation(Vector2 point, float angleInRadians, Vector2 rotationMiddle)
        {
            throw new NotImplementedException();
        }
    }
}
