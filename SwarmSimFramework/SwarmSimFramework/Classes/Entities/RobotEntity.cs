using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    public abstract class RobotEntity :CircleEntity
    {
        //STANDART VALUES
        public static Bounds StandardBounds = new Bounds() {Max = 100, Min = -100};
        //MEMBERs
        /// <summary>
        /// If entity has enough fuel & health integrity
        /// </summary>
        public bool Alive { get; protected set; }
        /// <summary>
        /// number of friendly team  
        /// </summary>
        public int TeamNumber { get; protected set; }
        /// <summary>
        /// Actual health of robot 
        /// </summary>
        public float Health { get; protected set; }
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
        /// Amount of invalid pick up or put with container
        /// </summary>
        public long InvalidContainerOperation;
        /// <summary>
        /// Amount of invalid refactor operation
        /// </summary>
        public long InvalidRefactorOperation;
        /// <summary>
        /// Amount of invalid weapon operation
        /// </summary>
        public long InvalidWeaponOperation;
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
        /// Interval  of values  between sensor -> brain , brain -> effector
        /// </summary>
        public Bounds NormalizedBound;
        /// <summary>
        /// Max capacity of container
        /// </summary>
        public int ContainerMaxCapacity { get; protected set; }
        /// <summary>
        /// Actual amount of entities in container
        /// </summary>
        public int ActualContainerCount { get; protected set; }
        //PRIVATE MEMBERS
        /// <summary>
        /// Health after creation
        /// </summary>
        protected float InitialHealth;
        /// <summary>
        /// Last values from last invoke of PrepareMove
        /// </summary>
        protected float[] LastReadValues;
        /// <summary>
        /// Effector setting from Brain based on lastReadValues
        /// </summary>
        protected float[] BrainDecidedValues;
        /// <summary>
        /// Intern container for entities 
        /// </summary>
        protected Stack<CircleEntity> Container;
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
        protected RobotEntity(Vector2 middle, float radius, string name, IEffector[] effectors,ISensor[] sensors, float amountOfFuel,int sizeOfContainer = 0, int teamNumber = 1, float health = 100, float normalizeMax =100,float normalizeMin = -100, float orientation = 0) : base(middle, radius, name, orientation)
        {
            //Normalize values
            NormalizedBound = new Bounds() {Max = normalizeMax, Min = normalizeMin};
            //Effectors & Sensors, container
            Container = new Stack<CircleEntity>(sizeOfContainer);
            ContainerMaxCapacity = sizeOfContainer;
            ActualContainerCount = 0;
            Effectors = effectors;
            Sensors = sensors;
            //Health settings 
            InitialHealth = health;
            Health = health;
            Alive = health > 0;
            TeamNumber = teamNumber;
            //Count dimensions
            EffectorsDimension = 0;
            SensorsDimension = 0;
            if(effectors != null)
            { foreach (var e in Effectors)
            { 
                EffectorsDimension += e.Dimension;
                e.ConnectToRobot(this);
            }}
            if(sensors != null)
            { foreach (var s in Sensors)
            {
                SensorsDimension += s.Dimension;
                s.ConnectToRobot(this);
            }}
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
            Health = InitialHealth;
            Alive = Health > 0;
            CollisionDetected = 0;
            InvalidContainerOperation = 0;
            InvalidRefactorOperation = 0;
            InvalidWeaponOperation = 0;
            StartingPoint = Middle;
            LastReadValues = null;
            BrainDecidedValues = null;
            Container.Clear();
            ActualContainerCount = 0;
        }
        /// <summary>
        /// Deep clone of Robot entity, map & brain are null
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            RobotEntity r = (RobotEntity) this.MemberwiseClone();
            r.Container = new Stack<CircleEntity>();
            r.Reset();
            r.Brain = null;
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
        //Container control
        /// <summary>
        /// Return true if adding was succesfull, return false if not,
        /// Add the entityCargo to robotContainer
        /// </summary>
        /// <param name="entityCargo"></param>
        /// <returns></returns>
        public bool PushContainer(CircleEntity entityCargo)
        {
            if (ContainerMaxCapacity == ActualContainerCount)
                return false;
            else
            { 
                Container.Push(entityCargo);
                ActualContainerCount++;
                return true;
            }
        }
        /// <summary>
        /// Return from top of the container or null if empty 
        /// </summary>
        /// <returns></returns>
        public CircleEntity PopContainer()
        {
            if (ActualContainerCount == 0)
                return null;
            else
            {
                ActualContainerCount--;
                return Container.Pop();
            }
        }
        /// <summary>
        /// Peek the top of container, null if empty
        /// </summary>
        /// <returns></returns>
        public CircleEntity PeekContainer()
        {
            if (ActualContainerCount == 0)
                return null;
            else
                return Container.Peek();
        }
        /// <summary>
        /// Deal damage to robot, if not any health integrity,die and return remaining fuel
        /// </summary>
        public void AcceptDamage(float amountOfDamage, Map.Map map)
        {
            Health -= amountOfDamage;
            if (Health <= 0)
            {
                Alive = false;
                //Return remaining fuel
                var remainingFuel = FuelAmount;
                FuelAmount = 0;
                map.FuelEntities.Add(new FuelEntity(this.Middle,FuelEntity.FuelRadius,remainingFuel));
                //Clear storage 
                Container.Clear();
                ActualContainerCount = 0;
            }
        }
        /// <summary>
        /// Recount dimensions of effectors and sensors
        /// </summary>
        public void CountDimension()
        {
            EffectorsDimension = 0;
            SensorsDimension = 0;
            if (Effectors != null)
            {
                foreach (var e in Effectors)
                {
                    EffectorsDimension += e.Dimension;
                    e.ConnectToRobot(this);
                }
            }
            if (Sensors != null)
            {
                foreach (var s in Sensors)
                {
                    SensorsDimension += s.Dimension;
                    s.ConnectToRobot(this);
                }
            }
        }
    }
}
