using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MathNet.Numerics;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments
{
    public class WalkingExperiment: IExperiment
    {
        /// <summary>
        /// Num of evolution algorithm
        /// </summary>
        public int numOfEvolutionAlg = 1; 
        //Evolution 
       
        public static int PopulationSize = 100;
        public static int NumberOfGenerations = 1000;
        public static int MapIteration = 1000;
        public static float MapHeight = 660;
        public static float MapWidth = 800;
        public static int AmountOfRobots = 10;
        public static RobotEntity modelRobot = new ScoutRobot(new Vector2(0,0),100);
        public static string initGenerationFile = "initGen.json";
        //State variable
        protected List<SingleLayerNeuronNetwork> actualGeneration;
        protected List<SingleLayerNeuronNetwork> followingGeneration = new List<SingleLayerNeuronNetwork>();
        protected int actualIterationIndex;
        protected int actualGenerationIndex;
        protected int actualBrainIndex;
        //Serialization
        protected string serializerFolderDir = "bestBrains";


        protected SingleLayerNeuronNetwork actualBrain;
        //Fitness Count
        protected int[] visitedBoxes;
        protected int boxesInRow;
        protected float sizeOfBox;
        protected float emptyBoxScore = 0;
        protected float oneBoxScore = 10;
        protected float moreBoxScore = 0.5f;

        public Map.Map Map { get; protected set; }
        public void Init()
        {
            Finnished = false;
            //Prep  are folder for serialized brains
            System.IO.Directory.CreateDirectory(serializerFolderDir);

            //Prepare fitness count 
            sizeOfBox = modelRobot.Radius * 2;
            boxesInRow = ((int) Math.Ceiling(MapHeight / sizeOfBox));
            int amountofBoxes = ((int) Math.Ceiling(MapHeight / sizeOfBox)) *
                                ((int) Math.Ceiling(MapWidth / sizeOfBox));
            visitedBoxes = new int[amountofBoxes];
            
            List<RobotEntity> robots = new List<RobotEntity>(AmountOfRobots);
            float gap = MapHeight / (AmountOfRobots +1);
            for (int i = 0; i < 10; i++)
            {
                var newRobot = (RobotEntity) modelRobot.DeepClone();
                newRobot.MoveTo(new Vector2(10, gap + i * gap)); 
                newRobot.RotateDegrees(-90);
                robots.Add(newRobot);
            }
            //Prepare map 
            Map = new Map.Map(MapHeight, MapWidth,robots);  
            //Prepare brains || read brains from  file
            if (File.Exists(initGenerationFile))
            {
                StreamReader s = new StreamReader(initGenerationFile);

                actualGeneration = BrainSerializer.DeserializeArray<SingleLayerNeuronNetwork>(s.ReadToEnd()).ToList();
            }
            else
            {
                actualGeneration = new List<SingleLayerNeuronNetwork>(PopulationSize);
                for (int i = 0; i < PopulationSize; i++)
                {
                    actualGeneration.Add(SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension()
                    {
                        Input = modelRobot.SensorsDimension,
                        Output = modelRobot.EffectorsDimension
                    }));
                }
                //Count fitness of brains 

                int brainI = -1;
                foreach (var brain in actualGeneration)
                {
                    brainI++;
                    info = new StringBuilder("Inicialization of random brain: " + brainI);
                    //Clear map 
                    Map.Reset();
                    //Give brains to robots 
                    foreach (var robot in Map.Robots)
                        robot.Brain = brain;
                    //MakeSimulation 
                    for (int i = 0; i < MapIteration; i++)
                    {
                        Map.MakeStep();
                        // DEBUG 
                        Map.CheckCorrectionOfPossition();
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
                StreamWriter file = new StreamWriter(initGenerationFile);
                file.Write(BrainSerializer.SerializeArray(actualGeneration.ToArray()));
                file.Close();
            }
            //prepare first brain for evolution
            actualBrain = actualGeneration[0];
            actualBrain.Fitness = 0;
            foreach (var r in Map.Robots)
            {
                r.Brain = actualBrain.GetCleanCopy();
                
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
            if(actualGenerationIndex == NumberOfGenerations)
                Finnished = true;
            //If all new brains generated 
            if (actualBrainIndex == PopulationSize-1)
                SingleGeneration();
            //If map iteration ended
            if (actualIterationIndex == MapIteration)
                SingleMapSimulation();

            //Make one iteration of map 
            Map.MakeStep();
            actualIterationIndex++;
            StringBuilder newInfo = new StringBuilder("Actual map iteration: ");
            newInfo.Append(actualIterationIndex);
            newInfo.Append(" Actual brain from population: ");
            newInfo.Append(actualBrainIndex);
            newInfo.Append(" Actual generation index: ");
            newInfo.Append(actualGenerationIndex);
            ExperimentInfo = newInfo;
            //Count fitness for all robot bodies
            foreach (var r in Map.Robots)
            {
                CountIndividualFitness(r);
            }
        }
        //If experiment finished
        public bool Finnished { get; protected set; }

        //Single steps of makestep 
        /// <summary>
        /// End of generation
        /// </summary>
        protected void SingleGeneration ()
        {
            actualGeneration = followingGeneration;
            followingGeneration = new List<SingleLayerNeuronNetwork>();
            actualBrainIndex = 0;
            actualGenerationIndex++;
            //Log actual generation
        
            var i = GenerationInfoStruct.GetGenerationInfo(actualGeneration);
            StringBuilder sb = new StringBuilder("Info about " + (actualGenerationIndex-1) + ". generation " );
            sb.AppendLine("Best fitness: " + i.FitnessMaximum);
            sb.AppendLine("Worst fitness " + i.FitnessMinimum);
            sb.AppendLine("Average fitness: " + i.FitnessAverage);
            sb.AppendLine("Brain: " + i.BestBrainInfo);
            GenerationInfo = sb;
            //Serialize brain 
            StreamWriter n = new StreamWriter(serializerFolderDir+"\\brain"+(actualGenerationIndex-1));
            n.Write(i.BestBrain.SerializeBrain());
            n.Close();
        }

        protected void SingleMapSimulation()
        {
            //Start new iteration cycle 
            actualIterationIndex = 0;
            //Choose brain with best fitness to push 
            actualBrain.Fitness = CountBrainFitness();
            if (actualBrain.Fitness > actualGeneration[actualBrainIndex].Fitness)
                followingGeneration.Add(actualBrain);
            else
                followingGeneration.Add(actualGeneration[actualBrainIndex]);
            this.ResetFitness();
            Map.Reset();
            
            //Prepare new brain use given evolution algorithm 
            if (numOfEvolutionAlg == 1)
                actualBrain = DifferentialEvolution.DifferentialEvolutionBrain(actualGeneration[actualBrainIndex],
                    actualGeneration);
            else
                actualBrain =
                    EvolutionWithMutation.MutateSingleLayerNeuronNetwork(actualGeneration[actualBrainIndex]);

            actualBrainIndex++;
            //set brain to robot bodies,Reset fitness
            actualBrain.Fitness = 0;
            foreach (var r in Map.Robots)
            {
                r.Brain = actualBrain.GetCleanCopy();
            }
        }

        protected StringBuilder info = new StringBuilder("Walking experiment: ");
        /// <summary>
        /// Thread safe info about Experiment
        /// </summary>
        public StringBuilder ExperimentInfo
        {
            get
            {
                lock (InfoLock)
                {
                    return info;
                }
            }
            protected set
            {
                lock (InfoLock)
                {
                    info = value;
                }
            }
        }

        private StringBuilder generationInfo;
        public StringBuilder GenerationInfo {
            get
            {
                lock (GenerationInfoLock)
                {
                    //Clean info
                    var v = generationInfo;
                    generationInfo = null;
                     return v;
                }

            }
            protected set { lock(GenerationInfoLock){generationInfo = value;} }
        }

        public bool FinnishedGeneration
        {
            get
            {
                lock (GenerationInfoLock)
                {
                    return generationInfo != null;
                }
            }
        }

        public object InfoLock { get; } = new Object();
        public object GenerationInfoLock = new Object();
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
        public static float F = 0.8f;
        /// <summary>
        /// Population size >= 4 
        /// </summary>
        public static int NP; 

        /// <summary>
        /// Make Differencial evolution on  brain 
        /// </summary>
        /// <param name="actualBrain"></param>
        /// <param name="wholeGeneration"></param>
        /// <returns></returns>

        public static SingleLayerNeuronNetwork DifferentialEvolutionBrain(SingleLayerNeuronNetwork actualBrain,
            List<SingleLayerNeuronNetwork> wholeGeneration)
        {
            Perceptron[] newNetwork = new Perceptron[actualBrain.Neurons.Length];
            // Three unique numbers from generation 
            int a, b, c;
            a = RandomNumber.GetRandomInt(0, wholeGeneration.Count);
            b = 0;
            c = 0; 
            while(wholeGeneration[a] == actualBrain)
                a = RandomNumber.GetRandomInt(0, wholeGeneration.Count);
            while (a == b || wholeGeneration[b] == actualBrain)
                b = RandomNumber.GetRandomInt(0, wholeGeneration.Count);
            while(a == c || b == c || wholeGeneration[c] == actualBrain)
                c = RandomNumber.GetRandomInt(0, wholeGeneration.Count);
            for (int i = 0; i < actualBrain.Neurons.Length; i++)
            {
                newNetwork[i] = DifferentialEvolutionPerceptron(actualBrain.Neurons[i],wholeGeneration[a].Neurons[i], wholeGeneration[b].Neurons[i], wholeGeneration[c].Neurons[i]);
            }

            SingleLayerNeuronNetwork newBrain = (SingleLayerNeuronNetwork) actualBrain.GetCleanCopy();
            newBrain.Neurons = newNetwork;
            return newBrain;
        }
        /// <summary>
        /// Create DifferencialEvolution on Perceptron 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Perceptron DifferentialEvolutionPerceptron(Perceptron x, Perceptron a, Perceptron b, Perceptron c)
        {
            Perceptron newPerceptron = (Perceptron) x.GetCleanCopy();
            float[] newWeights = new float[newPerceptron.Weights.Length];
            for (int i = 0; i < newWeights.Length; i++)
            {
                float U = RandomNumber.GetRandomFloat();
                if (U < CR)
                    newWeights[i] = a.Weights[i] + F * (b.Weights[i] - c.Weights[i]);
                else
                    newWeights[i] = x.Weights[i];
            }

            newPerceptron.Weights = newWeights;
            return newPerceptron;
        }
    }

    public static class EvolutionWithMutation
    {
        public static SingleLayerNeuronNetwork MutateSingleLayerNeuronNetwork(SingleLayerNeuronNetwork actualBrain)
        {
            return actualBrain.CreateMutatedNetwork();
        }
    }
}