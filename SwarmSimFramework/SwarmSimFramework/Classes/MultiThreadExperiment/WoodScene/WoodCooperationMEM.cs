using System.Numerics;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.Robots;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public class WoodCooperationMEM : WoodExperimentMt
    {
        protected override void Init(string[] nameOfInitialFile)
        {
            WorkingDir = "WoodCooperativeMEMDir";
            PopulationSize = 100;
            NumberOfGenerations = 1000;
            MapIteration = 3000;
            Name = "WoodCooperationMEM";
            Models = new RobotModel[2];
            Models[0] = new RobotModel()
            {
                amount = 5,
                model = new ScoutCutterRobot(new Vector2(0, 0))
            };
            Models[1] = new RobotModel()
            {
                amount = 4,
                model = new WoodWorkerRobot(new Vector2(0, 0))

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
        /// <summary>
        /// Change fitness to calculate 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        protected override double CountFitness(Map.Map map)
        {
            return StockContainerFitness(base.CountFitness(map), map);
        }
    }
}