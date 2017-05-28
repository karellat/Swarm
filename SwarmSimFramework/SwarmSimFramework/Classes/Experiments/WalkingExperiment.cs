using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments
{
    public class WalkingExperiment: IExperiment
    {
        public static int SizeOfGeneration = 1000;
        public static int AmountOfGenerations = 1000;
        public static int MapIteration = 1000;
        public static float MapHeight = 660;
        public static float MapWidth = 800;
        public static int AmountOfRobots = 10;
        public static RobotEntity modelRobot = new ScoutRobot(new Vector2(Single.NaN,Single.NaN),100);
        protected List<SingleLayerNeuronNetwork> actualGeneration;
        protected List<SingleLayerNeuronNetwork> followingGeneration;
        protected Func<SingleLayerNeuronNetwork, float> countFitness;




        public Map.Map Map { get; protected set; }
        public void Init()
        {
            List<RobotEntity> robots = new List<RobotEntity>(AmountOfRobots);
            float gap = MapWidth / AmountOfRobots;
            for (int i = 0; i < 10; i++)
            {
                var newRobot = (RobotEntity) modelRobot.DeepClone();
                newRobot.MoveTo(new Vector2(10, gap + i * gap)); 
                newRobot.RotateDegrees(-90);
                robots.Add(newRobot);
            }
            //Prepare map 
            Map = new Map.Map(MapHeight, MapWidth,robots);  
            //Prepare brains 
            actualGeneration = new List<SingleLayerNeuronNetwork>(SizeOfGeneration);
            for (int i = 0; i < SizeOfGeneration; i++)
            {
                actualGeneration.Add(SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension(){Input = modelRobot.SensorsDimension,Output = modelRobot.EffectorsDimension}));
            }
            //Count fitness of brains 
            foreach (var brain in actualGeneration)
            {
                foreach (var robot in Map.Robots)
                {
                    robot.Brain = brain;
                }
                for (int i = 0; i < P; i++)
                {
                    
                }
            }
        }

        public void MakeStep()
        {
            throw new System.NotImplementedException();
        }

        public bool Finnished { get; }
    }
}