using System.Numerics;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.Robots;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public class WoodCuttorWalkWithMem : WoodExperimentMt
    
    {
        protected override void Init()
        {
            Name = "WoodCuttorWalk";
            Models = new RobotModel[1];
            Models[0] = new RobotModel()
            {
                amount = 5,
                model = new ScoutCutterRobotWithMemory(new Vector2(0, 0))
            };
            AmountOfTrees = 200;
            AmountOfWood = 0;
            ValueOfDiscoveredTree = 1000;
            ValueOfCutWood = 1050;
            ValueOfCollision = 0;
            base.Init();
        }

        protected override void Init(string[] nameOfInitialFile)
        {
            Name = "WoodCuttorWalk";
            Models = new RobotModel[1];
            Models[0] = new RobotModel()
            {
                amount = 5,
                model = new ScoutCutterRobotWithMemory(new Vector2(0, 0))
            };
            AmountOfTrees = 200;
            AmountOfWood = 0;
            ValueOfDiscoveredTree = 1000;
            ValueOfCutWood = 1050;
            ValueOfCollision = 0;
            base.Init(nameOfInitialFile);
        }

    }
}