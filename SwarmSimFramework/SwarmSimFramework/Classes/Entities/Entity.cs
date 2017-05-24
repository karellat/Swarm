using System;
using System.Numerics;
using System.Text;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{

    public abstract class Entity
    {
        //Enumerate entity type 
        public enum EntityColor
        {
            ObstacleColor,
            RawMaterialColor, 
            FuelColor, 
            RobotColor,
            WoodColor,
        }

        public static int EntityColorCount = Enum.GetNames(typeof(EntityColor)).Length;

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

        /// <summary>
        /// Middle of rotation 
        /// </summary>
        public Vector2 RotationMiddle { get; protected set; }

        /// <summary>
        /// Get color of entity
        /// </summary>
        public EntityColor Color { get; protected set; }

        //METHODS
        /// <summary>
        /// Return clone of actual entity
        /// </summary>
        /// <returns></returns>
        public abstract Entity DeepClone();

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
        /// Log current entity, info about position, state
        /// </summary>
        /// <returns></returns>
        public abstract StringBuilder Log();

        //STATIC function
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
            return Vector2.Transform(point, Matrix3x2.CreateRotation(angleRadians, rotationMiddle));
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
            return Vector2.Transform(movingPoint, Matrix3x2.CreateTranslation(toPoint - fromPoint));
        }

        /// <summary>
        /// Move point for vector 
        /// </summary>
        /// <param name="movingPoint"></param>
        /// <param name="shiftVector"></param>
        /// <returns></returns>
        public static Vector2 MovePoint(Vector2 movingPoint, Vector2 shiftVector)
        {
            return Vector2.Transform(movingPoint, Matrix3x2.CreateTranslation(shiftVector));
        }

        public const float Pi2 = 2 * (float) Math.PI;

        /// <summary>
        /// Make normalization from one interval to another
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static NormalizeFunc MakeNormalizationFunc(Bounds from, Bounds to)
        {
            float sizeFrom = Math.Abs(from.Max - from.Min);
            float sizeTo = Math.Abs(to.Max - to.Min);
            float rescale = (sizeTo / sizeFrom);
            float min = from.Min * rescale;
            float shift = to.Min - min;
            return new NormalizeFunc() {Rescale = rescale, Shift = shift};
        }

        /// <summary>
        /// Return normalization functions for given localBounds to outputBounds 
        /// </summary>
        /// <param name="localBounds"></param>
        /// <param name="outputBounds"></param>
        public static NormalizeFunc[] MakeNormalizeFuncs(Bounds[] localBounds, Bounds outputBounds)
        {
            var output = new NormalizeFunc[localBounds.Length];
            for (int i = 0; i < localBounds.Length; i++)
            {
                output[i] = MakeNormalizationFunc(localBounds[i], outputBounds);
            }
            return output;
        }
        /// <summary>
        /// Make normalization functions from normalized values to localBounds
        /// </summary>
        /// <param name="fromBounds"></param>
        /// <param name="toBounds"></param>
        /// <returns></returns>
        public static NormalizeFunc[] MakeNormalizeFuncs(Bounds fromBounds, Bounds[] toBounds)
        {
            var output = new NormalizeFunc[toBounds.Length];
            for (int i = 0; i < toBounds.Length; i++)
            {
                output[i] = MakeNormalizationFunc(fromBounds, toBounds[i]);
            }
            return output;
        }

        /// <summary>
        /// Return Vector in given interval 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 KeepInBounds(Bounds b, Vector2 v)
        {
            if (v.X > b.Max)
                v.X = b.Max;
            if (v.X < b.Min)
                v.X = b.Min;
            if (v.Y > b.Max)
                v.Y = b.Max;
            if (v.Y < b.Min)
                v.Y = b.Min;
            return v;
        }

       
    }
}

