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
using Intersection2D;
using SwarmSimFramework.Classes.Experiments.FitnessCounters;
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
                case "-c":
                    {
                        Console.WriteLine("Reading set up from: {0}",args[1]);
                        exp = MTExperimentReader.ExperimentFromConfig(args[1]);
                        break;
                    }
                case "debug":
                    { 
                    BrainModel<SingleLayerNeuronNetwork>[][] brainModels = new BrainModel<SingleLayerNeuronNetwork>[8][];
                    for (int i = 0; i < brainModels.Length; i++)
                    {
                        brainModels[i] = new BrainModel<SingleLayerNeuronNetwork>[2];
                        brainModels[i][0].Robot = new ScoutCutterRobotMem();
                        brainModels[i][0].Brain = SingleLayerNeuronNetwork.GenerateNewRandomNetwork(dimension:
                            new IODimension
                            {
                                Input = brainModels[i][0].Robot.SensorsDimension,
                                Output = brainModels[i][0].Robot.EffectorsDimension
                            });
                        brainModels[i][1].Robot = new WoodWorkerRobotMem();
                        brainModels[i][1].Brain = SingleLayerNeuronNetwork.GenerateNewRandomNetwork(dimension:
                            new IODimension
                            {
                                Input = brainModels[i][1].Robot.SensorsDimension,
                                Output = brainModels[i][1].Robot.EffectorsDimension
                            });
                        }
                    


                   RobotModel[] robotModel = new RobotModel[2];
                    robotModel[0].model = new ScoutCutterRobotMem();
                    robotModel[0].amount = 5;
                    robotModel[1].model = new WoodWorkerRobotMem();
                    robotModel[1].amount = 4;

                    WoodScene.AmountOfTrees = 400;
                    MapModel model = WoodScene.MakeMapModel(robotModel);

                    EvolutionaryStrategies ev = new EvolutionaryStrategies(new WoodSceneFitnessCounter(), model,brainModels);
                    ev.Run();
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
