using Game.Core.Entities;

namespace Game.Core.Tests.Stubs
{
  internal class EntityLifeCycleStub
    : EntityLifeCycleBase
  {
    public EntityLifeCycleStub(bool initializationResult = true)
    {
      InitializationResult = initializationResult;
    }

    public bool InitializationResult { get; private set; }

    public int DoInitializeCallCount { get; private set; }
    public int DoActivateCallCount { get; private set; }
    public int DoDeactivateCallCount { get; private set; }
    public int DoDestroyCallCount { get; private set; }
    public int DisposeCallCount { get; private set; }

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
}
