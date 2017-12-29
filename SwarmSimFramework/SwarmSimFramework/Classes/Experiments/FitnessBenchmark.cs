using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Interpolation;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;

namespace SwarmSimFramework.Classes.Experiments
{
    class FitnessBenchmark
    {
        public MapModel[] MapModels;
        public IFitnessCounter Fitness;
        public List<BrainModel<SingleLayerNeuronNetwork>[]> Brains;
        public int MapIterations = 2000;
        public int MaxInt; 
        public enum Scene
        {
            Wood,
            Mineral, 
            Competive
        }

        public FitnessBenchmark(Scene sceneType, IFitnessCounter counter,RobotModel[] robots,List<BrainModel<SingleLayerNeuronNetwork>[]> brains)
        {
            MaxInt = brains.Count;
            Console.WriteLine("Creating fitness benchmark: for " +  brains.Count + " brains");
            foreach (var r in robots)
                Console.WriteLine("\t Robot - " + r.model.Name + " - " + r.amount);

            //DEBUG Output 
#if DEBUG
            Console.WriteLine(counter.Log().ToString());
            switch (sceneType)
            {
                case Scene.Wood:
                    Console.WriteLine(WoodScene.Log());
                    break;
                case Scene.Competive:
                    Console.WriteLine(CompetitiveScene<SingleLayerNeuronNetwork>.Log());
                    break;
                case Scene.Mineral:
                    Console.WriteLine(MineralScene.Log());
                    break;
            }

#endif        
            var differentMaps = 20; 
            MapModels = new MapModel[differentMaps];
            Fitness = counter;
            this.Brains = brains;


            for (int i = 0; i < MapModels.Length; i++)
            {
                switch (sceneType)
                {
                    case Scene.Wood:

                        MapModels[i] = WoodScene.MakeMapModel(robots);
                        break;
                    case Scene.Competive:
                        MapModels[i] = CompetitiveScene<SingleLayerNeuronNetwork>.MakeMapModel(robots);
                        break;
                    case Scene.Mineral:
                        MapModels[i] = MineralScene.MakeMapModel(robots);
                        break; 
                }
            }

        }

        public List<KeyValuePair<double, BrainModel<SingleLayerNeuronNetwork>[]>>  GetSortedBrainsByBenchmark()
        {
            ConcurrentBag<KeyValuePair<double,BrainModel<SingleLayerNeuronNetwork>[]>> evaluatedBrains = 
                new ConcurrentBag<KeyValuePair<double, BrainModel<SingleLayerNeuronNetwork>[]>>();

            Task[] tasks = new Task[Brains.Count];
            for (int i = 0; i < Brains.Count; i++)
            {
                int taskI = i; 
                tasks[i] = new Task(() =>
                {
                    evaluatedBrains.Add(SingleTaskEval(Brains[taskI],taskI));
                });
                tasks[i].Start();
            }

            Task.WaitAll(tasks);

            var list = evaluatedBrains.ToList(); 

            list.Sort((a, b) => -a.Key.CompareTo(b.Key));
            return list; 
        }

        public KeyValuePair<double, BrainModel<SingleLayerNeuronNetwork>[]> SingleTaskEval(
            BrainModel<SingleLayerNeuronNetwork>[] brains, int index)
        {
            double fitness = 0; 
            foreach (var map in MapModels)
            {
               MapSimulation<SingleLayerNeuronNetwork> sim = new MapSimulation<SingleLayerNeuronNetwork>(map,brains);
                fitness +=  Fitness.GetMapFitness(sim.Simulate(MapIterations));
            }
            Console.WriteLine("{0} - finnished",index);
            return new KeyValuePair<double, BrainModel<SingleLayerNeuronNetwork>[]>(fitness/((float) MapModels.Length),brains);
        }
    }
}
