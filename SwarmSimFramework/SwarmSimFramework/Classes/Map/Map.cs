using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
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
        public bool Collision(CircleEntity entity, Vector2 newMiddle)
        {
            //Collisions with borders: 

            //Collision with other robots: 

            //Collision with passive entities: 
            throw new NotImplementedException(); 
        }
        /// <summary>
        /// Return true, if given entity collides with something of the enviroment
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Collision(CircleEntity entity)
        {
            return Collision(entity, entity.Middle);
        }
        /// <summary>
        /// Collision between Line Entity & enviroment 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Vector2 Collision(LineEntity entity)
        {
            //Collisions with borders 

            //Collisions with other robots

            //Collision with passive entities 
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

        // PRIVATE METHODS
        // Intersection methods
        /// <summary>
        /// Return true if a, b circles intersects
        /// </summary>
        /// <param name="aMiddle"></param>
        /// <param name="aRadius"></param>
        /// <param name="bMiddle"></param>
        /// <param name="bRadius"></param>
        /// <returns></returns>
        bool CircleCircleIntersection(Vector2 aMiddle, float aRadius, Vector2 bMiddle, float bRadius)
        {
            float squaredDistanceBetweenAB = (bMiddle - aMiddle).LengthSquared();

            if (squaredDistanceBetweenAB <= ((aRadius + bRadius) * (aRadius + bRadius)))
                return true;
            return false;
        }
        /// <summary>
        /// Return intersection point of Line a and b
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns> Return NaN vector if no intersection, Infity if one line is part of the second </returns>
        Vector2 LineLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            //Transform points to equation form 
            //line a 
            float A1 = a1.Y - a2.Y;
            float B1 = a1.X - a2.X;
            float C1 = A1 * a1.X + B1 * a1.Y;

            //line b
            float A2 = b1.Y - b2.Y;
            float B2 = b1.X - b1.X;
            float C2 = A2 * b1.X + B2 * b1.Y;

            //assuming Ax + By = C form 
            float delta = A1 * B2 - A2 * B1; 
            //Check parallelity of lines 
            if (delta == 0)
            {
                //Check equality of lines 
                if(A2*b1.X + B2*b1.Y == C2)
                    return new Vector2(float.PositiveInfinity,float.PositiveInfinity);

                return new Vector2(float.NaN,float.NaN);
            }
            //If not parellel return point of intersection
            return new Vector2((B2*C1 - B1*C2)/delta,(A1*C2 - A2*C1)/delta);


        }
        /// <summary>
        /// Return intersection point of two line segment a & b 
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        Vector2 LinesegmentLinesegmentIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            //If not line throw exception
            Debug.Assert(a1-a2 == Vector2.Zero);
            Debug.Assert(b1-b2 == Vector2.Zero);
            //Find line & line intersection 
            Vector2 i = LineLineIntersection(a1, a2, b1, b2);
            //No intersection 
            if (float.IsNaN(i.X))
                return i;

            if (!float.IsPositiveInfinity(i.X))
            {
                //Check line a contains intersection 
                //if  perpendicular with axis 
                if (a1.X == a2.X)
                {
                    if (!MyExtensions.Between(a1.X, a2.X, i.X))
                        return new Vector2(float.NaN, float.NaN);
                }
                else
                {
                    if (!MyExtensions.Between(a1.Y, a2.Y, i.Y))
                        return new Vector2(float.NaN, float.NaN);
                }

                //Check line b contains intersection 
                if (b1.X == b2.X)
                {
                    if (!MyExtensions.Between(b1.X, b2.X, i.X))
                        return new Vector2(float.NaN, float.NaN);
                }
                else
                {
                    if (!MyExtensions.Between(b1.Y, b2.Y, i.Y))
                        return new Vector2(float.NaN, float.NaN);
                }

                return i;
            }
            //Same line 
            else 
            {
                Vector2 aMiddle = Vector2.Divide((a1 + a2), 2.0f);
                float aRadius = (a1 - aMiddle).Length();
                Vector2 bMiddle = Vector2.Divide((b1 + b2), 2.0f);
                float bRadius = (b1 - bMiddle).Length();

                //if circle intersects 
                if (CircleCircleIntersection(aMiddle, aRadius, bMiddle, bRadius))
                {
                    return i; 
                }
                else
                {
                    return new Vector2(float.NaN,float.NaN);
                }

            }
        }
        /// <summary>
        /// Return all point of intersections of circle & line 
        /// </summary>
        /// <param name="aMiddle"></param>
        /// <param name="aRadius"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        Vector2 CircleLineIntersection(Vector2 aMiddle, float aRadius, Vector2 b1, Vector2 b2)
        {
            //check if any intersection exists, find intersection with perpedicular line
            Vector2 p1 = aMiddle;
            Vector2 directionB = b1 - b2;
            //Create perpedicular vector to b
            Vector2 perpedicularB = new Vector2(directionB.Y,(-1.0f*directionB.X));
            Vector2 p2 = aMiddle + perpedicularB;
            //Intersection with perpedicular line
            Vector2 i = LinesegmentLinesegmentIntersection(p1, p2, b1, b2);
            //If not in the segment 
            if (float.IsNaN(i.X))
                return i;

        }
        /// <summary>
        /// Return all intersection points
        /// </summary>
        /// <param name="aMiddle"></param>
        /// <param name="aRadius"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        Vector2[] CircleLinesegmentIntersection(Vector2 aMiddle, float aRadius, Vector2 b1, Vector2 b2)
        {
            throw new NotImplementedException();
        }

        
    }
}
