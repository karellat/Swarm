using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;

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

            //Prepare first generation of brains

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