using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Experiments;
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
        public int PopulationSize = 1000;
        /// <summary>
        /// Amount of generation iteration 
        /// </summary>
        public int NumberOfGenerations = 20;
        /// <summary>
        /// Iteration between fitness count
        /// </summary>
        public int MapIteration = 1000;

        /// <summary>
        /// Model of testing robots
        /// </summary>
        public RobotEntity[] Models = null;


        //SERIALIZATION 
        /// <summary>
        /// After this amount cycles all brains are serialized
        /// </summary>
        protected int AllBrainSerializationCycle = 100;


        //GRAPH & LOG INFO 
        /// <summary>
        /// Cycle of graph drawing
        /// </summary>
        protected const int GraphGenerationIndex = 25;
        /// <summary>
        /// Cycle of loging info
        /// </summary>
        protected const int LogGenerationIndex = 10;
        /// <summary>
        /// Graphs of brains
        /// </summary>
        protected FitPlot[] Graphs = null;

        //MAP CHARACTERISTICS 
        /// <summary>
        /// Height of map
        /// </summary>
        public float MapHeight = 800;
        /// <summary>
        /// Width of map 
        /// </summary>
        public float MapWidth = 1200;

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
        /// Models with localization of robot bodies used for reading only
        /// </summary>
        protected List<RobotEntity> ModelsOfRobots;
        /// <summary>
        /// Models with localization of pasive entities used for reading only 
        /// </summary>
        protected List<CircleEntity> ModelsOfPasiveEntities;
        /// <summary>
        /// Models with localization of fuel entities used for reading only 
        /// </summary>
        protected List<FuelEntity> ModelsOfFuelEntities;
        /// <summary>
        /// Models with localization of radio signals which are permatnent used for reading only
        /// </summary>
        protected List<RadioEntity> PermanentRadioSignals;
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
            for (int i = 0; i < ActualGeneration.Length; i++)
            {
                FollowingGeneration[i] = new ConcurrentStack<T>();
                Graphs[i] = new FitPlot(ActualGeneration[i].Count,Models[i].Name);
            }
            for (int generationIndex  = 0; generationIndex  < NumberOfGenerations; generationIndex ++)
            {
                GenerationFinnished = false;
                lock(ControlLock)
                {
                    while (!GenerationFinnished)
                    {
                        for (int j = 0; j < Threads.Length; j++)
                        {
                            if (FreeBrainIndex == NumberOfGenerations) break;

                            if (Threads[generationIndex ].IsAlive == false)
                            {
                                var threadIndex = FreeBrainIndex;
                                Threads[generationIndex ] = new Thread(() => SingleBrainEvaluationMt(threadIndex));
                                Threads[generationIndex ].Start();
                                FreeBrainIndex++;
                            }
                        }
                        Monitor.Wait(ControlLock);
                    }
                }
                //Change generation
                for (int j = 0; j < ActualGeneration.Length; j++)
                {
                    ActualGeneration[j] = new List<T>(FollowingGeneration[generationIndex].ToArray());
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
                        StreamWriter sw = new StreamWriter("Generation" + generationIndex + "Brain" + index +".json");
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
            //TODO: add radio signals 
            //Create new map, just reading values
            Map.Map map = new Map.Map(MapHeight,MapWidth,ModelsOfRobots,ModelsOfPasiveEntities,ModelsOfFuelEntities);
            //model of brains, only for read
            var modelBrains = new T[ActualGeneration.Length];
            for (int i = 0; i < modelBrains.Length; i++)
            {
                modelBrains[i] = ActualGeneration[i][brainIndex];
            }
            //Create new brains implemented by evolution alg
            var evalBrains = SingleBrainSelection();
            //For model choose suitable brain
            foreach (var r in map.Robots)
            {
                for (int i = 0; i < Models.Length; i++)
                {
                    if (r.GetType() == Models[i].GetType())
                        r.Brain = evalBrains[i];
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
                    evalBrains[i].Fitness = fitness;
                    FollowingGeneration[i].Push(evalBrains[i]);
                }
            }
            else
            {
                for (int i = 0; i < modelBrains.Length; i++)
                {
                    FollowingGeneration[i].Push(modelBrains[i]);
                }
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
            Monitor.Pulse(ControlLock);
        }
        /// <summary>
        /// Thread safe brain creation 
        /// </summary>
        /// <returns></returns>
        protected abstract T[] SingleBrainSelection();
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