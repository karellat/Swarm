using System.Net;
using System.Numerics;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Effectors
{
    /// <summary>
    /// Implementation of two wheel motor: 
    /// Model:  http://rossum.sourceforge.net/papers/DiffSteer/DiffSteer.html
    /// </summary>
    public class TwoWheelMotor : CircleEntity, IEffector
    {
        /// <summary>
        /// maximal change of speed 
        /// </summary>
        protected static float MaxVelocityChange = 0.5f;
        /// <summary>
        /// Dimension of effector
        /// </summary>
        public int Dimension { get; }
        /// <summary>
        /// Intern values
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// Normalization fnc from robot bounds to local 
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }

        /// <summary>
        /// Last speed of right wheel 
        /// </summary>
        protected float RightVelocity;
        /// <summary>
        /// Last speed of left wheel
        /// </summary>
        protected float LeftVelocity;
        /// <summary>
        /// Distance between wheels
        /// </summary>
        protected float WheelDistance; 

        /// <summary>
        /// TwoWheelMotor constructor with same give +- speed
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="maxSpeed"></param>
        public TwoWheelMotor(RobotEntity robot, float maxSpeed) : base(robot.Middle, robot.Radius, "TwoWheelMotor",
            robot.Orientation)
        {
            //Intern values
            WheelDistance = 2 * Radius;
            Dimension = 2;
            LocalBounds = new Bounds[2];
            LocalBounds[0] = new Bounds() {Max = maxSpeed, Min = -maxSpeed};
            LocalBounds[1] = new Bounds() {Max = maxSpeed, Min = -maxSpeed};
            //Create normalization func from reading 
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);
            
            //Set initial velocities
            RightVelocity = 0;
            LeftVelocity = 0;
        }

        /// <summary>
        /// Reset position and normalization func
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
            //Reset position 
            Middle = robot.Middle;
            FPoint = robot.FPoint;
            Orientation = robot.Orientation;
            WheelDistance = 2 * robot.Radius;
            //Create normalization funcs
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);
            //reset Current values 
            RightVelocity = 0;
            LeftVelocity = 0;
        }

        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            throw new System.NotImplementedException();
        }

        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }

        public IEffector Clone()
        {
            return (IEffector) DeepClone();
        }
    }
}