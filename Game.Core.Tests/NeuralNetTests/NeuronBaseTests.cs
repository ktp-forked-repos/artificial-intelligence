using System;
using System.Linq;
using Game.Core.NeuralNet;
using Game.Core.NeuralNet.Interfaces;
using Game.Core.Tests.Stubs;
using Moq;
using NUnit.Framework;

namespace Game.Core.Tests.NeuralNetTests
{
  [TestFixture]
  public class NeuronBaseTests
  {
    private Mock<INetwork> networkMock;
    private Mock<ILayer> layerMock;
    private NeuronBase neuron;

    private const int NumInputs = 2;

    [SetUp]
    public void SetUp()
    {
      networkMock = new Mock<INetwork>();
      layerMock = new Mock<ILayer>();
      layerMock.SetupGet(m => m.NumInputs).Returns(NumInputs);
      neuron = new NeuronStub(layerMock.Object, 0);
    }

    [TearDown]
    public void TearDown()
    {
      networkMock = null;
      layerMock = null;
      neuron = null;
    }

    [Test]
    public void Constructor_HandlesNullLayer()
    {
      TestDelegate func = () => new NeuronStub(null, 0);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void Constructor_HandlesBadIndex()
    {
      TestDelegate func = () => new NeuronStub(layerMock.Object, -1);

      Assert.Throws<ArgumentOutOfRangeException>(func);
    }

    [Test]
    public void Constructor_InitializesWeights()
    {
      // acting is already done in setup...

      Assert.IsNotNull(neuron.Weights);
      Assert.AreEqual(NumInputs, neuron.Weights.Count);
      layerMock.VerifyAll();
    }

    [Test]
    public void Inputs_ReflectsLayer()
    {
      var expected = Enumerable.Repeat(1f, NumInputs).ToList();
      layerMock.SetupGet(m => m.Inputs).Returns(expected);

      var result = neuron.Inputs;

      CollectionAssert.AreEqual(expected, result);
    }

    [Test]
    public void Update_HandlesNull()
    {
      TestDelegate func = () => neuron.Update(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void Update_HandlesBadInputs()
    {
      var inputs = Enumerable.Repeat(0f, NumInputs + 1).ToList();

      TestDelegate func = () => neuron.Update(inputs);

      Assert.Throws<InvalidOperationException>(func);
      layerMock.VerifyAll();
    }

    [Test]
    public void Update_HandlesModifiedWeights()
    {
      var inputs = Enumerable.Repeat(0f, NumInputs).ToList();

      neuron.Weights.Clear();
      TestDelegate func = () => neuron.Update(inputs);

      Assert.Throws<InvalidOperationException>(func);
    }

    [TestCase(new[] {1f, 2f})]
    [TestCase(new[] { -1f, 2f })]
    [TestCase(new[] { 123f, 634f })]
    [TestCase(new[] { -1234f, 2134f })]
    [TestCase(new[] { 0f, 0f })]
    public void Update_Calculates(float[] inputs)
    {
      // weights of 1 means the output should be the sum of the inputs
      for (var i = 0; i < neuron.Weights.Count; i++)
      {
        neuron.Weights[i] = 1f;
      }
      var expected = inputs.Sum();

      neuron.Update(inputs.ToList());

      Assert.AreEqual(expected, neuron.Output, 1e-8);
    }
  }
}
