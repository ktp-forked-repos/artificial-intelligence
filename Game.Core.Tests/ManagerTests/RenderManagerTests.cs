using System;
using System.Diagnostics;
using Game.Core.Events;
using Game.Core.Events.Entity;
using Game.Core.Interfaces;
using Game.Core.Managers;
using Game.Core.SFML;
using Game.Core.Tests.Stubs;
using Moq;
using NUnit.Framework;
using SFML.Graphics;

namespace Game.Core.Tests.ManagerTests
{
  [TestFixture]
  public class RenderManagerTests
  {
    private Mock<IEntityManager> entityManMock;
    private Mock<IEventManager> eventManMock;
    private Mock<IRenderWindow> windowMock;
    private RenderManager renderManager;

    private Entity BuildEntity(int id = 1)
    {
      var entity = new Entity(id);
      var rc1 = new RenderComponentStub(entity);
      var rc2 = new RenderComponentStub2(entity);
      entity.AddComponent(rc1);
      entity.AddComponent(rc2);

      var result = entity.Initialize();
      Debug.Assert(result);
      return entity;
    }

    [SetUp]
    public void SetUp()
    {
      entityManMock = new Mock<IEntityManager>();
      eventManMock = new Mock<IEventManager>();
      windowMock = new Mock<IRenderWindow>();
      renderManager = new RenderManager(
        entityManMock.Object, eventManMock.Object, windowMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
      entityManMock = null;
      eventManMock = null;
      windowMock = null;
      renderManager = null;
    }
      
    [Test]
    public void Constructor_HandlesNull_EntityManager()
    {
      TestDelegate func = () => new RenderManager(null, null, null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void Constructor_HandlesNull_EventManager()
    {
      TestDelegate func = () => new RenderManager(entityManMock.Object, 
        null, null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void Constructor_HandlesNull_RenderTarget()
    {
      TestDelegate func = () => new RenderManager(
        entityManMock.Object, eventManMock.Object, null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void Initialize_Succeeds()
    {
      var result = renderManager.Initialize();

      Assert.IsTrue(result);
    }

    [Test]
    public void PostInitialize_Succeeds()
    {
      eventManMock.Setup(m =>
        m.AddListener<EntityAddedEvent>(It.IsNotNull<Action<EventBase>>()));
      renderManager.Initialize();

      var result = renderManager.PostInitialize();

      Assert.IsTrue(result);
      eventManMock.VerifyAll();
    }

    [Test]
    public void Update_DoesNothingWithInsufficientElapsedTime()
    {
      windowMock.Verify(m => m.Display(), Times.Never);
      renderManager.Initialize();
      renderManager.PostInitialize();
      var renderableMock = new Mock<IRenderable>();
      renderableMock.Verify(m => m.Draw(It.IsAny<RenderTarget>()), Times.Never);
      renderManager.AddRenderable(renderableMock.Object);

      renderManager.Update(0f, 0f);

      windowMock.VerifyAll();
      renderableMock.VerifyAll();
    }

    [Test]
    public void Update_DoesNothingWhenPaused()
    {
      windowMock.Verify(m => m.Display(), Times.Never);
      renderManager.Initialize();
      renderManager.PostInitialize();
      renderManager.Paused = true;
      var renderableMock = new Mock<IRenderable>();
      renderableMock.Verify(m => m.Draw(It.IsAny<RenderTarget>()), Times.Never);
      renderManager.AddRenderable(renderableMock.Object);

      renderManager.Update(1f, 1f);

      windowMock.VerifyAll();
      renderableMock.VerifyAll();
    }

    [Test]
    public void Update_DrawsAndFiresEvent()
    {
      windowMock.Setup(m => m.Display());
      renderManager.Initialize();
      renderManager.PostInitialize();
      var renderableMock = new Mock<IRenderable>();
      renderableMock.Setup(m => m.Draw(It.IsNotNull<RenderTarget>()));
      renderManager.AddRenderable(renderableMock.Object);
      
      renderManager.Update(1f, 1f);

      windowMock.VerifyAll();
      renderableMock.VerifyAll();
    }

    [Test]
    public void Shutdown_RemovesListener()
    {
      eventManMock.Setup(m =>
        m.RemoveListener<EntityAddedEvent>(It.IsNotNull<Action<EventBase>>()));
      renderManager.Initialize();
      renderManager.PostInitialize();

      renderManager.Shutdown();

      eventManMock.VerifyAll();
    }

    [Test]
    public void TargetFrameRate_RejectsBadValues()
    {
      TestDelegate func = () => renderManager.TargetFrameRate = 0;

      Assert.Throws<ArgumentOutOfRangeException>(func);
    }

    [Test]
    public void TargetFrameRate_SetsUpdateInterval()
    {
      renderManager.Initialize();
      renderManager.PostInitialize();
      var oldUpdateInterval = renderManager.UpdateInterval;
      var expected = RenderManager.DefaultFrameRate * 2;

      renderManager.TargetFrameRate = expected;

      Assert.AreEqual(expected, renderManager.TargetFrameRate);
      Assert.AreNotEqual(oldUpdateInterval, renderManager.UpdateInterval);
    }

    [Test]
    public void DrawOneFrame_HandlesNull()
    {
      renderManager.Initialize();
      renderManager.PostInitialize();

      TestDelegate func = () => renderManager.DrawOneFrame(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void DrawOneFrame_DrawsAllRenderables()
    {
      renderManager.Initialize();
      renderManager.PostInitialize();
      var targetMock = new Mock<RenderTarget>();
      var renderableMock1 = new Mock<IRenderable>();
      renderableMock1.Setup(m => m.Draw(It.IsNotNull<RenderTarget>()));
      renderManager.AddRenderable(renderableMock1.Object);
      var renderableMock2 = new Mock<IRenderable>();
      renderableMock2.Setup(m => m.Draw(It.IsNotNull<RenderTarget>()));
      renderManager.AddRenderable(renderableMock2.Object);

      renderManager.DrawOneFrame(targetMock.Object);

      renderableMock1.VerifyAll();
      renderableMock2.VerifyAll();
    }

    [Test]
    public void AddRenderable_HandlesNull()
    {
      renderManager.Initialize();
      renderManager.PostInitialize();

      TestDelegate func = () => renderManager.AddRenderable(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void AddRenderable_HandlesDuplicateId()
    {
      renderManager.Initialize();
      renderManager.PostInitialize();
      var id = 5;
      var renderableMock1 = new Mock<IRenderable>();
      renderableMock1.SetupGet(m => m.RenderId).Returns(id);
      renderManager.AddRenderable(renderableMock1.Object);
      var renderableMock2 = new Mock<IRenderable>();
      renderableMock2.SetupGet(m => m.RenderId).Returns(id);

      TestDelegate func = () => renderManager.AddRenderable(renderableMock2.Object);

      Assert.Throws<InvalidOperationException>(func);
      renderableMock1.VerifyAll();
      renderableMock2.VerifyAll();
    }

    [Test]
    public void AddRenderable_AssignsIdAndAdds()
    {
      renderManager.Initialize();
      renderManager.PostInitialize();
      var renderableMock = new Mock<IRenderable>();
      renderableMock.SetupProperty(m => m.RenderId);

      renderManager.AddRenderable(renderableMock.Object);

      Assert.AreNotEqual(0, renderableMock.Object.RenderId);
      CollectionAssert.Contains(renderManager.Renderables, renderableMock.Object);
    }

    [Test]
    public void RemoveRenderable_HandlesNull()
    {
      renderManager.Initialize();
      renderManager.PostInitialize();

      TestDelegate func = () => renderManager.RemoveRenderable(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void RemoveRenderable_Removes()
    {
      renderManager.Initialize();
      renderManager.PostInitialize();
      var renderableMock = new Mock<IRenderable>();
      renderableMock.SetupProperty(m => m.RenderId);
      renderManager.AddRenderable(renderableMock.Object);

      renderManager.RemoveRenderable(renderableMock.Object);

      Assert.IsEmpty(renderManager.Renderables);
    }

    [Test]
    public void EntityListener_HandlesEntityDoesNotExist()
    {
      Action<EventBase> listener = null;
      eventManMock.Setup(m =>
        m.AddListener<EntityAddedEvent>(It.IsNotNull<Action<EventBase>>()))
        .Callback<Action<EventBase>>(action => listener = action);
      Entity entity = null;
      entityManMock.Setup(m => m.TryGetEntity(1, out entity))
        .Returns(false);
      renderManager.Initialize();
      renderManager.PostInitialize();
      var @event = new EntityAddedEvent { EntityId = 1 };

      Debug.Assert(listener != null);
      listener(@event);

      Assert.IsEmpty(renderManager.Renderables);
      eventManMock.VerifyAll();
      entityManMock.VerifyAll();
    }

    [Test]
    public void EntityListener_DoesNotAddFromDeactivatedEntity()
    {
      Action<EventBase> listener = null;
      eventManMock.Setup(m =>
        m.AddListener<EntityAddedEvent>(It.IsNotNull<Action<EventBase>>()))
        .Callback<Action<EventBase>>(action => listener = action);
      var entity = BuildEntity();
      entityManMock.Setup(m => m.TryGetEntity(entity.Id, out entity))
        .Returns(true);
      renderManager.Initialize();
      renderManager.PostInitialize();
      var @event = new EntityAddedEvent { EntityId = entity.Id };

      Debug.Assert(listener != null);
      listener(@event);

      Assert.IsEmpty(renderManager.Renderables);
      eventManMock.VerifyAll();
      entityManMock.VerifyAll();
    }

    [Test]
    public void EntityListener_AddsRenderablesFromActiveEntity()
    {
      Action<EventBase> listener = null;
      eventManMock.Setup(m => 
        m.AddListener<EntityAddedEvent>(It.IsNotNull<Action<EventBase>>()))
        .Callback<Action<EventBase>>(action => listener = action);
      var entity = BuildEntity();
      entity.Activate();
      entityManMock.Setup(m => m.TryGetEntity(entity.Id, out entity))
        .Returns(true);
      renderManager.Initialize();
      renderManager.PostInitialize();
      var @event = new EntityAddedEvent {EntityId = entity.Id};

      Debug.Assert(listener != null);
      listener(@event);

      Assert.AreEqual(2, renderManager.Renderables.Count);
      eventManMock.VerifyAll();
      entityManMock.VerifyAll();
    }

    [Test]
    public void EntityListener_AddsRenderablesWhenEntityActivated()
    {
      Action<EventBase> listener = null;
      eventManMock.Setup(m => 
        m.AddListener<EntityAddedEvent>(It.IsNotNull<Action<EventBase>>()))
        .Callback<Action<EventBase>>(action => listener = action);
      var entity = BuildEntity();
      entityManMock.Setup(m => m.TryGetEntity(entity.Id, out entity))
        .Returns(true);
      renderManager.Initialize();
      renderManager.PostInitialize();
      var @event = new EntityAddedEvent { EntityId = entity.Id };

      Debug.Assert(listener != null);
      listener(@event);
      entity.Activate();

      Assert.AreEqual(2, renderManager.Renderables.Count);
      eventManMock.VerifyAll();
      entityManMock.VerifyAll();
    }

    [Test]
    public void EntityListener_RemovesRenderablesWhenEntityDeactivated()
    {
      Action<EventBase> listener = null;
      eventManMock.Setup(m =>
        m.AddListener<EntityAddedEvent>(It.IsNotNull<Action<EventBase>>()))
        .Callback<Action<EventBase>>(action => listener = action);
      var entity = BuildEntity();
      entity.Activate();
      entityManMock.Setup(m => m.TryGetEntity(entity.Id, out entity))
        .Returns(true);
      renderManager.Initialize();
      renderManager.PostInitialize();
      var @event = new EntityAddedEvent { EntityId = entity.Id };

      Debug.Assert(listener != null);
      listener(@event);
      entity.Deactivate();

      Assert.IsEmpty(renderManager.Renderables);
      eventManMock.VerifyAll();
      entityManMock.VerifyAll();
    }

    [Test]
    public void EntityListener_RemovesRenderablesWhenEntityDestroyed()
    {
      Action<EventBase> listener = null;
      eventManMock.Setup(m =>
        m.AddListener<EntityAddedEvent>(It.IsNotNull<Action<EventBase>>()))
        .Callback<Action<EventBase>>(action => listener = action);
      var entity = BuildEntity();
      entity.Activate();
      entityManMock.Setup(m => m.TryGetEntity(entity.Id, out entity))
        .Returns(true);
      renderManager.Initialize();
      renderManager.PostInitialize();
      var @event = new EntityAddedEvent { EntityId = entity.Id };

      Debug.Assert(listener != null);
      listener(@event);
      entity.Destroy();

      Assert.IsEmpty(renderManager.Renderables);
      eventManMock.VerifyAll();
      entityManMock.VerifyAll();
    }
  }
}
