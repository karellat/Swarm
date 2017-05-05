using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq.Expressions;
using System.Numerics;
using System.Threading;
using System.Xml;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Map
{
    /// <summary>
    /// provides enviroment for robotic swarm, collision detection, refueling, radio broadcasting, mineral mining
    /// </summary>
    class Map
    {

        //PUBLIC METHODS
        //GLOBAL METHODS 
        /// <summary>
        /// Creates new map with given set up of robots etc
        /// </summary>
        public Map()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Transform map to the inicial set up 
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Make single step of simulation
        /// </summary>
        public void MakeStep()
        {
            throw new NotImplementedException();
        }
        //COLISION AND MOVEMENTS 
        //COLLISION WITH OTHER ROBOTS OR PASSIVE ENTITY 
        /// <summary>
        /// Collision of circle to the newMiddle
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="newMiddle"></param>
        /// <returns></returns>
        public bool Collision(CircleEntity entity, Vector2 newMiddle)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Return true, if given entity collides with something of the enviroment
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Collision(CircleEntity entity)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Collision between Line Entity & enviroment 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Collision(LineEntity entity)
        {
            throw new NotImplementedException();
        }
        //COLISION WITH RADIO BROADCASTING
        /// <summary>
        /// true if colides with any radio signal except given signal
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CollisionRadio(CircleEntity entity)
        {
            throw new NotImplementedException();
        }
        //COLISION WITH FUEL 
        /// <summary>
        /// True if colides with any fuel and refuel entity 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CollisionFuel(RobotEntity entity)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// True if colides with any fuel, do not interact with that 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CollisionFuel(LineEntity entity)
        {
            throw new NotImplementedException();
        }
        //PUBLIC MEMBERS
        public long Cycle { get; protected set; }
        /// <summary>
        /// Stores every actively moving entities on the map 
        /// </summary>
        public List<RobotEntity> Robots;
        /// <summary>
        /// Stores every pasive entity on the map, as minerals, obstacles etc
        /// </summary>
        public List<CircleEntity> PasiveEntities;
        /// <summary>
        /// Stores all radio broadcast in the scope 
        /// </summary>
        public List<RadioEntity> RadionEntities;
        /// <summary>
        /// Stores all fuel entities in the scope 
        /// </summary>
        public List<FuelEntity> FuelEntities;
        //border vertices
        /// <summary>
        /// Upper right corner 
        /// </summary>
        public Vector2 A { get; protected set; }
        /// <summary>
        /// Upper left corner
        /// </summary>
        public Vector2 B { get; protected set; }
        /// <summary>
        ///  Lower left corner
        /// </summary>
        public Vector2 C { get; protected set; }
        /// <summary>
        ///  Lower right corner
        /// </summary>
        public Vector2 D { get; protected set;  }
        //PRIVATE MEMBERS
        // Model of the initicial position 
        /// <summary>
        /// Starting position of robots 
        /// </summary>
        private List<RobotEntity> modelRobotEntities; 
        /// <summary>
        /// Initial position of pasive entities
        /// </summary>
        private List<CircleEntity> modelPasiveEntities;
        /// <summary>
        /// Initial position of fuel
        /// </summary>
        private List<FuelEntity> modelFuelEntitiues;
    }
}
