using System;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.RobotBrains
{
    /// <summary>
    /// Single layer of weight mean brain:
    /// </summary>
    public class WeightMeanBrain : IRobotBrain
    {
        //MEMBERs 
        /// <summary>
        /// Fitness of the brain, given by evaluator 
        /// </summary>
        public double Fitness { get; set; }
        /// <summary>
        /// Bounds of input & output 
        /// </summary>
        public Bounds InOutBounds { get; }
        /// <summary>
        /// Local bounds of intern values
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// Length of input
        /// </summary>
        public int InputDimension { get; }
        /// <summary>
        /// Length of output
        /// </summary>
        public int OutputDimension { get; }
        /// <summary>
        /// Normalization funcs to normalize output
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; }
        /// <summary>
        /// Specific weights [outputDim][inputDim] 
        /// </summary>
        protected float[][] Weights;
        //Constructor
        /// <summary>
        /// Create new WeightMeanBrain with given brains
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="inOutBounds"></param>
        /// <param name="inputDimension"></param>
        /// <param name="outputDimension"></param>
        /// <param name="weights"></param>
        public WeightMeanBrain(Bounds inOutBounds ,int inputDimension, int outputDimension, float[][] weights)
        {
            //Check lengths 
           if(inputDimension != weights[0].Length)
                throw new ArgumentException("Not suitable weights, input does not fit length");
           if(outputDimension != weights.Length)
               throw new ArgumentException("Not suitable weights, output does not fit length");

            Weights = weights;
            InputDimension = inputDimension;
            OutputDimension = outputDimension;
            //Local bounds 
            InOutBounds = inOutBounds;
            LocalBounds = new Bounds[OutputDimension];
            for (int i = 0; i < weights.Length; i++)
            {
                float max = 0;
                float min = 0;
                foreach (var w in weights[i])
                {
                    float w1 = w * InOutBounds.Max;
                    float w2 = w * InOutBounds.Min;
                    if (w1 > w2)
                    {
                        max += w1;
                        min += w2;
                    }
                    else
                    {
                        max += w2;
                        min += w1;
                    }
                }
                LocalBounds[i] = new Bounds() {Max = max, Min = min};
            }
            //Make normalization funcs
            NormalizeFuncs = Entity.MakeNormalizeFuncs(LocalBounds, InOutBounds);
        }
        /// <summary>
        /// Count ouput from given values
        /// </summary>
        /// <param name="readValues"></param>
        /// <returns></returns>
        public float[] Decide(float[] readValues)
        {
            //Count output 
            float[] o = new float[OutputDimension];
            for (int i = 0; i < OutputDimension; i++)
            {
                float c = 0;
                for (int j = 0; j < InputDimension; j++)
                    c += Weights[i][j] * readValues[j];
                o[i] = c;
            }
            return o.Normalize(NormalizeFuncs);
        }
        /// <summary>
        /// Create clean copy of brain
        /// </summary>
        /// <returns></returns>
        public IRobotBrain GetCleanCopy()
        {
            return (IRobotBrain) this.MemberwiseClone();
        }
        //Static Methods 
        /// <summary>
        /// Generate new WeightMeanBrain
        /// </summary>
        /// <returns></returns>
        public static WeightMeanBrain GenerateRandomWeightMeanBrain(Bounds inOutBounds, int inputDimension, int outputDimension)
        {
             float[][] weights = new float[outputDimension][];
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = new float[inputDimension];
                for (int j = 0; j < weights[i].Length; j++)
                {
                    //Make random values 
                    weights[i][j] = RandomNumber.GetRandomInt((int) WeightedBounds.Min,
                        RandomNumber.GetRandomInt((int) WeightedBounds.Max));
                }
            }
            return new WeightMeanBrain(inOutBounds,inputDimension,outputDimension,weights);
        }
        /// <summary>
        /// Return new brain with mutated weights 
        /// </summary>
        /// <param name="mutatingBrain"></param>
        /// <param name="permileOfMutation"></param>
        /// <param name=""></param>
        /// <param name="addingConstant"></param>
        /// <returns></returns>
        public static WeightMeanBrain MutateBrainAdd(WeightMeanBrain mutatingBrain, int permileOfMutation,
            int addingConstant)
        {
            float[][] newWeights = new float[mutatingBrain.OutputDimension][];
            for (int i = 0; i < mutatingBrain.OutputDimension; i++)
            {
                newWeights[i] = new float[mutatingBrain.InputDimension];
                for (int j = 0; j < mutatingBrain.InputDimension; j++)
                {
                    newWeights[i][j] = mutatingBrain.Weights[i][j];
                    if (RandomNumber.GetRandomInt(0, 1000) <= permileOfMutation)
                        newWeights[i][j] += RandomNumber.GetRandomInt(-addingConstant, addingConstant);
                }
            }

            return new WeightMeanBrain(mutatingBrain.InOutBounds,mutatingBrain.InputDimension,mutatingBrain.OutputDimension, newWeights);
        }
        /// <summary>
        /// Bounds of weight constant 
        /// </summary>
        public static Bounds WeightedBounds = new Bounds() {Max = 100.0f, Min = -100.0f};
        
    }


}