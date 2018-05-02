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
    public class CompetiveExperimentMT : MultiThreadExperimentClasicApproach<SingleLayerNeuronNetwork>
    {
        // Basic Evolution VARIABLE
        public static int AmountOfObstacle = 500;
        // Fitness settings
        public static double ValueOfDiscoveredObstacle = 0;
        public static double ValueOfDeadRobot = 0;
        public static double ValueOfRemainingHealth = 0;
        public static double ValueOfColissions = 0;
        // Enemy settings
        public static RobotModel[] EnemyModels = null;
        public static BrainModel<SingleLayerNeuronNetwork>[] EnemyBrainModels = null; 

        protected BrainModel<SingleLayerNeuronNetwork>[] BrainModels;

        protected override void Init(string[] nameOfInitialFile)
        {
            if (EnemyBrainModels == null)
                throw new NullReferenceException("EnemyBrainModels is not set");
            if (EnemyModels == null)
                throw new NullReferenceException("EnemyModels is not set");

            Map.CompetitiveScene<SingleLayerNeuronNetwork>.AmountOfObstacles = AmountOfObstacle;
            Map.CompetitiveScene<SingleLayerNeuronNetwork>.SetUpEnemies(EnemyModels,EnemyBrainModels);
                
            MapModel = Map.CompetitiveScene<SingleLayerNeuronNetwork>.MakeMapModel(Models);

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
                foreach (var r in map.Robots)
                {
                    for (int j = 0; j < BrainModels.Length; j++)
                    {
                        if (BrainModels[j].SuitableRobot(r) && BrainModels[j].SuitableBrain(ActualGeneration[j][i]))
                            r.Brain = ActualGeneration[j][i].GetCleanCopy();
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
            long colisions = 0;
            int discoveredObstacle = 0;
            int deadRobots = 0;
            double remainingHealth = 0;


            //discovered minerals
            foreach (var p in map.PasiveEntities)
            {
                if (p.Discovered && p.Color == Entity.EntityColor.ObstacleColor)
                    discoveredObstacle++;
            }
            
            foreach (var r in map.Robots)
            {
                if(r.TeamNumber != 1) continue;
                
                //Count dead robots
                if (!r.Alive)
                {
                    deadRobots++; 
                    continue;
                }

                //Count remaining health 
                remainingHealth += r.Health;
                colisions += r.CollisionDetected;
               
            }

            return discoveredObstacle * ValueOfDiscoveredObstacle + ValueOfDeadRobot * deadRobots +
                   (ValueOfRemainingHealth + remainingHealth) + colisions * ValueOfColissions;
        } 
    }
}