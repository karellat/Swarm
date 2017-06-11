using System.IO.Pipes;
using System.Numerics;
using SwarmSimFramework.Classes.Robots;

namespace SwarmSimFramework.Classes.Experiments.WoodCuttingExperiment
{
    public class WoodCuttingExperimentWorkerWalking: WoodCuttingExperimentWalking
    {
        public WoodCuttingExperimentWorkerWalking()
        {
            AmountOfTrees = 50;
            AmountOfWood = 200;
            NameOfInitFile = "WorkerWalkingInit";
            WorkingDir = "WorkerWalking";
            model = new WorkerCutterRobot(new Vector2(0,0));
            ValueOfDiscoveredTree = 10;
            ValueOfCollision = 0;
            ValueOfCutWood = 1000;
        }
    }
}