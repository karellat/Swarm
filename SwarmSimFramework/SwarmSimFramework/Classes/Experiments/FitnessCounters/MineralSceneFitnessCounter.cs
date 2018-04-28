using System.Text;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Experiments.FitnessCounters
{

    public class MineralSceneFitnessCounter : IFitnessCounter
    {
        public double ValueOfDiscoveredMineral = 0.0;
        public double ValueOfStockedMineral = 0.0;
        public double ValueOfRefactoredFuel = 0.0;
        public double ValueOfRemainingFuel = 0.0;
        public double ValueOfCollisions = 0.0;
        public double GetMapFitness(Map.Map map)
        {
            int discoveredMineral = 0;
            int fuelInMap = 0;
            int deadRobots = 0;
            long collisions = 0;
            //discovered minerals
            foreach (var p in map.PasiveEntities)
            {
                if (p.Discovered && p.Color == Entity.EntityColor.RawMaterialColor)
                    discoveredMineral++;
            }

            //count fuel 
            foreach (var f in map.FuelEntities)
                fuelInMap++;

            //stock fuels 
            int mineralInContainer = 0;
            int fuelInContainers = 0;
            double fuelInTanks = 0;
            foreach (var r in map.Robots)
            {
                collisions += r.CollisionDetected;
                //Count dead robots
                if (!r.Alive)
                {
                    deadRobots++;
                    continue;
                }

                //Count remaining fuel 
                fuelInTanks += r.FuelAmount;


                foreach (var item in r.ContainerList())
                {
                    if (item.Color == Entity.EntityColor.FuelColor)
                        fuelInContainers++;
                    if (item.Color == Entity.EntityColor.RawMaterialColor)
                        mineralInContainer++;
                }
            }
            return ValueOfCollisions * collisions + discoveredMineral * ValueOfDiscoveredMineral + mineralInContainer * ValueOfStockedMineral +
                   (fuelInContainers + fuelInMap) * ValueOfRefactoredFuel + fuelInTanks * ValueOfRemainingFuel;

        }

        public StringBuilder Log()
        {
            var s = new StringBuilder();
            s.AppendLine("ValueOfDiscoveredMineral = " + ValueOfDiscoveredMineral);
            s.AppendLine("ValueOfCollisions = " + ValueOfCollisions);
            s.AppendLine("ValueOfRefactoredFuel = " + ValueOfRefactoredFuel);
            s.AppendLine("ValueOfStockedMineral = " + ValueOfStockedMineral);
            s.AppendLine("ValueOfRemainingFuel = " + ValueOfRemainingFuel);
            return s;
        }
    }
}