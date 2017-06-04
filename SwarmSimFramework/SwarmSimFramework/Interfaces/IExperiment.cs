using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SwarmSimFramework.Classes.Map;

namespace SwarmSimFramework.Interfaces
{
    public interface IExperiment
    {
        Map Map { get; }

        void Init();

        void MakeStep();

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
        /// Get basic information about generation of brains 
        /// </summary>
        /// <param name="brains"></param>
        /// <returns></returns>
        public static GenerationInfoStruct GetGenerationInfo<T>(List<T> brains) where T : IRobotBrain
        {
            
            double fitnessAverage = 0;
            double fitnessMinimum = float.NegativeInfinity;
            double fitnessMaximum = float.NegativeInfinity;
            IRobotBrain bestBrain = null; 

            foreach (var b in brains)
            {
                fitnessAverage += b.Fitness;
                if (b.Fitness < fitnessMinimum)
                    fitnessMinimum = b.Fitness;
                if (b.Fitness > fitnessMaximum)
                {
                    fitnessMaximum = b.Fitness;
                    bestBrain = b;
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
                FitnessMinimum = fitnessMinimum
            };
        }
    }
}