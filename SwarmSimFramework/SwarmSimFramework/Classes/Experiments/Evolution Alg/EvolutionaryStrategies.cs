﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.Statistics;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments
{


    public class EvolutionaryStrategies
    {
        private struct Mutation
        {

            public static Mutation Zero(int size)
            {
                Mutation m;
                m.weigths = new float[size];
                Debug.Assert(m.weigths.All(f => f == 0.0f));
                return m;
            }

            public float[] weigths;

            public static BrainModel<SingleLayerNeuronNetwork>[] operator +(
                BrainModel<SingleLayerNeuronNetwork>[] brain, Mutation m)
            {
                BrainModel<SingleLayerNeuronNetwork>[] newBrains =
                    new BrainModel<SingleLayerNeuronNetwork>[brain.Length];
                for (int i = 0; i < newBrains.Length; i++)
                {
                    newBrains[i] = new BrainModel<SingleLayerNeuronNetwork>
                    {
                        Brain = (SingleLayerNeuronNetwork) brain[i].Brain.GetCleanCopy(),
                        Robot = brain[i].Robot
                    };
                }
#if DEBUG
                int expectedSizeOfWeights = 0;
                foreach (var b in newBrains)
                {
                    foreach (var n in b.Brain.Neurons)
                        expectedSizeOfWeights += n.Weights.Length;
                }
                Debug.Assert(m.weigths.Length == expectedSizeOfWeights);
#endif

                int index = 0;
                for (int i = 0; i < newBrains.Length; i++)
                {
                    newBrains[i].Brain = (SingleLayerNeuronNetwork) newBrains[i].Brain.ChangeWeights(
                        m.weigths.SubArray(index, newBrains[i].Brain.GetWeigths().Length), (origin, change) =>
                        {
                            return origin + change;
                        });
                }
                return newBrains;
            }

            public static Mutation operator *(Mutation a, float b)
            {

                var newWeights = new float[a.weigths.Length];
                for (int i = 0; i < a.weigths.Length; i++)
                    newWeights[i] = a.weigths[i] * b;

                return new Mutation() {weigths = newWeights};
            }

            public static Mutation operator +(Mutation a, Mutation b)
            {
                if (a.weigths.Length != b.weigths.Length)
                    throw new ArgumentException("a and b have different dimensions");
                Mutation c;

                c.weigths = new float[a.weigths.Length];
                for (int i = 0; i < c.weigths.Length; i++)
                    c.weigths[i] = a.weigths[i] + b.weigths[i];

                return c;

            }

        }

        public string Name = "nameOfES";

        public string WorkingDir = "ES";

        /// <summary>
        /// Number of evaluated brains
        /// </summary>
        public int PopulationSize = 2;

        /// <summary>
        /// Turn on elitism
        /// </summary>
        public bool Elitism = false;

        /// <summary>
        /// Maximum of best brains 
        /// </summary>
        public int bestBrainsCount = 20;

        /// <summary>
        /// Mutation step mutation size
        /// </summary>
        public int SingleStepPopulationSize = 20;

        /// <summary>
        /// Number of generation(jitters in terms of evolutionary strategies
        /// </summary>
        public int NumberOfGenerations = 10000;

        /// <summary>
        /// Number of iterations of map 
        /// </summary>
        public int NumberOfMapIterations = 2000;

        /// <summary>
        /// Noise standard deviation 
        /// </summary>
        public float sigma = 0.1f;

        /// <summary>
        /// Learning rate
        /// </summary>
        public float alpha = 0.05f;

        /// <summary>
        /// Model of current map
        /// </summary>
        private MapModel mapModel;

        /// <summary>
        /// Fitness function 
        /// </summary>
        private IFitnessCounter fitnessFunction;

        /// <summary>
        /// Models of curently evolved brains
        /// </summary>
        private BrainModel<SingleLayerNeuronNetwork>[][] brainModels;


        private void WriteGraph(int indexOfIndividual, double actualFitness, int genIndex)
        {
            Graphs[indexOfIndividual].WriteLine(genIndex.ToString() + ";" + actualFitness.ToString());
#if DEBUG
            Graphs[indexOfIndividual].Flush();
#endif
        }

        private void CloseGraphs()
        {
            foreach (var g in Graphs)
            {
                g.Close();
            }       
        }

    /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <param name="mapModel"></param>
        /// <param name="modelsOfBrains"></param>
        public EvolutionaryStrategies(IFitnessCounter f, MapModel mapModel, BrainModel<SingleLayerNeuronNetwork>[][] modelsOfBrains)
        {
            this.mapModel = mapModel;
            this.fitnessFunction = f;
            this.brainModels = modelsOfBrains;

            PopulationSize = modelsOfBrains.Length;


        }

        public void Run()
        {
            //Prepare dir
            System.IO.Directory.CreateDirectory(WorkingDir);
            PrepareFiles();
            var tasks = Enumerable.Range(0, PopulationSize).Select(i =>
                {
                    Console.WriteLine("My {0}",i.ToString());
                    return Task.Run(() => SingleIndividualRun(i));
                })
                .ToArray();             //Init number of threads same as the number of evolving individual
            Task.WaitAll(tasks);    
            
            Console.WriteLine("All threads finnished");
            CloseGraphs();
            //Serialize brains
        }

        public void SingleIndividualRun(int indexOfIndividual)
        {
            //
            var bestBrains = new SortedList<double,BrainModel<SingleLayerNeuronNetwork>[]>(new IRobotDoubleComperer());
            BrainModel<SingleLayerNeuronNetwork>[] myBrainModels = new BrainModel<SingleLayerNeuronNetwork>[brainModels[indexOfIndividual].Length];
            double actualFitness = 0;
        

            for (int i = 0; i < myBrainModels.Length; i++)
                myBrainModels[i] = brainModels[indexOfIndividual][i];

            MapSimulation<SingleLayerNeuronNetwork> simulation = new MapSimulation<SingleLayerNeuronNetwork>(mapModel, myBrainModels);

            var map = simulation.Simulate(NumberOfMapIterations);
            actualFitness = fitnessFunction.GetMapFitness(map);
            StringBuilder robotsString = new StringBuilder();

            foreach (var brain in myBrainModels)
                robotsString.Append(brain.Robot.Name + ";");

            //Log the summary fitness 
            Console.WriteLine("Thread " + indexOfIndividual + ". of Evolutionary Strategies \n" +
                              "started with robots " + robotsString.ToString() + "\n" +
                              "Evaluated with: " + actualFitness.ToString("##.000"));

            int totalDimension = 0;

            foreach (var b in myBrainModels)
            {
                foreach (var p in b.Brain.Neurons)
                    totalDimension += p.Weights.Length;
            }


            for (int genIndex = 0; genIndex < NumberOfGenerations; genIndex++)
            {
                double[] F = new double[SingleStepPopulationSize];
                Mutation[] M = new Mutation[SingleStepPopulationSize];

                for (int mutIndex = 0; mutIndex < SingleStepPopulationSize; mutIndex++)
                {
                    M[mutIndex] = CreateMutation(totalDimension);
                    var actualModels = myBrainModels + M[mutIndex];
                    simulation.Reset(actualModels);
                    F[mutIndex] = fitnessFunction.GetMapFitness(simulation.Simulate(NumberOfMapIterations));
                }
#if DEBUG
                StringBuilder s = new StringBuilder();
                for (int i = 0; i < M.Length; i++)
                    s.Append("Fitness of " + i + ". mutation = " + F[i] + " \n");

                Console.WriteLine("Thread {0}, finnnished {1}.generation \n With Folowing fitnesses \n {2}",indexOfIndividual,genIndex,s.ToString());
#endif

                var mean = F.Mean();
                var std = F.StandardDeviation();
                double[] A = new double[F.Length];
                for (int i = 0; i < F.Length; i++)
                    A[i] = std != 0 ? (F[i] - mean) / std : 0.001; //standardize the rewards to have a gaussian distribution

               
                Mutation sumOfMutation = Mutation.Zero(totalDimension); 

                for (int i = 0; i < A.Length; i++)
                {
                    var m = M[i] * (float) A[i];
                    sumOfMutation = sumOfMutation + m; 
                }
                // perform the brains update.
                sumOfMutation = sumOfMutation * (alpha / (SingleStepPopulationSize * sigma));
                var offspringBrainModels = myBrainModels + sumOfMutation;
                // simulate with changed brain
                simulation.Reset(offspringBrainModels);
                var offspringFitness = fitnessFunction.GetMapFitness(simulation.Simulate(NumberOfMapIterations));


                //Log the summary 
                //Log the summary fitness 
                Console.WriteLine("Thread " + indexOfIndividual + ". of Evolutionary Strategies, iteration " + genIndex+ ". \n" +
                                  "Evaluated with: " + offspringFitness.ToString("##.000") + " best fitness: " + actualFitness);

                //Add to best brains if 
                if (bestBrains.Count < bestBrainsCount || bestBrains.Last().Key < offspringFitness)
                {
                    //If full remove last one
                   if(bestBrains.Count >= bestBrainsCount)
                        bestBrains.RemoveAt(bestBrains.Count-1);
                    bestBrains.Add(offspringFitness,offspringBrainModels);
                }

                if (!Elitism || actualFitness <= offspringFitness)
                {
                    actualFitness = offspringFitness;
                    myBrainModels = offspringBrainModels;
                }
                WriteGraph(indexOfIndividual, actualFitness, genIndex);
                Serialize(myBrainModels,indexOfIndividual,genIndex.ToString());
               
            }
            int index = 0; 
            foreach (var bests in bestBrains)
            {
                Serialize(bests.Value, indexOfIndividual, "best" + index.ToString());
                index++; 
            }
            Console.WriteLine("Thread with {0}. individual finnished the evaluation",indexOfIndividual);

        }

        private void PrepareFiles()
        {
            Graphs = new StreamWriter[PopulationSize];
            ThreadDirs = new string[PopulationSize];
            for (int i = 0; i < PopulationSize; i++)
            {
                Graphs[i] = new StreamWriter(WorkingDir + "//graph_of_" + i + ".txt");
                var tdir = WorkingDir + "//" + i.ToString() + "thread";
                System.IO.Directory.CreateDirectory(tdir);
                ThreadDirs[i] = tdir;
            }
        }

        private StreamWriter[] Graphs;
        private string[] ThreadDirs; 


        private void Serialize(BrainModel<SingleLayerNeuronNetwork>[] myBrainModels,int threadID, string stringId)
        {
            for (int i = 0; i < myBrainModels.Length; i++)
            {
                StreamWriter output = new StreamWriter(ThreadDirs[threadID] + "//" + stringId + "_" + myBrainModels[i].Robot.Name);
                output.Write(JsonConvert.SerializeObject(myBrainModels[i].Brain, BrainSerializer.JsonSettings));
                output.Close();
            }
        }

        private Mutation CreateMutation(int numberWeights)
        {
            Mutation m;
            m.weigths = new float[numberWeights];
            //Normal distribution sample
            for (int i = 0; i < m.weigths.Length; i++)
                m.weigths[i] = (float)Normal.Sample(0, 1);

            return m;
        }
    }
}