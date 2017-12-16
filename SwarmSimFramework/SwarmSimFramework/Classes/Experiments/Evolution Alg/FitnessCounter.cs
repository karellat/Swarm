namespace SwarmSimFramework.Classes.Experiments
{
    public interface IFitnessCounter
    {
        /// <summary>
        /// Get fitness of the map
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        double GetMapFitness(Map.Map map); 

    }
}