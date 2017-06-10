﻿using System;
using System.Collections.Generic;
using System.IO;
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
        public int NumberOfGenerations = 1000;
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
        public string[] InitGenerationFile = null;
        
        
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

        //SERIALIZATION 
        /// <summary>
        /// After this amount cycles all brains are serialized
        /// </summary>
        protected int AllBrainSerializationCycle = 100;
        /// <summary>
        /// Name of file with saved brains
        /// </summary>
        protected string[] NamesOfSerializationFiles;

        //GRAPH
        /// <summary>
        /// Amount of cycles aftert the graphs is drawn
        /// </summary>
        protected int DrawGraphCycle = 10;
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
        public virtual void Init()
        {
            //prepare graph 
            GnuPlot.HoldOn();
        }

        /// <summary>
        /// Make single step of iteration, so map can be drawn after iteration
        /// </summary>
        public void MakeStep()
        {
            
            //If all generation simulated
            if (GenerationIndex == NumberOfGenerations)
            {
                //Draw Graph
                DrawGraphs();
                //Serialize actual generation
                int index = 0;
                foreach (var a in ActualGeneration)
                {
                    StreamWriter sw = new StreamWriter("finalGeneration" + index + ".json");
                    BrainSerializer.SerializeArray(a.ToArray());
                    index++;
                }
                Finnished = true;
                return;
            }

            //If all new brains generated
            if (BrainIndex == PopulationSize)
            {
               GenerationFinnish();
                return;
            }
        //if map iterations ended
        if (IterationIndex == MapIteration)
            {
                MapIterationFinnish();
                return;
            }
            //Make single iteration of map
            MapSingleIteration();
        }
        /// <summary>
        /// Single general iteration of map
        /// </summary>
        protected void MapSingleIteration()
        {
            //Make one iteration of map 
            Map.MakeStep();
            //Virtual call of single iteration of map
            SingleIteration();
            IterationIndex++;
        }
        /// <summary>
        /// General implementation of operations after map iterations ended
        /// </summary>
        protected void MapIterationFinnish()
        {
            //Virtual call of concrete experiment implementation of setting next  brain to evaluate 
            SingleMapSimulation();
            BrainIndex++;
            Map.Reset();
            IterationIndex = 0;

            foreach (var r in Map.Robots)
            {
                for (int i = 0; i < Models.Length; i++)
                {
                    if (r.GetType() == Models[i].GetType())
                        r.Brain = ActualBrains[i];
                }
            }
        }
        /// <summary>
        /// Operation swaping actual and following generation
        /// </summary>
        protected void GenerationFinnish()
        {
            //Serialize if serialization cycle
            if (GenerationIndex % AllBrainSerializationCycle == 0)
            {
                foreach (var a in ActualGeneration)
                {
                    StreamWriter sw = new StreamWriter("Generation" + GenerationIndex + ".json");
                    BrainSerializer.SerializeArray(a.ToArray());
                }
            }

            //fill graphs
            for (var index = 0; index < ActualGeneration.Length; index++)
            {
                var l = ActualGeneration[index];
                foreach (var b in l)
                {
                    //fill suitable graph with given values
                    Graphs[index].AddFitness(b.Fitness, GenerationIndex);
                }
            }
            //Virtual call of specific experiment implemention
            SingleGeneration();

            //Show grahs 
            if (GenerationIndex % DrawGraphCycle == 0)
            {
                DrawGraphs();
                //Clear old values
                foreach (var g in Graphs)
                {
                    g.ClearActualValue();
                }
            }
            BrainIndex = 0;
            GenerationIndex++;
            //Init new generation & change actual generation
            ActualGeneration = FollowingGeneration;
            FollowingGeneration = new List<T>[ActualGeneration.Length];
            for (int i = 0; i < FollowingGeneration.Length; i++)
                FollowingGeneration[i] = new List<T>();

        }
        /// <summary>
        /// One iteration of map 
        /// </summary>
        protected virtual  void SingleIteration()
        {
            StringBuilder newInfo = new StringBuilder("Population Size: ");
            newInfo.Append(PopulationSize);
            newInfo.Append(" Number of generation: ");
            newInfo.Append(NumberOfGenerations);
            newInfo.Append(" Map iteration: ");
            newInfo.Append(MapIteration);
            newInfo.Append(" Actual map iteration: ");
            newInfo.Append(IterationIndex);
            newInfo.Append(" Actual brain from population: ");
            newInfo.Append(BrainIndex);
            newInfo.Append(" Actual generation index: ");
            newInfo.Append(GenerationIndex);
            ExperimentInfo = newInfo;
            //Count fitness for all robot bodies
            foreach (var r in Map.Robots)
            {
                CountIndividualFitness(r);
            }
            
            
        }
        /// <summary>
        /// Draw graphs
        /// </summary>
        protected virtual void DrawGraphs()
        {
           
            GnuPlot.Set("title \"Fitness of " + GenerationIndex + " generations \"");
            GnuPlot.Set("xlabel \"Index of generation\"");
            GnuPlot.Set("ylabel \"Fitness value\"");
            foreach (var g in Graphs)
                g.PlotGraph();
            
        }
        /// <summary>
        /// Count fitness of single robot body
        /// </summary>
        /// <param name="robotEntity"></param>
        protected abstract void CountIndividualFitness(RobotEntity robotEntity);
        /// <summary>
        /// Count fitness of global goals
        /// </summary>
        protected abstract void CountIterationFitness(); 
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
        /// Init basic variables TypeCount,Models,AmountOfRobots, Graph, Generation 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="amount"></param>
        public void InitRobotEntities(RobotEntity[] models, int[] amount)
        {
            TypesCount = models.Length;
            Models = models;
            AmountOfRobots = amount;
            InitGenerationFile = new string[TypesCount]; 
            Graphs  = new FitPlot[TypesCount];
            NamesOfSerializationFiles = new string[TypesCount];
            for (int i = 0; i < Models.Length; i++)
            {
                Graphs[i] = new FitPlot(PopulationSize*NumberOfGenerations,Models[i].GetType().ToString());
                NamesOfSerializationFiles[i] = Models[i].GetType().ToString();
            }
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

        /// <summary>
        /// Clean all values from graph
        /// </summary>
        public void ClearActualValue()
        {
            Xs.Clear();
            Ys.Clear();
        }
    }
}