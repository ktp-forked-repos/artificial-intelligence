using NUnit.Framework;

namespace Game.Core.Tests
{
  [SetUpFixture]
  public class GlobalSetup
  {
    [SetUp]
    public void SetUp()
    {
      log4net.Config.XmlConfigurator.Configure();
    }
  }
}
