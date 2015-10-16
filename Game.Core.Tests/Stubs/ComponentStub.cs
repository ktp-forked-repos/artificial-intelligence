using Game.Core.Components;
using Microsoft.Xna.Framework;

namespace Game.Core.Tests.Stubs
{
  internal class ComponentStub
    : ComponentBase
  {
    public ComponentStub(Entity parent, bool initializationResult = true) 
      : base(parent)
    {
      InitializationResult = initializationResult;
    }

    public bool InitializationResult { get; private set; }

    public float LastUpdateDeltaTime { get; private set; }
    public int UpdateCallCount { get; private set; }
    public int DoInitializeCallCount { get; private set; }
    public int DoActivateCallCount { get; private set; }
    public int DoDeactivateCallCount { get; private set; }
    public int DoDestroyCallCount { get; private set; }
    public int DisposeCallCount { get; private set; }

    public override void Update(float deltaTime)
    {
      LastUpdateDeltaTime = deltaTime;
      UpdateCallCount++;
    }

    protected override bool DoInitialize()
    {
      DoInitializeCallCount++;
      return InitializationResult;
    }

    protected override void DoActivate()
    {
      DoActivateCallCount++;
    }

    protected override void DoDeactivate()
    {
      DoDeactivateCallCount++;
    }

    protected override void DoDestroy()
    {
      DoDestroyCallCount++;
    }

    protected override void Dispose(bool disposing)
    {
      DisposeCallCount++;
    }
  }

  internal class UpdateComponentStub
    : ComponentStub
  {
    public UpdateComponentStub(Entity parent, bool initializationResult = true) 
      : base(parent, initializationResult)
    {
      NeedsUpdate = true;
    }
  }

  internal class TransformComponentStub
    : TransformComponentBase
  {
    public TransformComponentStub(Entity parent, bool initializationResult = true) 
      : base(parent)
    {
      InitializationResult = initializationResult;
    }

    public bool InitializationResult { get; private set; }

    public float LastUpdateDeltaTime { get; private set; }
    public int UpdateCallCount { get; private set; }
    public int DoInitializeCallCount { get; private set; }
    public int DoActivateCallCount { get; private set; }
    public int DoDeactivateCallCount { get; private set; }
    public int DoDestroyCallCount { get; private set; }
    public int DisposeCallCount { get; private set; }

    public override void Update(float deltaTime)
    {
      LastUpdateDeltaTime = deltaTime;
      UpdateCallCount++;
    }

    protected override bool DoInitialize()
    {
      DoInitializeCallCount++;
      return InitializationResult;
    }

    protected override void DoActivate()
    {
      DoActivateCallCount++;
    }

    protected override void DoDeactivate()
    {
      DoDeactivateCallCount++;
    }

    protected override void DoDestroy()
    {
      DoDestroyCallCount++;
    }

    protected override void Dispose(bool disposing)
    {
      DisposeCallCount++;
    }

    public override Vector2 Position { get; set; }

    public override float Rotation { get; set; }
  }
}
