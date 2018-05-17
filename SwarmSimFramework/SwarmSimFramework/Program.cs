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
                    int amountOfSimulation = 100; 
                    Console.WriteLine("Generating statistics reading map from: {0}", args[1]);
                    var s = TestingBrainReader.ReadTestingBrainFromFile(args[1]);

                    Dictionary<string, double> statistics = new Dictionary<string, double>();
                    for (int j = 0; j < amountOfSimulation; j++)
                    {
                        s.Init();
                        for (int i = 0; i < s.TestingCycle; i++)
                        {
                            //Add function
                            s.MakeStep(() => { return; });
                        }
                        foreach (var item in s.Map.GetStatistics())
                            statistics.SafeIncrementBy(item.Key,item.Value);
                    }

                    foreach (var item in statistics)
                        Console.WriteLine("{0} : {1}",item.Key,item.Value/(double) amountOfSimulation);
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
        }
    }
}
