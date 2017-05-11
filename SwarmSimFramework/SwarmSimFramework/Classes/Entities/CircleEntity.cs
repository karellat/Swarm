using System.Numerics;

namespace SwarmSimFramework.Classes.Entities
{
    public abstract class CircleEntity : Entity
    {
        //CONSTRUCTOR
        /// <summary>
        /// Presumes manual inicialization 
        /// </summary>
        /// <param name="name"></param>
        protected CircleEntity(string name) : this(Vector2.Zero, 0, name)
        {
            
        }
        protected CircleEntity(Vector2 middle, float radius,string name, float orientation=0)
        {
            Middle = middle;
            Radius = radius;
            Name = name;
            GetShape = Shape.Circle;
            Orientation = orientation;
            //Make front point pointing to the top of map 
            FPoint = new Vector2(middle.X,Middle.Y+Radius);
            //If no initial rotation 
            if (orientation != 0)
            {
                FPoint = RotatePoint(orientation, FPoint, Middle);
            }
        }

        //MEMBERS 
        /// <summary>
        /// Middle of the circle
        /// </summary>
        public Vector2 Middle { get; protected set; }
        /// <summary>
        /// Radius of the entity
        /// </summary>
        public float Radius { get; protected set; }
        /// <summary>
        /// Head of circle
        /// </summary>
        public Vector2 FPoint { get; protected set; }
        
        //METHODS 
        /// <summary>
        /// Rotate fPoint around Middle
        /// </summary>
        /// <param name="angleInRadians"></param>
        public override void RotateRadians(float angleInRadians)
        {
            //Rotate pointing point
            if (GetRotationMiddle() != Middle)
                Middle = RotatePoint(angleInRadians, Middle, GetRotationMiddle());
            FPoint = RotatePoint(angleInRadians, FPoint, GetRotationMiddle());
            Orientation += angleInRadians;
            while (Orientation < 0)
                Orientation += Pi2;
            Orientation = Orientation % Pi2;
        }
        /// <summary>
        /// Move middle and FPoint to newMiddle 
        /// </summary>
        /// <param name="newMiddle"></param>
        public override void MoveTo(Vector2 newMiddle)
        {
            Vector2 shift = newMiddle - GetRotationMiddle(); 
            Middle = MovePoint(Middle, shift);
            FPoint = MovePoint(FPoint, shift);
        }

        /// <summary>
        /// Move for given distance in the direction of FPoint
        /// </summary>
        /// <param name="distance"></param>
        public void MoveTo(float distance)
        {
            Vector2 shift = FPoint - Middle; 
            shift = Vector2.Normalize(shift);
            shift *= distance;

            Middle = MovePoint(Middle, shift);
            FPoint = MovePoint(FPoint, shift);
        }
        /// <summary>
        /// Return Middle as rotation middle
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetRotationMiddle()
        {
            return Middle;
        }
    }
}
