using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Extensions;
using Game.Core.NeuralNet.Interfaces;

namespace Game.Core.NeuralNet
{
  /// <summary>
  ///   The base layer implementation.
  /// </summary>
  public class Layer 
    : ILayer
  {
    private readonly List<INeuron> m_neurons;
    private readonly List<float> m_inputs; 
    private readonly List<float> m_outputs;

    /// <summary>
    ///   Construct an empty layer.
    /// </summary>
    /// <param name="network"></param>
    /// <param name="index"></param>
    /// <param name="numInputs"></param>
    /// <exception cref="ArgumentNullException">
    ///   network is null
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   index is less than 0
    ///   -or-
    ///   numInputs is 0 or less
    /// </exception>
    public Layer(INetwork network, int index, int numInputs)
    {
      if (network == null) throw new ArgumentNullException("network");
      if (index < 0) throw new ArgumentOutOfRangeException("index");
      if (numInputs <= 0) throw new ArgumentOutOfRangeException("numInputs");

      Network = network;
      Index = index;
      Name = string.Format("{0}:Layer {1}", Network.Name, Index);
      NumInputs = numInputs;
      m_inputs = new List<float>(Enumerable.Repeat(0f, NumInputs));
      m_neurons = new List<INeuron>();
      m_outputs = new List<float>();
    }

    public INetwork Network { get; private set; }

    public int Index { get; private set; }

    public string Name { get; private set; }

    public int NumInputs { get; private set; }

    public int NumOutputs
    {
      get { return m_outputs.Count; }
    }

    public IReadOnlyList<INeuron> Neurons
    {
      get { return m_neurons; }
    }

    public IReadOnlyList<float> Inputs
    {
      get { return m_inputs; }
    }

    public IReadOnlyList<float> Outputs
    {
      get { return m_outputs; }
    }

    public void AddNeuron(INeuron neuron)
    {
      if (neuron == null) throw new ArgumentNullException("neuron");
      if (!ReferenceEquals(this, neuron.Layer))
        throw new InvalidOperationException(
          "neuron is not assigned to this layer");
      if (neuron.NumInputs != NumInputs)
        throw new InvalidOperationException(string.Format(
          "Expected {0} inputs, neuron had {1}", NumInputs, neuron.NumInputs));

      m_neurons.Add(neuron);
      m_outputs.Add(0f);
    }

    public void Update(IReadOnlyList<float> inputs)
    {
      if (inputs == null) throw new ArgumentNullException("inputs");
      if (inputs.Count != NumInputs)
        throw new InvalidOperationException(string.Format(
          "Expected {0} inputs, got {1}", NumInputs, inputs.Count));
      if (!Neurons.Any()) 
        throw new InvalidOperationException("No neurons in the layer");

      inputs.CopyAllTo(m_inputs);
      for (var i = 0; i < Neurons.Count; i++)
      {
        Neurons[i].Update(inputs);
        m_outputs[i] = Neurons[i].Output;
      }
    }
  }
}
