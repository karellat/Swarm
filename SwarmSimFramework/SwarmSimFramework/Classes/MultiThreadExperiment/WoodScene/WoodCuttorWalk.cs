﻿using System.Numerics;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.Robots;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public class WoodCuttorWalk : WoodExperimentMt
    {
        /// <summary>
        /// Prepare models and fitness eval
        /// </summary>
        protected override void Init()
        {
            PopulationSize = 100;
            NumberOfGenerations = 1000;
            Name = "WoodCuttorWalk";
            Models = new RobotModel[1];
            Models[0] = new RobotModel()
            {
                amount = 5,
                model = new ScoutCutterRobot(new Vector2(0, 0))
            };
            AmountOfTrees = 200;
            AmountOfWood = 0;
            ValueOfDiscoveredTree = 1000;
            ValueOfCutWood = 1050;
            ValueOfCollision = 0;
            base.Init();
        }

        protected override void Init(string[] nameOfInitialFile)
        {
            PopulationSize = 100;
            NumberOfGenerations = 1000;
            Name = "WoodCuttorWalk";
            Models = new RobotModel[1];
            Models[0] = new RobotModel()
            {
                amount = 5,
                model = new ScoutCutterRobot(new Vector2(0, 0))
            };
            AmountOfTrees = 200;
            AmountOfWood = 0;
            ValueOfDiscoveredTree = 1000;
            ValueOfCutWood = 1050;
            ValueOfCollision = 0;
            base.Init(nameOfInitialFile);
        }
    }
}