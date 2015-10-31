using System;
using Game.Core.Tests.Stubs;
using NUnit.Framework;

namespace Game.Core.Tests.EntityTests.ComponentTests
{
  [TestFixture]
  public class ComponentBaseTests
  {
    [Test]
    public void Constructor_HandlesNull()
    {
      TestDelegate func = () => new ComponentStub(null);

      Assert.Throws<ArgumentNullException>(func);
    }
  }
}
