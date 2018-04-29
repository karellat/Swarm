﻿using System.Numerics;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Classes.Robots.WoodRobots;

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    public class WoodWorkerWalkMem :WoodExperimentMt
    {
        /// <summary>
        /// Prepare models and fitness eval
        /// </summary>
        protected override void Init(string[] nameOfInitialFile)
        {
            WorkingDir = "WoodWorkerWalkMem";
            PopulationSize = 1000;
            NumberOfGenerations = 1000;
            MapIteration = 2000;
            Name = "WoodWorkerWalkMem";
            Models = new RobotModel[1];
            Models[0] = new RobotModel()
            {
                amount = 4,
                model = new WoodWorkerRobotMem(Vector2.Zero)
            };
            AmountOfTrees = 50;
            AmountOfWood = 200;
            ValueOfDiscoveredTree = 0;
            ValueOfCutWood = 1000;
            ValueOfCollision = 0;
            base.Init(nameOfInitialFile);
        }
    }
}