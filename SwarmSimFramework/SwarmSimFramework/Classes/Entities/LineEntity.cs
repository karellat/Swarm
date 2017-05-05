using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SwarmSimFramework.Classes.Entities
{
    //Entity represents line segment
    public abstract class LineEntity : Entity
    {
        //CONSTRUCTOR
        protected LineEntity(Vector2 a, Vector2 b, Vector2 rotationMiddle)
        {
            GetShape = Shape.LineSegment;
            throw new NotImplementedException();
        }
        //MEMBERS
        /// <summary>
        /// first of the LineSegment verteces 
        /// </summary>
        public Vector2 A { get; protected set; }
        /// <summary>
        /// second of the LineSegment verteces
        /// </summary>
        public Vector2 B { get; protected set;  }

        /// <summary>
        /// Rotation middle of the segment
        /// </summary>
        protected Vector2 RotationMiddle;
        /// <summary>
        /// Distance between A & B 
        /// </summary>
        public float Length { get; protected set; }
        //METHODS 
        /// <summary>
        /// Return RotationMiddle as rotation middle
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetRotationMiddle()
        {
            return RotationMiddle;
        }
        /// <summary>
        /// Rotate line segment around RotationMiddle for angleInRadians
        /// </summary>
        /// <param name="angleInRadians"></param>
        public override void RotateRadians(float angleInRadians)
        {
            throw new NotImplementedException();
        }
    }
}
