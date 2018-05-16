using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
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
    public class TestingBrain : IExperiment
    {

        ///Prepare testing map and brains to test
        public TestingBrain(Map.Map Map, BrainModel<IRobotBrain>[] brain, int lengthOfCycle,BrainModel<SingleLayerNeuronNetwork>[] enemyBrain = null)
        {
            TestedBrains = brain;
            this.EnemyBrains = enemyBrain;
            this.Map = Map;
            TestingCycle = lengthOfCycle;
            Finnished = false;
        }
        /// <summary>
        /// Testing brain
        /// </summary>
        public BrainModel<IRobotBrain>[] TestedBrains;
        /// <summary>
        /// Enemy brains
        /// </summary>
        public BrainModel<SingleLayerNeuronNetwork>[] EnemyBrains; 
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
                if (r.TeamNumber == 2)
                {
                    foreach (var b in EnemyBrains)
                    {
                        if (b.SuitableRobot(r))
                        {
                            r.Brain = b.Brain.GetCleanCopy();
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var b in TestedBrains)
                    {
                        if (b.SuitableRobot(r))
                        {
                            r.Brain = b.Brain.GetCleanCopy();
                            break;
                        }
                    }
                }

            }
            MapIterationIndex = 0;
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
                    //Do not change brains of enemy 
                    if (r.Brain != null)
                    {
                        if(r.TeamNumber != 2)
                            throw new Exception("Robot with prepared brain and not enemy robot ");
                        continue;
                    }
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

                if (mapName == "CompetitiveScene")
                    return new TestingBrain(mapModel, brainModels, lengthCycle,
                        CompetitiveScene<SingleLayerNeuronNetwork>.EnemyBrainModels);
                else
                    return new TestingBrain(mapModel, brainModels, lengthCycle);
            }
            catch (Exception e)
            {
                Console.WriteLine("Reading testing brain from file failed: {0}",e.Message);
                return null;
            }
        }
    }
} 