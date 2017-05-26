using System;
using System.Runtime.CompilerServices;
using System.Text;
using MathNet.Numerics.Distributions;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.RobotBrains
{

    /// <summary>
    /// Basic construction part of RobotBrain:
    /// </summary>
    public class Perceptron : IRobotBrain
    {
        //STATIC INITIAL  VALUES
        public static Bounds StandartBounds = RobotEntity.StandardBounds;
        /// <summary>
        /// Bounds of input/output values
        /// </summary>
        public Bounds InOutBounds { get; protected set; }
        /// <summary>
        /// Dimension of input output
        /// </summary>
        public IODimension IoDimension { get; protected set; }
        /// <summary>
        /// Specific weights of input values
        /// </summary>
        public float[] Weights;
        /// <summary>
        /// Transformation fncs transforming output
        /// </summary>
        public Func<float, float> ActivationFnc { get; protected set; }
        /// <summary>
        /// Fitness of actual brain 
        /// </summary>
        public double Fitness { get; set; }
        /// <summary>
        /// Decide readValues
        /// </summary>
        /// <param name="readValues"></param>
        /// <returns></returns>
        float[] IRobotBrain.Decide(float[] readValues)
        {
            return new[] {Decide(readValues)};
        }
        /// <summary>
        /// Create a new copy of Perceptron
        /// </summary>
        /// <returns></returns>
        public IRobotBrain GetCleanCopy()
        {
            IRobotBrain r = (IRobotBrain) this.MemberwiseClone();
            r.Fitness = 0;
            return r; 
        }

        public StringBuilder Log()
        {
            return new StringBuilder("Perceptron");
        }

        /// <summary>
        /// Decide actual input
        /// </summary>
        /// <param name="inputFloats"></param>
        /// <returns></returns>
        public float Decide(float[] inputFloats)
        {
            float dec = 0;
            if(inputFloats.Length != Weights.Length)
                throw new ArgumentException("Unsupported length of input");
            for (int i = 0; i < inputFloats.Length; i++)
            {
                dec += Weights[i] * inputFloats[i];
            }

            dec = ActivationFnc.Invoke(dec);
            if (dec > InOutBounds.Max)
                return InOutBounds.Max;

            if (dec < InOutBounds.Min)
                return InOutBounds.Min;

            return dec;
        }

        /// <summary>
        /// Generate random perceptron with given expected value & variance and give ActivationFncs 
        /// </summary>
        public static Perceptron GenerateRandom(float expectedValue, float variance,Func<float,float> activationFunc, IODimension dim, Bounds inOutBounds)
        {
            if(dim.Output != 1)
                throw new ArgumentException("Not suitable size of output for perceptron");
            var p = new Perceptron
            {
                InOutBounds = inOutBounds,
                IoDimension = dim,
                ActivationFnc = activationFunc,
                Fitness = 0
            };

            p.Weights = new float[p.IoDimension.Input];

            for (int i = 0; i < p.IoDimension.Input; i++)
            {
                p.Weights[i] = (float) Normal.Sample(expectedValue, variance);
            }

            return p;
        }
        /// <summary>
        /// Generate random perceptron with given expected value & variance and give ActivationFncs  and standard bounds
        /// </summary>
        public static Perceptron GenerateRandom(float expectedValue, float variance,
            Func<float, float> activationFunc, IODimension dim)
        {
           return  GenerateRandom(expectedValue, variance, activationFunc, dim, StandartBounds);
        }

        /// <summary>
        /// Create new brain from local and make mutation
        /// </summary>
        /// <returns></returns>
        public Perceptron CreateNewMutatedBrain(int permilleOfMutation, float varianceOfAddingValue)
        {
            var newPerceptron = (Perceptron) this.GetCleanCopy();
            newPerceptron.Mutate(permilleOfMutation,varianceOfAddingValue);

            return newPerceptron;
        }
        /// <summary>
        /// Mutated this brain with permilleOFMutation propabillity of mutation, with give Gaussian mutation
        /// </summary>
        /// <param name="permilleOfMutation"></param>
        /// <param name="varianceOfAddingValue"></param>
        public void Mutate(int permilleOfMutation, float varianceOfAddingValue)
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                if (RandomNumber.GetRandomInt(0, 1000) <= permilleOfMutation)
                    Weights[i] += (float)Normal.Sample(0, varianceOfAddingValue);
            }
        }
    }

}