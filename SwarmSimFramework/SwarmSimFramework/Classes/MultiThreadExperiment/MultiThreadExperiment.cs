using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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
        protected const int GraphGenerationIndex = 1;
        /// <summary>
        /// Cycle of loging info
        /// </summary>
        protected const int LogGenerationIndex = 1;
        /// <summary>
        /// Graphs of brains
        /// </summary>
        protected FitPlot[] Graphs = null;


        //GENERATION 
        /// <summary>
        /// Actual generation of brains read only values 
        /// </summary>
        public List<T>[] ActualGeneration;
        /// <summary>
        /// Following generation of brains thread safe for adding
        /// </summary>
        public ConcurrentStack<T>[] FollowingGeneration;

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
        public void Run()
        {
            //Init Threads
            Threads = new Thread[Environment.ProcessorCount];
            Init();
            //Init part
            FollowingGeneration = new ConcurrentStack<T>[ActualGeneration.Length];
            Graphs = new FitPlot[ActualGeneration.Length];
            for (int i = 0; i < ActualGeneration.Length; i++)
            {
                FollowingGeneration[i] = new ConcurrentStack<T>();
                Graphs[i] = new FitPlot(ActualGeneration[i].Count,Models[i].model.Name);
            }
            for (int generationIndex  = 0; generationIndex  < NumberOfGenerations; generationIndex ++)
            {
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
       
                Console.WriteLine(generationIndex +". Generation finnished");
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
                    StreamWriter n = new StreamWriter("bestbrain" + generationIndex + ".json");
                    n.Write(i.BestBrain.SerializeBrain());
                    n.Close();
                }
                //fill graphs
                FillGraphs(generationIndex);
                //Draw graph
                if (generationIndex % GraphGenerationIndex == 0)
                {
                    DrawGraphs(generationIndex);
                }
                //Serialize brains
                if (generationIndex % AllBrainSerializationCycle == 0)
                {
                    for (var index = 0; index < ActualGeneration.Length; index++)
                    {
                        var a = ActualGeneration[index];
                        StreamWriter sw = new StreamWriter( Name + "gen" + generationIndex + "Brain" + index +".json");
                        BrainSerializer.SerializeArray(a.ToArray());
                    }
                }
            }
            
        }
        /// <summary>
        /// Init Experiment fill models, prepare actual generation
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// Threadsafe implementation of evaluating brain
        /// </summary>
        /// <param name="brainIndex"></param>
        protected void SingleBrainEvaluationMt(int brainIndex)
        {
            
            //Create new map, just reading values
            Map.Map map = MapModel.ConstructMap();
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
            GnuPlot.Set("title \"Fitness of " + generationIndex + " generations \"");
            GnuPlot.Set("xlabel \"Index of generation\"");
            GnuPlot.Set("ylabel \"Fitness value\"");
            foreach (var g in Graphs)
                g.PlotGraph();
        }
    }
}