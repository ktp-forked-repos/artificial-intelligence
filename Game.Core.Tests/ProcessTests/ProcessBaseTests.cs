using System;
using Game.Core.Tests.Stubs;
using NUnit.Framework;
// ReSharper disable RedundantArgumentDefaultValue

namespace Game.Core.Tests.ProcessTests
{
  [TestFixture]
  public class ProcessBaseTests
  {
    [Test]
    public void Initialize_Fails()
    {
      var proc = new ProcessStub(1, false);
      var eventFired = false;
      proc.Initialized += (sender, args) => eventFired = true;

      var result = proc.Initialize();

      Assert.IsFalse(result);
      Assert.IsFalse(eventFired);
      Assert.AreEqual(1, proc.DoInitializeCallCount);
      Assert.IsFalse(proc.IsInitialized);
    }

    [Test]
    public void Initialize_Success_StartsRunning()
    {
      var proc = new ProcessStub {BeginPaused = false};
      var eventFired = false;
      proc.Initialized += (sender, args) => eventFired = true;

      var result = proc.Initialize();

      Assert.IsTrue(result);
      Assert.IsTrue(eventFired);
      Assert.AreEqual(1, proc.DoInitializeCallCount);
      Assert.IsTrue(proc.IsInitialized);
      Assert.IsTrue(proc.IsRunning);
    }

    [Test]
    public void Initialize_Success_StartsPaused()
    {
      var proc = new ProcessStub { BeginPaused = true };
      var initializeEvent = false;
      proc.Initialized += (sender, args) => initializeEvent = true;
      var pausedEvent = false;
      proc.Paused += (sender, args) => pausedEvent = true;

      var result = proc.Initialize();

      Assert.IsTrue(result);
      Assert.IsTrue(initializeEvent);
      Assert.AreEqual(1, proc.DoInitializeCallCount);
      Assert.IsTrue(pausedEvent);
      Assert.AreEqual(1, proc.DoPauseCallCount);
      Assert.IsTrue(proc.IsInitialized);
      Assert.IsTrue(proc.IsPaused);
    }

    [Test]
    public void Update_CallsHandlers()
    {
      var proc = new ProcessStub();
      proc.Initialize();

      proc.Update(1f);

      Assert.AreEqual(1, proc.DoUpdateCallCount);
      Assert.AreEqual(1f, proc.RunningTime);
    }

    [Test]
    public void Update_AccumulatesRunTime()
    {
      var proc = new ProcessStub();
      proc.Initialize();

      proc.Update(1f);
      proc.Update(1f);

      Assert.AreEqual(2, proc.DoUpdateCallCount);
      Assert.AreEqual(2f, proc.RunningTime);
    }

    [Test]
    public void Pause_DoesNothingIfPaused()
    {
      var proc = new ProcessStub {BeginPaused = true};
      var pauseEvent = false;
      proc.Initialize();
      proc.Paused += (sender, args) => pauseEvent = true;

      proc.Pause();

      Assert.AreEqual(1, proc.DoPauseCallCount); // 1 startup pause
      Assert.IsFalse(pauseEvent);
      Assert.IsTrue(proc.IsPaused);
    }

    [Test]
    public void Pause_Pauses()
    {
      var proc = new ProcessStub();
      var pauseEvent = false;
      proc.Paused += (sender, args) => pauseEvent = true;
      proc.Initialize();

      proc.Pause();

      Assert.AreEqual(1, proc.DoPauseCallCount);
      Assert.IsTrue(pauseEvent);
      Assert.IsTrue(proc.IsPaused);
    }

    [Test]
    public void Resume_DoesNothingIfRunning()
    {
      var proc = new ProcessStub();
      var resumeEvent = false;
      proc.Resumed += (sender, args) => resumeEvent = true;
      proc.Initialize();

      proc.Resume();

      Assert.AreEqual(0, proc.DoResumeCallCount);
      Assert.IsFalse(resumeEvent);
      Assert.IsTrue(proc.IsRunning);
    }

    [Test]
    public void Resume_Resumes()
    {
      var proc = new ProcessStub { BeginPaused = true };
      var resumeEvent = false;
      proc.Resumed += (sender, args) => resumeEvent = true;
      proc.Initialize();

      proc.Resume();

      Assert.AreEqual(1, proc.DoResumeCallCount);
      Assert.IsTrue(resumeEvent);
      Assert.IsTrue(proc.IsRunning);
    }

    [Test]
    public void Succeed_ChangesState()
    {
      var proc = new ProcessStub();
      var succeedEvent = false;
      proc.Succeeded += (sender, args) => succeedEvent = true;
      proc.Initialize();

      proc.Succeed();

      Assert.AreEqual(1, proc.DoSucceedCallCount);
      Assert.IsTrue(succeedEvent);
      Assert.IsTrue(proc.HasSucceeded);
      Assert.IsTrue(proc.IsCompleted);
    }

    [Test]
    public void Fail_ChangesState()
    {
      var proc = new ProcessStub();
      var failEvent = false;
      proc.Failed += (sender, args) => failEvent = true;
      proc.Initialize();

      proc.Fail();

      Assert.AreEqual(1, proc.DoFailCallCount);
      Assert.IsTrue(failEvent);
      Assert.IsTrue(proc.HasFailed);
      Assert.IsTrue(proc.IsCompleted);
    }

    [Test]
    public void Abort_HandlesUninitializedState()
    {
      var proc = new ProcessStub();
      var abortEvent = false;
      proc.Aborted += (sender, args) => abortEvent = true;

      proc.Abort();

      Assert.AreEqual(1, proc.DoAbortCallCount);
      Assert.IsTrue(abortEvent);
      Assert.IsTrue(proc.WasAborted);
      Assert.IsTrue(proc.IsCompleted);
    }

    [Test]
    public void Abort_HandlesInitializedState()
    {
      var proc = new ProcessStub();
      var abortEvent = false;
      proc.Aborted += (sender, args) => abortEvent = true;
      proc.Initialize();

      proc.Abort();

      Assert.AreEqual(1, proc.DoAbortCallCount);
      Assert.IsTrue(abortEvent);
      Assert.IsTrue(proc.WasAborted);
      Assert.IsTrue(proc.IsCompleted);
    }

    [Test]
    public void AttachChild_HandlesNull()
    {
      var proc = new ProcessStub();

      TestDelegate func = () => proc.AttachChild(null);

      Assert.Throws<ArgumentNullException>(func);
    }

    [Test]
    public void AttachChild_RejectsInitializedChild()
    {
      var proc = new ProcessStub(1);
      var child = new ProcessStub(2);
      child.Initialize();
      TestDelegate func = () => proc.AttachChild(child);

      Assert.Throws<InvalidOperationException>(func);
      Assert.IsNull(proc.Child);
    }

    [Test]
    public void AttachChild_AddsFirstChild()
    {
      var proc = new ProcessStub(1);
      var child = new ProcessStub(2);

      proc.AttachChild(child);

      Assert.IsNotNull(proc.Child);
      Assert.AreEqual(2, proc.Child.Id);
    }

    [Test]
    public void AttachChild_ChainsChildren()
    {
      var proc = new ProcessStub(1);
      var child1 = new ProcessStub(2);
      var child2 = new ProcessStub(3);

      proc.AttachChild(child1);
      proc.AttachChild(child2);

      Assert.IsNotNull(proc.Child);
      Assert.AreEqual(2, proc.Child.Id);
      Assert.IsNotNull(proc.Child.Child);
      Assert.AreEqual(3, proc.Child.Child.Id);
    }

    [Test]
    public void RemoveChild_HandlesNoChild()
    {
      var proc = new ProcessStub(1);

      var result = proc.RemoveChild();

      Assert.IsNull(result);
    }

    [Test]
    public void RemoveChild_DetachesChild()
    {
      var proc = new ProcessStub(1);
      var child = new ProcessStub(2);
      proc.AttachChild(child);

      var result = proc.RemoveChild();

      Assert.IsNotNull(result);
      Assert.AreEqual(2, child.Id);
    }

    [Test]
    public void AbortAll_HandlesNoChild()
    {
      var proc = new ProcessStub();
      var abortEvent = false;
      proc.Aborted += (sender, args) => abortEvent = true;

      proc.AbortAll();

      Assert.AreEqual(1, proc.DoAbortCallCount);
      Assert.IsTrue(abortEvent);
      Assert.IsTrue(proc.WasAborted);
      Assert.IsTrue(proc.IsCompleted);
      Assert.IsNull(proc.Child);
    }

    [Test]
    public void AbortAll_AbortsChild()
    {
      var proc = new ProcessStub(1);
      var child = new ProcessStub(2);
      proc.AttachChild(child);
      var abortEvent = false;
      proc.Aborted += (sender, args) => abortEvent = true;

      proc.AbortAll();

      Assert.AreEqual(1, proc.DoAbortCallCount);
      Assert.IsTrue(abortEvent);
      Assert.IsTrue(proc.WasAborted);
      Assert.IsTrue(proc.IsCompleted);
      Assert.IsNull(proc.Child);
      Assert.IsTrue(child.WasAborted);
    }

    [Test]
    public void AbortAll_AbortsChain()
    {
      var proc = new ProcessStub(1);
      var child1 = new ProcessStub(2);
      var child2 = new ProcessStub(3);
      proc.AttachChild(child1);
      proc.AttachChild(child2);
      var abortEvent = false;
      proc.Aborted += (sender, args) => abortEvent = true;

      proc.AbortAll();

      Assert.AreEqual(1, proc.DoAbortCallCount);
      Assert.IsTrue(abortEvent);
      Assert.IsTrue(proc.WasAborted);
      Assert.IsTrue(proc.IsCompleted);
      Assert.IsNull(proc.Child);
      Assert.IsTrue(child1.WasAborted);
      Assert.IsTrue(child2.WasAborted);
    }
    
    [Test]
    public void IsPaused_Setter_Pauses()
    {
      var proc = new ProcessStub();
      var pauseEvent = false;
      proc.Paused += (sender, args) => pauseEvent = true;
      proc.Initialize();

      proc.IsPaused = true;

      Assert.AreEqual(1, proc.DoPauseCallCount);
      Assert.IsTrue(pauseEvent);
      Assert.IsTrue(proc.IsPaused);
    }

    [Test]
    public void IsPaused_Setter_Resumes()
    {
      var proc = new ProcessStub { BeginPaused = true };
      var resumeEvent = false;
      proc.Resumed += (sender, args) => resumeEvent = true;
      proc.Initialize();

      proc.IsPaused = false;

      Assert.AreEqual(1, proc.DoResumeCallCount);
      Assert.IsTrue(resumeEvent);
      Assert.IsTrue(proc.IsRunning);
    }
  }
}
