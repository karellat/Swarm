using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Numerics;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;

namespace SwarmSimFramework.Classes.Map
{
    public struct RobotModel
    {
        public RobotEntity model;
        public int amount; 
    }

    public struct MapModel
    {
        public float MapHeight;
        public float MapWidth;
        public List<RobotEntity> EnviromentRobots; 
        public List<RobotEntity> RobotBodies;
        public List<CircleEntity> PassiveEntities;
        public List<FuelEntity> FuelEntities;
        public List<RadioEntity> ConstRadioSignals;

        public Map ConstructMap()
        {
            var rb = new List<RobotEntity>(RobotBodies.Count + EnviromentRobots.Count);
            foreach (var r in RobotBodies)
                rb.Add((RobotEntity) r.DeepClone());
            foreach (var ev in EnviromentRobots)
            {
                var b = ev.Brain.GetCleanCopy();
                var nr = (RobotEntity) ev.DeepClone();
                nr.Brain = b;
                rb.Add(nr);               
            }
            var pe = new List<CircleEntity>(PassiveEntities.Count);
            foreach (var p in PassiveEntities)
                pe.Add((CircleEntity)p.DeepClone());
            var fe = new List<FuelEntity>(FuelEntities.Count);
            foreach (var f in FuelEntities)
                fe.Add((FuelEntity) f.DeepClone());
            var cr = new List<RadioEntity>(ConstRadioSignals.Count);
            foreach (var c in ConstRadioSignals)
                cr.Add((RadioEntity) c.DeepClone());
                
            
                return new Map(MapHeight,MapWidth,rb,pe,fe,cr);
        }
    }
    public class WoodScene
    {
        //WOOD EXPERIMENT - VARIABLES 
        /// <summary>
        /// Amount woods
        /// </summary>
        public static int AmountOfWoods;
        /// <summary>
        /// Amount trees
        /// </summary>
        public static int AmountOfTrees= 200;
        /// <summary>
        /// Max amount of robots in map 
        /// </summary>
        public static int MaxOfAmountRobots = 9;
        /// <summary>
        /// Height of map
        /// </summary>
        public static float MapHeight = 800;
        /// <summary>
        /// Width of map 
        /// </summary>
        public static float MapWidth = 1200;
        /// <summary>
        /// Max size of robot
        /// </summary>
        public static float RobotMaxRadius = 5;
        /// <summary>
        /// Init positions of vector 
        /// </summary>
        /// <returns></returns>
        public static Vector2[] InitPositionOfRobot(){
            List<Vector2> vectors = new List<Vector2>();
            float sW = MapWidth / 2 + 22;
            float sH = MapHeight / 2 + 22;
            for (int i = 0; i < 5; i++)
            {
                var nW = sW - 11 * i;
                var nH = sH - 11 * i;
                if (nW == MapWidth/2 && nH == MapHeight/2)
                {
                    vectors.Add(new Vector2(nW,nH));
                }
                else
                {
                    vectors.Add(new Vector2(nW,MapHeight/2));
                    vectors.Add(new Vector2(MapWidth/2,nH));
                }
            }
            return vectors.ToArray();
        }
        //CREATION OF MAP 
        /// <summary>
        /// Create empty map for wood scene
        /// </summary>
        /// <returns></returns>
        public static Map MakeEmptyMap()
        {
            //Prepare objects of map 
            RawMaterialEntity tree = new RawMaterialEntity(new Vector2(0, 0), 5, 10, 10);
            WoodEntity wood = new WoodEntity(new Vector2(0, 0), 5, 10);
            ObstacleEntity initPosition = new ObstacleEntity(new Vector2(MapWidth / 2, MapHeight / 2), 60);
            //Generate randomly deployed tree
            Classes.Map.Map preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, new List<CircleEntity>() { initPosition });
            List<CircleEntity> trees =
                Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, tree, AmountOfTrees);
            var tp = trees.ToList();
            tp.Add(initPosition);
            preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, tp);
            List<CircleEntity> woods = Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, wood, AmountOfWoods);
            List<CircleEntity> passive = woods;

            List<RadioEntity> constSignals = new List<RadioEntity>(new []{new RadioEntity(new Vector2(MapWidth/2,MapHeight/2),60,2)});
            foreach (var t in trees)
                passive.Add(t);
            return new Map(MapHeight,MapWidth,null,passive,null,constSignals);

        }
        /// <summary>
        /// Create map with given models 
        /// </summary>
        /// <param name="robotModels"></param>
        /// <param name="amountOfRobots"></param>
        /// <returns></returns>
        public static MapModel MakeMapModel(RobotModel[] models)
        {
            //Check size & amount 
            int amountOfRobot = 0;
            foreach (var m in models)
            {
                amountOfRobot += m.amount; 
                if(m.model.Radius > RobotMaxRadius)
                    throw new ArgumentException("Robot with bigger radius");
                if(amountOfRobot > MaxOfAmountRobots)
                    throw new ArgumentException("More robots than maximum size");
            }
            List<RobotEntity> robots = new List<RobotEntity>();
            foreach (var m in models)
            {
                for (int i = 0;  i < m.amount; ++i)
                {
                    robots.Add((RobotEntity) m.model.DeepClone());
                }
            }
            Vector2[] initPos = InitPositionOfRobot();
            for (var index = 0; index < robots.Count; index++)
            {
                var r = robots[index];
                r.MoveTo(initPos[index]);
            }

            Map emptyMap = MakeEmptyMap();
            return new MapModel()
            {
                MapHeight = MapHeight,
                MapWidth = MapWidth,
                ConstRadioSignals = emptyMap.constantRadioSignal,
                FuelEntities = emptyMap.FuelEntities.ToList(),
                PassiveEntities = emptyMap.PasiveEntities.ToList(),
                RobotBodies = robots,
                EnviromentRobots = new List<RobotEntity>(0)
            };
        }

        public static Map MakeMap(RobotModel[] models)
        {
            return MakeMapModel(models).ConstructMap();
        }

        public static StringBuilder Log()
        {
            StringBuilder s = new StringBuilder();

            s.AppendLine("AmountOfWoods = " + AmountOfWoods);
            s.AppendLine("AmountOfTrees = " + AmountOfTrees);
            s.AppendLine("MaxOfAmountRobots = " + MaxOfAmountRobots);
            s.AppendLine("MapHeight = " + MapHeight);
            s.AppendLine("MapWidth = " + MapWidth);
            s.AppendLine("RobotMaxRadius = " + RobotMaxRadius);

            return s; 
        }
    }

    public static class MineralScene
    {
        //Mineral experiment - VARIABLES 
        /// <summary>
        /// Amount of mineral 
        /// </summary>
        public static int AmountOfMineral;
        /// <summary>
        /// Amount of obstacles
        /// </summary>
        public static int AmountOfObstacles;
        /// <summary>
        /// Amount of fuel in map
        /// </summary>
        public static int AmountOfFreeFuel;
        /// <summary>
        /// Signal in the middle of map with code 2(same as the refactor robot) 
        /// </summary>
        public static bool ConstEnviromentalSignal = false;
        /// <summary>
        /// Max amount of robots in map 
        /// </summary>
        public static int MaxOfAmountRobots = 15;
        /// <summary>
        /// Height of map
        /// </summary>
        public static float MapHeight = 800;
        /// <summary>
        /// Width of map 
        /// </summary>
        public static float MapWidth = 1200;
        /// <summary>
        /// Max size of robot
        /// </summary>
        public static float RobotMaxRadius = 15;
        /// <summary>
        /// Position for robot to start
        /// </summary>
        public static float initHeight = MapHeight / 2.0f;
        /// <summary>
        /// Position for robot to start 
        /// </summary>
        public static float initWidth = MapWidth / 2.0f;
        /// <summary>
        /// Radius for starting place of robots
        /// </summary>
        public static float initRadius = 95;
        

        /// <summary>
        /// Init positions of vector 
        /// </summary>
        /// <returns></returns>
        public static Vector2[] InitPositionOfRobot()
        {

            if(MaxOfAmountRobots > 15) 
                throw new ArgumentException("More robot than the maximum. Maximum is 15 and trying to add "
                    + MaxOfAmountRobots.ToString());

            if (RobotMaxRadius > 15)
                throw new ArgumentException("Bigger robot than the maximum. Maximum size is 15 and trying to add "
                                            + RobotMaxRadius.ToString());

            List<CircleEntity> placedEntities = new List<CircleEntity>();
            List<Vector2> vectors = new List<Vector2>();
            int maxH = (int)Math.Floor(initHeight + (initRadius - RobotMaxRadius));
            int minH = (int)Math.Floor(initHeight - (initRadius - RobotMaxRadius));

            int maxW = (int) Math.Floor(initWidth + (initRadius - RobotMaxRadius));
            int minW = (int) Math.Floor(initWidth - (initRadius - RobotMaxRadius));

            for (int attemps = 0; attemps < 1000; attemps++)
            {
                for (int amountOfRobots = 0; amountOfRobots < MaxOfAmountRobots; amountOfRobots++)
                {
                    for (int placeRobotAttempts = 0; placeRobotAttempts < 1000; placeRobotAttempts++)
                    {
                        //Create new random position of the robot, inside the initial position 
                        float vH = SupportClasses.RandomNumber.GetRandomInt(minH, maxH);
                        float vW = SupportClasses.RandomNumber.GetRandomInt(minW, maxW);
                        Vector2 middle = new Vector2(vW, vH);
                        ObstacleEntity e = new ObstacleEntity(middle, RobotMaxRadius);

                        bool correct = true; 
                        foreach (var p in placedEntities)
                        {
                            if (Map.Colides(e, p))
                            {
                                correct = false;
                                break; 
                            }
                                
                        }

                        if (correct)
                        {
                            vectors.Add(e.Middle);
                            placedEntities.Add(e);
                            break;
                        }
                    }
                }
                if (placedEntities.Count == MaxOfAmountRobots)
                    return vectors.ToArray();
            }
            throw new ArgumentOutOfRangeException("Unable to place initial position ");
        }

        public static Map MakeEmptyMap()
        {
            //Prepare objects of map 
            RawMaterialEntity mineral = new RawMaterialEntity(new Vector2(0, 0), 5, 100, 3);
            ObstacleEntity obstacle = new ObstacleEntity(Vector2.Zero, 5);

            ObstacleEntity initPosition = new ObstacleEntity(new Vector2(initWidth, initHeight), initRadius);
            //Generate randomly deployed minerals 
            Classes.Map.Map preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, new List<CircleEntity>() { initPosition });
            List<CircleEntity> minerals =
                Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, mineral, AmountOfMineral);
            var tp = minerals.ToList();
            tp.Add(initPosition);
            preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, tp);
            List<CircleEntity> obstacles = Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, obstacle, AmountOfObstacles);
            List<CircleEntity> passive = obstacles;
            
            foreach (var t in minerals)
                passive.Add(t);
            preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, passive.ToList());
            List<FuelEntity> fuels;

            if (AmountOfFreeFuel > 0)
                fuels =
                    Classes.Map.Map.GenerateRandomPos<FuelEntity>(preparedMap, new FuelEntity(Vector2.Zero, 5, 1000),
                        AmountOfFreeFuel);
            else
                fuels = null;

            List<RadioEntity> constSignals = null;
            if (ConstEnviromentalSignal)
                constSignals = new List<RadioEntity>() { new RadioEntity(new Vector2(initWidth,initHeight),initRadius,2)};
                

            return new Map(MapHeight, MapWidth, null, passive, fuels, constSignals);
        }

        /// <summary>
        /// Create map with given models 
        /// </summary>
        /// <param name="robotModels"></param>
        /// <param name="amountOfRobots"></param>
        /// <returns></returns>
        public static MapModel MakeMapModel(RobotModel[] models)
        {
            //Check size & amount 
            int amountOfRobot = 0;
            foreach (var m in models)
            {
                amountOfRobot += m.amount;
                if (m.model.Radius > RobotMaxRadius)
                    throw new ArgumentException("Robot with bigger radius");
                if (amountOfRobot > MaxOfAmountRobots)
                    throw new ArgumentException("More robots than maximum size");
            }
            List<RobotEntity> robots = new List<RobotEntity>();
            foreach (var m in models)
            {
                for (int i = 0; i < m.amount; ++i)
                {
                    robots.Add((RobotEntity)m.model.DeepClone());
                }
            }
            Vector2[] initPos = InitPositionOfRobot();
            for (var index = 0; index < robots.Count; index++)
            {
                var r = robots[index];
                r.MoveTo(initPos[index]);
            }

            Map emptyMap = MakeEmptyMap();
            return new MapModel()
            {
                MapHeight = MapHeight,
                MapWidth = MapWidth,
                ConstRadioSignals = emptyMap.constantRadioSignal,
                FuelEntities = emptyMap.FuelEntities.ToList(),
                PassiveEntities = emptyMap.PasiveEntities.ToList(),
                RobotBodies = robots,
                EnviromentRobots = new List<RobotEntity>(0)
            };
        }

        public static Map MakeMap(RobotModel[] models)
        {
            return MakeMapModel(models).ConstructMap();
        }

        public static StringBuilder Log()
        {
            throw new NotImplementedException();
        }
    }

    public static class CompetitiveScene<T> where T : IRobotBrain
    {
        /// <summary>
        /// Amount of obstacles
        /// </summary>
        public static int AmountOfObstacles;
        /// <summary>
        /// Max amount of robots in map 
        /// </summary>
        public static int MaxOfAmountRobots = 10;
        /// <summary>
        /// Height of map
        /// </summary>
        public static float MapHeight = 800;
        /// <summary>
        /// Width of map 
        /// </summary>
        public static float MapWidth = 1200;
        /// <summary>
        /// Max size of robot
        /// </summary>
        public static float RobotMaxRadius = 10;
        /// <summary>
        /// Robot models, null if not set
        /// </summary>
        public static RobotModel[] EnemyModels = null;
        /// <summary>
        /// Brain models, null if not set 
        /// </summary>
        public static BrainModel<T>[] EnemyBrainModels = null;
        /// <summary>
        /// Position for robot to start
        /// </summary>
        public static float initHeight = MapHeight * 0.25f;
        /// <summary>
        /// Position for robot to start 
        /// </summary>
        public static float initWidth = MapWidth * 0.25f;
        /// <summary>
        /// Radius for starting place of robots
        /// </summary>
        public static float initRadius = 95;
        /// <summary>
        /// Position for enemy robot to start
        /// </summary>
        public static float enemyInitHeight = MapHeight * 0.75f;
        /// <summary>
        /// Position for enemy robot to start
        /// </summary>
        public static float enemyInitWidth = MapWidth * 0.75f;
        /// <summary>
        /// Radius for starting place of enemy robots
        /// </summary>
        public static float enemyInitRadius = 95; 


        /// <summary>
        /// Init positions of vector 
        /// </summary>
        /// <returns></returns>
        public static Vector2[] InitPositionOfRobot()
        {
            List<CircleEntity> placedEntities = new List<CircleEntity>();
            List<Vector2> vectors = new List<Vector2>();

            int maxH = (int)Math.Floor(initHeight + (initRadius - RobotMaxRadius));
            int minH = (int)Math.Floor(initHeight - (initRadius - RobotMaxRadius));

            int maxW = (int)Math.Floor(initWidth + (initRadius - RobotMaxRadius));
            int minW = (int)Math.Floor(initWidth - (initRadius - RobotMaxRadius));

            for (int attemps = 0; attemps < 1000; attemps++)
            {
                for (int amountOfRobots = 0; amountOfRobots < MaxOfAmountRobots; amountOfRobots++)
                {
                    for (int placeRobotAttempts = 0; placeRobotAttempts < 1000; placeRobotAttempts++)
                    {
                        //Create new random position of the robot, inside the initial position 
                        float vH = SupportClasses.RandomNumber.GetRandomInt(minH, maxH);
                        float vW = SupportClasses.RandomNumber.GetRandomInt(minW, maxW);
                        Vector2 middle = new Vector2(vW, vH);
                        ObstacleEntity e = new ObstacleEntity(middle, RobotMaxRadius);

                        bool correct = true;
                        foreach (var p in placedEntities)
                        {
                            if (Map.Colides(e, p))
                            {
                                correct = false;
                                break;
                            }

                        }

                        if (correct)
                        {
                            vectors.Add(e.Middle);
                            placedEntities.Add(e);
                            break;
                        }
                    }
                }
                if (placedEntities.Count == MaxOfAmountRobots)
                    return vectors.ToArray();
            }
            throw new ArgumentOutOfRangeException("Unable to place initial position ");
        }
        /// <summary>
        /// Init position of non evolving group of robots 
        /// </summary>
        /// <returns></returns>
        public static Vector2[] InitPositionOfEnemy()
        {
            List<CircleEntity> placedEntities = new List<CircleEntity>();
            List<Vector2> vectors = new List<Vector2>();

            int maxH = (int)Math.Floor(enemyInitHeight + (enemyInitRadius - RobotMaxRadius));
            int minH = (int)Math.Floor(enemyInitHeight - (enemyInitRadius - RobotMaxRadius));

            int maxW = (int)Math.Floor(enemyInitWidth + (enemyInitRadius - RobotMaxRadius));
            int minW = (int)Math.Floor(enemyInitWidth - (enemyInitRadius - RobotMaxRadius));

            for (int attemps = 0; attemps < 1000; attemps++)
            {
                for (int amountOfRobots = 0; amountOfRobots < MaxOfAmountRobots; amountOfRobots++)
                {
                    for (int placeRobotAttempts = 0; placeRobotAttempts < 1000; placeRobotAttempts++)
                    {
                        //Create new random position of the robot, inside the initial position 
                        float vH = SupportClasses.RandomNumber.GetRandomInt(minH, maxH);
                        float vW = SupportClasses.RandomNumber.GetRandomInt(minW, maxW);
                        Vector2 middle = new Vector2(vW, vH);
                        ObstacleEntity e = new ObstacleEntity(middle, RobotMaxRadius);

                        bool correct = true;
                        foreach (var p in placedEntities)
                        {
                            if (Map.Colides(e, p))
                            {
                                correct = false;
                                break;
                            }

                        }

                        if (correct)
                        {
                            vectors.Add(e.Middle);
                            placedEntities.Add(e);
                            break;
                        }
                    }
                }
                if (placedEntities.Count == MaxOfAmountRobots)
                    return vectors.ToArray();
            }
            throw new ArgumentOutOfRangeException("Unable to place initial position ");
        }
        /// <summary>
        /// Before using any methods of class
        /// </summary>
        public static void SetUpEnemies(RobotModel[] enemyModels, BrainModel<T>[] enemyBrains)
        {
            EnemyModels = enemyModels;
            EnemyBrainModels = enemyBrains;
        }
        /// <summary>
        /// Make empty map
        /// </summary>
        public static Map MakeEmptyMap()
        {
            if (EnemyBrainModels == null)
                throw new NullReferenceException("EnemyBrainModels not set!");
            if (EnemyModels == null)
                throw new NullReferenceException("EnemyModels not set");

            //Prepare objects of map 
            ObstacleEntity obstacle = new ObstacleEntity(Vector2.Zero, 5);
            float tW = (MapWidth / 4) * 3;

            ObstacleEntity initPosition = new ObstacleEntity(new Vector2(initWidth, initHeight), initRadius);
            ObstacleEntity enemyInitPosition = new ObstacleEntity(new Vector2(enemyInitWidth,enemyInitHeight), enemyInitRadius);
            //Generate randomly deployed minerals 
            Classes.Map.Map preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, new List<CircleEntity>() { initPosition,enemyInitPosition });
            List<CircleEntity> obstacles = Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, obstacle, AmountOfObstacles);
           
            var output = new Map(MapHeight, MapWidth, null, obstacles, null, null);
            return output;
        }
        /// <summary>
        /// Create map with given models 
        /// </summary>
        /// <param name="robotModels"></param>
        /// <param name="amountOfRobots"></param>
        /// <returns></returns>
        public static MapModel MakeMapModel(RobotModel[] models)
        {
            if (EnemyBrainModels == null)
                throw new NullReferenceException("EnemyBrainModels not set!");
            if (EnemyModels == null)
                throw new NullReferenceException("EnemyModels not set");

            //Check size & amount 
            int amountOfRobot = 0;
            foreach (var m in models)
            {
                amountOfRobot += m.amount;
                if (m.model.Radius > RobotMaxRadius)
                    throw new ArgumentException("Robot with bigger radius");
                if (amountOfRobot > MaxOfAmountRobots)
                    throw new ArgumentException("More robots than maximum size");
            }
            List<RobotEntity> robots = new List<RobotEntity>();
            foreach (var m in models)
            {
                for (int i = 0; i < m.amount; ++i)
                {
                    robots.Add((RobotEntity)m.model.DeepClone());
                }
            }
            Vector2[] initPos = InitPositionOfRobot();
            for (var index = 0; index < robots.Count; index++)
            {
                var r = robots[index];
                r.MoveTo(initPos[index]);
            }

            //Add enemies to map 
            var enemies = new List<RobotEntity>();
            Vector2[] initEnemyPos = InitPositionOfEnemy();
            Map emptyMap = MakeEmptyMap();
            int freeIndex = 0;
            foreach (var model in EnemyModels)
            {
                IRobotBrain brain = null;
                foreach (var b in EnemyBrainModels)
                {
                    if (b.SuitableRobot(model.model))
                    {
                        brain = b.Brain.GetCleanCopy();
                        break; 
                    }
                }

                if(brain == null)
                    throw new NotImplementedException("Unknown brain for this enemy robot");

                for (int i = 0; i < model.amount; i++)
                {
                    var newEnemy = (RobotEntity) model.model.DeepClone();
                    newEnemy.TeamNumber = 2; 
                    newEnemy.MoveTo(initEnemyPos[freeIndex]);
                    freeIndex++;
                    newEnemy.Brain = brain.GetCleanCopy();
                    
                    enemies.Add(newEnemy);
                }

                
            }

            return new MapModel()
            {
                MapHeight = MapHeight,
                MapWidth = MapWidth,
                ConstRadioSignals = emptyMap.constantRadioSignal,
                FuelEntities = emptyMap.FuelEntities.ToList(),
                PassiveEntities = emptyMap.PasiveEntities.ToList(),
                RobotBodies = robots,
                EnviromentRobots = enemies
            };
        }
        /// <summary>
        /// Make map 
        /// </summary>
        public static Map MakeMap(RobotModel[] models)
        {
            if (EnemyBrainModels == null)
                throw new NullReferenceException("EnemyBrainModels not set!");
            if (EnemyModels == null)
                throw new NullReferenceException("EnemyModels not set");

            return MakeMapModel(models).ConstructMap();
        }

        public static StringBuilder Log()
        {
            throw new NotImplementedException();
        }
    }
    
   
}   