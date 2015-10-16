using System;
using System.Linq;
using Game.Core.Components;
using Game.Core.Tests.Stubs;
using NUnit.Framework;

namespace Game.Core.Tests.EntityTests
{
  [TestFixture]
  public class EntityTests
  {
    private class TransformComponentStub2
      : TransformComponentStub
    {
      public TransformComponentStub2(Entity parent, bool initializationResult = true) 
        : base(parent, initializationResult)
      {
      }
    }

    private class UpdateComponentStub2
      : UpdateComponentStub
    {
      public UpdateComponentStub2(Entity parent, bool initializationResult = true) 
        : base(parent, initializationResult)
      {
      }
    }

    [Test]
    public void AddComponent_HandlesNull()
    {
      var entity = new Entity(1);

      TestDelegate func = () => entity.AddComponent(null);

      Assert.Throws<ArgumentNullException>(func);
      Assert.AreEqual(0, entity.Components.Count());
    }

    [Test]
    public void AddComponent_RejectsInitializedComponent()
    {
      var entity = new Entity(1);
      var component = new ComponentStub(entity);
      component.Initialize();

      TestDelegate func = () => entity.AddComponent(component);

      Assert.Throws<InvalidOperationException>(func);
      Assert.AreEqual(0, entity.Components.Count());
    }

    [Test]
    public void AddComponent_RejectsAfterInitialization()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      entity.AddComponent(component1);
      entity.Initialize();
      var component2 = new UpdateComponentStub(entity);

      TestDelegate func = () => entity.AddComponent(component2);

      Assert.Throws<InvalidOperationException>(func);
      Assert.AreEqual(1, entity.Components.Count());
      Assert.AreSame(component1, entity.GetComponent<ComponentStub>());
    }

    [Test]
    public void AddComponent_RejectsBadParent()
    {
      var entity1 = new Entity(1);
      var entity2 = new Entity(2);
      var component = new ComponentStub(entity2);

      TestDelegate func = () => entity1.AddComponent(component);

      Assert.Throws<InvalidOperationException>(func);
      Assert.AreEqual(0, entity1.Components.Count());
    }

    [Test]
    public void AddComponent_RejectsDuplicateComponent()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      var component2 = new ComponentStub(entity);
      entity.AddComponent(component1);

      TestDelegate func = () => entity.AddComponent(component2);

      Assert.Throws<InvalidOperationException>(func);
      Assert.AreEqual(1, entity.Components.Count());
      Assert.AreSame(component1, entity.GetComponent<ComponentStub>());
    }

    [Test]
    public void AddComponent_AddsComponent()
    {
      var entity = new Entity(1);
      var component = new ComponentStub(entity);

      entity.AddComponent(component);

      Assert.AreEqual(1, entity.Components.Count());
      Assert.AreSame(component, entity.GetComponent<ComponentStub>());
    }
    
    [Test]
    public void HasComponent_FindsComponent()
    {
      var entity = new Entity(1);
      var component = new ComponentStub(entity);
      entity.AddComponent(component);

      var result = entity.HasComponent<ComponentStub>();

      Assert.IsTrue(result);
    }

    [Test]
    public void HasComponent_FindsWithMultipleComponents()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      var component2 = new UpdateComponentStub(entity);
      entity.AddComponent(component1);
      entity.AddComponent(component2);

      var result1 = entity.HasComponent<ComponentStub>();
      var result2 = entity.HasComponent<UpdateComponentStub>();

      Assert.IsTrue(result1);
      Assert.IsTrue(result2);
    }

    [Test]
    public void TryGetComponent_FindsComponent()
    {
      var entity = new Entity(1);
      var component = new ComponentStub(entity);
      entity.AddComponent(component);
      ComponentStub outResult;

      var result = entity.TryGetComponent(out outResult);

      Assert.IsTrue(result);
      Assert.AreSame(component, outResult);
    }

    [Test]
    public void TryGetComponent_FindsWithMultipleComponents()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      var component2 = new UpdateComponentStub(entity);
      entity.AddComponent(component1);
      entity.AddComponent(component2);
      ComponentStub outResult1;
      UpdateComponentStub outResult2;

      var result1 = entity.TryGetComponent(out outResult1);
      var result2 = entity.TryGetComponent(out outResult2);

      Assert.IsTrue(result1);
      Assert.AreSame(component1, outResult1);
      Assert.IsTrue(result2);
      Assert.AreSame(component2, outResult2);
    }

    [Test]
    public void GetComponent_FindsComponent()
    {
      var entity = new Entity(1);
      var component = new ComponentStub(entity);
      entity.AddComponent(component);

      var result = entity.GetComponent<ComponentStub>();

      Assert.AreSame(component, result);
    }

    [Test]
    public void GetComponent_FindsWithMultipleComponents()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      var component2 = new UpdateComponentStub(entity);
      entity.AddComponent(component1);
      entity.AddComponent(component2);

      var result1 = entity.GetComponent<ComponentStub>();
      var result2 = entity.GetComponent<UpdateComponentStub>();

      Assert.AreSame(component1, result1);
      Assert.AreSame(component2, result2);
    }

    [Test]
    public void GetComponentsByBase_GetsComponents()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      var component2 = new UpdateComponentStub(entity);
      entity.AddComponent(component1);
      entity.AddComponent(component2);

      var result1 = entity.GetComponentsByBase<ComponentBase>();
      var result2 = entity.GetComponentsByBase<ComponentStub>();
      var result3 = entity.GetComponentsByBase<UpdateComponentStub>();

      Assert.AreEqual(2, result1.Count);
      Assert.AreEqual(2, result2.Count);
      Assert.AreEqual(1, result3.Count);
    }

    [Test]
    public void DoInitialize_RequiresComponents()
    {
      var entity = new Entity(1);
      
      TestDelegate func = () => entity.Initialize();

      Assert.Throws<InvalidOperationException>(func);
      Assert.IsFalse(entity.IsInitialized);
    }

    [Test]
    public void DoInitialize_RejectsMultipleTransforms()
    {
      var entity = new Entity(1);
      var transform1 = new TransformComponentStub(entity);
      var transform2 = new TransformComponentStub2(entity);
      entity.AddComponent(transform1);
      entity.AddComponent(transform2);

      TestDelegate func = () => entity.Initialize();

      Assert.Throws<InvalidOperationException>(func);
      Assert.IsFalse(entity.IsInitialized);
      Assert.IsFalse(transform1.IsInitialized);
      Assert.IsFalse(transform2.IsInitialized);
    }

    [Test]
    public void DoInitialize_SetsTransform()
    {
      var entity = new Entity(1);
      var transform = new TransformComponentStub(entity);
      entity.AddComponent(transform);

      var result = entity.Initialize();

      Assert.IsTrue(result);
      Assert.IsTrue(entity.IsInitialized);
      Assert.IsNotNull(entity.TransformComponent);
      Assert.AreSame(transform, entity.TransformComponent);
    }

    [Test]
    public void DoInitialize_FailsWhenComponentFails()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      entity.AddComponent(component1);
      var component2 = new UpdateComponentStub(entity, false);
      entity.AddComponent(component2);

      var result = entity.Initialize();

      Assert.IsFalse(result);
      Assert.IsFalse(entity.IsInitialized);
      Assert.IsTrue(entity.IsDestroyed);
      Assert.AreEqual(1, component1.DoInitializeCallCount);
      Assert.AreEqual(1, component1.DoDestroyCallCount);
      Assert.AreEqual(1, component2.DoInitializeCallCount);
      Assert.AreEqual(1, component2.DoDestroyCallCount);
    }

    [Test]
    public void DoInitialize_InitializesComponents()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      entity.AddComponent(component1);
      var component2 = new UpdateComponentStub(entity);
      entity.AddComponent(component2);

      var result = entity.Initialize();

      Assert.IsTrue(result);
      Assert.IsTrue(entity.IsInitialized);
      Assert.AreEqual(1, component1.DoInitializeCallCount);
      Assert.AreEqual(1, component2.DoInitializeCallCount);
    }
    
    [Test]
    public void DoInitialize_SetsUpdateFlag()
    {
      var entity = new Entity(1);
      var component = new UpdateComponentStub(entity);
      entity.AddComponent(component);

      var result = entity.Initialize();

      Assert.IsTrue(result);
      Assert.IsTrue(entity.NeedsUpdate);
      Assert.IsTrue(entity.IsInitialized);
      Assert.AreEqual(1, component.DoInitializeCallCount);
    }

    [Test]
    public void DoActivate_ActivatesComponents()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      entity.AddComponent(component1);
      var component2 = new UpdateComponentStub(entity);
      entity.AddComponent(component2);
      entity.Initialize();

      entity.Activate();

      Assert.IsTrue(entity.IsActive);
      Assert.IsTrue(component1.IsActive);
      Assert.AreEqual(1, component1.DoActivateCallCount);
      Assert.IsTrue(component2.IsActive);
      Assert.AreEqual(1, component2.DoActivateCallCount);
    }

    [Test]
    public void DoDeActivate_DeActivatesComponents()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      entity.AddComponent(component1);
      var component2 = new UpdateComponentStub(entity);
      entity.AddComponent(component2);
      entity.Initialize();
      entity.Activate();

      entity.Deactivate();

      Assert.IsTrue(entity.IsDeactivated);
      Assert.IsTrue(component1.IsDeactivated);
      Assert.AreEqual(1, component1.DoDeactivateCallCount);
      Assert.IsTrue(component2.IsDeactivated);
      Assert.AreEqual(1, component2.DoDeactivateCallCount);
    }

    [Test]
    public void DoDestroy_DestroysComponents()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      entity.AddComponent(component1);
      var component2 = new UpdateComponentStub(entity);
      entity.AddComponent(component2);
      entity.Initialize();

      entity.Destroy();

      Assert.IsTrue(entity.IsDestroyed);
      Assert.IsTrue(component1.IsDestroyed);
      Assert.AreEqual(1, component1.DoDestroyCallCount);
      Assert.IsTrue(component2.IsDestroyed);
      Assert.AreEqual(1, component2.DoDestroyCallCount);
    }
    
    [Test]
    public void Update_IgnoresNonUpdateComponents()
    {
      var entity = new Entity(1);
      var component1 = new ComponentStub(entity);
      entity.AddComponent(component1);
      var component2 = new UpdateComponentStub(entity);
      entity.AddComponent(component2);
      entity.Initialize();
      entity.Activate();
      var time = 1.0f;

      entity.Update(time);

      Assert.AreEqual(0, component1.UpdateCallCount);
      Assert.AreEqual(1, component2.UpdateCallCount);
      Assert.AreEqual(time, component2.LastUpdateDeltaTime);
    }

    [Test]
    public void Update_UpdatesAllUpdateComponents()
    {
      var entity = new Entity(1);
      var component1 = new UpdateComponentStub(entity);
      entity.AddComponent(component1);
      var component2 = new UpdateComponentStub2(entity);
      entity.AddComponent(component2);
      entity.Initialize();
      entity.Activate();
      var time = 1.0f;

      entity.Update(time);

      Assert.AreEqual(1, component1.UpdateCallCount);
      Assert.AreEqual(time, component2.LastUpdateDeltaTime);
      Assert.AreEqual(1, component2.UpdateCallCount);
      Assert.AreEqual(time, component2.LastUpdateDeltaTime);
    }
  }
}
