using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SwarmSimFramework.Classes.MultiThreadExperiment.MineralScene
{
    class MineralScoutWalkMem : MineralExperimentMT
    {
        protected override void Init(string[] nameOfInitialFile)
        {
            WorkingDir = "MineralScoutWalkMem";
            PopulationSize = 1000;
            NumberOfGenerations = 1000;
            MapIteration = 2000;
            Name = "MineralScoutWalkMem";
            Models = new Map.RobotModel[1];
            Models[0] = new Map.RobotModel()
            {
                amount = 5,
                model = new Robots.MineralRobots.ScoutRobotMem(new Vector2(0, 0), 1000)
            };
            AmountOfMineral = 200;
            AmountOfObstacle = 100;
            ValueOfDiscoveredMineral = 100;
            ValueOfRefactoredFuel = 0;
            ValueOfStockedMineral = 0;
            ValueOfStockedMineral = 0;
            base.Init(nameOfInitialFile);
        }
    }
}
