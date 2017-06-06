using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses.AwokeKnowing.GnuplotCSharp;

namespace SwarmSimFramework.Classes.Experiments
{
    public abstract class Experiment<T> : IExperiment where T : IRobotBrain
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
        /// Amount of different types of robots
        /// </summary>
        public int TypesCount;
        /// <summary>
        /// amounts of specific models of robots 
        /// </summary>
        public int[] AmountOfRobots = null;
        /// <summary>
        /// Model of testing robots
        /// </summary>
        public RobotEntity[] Models = null;
        /// <summary>
        /// Path to randomly Generated robots brains of specific model
        /// </summary>
        public string[] initGenerationFile = null;
        
        
        //STATE VARIABLE
        /// <summary>
        /// Actual iteration of map for evaluating individual 
        /// </summary>
        protected int IterationIndex = 0;
        /// <summary>
        /// Actual generation index  
        /// </summary>
        protected int GenerationIndex = 0;
        /// <summary>
        /// Index of the actual evaluating brain
        /// </summary>
        protected int BrainIndex = 0;

        /// <summary>
        /// Actual evaluated brains, index of the brains suits to robot model of same index 
        /// </summary>
        protected T[] ActualBrains = null; 
        
        //GRAPH
        /// <summary>
        /// Amount of cycles aftert the graphs is drawn
        /// </summary>
        protected int DrawGraphCycle = 100;

        /// <summary>
        /// Graph of fitness : index of generation
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
        /// Actual generation of brains
        /// </summary>
        public List<T>[] ActualGeneration;
        /// <summary>
        /// Following generation of brains
        /// </summary>
        public List<T>[] FollowingGeneration; 
        /// <summary>
        /// Map enviroment
        /// </summary>
        public Map.Map Map { get; protected set; }

        /// <summary>
        /// Prepare map, initial generation
        /// </summary>
        public abstract void Init();
        /// <summary>
        /// Make single step of iteration, so map can be drawn after iteration
        /// </summary>
        public void MakeStep()
        {
            //If all generation simulated
            if (GenerationIndex == NumberOfGenerations)
                Finnished = true;
            //If all new brains generated
            if (BrainIndex == PopulationSize - 1)
            {
                //fill graphs
                for (var index = 0; index < ActualGeneration.Length; index++)
                {
                    var l = ActualGeneration[index];
                    foreach (var b in l)
                    {
                        //fill suitable graph with given values
                        Graphs[index].AddFitness(b.Fitness,GenerationIndex);
                    }
                }
                SingleGeneration();

                //Show grahs 
                if (GenerationIndex % DrawGraphCycle == 0)
                {
                   
                    GnuPlot.HoldOn();
                    GnuPlot.Set("title \"Fitness of " + GenerationIndex + " generations \"" );
                    GnuPlot.Set("xlabel \"Index of generation\"");
                    GnuPlot.Set("ylabel \"Fitness value\"");
                    foreach (var g in Graphs)
                        g.PlotGraph();
                    GnuPlot.HoldOff();
                }

                BrainIndex = 0;
                GenerationIndex++;
                //Init new generation & change actual generation
                ActualGeneration = FollowingGeneration;
                FollowingGeneration = new List<T>[ActualGeneration.Length];
                for (int i = 0; i < FollowingGeneration.Length; i++)
                    FollowingGeneration[i] = new List<T>();
            }
            //if map iterations ended
            if (IterationIndex == MapIteration)
            {
                SingleMapSimulation();
                IterationIndex = 0;
                BrainIndex++;
                foreach (var r in Map.Robots)
                {
                    for (int i = 0; i < Models.Length; i++)
                    {
                        if (r.GetType() == Models[i].GetType())
                            r.Brain = ActualBrains[i];
                    }
                }
            }

            //Make one iteration of map 
            Map.MakeStep();
            SingleIteration();
            IterationIndex++;
        }
        /// <summary>
        /// One iteration of map 
        /// </summary>
        protected virtual  void SingleIteration()
        {
            StringBuilder newInfo = new StringBuilder("Actual map iteration" );
            newInfo.Append(IterationIndex);
            newInfo.Append(" Actual brain from population: ");
            newInfo.Append(BrainIndex);
            newInfo.Append("Actual generation index: ");
            newInfo.Append(GenerationIndex);
            ExperimentInfo = newInfo;
            //Count fitness for all robot bodies
            foreach (var r in Map.Robots)
            {
                CountIndividualFitness(r);
            }
        }
        /// <summary>
        /// Count fitness of single robot body
        /// </summary>
        /// <param name="robotEntity"></param>
        protected abstract void CountIndividualFitness(RobotEntity robotEntity);

        /// <summary>
        /// Operations after all map iteration
        /// </summary>
        protected abstract void SingleMapSimulation();

        /// <summary>
        /// Operations after generation finnished
        /// </summary>
        protected abstract void SingleGeneration();

        /// <summary>
        /// If experiment evaluate all generation 
        /// </summary>
        public bool Finnished { get; protected set; }

        //INIT METHODS
        /// <summary>
        /// Init basic variables TypeCount,Models,AmountOfRobots, Graph, Generation 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="amount"></param>
        public void InitRobotEntities(RobotEntity[] models, int[] amount)
        {
            TypesCount = models.Length;
            Models = models;
            AmountOfRobots = amount;
            initGenerationFile = new string[TypesCount]; 
            ActualBrains = new T[TypesCount];
            ActualGeneration = new List<T>[TypesCount];
            FollowingGeneration = new List<T>[TypesCount];
        }

        //LOGING INFO
        /// <summary>
        /// Sigle map iteration info 
        /// </summary>
        public StringBuilder ExperimentInfo {
            get
            {
                lock (_experimentInfoLock)
                {
                    return _experimentInfo;
                }
            }
            protected set
            {
                lock (_experimentInfoLock)
                    _experimentInfo = value;
            }
        }
        /// <summary>
        /// Lock of experiment info
        /// </summary>
        private readonly object _experimentInfoLock = new object();
        /// <summary>
        /// intern implementation of experiment info 
        /// </summary>
        private StringBuilder _experimentInfo = null;
        /// <summary>
        /// Info about finnished generation, deleted after reading
        /// </summary>
        public StringBuilder GenerationInfo {
            get
            {
                lock (_generationInfoLock)
                {
                    var v = _generationInfo;
                    _generationInfo = null;
                    return v;
                }
            }
            protected set
            {
                lock (_generationInfoLock)
                {
                    _generationInfo = value;
                }
            } }
        /// <summary>
        /// Intern implementation of generation info
        /// </summary>
        private StringBuilder _generationInfo;
        /// <summary>
        /// Lock of current 
        /// </summary>
        private readonly object _generationInfoLock = new  object();
        /// <summary>
        /// If generation finnished 
        /// </summary>
        public bool FinnishedGeneration {
            get
            {
                lock (_generationInfoLock)
                {
                    return _generationInfo != null;
                }
            }}
    }

    //Other classes connected to experiment

    /// <summary>
    /// Implement fitness ploting
    /// </summary>
    public class FitPlot
    {
        /// <summary>
        /// Generation index axis
        /// </summary>
        protected List<double> Xs;
        /// <summary>
        /// Fitness axis
        /// </summary>
        protected List<double> Ys;
        /// <summary>
        /// Title of plot
        /// </summary>
        protected string Title; 
        /// <summary>
        /// Create new fitPlot
        /// </summary>
        /// <param name="expectedSize"></param>
        public FitPlot(int expectedSize,string title)
        {
            Xs = new List<double>(expectedSize);
            Ys = new List<double>(expectedSize);
            this.Title = "title \"" + title + "\"";
        }
        /// <summary>
        /// Add new fitness to graph
        /// </summary>
        /// <param name="fitness"></param>
        /// <param name="generationIndex"></param>
        public void AddFitness(double fitness, int generationIndex)
        {
            Xs.Add(generationIndex);
            Ys.Add(fitness);
        }
        /// <summary>
        /// Draw graph by GNUplot
        /// </summary>
        public void PlotGraph()
        {
            SupportClasses.AwokeKnowing.GnuplotCSharp.GnuPlot.Plot(Xs.ToArray(), Ys.ToArray(),Title);
        }
    }
}