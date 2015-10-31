using System;
using Game.Core.Interfaces;
using Moq;
using NUnit.Framework;

namespace Game.Core.Tests.MiscTests
{
  [TestFixture]
  public class RenderCompareTests
  {
    [Test]
    public void CompareTo_HandlesANull()
    {
      TestDelegate func = () => RenderableCompare.CompareTo(null, null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void CompareTo_HandlesBNull()
    {
      var aMock = new Mock<IRenderable>();

      TestDelegate func = () => RenderableCompare.CompareTo(aMock.Object, null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void CompareTo_HandlesAFirstInOrder()
    {
      var aMock = new Mock<IRenderable>();
      aMock.SetupGet(m => m.RenderDepth).Returns(0);
      var bMock = new Mock<IRenderable>();
      bMock.SetupGet(m => m.RenderDepth).Returns(1);

      var result = RenderableCompare.CompareTo(aMock.Object, bMock.Object);

      Assert.AreEqual(1, result);
    }

    [Test]
    public void CompareTo_HandlesEqualOrder()
    {
      var aMock = new Mock<IRenderable>();
      aMock.SetupGet(m => m.RenderDepth).Returns(1);
      var bMock = new Mock<IRenderable>();
      bMock.SetupGet(m => m.RenderDepth).Returns(1);

      var result = RenderableCompare.CompareTo(aMock.Object, bMock.Object);

      Assert.AreEqual(0, result);
    }

    [Test]
    public void CompareTo_HandlesBFirstInOrder()
    {
      var aMock = new Mock<IRenderable>();
      aMock.SetupGet(m => m.RenderDepth).Returns(1);
      var bMock = new Mock<IRenderable>();
      bMock.SetupGet(m => m.RenderDepth).Returns(0);

      var result = RenderableCompare.CompareTo(aMock.Object, bMock.Object);

      Assert.AreEqual(1, result);
    }
  }
}
