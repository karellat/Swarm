using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Experiments;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public  abstract  class WoodExperimentMT : MultiThreadExperiment<SingleLayerNeuronNetwork> 
    {
        public int AmountOfTrees = 0;
        public int AmountOfWood = 0;

        protected WoodExperimentMT()
        {
            Models = new RobotEntity[2];
        }

        /// <summary>
        /// 
        /// </summary>
        //protected override void Init()
        //{
        //    //Prepare fitness count
        //    RawMaterialEntity tree = new RawMaterialEntity(new Vector2(0, 0), 5, 10, 10);
        //    WoodEntity wood = new WoodEntity(new Vector2(0, 0), 5, 10);
        //    ObstacleEntity initPosition = new ObstacleEntity(new Vector2(MapWidth / 2, MapHeight / 2), 30);
        //    //Generate randomly deployed tree
        //    Map.Map preparedMap = new Map.Map(MapHeight, MapWidth, null, new List<CircleEntity>() { initPosition });
        //    List<CircleEntity> trees =
        //        Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, tree, AmountOfTrees);
        //    var tp = trees.ToList();
        //    tp.Add(initPosition);
        //    preparedMap = new Map.Map(MapHeight, MapWidth, null, tp);
        //    List<CircleEntity> woods = Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, wood, AmountOfWood);

        //    //Prepare robot bodies
        //    List<RobotEntity> robots = new List<RobotEntity>();
        //    //Prepare Scouts
        //    if(Models[0] != null)
        //    {
        //        for (int i = 0; i < 6; i++)
        //        {
        //            var r = (RobotEntity) Models[0].DeepClone();
        //            robots.Add(r);
        //        }
        //    }
        //    if (Models[1] != null)
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            var r = (RobotEntity) Models[1].DeepClone();
                    
        //            robots.Add(r);
        //        }
        //    }


        //    //Prepare Map 
        //    //merge trees and woods
        //    var pas = woods.Concat(trees).ToList();
            
        //    //Set models
        //    ModelsOfRobots = robots;
        //    ModelsOfPasiveEntities = pas;
        //    ModelsOfFuelEntities = null;
        //    PermanentRadioSignals = null;

        //    //read from file, if exists
        //    if (File.Exists(InitGenerationFile[0]))
        //    {
        //        StreamReader s = new StreamReader(InitGenerationFile[0]);
        //        ActualGeneration[0] = BrainSerializer.DeserializeArray<SingleLayerNeuronNetwork>(s.ReadToEnd())
        //            .ToList();
        //    }
        //    //Prepare first generation of brains
        //    else
        //    {
        //        ActualGeneration[0] = new List<T>(PopulationSize);
        //        for (int i = 0; i < PopulationSize; i++)
        //        {
        //            ActualGeneration[0].Add(SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension()
        //            {
        //                Input = Models[0].SensorsDimension,
        //                Output = Models[0].EffectorsDimension
        //            }));
        //        }

        //        //Count fitness of brains
        //        int brainI = 0;
        //        //over all brains
        //        foreach (var brain in ActualGeneration[0])
        //        {
        //            ExperimentInfo = new StringBuilder("Inicialization of random brain: " + brainI);
        //            //Clean map 
        //            Map.Reset();
        //            ResetFitness();
        //            foreach (var robot in Map.Robots)
        //                robot.Brain = brain;

        //            //Make Simulation 
        //            for (int i = 0; i < MapIteration; i++)
        //            {
        //                Map.MakeStep();
        //                //DEBUG
        //                Map.CheckCorrectionOfPossition();
        //                foreach (var r in Map.Robots)
        //                {
        //                    CountIndividualFitness(r);
        //                }
        //                CountIterationFitness();
        //            }

        //            //Set created fitness
        //            brain.Fitness = CountBrainFitness();
        //            Map.Reset();
        //            ResetFitness();
        //        }
        //        StreamWriter file = new StreamWriter(InitGenerationFile[0]);
        //        file.Write(BrainSerializer.SerializeArray(ActualGeneration[0].ToArray()));
        //        file.Close();
        //    }
        //    //Prepare first brain for evolution 
        //    FollowingGeneration[0] = new List<SingleLayerNeuronNetwork>();
        //    originNetwork = ActualGeneration[0][0];
        //    ActualBrains[0] =
        //        DifferentialEvolution.DifferentialEvolutionBrain(ActualGeneration[0][0], ActualGeneration[0]);
        //    ActualBrains[0].Fitness = 0;
        //    foreach (var r in Map.Robots)
        //        r.Brain = ActualBrains[0].GetCleanCopy();
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override SingleLayerNeuronNetwork[] SingleBrainSelection()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        protected override double CountFitness(Map.Map map)
        {
            throw new System.NotImplementedException();
        }
    }
}