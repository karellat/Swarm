using System.Numerics;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Robots;

namespace SwarmSimFramework.Classes.Experiments.WoodCuttingExperiment
{
    public class WoodCuttingExperimentPickUp : WoodCuttingExperimentWalking
    {
        public double ValueOfPickedWood = 1000;
        public WoodCuttingExperimentPickUp()
        {
            AmountOfTrees = 50;
            AmountOfWood = 200;
            NameOfInitFile = "WorkerPickUpInit";
            WorkingDir = "WorkerPickUp";
            model = new WorkerCutterRobot(new Vector2(0, 0));
            ValueOfDiscoveredTree = 0;
            ValueOfCollision = 0;
            ValueOfCutWood = 0;
        }

        protected override double CountBrainFitness()
        {
            double i =  base.CountBrainFitness();
            int pickedUpWood = 0;
            foreach (var r in Map.Robots)
            {
                foreach (var e in r.ContainerList())
                {
                    if (e.Color == Entity.EntityColor.WoodColor)
                        pickedUpWood++;
                }
            }

            return i + (pickedUpWood * ValueOfPickedWood);
        }
    }
}