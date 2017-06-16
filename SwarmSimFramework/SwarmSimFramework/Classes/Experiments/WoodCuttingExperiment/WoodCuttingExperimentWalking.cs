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
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments.WoodCuttingExperiment
{
    /// <summary>
    /// First step of wood cutting experiment, walking part & discovery of trees
    /// </summary>
    public class WoodCuttingExperimentWalking : Experiment<SingleLayerNeuronNetwork>
    {
        //ENVIROMENT Variables
        /// <summary>
        /// Number of trees in the eviroment after init
        /// </summary>
        protected  int AmountOfTrees = 200;
        /// <summary>
        /// Number of woods in the enviroment after init
        /// </summary>
        protected  int AmountOfWood = 0;

        /// <summary>
        /// Name  of init file
        /// </summary>
        protected string NameOfInitFile = "scoutCutterInitWithMem";

        protected RobotEntity model = new ScoutCutterRobotWithMemory(new Vector2(0, 0));


/// <summary>
/// Origin network
/// </summary>
protected SingleLayerNeuronNetwork originNetwork;

        //FITNESS VARIABLES
        /// <summary>
        /// Trees discovered by current brain
        /// </summary>
        protected int DiscoveredTrees = 0;

        /// <summary>
        /// Amount of discovered woods 
        /// </summary>
        protected int DiscoveredWood = 0;

        
        /// <summary>
        /// Value of single discovered tree
        /// </summary>
        protected  double ValueOfDiscoveredTree = 1000;
        /// <summary>
        /// Value of tree changed to the wood or discovered wood 
        /// </summary>
        protected  double ValueOfCutWood = 1050;
        /// <summary>
        /// Value of single collision
        /// </summary>
        protected  double ValueOfCollision = 0;
        

        /// <summary>
        /// Directory for ass
        /// </summary>
        protected string WorkingDir = "walkwoodExpMem";
        /// <summary>
        /// Init of wood cutting experiment walking
        /// </summary>
        public override void Init()
        {
            base.Init();
            //Prepare stuff for serialization 
            System.IO.Directory.CreateDirectory(WorkingDir);

         
            //set experiment
            InitRobotEntities(new [] {model}, new[] { 5 });
            InitGenerationFile[0] = NameOfInitFile+ ".json";
           
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
                        Output = Models[0].EffectorsDimension
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
            FollowingGeneration[0] = new List<SingleLayerNeuronNetwork>();
            originNetwork = ActualGeneration[0][0];
            ActualBrains[0] =
                DifferentialEvolution.DifferentialEvolutionBrain(ActualGeneration[0][0], ActualGeneration[0]);
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
        protected virtual double CountBrainFitness()
        {
            DiscoveredTrees = 0;
            int CutWoods = 0;
            //Find discovered trees 
            foreach (var p in Map.PasiveEntities)
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
            foreach (var r in Map.Robots)
            {
                checked
                {
                    amountOfCollision += r.CollisionDetected;
                }
            }
            return (DiscoveredTrees * ValueOfDiscoveredTree) + (ValueOfCollision * amountOfCollision) + (CutWoods * ValueOfCutWood);
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
            if (nbFitness >= originNetwork.Fitness)
            {
                ActualBrains[0].Fitness = nbFitness;
                FollowingGeneration[0].Add(ActualBrains[0]);
            }
            else
            {
                FollowingGeneration[0].Add(originNetwork);
            }
            //Prepare new brain by differencial evolution algorithm
            if (BrainIndex + 1 != PopulationSize)
            {
                originNetwork = ActualGeneration[0][BrainIndex + 1];
                ActualBrains[0] =
                    DifferentialEvolution.DifferentialEvolutionBrain(ActualGeneration[0][BrainIndex],
                        ActualGeneration[0]);
            }
            else
            {
                originNetwork = FollowingGeneration[0][0];
                ActualBrains[0] =
                    DifferentialEvolution.DifferentialEvolutionBrain(FollowingGeneration[0][0],
                        FollowingGeneration[0]);
            }
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