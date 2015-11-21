using System;
using System.Collections.Generic;

namespace Game.Core.NeuralNet.Interfaces
{
  /// <summary>
  ///   The interface for a neuron in a neural network.
  /// </summary>
  public interface INeuron
  {
    /// <summary>
    ///   Get the layer that contains the neuron.
    /// </summary>
    ILayer Layer { get; }

    /// <summary>
    ///   Get the neuron's position in the layer.
    /// </summary>
    int Index { get; }

    /// <summary>
    ///   Get the unique name for the neuron.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///   Get the number of inputs the neuron takes.
    /// </summary>
    int NumInputs { get; }

    /// <summary>
    ///   Get the input weights.  Modifying these values is allowed, but 
    ///   changing the number of weights is an invalid operation.
    /// </summary>
    IList<float> Weights { get; }

    /// <summary>
    ///   Get or set the activation bias.
    /// </summary>
    float Bias { get; set; }

    /// <summary>
    ///   Get the current inputs to the neuron.
    /// </summary>
    IReadOnlyList<float> Inputs { get; }

    /// <summary>
    ///   Get the last output (activation) calculated by the neuron.
    /// </summary>
    float Output { get; }

    /// <summary>
    ///   Update the neuron's output based on the inputs assigned to its layer.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   inputs is null
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   inputs is the wrong size
    ///   -or-
    ///   You've changed the number of <see cref="NeuronBase.Weights"/>, 
    /// you bastard.
    /// </exception>
    void Update(IReadOnlyList<float> inputs);
  }
}