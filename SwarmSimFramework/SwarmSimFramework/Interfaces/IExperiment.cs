﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Policy;
using System.Text;
using SwarmSimFramework.Classes.Map;

namespace SwarmSimFramework.Interfaces
{
    public interface IExperiment
    {
        Map Map { get; }

        void Init();

        void MakeStep(Action func);

        bool Finnished { get; }
        /// <summary>
        /// Thread safe operation for reading metainfos
        /// </summary>
        StringBuilder ExperimentInfo { get; }
        /// <summary>
        /// Thread safe info about sigle generation: 
        /// </summary>
        StringBuilder GenerationInfo { get; }
        /// <summary>
        /// info about generation 
        /// </summary>
        bool FinnishedGeneration { get; }
    }
    /// <summary>
    /// Basic information about single generation
    /// </summary>
    public struct GenerationInfoStruct
    {
        /// <summary>
        /// Average above all brains of generation
        /// </summary>
        public double FitnessAverage;
        /// <summary>
        /// Minimum fitness of brains
        /// </summary>
        public double FitnessMinimum;
        /// <summary>
        /// Maximum fitness of brains
        /// </summary>
        public double FitnessMaximum;
        /// <summary>
        /// Best brain logging info
        /// </summary>
        public string BestBrainInfo;
        /// <summary>
        /// Best brain
        /// </summary>
        public IRobotBrain BestBrain;
        /// <summary>
        /// Best brain index
        /// </summary>
        public int BestBrainIndex; 

        /// <summary>
        /// Get basic information about generation of brains 
        /// </summary>
        /// <param name="brains"></param>
        /// <returns></returns>
        public static GenerationInfoStruct GetGenerationInfo<T>(List<T> brains) where T : IRobotBrain
        {
            
            double fitnessAverage = 0;
            double fitnessMinimum = float.PositiveInfinity;
            double fitnessMaximum = float.NegativeInfinity;
            IRobotBrain bestBrain = null;
            int brainIndex = -1;

            for (var index = 0; index < brains.Count; index++)
            {
                var b = brains[index];
                fitnessAverage += b.Fitness;
                if (b.Fitness < fitnessMinimum)
                    fitnessMinimum = b.Fitness;
                if (b.Fitness > fitnessMaximum)
                {
                    fitnessMaximum = b.Fitness;
                    bestBrain = b;
                    brainIndex = index;
                }
            }
            string info = "";
            if (bestBrain != null)
                 info = bestBrain.Log().ToString();
            return new GenerationInfoStruct()
            {
                BestBrainInfo = info,
                FitnessAverage = fitnessAverage / (double) brains.Count,
                FitnessMaximum = fitnessMaximum,
                FitnessMinimum = fitnessMinimum,
                BestBrain = bestBrain,
                BestBrainIndex =  brainIndex
            };
        }
    }
}