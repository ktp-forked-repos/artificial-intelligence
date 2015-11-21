using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.NeuralNet.Interfaces;

namespace Game.Core.NeuralNet
{
  /// <summary>
  ///   The base for neuron implementations.
  /// </summary>
  public abstract class NeuronBase 
    : INeuron
  {
    /// <summary>
    ///   Create a neuron.  The number of inputs expected are determined by the
    ///   assigned layer.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="index">
    ///   This neuron's position in the layer.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   layer is null
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   index is less than 0
    /// </exception>
    protected NeuronBase(ILayer layer, int index)
    {
      if (layer == null) throw new ArgumentNullException("layer");
      if (index < 0) throw new ArgumentOutOfRangeException("index");

      Layer = layer;
      Index = index;
      Name = string.Format("{0}:Neuron {1}", Layer.Name, Index);
      Weights = new List<float>(Enumerable.Repeat(0f, NumInputs));
    }

    public ILayer Layer { get; private set; }

    public int Index { get; private set; }

    public string Name { get; private set; }

    public int NumInputs
    {
      get { return Layer.NumInputs; }
    }
    
    public IList<float> Weights { get; private set; }

    public float Bias { get; set; }

    public IReadOnlyList<float> Inputs
    {
      get { return Layer.Inputs; }
    }

    public float Output { get; private set; }

    public void Update(IReadOnlyList<float> inputs)
    {
      if (inputs == null) throw new ArgumentNullException("inputs");
      if (inputs.Count != NumInputs)
        throw new InvalidOperationException(string.Format(
          "Expected {0} inputs, got {1}", NumInputs, inputs.Count));
      if (Weights.Count != NumInputs)
        throw new InvalidOperationException("Invalid number of weights");

      var netInput = 0f;
      for (var i = 0; i < NumInputs; i++)
      {
        netInput += inputs[i] * Weights[i];
      }

      Output = Activation(netInput + Bias);
    }

    /// <summary>
    ///   Calculates and returns the neuron's activation based on the provided
    ///   input.
    /// </summary>
    /// <param name="biasedNetInput"></param>
    /// <returns></returns>
    protected abstract float Activation(float biasedNetInput);
  }
}
