using System.Text;
using SwarmSimFramework.Interfaces;

namespace SwarmSimFramework.Classes.Experiments.WoodCuttingExperiment
{
    public class WoodCuttingExperimentWalkingExperiment : IExperiment
    {
        public static float MapHeight = 800;
        public static float MapWidth = 600;
        public Map.Map Map { get; }

        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void MakeStep()
        {
            throw new System.NotImplementedException();
        }

        public bool Finnished { get; protected set; }

        public StringBuilder ExperimentInfo { get; }
        public StringBuilder GenerationInfo { get; }
        public bool FinnishedGeneration { get; }
    }
}