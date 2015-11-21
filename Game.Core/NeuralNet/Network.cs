using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.NeuralNet.Interfaces;

namespace Game.Core.NeuralNet
{
  /// <summary>
  ///   The base network implementation
  /// </summary>
  public class Network 
    : INetwork
  {
    private readonly List<ILayer> m_layers = new List<ILayer>();

    public Network(int id)
    {
      Id = id;
      Name = string.Format("Network {0}", Id);
    }

    public int Id { get; private set; }

    public string Name { get; private set; }
    
    public int NumInputs
    {
      get
      {
        if (!Layers.Any()) 
          throw new InvalidOperationException("The network has no layers");

        return Layers.First().NumInputs;
      }
    }

    public int NumOutputs
    {
      get
      {
        if (!Layers.Any())
          throw new InvalidOperationException("The network has no layers");

        return Layers.Last().NumOutputs;
      }
    }

    public IReadOnlyList<ILayer> Layers
    {
      get { return m_layers; }
    }

    public IReadOnlyList<float> Inputs
    {
      get
      {
        if (!Layers.Any())
          throw new InvalidOperationException("The network has no layers");

        return Layers.First().Inputs;
      }
    }

    public IReadOnlyList<float> Outputs
    {
      get
      {
        if (!Layers.Any())
          throw new InvalidOperationException("The network has no layers");

        return Layers.Last().Outputs;
      }
    }

    public void AddLayer(ILayer layer)
    {
      if (layer == null) throw new ArgumentNullException("layer");
      if (Layers.Any() && NumOutputs != layer.NumInputs)
        throw new InvalidOperationException(string.Format(
          "Expected layer to take {0} inputs, but it takes {1}",
          NumOutputs, layer.NumInputs));

      m_layers.Add(layer);
    }

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
        layer.Update(inputs);
        inputs = layer.Outputs;
      }
    }
  }
}
