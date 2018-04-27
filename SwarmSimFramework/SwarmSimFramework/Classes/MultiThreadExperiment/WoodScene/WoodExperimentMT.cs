using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Experiments;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;
using Newtonsoft.Json; 

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    /// <summary>
    /// Basic general Wood experiment MT implementation
    /// </summary>
    public class WoodExperimentMt : MultiThreadExperimentClasicApproach<SingleLayerNeuronNetwork> 
    {
        // Basic Evolution VARIABLE
        public int AmountOfTrees = 0;
        public int AmountOfWood = 0;
        public int ValueOfCutWood = 0;
        public long ValueOfCollision = 0;

        public int ValueOfDiscoveredTree = 0;

        public double ValueOfStockedWood = 0;
        public double ValueOfContaineredWood = 0;
        public double ValueOfContaineredNoWood = 0;
        public double ValueOfDistanceToMiddle = 0;

        [JsonProperty]
        protected BrainModel<SingleLayerNeuronNetwork>[] BrainModels;

        /// <summary>
        /// Init map for woodExperiment with given models init in children
        /// </summary>
        protected override void Init(string[] nameOfInitialFile = null)
        {
            WoodScene.AmountOfTrees = AmountOfTrees;
            WoodScene.AmountOfWoods = AmountOfWood;
            MapModel = WoodScene.MakeMapModel(Models);

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

            if(nameOfInitialFile == null)
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
                foreach (var r in map.Robots)
                {
                    for (int j = 0; j < BrainModels.Length; j++)
                    {
                        if (BrainModels[j].SuitableRobot(r))
                            foreach (List<SingleLayerNeuronNetwork> t in ActualGeneration)
                            {
                                if(BrainModels[j].SuitableBrain(t[i]))
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
        /// Create new brain for suitable robot
        /// </summary>
        /// <param name="evolveBrains"></param>
        /// <returns></returns>
        protected override BrainModel<SingleLayerNeuronNetwork>[] SingleBrainSelection(SingleLayerNeuronNetwork[] evolveBrains)
        {
            BrainModel<SingleLayerNeuronNetwork>[] output =
                new BrainModel<SingleLayerNeuronNetwork> [evolveBrains.Length];

            for (int i = 0; i < evolveBrains.Length; i++)
            {
                //Setings of diffretial evolution 

                var o  = DifferentialEvolution.DifferentialEvolutionBrain(evolveBrains[i], ActualGeneration[i]);

                foreach (var b in BrainModels)
                {
                    if (b.SuitableBrain(o))
                        output[i] = new BrainModel<SingleLayerNeuronNetwork>()
                        {
                            Brain = o,
                            Robot = (RobotEntity) b.Robot.DeepClone()
                        };
                }
            }
            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        protected override double CountFitness(Map.Map map)
        {
            double mapFitness = CountFitnessOfMap(map); 
            double robotFitnes = 0;
            if (ValueOfStockedWood != 0 || ValueOfContaineredWood != 0)
                robotFitnes = StockContainerFitness(map);
            return mapFitness + robotFitnes;
        }

        public double CountFitnessOfMap(Map.Map map)
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
        protected double StockContainerFitness(Map.Map map)
        {
         
            //Count wood on the stockPlace
            var stockSignal = map.constantRadioSignal[0];
            var stockItems = map.CollisionColor(stockSignal);

            int minedWood = 0;
            double distanceSum = 0;
            var collidingItems = map.GetAllCollidingPassive(stockSignal);
            foreach (var item in collidingItems)
            {
                if (item.Color == Entity.EntityColor.WoodColor)
                    minedWood++;
            }

            // Count wood in containers 
            int woodInContainers = 0;
            int noWoodInContainers = 0;

            foreach (var r in map.Robots)
            {
                foreach (var item in r.ContainerList())
                {
                    if (item.Color == Entity.EntityColor.WoodColor)
                        woodInContainers++;
                    else
                        noWoodInContainers++; 
                }
            }

            return (ValueOfContaineredNoWood * noWoodInContainers) +
                (ValueOfContaineredWood * woodInContainers) + (ValueOfStockedWood * minedWood) + distanceSum;
        }

        
    }
}