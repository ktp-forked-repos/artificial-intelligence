using Game.Core.Tests.Stubs;
using NUnit.Framework;

namespace Game.Core.Tests.EntityTests
{
  [TestFixture]
  public class EntityLifeCycleBaseTests
  {
    [Test]
    public void Initialize_Fails()
    {
      var entity = new EntityLifeCycleStub(false);
      var eventFired = false;
      entity.Initialized += (sender, args) => eventFired = true;

      var result = entity.Initialize();

      Assert.IsFalse(result);
      Assert.IsFalse(eventFired);
      Assert.IsFalse(entity.IsInitialized);
      Assert.AreEqual(1, entity.DoInitializeCallCount);
    }

    [Test]
    public void Initialize_Succeeds()
    {
      var entity = new EntityLifeCycleStub();
      var eventFired = false;
      entity.Initialized += (sender, args) => eventFired = true;

      var result = entity.Initialize();

      Assert.IsTrue(result);
      Assert.IsTrue(eventFired);
      Assert.IsTrue(entity.IsInitialized);
      Assert.IsTrue(entity.IsDeactivated);
      Assert.AreEqual(1, entity.DoInitializeCallCount);
    }

    [Test]
    public void Activate_ChangesState()
    {
      var entity = new EntityLifeCycleStub();
      var eventFired = false;
      entity.Activated += (sender, args) => eventFired = true;
      entity.Initialize();

      entity.Activate();

      Assert.IsTrue(eventFired);
      Assert.IsTrue(entity.IsActive);
      Assert.AreEqual(1, entity.DoActivateCallCount);
    }

    [Test]
    public void Activate_IgnoresDuplicateActivation()
    {
      var entity = new EntityLifeCycleStub();
      entity.Initialize();
      entity.Activate();
      var eventFired = false;
      entity.Activated += (sender, args) => eventFired = true;

      entity.Activate();

      Assert.IsFalse(eventFired);
      Assert.IsTrue(entity.IsActive);
      Assert.AreEqual(1, entity.DoActivateCallCount);
    }

    [Test]
    public void Deactivate_ChangesState()
    {
      var entity = new EntityLifeCycleStub();
      entity.Initialize();
      entity.Activate();
      var eventFired = false;
      entity.DeActivated += (sender, args) => eventFired = true;

      entity.Deactivate();

      Assert.IsTrue(eventFired);
      Assert.IsTrue(entity.IsDeactivated);
      Assert.AreEqual(1, entity.DoDeactivateCallCount);
    }

    [Test]
    public void Deactivate_IgnoresDuplicateDeactivation()
    {
      var entity = new EntityLifeCycleStub();
      entity.Initialize();
      entity.Activate();
      entity.Deactivate();
      var eventFired = false;
      entity.DeActivated += (sender, args) => eventFired = true;

      entity.Deactivate();

      Assert.IsFalse(eventFired);
      Assert.IsTrue(entity.IsDeactivated);
      Assert.AreEqual(1, entity.DoDeactivateCallCount);
    }

    [Test]
    public void Destroy_FromUnInitialized()
    {
      var entity = new EntityLifeCycleStub();
      var eventFired = false;
      entity.Destroyed += (sender, args) => eventFired = true;

      entity.Destroy();

      Assert.IsTrue(eventFired);
      Assert.IsTrue(entity.IsDestroyed);
      Assert.AreEqual(1, entity.DoDestroyCallCount);
    }

    [Test]
    public void Destroy_FromInitialized()
    {
      var entity = new EntityLifeCycleStub();
      entity.Initialize();
      var eventFired = false;
      entity.Destroyed += (sender, args) => eventFired = true;

      entity.Destroy();

      Assert.IsTrue(eventFired);
      Assert.IsTrue(entity.IsDestroyed);
      Assert.AreEqual(1, entity.DoDestroyCallCount);
    }

    [Test]
    public void Destroy_FromActivated()
    {
      var entity = new EntityLifeCycleStub();
      entity.Initialize();
      entity.Activate();
      var eventFired = false;
      entity.Destroyed += (sender, args) => eventFired = true;

      entity.Destroy();

      Assert.IsTrue(eventFired);
      Assert.IsTrue(entity.IsDestroyed);
      Assert.AreEqual(1, entity.DoDestroyCallCount);
    }

    [Test]
    public void Destroy_FromDeActivated()
    {
      var entity = new EntityLifeCycleStub();
      entity.Initialize();
      entity.Activate();
      entity.Deactivate();
      var eventFired = false;
      entity.Destroyed += (sender, args) => eventFired = true;

      entity.Destroy();

      Assert.IsTrue(eventFired);
      Assert.IsTrue(entity.IsDestroyed);
      Assert.AreEqual(1, entity.DoDestroyCallCount);
    }
  }
}
