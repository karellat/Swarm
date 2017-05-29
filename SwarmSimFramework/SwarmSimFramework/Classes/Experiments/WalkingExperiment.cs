﻿using System;
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
        //Evolution 
        public static int SizeOfGeneration = 1000;
        public static int AmountOfGenerations = 1000;
        public static int MapIteration = 1000;
        public static float MapHeight = 660;
        public static float MapWidth = 800;
        public static int AmountOfRobots = 10;
        public static RobotEntity modelRobot = new ScoutRobot(new Vector2(Single.NaN,Single.NaN),100);
        protected List<SingleLayerNeuronNetwork> actualGeneration;
        protected List<SingleLayerNeuronNetwork> followingGeneration;

        protected int actualIterationIndex;

        protected int actualGenerationIndex;

        protected int actualBrainIndex;

        protected SingleLayerNeuronNetwork actualBrain;
        //Fitness Count
        protected int[] visitedBoxes;
        protected int boxesInRow;
        protected float sizeOfBox;
        protected float emptyBoxScore = -100;
        protected float oneBoxScore = 10;
        protected float moreBoxScore = 0.5f;

        public Map.Map Map { get; protected set; }
        public void Init()
        {
            Finnished = false;
            //Prepare fitness count 
            sizeOfBox = modelRobot.Radius * 2;
            boxesInRow = ((int) Math.Ceiling(Map.MaxWidth / sizeOfBox));
            int amountofBoxes = ((int) Math.Ceiling(Map.MaxHeight / sizeOfBox)) *
                                ((int) Math.Ceiling(Map.MaxWidth / sizeOfBox));
            visitedBoxes = new int[amountofBoxes];
            
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
                //Clear map 
                Map.Reset();
                //Give brains to robots 
                foreach (var robot in Map.Robots)
                    robot.Brain = brain;
                //MakeSimulation 
                for (int i = 0; i < MapIteration; i++)
                {
                    Map.MakeStep();
                    //Count fitness
                    foreach (var r in Map.Robots)
                    {
                        CountIndividualFitness(r);
                    }
                }
                //Set created Fitness
                brain.Fitness = CountBrainFitness();
                //Reset fitness counting
                ResetFitness();
            }

        }
        /// <summary>
        /// Reset fitness of brain
        /// </summary>
        private void ResetFitness()
        {
            for (int i = 0; i < visitedBoxes.Length; i++)
                visitedBoxes[i] = 0;
        }
        /// <summary>
        /// Add one of the swarm individual to the global fitness
        /// </summary>
        /// <param name="robot"></param>
        public void CountIndividualFitness(RobotEntity robot)
        {
            //border points 
            Vector2[] vs = new Vector2[4];
            vs[0] = robot.Middle;
            vs[0].X += robot.Radius;
            vs[1] = robot.Middle;
            vs[1].X -= robot.Radius;
            vs[2] = robot.Middle;
            vs[2].Y += robot.Radius;
            vs[3] = robot.Middle;
            vs[3].Y -= robot.Radius;
            //Mark to the boxes list
            foreach (var v in vs)
            {
                //index of box
                int index = ((int) Math.Floor(v.Y / sizeOfBox)) * boxesInRow + ((int) Math.Floor(v.X / sizeOfBox));
                visitedBoxes[index]++;
            }

        }
        /// <summary>
        /// Return fitness of the swarm
        /// </summary>
        /// <returns></returns>
        public float CountBrainFitness()
        {
            float score = 0;
            //Count score for visited box 
            for (int i = 0; i < visitedBoxes.Length; i++)
            {
                if (visitedBoxes[i] == 0)
                    score += emptyBoxScore;
                else if (visitedBoxes[i] == 1)
                    score += oneBoxScore;
                else
                {
                    score += oneBoxScore;
                    score += moreBoxScore * (visitedBoxes[i] - 1);
                }
            }
            return score;
        }
        /// <summary>
        /// Make step of evolution
        /// </summary>
        public void MakeStep()
        {
            //If all generation simulated 
            if(actualGenerationIndex == AmountOfGenerations)
                Finnished = true;
            //If all new brains generated 
            if (actualBrainIndex == SizeOfGeneration)
            {
                actualGeneration = followingGeneration;
                followingGeneration = new List<SingleLayerNeuronNetwork>();
                actualBrainIndex = 0;
                actualGenerationIndex++;
            }
            //If map iteration ended
            if (actualIterationIndex == MapIteration)
            {
                actualIterationIndex = 0;
                //Choose brain with best fitness
                actualBrain.Fitness = CountBrainFitness();
                if(actualBrain.Fitness > actualGeneration[actualBrainIndex].Fitness)
                    followingGeneration.Add(actualBrain);
                else
                   followingGeneration.Add(actualGeneration[actualBrainIndex]);
                this.ResetFitness();
                Map.Reset();
                actualBrainIndex++;
                //Prepare new brain 


            }
            
        }

        public bool Finnished { get; protected set; }


    }

    public static class DifferentialEvolution
    {
        /// <summary>
        /// CrossOver propabillity [0,1] 
        /// </summary>
        public static float CR;
        /// <summary>
        /// Differential weight[0, 2]
        /// </summary>
        public static float F;
        /// <summary>
        /// Population size >= 4 
        /// </summary>
        public static int NP; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualBrain"></param>
        /// <param name="wholeGeneration"></param>
        /// <returns></returns>

        public static SingleLayerNeuronNetwork DifferentialEvolutionBrain(SingleLayerNeuronNetwork actualBrain,
            List<SingleLayerNeuronNetwork> wholeGeneration)
        {
            throw new NotImplementedException();
        }
    }
}