using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Interfaces
{
    public interface INewExperiment
    {
        /// <summary>
        /// Size of population 
        /// </summary>
        int  PopulationSize { get; }
        /// <summary>
        /// maximum amount of population
        /// </summary>
        int  MaxPopulation { get; }
        /// <summary>
        /// Amount of robots of given model[]
        /// </summary>
        int[] AmountOfRobots { get; }
        /// <summary>
        /// Max iteration of map for single population
        /// </summary>
        int MapMaxIteration { get; }
        /// <summary>
        /// Height of map
        /// </summary>
        float MapHeight { get;  }
        /// <summary>
        /// Width of map 
        /// </summary>
        float MapWidth { get; }
        /// <summary>
        /// Models of robots 
        /// </summary>
        RobotEntity[] ModelsOfRobots { get; }
    }
}