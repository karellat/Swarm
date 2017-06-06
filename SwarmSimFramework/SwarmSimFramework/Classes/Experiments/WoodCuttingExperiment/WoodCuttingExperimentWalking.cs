using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments.WoodCuttingExperiment
{
    /// <summary>
    /// First step of wood cutting experiment, walking part & discovery of trees
    /// </summary>
    public class WoodCuttingExperimentWalking : Experiment<SingleLayerNeuronNetwork>
    {
        //ENVIROMENT Variables
        protected const int AmountOfTrees = 100;

        //FITNESS VARIABLES
        /// <summary>
        /// Trees discovered by current brain
        /// </summary>
        protected int DiscoveredTrees = 0;
        /// <summary>
        /// Value of single discovered tree
        /// </summary>
        protected const double ValueOfDiscoveredTree = 100;
        /// <summary>
        /// Value of single collision
        /// </summary>
        protected const double ValueOfCollision = -1;


        /// <summary>
        /// Directory for ass
        /// </summary>
        protected string WorkingDir = "walkwoodExp";
        /// <summary>
        /// Init of wood cutting experiment walking
        /// </summary>
        public override void Init()
        {
            //Prepare stuff for serialization 
            System.IO.Directory.CreateDirectory(WorkingDir);

            //Prepare fitness count
            RawMaterialEntity tree = new RawMaterialEntity(new Vector2(0,0),5,10,10);
            ObstacleEntity initPosition = new ObstacleEntity(new Vector2(Map.MaxWidth/2,Map.MaxHeight/2),20);
            //Generate randomly deployed tree
            Map.Map preparedMap = new Map.Map(MapHeight,MapWidth,null, new List<CircleEntity>() {initPosition});
            List<CircleEntity> trees =
                Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, tree, 100);
            //set experiment
            InitRobotEntities(new [] {new ScoutCutterRobot(new Vector2(0,0))},new []{5});
            InitGenerationFile[0] =  "scoutCutterInit.json";
            //Prepare robot bodies
            List<RobotEntity> robots = new List<RobotEntity>();
            for (int i = 0; i < Models.Length; i++)
            {
                for (int j = 0; j < AmountOfRobots[i]; j++)
                    robots.Add((RobotEntity) Models[i].DeepClone());
            }
            //Initial position 
            robots[0].MoveTo(new Vector2(MapWidth/2,MapHeight/2));
            robots[1].MoveTo(new Vector2(MapWidth / 2 + 10, MapHeight / 2));
            robots[2].MoveTo(new Vector2(MapWidth / 2, MapHeight / 2 + 10));
            robots[3].MoveTo(new Vector2(MapWidth / 2 -10, MapHeight / 2));
            robots[4].MoveTo(new Vector2(MapWidth / 2, MapHeight / 2 -10));
            //Prepare Map 
            Map = new Map.Map(MapHeight,MapWidth,robots,trees);

            //read from file, if exists
            if (File.Exists(InitGenerationFile[0]))
            {
                StreamReader s =  new StreamReader(InitGenerationFile[0]);
                ActualGeneration[0] = BrainSerializer.DeserializeArray<SingleLayerNeuronNetwork>(s.ReadToEnd())
                    .ToList();
            }
            //Prepare first generation of brains
            else
            {
                ActualGeneration[0] = new List<SingleLayerNeuronNetwork>(PopulationSize);
                for (int i = 0; i < PopulationSize; i++)
                {
                    ActualGeneration[0].Add(SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension()
                    {
                        Input = Models[0].SensorsDimension,
                        Output = Models[1].EffectorsDimension
                    }));
                }

                //Count fitness of brains
                int brainI = 0;
                //over all brains
                foreach (var brain in ActualGeneration[0])
                {
                    ExperimentInfo = new StringBuilder("Inicialization of random brain: " + brainI);
                    //Clean map 
                    Map.Reset();
                    ResetFitness();
                    foreach (var robot in Map.Robots)
                        robot.Brain = brain; 

                    //Make Simulation 
                    for (int i = 0; i < MapIteration; i++)
                    {
                        Map.MakeStep();
                        //DEBUG
                        Map.CheckCorrectionOfPossition();
                        foreach (var r in Map.Robots)
                        {
                            CountIndividualFitness(r);
                        }
                        CountIterationFitness();
                    }

                    //Set created fitness
                    brain.Fitness = CountBrainFitness();
                    Map.Reset();
                    ResetFitness();
                }
                StreamWriter file = new StreamWriter(InitGenerationFile[0]);
                file.Write(BrainSerializer.SerializeArray(ActualGeneration[0].ToArray()));
                file.Close();
            }
            //Prepare first brain for evolution 
            ActualBrains[0] = ActualGeneration[0][0];
            ActualBrains[0].Fitness = 0;
            foreach (var r in Map.Robots)
                r.Brain = ActualBrains[0].GetCleanCopy();

        }
        /// <summary>
        /// Global fitnessCount after single iteration
        /// </summary>
        protected  override void CountIterationFitness()
        {
            //No need for iteration fitness count
        }
        /// <summary>
        /// Reset fitness counters 
        /// </summary>
        private void ResetFitness()
        {
           
        }
        /// <summary>
        /// Count fitness of actual brain
        /// </summary>
        /// <returns></returns>
        private double CountBrainFitness()
        {
            DiscoveredTrees = 0;
            //Find discovered trees 
            foreach (var p in Map.PasiveEntities)
            {
                if (p.Color == Entity.EntityColor.RawMaterialColor)
                {
                    if ((p as RawMaterialEntity).Discovered)
                        DiscoveredTrees++;
                }
            }
            long amountOfCollision = 0;
            //Find collision
            foreach (var r in Map.Robots)
            {
                checked
                {
                    amountOfCollision += r.CollisionDetected;
                }
            }
            return (DiscoveredTrees * ValueOfDiscoveredTree) + (ValueOfCollision * amountOfCollision);
        }

        /// <summary>
        /// Fitness of individual
        /// </summary>
        /// <param name="robotEntity"></param>
        protected override void CountIndividualFitness(RobotEntity robotEntity)
        {
            //No need for iteration fitness count of individual
        }
        /// <summary>
        /// Single map iterations 
        /// </summary>
        protected override void SingleMapSimulation()
        {
            //Give brain to following generation
            double nbFitness = CountBrainFitness();
            if (nbFitness >= ActualGeneration[0][BrainIndex].Fitness)
            {
                ActualBrains[0].Fitness = nbFitness;
                FollowingGeneration[0].Add(ActualBrains[0]);
            }
            else
            {
                FollowingGeneration[0].Add(ActualGeneration[0][BrainIndex]);
            }
            //Prepare new brain by differencial evolution algorithm
            ActualBrains[0] =
                DifferentialEvolution.DifferentialEvolutionBrain(ActualGeneration[0][BrainIndex + 1],
                    ActualGeneration[0]);
        }
        /// <summary>
        /// After signle generation
        /// </summary>
        protected override void SingleGeneration()
        {
            //log generation
            var i = GenerationInfoStruct.GetGenerationInfo(ActualGeneration[0]);
            StringBuilder sb = new StringBuilder("Info about " + (GenerationIndex) + ". generation ");
            sb.AppendLine("Best fitness: " + i.FitnessMaximum);
            sb.AppendLine("Worst fitness " + i.FitnessMinimum);
            sb.AppendLine("Average fitness: " + i.FitnessAverage);
            sb.AppendLine("Brain: " + i.BestBrainInfo);
            GenerationInfo = sb;
            //Serialize brain 
            StreamWriter n = new StreamWriter( WorkingDir + "\\brain" + (GenerationIndex));
            n.Write(i.BestBrain.SerializeBrain());
            n.Close();
        }
    }
}