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

namespace SwarmSimFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            MultiThreadExperiment<SingleLayerNeuronNetwork> exp;
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
                        var A = new Vector2(7.17f,797.34f);
                        var B = new Vector2(-18.31f,813.17f);

                        var P1 = new Vector2(1061.67f,142.25f);
                        var P2 = new Vector2(1069.7f,137.26f);

                        var p1 = Intersections.MyExtensions.PointOnLine(A, B, P1);
                        var p2 = Intersections.MyExtensions.PointOnLine(A, B, P2);
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

        private static MultiThreadExperiment<SingleLayerNeuronNetwork> WoodSceneSelection(string index)
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
}
