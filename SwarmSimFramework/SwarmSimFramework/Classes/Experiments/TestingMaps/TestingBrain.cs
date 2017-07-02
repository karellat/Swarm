using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Resources;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;

namespace SwarmSimFramework.Classes.Experiments.TestingMaps
{
    /// <summary>
    /// Prepared maps for testing specific brains
    /// </summary>
    public static class TestingMaps
    {
        /// <summary>
        /// Trees in wood experiment
        /// </summary>
        public const int WoodTreesAmount = 200;
        /// <summary>
        /// Number of wood
        /// </summary>
        public const int WoodWoodAmount = 100;
        /// <summary>
        /// Height of map
        /// </summary>
        public const float WoodMapHeight = 800;
        /// <summary>
        /// Width of map 
        /// </summary>
        public const float WoodMapWidth = 1200;
        public static Map.Map GetWoodMapCuters()
        {
            //Prepare models and fix the initial position
            RawMaterialEntity tree = new RawMaterialEntity(new Vector2(0,0),5,10,10);
            ObstacleEntity initPosition = new ObstacleEntity(new Vector2((WoodMapWidth/2),(WoodMapHeight/2)),20);
            //Generate radomly deployed tree
            Map.Map preparedMap = new Map.Map(WoodMapHeight, WoodMapWidth, null, new List<CircleEntity>() { initPosition });
            List<CircleEntity> trees =
                Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, tree, WoodTreesAmount);
            //set experiment
            RobotEntity robotModel = new ScoutCutterRobot(new Vector2(0, 0));
            List<RobotEntity> robots = new List<RobotEntity>();
            for (int i = 0; i < 5; i++)
            {
               robots.Add((RobotEntity) robotModel.DeepClone());
            }
            //Initial position 
            robots[0].MoveTo(new Vector2(WoodMapWidth / 2, WoodMapHeight / 2));
            robots[1].MoveTo(new Vector2(WoodMapWidth / 2 + 10, WoodMapHeight / 2));
            robots[2].MoveTo(new Vector2(WoodMapWidth / 2, WoodMapHeight / 2 + 10));
            robots[3].MoveTo(new Vector2(WoodMapWidth / 2 - 10, WoodMapHeight / 2));
            robots[4].MoveTo(new Vector2(WoodMapWidth / 2, WoodMapHeight / 2 - 10));

            return new Map.Map(WoodMapHeight,WoodMapWidth,robots,trees,null);
        }
        public static Map.Map GetWoodMapCutersWithMem()
        {
            //Prepare models and fix the initial position
            RawMaterialEntity tree = new RawMaterialEntity(new Vector2(0, 0), 5, 10, 10);
            ObstacleEntity initPosition = new ObstacleEntity(new Vector2((WoodMapWidth / 2), (WoodMapHeight / 2)), 20);
            //Generate radomly deployed tree
            Map.Map preparedMap = new Map.Map(WoodMapHeight, WoodMapWidth, null, new List<CircleEntity>() { initPosition });
            List<CircleEntity> trees =
                Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, tree, WoodTreesAmount);
            //set experiment
            RobotEntity robotModel = new ScoutCutterRobotWithMemory(new Vector2(0, 0));
            List<RobotEntity> robots = new List<RobotEntity>();
            for (int i = 0; i < 5; i++)
            {
                robots.Add((RobotEntity)robotModel.DeepClone());
            }
            //Initial position 
            robots[0].MoveTo(new Vector2(WoodMapWidth / 2, WoodMapHeight / 2));
            robots[1].MoveTo(new Vector2(WoodMapWidth / 2 + 10, WoodMapHeight / 2));
            robots[2].MoveTo(new Vector2(WoodMapWidth / 2, WoodMapHeight / 2 + 10));
            robots[3].MoveTo(new Vector2(WoodMapWidth / 2 - 10, WoodMapHeight / 2));
            robots[4].MoveTo(new Vector2(WoodMapWidth / 2, WoodMapHeight / 2 - 10));

            return new Map.Map(WoodMapHeight, WoodMapWidth, robots, trees, null);
        }

        public static Map.Map GetWoodMapWorkers()
        {
            //Prepare models and fix the initial position
            WoodEntity wood = new WoodEntity(new Vector2(0,0),5,10);
            ObstacleEntity initPosition = new ObstacleEntity(new Vector2((WoodMapWidth / 2), (WoodMapHeight / 2)), 20);
            //Generate radomly deployed tree
            Map.Map preparedMap = new Map.Map(WoodMapHeight, WoodMapWidth, null, new List<CircleEntity>() { initPosition });
            List<CircleEntity> trees =
                Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, wood, WoodTreesAmount);
            //set experiment
            RobotEntity robotModel = new WoodWorkerRobot(new Vector2(0, 0));
            List<RobotEntity> robots = new List<RobotEntity>();
            for (int i = 0; i < 5; i++)
            {
                robots.Add((RobotEntity)robotModel.DeepClone());
            }
            //Initial position 
            robots[0].MoveTo(new Vector2(WoodMapWidth / 2, WoodMapHeight / 2));
            robots[1].MoveTo(new Vector2(WoodMapWidth / 2 + 10, WoodMapHeight / 2));
            robots[2].MoveTo(new Vector2(WoodMapWidth / 2, WoodMapHeight / 2 + 10));
            robots[3].MoveTo(new Vector2(WoodMapWidth / 2 - 10, WoodMapHeight / 2));
            robots[4].MoveTo(new Vector2(WoodMapWidth / 2, WoodMapHeight / 2 - 10));

            return new Map.Map(WoodMapHeight, WoodMapWidth, robots, trees, null);
        }
    }
    public class TestingBrain : IExperiment
    {

        ///Prepare testing map and brains to test
        public TestingBrain(Map.Map Map, BrainModel<IRobotBrain>[] brain, int lengthOfCycle)
        {

            TestedBrains = brain;
            this.Map = Map;
            TestingCycle = lengthOfCycle;
            Finnished = false;
        }
        /// <summary>
        /// Testing brain
        /// </summary>
        public BrainModel<IRobotBrain>[] TestedBrains;
        /// <summary>
        /// Map where the brain is tested
        /// </summary>
        public Map.Map Map { get; }
        /// <summary>
        /// Amount of map iteration
        /// </summary>
        public int TestingCycle;
        /// <summary>
        /// Actual iteration of map
        /// </summary>
        protected int MapIterationIndex;
        /// <summary>
        /// Init Cycle
        /// </summary>
        public void Init()
        {
            GenerationInfo = null;

            FinnishedGeneration = false;
            Map.Reset();
            foreach (var r in Map.Robots)
            {
                foreach (var b in TestedBrains)
                {
                    if(b.SuitableRobot(r))
                        r.Brain = b.Brain.GetCleanCopy();
                }
                
            }
            MapIterationIndex = 0;
        }
        /// <summary>
        /// Repeat map simulation
        /// </summary>
        public void MakeStep()
        {
            if (MapIterationIndex == TestingCycle)
            {
                Map.Reset();
                foreach (var r in Map.Robots)
                {
                    foreach (var b in TestedBrains)
                    {
                        if (b.SuitableRobot(r))
                            r.Brain = b.Brain.GetCleanCopy();
                    }
                }
                MapIterationIndex = 0;
            }
            Map.MakeStep();
            
            ExperimentInfo = new StringBuilder("Max cycle: " + TestingCycle + " Actual map iteration: " + MapIterationIndex);
            MapIterationIndex++;
        }

        public bool Finnished { get; set; }
        protected StringBuilder experimentInfo = new StringBuilder();
        protected Object experimentInfoLock = new object();

        public StringBuilder ExperimentInfo
        {
            get
            {
                lock (experimentInfoLock)
                {
                    return experimentInfo;
                }
            }
            protected set
            {
                lock (experimentInfoLock)
                {
                    experimentInfo = value;
                }
            }
        }

        public StringBuilder GenerationInfo { get; protected set; }
        public bool FinnishedGeneration { get; protected set; }
    }
}