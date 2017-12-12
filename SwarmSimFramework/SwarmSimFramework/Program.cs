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
using SwarmSimFramework.SupportClasses;

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
                    var r = new Random();
                    var O_watch = new Stopwatch();
                    var N_watch = new Stopwatch();
                        for (int i = 0; i < 10000; i++)
                        {
                           
                            HashSet<object> A1 = new HashSet<object>();
                            HashSet<MyObject> A2 = new HashSet<MyObject>();

                            HashSet<object> B1 = new HashSet<object>();
                            HashSet<MyObject> B2 = new HashSet<MyObject>();

                            for (int j = 0; j <r.Next(1000000,100000000) ; j++)
                            {
                                var a = new object();
                                var b = new object(); 
                                var a1 = new MyObject();
                                var b1 = new MyObject();

                                A1.Add(a);
                                A2.Add(a1);

                                B1.Add(b);
                                B2.Add(b1); 
                            }

                            O_watch.Start();
                            A1.UnionWith(B1);
                            O_watch.Stop();

                            N_watch.Start();
                            A2.UnionWith(B2);
                            N_watch.Stop(); 

                            Console.WriteLine("Original solution {0} my solution {1}" ,O_watch.ElapsedMilliseconds,N_watch.ElapsedMilliseconds);
                            N_watch.Reset();
                            O_watch.Reset();
                        }
                        
                       
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
