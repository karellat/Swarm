using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    //Entity represents line segment
    public abstract class LineEntity : Entity
    {
        //CONSTRUCTOR

        protected LineEntity(Vector2 a, Vector2 b, Vector2 rotationMiddle, string name, float orientation = 0)
        {
            Name = name; 
            GetShape = Shape.LineSegment;
            A = a;
            B = b;
            RotationMiddle = rotationMiddle;
            Orientation = orientation;
            Length = Vector2.Distance(A, B);
        }
        protected LineEntity(string name) :this(Vector2.Zero, Vector2.Zero, Vector2.Zero,name)
        { }
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
            A = RotatePoint(angleInRadians, A, RotationMiddle);
            B = RotatePoint(angleInRadians, B, RotationMiddle);
            Orientation += angleInRadians;
           while (Orientation < 0)
                Orientation += Pi2;
            Orientation = Orientation % Pi2;
        }
        /// <summary>
        /// Change rotation middle to the new possition
        /// </summary>
        /// <param name="newMiddle"></param>
        public override void MoveTo(Vector2 newMiddle)
        {
            Vector2 shiftVector = newMiddle - RotationMiddle;
            A = MovePoint(A, shiftVector);
            B = MovePoint(B, shiftVector);
            RotationMiddle = newMiddle;
        }
    }
}
