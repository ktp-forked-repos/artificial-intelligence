using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Core.NeuralNet;
using Game.Core.NeuralNet.Interfaces;
using Game.Core.Tests.Stubs;
using Moq;
using NUnit.Framework;

namespace Game.Core.Tests.NeuralNetTests
{
  [TestFixture]
  public class LayerTests
  {
    private Mock<INetwork> networkMock;
    private Layer layer;

    private const int NumInputs = 2;

    private Mock<INeuron> MakeNeuronMock()
    {
      var result = new Mock<INeuron>();
      result.SetupGet(m => m.Layer).Returns(layer);
      result.SetupGet(m => m.NumInputs).Returns(NumInputs);
      return result;
    }

    [SetUp]
    public void SetUp()
    {
      networkMock = new Mock<INetwork>();
      layer = new Layer(networkMock.Object, 0, NumInputs);
    }

    [TearDown]
    public void TearDown()
    {
      networkMock = null;
      layer = null;
    }

    [Test]
    public void Constructor_HandlesNullNetwork()
    {
      TestDelegate func = () => new Layer(null, 0, NumInputs);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void Constructor_HandlesBadIndex()
    {
      TestDelegate func = () => new Layer(networkMock.Object, -1, NumInputs);

      Assert.Throws<ArgumentOutOfRangeException>(func);
    }

    [Test]
    public void Constructor_HandlesBadNumInputs()
    {
      TestDelegate func = () => new Layer(networkMock.Object, 0, 0);

      Assert.Throws<ArgumentOutOfRangeException>(func);
    }

    [Test]
    public void Constructor_InitializesInputs()
    {
      // action is done in the setup

      Assert.IsNotNull(layer.Inputs);
      Assert.AreEqual(NumInputs, layer.Inputs.Count);
    }

    [Test]
    public void AddNeuron_HandlesNull()
    {
      TestDelegate func = () => layer.AddNeuron(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void AddNeuron_RejectsNeuronInOtherLayer()
    {
      var layerMock = new Mock<ILayer>();
      var neuronMock = new Mock<INeuron>();
      neuronMock.SetupGet(m => m.Layer).Returns(layerMock.Object);

      TestDelegate func = () => layer.AddNeuron(neuronMock.Object);

      Assert.Throws<InvalidOperationException>(func);
      layerMock.VerifyAll();
      neuronMock.VerifyAll();
    }

    [Test]
    public void AddNeuron_RejectsInputsMismatch()
    {
      var neuronMock = new Mock<INeuron>();
      neuronMock.SetupGet(m => m.Layer).Returns(layer);
      neuronMock.SetupGet(m => m.NumInputs).Returns(NumInputs + 1);

      TestDelegate func = () => layer.AddNeuron(neuronMock.Object);

      Assert.Throws<InvalidOperationException>(func);
      neuronMock.VerifyAll();
    }

    [Test]
    public void AddNeuron_AddsNeuron()
    {
      var neuronMock = MakeNeuronMock();

      layer.AddNeuron(neuronMock.Object);

      Assert.AreEqual(1, layer.Neurons.Count);
      Assert.AreEqual(1, layer.NumOutputs);
      Assert.AreSame(neuronMock.Object, layer.Neurons.Last());
      neuronMock.VerifyAll();
    }

    [Test]
    public void Update_HandlesNull()
    {
      var neuronMock = MakeNeuronMock();
      layer.AddNeuron(neuronMock.Object);

      TestDelegate func = () => layer.Update(null);

      Assert.Throws<ArgumentNullException>(func);
      neuronMock.VerifyAll();
    }

    [Test]
    public void Update_HandlesBadInput()
    {
      var neuronMock = MakeNeuronMock();
      layer.AddNeuron(neuronMock.Object);
      var inputs = Enumerable.Repeat(0f, NumInputs + 1).ToList();

      TestDelegate func = () => layer.Update(inputs);

      Assert.Throws<InvalidOperationException>(func);
      neuronMock.VerifyAll();
    }

    [Test]
    public void Update_HandlesNoNeurons()
    {
      var inputs = Enumerable.Repeat(0f, NumInputs).ToList();

      TestDelegate func = () => layer.Update(inputs);

      Assert.Throws<InvalidOperationException>(func);
    }

    [Test]
    public void Update_UpdatesNeurons()
    {
      var expected = 10f;
      var neuronUpdateCalled = false;
      var neuronMock = MakeNeuronMock();
      neuronMock.Setup(m => m.Update(It.IsNotNull<IReadOnlyList<float>>()))
        .Callback(() => neuronUpdateCalled = true);
      neuronMock.SetupGet(m => m.Output).Returns(expected);
      layer.AddNeuron(neuronMock.Object);
      var inputs = Enumerable.Repeat(0f, NumInputs).ToList();

      layer.Update(inputs);

      Assert.IsTrue(neuronUpdateCalled);
      Assert.AreEqual(expected, layer.Outputs.First());
      neuronMock.VerifyAll();
    }
  }
}
