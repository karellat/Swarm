﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Interfaces;

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
        protected int IterationIndex;
        /// <summary>
        /// Actual generation index  
        /// </summary>
        protected int GenerationIndex;
        /// <summary>
        /// Index of the actual evaluating brain
        /// </summary>
        protected int BrainIndex; 
        
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
        public Map.Map Map { get; }

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
                SingleGeneration();
            }
            //if map iteration ended
            if (IterationIndex == MapIteration)
            {
                SingleMapSimulation();
            }

            //Make one iteration of map 
            Map.MakeStep();
            IterationIndex++;
            SingleIteration();
        }

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
}