using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Game.Core.NeuralNetwork
{
  /// <summary>
  ///   Represents the activation function used by a neuron.
  /// </summary>
  public enum ActivationFunction
  {
    /// <summary>
    ///   Output is 0 or 1.
    /// </summary>
    HardLimit,

    /// <summary>
    ///   Output is in the range [0, 1], with linear growth.
    /// </summary>
    PiecewiseLinear,

    /// <summary>
    ///   Output is in the range [0, 1] , with growth on a sigmoid curve.
    /// </summary>
    Sigmoid
  }

  /// <summary>
  ///   Represents a neuron in a neural network.
  /// </summary>
  public class Neuron
  {
    private readonly List<float> m_inputs; 

    /// <summary>
    ///   Create a neuron.
    /// </summary>
    /// <param name="index">
    ///   This neuron's position in the layer.
    /// </param>
    /// <param name="numInputs">
    ///   The number of inputs this neuron takes.
    /// </param>
    public Neuron(int index, int numInputs)
    {
      if (index < 0) throw new ArgumentOutOfRangeException("index");
      if (numInputs < 0) throw new ArgumentOutOfRangeException("numInputs");

      Index = index;
      NumInputs = numInputs;
      ActivationFunction = ActivationFunction.HardLimit;
      Weights = new List<float>(Enumerable.Repeat(0f, NumInputs));
      m_inputs = new List<float>(Enumerable.Repeat(0f, NumInputs));
    }

    /// <summary>
    ///   The neuron's position in the layer.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    ///   The number of inputs the neuron takes.
    /// </summary>
    public int NumInputs { get; private set; }
    
    /// <summary>
    ///   The input weights.
    /// </summary>
    public IList<float> Weights { get; private set; }

    /// <summary>
    ///   The activation bias.
    /// </summary>
    public float Bias { get; set; }

    /// <summary>
    ///   The last inputs the neuron received.
    /// </summary>
    public IReadOnlyList<float> Inputs { get { return m_inputs; } }

    /// <summary>
    ///   The last output (activation) calculated by the neuron.
    /// </summary>
    public float Output { get; private set; }

    /// <summary>
    ///   The activation function in use.
    /// </summary>
    /// <remarks>
    ///   Default: Hard limit
    /// </remarks>
    public ActivationFunction ActivationFunction { get; set; }

    /// <summary>
    ///   Update the neuron with new inputs.
    /// </summary>
    /// <param name="inputs">
    ///   Inputs for the neuron.  The list size must equal 
    ///   <see cref="NumInputs"/>.
    /// </param>
    public void Update(IReadOnlyList<float> inputs)
    {
      Debug.Assert(inputs.Count == NumInputs);

      var netInput = 0f;
      for (var i = 0; i < NumInputs; i++)
      {
        m_inputs[i] = inputs[i];
        netInput += inputs[i] * Weights[i];
      }

      Output = Activation(netInput + Bias);
    }

    private float Activation(float biasedNetInput)
    {
      switch (ActivationFunction)
      {
        case ActivationFunction.HardLimit:
          return biasedNetInput < 0f ? 0f : 1f;

        case ActivationFunction.PiecewiseLinear:
          if (biasedNetInput >= 1f)
          {
            return 1f;
          }
          else if (biasedNetInput <= 0f)
          {
            return 0f;
          }
          else
          {
            return biasedNetInput;
          }

        case ActivationFunction.Sigmoid:
          return 1f / (1f + (float)Math.Pow(Math.E, -biasedNetInput));

        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
