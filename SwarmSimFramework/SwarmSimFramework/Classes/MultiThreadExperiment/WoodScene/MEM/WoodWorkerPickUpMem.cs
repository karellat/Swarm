﻿using System.Numerics;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.Robots.WoodRobots;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public class WoodWorkerPickUpMem : WoodExperimentMt
    {


        /// <summary>
        /// Prepare models and fitness eval
        /// </summary>
        protected override void Init(string[] nameOfInitialFile)
        {
            WorkingDir = "WoodWorkerPickUpMem";
            PopulationSize = 1000;
            NumberOfGenerations = 1000;
            MapIteration = 2000;
            Name = "WoodWorkerPickUpMem";
            Models = new RobotModel[1];
            Models[0] = new RobotModel()
            {
                amount = 4,
                model = new WoodWorkerRobotMem(Vector2.Zero)
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