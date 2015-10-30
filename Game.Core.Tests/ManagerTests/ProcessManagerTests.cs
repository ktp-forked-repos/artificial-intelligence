using System;
using System.Linq;
using Game.Core.Managers;
using Game.Core.Tests.Stubs;
using NUnit.Framework;

namespace Game.Core.Tests.ManagerTests
{
  [TestFixture]
  public class ProcessManagerTests
  {
    private class ProcessStub2
      : ProcessStub
    {
      public ProcessStub2(int id, bool initializationResult = true)
        : base(id, initializationResult)
      {
      }
    }

    [Test]
    public void Initialize_Succeeds()
    {
      var pm = new ProcessManager();

      var result = pm.Initialize();

      Assert.IsTrue(result);
    }

    [Test]
    public void PostInitialize_Succeeds()
    {
      var pm = new ProcessManager();
      pm.Initialize();

      var result = pm.PostInitialize();

      Assert.IsTrue(result);
    }

    [Test]
    public void Update_DoesNothingWhenPaused()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process = new ProcessStub(1);
      process.Initialize();
      process.Resume();
      pm.AddProcess(process);
      pm.Paused = true;
      const float time = 1.0f;

      pm.Update(time);

      Assert.AreEqual(0, process.DoUpdateCallCount);
    }

    [Test]
    public void Update_DoesNotUpdatePausedProcess()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process = new ProcessStub(1);
      process.Initialize();
      pm.AddProcess(process);
      const float time = 1.0f;

      pm.Update(time);

      Assert.AreEqual(0, process.DoUpdateCallCount);
    }

    [Test]
    public void Update_UpdatesRunningProcess()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process = new ProcessStub(1);
      process.Initialize();
      pm.AddProcess(process);
      process.Resume();
      const float time = 1.0f;

      pm.Update(time);

      Assert.AreEqual(1, process.DoUpdateCallCount);
      Assert.AreEqual(time, process.LastUpdateDeltaTime);
    }

    [Test]
    public void Update_RemovesSucceededProcess()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process = new ProcessStub(1);
      process.Initialize();
      pm.AddProcess(process);
      const float time = 1.0f;

      process.Succeed();
      pm.Update(time);

      Assert.IsEmpty(pm.Processes);
    }

    [Test]
    public void Update_RemovesFailedProcess()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process = new ProcessStub(1);
      process.Initialize();
      pm.AddProcess(process);
      const float time = 1.0f;

      process.Fail();
      pm.Update(time);

      Assert.IsEmpty(pm.Processes);
    }

    [Test]
    public void Update_RemovesAbortedProcess()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process = new ProcessStub(1);
      process.Initialize();
      pm.AddProcess(process);
      const float time = 1.0f;

      process.Abort();
      pm.Update(time);

      Assert.IsEmpty(pm.Processes);
    }

    [Test]
    public void Update_DoesNotActivateChildOnFailure()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);
      var process2 = new ProcessStub(2);
      process1.AttachChild(process2);
      const float time = 1.0f;

      process1.Fail();
      pm.Update(time);

      Assert.IsEmpty(pm.Processes);
      Assert.IsTrue(process2.WasAborted);
    }

    [Test]
    public void Update_DoesNotActivateChildOnAbort()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);
      var process2 = new ProcessStub(2);
      process1.AttachChild(process2);
      const float time = 1.0f;

      process1.Abort();
      pm.Update(time);

      Assert.IsEmpty(pm.Processes);
      Assert.IsTrue(process2.WasAborted);
    }

    [Test]
    public void Update_ActivatesChildOnSuccess()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);
      var process2 = new ProcessStub(2);
      process1.AttachChild(process2);
      const float time = 1.0f;

      process1.Succeed();
      pm.Update(time);

      Assert.AreEqual(1, pm.Processes.Count);
      Assert.IsNotNull(pm.Processes.Single(p => ReferenceEquals(p, process2)));
      Assert.IsTrue(process2.IsRunning);
    }

    [Test]
    public void Update_ActivatesChildOnFailWithFlag()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);
      var process2 = new ProcessStub(2);
      process1.ActivateChildOnFailure = true;
      process1.AttachChild(process2);
      const float time = 1.0f;

      process1.Fail();
      pm.Update(time);

      Assert.AreEqual(1, pm.Processes.Count);
      Assert.IsNotNull(pm.Processes.Single(p => ReferenceEquals(p, process2)));
      Assert.IsTrue(process2.IsRunning);
    }

    [Test]
    public void Update_ActivatesChildOnAbortWithFlag()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);
      var process2 = new ProcessStub(2);
      process1.ActivateChildOnAbort = true;
      process1.AttachChild(process2);
      const float time = 1.0f;

      process1.Abort();
      pm.Update(time);

      Assert.AreEqual(1, pm.Processes.Count);
      Assert.IsNotNull(pm.Processes.Single(p => ReferenceEquals(p, process2)));
      Assert.IsTrue(process2.IsRunning);
    }

    [Test]
    public void Update_AbortsChildThatFailsInitialization()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);
      var process2 = new ProcessStub(2, false);
      process1.AttachChild(process2);
      const float time = 1.0f;

      process1.Succeed();
      pm.Update(time);

      Assert.IsEmpty(pm.Processes);
      Assert.IsTrue(process2.WasAborted);
    }

    [Test]
    public void Update_ActivatesUnInitializedChild()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);
      var process2 = new ProcessStub(2);
      process1.AttachChild(process2);
      const float time = 1.0f;

      process1.Succeed();
      pm.Update(time);

      Assert.AreEqual(1, pm.Processes.Count);
      Assert.IsNotNull(pm.Processes.Single(p => ReferenceEquals(p, process2)));
      Assert.IsTrue(process2.IsRunning);
    }

    [Test]
    public void Shutdown_AbortsAllProcesses()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);
      var process2 = new ProcessStub(2);
      process1.AttachChild(process2);
      var process3 = new ProcessStub(3);
      process3.Initialize();
      pm.AddProcess(process3);

      pm.Shutdown();

      Assert.IsEmpty(pm.Processes);
      Assert.IsTrue(process1.WasAborted);
      Assert.IsTrue(process2.WasAborted);
      Assert.IsTrue(process3.WasAborted);
    }

    [Test]
    public void GetProcess_DoesNotFindNonExistingProcess()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process = new ProcessStub(1);
      process.Initialize();
      pm.AddProcess(process);

      var result = pm.GetProcess(2);

      Assert.IsNull(result);
    }

    [Test]
    public void GetProcess_FindsProcess()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process = new ProcessStub(1);
      process.Initialize();
      pm.AddProcess(process);

      var result = pm.GetProcess(1);

      Assert.IsNotNull(result);
      Assert.AreSame(process, result);
    }
    
    [Test]
    public void GetProcesses_DoesNotFindNonExisting()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);

      var result = pm.GetProcesses<ProcessStub2>().ToList();

      Assert.IsEmpty(result);
    }

    [Test]
    public void GetProcesses_FindsProcessesByBase()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);
      var process2 = new ProcessStub2(2);
      process2.Initialize();
      pm.AddProcess(process2);

      var result = pm.GetProcesses<ProcessStub>().ToList();

      Assert.AreEqual(2, result.Count);
      Assert.IsNotNull(result.Single(p => ReferenceEquals(p, process1)));
      Assert.IsNotNull(result.Single(p => ReferenceEquals(p, process2)));
    }

    [Test]
    public void GetProcesses_FindsProcessesByMostDerived()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      pm.AddProcess(process1);
      var process2 = new ProcessStub2(2);
      process2.Initialize();
      pm.AddProcess(process2);

      var result = pm.GetProcesses<ProcessStub2>().ToList();

      Assert.AreEqual(1, result.Count);
      Assert.IsNotNull(result.Single(p => ReferenceEquals(p, process2)));
    }

    [Test]
    public void AddProcess_HandlesNull()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();

      TestDelegate func = () => pm.AddProcess(null);

      Assert.Throws<ArgumentNullException>(func);
      Assert.IsFalse(pm.Processes.Any());
    }

    [Test]
    public void AddProcess_RejectsUnInitializedProcess()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process = new ProcessStub(1);

      TestDelegate func = () => pm.AddProcess(process);

      Assert.Throws<InvalidOperationException>(func);
      Assert.IsFalse(pm.Processes.Any());
    }

    [Test]
    public void AddProcess_RejectsDuplicateProcessId()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process1 = new ProcessStub(1);
      process1.Initialize();
      var process2 = new ProcessStub(1);
      process2.Initialize();
      pm.AddProcess(process1);

      TestDelegate func = () => pm.AddProcess(process2);

      Assert.Throws<InvalidOperationException>(func);
      Assert.AreEqual(1, pm.Processes.Count);
    }

    [Test]
    public void AddProcess_AcceptsInitializedProcess()
    {
      var pm = new ProcessManager();
      pm.Initialize();
      pm.PostInitialize();
      var process = new ProcessStub(1);
      process.Initialize();

      pm.AddProcess(process);
    }
  }
}
