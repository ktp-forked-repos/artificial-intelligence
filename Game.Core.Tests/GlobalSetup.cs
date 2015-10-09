using NUnit.Framework;

namespace Commont.Tests
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
