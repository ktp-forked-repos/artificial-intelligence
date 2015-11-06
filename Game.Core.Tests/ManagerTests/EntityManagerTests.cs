using System;
using System.Diagnostics;
using Game.Core.Entities;
using Game.Core.Factories;
using Game.Core.Managers;
using Game.Core.Managers.Interfaces;
using Game.Core.Tests.Stubs;
using Moq;
using NUnit.Framework;
using SFML.Graphics;

namespace Game.Core.Tests.ManagerTests
{
  [TestFixture]
  public class EntityManagerTests
  {
    private Mock<IEventManager> eventManMock;
    private EntityManager entityManager;

    private Entity BuildEntity(int id = 1)
    {
      var entity = new Entity(id);
      var c = new UpdateComponentStub(entity);
      entity.AddComponent(c);

      var result = entity.Initialize();
      Debug.Assert(result);
      return entity;
    }

    [SetUp]
    public void SetUp()
    {
      eventManMock = new Mock<IEventManager>();
      entityManager = new EntityManager(eventManMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
      eventManMock = null;
      entityManager = null;
    }

    [Test]
    public void Constructor_HandlesNull()
    {
      TestDelegate func = () => new EntityManager(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void Initialize_Succeeds()
    {
      var result = entityManager.Initialize();

      Assert.IsTrue(result);
    }

    [Test]
    public void PostInitialize_Succeeds()
    {
      entityManager.Initialize();

      var result = entityManager.PostInitialize();

      Assert.IsTrue(result);
    }

    [Test]
    public void Update_DoesNothingWhenPaused()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      entityManager.Paused = true;
      var entity = BuildEntity();
      entity.Activate();
      entityManager.AddEntity(entity);

      entityManager.Update(1f);

      Assert.AreEqual(0, entity.GetComponent<UpdateComponentStub>().UpdateCallCount);
    }

    [Test]
    public void Update_IgnoresInactiveEntities()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = BuildEntity();
      entityManager.AddEntity(entity);

      entityManager.Update(1f);

      Assert.AreEqual(0, entity.GetComponent<UpdateComponentStub>().UpdateCallCount);
    }

    [Test]
    public void Update_IgnoresEntitiesAfterDeactivate()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = BuildEntity();
      entity.Activate();
      entityManager.AddEntity(entity);

      entityManager.Update(1f);
      entity.Deactivate();
      entityManager.Update(1f);

      Assert.AreEqual(1, entity.GetComponent<UpdateComponentStub>().UpdateCallCount);
    }

    [Test]
    public void Update_UpdatesEntities()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = BuildEntity();
      entity.Activate();
      entityManager.AddEntity(entity);

      entityManager.Update(1f);

      Assert.AreEqual(1, entity.GetComponent<UpdateComponentStub>().UpdateCallCount);
    }

    [Test]
    public void Shutdown_DestroysEntities()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = BuildEntity();
      entityManager.AddEntity(entity);

      entityManager.Shutdown();

      Assert.AreEqual(1, entity.GetComponent<UpdateComponentStub>().DoDestroyCallCount);
    }

    [Test]
    public void GetEntity_ReturnsNullOnBadId()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();

      var result = entityManager.GetEntity(1);

      Assert.IsNull(result);
    }

    [Test]
    public void GetEntity_RetrievesEntity()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = BuildEntity(2);
      entityManager.AddEntity(entity);

      var result = entityManager.GetEntity(2);

      Assert.IsNotNull(result);
    }

    [Test]
    public void TryGetEntity_HandlesBadId()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();

      Entity entity;
      var result = entityManager.TryGetEntity(1, out entity);

      Assert.IsFalse(result);
      Assert.IsNull(entity);
    }

    [Test]
    public void TryGetEntity_RetrievesEntity()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = BuildEntity(2);
      entityManager.AddEntity(entity);

      Entity entityResult;
      var result = entityManager.TryGetEntity(2, out entityResult);

      Assert.IsTrue(result);
      Assert.IsNotNull(entityResult);
      Assert.AreSame(entity, entityResult);
    }

    [Test]
    public void CreateEntity_HandlesNull()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();

      TestDelegate func = () => entityManager.CreateEntity(null);

      Assert.Throws<ArgumentNullException>(func);
      Assert.IsEmpty(entityManager.Entities);
    }

    [Test]
    public void CreateEntity_HandlesEntityFactoryNull()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();

      TestDelegate func = () => entityManager.CreateEntity("test");

      Assert.Throws<InvalidOperationException>(func);
      Assert.IsEmpty(entityManager.Entities);
    }

    [Test]
    public void CreateEntity_HandlesCreationFailed()
    {
      var factoryMock = new Mock<IEntityFactory>();
      factoryMock.Setup(m => m.Create(It.IsNotNull<string>()))
        .Returns((Entity) null);
      entityManager.EntityFactory = factoryMock.Object;
      entityManager.Initialize();
      entityManager.PostInitialize();

      var result = entityManager.CreateEntity("test");

      Assert.IsNull(result);
      Assert.IsEmpty(entityManager.Entities);
    }

    [Test]
    public void CreateEntity_HandlesInitializationFailure()
    {
      var entity = new Entity(1);
      entity.AddComponent(new ComponentStub(entity, false));
      var factoryMock = new Mock<IEntityFactory>();
      factoryMock.Setup(m => m.Create(It.IsNotNull<string>()))
        .Returns(entity);
      entityManager.EntityFactory = factoryMock.Object;
      entityManager.Initialize();
      entityManager.PostInitialize();

      var result = entityManager.CreateEntity("test");

      Assert.IsNull(result);
      Assert.IsEmpty(entityManager.Entities);
    }

    [Test]
    public void CreateEntity_AddsEntity()
    {
      var entity = new Entity(1);
      entity.AddComponent(new ComponentStub(entity));
      var factoryMock = new Mock<IEntityFactory>();
      factoryMock.Setup(m => m.Create(It.IsNotNull<string>()))
        .Returns(entity);
      entityManager.EntityFactory = factoryMock.Object;
      entityManager.Initialize();
      entityManager.PostInitialize();

      var result = entityManager.CreateEntity("test");

      Assert.IsNotNull(result);
      Assert.IsTrue(result.IsInitialized);
      Assert.AreEqual(1, entityManager.Entities.Count);
    }

    [Test]
    public void AddEntity_HandlesNull()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();

      TestDelegate func = () => entityManager.AddEntity(null);

      Assert.Throws<ArgumentNullException>(func);
      Assert.IsEmpty(entityManager.Entities);
    }

    [Test]
    public void AddEntity_HandlesEntityNotInitialized()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = new Entity(1);

      TestDelegate func = () => entityManager.AddEntity(entity);

      Assert.Throws<InvalidOperationException>(func);
      Assert.IsEmpty(entityManager.Entities);
    }

    [Test]
    public void AddEntity_HandlesDuplicateEntity()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var e1 = BuildEntity(1);
      entityManager.AddEntity(e1);
      var e2 = BuildEntity(1);

      TestDelegate func = () => entityManager.AddEntity(e2);

      Assert.Throws<InvalidOperationException>(func);
      Assert.AreEqual(1, entityManager.Entities.Count);
    }

    [Test]
    public void AddEntity_AddsEntity()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = BuildEntity(1);
      
      entityManager.AddEntity(entity);

      Assert.AreEqual(1, entityManager.Entities.Count);
      CollectionAssert.Contains(entityManager.Entities, entity);
    }

    [Test]
    public void ActivateEntity_Activates()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = BuildEntity(1);
      entityManager.AddEntity(entity);

      entityManager.ActivateEntity(1);

      Assert.AreEqual(1, entity.GetComponent<UpdateComponentStub>().DoActivateCallCount);
      Assert.IsTrue(entity.IsActive);
    }

    [Test]
    public void DeActivateEntity_Deactivates()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = BuildEntity(1);
      entity.Activate();
      entityManager.AddEntity(entity);

      entityManager.DeActivateEntity(1);

      Assert.AreEqual(1, entity.GetComponent<UpdateComponentStub>().DoDeactivateCallCount);
      Assert.IsTrue(entity.IsDeactivated);
    }

    [Test]
    public void DestroyEntity_Destroys()
    {
      entityManager.Initialize();
      entityManager.PostInitialize();
      var entity = BuildEntity(1);
      entityManager.AddEntity(entity);

      entityManager.DestroyEntity(1);

      Assert.AreEqual(1, entity.GetComponent<UpdateComponentStub>().DoDestroyCallCount);
      Assert.IsTrue(entity.IsDestroyed);
    }
  }
}
