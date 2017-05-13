using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Intersection2D;
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
            MaxHeight = height;
            MaxWidth = width;
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
            modelRobotEntities = new List<RobotEntity>(Robots.Count);
            modelFuelEntities = new List<FuelEntity>(FuelEntities.Count);
            modelPasiveEntities = new List<CircleEntity>(RadionEntities.Count);
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
            //Clean empty fuels 
            FuelEntities.RemoveAll((x => x.Empty));
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
        public bool Collision(CircleEntity entity, Vector2 newMiddle,Entity ignoredEntity = null)
        {
            if (ignoredEntity == null)
                ignoredEntity = entity;
            //Collisions with borders: 
            if (Intersections.CircleLineSegmentIntersection(newMiddle, entity.Radius, A, B).Length != 0)
                return true;
            if (Intersections.CircleLineSegmentIntersection(newMiddle, entity.Radius, B, D).Length != 0)
                return true;
            if (Intersections.CircleLineSegmentIntersection(newMiddle, entity.Radius, C, D).Length != 0)
                return true;
            if (Intersections.CircleLineSegmentIntersection(newMiddle, entity.Radius, A, C).Length != 0)
                return true;
            //Collision with other robots: 
            foreach (var r in Robots)
            {
                //jump over the sameEntity
                if(r == ignoredEntity)
                    continue;
                if (Intersections.CircleCircleIntersection(newMiddle, entity.Radius, r.Middle, r.Radius))
                    return true;
            }
            //Collision with passive entities: 
            foreach (var p in PasiveEntities)
            {
                if(p == ignoredEntity)
                    continue;
                if (Intersections.CircleCircleIntersection(newMiddle, entity.Radius, p.Middle, p.Radius))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Return true, if given entity collides with something of the enviroment
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Collision(CircleEntity entity)
        {
            return Collision(entity, entity.Middle,entity);
        }
        /// <summary>
        /// Return true, if given entity collides with something of the enviroment and ignore collision with ignoredEntity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignoredEntity"></param>
        /// <returns></returns>
        public bool Collision(CircleEntity entity, Entity ignoredEntity)
        {
            return Collision(entity, entity.Middle, ignoredEntity);
        }
        /// <summary>
        /// Collision between Line Entity & enviroment 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignoredEntity"></param>
        /// <returns>The nearest point to the first point of Line entity </returns>
        public Intersection Collision(LineEntity entity,Entity ignoredEntity = null)
        {
            Intersection theNearestIntersection = new Intersection
            {
                CollidingEntity = null,
                Distance = float.PositiveInfinity,
                IntersectionPoint = new Vector2(float.PositiveInfinity, float.PositiveInfinity)
            };
            Vector2 intersection;
            float testedDistance;
            //Collisions with borders 
            // A -------- B 
            intersection = Intersections.LinesegmentLinesegmentIntersection(entity.A, entity.B, A, B);
            testedDistance = Vector2.DistanceSquared(intersection, entity.A);
            if (testedDistance < theNearestIntersection.Distance)
            {
                theNearestIntersection.Distance = testedDistance;
                theNearestIntersection.IntersectionPoint = intersection;
            }
            // B
            // |
            // | 
            // C 
            intersection = Intersections.LinesegmentLinesegmentIntersection(entity.A, entity.B, B, D);
            testedDistance = Vector2.DistanceSquared(intersection, entity.A);
            if (testedDistance < theNearestIntersection.Distance)
            {
                theNearestIntersection.Distance = testedDistance;
                theNearestIntersection.IntersectionPoint = intersection;
            }
            //D ---------------C 
            intersection = Intersections.LinesegmentLinesegmentIntersection(entity.A, entity.B, C, D);
            testedDistance = Vector2.DistanceSquared(intersection, entity.A);
            if (testedDistance < theNearestIntersection.Distance)
            {
                theNearestIntersection.Distance = testedDistance;
                theNearestIntersection.IntersectionPoint = intersection;
            }
            // A
            // |
            // | 
            // D 
            intersection = Intersections.LinesegmentLinesegmentIntersection(entity.A, entity.B, A, C);
            testedDistance = Vector2.DistanceSquared(intersection, entity.A);
            if (testedDistance < theNearestIntersection.Distance)
            {
                theNearestIntersection.Distance = testedDistance;
                theNearestIntersection.IntersectionPoint = intersection;
            }

            //Collisions with other robots
            foreach (var r in Robots)
            {
                if(r == ignoredEntity) continue;
                foreach (var i in Intersections.CircleLineSegmentIntersection(r.Middle,r.Radius,entity.A,entity.B))
                {
                    testedDistance = Vector2.DistanceSquared(i, entity.A);
                    if (testedDistance < theNearestIntersection.Distance)
                    {
                        theNearestIntersection.Distance = testedDistance;
                        theNearestIntersection.IntersectionPoint = i;
                        theNearestIntersection.CollidingEntity = r; 
                    }
                }
            }
            //Collision with passive entities 
            foreach (var p in PasiveEntities)
            {
                if (p == ignoredEntity) continue; 
                foreach (var i in Intersections.CircleLineSegmentIntersection(p.Middle, p.Radius, entity.A, entity.B))
                {
                    testedDistance = Vector2.DistanceSquared(i, entity.A);
                    if (testedDistance < theNearestIntersection.Distance)
                    {
                        theNearestIntersection.Distance = testedDistance;
                        theNearestIntersection.IntersectionPoint = i;
                        theNearestIntersection.CollidingEntity = p;
                    }
                }
            }

            return theNearestIntersection;
        }
        //COLISION WITH RADIO BROADCASTING
        /// <summary>
        /// true if colides with any radio signal except given signal
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CollisionRadio(CircleEntity entity)
        {
            var intersections = new Dictionary<int, RadioIntersection>();
            foreach (var r in RadionEntities)
            {
                if (Intersections.CircleCircleIntersection(entity.Middle, entity.Radius, r.Middle, r.Radius))
                {
                    var i = (RadioEntity) r; 
                    if(intersections.ContainsKey(r.ValueOfSignal))
                    {
                        intersections[i.ValueOfSignal].SumOfDirections += i.Middle;
                        intersections[i.ValueOfSignal].AmountOfSignal++;
                    }
                    else
                    {
                        var ir = new RadioIntersection(i.ValueOfSignal);
                        ir.AmountOfSignal++;
                        ir.SumOfDirections += i.Middle;
                        intersections.Add(i.ValueOfSignal,ir);
                    }
                }
            }

            return false; 
        }
        //COLISION WITH FUEL 
        /// <summary>
        /// True if colides with any fuel and refuel entity 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CollisionFuel(RobotEntity entity)
        {
            bool o = false;
            List<FuelEntity> l = new List<FuelEntity>();
            //Prepare colliding fuel
            foreach (var f in FuelEntities)
            {
                if (Intersections.CircleCircleIntersection(entity.Middle, entity.Radius, f.Middle, f.Radius))
                    l.Add(f);
            }

            foreach (var f in l)
            {
                if (!f.Empty)
                {
                    entity.ConsumeFuel(f);
                    o = true;
                }
            }

            return o; 
        }
        /// <summary>
        /// True if colides with any fuel, do not interact with that 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Vector2 CollisionFuel(LineEntity entity)
        {
            Vector2 theNearestPoint = new Vector2(float.PositiveInfinity,float.PositiveInfinity);
            float theNearestDistance = float.PositiveInfinity;
            foreach (var f in FuelEntities)
            {
                foreach (var i in Intersections.CircleLineSegmentIntersection(f.Middle, f.Radius, entity.A, entity.B))
                {
                    float testedDistance = Vector2.DistanceSquared(entity.A,i);
                    if (testedDistance < theNearestDistance)
                    {
                        theNearestDistance = testedDistance;
                        theNearestPoint = i;
                    }
                }
            }

            return theNearestPoint;
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
        ///  Lower right corner
        /// </summary>
        public Vector2 C { get; protected set; }
        /// <summary>
        ///  Lower left corner
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
