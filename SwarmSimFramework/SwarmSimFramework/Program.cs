using SwarmSimFramework.Classes.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SwarmSimFramework.Classes.Experiments;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.MultiThreadExperiment;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.Classes.Robots.MineralRobots;
using SwarmSimFramework.Classes.MultiThreadExperiment.MineralScene;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Intersection2D;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Experiments.FitnessCounters;
using SwarmSimFramework.Classes.Experiments.TestingMaps;
using SwarmSimFramework.Classes.Robots.WoodRobots;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework
{
    class Program
    {

        static void Main(string[] args)
        {
            MultiThreadExperimentClasicApproach<SingleLayerNeuronNetwork> exp;
            //Selection of experiments args[0]
            if (args.Length < 2  && args[0] != "debug")
            {
               Console.WriteLine("Wrong parameters!");
                Console.ReadLine();
                return;
            }
            switch (args[0])
            {
                case "-stats":
                {
                    Console.WriteLine("Generating statistics reading map from: {0}", args[1]);
                    var s = TestingBrainReader.ReadTestingBrainFromFile(args[1]);
                    s.Init();
                    for (int i = 0; i < s.TestingCycle; i++)
                    {
                        //Add function
                        s.MakeStep(()=> { return;  });
                    }
                    foreach (var item in s.Map.GetStatistics())
                        Console.WriteLine("{0} : {1}",item.Key,item.Value);
                    return;
                }
                case "-de":
                    {
                        Console.WriteLine("Reading set up from: {0}",args[1]);
                        exp = MTExperimentReader.ExperimentFromConfig(args[1]);
                        break;
                    }
                case "-es":
                    {
                        Console.WriteLine("Reading set up from: {0}",args[1]);
                        EvolutionaryStrategies es = ESReader.ReadFrom(args[1]); 
                        es.Run();
                        return;
                    }
                case "-fb":
                {
                        IFitnessCounter counter;
                        Dictionary<string, string> fSettings; 

                        if (args[2].StartsWith("-f"))
                             fSettings = SettingsReader.GetSettingsFromFile(args[2].Substring(2));
                        else
                            throw new NotImplementedException();
                        
                        var mapSettings = SettingsReader.GetSettingsFromFile(args[1].Substring(2));
                        if (args[1].StartsWith("-w"))
                        {
                            counter = FitnessCounterReader.GetCounter(FitnessCounterReader.Scene.Wood, fSettings);

                            RobotModel[] robots; 
                            if (args[3].StartsWith("-") && args[3].Length > 1)
                            {
                                robots = new RobotModel[args[3].Length-1];
                                if (args[3].Contains("c"))
                                    robots[0] = new RobotModel()
                                    {
                                        amount = 5,
                                        model = new ScoutCutterRobotMem()
                                    };
                                if (args[3].Contains("w"))
                                {
                                    robots[robots.Length - 1] = new RobotModel()
                                    {
                                        amount = 4,
                                        model = new WoodWorkerRobotMem()
                                    };
                                }

                                List<BrainModel<SingleLayerNeuronNetwork>[]> brains = new List<BrainModel<SingleLayerNeuronNetwork>[]>();
                                for (int i = 4; i < args.Length; i++)
                                {
                                    var brainFiles = args[i].Split(new[] { '#' });

                                    if (brainFiles.Length != robots.Length) throw new ArgumentException();

                                    GetBrains(brainFiles, robots).ForEach(x => brains.Add(x));
                                }

                                MapReader.SetMapValues(MapReader.Scene.Wood,mapSettings);
                                FitnessBenchmark benchmark = new FitnessBenchmark(FitnessBenchmark.Scene.Wood,counter,robots,brains);
                                ;
                                int index = 0;
                                var benchmarkedList = benchmark.GetSortedBrainsByBenchmark(); 
                                foreach (var b in benchmarkedList.First().Value)
                                {
                                    string json = JsonConvert.SerializeObject(
                                       b.Brain , BrainModelsSerializer.JsonSettings);
                                    StreamWriter file = new StreamWriter("benchmarkBrain" + index +".json");
                                    file.Write(json);
                                    file.Close();
                                    index++;
                                }
                                Console.WriteLine("Best fitness: {0}", benchmarkedList.First().Key);


                             }
                            else
                                throw new ArgumentException();
                        }
                        else if (args[1].StartsWith("-c"))
                        {
                            counter = FitnessCounterReader.GetCounter(FitnessCounterReader.Scene.Competive, fSettings);

                        }
                        else if (args[1].StartsWith("-m"))
                        {
                            counter = FitnessCounterReader.GetCounter(FitnessCounterReader.Scene.Mineral, fSettings);

                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        return;
                    }
                case "debug":
                {
                    Console.WriteLine(args[1]);    
                    
                  //TestingBrain experiment = new TestingBrain();
                    return; 
                }
                case "D":
                {
                    for (int i = 0; i < args.Length -1 ; i++)
                    {
                        StreamReader sr = new StreamReader(args[i + 1]);
                        SingleLayerNeuronNetwork[] sln =
                            BrainSerializer.DeserializeArray<SingleLayerNeuronNetwork>(sr.ReadToEnd());
                            sr.Close();
                        var j = GenerationInfoStruct.GetGenerationInfo(sln.ToList());
                            StreamWriter sw = new StreamWriter("bestBrain" + i  + ".json");
                            sw.Write(j.BestBrain.SerializeBrain());
                            sw.Close();
                            Console.WriteLine(j.BestBrainInfo);
                    }
                    return; 
                }
                default:
                {
                    Console.WriteLine("Wrong format of parameters!");
                    Console.ReadLine();
                    return;
                }
            }
            if (exp == null)
            {
                Console.WriteLine("Wrong format of parameters!");
                Console.ReadLine();
                return;
            }
            else
            {

              Console.WriteLine("Basic info about experiment");
              Console.WriteLine("\t Name:{0}", exp.Name);
              Console.WriteLine("\t Map iterations:{0} Number of generations: {1} Population size: {2} ",exp.MapIteration,exp.NumberOfGenerations,exp.PopulationSize);
              if(args.Length > 2)
                    exp.Run(args.Skip(2).ToArray());
              else
                exp.Run();
              Console.WriteLine("Simulation finnished");
                
            }
#if DEBUG
            //Console.ReadLine();
#endif 
        }

        private static List<BrainModel<SingleLayerNeuronNetwork>[]> GetBrains(string[] brainFiles,RobotModel[] robots )
        {
            var output = new List<BrainModel<SingleLayerNeuronNetwork>[]>();
            var brainModels = new List<SingleLayerNeuronNetwork>[robots.Length];
            for (int i = 0; i < brainModels.Length; i++)
                brainModels[i] = new List<SingleLayerNeuronNetwork>();


            for (int i = 0; i < brainFiles.Length; i++)
            {
                FileAttributes attr = File.GetAttributes(brainFiles[i]);
               
                //detect whether its a directory or file
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var files = Directory.GetFiles(brainFiles[i]);
                    var list = new List<BrainModel<SingleLayerNeuronNetwork>>();
                    foreach (var f in files)
                        GetBrains(new []{f}, new[] {robots[i]}).ForEach(x => list.Add(x[0]));

                    list.ForEach(x => brainModels[i].Add(x.Brain));
                }
                else
                {
                    StreamReader file = new StreamReader(brainFiles[i]);
                    string json = file.ReadToEnd(); 
                    file.Close();

                    //Try to read as a single brain
                    SingleLayerNeuronNetwork brain; 
                    try
                    {
                        brain = JsonConvert.DeserializeObject<SingleLayerNeuronNetwork>(json);
                    }
                    catch (Exception e)
                    {
                        brain = null;
                    }
                    
               
                    if (brain == null)
                    {
                        SingleLayerNeuronNetwork[] brains = BrainSerializer.DeserializeArray<SingleLayerNeuronNetwork>(json);
                        foreach (var b in brains)
                            brainModels[i].Add(b);
                    }
                    else
                    {
                       brainModels[i].Add(brain);
                    }
                }
            }

            
            for (int i = 0; i < brainModels[0].Count; i++)
            {
                var couple = new BrainModel<SingleLayerNeuronNetwork>[robots.Length];

                for (int j = 0; j < brainModels.Length; j++)
                {
                    var m = new BrainModel<SingleLayerNeuronNetwork>()
                    {
                        Brain = brainModels[j][i],
                        Robot = robots[j].model
                    };
                    Debug.Assert(m.SuitableBrain(m.Brain) && m.SuitableRobot(m.Robot));
                    couple[j] = m;
                }
                output.Add(couple);
            }

            return output;
        }

        private static MultiThreadExperimentClasicApproach<SingleLayerNeuronNetwork> WoodSceneSelection(string index)
        {
            switch (index)
            {
                case "0":
                {
                    return new WoodCuttorWalk();
                }
                case "0M":
                {
                    return new WoodCuttorWalkWithMem();
                }
                case "1":
                {
                    return new WoodCuttorCut();
                }
                case "1M":
                {
                    return new WoodCuttorCutMEM();
                }
                case "2":
                {
                    return new WoodWorkerWalk();
                }
                case "2M":
                {
                    return new WoodWorkerWalkMem();
                }
                case "3":
                {
                    return new WoodWorkerPickUp();
                }
                case "3M":
                {
                    return new WoodWorkerPickUpMem();
                }
                case "4":
                {
                    return new WoodCooperation();
                }
                case "4M":
                {
                    return new WoodCooperationMEM();
                }
                default:
                    return null;



            }
        }
    }

    public static class HashSetsEx
    {
        public static void MyMerge(HashSet<object> set, HashSet<object> mergingSet)
        {
            foreach (var i in mergingSet)
                if (!set.Contains(i))
                    set.Add(i);
        }
    }

    public class MyObject
    {
        public static int GeneralID = 0;
        public int id; 
        public MyObject()
        {
            id = GeneralID;
            checked
            {
                GeneralID++;
            }
            
        }
        public override int GetHashCode()
        {
            return id; 
        }
    }

}
