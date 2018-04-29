﻿using System.Numerics;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Classes.Robots.WoodRobots;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public class WoodWorkerPickUp : WoodExperimentMt
    {


        /// <summary>
        /// Prepare models and fitness eval
        /// </summary>
        protected override void Init(string[] nameOfInitialFile)
        {
            WorkingDir = "WoodWorkerPickUp";
            PopulationSize = 100;
            NumberOfGenerations = 1000;
            MapIteration = 2000;
            Name = "WoodWorkerPickUp";
            Models = new RobotModel[1];
            Models[0] = new RobotModel()
            {
                amount = 4,
                model = new WoodWorkerRobot(Vector2.Zero)
            };
            AmountOfTrees = 50;
            AmountOfWood = 200;
            ValueOfDiscoveredTree = 0;
            ValueOfCutWood = 1;
            ValueOfCollision = 0;
            ValueOfStockedWood = 1000;
            ValueOfContaineredWood = 100;
            base.Init(nameOfInitialFile);
        }
    }

}