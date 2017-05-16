using System.Numerics;
using System.Runtime.Serialization.Formatters;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Effectors
{
    public class Picker : LineEntity,IEffector
    {
        /// <summary>
        /// Dimension of picker 
        /// </summary>
        public int Dimension { get; }
        /// <summary>
        /// Local bounds 0-1 - put 1-2 - idle 2-3 pick
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// Normalization fnc from brain
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; protected set; }
        /// <summary>
        /// Orientation shift to FPoint of robot
        /// </summary>
        protected float OrientationToRobotFPoint;
        /// <summary>
        /// Creates new picker with given  length & orientation to robot FPoint
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="length"></param>
        /// <param name="orientation"></param>
        public Picker(RobotEntity robot, float length, float orientation) :
            base(robot.FPoint, Entity.MovePoint(robot.FPoint, Vector2.Normalize(robot.FPoint - robot.Middle) * length),
                robot.Middle, "Picker Effector")
        {
            //Rotate sensor to its possition
            OrientationToRobotFPoint = orientation; 
            this.RotateRadians(OrientationToRobotFPoint + robot.Orientation);
            
            //Dimension
            Dimension = 1;
            LocalBounds = new Bounds[1];
            LocalBounds[0] = new Bounds() {Max = 3, Min = 0};
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);


        }
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }


        public void ConnectToRobot(RobotEntity robot)
        {
            //Connect to the middle of the robot
            this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
            this.MoveTo(robot.Middle);
            //Count normalization 
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }

        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            //Update possition 
            if (robot.Middle != this.RotationMiddle)
                this.MoveTo(robot.Middle);
            if (Orientation != robot.Orientation + OrientationToRobotFPoint)
                this.RotateRadians((robot.Orientation + OrientationToRobotFPoint) - Orientation);
            var s = settings.Normalize(NormalizeFuncs)[0];
            //put entity from container
            if (s>= 0 && s < 1)
            {
                //if empty return
                if (robot.ActualContainerCount == 0)
                {
                    robot.InvalidOperationWithContainer++;
                    return; 
                }
                var removingEntity = robot.PeekContainer();
                //Move to the end point of sensor 
                removingEntity.MoveTo(B);
                //Check collision
                if (map.Collision(removingEntity))
                {
                    robot.InvalidOperationWithContainer++;
                    return;
                }
                //Add to map
                else
                {
                    if(removingEntity is FuelEntity)
                        map.FuelEntities.Add((FuelEntity) robot.PopContainer());
                    else
                        map.PasiveEntities.Add(robot.PopContainer());                        
                }
            }
            //pick from map
            else if (s <= 3 && s > 2)
            {
                //Check, if picker collides with anything
                var intersections = map.Collision(this, robot);
                //if no intersection
                if (float.IsPositiveInfinity(intersections.Distance))
                {
                    robot.InvalidOperationWithContainer++;
                    return;
                }
                //Check if entity is Circle & size is suitable for container
                if (intersections.CollidingEntity is CircleEntity &&
                    ((CircleEntity) intersections.CollidingEntity).Radius * 2 < this.Length)
                {
                    if (robot.PushContainer((CircleEntity) intersections.CollidingEntity))
                    {
                        if (intersections.CollidingEntity is FuelEntity)
                            map.FuelEntities.Remove((FuelEntity) intersections.CollidingEntity);
                        else
                            map.PasiveEntities.Remove((CircleEntity) intersections.CollidingEntity); 
                    }
                    else
                    {
                        robot.InvalidOperationWithContainer++;
                        return;
                    }
                }
                else
                {
                    robot.InvalidOperationWithContainer++;
                    return; 
                }

            }
            //idle 
            else
            {
                return;
            }
        }


        public IEffector Clone()
        {
            return (IEffector) DeepClone();
        }
    }
}