using System;
using System.Diagnostics.Tracing;
using System.Text;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.RobotBrains
{
    public class SingleLayerNeuronNetwork : IRobotBrain
    {
        /// <summary>
        /// expected values of weights 
        /// </summary>
        public static float ExpectedValue = 0;

        /// <summary>
        /// variance of weights 
        /// </summary>
        public static float Variance = 0.1f;
        /// <summary>
        /// Variance of adding values of mutation 
        /// </summary>
        public static float VarianceOfGaussianMutations = 0.05f;

        /// <summary>
        /// Permille propability of mutation
        /// </summary>
        public static int PermilleOfMutation = 50;

        /// <summary>
        /// Activation funcs 
        /// </summary>
         [JsonIgnore]
        protected static Func<float, float> ActivationFunc => (float x) => ActivationFuncs.ResieTanh(100, x);

        /// <summary>
        /// Fitness of current brain
        /// </summary>
        public double Fitness { get; set; }
        /// <summary>
        /// Transformation fncs for output
        /// </summary>
        [JsonIgnore]
        public Func<float, float> ActivationFnc => SingleLayerNeuronNetwork.ActivationFunc;

        /// <summary>
        /// Input & Output dimensions
        /// </summary>
        [JsonProperty]
        public IODimension IoDimension { get; protected set; }
        /// <summary>
        /// bounds for input & outpus
        /// </summary>
        [JsonProperty]
        public Bounds InOutBounds { get; protected set; }
        /// <summary>
        /// Intern neurons of single output 
        /// </summary>
        public Perceptron[] Neurons { get; set; }
        /// <summary>
        /// Make all neurons decide, set input
        /// </summary>
        /// <param name="readValues"></param>
        /// <returns></returns>
        public float[] Decide(float[] readValues)
        {
           float[] o = new float[IoDimension.Output];
            for (int i = 0; i < IoDimension.Output; i++)
            {
                o[i] = Neurons[i].Decide(readValues);
            }

            return o;
        }
        /// <summary>
        /// Generate clean copy of brain
        /// </summary>
        /// <returns></returns>
        public IRobotBrain GetCleanCopy()
        {
            var nn = (SingleLayerNeuronNetwork) this.MemberwiseClone();
            for (int i = 0; i < this.IoDimension.Output; i++)
            {
                Neurons[i] = (Perceptron) Neurons[i].GetCleanCopy();
            }

            return nn;

        }

        public StringBuilder Log()
        {
            return new StringBuilder("SingleLayredNeuronNetwork");
        }

        IRobotBrain IRobotBrain.DeserializeBrain(string jsonString)
        {
            return DeserializeBrain(jsonString);
        }

        public  static IRobotBrain DeserializeBrain(string jsonString)
        {
            return JsonConvert.DeserializeObject<Perceptron>(jsonString, BrainSerializer.JsonSettings);
        }

        public string SerializeBrain()
        {
            return JsonConvert.SerializeObject(this, BrainSerializer.JsonSettings);
        }

        /// <summary>
        /// Mutate every percepton 
        /// </summary>
        public void Mutate()
        {
            foreach (var n in Neurons)
            {
                n.Mutate(PermilleOfMutation,VarianceOfGaussianMutations);
            }  
        }
        /// <summary>
        /// Generate random single layered network with given static values
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static SingleLayerNeuronNetwork GenerateNewRandomNetwork(IODimension dimension)
        {
            var nn = new SingleLayerNeuronNetwork
            {
                IoDimension = dimension,
                InOutBounds = RobotEntity.StandardBounds
            };
            nn.Neurons = new Perceptron[nn.IoDimension.Output];
            for (int i = 0; i < nn.IoDimension.Output; i++)
            {
                nn.Neurons[i] = Perceptron.GenerateRandom(ExpectedValue,Variance,nn.ActivationFnc,new IODimension() {Input = nn.IoDimension.Input, Output = 1});
            }

            return nn;
        }
        /// <summary>
        /// Return new mutated brain
        /// </summary>
        /// <returns></returns>
        public SingleLayerNeuronNetwork CreateMutatedNetwork()
        {
            var nn = (SingleLayerNeuronNetwork) this.GetCleanCopy();
            nn.Mutate();
            return nn;
        }

        public override string ToString()
        {
            return base.ToString() + " - " + Fitness.ToString();
        }
    }
}