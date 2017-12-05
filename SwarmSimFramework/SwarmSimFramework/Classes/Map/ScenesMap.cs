﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Numerics;
using SwarmSimFramework.Classes.Entities;
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
                rb.Add(ev);
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
        public static int AmountOfTrees;

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
            ObstacleEntity initPosition = new ObstacleEntity(new Vector2(MapWidth / 2, MapHeight / 2), 30);
            //Generate randomly deployed tree
            Classes.Map.Map preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, new List<CircleEntity>() { initPosition });
            List<CircleEntity> trees =
                Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, tree, AmountOfTrees);
            var tp = trees.ToList();
            tp.Add(initPosition);
            preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, tp);
            List<CircleEntity> woods = Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, wood, AmountOfWoods);
            List<CircleEntity> passive = woods;

            List<RadioEntity> constSignals = new List<RadioEntity>(new []{new RadioEntity(new Vector2(MapWidth/2,MapHeight/2),50,0)});
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
                FuelEntities = emptyMap.FuelEntities.list,
                PassiveEntities = emptyMap.PasiveEntities.list,
                RobotBodies = robots,
                EnviromentRobots = new List<RobotEntity>(0)
            };
        }

        public static Map MakeMap(RobotModel[] models)
        {
            return MakeMapModel(models).ConstructMap();
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
        public static float RobotMaxRadius = 5;
        /// <summary>
        /// Init positions of vector 
        /// </summary>
        /// <returns></returns>
        public static Vector2[] InitPositionOfRobot()
        {
            List<Vector2> vectors = new List<Vector2>();
            float sW = MapWidth / 2 + 44;
            float sH = MapHeight / 2 + 44;
            for (int i = 0; i < 9; i++)
            {
                var nW = sW - 11 * i;
                var nH = sH - 11 * i;
                if (nW == MapWidth / 2 && nH == MapHeight / 2)
                {
                    vectors.Add(new Vector2(nW, nH));
                }
                else
                {
                    vectors.Add(new Vector2(nW, MapHeight / 2));
                    vectors.Add(new Vector2(MapWidth / 2, nH));
                }
            }
            return vectors.ToArray();
        }

        public static Map MakeEmptyMap()
        {
            //Prepare objects of map 
            RawMaterialEntity mineral = new RawMaterialEntity(new Vector2(0, 0), 5, 100, 3);
            ObstacleEntity obstacle = new ObstacleEntity(Vector2.Zero, 5);

            ObstacleEntity initPosition = new ObstacleEntity(new Vector2(MapWidth / 2, MapHeight / 2), 50);
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
                    
            return new Map(MapHeight, MapWidth, null, passive, fuels, null);

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
                FuelEntities = emptyMap.FuelEntities.list,
                PassiveEntities = emptyMap.PasiveEntities.list,
                RobotBodies = robots,
                EnviromentRobots = new List<RobotEntity>(0)
            };
        }

        public static Map MakeMap(RobotModel[] models)
        {
            return MakeMapModel(models).ConstructMap();
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
        public static float RobotMaxRadius = 5;

        public static RobotModel[] enemyModels = new RobotModel[0] { };

        public static BrainModel<T>[] EnemyBrainModels = new BrainModel<T>[0];

        /// <summary>
        /// Init positions of vector 
        /// </summary>
        /// <returns></returns>
        public static Vector2[] InitPositionOfRobot()
        {
            List<Vector2> vectors = new List<Vector2>();
            float sW = MapWidth / 4 + 44;
            float sH = MapHeight / 2 + 44;
            for (int i = 0; i < 9; i++)
            {
                var nW = sW - 11 * i;
                var nH = sH - 11 * i;
                if (nW == MapWidth / 4 && nH == MapHeight / 2)
                {
                    vectors.Add(new Vector2(nW, nH));
                }
                else
                {
                    vectors.Add(new Vector2(nW, MapHeight / 2));
                    vectors.Add(new Vector2(MapWidth / 4, nH));
                }
            }
            return vectors.ToArray();
        }
        /// <summary>
        /// Init position of non evolving group of robots 
        /// </summary>
        /// <returns></returns>
        public static Vector2[] InitPositionOfEnemy()
        {
            List<Vector2> vectors = new List<Vector2>();
            float tW = (MapWidth / 4) * 3;
            float sW = tW + 44;
            float sH = MapHeight / 2 + 44;
            for (int i = 0; i < 9; i++)
            {
                var nW = sW - 11 * i;
                var nH = sH - 11 * i;
                if (nW == tW && nH == MapHeight / 2)
                {
                    vectors.Add(new Vector2(nW, nH));
                }
                else
                {
                    vectors.Add(new Vector2(nW, MapHeight / 2));
                    vectors.Add(new Vector2(tW, nH));
                }
            }
            return vectors.ToArray();
        }
        public static Map MakeEmptyMap()
        {
            //Prepare objects of map 
            ObstacleEntity obstacle = new ObstacleEntity(Vector2.Zero, 5);
            float tW = (MapWidth / 4) * 3;

            ObstacleEntity initPosition = new ObstacleEntity(new Vector2(MapWidth / 4, MapHeight / 2), 50);
            ObstacleEntity enemyInitPosition = new ObstacleEntity(new Vector2(tW,MapHeight/2), 50);
            //Generate randomly deployed minerals 
            Classes.Map.Map preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, new List<CircleEntity>() { initPosition,enemyInitPosition });
            List<CircleEntity> obstacles = Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, obstacle, AmountOfObstacles);

            return new Map(MapHeight, MapWidth, null, obstacles, null, null);

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

            //Add enemies to map 
            var enemies = new List<RobotEntity>();
            Vector2[] initEnemyPos = InitPositionOfEnemy();
            Map emptyMap = MakeEmptyMap();
            int freeIndex = 0;
            foreach (var model in enemyModels)
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
                    throw new NotImplementedException("Unknown brain for this robot");

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
                FuelEntities = emptyMap.FuelEntities.list,
                PassiveEntities = emptyMap.PasiveEntities.list,
                RobotBodies = robots,
                EnviromentRobots = enemies
            };
        }

        public static Map MakeMap(RobotModel[] models)
        {
            return MakeMapModel(models).ConstructMap();
        }

    }
}   