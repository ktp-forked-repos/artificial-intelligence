using System;
using System.Collections.Generic;

namespace Game.Core.NeuralNet.Interfaces
{
  public interface INetwork
  {
    /// <summary>
    ///   Get the id of the network.
    /// </summary>
    int Id { get; }

    /// <summary>
    ///   Get the unique name of the network.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///   Get the number of inputs expected by the network.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The network has no layers.
    /// </exception>
    int NumInputs { get; }

    /// <summary>
    ///   Get the number of outputs produced by the network.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The network has no layers.
    /// </exception>
    int NumOutputs { get; }

    /// <summary>
    ///   Get the layers in the network.
    /// </summary>
    IReadOnlyList<ILayer> Layers { get; }

    /// <summary>
    ///   Get the last inputs to the network.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The network has no layers.
    /// </exception>
    IReadOnlyList<float> Inputs { get; }

    /// <summary>
    ///   Get the last outputs produced by the network.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The network has no layers.
    /// </exception>
    IReadOnlyList<float> Outputs { get; }

    /// <summary>
    ///   Add a layer to the neural network.  The number of inputs expected by
    ///   the layer must match the number of outputs byt the last layer in
    ///   the network (unless layer is the first layer added).
    /// </summary>
    /// <param name="layer"></param>
    /// <exception cref="ArgumentNullException">
    ///   layer is null
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   Layer's expected number of inputs is incorrect.
    /// </exception>
    void AddLayer(ILayer layer);

    /// <summary>
    ///   Update the network with new inputs.
    /// </summary>
    /// <param name="inputs"></param>
    /// <exception cref="ArgumentNullException">
    ///   inputs is null
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The network has no layers
    ///   -or-
    ///   The number of inputs is incorrect.
    /// </exception>
    void Update(IReadOnlyList<float> inputs);
  }
}