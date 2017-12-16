using System;
using System.Diagnostics;
using SwarmSimFramework.Interfaces;

namespace SwarmSimFramework.Classes.Map
{
    public class MapSimulation<T> where T : IRobotBrain
    {
        private Map map;
        private MapModel model;

        public MapSimulation(MapModel model, BrainModel<T>[] models)
        {
            map = model.ConstructMap();
            this.model = model; 
            foreach (var r in map.Robots)
            {
                foreach (var b in models)
                {
                    if (b.SuitableRobot(r))
                        r.Brain = b.Brain.GetCleanCopy();
                }
                if (r.Brain == null)
                    throw new ArgumentException("Not connected robot, models given to the simulation are incorrect");
            }
        }

        public Map Simulate(int numberOfIteration)
        {
            Debug.Assert(map.Cycle == 0);
            for (int i = 0; i < numberOfIteration; i++)
                map.MakeStep();

            return map;
        }

        public void Reset(BrainModel<T>[] models)
        {
            map = model.ConstructMap();
            foreach (var r in map.Robots)
            {
                foreach (var b in models)
                {
                    if (b.SuitableRobot(r))
                        r.Brain = b.Brain.GetCleanCopy();
                }
                if (r.Brain == null)
                    throw new ArgumentException("");
            }
        }
    }
}