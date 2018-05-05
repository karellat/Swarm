using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Resources;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots.WoodRobots;
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
            RobotEntity robotModel = new ScoutCutterRobotMem(new Vector2(0, 0));
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
                //do not give brains to enemy robots
                if (r.Brain != null)
                {
                    Debug.Assert(r.TeamNumber == 2);
                    continue;
                }   

                foreach (var b in TestedBrains)
                {
                    if(b.SuitableRobot(r))
                        r.Brain = b.Brain.GetCleanCopy();
                }
                
            }
            MapIterationIndex = 0;
            Map.RotateRobotsRandomly();
        }
        /// <summary>
        /// Repeat map simulation
        /// </summary>
        public void MakeStep(Action func)
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
            Map.MakeStepwithFnc(func);
            
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

    public static class TestingBrainReader
    {
        private enum ReaderState
        {
            Robots, 
            Stats, 
            Map,
            Idle
        } 

        public static TestingBrain ReadTestingBrainFromFile(string path)
        {
            try
            {

                StreamReader streamReader = new StreamReader(path);
                string mapName = streamReader.ReadLine();

                Dictionary<string, string> mapFields = new Dictionary<string, string>();
                Dictionary<string, string> robotFields = new Dictionary<string, string>();
                Dictionary<string, string> statsFields = new Dictionary<string, string>();

                ReaderState state = ReaderState.Idle;
                //Reading from file
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line == "")
                        continue;
                    else if (line.StartsWith("#"))
                    {
   
                        if (line.StartsWith("#MAP SETTINGS"))
                            state = ReaderState.Map;
                        else if (line.StartsWith("#ROBOT SETTINGS"))
                            state = ReaderState.Robots;
                        else if (line.StartsWith("#STATS SETTINGS"))
                            state = ReaderState.Stats;
                        else
                            throw new NotImplementedException("Reading unknown state");

                        continue;
                    }

                    switch (state)
                    {
                        case ReaderState.Robots:
                            {
                                robotFields[line.Split(new[] { ':' })[0]] = line.Split(new[] { ':' })[1];
                                break;
                            }

                        case ReaderState.Stats:
                            {
                                statsFields[line.Split(new[] { ':' })[0]] = line.Split(new[] { ':' })[1];
                                break;
                            }
                        case ReaderState.Map:
                            {
                                mapFields[line.Split(new[] { ':' })[0]] = line.Split(new[] { ':' })[1];
                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException("Invalid state");

                            }

                    }
                }
                //Set map
                switch (mapName)
                {
                    case "WoodScene":
                    {
                        MapReader.SetMapValues(MapReader.Scene.Wood,mapFields);
                        break; 
                    }
                    case "CompetitiveScene":
                    {
                        MapReader.SetMapValues(MapReader.Scene.Competitive,mapFields);
                        break;
                    }
                    case "MineralScene":
                    {
                        MapReader.SetMapValues(MapReader.Scene.Mineral,mapFields);
                        break;
                    }
                    default:
                    {
                        throw new Exception("Unknown name of the map: " + mapName);
                    }
                }
                //Set up stats
                var lengthCycle = int.Parse(statsFields["lengthCycle"]);
                //Prepare robots
                BrainModel<SingleLayerNeuronNetwork>[] _brainModels = (BrainModel<SingleLayerNeuronNetwork>[])
                    MapReader.ParseExperimentValue(robotFields["brainModels"],typeof(BrainModel<SingleLayerNeuronNetwork>[]));
                //change brain model to interface
                BrainModel<IRobotBrain>[] brainModels = new BrainModel<IRobotBrain>[_brainModels.Length];
                for (int i = 0; i < brainModels.Length; i++)
                {
                    brainModels[i] = new BrainModel<IRobotBrain>()
                    {
                        Brain = _brainModels[i].Brain,
                        Robot = _brainModels[i].Robot

                    };
                }

                RobotModel[] robotModels =
                    (RobotModel[]) MapReader.ParseExperimentValue(robotFields["robotModels"], typeof(RobotModel[]));
                //Prepare map
                Map.Map mapModel; 
                switch (mapName)
                {
                    case "WoodScene":
                    {
                        mapModel = WoodScene.MakeMap(robotModels);
                        break;
                    }
                    case "CompetitiveScene":
                    {
                        mapModel = CompetitiveScene<SingleLayerNeuronNetwork>.MakeMap(robotModels);
                        break;
                    }
                    case "MineralScene":
                    {
                        mapModel = MineralScene.MakeMap(robotModels);
                        break;
                    }
                    default:
                    {
                        throw new Exception("Unknown name of the map: " + mapName);
                    }
                }

                return new TestingBrain(mapModel,brainModels, lengthCycle);
            }
            catch (Exception e)
            {
                Console.WriteLine("Reading testing brain from file failed: {0}",e.Message);
                return null;
            }
        }
    }
} 