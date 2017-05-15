using System.Collections.Generic;
using System.Numerics;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    /// <summary>
    /// Return amount of types in radius: MineralEntities, ObstacleEntities, FuelEntities, 
    /// </summary>
    public class TypeCircleSensor : CircleEntity, ISensor
    {
        /// <summary>
        /// Dimension of TypeSensor 
        /// </summary>
        public int Dimension { get; }
        /// <summary>
        /// Local intern values 
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// Normalization fncs 
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; protected set;  }
        /// <summary>
        /// Create new type sensor
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="name"></param>
        /// <param name="rotationMiddle"></param>
        /// <param name="orientation"></param>
        public TypeCircleSensor(RobotEntity robot,float radius) : base(robot.Middle, radius, "Type Sensor")
        {
            //No need for rotation 
            FPoint = Middle;
            Dimension = 3;
            //set bounds of local values
            LocalBounds = new Bounds[3];
            LocalBounds[0] = new Bounds() {Min = 0, Max = 100};
            LocalBounds[1] = new Bounds() {Min = 0, Max = 100};
            LocalBounds[2] = new Bounds() {Min = 0, Max = 100};
             
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);

        }
        /// <summary>
        /// Count types in surrounding 
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public float[] Count(RobotEntity robot, Map.Map map)
        {
            if (robot.Middle != Middle)
            {
                Middle = robot.Middle;
                FPoint = Middle;

            }
            float[] o = new float[Dimension];

            Dictionary<EntityColor, ColorIntersection> dic = map.CollisionColor(this);

            o[0] = dic.ContainsKey(EntityColor.MineralColor) ? dic[EntityColor.MineralColor].Amount : 0;
            o[1] = dic.ContainsKey(EntityColor.ObstacleColor) ? dic[EntityColor.ObstacleColor].Amount : 0;
            o[2] = dic.ContainsKey(EntityColor.FuelColor) ? dic[EntityColor.FuelColor].Amount : 0;

            return o.Normalize(NormalizeFuncs);
        }
        /// <summary>
        /// Make clone of Sensor
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }
        /// <summary>
        /// Create normalization fncs for given robot
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
            NormalizeFuncs = MakeNormalizeFuncs(LocalBounds, robot.NormalizedBound);
        }
        /// <summary>
        /// Return clone sensor 
        /// </summary>
        /// <returns></returns>
        public ISensor Clone()
        {
            return (ISensor) DeepClone();
        }
    }

}