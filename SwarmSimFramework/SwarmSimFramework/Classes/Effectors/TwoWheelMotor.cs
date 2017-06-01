using System;
using System.Data;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading;
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
        public static float MaxVelocityChange = 1.0f;
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
            RotationMiddle = robot.Middle;
            Orientation = robot.Orientation;
            WheelDistance = 2 * robot.Radius;
            //Create normalization funcs
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);
            //reset Current values 
            RightVelocity = 0;
            LeftVelocity = 0;
        }

        /// <summary>
        /// Make effect on Robot and the enviroment
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            lastSettings = settings;
            //Check position, if robot on different possition make connect
            if (this.Middle != robot.Middle)
                ConnectToRobot(robot);
            float[] normalizeSettings = settings.Normalize(NormalizeFuncs);
            //Set speeds
            //Check speed change bound 
            RightVelocity = (Math.Abs(RightVelocity - normalizeSettings[0])) < MaxVelocityChange
                ? normalizeSettings[0]
                : Math.Sign(normalizeSettings[0] - RightVelocity) * MaxVelocityChange + RightVelocity;
            LeftVelocity = (Math.Abs(LeftVelocity - normalizeSettings[1])) < MaxVelocityChange
                ? normalizeSettings[1]
                : Math.Sign(settings[1] - LeftVelocity) * MaxVelocityChange + LeftVelocity;
            // Count, speed, rotation and make move if not collide
            float s = (RightVelocity + LeftVelocity) / 2.0f;
            float o = ((RightVelocity - LeftVelocity) / WheelDistance);
            robot.RotateRadians(o);
            this.RotateRadians(o);
            Vector2 newPos = new Vector2(s * (float) Math.Sin(Pi2 - Orientation) + Middle.X,
                s * (float) Math.Cos(Pi2 - Orientation) + Middle.Y);
        
        //Make move if not collide, if colide mark collision
        if (!map.Collision(robot, newPos))
           {
               robot.MoveTo(newPos);
               this.MoveTo(newPos);
           }
           else
           {
               robot.CollisionDetected++;
           }
       }
        /// <summary>
        /// Make a clone of effector entity 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }
        /// <summary>
        /// Make a clone of effector 
        /// </summary>
        /// <returns></returns>
        public IEffector Clone()
        {
            return (IEffector) DeepClone();
        }
        /// <summary>
        /// Last settings 
        /// </summary>
        public float[] lastSettings = new float[2];
        /// <summary>
        /// Log two wheel motor 
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s = new StringBuilder("TwoWheelMotor: ");
            s.AppendLine("\t " + base.Log());
            s.AppendLine("Right speed: " + RightVelocity + " Left speed: " + LeftVelocity + " WheelDistance: " + WheelDistance);
            s.AppendLine("RightSettings: " + lastSettings[0] + " LeftSettings: " + lastSettings[1]);
            return s;
        }
    }
}