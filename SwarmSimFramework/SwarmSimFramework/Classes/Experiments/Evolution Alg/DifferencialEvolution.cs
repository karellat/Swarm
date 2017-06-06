using System.Collections.Generic;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments
{
/// <summary>
/// Differencial evolution
/// </summary>
public static class DifferentialEvolution
{
    /// <summary>
    /// CrossOver propabillity [0,1] 
    /// </summary>
    public static float CR = 0.5f;

    /// <summary>
    /// Differential weight[0, 2]
    /// </summary>
    public static float F = 0.8f;

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
        while (wholeGeneration[a] == actualBrain)
            a = RandomNumber.GetRandomInt(0, wholeGeneration.Count);
        while (a == b || wholeGeneration[b] == actualBrain)
            b = RandomNumber.GetRandomInt(0, wholeGeneration.Count);
        while (a == c || b == c || wholeGeneration[c] == actualBrain)
            c = RandomNumber.GetRandomInt(0, wholeGeneration.Count);
        for (int i = 0; i < actualBrain.Neurons.Length; i++)
        {
            newNetwork[i] = DifferentialEvolutionPerceptron(actualBrain.Neurons[i], wholeGeneration[a].Neurons[i], wholeGeneration[b].Neurons[i], wholeGeneration[c].Neurons[i]);
        }

        SingleLayerNeuronNetwork newBrain = (SingleLayerNeuronNetwork)actualBrain.GetCleanCopy();
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
        Perceptron newPerceptron = (Perceptron)x.GetCleanCopy();
        float[] newWeights = new float[newPerceptron.Weights.Length];
        //Changed weight 
        int R = RandomNumber.GetRandomInt(0, newWeights.Length - 1);
        for (int i = 0; i < newWeights.Length; i++)
        {
            float U = RandomNumber.GetRandomFloat();
            if (U < CR || R == i)
                newWeights[i] = a.Weights[i] + F * (b.Weights[i] - c.Weights[i]);
            else
                newWeights[i] = x.Weights[i];
        }

        newPerceptron.Weights = newWeights;
        return newPerceptron;
    }
}
}