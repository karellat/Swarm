using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SwarmSimFramework.Classes.Entities
{
    public abstract class CircleEntity : Entity
    {
        //CONSTRUCTOR
        protected CircleEntity(Vector2 middle, float radius,float orientation=0)
        {
            Middle = middle;
            Radius = radius;
            Name = "CircleEntity";
            if (orientation != 0)
                throw new  NotImplementedException();
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
        public Vector2 fPoint { get; protected set; }
        /// <summary>
        ///  Actual orientation in radians
        /// </summary>
        public float Orientation { get; protected set; }
        
        //METHODS 
        /// <summary>
        /// Rotate fPoint around Middle
        /// </summary>
        /// <param name="angleInRadians"></param>
        public override void RotateRadians(float angleInRadians)
        {
            throw new NotImplementedException();
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
