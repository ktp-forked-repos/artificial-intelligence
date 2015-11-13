using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Core.Extensions;

namespace Game.Core.NeuralNetwork
{
  public class Layer
  {
    private readonly List<float> m_inputs;
    private readonly List<float> m_outputs; 

    public Layer(int index, int numInputs, int numOutputs, 
      ActivationFunction activationFunction = ActivationFunction.HardLimit)
    {
      if (index < 0) throw new ArgumentOutOfRangeException("index");
      if (numInputs < 0) throw new ArgumentOutOfRangeException("numInputs");
      if (numOutputs < 0) throw new ArgumentOutOfRangeException("numOutputs");
      
      Index = index;
      NumInputs = numInputs;
      NumOutputs = numOutputs;
      Neurons = new List<Neuron>(numOutputs);
      m_inputs = new List<float>(Enumerable.Repeat(0f, NumInputs));
      m_outputs = new List<float>(Enumerable.Repeat(0f, numOutputs));

      for (var i = 0; i < numOutputs; i++)
      {
        Neurons.Add(new Neuron(i, numInputs)
        {
          ActivationFunction = activationFunction
        });
      }
    }

    public int Index { get; private set; }

    public int NumInputs { get; private set; }

    public int NumOutputs { get; private set; }

    public IList<Neuron> Neurons { get; private set; }

    public IReadOnlyList<float> Inputs { get { return m_inputs; } }

    public IReadOnlyList<float> Outputs { get { return m_outputs; } }

    public void Update(IReadOnlyList<float> inputs)
    {
      Debug.Assert(inputs.Count == NumInputs);

      inputs.CopyAllTo(m_inputs);
      for (var i = 0; i < Neurons.Count; i++)
      {
        Neurons[i].Update(Inputs);
        m_outputs[i] = Neurons[i].Output;
      }
    }
  }
}
