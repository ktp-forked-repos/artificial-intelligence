using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Extensions;

namespace Game.Core.NeuralNet
{
  /// <summary>
  ///   Represents a neuron in a neural network.
  /// </summary>
  public abstract class NeuronBase
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
    protected NeuronBase(Layer layer, int index)
    {
      if (layer == null) throw new ArgumentNullException("layer");
      if (index < 0) throw new ArgumentOutOfRangeException("index");

      Layer = layer;
      Index = index;
      Weights = new List<float>(Enumerable.Repeat(0f, NumInputs));
    }

    /// <summary>
    ///   Get the layer that contains this neuron.
    /// </summary>
    public Layer Layer { get; private set; }

    /// <summary>
    ///   Get the neuron's position in the layer.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    ///   Get the number of inputs the neuron takes.
    /// </summary>
    public int NumInputs
    {
      get { return Layer.NumInputs; }
    }
    
    /// <summary>
    ///   Get the input weights.  Modifying these values is expected.  Changing
    ///   the number of weights is an invalid operation.
    /// </summary>
    public IList<float> Weights { get; private set; }

    /// <summary>
    ///   Get or set the activation bias.
    /// </summary>
    public float Bias { get; set; }

    /// <summary>
    ///   Get the current inputs to the neuron.
    /// </summary>
    public IReadOnlyList<float> Inputs
    {
      get { return Layer.Inputs.AsReadOnly(); }
    }

    /// <summary>
    ///   Get the last output (activation) calculated by the neuron.
    /// </summary>
    public float Output { get; private set; }

    /// <summary>
    ///   Update the neuron's output based on the inputs assigned to its layer.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   You've changed the number of <see cref="Weights"/>, you bastard.
    /// </exception>
    public void Update()
    {
      if (Weights.Count != Inputs.Count)
        throw new InvalidOperationException("Invalid number of weights");

      var netInput = 0f;
      for (var i = 0; i < NumInputs; i++)
      {
        netInput += Inputs[i] * Weights[i];
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
