using System.Numerics;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Classes.Robots.WoodRobots;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public class WoodCuttorCutMEM : WoodExperimentMt

    {
        protected override void Init(string[] nameOfInitialFile)
        {
            WorkingDir = "WoodCuttorMemCut";
            PopulationSize = 1000;
            NumberOfGenerations = 1000;
            MapIteration = 2000;
            Name = "WoodCuttorMemCut";
            Models = new RobotModel[1];
            Models[0] = new RobotModel()
            {
                amount = 5,
                model = new ScoutCutterRobotWithMemory(new Vector2(0, 0))
            };
            AmountOfTrees = 200;
            AmountOfWood = 0;
            ValueOfDiscoveredTree = 10;
            ValueOfCutWood = 1000;
            ValueOfCollision = 0;
            base.Init(nameOfInitialFile);
        }
    }
}