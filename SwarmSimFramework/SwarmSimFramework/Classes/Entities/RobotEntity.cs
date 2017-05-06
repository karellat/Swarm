using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SwarmSimFramework.Interfaces;

namespace SwarmSimFramework.Classes.Entities
{
    public abstract class RobotEntity :CircleEntity
    {
        //MEMBERS
        /// <summary>
        /// Effectors of robot
        /// </summary>
        public IEffector[] Effectors;
        /// <summary>
        /// Sensors of robot
        /// </summary>
        public ISensor[] Sensors;
        /// <summary>
        /// Robot brains decide setting of effectors based on read sensors 
        /// </summary>
        public IRobotBrain Brain;
        /// <summary>
        /// Amount of fuel
        /// </summary>
        public float FuelAmount;
        /// <summary>
        /// Detected collision from last reset
        /// </summary>
        public long CollisionDetected;
        /// <summary>
        /// Starting point of robot
        /// </summary>
        public Vector2 StartingPoint;
        /// <summary>
        /// Sensor dimension
        /// </summary>
        public int SensorDimension { get; protected set; }
        /// <summary>
        /// Effector dimension 
        /// </summary>
        public int EffectorDimension { get; protected set;  }

        //CONSTRUCTOR 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="orientation"></param>
        protected RobotEntity(Vector2 middle, float radius, float orientation = 0) : base(middle, radius, orientation)
        {
           
        }
        /// <summary>
        /// Prepare for movement
        /// 1. read sensors
        /// 2. let brain decide effectors setting
        /// </summary>
        public void PrepareMove(Map.Map map)
        {
            throw new NotImplementedException();
        }
        //METHODS 
        /// <summary>
        /// Make movement 
        /// 1. Give it to the effectors 
        /// 2. Burn fuel
        /// </summary>
        /// <param name="map"></param>
        public void Move(Map.Map map)
        {
            
        }
        /// <summary>
        /// Reset calculators
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Deep clone of Robot entity 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            throw new NotImplementedException();
        }
    }
}
