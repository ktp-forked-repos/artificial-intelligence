using NUnit.Framework;

namespace Common.Tests
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
