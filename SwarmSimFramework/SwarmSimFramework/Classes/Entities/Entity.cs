using System;
using System.Numerics;

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
            LineSegment,
            Abstract
        }
        /// <summary>
        /// Name of entity
        /// </summary>
        public string Name { get; protected set; } = "Entity";
        /// <summary>
        /// Return Shape of entity
        /// </summary>
        public Shape GetShape { get; protected set; }
        /// <summary>
        ///  Actual orientation in radians
        /// </summary>
        public float Orientation { get; protected set; }
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
        /// Move to the new possition
        /// </summary>
        /// <param name="newMiddle"></param>
        public abstract void MoveTo(Vector2 newMiddle);
        /// <summary>
        /// Convert degrees into radians and call virtual method on Entity
        /// </summary>
        /// <param name="angleInDegrees"></param>
        public virtual void RotateDegrees(float angleInDegrees)
        {
            RotateRadians(DegreesToRadians(angleInDegrees));
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
        /// <summary>
        /// Convert angle in deradians to degrees 
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static float DegreesToRadians(float degree)
        {
            degree = degree % 360.0f;
            return (degree * (float) Math.PI) / 180.0f;
        }
        /// <summary>
        /// Return rotated point around rotationMiddle for angleRadians
        /// </summary>
        /// <param name="angleRadians"></param>
        /// <param name="point"></param>
        /// <param name="rotationMiddle"></param>
        /// <returns></returns>
        public static Vector2 RotatePoint(float angleRadians, Vector2 point, Vector2 rotationMiddle)
        {
            return Vector2.Transform(point,Matrix3x2.CreateRotation(angleRadians,rotationMiddle));
        }
        /// <summary>
        /// Move point to  new location
        /// </summary>
        /// <param name="fromPoint"></param>
        /// <param name="toPoint"></param>
        /// <param name="movingPoint"></param>
        /// <returns></returns>
        public static Vector2 MovePoint(Vector2 fromPoint, Vector2 toPoint, Vector2 movingPoint)
        {
            return Vector2.Transform(movingPoint,Matrix3x2.CreateTranslation(toPoint - fromPoint));
        }
        /// <summary>
        /// Move point for vector 
        /// </summary>
        /// <param name="movingPoint"></param>
        /// <param name="shiftVector"></param>
        /// <returns></returns>
        public static Vector2 MovePoint(Vector2 movingPoint, Vector2 shiftVector)
        {
            return Vector2.Transform(movingPoint,Matrix3x2.CreateTranslation(shiftVector));
        }
        public const float Pi2 = 2 * (float)Math.PI;
    }
}
