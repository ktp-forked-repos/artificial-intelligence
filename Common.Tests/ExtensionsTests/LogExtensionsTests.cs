using System;
using Common.Extensions;
using log4net;
using log4net.Core;
using Moq;
using NUnit.Framework;

namespace Common.Tests.ExtensionsTests
{
  [TestFixture]
  public class LogExtensionsTests
  {
    private Mock<ILog> m_logMock;
    private Mock<ILogger> m_loggerMock;

    [SetUp]
    public void SetUp()
    {
      m_logMock = new Mock<ILog>();
      m_loggerMock = new Mock<ILogger>();

      m_logMock.SetupGet(m => m.Logger)
        .Returns(() => m_loggerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
      m_logMock = null;
      m_loggerMock = null;
    }
    
    [Test]
    public void Verbose_Message()
    {
      m_loggerMock.Setup(
        m => m.Log(It.IsNotNull<Type>(), It.Is<Level>(l => l == Level.Verbose),
          It.IsNotNull<string>(), It.Is<Exception>(e => e == null)));

      m_logMock.Object.Verbose("test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void Verbose_Exception()
    {
      m_loggerMock.Setup(
        m => m.Log(It.IsNotNull<Type>(), It.Is<Level>(l => l == Level.Verbose),
          It.Is<string>(s => string.IsNullOrEmpty(s)), It.IsNotNull<Exception>()));

      m_logMock.Object.Verbose(new Exception());

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void VerboseFmt_Disabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.VerboseFmt("test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void VerboseFmt_Enabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(), 
          It.Is<Level>(l => l == Level.Verbose), It.IsNotNull<string>(), 
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.VerboseFmt("{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void VerboseFmtIf_False()
    {
      // intentional setup override
      m_logMock = new Mock<ILog>();
      m_loggerMock = new Mock<ILogger>();

      m_logMock.Object.VerboseFmtIf(false, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void VerboseFmtIf_TrueAndDisabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.VerboseFmtIf(true, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void VerboseFmtIf_TrueAndEnabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Verbose), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.VerboseFmtIf(true, "{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void DebugFmt_Disabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.DebugFmt("test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void DebugFmt_Enabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Debug), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.DebugFmt("{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void DebugFmtIf_False()
    {
      // intentional setup override
      m_logMock = new Mock<ILog>();
      m_loggerMock = new Mock<ILogger>();

      m_logMock.Object.DebugFmtIf(false, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void DebugFmtIf_TrueAndDisabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.DebugFmtIf(true, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void DebugFmtIf_TrueAndEnabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Debug), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.DebugFmtIf(true, "{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void InfoFmt_Disabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.InfoFmt("test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void InfoFmt_Enabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Info), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.InfoFmt("{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void InfoFmtIf_False()
    {
      // intentional setup override
      m_logMock = new Mock<ILog>();
      m_loggerMock = new Mock<ILogger>();

      m_logMock.Object.InfoFmtIf(false, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void InfoFmtIf_TrueAndDisabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.InfoFmtIf(true, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void InfoFmtIf_TrueAndEnabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Info), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.InfoFmtIf(true, "{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void WarnFmt_Disabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.WarnFmt("test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void WarnFmt_Enabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Warn), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.WarnFmt("{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void WarnFmtIf_False()
    {
      // intentional setup override
      m_logMock = new Mock<ILog>();
      m_loggerMock = new Mock<ILogger>();

      m_logMock.Object.WarnFmtIf(false, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void WarnFmtIf_TrueAndDisabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.WarnFmtIf(true, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void WarnFmtIf_TrueAndEnabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Warn), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.WarnFmtIf(true, "{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void ErrorFmt_Disabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.ErrorFmt("test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void ErrorFmt_Enabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Error), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.ErrorFmt("{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void ErrorFmtIf_False()
    {
      // intentional setup override
      m_logMock = new Mock<ILog>();
      m_loggerMock = new Mock<ILogger>();

      m_logMock.Object.ErrorFmtIf(false, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void ErrorFmtIf_TrueAndDisabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.ErrorFmtIf(true, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void ErrorFmtIf_TrueAndEnabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Error), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.ErrorFmtIf(true, "{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void FatalFmt_Disabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.FatalFmt("test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void FatalFmt_Enabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Fatal), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.FatalFmt("{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void FatalFmtIf_False()
    {
      // intentional setup override
      m_logMock = new Mock<ILog>();
      m_loggerMock = new Mock<ILogger>();

      m_logMock.Object.FatalFmtIf(false, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void FatalFmtIf_TrueAndDisabled()
    {
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(false);
      m_loggerMock.Verify(m => m.Log(It.IsAny<Type>(), It.IsAny<Level>(),
        It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

      m_logMock.Object.FatalFmtIf(true, "test");

      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }

    [Test]
    public void FatalFmtIf_TrueAndEnabled()
    {
      var result = string.Empty;
      var expected = string.Format("{0}{1}", 1, 2.34);
      m_loggerMock.Setup(m => m.IsEnabledFor(It.IsAny<Level>()))
        .Returns(true);
      m_loggerMock.Setup(m => m.Log(It.IsNotNull<Type>(),
          It.Is<Level>(l => l == Level.Fatal), It.IsNotNull<string>(),
          It.Is<Exception>(e => e == null)))
        .Callback<Type, Level, object, Exception>((t, l, o, e) => result = o.ToString());

      m_logMock.Object.FatalFmtIf(true, "{0}{1}", 1, 2.34);

      Assert.AreEqual(expected, result);
      m_loggerMock.VerifyAll();
      m_logMock.VerifyAll();
    }
  }
}
