using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;
using System.IO;
using System.Linq;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments.WoodCuttingExperiment
{
    public class WoodCuttingExperimentWalking : Experiment<SingleLayerNeuronNetwork>
    {
        //ENVIROMENT Variables
        protected int AmountOfTrees = 100;

        //FITNESS VARIABLES
        protected List<RawMaterialEntity> Trees;
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
            initGenerationFile[0] =  "scoutCutterInit.json";
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
            if (File.Exists(initGenerationFile[0]))
            {
                StreamReader s =  new StreamReader(initGenerationFile[0]);
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
                    }

                    //Set created fitness
                    brain.Fitness = CountBrainFitness();
                    ResetFitness();
                }
            }


        }

        private double CountBrainFitness()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Fitness of individual
        /// </summary>
        /// <param name="robotEntity"></param>
        protected override void CountIndividualFitness(RobotEntity robotEntity)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Single map iterations 
        /// </summary>
        protected override void SingleMapSimulation()
        {
            throw new System.NotImplementedException();
        }

        protected override void SingleGeneration()
        {
            throw new System.NotImplementedException();
        }
    }
}