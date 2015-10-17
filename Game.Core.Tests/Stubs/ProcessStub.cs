using Game.Core.Processes;

namespace Game.Core.Tests.Stubs
{
  internal class ProcessStub
    : ProcessBase
  {
    public ProcessStub(int id, bool initializationResult = true) 
      : base(id, "ProcessStub")
    {
      InitializationResult = initializationResult;
    }

    public bool InitializationResult { get; private set; }
    public float LastUpdateDeltaTime { get; private set; }

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
      LastUpdateDeltaTime = deltaTime;
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