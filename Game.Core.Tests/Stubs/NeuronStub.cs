using Game.Core.NeuralNet;
using Game.Core.NeuralNet.Interfaces;

namespace Game.Core.Tests.Stubs
{
  internal class NeuronStub
    : NeuronBase
  {
    public NeuronStub(ILayer layer, int index) 
      : base(layer, index)
    {
    }

    protected override float Activation(float biasedNetInput)
    {
      return biasedNetInput;
    }
  }
}
