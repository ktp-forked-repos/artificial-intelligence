using Game.Core.Processes;

namespace Game.Core.Tests.Stubs
{
  internal sealed class StubProcess
    : ProcessBase
  {
    public StubProcess(int id = 1, bool initializationResult = true) 
      : base(id, "StubProcess")
    {
      InitializationResult = initializationResult;
    }

    public bool InitializationResult { get; private set; }

    public int DoInitializeCallCount { get; private set; }
    public int DoUpdateCallCount { get; private set; }
    public int DoPauseCallCount { get; private set; }
    public int DoResumeCallCount { get; private set; }
    public int DoSucceedCallCount { get; private set; }
    public int DoFailCallCount { get; private set; }
    public int DoAbortCallCount { get; private set; }

    protected override bool DoInitialize()
    {
      DoInitializeCallCount++;
      return InitializationResult;
    }

    protected override void DoUpdate(float deltaTime)
    {
      DoUpdateCallCount++;
    }

    protected override void DoPause()
    {
      DoPauseCallCount++;
    }

    protected override void DoResume()
    {
      DoResumeCallCount++;
    }

    protected override void DoSucceed()
    {
      DoSucceedCallCount++;
    }

    protected override void DoFail()
    {
      DoFailCallCount++;
    }

    protected override void DoAbort()
    {
      DoAbortCallCount++;
    }
  }
}