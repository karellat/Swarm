using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Experiments.FitnessCounters
{
    public class WoodSceneFitnessCounter : IFitnessCounter
    {
        public double ValueOfDiscoveredTree = 10;
        public double ValueOfCollision = 0;
        public double ValueOfCutWood   = 0; 

        public double GetMapFitness(Map.Map map)
        {
            int DiscoveredTrees = 0;
            int CutWoods = 0;
            //Find discovered trees and cut woods
            foreach (var p in map.PasiveEntities)
            {
                if (p.Discovered)
                {
                    if (p.Color == Entity.EntityColor.RawMaterialColor)
                    {
                        DiscoveredTrees++;
                    }
                    else if (p.Color == Entity.EntityColor.WoodColor)
                    {
                        CutWoods++;
                    }
                }
            }
            long amountOfCollision = 0;
            //Find collision
            foreach (var r in map.Robots)
            {
                checked
                {
                    amountOfCollision += r.CollisionDetected;
                }

            }

            return (DiscoveredTrees * ValueOfDiscoveredTree) + (ValueOfCollision * amountOfCollision) + (CutWoods * ValueOfCutWood);
        }
    }
}