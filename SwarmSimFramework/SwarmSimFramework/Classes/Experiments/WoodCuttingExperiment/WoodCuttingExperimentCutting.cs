using System.Numerics;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Classes.Robots.WoodRobots;

namespace SwarmSimFramework.Classes.Experiments.WoodCuttingExperiment
{
    public class WoodCuttingExperimentCutting : WoodCuttingExperimentWalking
    {
        public WoodCuttingExperimentCutting()
        {
            AmountOfTrees = 200;
            AmountOfWood = 0;
            NameOfInitFile = "CuttingExperimentInit";
            WorkingDir = "CuttingExperiment";
            model = new ScoutCutterRobot(new Vector2(0, 0));
            ValueOfDiscoveredTree = 10;
            ValueOfCollision = 0;
            ValueOfCutWood = 1000;
        }
    }
}