using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Core.Extensions;

namespace Game.Core.NeuralNet
{
  /// <summary>
  ///   Represents a neural network.
  /// </summary>
  public class Network
  {
    private readonly List<Layer> m_layers = new List<Layer>(); 

    /// <summary>
    ///   Get or set the id of this network.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///   Get the number of inputs expected by the network.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The network has no layers.
    /// </exception>
    public int NumInputs
    {
      get
      {
        if (!Layers.Any()) 
          throw new InvalidOperationException("The network has no layers");

        return Layers.First().NumInputs;
      }
    }

    /// <summary>
    ///   Get the number of outputs produced by the network.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The network has no layers.
    /// </exception>
    public int NumOutputs
    {
      get
      {
        if (!Layers.Any())
          throw new InvalidOperationException("The network has no layers");

        return Layers.Last().NumOutputs;
      }
    }

    /// <summary>
    ///   Get the layers in the network.
    /// </summary>
    public IReadOnlyList<Layer> Layers
    {
      get { return m_layers; }
    }

    /// <summary>
    ///   Get the last inputs to the network.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The network has no layers.
    /// </exception>
    public IReadOnlyList<float> Inputs
    {
      get
      {
        if (!Layers.Any())
          throw new InvalidOperationException("The network has no layers");

        return Layers.First().Inputs.AsReadOnly();
      }
    }

    /// <summary>
    ///   Get the last outputs produced by the network.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The network has no layers.
    /// </exception>
    public IReadOnlyList<float> Outputs
    {
      get
      {
        if (!Layers.Any())
          throw new InvalidOperationException("The network has no layers");

        return Layers.Last().Outputs;
      }
    }

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
    public void AddLayer(Layer layer)
    {
      if (layer == null) throw new ArgumentNullException("layer");
      if (Layers.Any() && NumOutputs != layer.NumInputs)
        throw new InvalidOperationException(string.Format(
          "Expected layer to take {0} inputs, but it takes {1}",
          NumOutputs, layer.NumInputs));

      m_layers.Add(layer);
    }

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
    public void Update(IReadOnlyList<float> inputs)
    {
      if (inputs == null) throw new ArgumentNullException("inputs");
      if (!Layers.Any())
        throw new InvalidOperationException("The network has no layers");
      if (inputs.Count != NumInputs)
        throw new InvalidOperationException(string.Format(
          "Expected {0} inputs, got {1}", NumInputs, inputs.Count));

      foreach (var layer in Layers)
      {
        inputs.CopyAllTo(layer.Inputs);
        layer.Update();
        inputs = layer.Outputs;
      }
    }
  }
}
