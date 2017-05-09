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
        public int SensorsDimension { get; protected set; }
        /// <summary>
        /// Effector dimension 
        /// </summary>
        public int EffectorsDimension { get; protected set;  }
        /// <summary>
        /// Maximum of communication values between Sensor => Brain && Brain => Effectors
        /// </summary>
        public float NormalizeMax { get; protected set;  }
        /// <summary>
        /// Minimum of communication values between Sensor => Brain && Brain => Effectors
        /// </summary>
        public float NormalizeMin { get; protected set; }
        /// <summary>
        /// Actual map, where the robot moves 
        /// </summary>
        public Map.Map Map; 
        //PRIVATE MEMBERS
        /// <summary>
        /// Last values from last invoke of PrepareMove
        /// </summary>
        protected float[] LastReadValues;
        /// <summary>
        /// Effector setting from Brain based on lastReadValues
        /// </summary>
        protected float[] BrainDecidedValues;
        //CONSTRUCTOR
        /// <summary>
        /// Create new entity with given sensors and effectors 
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="name"></param>
        /// <param name="map"></param>
        /// <param name="effectors"></param>
        /// <param name="sensors"></param>
        /// <param name="amountOfFuel"></param>
        /// <param name="normalizeMax"></param>
        /// <param name="normalizeMin"></param>
        /// <param name="orientation"></param>
        protected RobotEntity(Vector2 middle, float radius, string name,Map.Map map, IEffector[] effectors,ISensor[] sensors, float amountOfFuel,float normalizeMax =100,float normalizeMin = -100, float orientation = 0) : base(middle, radius, name, orientation)
        {
            //Normalize values
            NormalizeMax = normalizeMax;
            NormalizeMin = normalizeMin;
            Map = map;
            //Effectors & Sensors 
            Effectors = effectors;
            Sensors = sensors;
            //Count dimensions
            EffectorsDimension = 0;
            SensorsDimension = 0; 
            foreach (var e in Effectors)
            { 
                EffectorsDimension += e.Dimension;
                e.ConnectToRobot(this);
            }
            foreach (var s in Sensors)
            {
                SensorsDimension += s.Dimension;
                s.ConnectToRobot(this);
            }
            StartingPoint = middle;
            FuelAmount = amountOfFuel;
            CollisionDetected = 0; 
        }
        /// <summary>
        /// Prepare for movement
        /// 1. read sensors
        /// 2. let brain decide effectors setting
        /// </summary>
        public void PrepareMove(Map.Map map)
        {
           if(Brain == null)
                throw  new NotImplementedException("Robot body without brain can not move");

            //Read all sensors && return 
            float[] readValues = new float[SensorsDimension];
            int index = 0;
            foreach (var s in Sensors)
            {
                foreach (var f in s.Count(this,map))
                {
                    readValues[index] = f;
                    index++;
                }
            }
            LastReadValues = readValues;
            //Feed brain
            BrainDecidedValues = Brain.Decide(LastReadValues);
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
            //Invoke all effectors with brain decided values
            int index = 0;
            foreach (var e in Effectors)
            {
                float[] set = new float[e.Dimension];
                for (int i = 0; i < e.Dimension; i++)
                {
                    set[i] = LastReadValues[index];
                    index++;
                }
                e.Effect(set,this,map);
            }
        }
        /// <summary>
        /// Reset calculators
        /// </summary>
        public void Reset()
        {
            CollisionDetected = 0;
            StartingPoint = Middle;
            LastReadValues = null;
            BrainDecidedValues = null;
        }
        /// <summary>
        /// Deep clone of Robot entity, map & brain are null
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            RobotEntity r = (RobotEntity) this.MemberwiseClone();
            r.Reset();
            r.Brain = null;
            r.Map = null;
            r.Effectors = new IEffector[Effectors.Length];
            r.Sensors = new ISensor[Sensors.Length];
            for (int i = 0; i < Effectors.Length; i++)
            {
                r.Effectors[i] = Effectors[i].Clone(); 
                r.Effectors[i].ConnectToRobot(r);
            }
            for (int i = 0; i < Sensors.Length; i++)
            {
                r.Sensors[i] = Sensors[i].Clone(); 
                r.Sensors[i].ConnectToRobot(r);
            }

            return r;
        }
        /// <summary>
        /// Consume fuel tank and make it empty
        /// </summary>
        /// <param name="fuelTank"></param>
        public void ConsumeFuel(FuelEntity fuelTank)
        {
           FuelAmount += fuelTank.Suck();
        }

 
    }
}
