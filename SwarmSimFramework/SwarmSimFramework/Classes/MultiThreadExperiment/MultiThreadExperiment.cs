using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics; 
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Experiments;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses.AwokeKnowing.GnuplotCSharp;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public abstract class MultiThreadExperiment<T> where T : IRobotBrain
    {
        //BASIC EVOLUTION CHARACTERISTICS
        /// <summary>
        /// Size of population(amount of brains) 
        /// </summary>
        public int PopulationSize = 100;
        /// <summary>
        /// Amount of generation iteration 
        /// </summary>
        public int NumberOfGenerations = 100;
        /// <summary>
        /// Iteration between fitness count
        /// </summary>
        public int MapIteration = 1000;
        /// <summary>
        /// Name of experiment
        /// </summary>
        public string Name;
        /// <summary>
        /// Dir for saving serialization files
        /// </summary>
        public string WorkingDir = "mtdir";

        //ROBOTS & BRAIN FOR THEM
        /// <summary>
        /// Model of testing robots with amount of them 
        /// </summary>
        public RobotModel[] Models;


        //SERIALIZATION 
        /// <summary>
        /// After this amount cycles all brains are serialized
        /// </summary>
        protected int AllBrainSerializationCycle = 100;


        //GRAPH & LOG INFO 
        /// <summary>
        /// Cycle of graph drawing
        /// </summary>
        protected const int GraphGenerationIndex = 10;
        /// <summary>
        /// Cycle of loging info
        /// </summary>
        protected const int LogGenerationIndex = 10;
        /// <summary>
        /// Graphs of brains
        /// </summary>
        protected FitPlot[] Graphs = null;

        //GENERATION 
        /// <summary>
        /// Actual generation of brains read only values 
        /// </summary>
        protected List<T>[] ActualGeneration;
        /// <summary>
        /// Following generation of brains thread safe for adding
        /// </summary>
        protected ConcurrentStack<T>[] FollowingGeneration;

        //MAP STATES 
        /// <summary>
        /// Model of map including models of Entities, Height and Width
        /// </summary>
        protected MapModel MapModel;
        /// <summary>
        /// Threads making evaluating of brains
        /// </summary>
        protected Thread[] Threads;
 
        /// <summary>
        /// If all threads finnished generation
        /// </summary>
        protected bool GenerationFinnished = false;
        /// <summary>
        /// Lock of thread control
        /// </summary>
        protected object ControlLock = new object();

        /// <summary>
        /// Index of first 
        /// </summary>
        protected int FreeBrainIndex = 0;
        public void Run(string[] nameOfInitFile=null)
        {
            //Init Threads
            Threads = new Thread[Environment.ProcessorCount];
            //Init specific experiment
            Init(nameOfInitFile);
            //Init part
            //Log info dir 
            System.IO.Directory.CreateDirectory(WorkingDir);
            FollowingGeneration = new ConcurrentStack<T>[ActualGeneration.Length];
            
            Graphs = new FitPlot[ActualGeneration.Length];
            for (int i = 0; i < ActualGeneration.Length; i++)
            {
                FollowingGeneration[i] = new ConcurrentStack<T>();
                Graphs[i] = new FitPlot(ActualGeneration[i].Count,Models[i].model.Name);
            }
            //Stop watch count 
            var watch = new Stopwatch(); 
            for (int generationIndex  = 0; generationIndex  < NumberOfGenerations; generationIndex ++)
            {
                watch.Reset(); 
                watch.Start();
                GenerationFinnished = false;
                FreeBrainIndex = 0;
                lock(ControlLock)
                {
                    while (!GenerationFinnished)
                    {
                        for (int j = 0; j < Threads.Length; j++)
                        {
                            if (FreeBrainIndex == PopulationSize) break;

                            if (Threads[j] == null || Threads[j].IsAlive == false)
                            {
                                int threadIndex = FreeBrainIndex;
                                Threads[j] = new Thread(() => SingleBrainEvaluationMt(threadIndex));
                                Threads[j].Start();
                                FreeBrainIndex++;
                            }
                        }
                        //Monitor.Wait(ControlLock);
                    }
                }
                //Change generation, clear buffer
                for (int j = 0; j < ActualGeneration.Length; j++)
                {
                    ActualGeneration[j] = new List<T>(FollowingGeneration[j].ToArray());
                    FollowingGeneration[j].Clear();
                }
                watch.Stop(); 
                Console.WriteLine(generationIndex +". Generation finnished, elapsed time " + watch.ElapsedMilliseconds + " miliseconds");
                //Log to console generation 
                if (generationIndex % LogGenerationIndex == 0)
                {
                    //log generation
                    var i = GenerationInfoStruct.GetGenerationInfo(ActualGeneration[0]);
                    StringBuilder sb = new StringBuilder("\tInfo about ");
                    sb.Append(generationIndex);
                    sb.Append(".generation\n");
                    sb.AppendLine("\t\tBest fitness: " + i.FitnessMaximum);
                    sb.AppendLine("\t\tWorst fitness " + i.FitnessMinimum);
                    sb.AppendLine("\t\tAverage fitness: " + i.FitnessAverage);
                    for (int j = 0; j < ActualGeneration.Length; j++)
                    {
                        sb.AppendLine("\t\tBrain " + j + ". " + ActualGeneration[j][i.BestBrainIndex].Log().ToString());
                    }
                    Console.WriteLine(sb.ToString());
                    //Serialize best brain
                    //Serialize brain
                    for (var index = 0; index < ActualGeneration.Length; index++)
                    {
                        var g = ActualGeneration[index];
                        var bestBrain = GenerationInfoStruct.GetGenerationInfo(g);
                        StreamWriter n = new StreamWriter(WorkingDir + "\\"+ generationIndex + "bestbrain" + index+  ".json");
                        n.Write(i.BestBrain.SerializeBrain());
                        n.Close();
                    }
                }
                //fill graphs
                FillGraphs(generationIndex);
                //Draw graph
                if (generationIndex % GraphGenerationIndex == 0)
                {
                    DrawGraphs(generationIndex);
                }
                //Serialize brains
                if (generationIndex % AllBrainSerializationCycle == 0 || generationIndex == NumberOfGenerations-1 )
                {
                    for (var index = 0; index < ActualGeneration.Length; index++)
                    {
                        var a = ActualGeneration[index];
                        StreamWriter sw = new StreamWriter(WorkingDir + "\\" + Name + "gen" + generationIndex + "Brain" + index +".json");
                        sw.Write(BrainSerializer.SerializeArray(a.ToArray()));
                        sw.Close();
                    }
                }
                //If evolution finnished, write file outside of the working dir 
                if(generationIndex == NumberOfGenerations-1)
                {
                    for (var index = 0; index < ActualGeneration.Length; index++)
                    {
                        var a = ActualGeneration[index];
                        StreamWriter sw = new StreamWriter(Name + "Brain" + index + ".json");
                        sw.Write(BrainSerializer.SerializeArray(a.ToArray()));
                        sw.Close();
                    }
                }
            }
            
        }

        /// <summary>
        /// Init Experiment fill models, prepare actual generation
        /// </summary>
        /// <param name="nameOfInitialFile"></param>
        protected abstract void Init(string[] nameOfInitialFile);
        /// <summary>
        /// Threadsafe implementation of evaluating brain
        /// </summary>
        /// <param name="brainIndex"></param>
        protected void SingleBrainEvaluationMt(int brainIndex)
        {
            
            //Create new map, just reading values
            Map.Map map = MapModel.ConstructMap();
            //DEBUG
            foreach (var p in map.PasiveEntities)
            {
                if(p.Discovered)
                    throw new NotImplementedException("Discovered entity");
            }
            //model of brains, only for read
            var modelBrains = new T[ActualGeneration.Length];
            for (int i = 0; i < modelBrains.Length; i++)
            {
                modelBrains[i] = ActualGeneration[i][brainIndex];
            }
            //Create new brains implemented by evolution alg
            var evalBrains = SingleBrainSelection(modelBrains);
            //For model choose suitable brain
            foreach (var r in map.Robots)
            {
                for (int i = 0; i < Models.Length; i++)
                {
                    if (evalBrains[i].SuitableRobot(r))
                        r.Brain = evalBrains[i].Brain.GetCleanCopy();
                }
            }

            for (int i = 0; i < MapIteration; i++)
            {
                map.MakeStep();
            }
            double fitness = CountFitness(map);
            //If created fitness of brain is better
            if (fitness > modelBrains[0].Fitness)
            {
                for (int i = 0; i < evalBrains.Length; i++)
                {
                    //Set fitness to brains
                    evalBrains[i].Brain.Fitness = fitness;
                    FollowingGeneration[i].Push(evalBrains[i].Brain);
                }
            }
            else
            {
                for (int i = 0; i < modelBrains.Length; i++)
                    FollowingGeneration[i].Push(modelBrains[i]);
            }

            //Check if this is the last running thread
            bool lastRunning = true;

            for (int i = 0; i < FollowingGeneration.Length; i++)
            {
                if (FollowingGeneration[i].Count != PopulationSize)
                    lastRunning = false;
            }
            //Set if generation done
            if (lastRunning) GenerationFinnished = true;
            //Wake up the Control Thread
            //lock (ControlLock)
            //{
            //    Monitor.Pulse(ControlLock);
            //}
        }
        /// <summary>
        /// Thread safe brain creation 
        /// </summary>
        /// <returns></returns>
        protected abstract BrainModel<T>[] SingleBrainSelection(T[] evolveBrains);
        /// <summary>
        /// Fitness count after one map simulation
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        protected abstract double CountFitness(Map.Map map);

        //GRAPH Methods
        /// <summary>
        /// Fill graphs 
        /// </summary>
        public void FillGraphs(int generationIndex)
        {
            for (var index = 0; index < ActualGeneration.Length; index++)
            {
                var l = ActualGeneration[index];
                foreach (var b in l)
                {
                    //fill suitable graph with given values
                    Graphs[index].AddFitness(b.Fitness, generationIndex);
                }
            }
        }
        /// <summary>
        /// Draw graphs 
        /// </summary>
        public void DrawGraphs(int generationIndex)
        {
            //Draw graph if possible
            if (Directory.Exists(GnuPlot.PathToGnuplot))
            {
                GnuPlot.Set("title \"Fitness of " + generationIndex + " generations \"");
                GnuPlot.Set("xlabel \"Index of generation\"");
                GnuPlot.Set("ylabel \"Fitness value\"");
                GnuPlot.HoldOn();
                foreach (var g in Graphs)
                    g.PlotGraph();
                GnuPlot.HoldOff();
            }

            //serialize graph 
            foreach (var g in Graphs)
            {
                StreamWriter n = new StreamWriter(WorkingDir + "\\graph" + g.Name+ ".json");
                n.Write(g.SerializeGraph());
                n.Close();
            }
            
        }
    }
}