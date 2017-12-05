//#define POSCORRECT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
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
#if DEBUG
        public Stopwatch SHwatch = new Stopwatch();
        public Stopwatch Owatch  = new Stopwatch();

#endif 

        //PUBLIC METHODS
        //GLOBAL METHODS 
        /// <summary>
        /// Creates new map with given set up of robots etc
        /// </summary>
        public Map(float height, float width, List<RobotEntity> robots = null, List<CircleEntity> pasiveEntities = null,
            List<FuelEntity> fuelEntities = null, List<RadioEntity> constRadioSignals = null)
        {
            //Init characteristics of map 
            MaxHeight = height;
            MaxWidth = width;
            Cycle = 0;
            //Set border points
            A = new Vector2(0, 0);
            B = new Vector2(MaxWidth, 0);
            C = new Vector2(0, MaxHeight);
            D = new Vector2(MaxWidth, MaxHeight);

            //Copy initial set up 
            Robots = robots ?? new List<RobotEntity>();
            PasiveEntities = new EntityMap<CircleEntity>(MaxHeight,MaxWidth,50);
            if (pasiveEntities != null)
                foreach (var p in pasiveEntities)
                    PasiveEntities.Add(p);
            FuelEntities = new EntityMap<FuelEntity>(MaxHeight, MaxWidth, 50);
            if (fuelEntities != null)
            {
                foreach (var f in fuelEntities)
                {
                    FuelEntities.Add(f);
                }
            }
            //No radio signals in the begining 
            RadioEntities = new EntityMap<RadioEntity>(MaxHeight, MaxWidth, 200);
            //Mark down initial set up 
            modelRobotEntities = new List<RobotEntity>(Robots.Count);
            modelFuelEntities = new List<FuelEntity>(FuelEntities.Count);
            modelPasiveEntities = new List<CircleEntity>(RadioEntities.Count);
            constantRadioSignal = constRadioSignals ?? new List<RadioEntity>(); 

            foreach (var r in Robots)
            {
                modelRobotEntities.Add((RobotEntity) r.DeepClone());
            }

            foreach (var p in PasiveEntities)
            {
                modelPasiveEntities.Add((CircleEntity) p.DeepClone());
            }

            foreach (var f in FuelEntities)
            {
                modelFuelEntities.Add((FuelEntity) f.DeepClone());
            }
            foreach (var r in constantRadioSignal)
            {
                RadioEntities.Add(r);
            }
            //Init colliding testing 
#if DEBUG
            CheckCorrectionOfPossition();
#endif    

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
            RadioEntities.Clear();
            PasiveEntities.Clear();
            FuelEntities.Clear();
            //Copy models from set up lists
            foreach (var r in modelRobotEntities)
            {
                Robots.Add((RobotEntity) r.DeepClone());
            }
            foreach (var p in modelPasiveEntities)
            {
                PasiveEntities.Add((CircleEntity) p.DeepClone());
            }
            foreach (var f in modelFuelEntities)
            {
                FuelEntities.Add(f);
            }
            foreach (var r in constantRadioSignal)
            {
                RadioEntities.Add(r);
            }
            //No point of coping passive entities 
#if DEBUG
            CheckCorrectionOfPossition();
#endif 
        }

        /// <summary>
        /// Make single step of simulation
        /// </summary>
        public void MakeStep()
        {
#if DEBUG && POSCORRECT
            CheckCorrectionOfPossition();
#endif
            //Make all robots read situation & decide, no point of random iteration 
            foreach (var r in Robots)
            {
                if (r.Alive)
                    r.PrepareMove(this);
                Debug.Assert(!OutOfBorderTest(r));
            }
            //Clean signals from map 
            RadioEntities.Clear();
            //Add constant signals
            constantRadioSignal.ForEach((entity =>RadioEntities.Add(entity)));
            //Random iteration through list of robot
            //Make movent, activate effectors
            foreach (int i in Enumerable.Range(0, Robots.Count).OrderBy(x => RandomNumber.GetRandomInt()))
            {
                Debug.Assert(!OutOfBorderTest(Robots[i]));
                if (Robots[i].Alive)
                    Robots[i].Move(this);
                Debug.Assert(!OutOfBorderTest(Robots[i]));
            }
#if DEBUG && POSCORRECT
            CheckCorrectionOfPossition();
#endif
            //Find intersection with fuels
            if (FuelEntities.Count != 0)
            {
                foreach (var r in Robots)
                    CollisionFuel(r);
            }
            //Clean empty fuels 
            FuelEntities.RemoveAll((x => x.Empty));
            //Cycle change 
            Cycle++;
        }

        //GENERATOR
        /// <summary>
        /// Gener
        /// </summary>
        /// <param name="map"></param>
        /// <param name="model"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static List<T> GenerateRandomPos<T>(Map map, CircleEntity model, int amount) where T : CircleEntity
        {
            List<T> newList = new List<T>();
            float newX;
            float newY;

            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    //Generate Random position
                    newX = RandomNumber.GetRandomFloat() * map.MaxWidth;
                    newY = RandomNumber.GetRandomFloat() * map.MaxHeight;
                    T c = (T) model.DeepClone();
                    c.MoveTo(new Vector2(newX,newY));
                    if (!map.Collision(c) && !map.OutOfBorderTest(c))
                    {
                        map.PasiveEntities.Add(c);
                        newList.Add(c);
                        break;
                    }
                }
            }
            if (newList.Count != amount)
                throw new ArgumentException("Wrong generating random possition ");

            return newList;


        }

        //COLISION AND MOVEMENTS 
        //COLLISION WITH OTHER ROBOTS OR PASSIVE ENTITY 
        /// <summary>
        /// Collision of circle to the newMiddle
        /// </summary>
        public bool Collision(CircleEntity entity, Vector2 newMiddle, Entity ignoredEntity = null)
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
                if (!r.Alive || r == ignoredEntity)
                    continue;
                if (Intersections.CircleCircleIntersection(newMiddle, entity.Radius, r.Middle, r.Radius))
                    return true;
            }
            //Collision with passive entities: 
            bool list_output = false;
            bool spatialHashing_output = false;
            foreach (var p in PasiveEntities)
            {
                if (p == ignoredEntity)
                    continue;
                if (Intersections.CircleCircleIntersection(newMiddle, entity.Radius, p.Middle, p.Radius))
                    list_output = true;
            }

            foreach (var p  in PasiveEntities.CircleIntersection(new ObstacleEntity(newMiddle,entity.Radius)))
            {
                if (p == ignoredEntity)
                    continue;
                if (Intersections.CircleCircleIntersection(newMiddle, entity.Radius, p.Middle, p.Radius))
                    spatialHashing_output = true;
            }

            Debug.Assert(spatialHashing_output == list_output);

            return spatialHashing_output;
        }

        /// <summary>
        /// Return true, if given entity collides with something of the enviroment
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Collision(CircleEntity entity)
        {
            return Collision(entity, entity.Middle, entity);
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
        public Intersection Collision(LineEntity entity, Entity ignoredEntity = null, bool discovering = false)
        {
            Intersection theNearestIntersection = new Intersection
            {
                CollidingEntity = null,
                Distance = float.PositiveInfinity,
                IntersectionPoint = new Vector2(float.PositiveInfinity, float.PositiveInfinity)
            };
            //Collisions with borders 
            // A -------- B 
            var intersection = Intersections.LinesegmentLinesegmentIntersection(entity.A, entity.B, A, B);
            var testedDistance = Vector2.DistanceSquared(intersection, entity.A);
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
                if (!r.Alive || r == ignoredEntity) continue;
                foreach (var i in Intersections.CircleLineSegmentIntersection(r.Middle, r.Radius, entity.A, entity.B))
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
            Intersection list_intersection = new Intersection();
            list_intersection.Distance = float.PositiveInfinity;
            list_intersection.CollidingEntity = null;
            list_intersection.IntersectionPoint = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            //Collision with passive entities 
#if DEBUG
            Owatch.Start();
#endif
            foreach (var p in PasiveEntities)
            {
                if (p == ignoredEntity) continue;
                foreach (var i in Intersections.CircleLineSegmentIntersection(p.Middle, p.Radius, entity.A, entity.B))
                {
                    testedDistance = Vector2.DistanceSquared(i, entity.A);
                    if (testedDistance < list_intersection.Distance)
                    {
                        list_intersection.Distance = testedDistance;
                        list_intersection.IntersectionPoint = i;
                        list_intersection.CollidingEntity = p;
                    }
                }
            }
#if DEBUG
         Owatch.Stop();
#endif

            Debug.Assert(float.IsPositiveInfinity(list_intersection.Distance) || Vector2.Distance( list_intersection.IntersectionPoint,entity.A) - entity.Length <= 0.01);

            Intersection spatial_intersection = new Intersection();
            spatial_intersection.Distance = float.MaxValue;
            spatial_intersection.CollidingEntity = null;
            spatial_intersection.IntersectionPoint = new Vector2(float.PositiveInfinity,float.PositiveInfinity);

#if DEBUG
            SHwatch.Start();
#endif

            foreach (var p in PasiveEntities.LineIntersection(entity))
            {
                if (p == ignoredEntity) continue;
                foreach (var i in Intersections.CircleLineSegmentIntersection(p.Middle, p.Radius, entity.A, entity.B))
                {
                    testedDistance = Vector2.DistanceSquared(i, entity.A);
                    if (testedDistance < spatial_intersection.Distance)
                    {
                        spatial_intersection.Distance = testedDistance;
                        spatial_intersection.IntersectionPoint = i;
                        spatial_intersection.CollidingEntity = p;
                    }
                }
            }
#if DEBUG
            SHwatch.Stop();
#endif

            if (list_intersection.Distance <= theNearestIntersection.Distance)
            {
                theNearestIntersection = list_intersection;

                Debug.Assert( (list_intersection.CollidingEntity == null && spatial_intersection.CollidingEntity == null) ||  Math.Abs(list_intersection.Distance - spatial_intersection.Distance) < 0.0001);
                 Debug.Assert(list_intersection.CollidingEntity == spatial_intersection.CollidingEntity);
                Debug.Assert((list_intersection.CollidingEntity == null && spatial_intersection.CollidingEntity == null) || list_intersection.IntersectionPoint == spatial_intersection.IntersectionPoint);
            }



            //if the nearest intersection is a raw material markdown discovery
            if (!discovering || theNearestIntersection.CollidingEntity == null)
                return theNearestIntersection;
            (theNearestIntersection.CollidingEntity as CircleEntity).Discovered = true;
            return theNearestIntersection;
        }


        //COLISION WITH RADIO BROADCASTING
        /// <summary>
        /// Return dictionary of colliding radioEntities with frequency and mean direction  
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Dictionary<int, RadioIntersection> CollisionRadio(CircleEntity entity)
        {
            var intersections = new Dictionary<int, RadioIntersection>();

            foreach (var r in RadioEntities.CircleIntersection(new ObstacleEntity(entity.Middle,entity.Radius)))
            {
                if (Intersections.CircleCircleIntersection(entity.Middle, entity.Radius, r.Middle, r.Radius))
                {
                    //Ignore local transmitting
                    if (r.Middle == entity.Middle)
                        continue;
                    var i = (RadioEntity) r;
                    if (intersections.ContainsKey(r.ValueOfSignal))
                    {
                        intersections[i.ValueOfSignal].SumOfDirections += i.Middle - entity.Middle;
                        intersections[i.ValueOfSignal].AmountOfSignal++;
                    }
                    else
                    {
                        var ir = new RadioIntersection(i.ValueOfSignal);
                        ir.AmountOfSignal++;
                        ir.SumOfDirections += i.Middle - entity.Middle;
                        intersections.Add(i.ValueOfSignal, ir);
                    }
                }
            }
            return intersections;
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
        /// Return Intersection of colliding fuel 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Intersection CollisionFuel(LineEntity entity)
        {
            Intersection origin_nearestPoint = new Intersection()
            {
                CollidingEntity = null,
                Distance = float.PositiveInfinity,
                IntersectionPoint = new Vector2(float.PositiveInfinity, float.PositiveInfinity)
            };

            Intersection SH_theNearestPoint = new Intersection()
            {
                CollidingEntity = null,
                Distance = float.PositiveInfinity,
                IntersectionPoint = new Vector2(float.PositiveInfinity, float.PositiveInfinity)
            };

            foreach (var f in FuelEntities)
            {
                foreach (var i in Intersections.CircleLineSegmentIntersection(f.Middle, f.Radius, entity.A, entity.B))
                {
                    float testedDistance = Vector2.DistanceSquared(entity.A, i);
                    if (testedDistance < origin_nearestPoint.Distance)
                    {
                        origin_nearestPoint.Distance = testedDistance;
                        origin_nearestPoint.IntersectionPoint = i;
                        origin_nearestPoint.CollidingEntity = f;
                    }
                }
            }

            foreach (var f in FuelEntities.LineIntersection(entity))
            {
                foreach (var i in Intersections.CircleLineSegmentIntersection(f.Middle, f.Radius, entity.A, entity.B))
                {
                    float testedDistance = Vector2.DistanceSquared(entity.A, i);
                    if (testedDistance < SH_theNearestPoint.Distance)
                    {
                        SH_theNearestPoint.Distance = testedDistance;
                        SH_theNearestPoint.IntersectionPoint = i;
                        SH_theNearestPoint.CollidingEntity = f;
                    }
                }
            }

            Debug.Assert((float.IsPositiveInfinity(SH_theNearestPoint.Distance) && float.IsPositiveInfinity(origin_nearestPoint.Distance)) || Math.Abs(SH_theNearestPoint.Distance - origin_nearestPoint.Distance) < 0.01);
            Debug.Assert(SH_theNearestPoint.CollidingEntity == origin_nearestPoint.CollidingEntity);
            Debug.Assert((float.IsPositiveInfinity(SH_theNearestPoint.Distance) && float.IsPositiveInfinity(origin_nearestPoint.Distance)) || Math.Abs(SH_theNearestPoint.IntersectionPoint.X - origin_nearestPoint.IntersectionPoint.X) < 0.01);
            Debug.Assert((float.IsPositiveInfinity(SH_theNearestPoint.Distance) && float.IsPositiveInfinity(origin_nearestPoint.Distance)) || Math.Abs(SH_theNearestPoint.IntersectionPoint.Y - origin_nearestPoint.IntersectionPoint.Y) < 0.01);

            return origin_nearestPoint;

        }

        // COLISION THAT COUNTS COLOR 
        public Dictionary<Entity.EntityColor, ColorIntersection> CollisionColor(CircleEntity entity,RobotEntity ignoredRobot=null)
        {
            Dictionary<Entity.EntityColor, ColorIntersection> o =
                new Dictionary<Entity.EntityColor, ColorIntersection>();
            //Collision with other robots: 
            foreach (var r in Robots)
            {
                if(r == ignoredRobot) continue;
                
                if (!r.Alive) continue;

                if (Intersections.CircleCircleIntersection(entity.Middle, entity.Radius, r.Middle, r.Radius))
                {
                    if (o.ContainsKey(r.Color))
                        o[r.Color].Amount++;
                    else
                        o.Add(r.Color, new ColorIntersection(r.Color));
                }
            }
            //Collision with passive entities: 
            foreach (var p in PasiveEntities)
            {
                if (Intersections.CircleCircleIntersection(entity.Middle, entity.Radius, p.Middle, p.Radius))
                {
                    if (o.ContainsKey(p.Color))
                        o[p.Color].Amount++;
                    else
                        o.Add(p.Color, new ColorIntersection(p.Color));
                }
            }
            //Collision with fuel entities 
            foreach (var f in FuelEntities)
            {
                if (Intersections.CircleCircleIntersection(entity.Middle, entity.Radius, f.Middle, f.Radius))
                {
                    if (o.ContainsKey(f.Color))
                        o[f.Color].Amount++;
                    else
                        o.Add(f.Color, new ColorIntersection(f.Color));
                }
            }

            return o;
        }
        //BORDER CHECK TESTS
        /// <summary>
        /// Check if line entity lies out of map borders 
        /// return false if not 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool OutOfBorderTest(LineEntity entity)
        {
            if (entity.A.X < 0 || entity.A.Y < 0 || entity.A.X > MaxWidth || entity.A.Y > MaxHeight)
                return true;
            if (entity.B.X < 0 || entity.B.Y < 0 || entity.B.X > MaxWidth || entity.B.Y > MaxHeight)
                return true;
            return false;
        }
        /// <summary>
        /// Check if circle entity lies out of map borders 
        /// return false if not 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool OutOfBorderTest(CircleEntity entity)
        {
            if (entity.Middle.X - entity.Radius < 0 || entity.Middle.Y - entity.Radius < 0)
                return true;
            if (entity.Middle.X + entity.Radius > MaxWidth || entity.Middle.Y + entity.Radius > MaxHeight)
                return true;
            return false;
        }
        //GLOBAL Map Test
        public void CheckCorrectionOfPossition()
        {
            foreach (var r in Robots)
            {
                if (OutOfBorderTest(r))
                    throw new ArgumentException("Robot out of map");
                if(Collision(r,r))
                    throw new ArgumentException("Robot colides with enviroment");
            }
            foreach (var p in PasiveEntities)
            {
                if (OutOfBorderTest(p))
                    throw new ArgumentException("Passive entity out of map");
                if (Collision(p,p))
                    throw new ArgumentException("Passive entity colides with enviroment");
            }
            foreach (var f in FuelEntities)
            {
                if (OutOfBorderTest(f))
                    throw new ArgumentException("Fuel out of map");
            }
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
        public EntityMap<CircleEntity> PasiveEntities;
        /// <summary>
        /// Stores all radio broadcast in the scope 
        /// </summary>
        public EntityMap<RadioEntity> RadioEntities;
        //public List<RadioEntity> RadioEntities;
        /// <summary>
        /// Stores all fuel entities in the scope 
        /// </summary>
        public EntityMap<FuelEntity> FuelEntities;

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
        /// <summary>
        /// Const radio signals
        /// </summary>
        public List<RadioEntity> constantRadioSignal;
    }
}
