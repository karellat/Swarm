using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Map
{
    /// <summary>
    /// provides enviroment for robotic swarm, collision detection, refueling, radio broadcasting, mineral mining
    /// </summary>
    public class Map
    {

        //PUBLIC METHODS
        //GLOBAL METHODS 
        /// <summary>
        /// Creates new map with given set up of robots etc
        /// </summary>
        public Map(float height,float width,List<RobotEntity> robots,List<CircleEntity> pasiveEntities,List<FuelEntity> fuelEntities)
        {
            //Init characteristics of map 
            this.MaxHeight = height;
            this.MaxWidth = width;
            Cycle = 0;
            //Set border points
            A = new Vector2(0,0);
            B = new Vector2(MaxWidth,0);
            C = new Vector2(0,MaxHeight);
            D = new Vector2(MaxWidth,MaxHeight);

            //Copy initial set up 
            Robots = robots;
            PasiveEntities = pasiveEntities;
            FuelEntities = fuelEntities;
            //No radio signals in the begining 
            RadionEntities = new List<RadioEntity>();
            //Mark down initial set up 
            foreach (var r in robots)
            {
                modelRobotEntities.Add((RobotEntity)r.DeepClone());
            }
            foreach (var p in pasiveEntities)
            {
                modelPasiveEntities.Add((CircleEntity) p.DeepClone());
            }
            foreach (var f in fuelEntities)
            {
                modelFuelEntities.Add((FuelEntity)f.DeepClone());
            }
        }
        /// <summary>
        /// Transform map to the inicial set up 
        /// </summary>
        public void Reset()
        {
            //Borders vertices remain same 
            Cycle = 0; 
            //Clear old bodies
            Robots.Clear();
            RadionEntities.Clear();
            FuelEntities.Clear();
            //Copy models from set up lists
            foreach (var r in modelRobotEntities)
            {
                Robots.Add((RobotEntity)r.DeepClone());
            }
            foreach (var f in modelFuelEntities)
            {
                FuelEntities.Add(f);
            }
            //No point of coping passive entities 
        }
        /// <summary>
        /// Make single step of simulation
        /// </summary>
        public void MakeStep()
        {
            //Make all robots read situation & decide, no point of random iteration 
            foreach (var r in Robots)
            {
                r.PrepareMove(this);
            }
            //Clean signals from map 
            RadionEntities.Clear();
            //Random iteration through list of robot
            //Make movent, activate effectors
            foreach (int i in Enumerable.Range(0,Robots.Count).OrderBy(x => RandomNumber.GetRandomInt()))
                Robots[i].Move(this);
            //Cycle change 
            Cycle++; 
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
        public Vector2 Collision(LineEntity entity)
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
        public Vector2 CollisionFuel(LineEntity entity)
        {
            throw new NotImplementedException();
        }
        //PUBLIC MEMBERS
        //Characteristcs of map 
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
        public Vector2 D { get; protected set; }
        /// <summary>
        /// Cycle simulation
        /// </summary>
        public long Cycle { get; protected set; }
        /// <summary>
        /// Maximum height of map
        /// </summary>
        public float MaxHeight { get; protected set; }
        /// <summary>
        /// Maximum width of map 
        /// </summary>
        public float MaxWidth { get; protected set;  }
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
        private List<FuelEntity> modelFuelEntities;
    }
}
