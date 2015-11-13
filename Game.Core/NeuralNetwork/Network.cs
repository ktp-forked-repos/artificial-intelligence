using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Game.Core.NeuralNetwork
{
  public class Network
  {
    public Network()
    {
      Layers = new List<Layer>();
    }

    public int NumInputs
    {
      get
      {
        Debug.Assert(Layers != null);
        Debug.Assert(Layers.Any());
        return Layers.First().NumInputs;
      }
    }

    public int NumOutputs
    {
      get
      {
        Debug.Assert(Layers != null);
        Debug.Assert(Layers.Any());
        return Layers.Last().NumOutputs;
      }
    }

    public IList<Layer> Layers { get; set; }

    public IReadOnlyList<float> Inputs
    {
      get
      {
        Debug.Assert(Layers != null);
        Debug.Assert(Layers.Any());
        return Layers.First().Inputs;
      }
    }

    public IReadOnlyList<float> Outputs
    {
      get
      {
        Debug.Assert(Layers != null);
        Debug.Assert(Layers.Any());
        return Layers.Last().Outputs;
      }
    }

    public void Update(IReadOnlyList<float> inputs)
    {
      Debug.Assert(inputs.Count == NumInputs);

      foreach (var layer in Layers)
      {
        layer.Update(inputs);
        inputs = layer.Outputs;
      }
    }
  }
}
