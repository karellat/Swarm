using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Experiments;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public class MineralExperimentMT : MultiThreadExperiment<SingleLayerNeuronNetwork>
    {
        // Basic Evolution VARIABLE
        public int AmountOfMineral = 0;
        public int AmountOfObstacle = 0;
        public int AmountOfFreeFuel = 0;
        public double ValueOfDiscoveredMineral = 0.0;
        public double ValueOfStockedMineral = 0.0;
        public double ValueOfRefactoredFuel = 0.0;
        public double ValueOfRemainingFuel = 0.0;

        protected BrainModel<SingleLayerNeuronNetwork>[] BrainModels;

        protected override void Init(string[] nameOfInitialFile)
        {
            Map.MineralScene.AmountOfMineral = AmountOfMineral;
            Map.MineralScene.AmountOfObstacles = AmountOfObstacle;
            Map.MineralScene.AmountOfFreeFuel = AmountOfFreeFuel;
            MapModel = Map.MineralScene.MakeMapModel(Models);

            //Prepare model brains from models
            BrainModels = new BrainModel<SingleLayerNeuronNetwork>[Models.Length];
            for (int i = 0; i < Models.Length; i++)
            {
                BrainModels[i] = new BrainModel<SingleLayerNeuronNetwork>()
                {
                    Robot = Models[i].model,
                    Brain = SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension()
                    {
                        Input = Models[i].model.SensorsDimension,
                        Output = Models[i].model.EffectorsDimension
                    })
                };
            }

            if (nameOfInitialFile == null)
                GenerateBrains();
            else
                ReadBrainsFromFiles(nameOfInitialFile);
        }

        /// <summary>
        /// Generate new brains for actual assignment
        /// </summary>
        protected void GenerateBrains()
        {
            Console.WriteLine("Generationg new brains.");
            //Generate actual Generation
            ActualGeneration = new List<SingleLayerNeuronNetwork>[Models.Length];
            for (int i = 0; i < ActualGeneration.Length; i++)
            {
                ActualGeneration[i] = new List<SingleLayerNeuronNetwork>(PopulationSize);
                var m = Models[i].model;
                for (int j = 0; j < PopulationSize; j++)
                {
                    ActualGeneration[i].Add(SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension() { Input = m.SensorsDimension, Output = m.EffectorsDimension }));
                }
            }
            Console.WriteLine("Brains generated");
        }

        /// <summary>
        /// Create brains from a file 
        /// </summary>
        /// <param name="nameOfInitialFile"></param>
        protected void ReadBrainsFromFiles(string[] nameOfInitialFile)
        {
            if (Models.Length != nameOfInitialFile.Length)
                throw new ArgumentException("Wrong parameters");

            //Generate actual Generation
            ActualGeneration = new List<SingleLayerNeuronNetwork>[Models.Length];
            for (int i = 0; i < nameOfInitialFile.Length; i++)
            {
                StreamReader s = new StreamReader(nameOfInitialFile[i]);
                String text = s.ReadToEnd();
                ActualGeneration[i] = BrainSerializer.DeserializeArray<SingleLayerNeuronNetwork>(text).ToList();
            }

            //DEBUG 
            int size = ActualGeneration[0].Count;
            foreach (var a in ActualGeneration)
            {
                if (a.Count != size)
                    throw new ArgumentException("Invalid size of actual generation ");
            }

            //Evaluate brains for actual fitness
            Console.WriteLine("Brains read from serialization file, evaluating brains by local fitness");


            for (int i = 0; i < ActualGeneration[0].Count; i++)
            {
                Map.Map map = MapModel.ConstructMap();
                //Set all brains to Robots  
                //Set all brains to Robots  
                foreach (var r in map.Robots)
                {
                    for (int j = 0; j < BrainModels.Length; j++)
                    {
                        if (BrainModels[j].SuitableRobot(r))
                            foreach (List<SingleLayerNeuronNetwork> t in ActualGeneration)
                            {
                                if (BrainModels[j].SuitableBrain(t[i]))
                                    r.Brain = t[i].GetCleanCopy();
                            }
                    }
                }

                //iterate map
                for (int q = 0; q < MapIteration; q++)
                    map.MakeStep();
                double fitness = CountFitness(map);
                //Set fitness from current experiment
                for (int j = 0; j < ActualGeneration.Length; j++)
                    ActualGeneration[j][i].Fitness = fitness;
            }
            Console.WriteLine("Init from files finnished.");
        }
        /// <summary>
        /// Selection of single brain
        /// </summary>
        /// <param name="evolveBrains"></param>
        /// <returns></returns>
        protected override BrainModel<SingleLayerNeuronNetwork>[] SingleBrainSelection(SingleLayerNeuronNetwork[] evolveBrains)
        {
            BrainModel<SingleLayerNeuronNetwork>[] output =
                new BrainModel<SingleLayerNeuronNetwork>[evolveBrains.Length];

            for (int i = 0; i < evolveBrains.Length; i++)
            {
                var o = DifferentialEvolution.DifferentialEvolutionBrain(evolveBrains[i], ActualGeneration[i]);
                foreach (var b in BrainModels)
                {
                    if (b.SuitableBrain(o))
                        output[i] = new BrainModel<SingleLayerNeuronNetwork>()
                        {
                            Brain = o,
                            Robot = (RobotEntity)b.Robot.DeepClone()
                        };
                }
            }
            return output;
        }

        protected override double CountFitness(Map.Map map)
        {
            int discoveredMineral = 0;
            int fuelInMap = 0;
            int deadRobots = 0;

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
            return discoveredMineral * ValueOfDiscoveredMineral + mineralInContainer * ValueOfStockedMineral +
                   (fuelInContainers + fuelInMap) * ValueOfRefactoredFuel + fuelInTanks * ValueOfRemainingFuel;
        } 
    }
}