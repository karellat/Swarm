using System.CodeDom;
using System.Text;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Experiments.FitnessCounters
{
    public class WoodSceneFitnessCounter : IFitnessCounter
    {
        public double ValueOfDiscoveredTree = 0;
        public double ValueOfCollision = 0;
        public double ValueOfCutWood   = 0;
        public double ValueOfStockedWood = 0;
        public double ValueOfContaineredWood = 0;
        public double ValueOfContaineredNoWood = 0; 

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

            //Count wood on the stockPlace
            var stockSignal = map.constantRadioSignal[0];
 
            int woodInContainers = 0;
            int minedWood = 0;
            int otherObjectInContainers = 0; 
            if (ValueOfStockedWood != 0)
            {
                var stockItems = map.CollisionColor(stockSignal);
                minedWood = stockItems.ContainsKey(Entity.EntityColor.WoodColor)
                    ? stockItems[Entity.EntityColor.WoodColor].Amount
                    : 0;
            }
            if (ValueOfContaineredWood != 0)
            {


                // Count wood in containers 

                foreach (var r in map.Robots)
                {
                    foreach (var item in r.ContainerList())
                    {
                        if (item.Color == Entity.EntityColor.WoodColor)
                            woodInContainers++;
                        else
                            otherObjectInContainers++; 
                    }
                }
            }
           

            return (ValueOfContaineredNoWood * otherObjectInContainers) +  
                (DiscoveredTrees * ValueOfDiscoveredTree) + 
                (ValueOfCollision * amountOfCollision) + (CutWoods * ValueOfCutWood) +
                   (ValueOfContaineredWood * woodInContainers) + (ValueOfStockedWood * minedWood);
        }

        public StringBuilder Log()
        {
            StringBuilder s = new StringBuilder();

            s.AppendLine("ValueOfDiscoveredTree = " + ValueOfDiscoveredTree);
            s.AppendLine("ValueOfCollision = " + ValueOfCollision);
            s.AppendLine("ValueOfCutWood = " + ValueOfCutWood);
            s.AppendLine("ValueOfStockedWood = " + ValueOfStockedWood);
            s.AppendLine("ValueOfContaineredWood = " + ValueOfContaineredWood);
            s.AppendLine("ValueOfContaineredNoWood = " + ValueOfContaineredNoWood);

            return s; 
        }
    }
}