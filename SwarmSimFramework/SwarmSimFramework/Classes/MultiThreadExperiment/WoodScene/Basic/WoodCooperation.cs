using System.Numerics;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.Robots.WoodRobots;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public class WoodCooperation : WoodExperimentMt
    {
        protected override void Init(string[] nameOfInitialFile)
        {
            WorkingDir = "WoodCooperativeDir";
            PopulationSize = 100;
            NumberOfGenerations = 1000;
            MapIteration = 2000;
            Name = "WoodCooperation";
            Models = new RobotModel[2];
            Models[0] = new RobotModel()
            {
                amount = 5,
                model = new ScoutCutterRobot(new Vector2(0, 0))
            };
            Models[1] = new RobotModel()
            {
               amount = 4,
               model = new WoodWorkerRobot(new Vector2(0,0))
                
            };
            AmountOfTrees = 200;
            AmountOfWood = 0;
            ValueOfDiscoveredTree = 5;
            ValueOfCutWood = 10;
            ValueOfCollision = 0;
            ValueOfContaineredWood = 100;
            ValueOfStockedWood = 1000;
            base.Init(nameOfInitialFile);
        }

    }
}