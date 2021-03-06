﻿using System;
using System.Diagnostics;
using NLog;

namespace Game.Core.Processes
{
  /// <summary>
  ///   The states that a process can exist in.
  /// </summary>
  public enum ProcessState
  {
    /// <summary>
    ///   Initial state, inactive.
    /// </summary>
    NotInitialized,
    /// <summary>
    ///   Actively running.
    /// </summary>
    Running,
    /// <summary>
    ///   Active, but temporarily paused.
    /// </summary>
    Paused,
    /// <summary>
    ///   Completed successfully.
    /// </summary>
    Succeeded,
    /// <summary>
    ///   Completed unsuccessfully.
    /// </summary>
    Failed,
    /// <summary>
    ///   Manually stopped before completing.
    /// </summary>
    Aborted
  }

  /// <summary>
  ///   The base for all processes.  A process encapsulates any action that 
  ///   takes multiple frames.  Processes may be chained together to create 
  ///   sequences of actions.
  /// </summary>
  public abstract class ProcessBase
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

    /// <summary>
    ///   Create the process.
    /// </summary>
    /// <param name="id">
    ///   The unique id of this process.
    /// </param>
    /// <param name="name">
    ///   
    /// </param>
    protected ProcessBase(int id, string name = "")
    {
      Id = id;
      Name = string.Format("{0} {1}{2}", GetType().Name, Id,
        string.IsNullOrEmpty(name) ? string.Empty : ": " + name);
      State = ProcessState.NotInitialized;
      ActivateChildOnFailure = false;
      ActivateChildOnAbort = false;
    }

    #region Events
    
    /// <summary>
    ///   Event fires when the process transitions from Running -> Paused.
    /// </summary>
    public event EventHandler Paused;

    /// <summary>
    ///   Event fires when the process transitions from Paused -> Running.
    /// </summary>
    public event EventHandler Resumed;

    /// <summary>
    ///   Event fires when the process transitions to Succeeded.
    /// </summary>
    public event EventHandler Succeeded;

    /// <summary>
    ///   Event fires when the process transitions to Failed.
    /// </summary>
    public event EventHandler Failed;

    /// <summary>
    ///   Event fires when the process transitions to Aborted.
    /// </summary>
    public event EventHandler Aborted;

    #endregion
    #region Properties

    /// <summary>
    ///   The unique id for this process.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///   The process name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///   How long the process has been in the running state.
    /// </summary>
    public float RunningTime { get; private set; }

    /// <summary>
    ///   The current state of the process.
    /// </summary>
    public ProcessState State { get; private set; }
    
    /// <summary>
    ///   The child of this process, which will be activated if this process
    ///   completes successfully, and possibly if it fails or aborts, depending 
    ///   on <see cref="ActivateChildOnFailure"/> and 
    ///   <see cref="ActivateChildOnAbort"/>.
    /// </summary>
    public ProcessBase Child { get; private set; }

    /// <summary>
    ///   If true, the child process will activate if this process fails.
    /// 
    ///   Default: false.
    /// </summary>
    public bool ActivateChildOnFailure { get; set; }

    /// <summary>
    ///   If true, the child process will activate if this process is aborted.
    /// 
    ///   Default: false.
    /// </summary>
    public bool ActivateChildOnAbort { get; set; }

    #endregion
    #region State Properties

    /// <summary>
    ///   Was the process initialized successfully.
    /// </summary>
    public bool IsInitialized
    {
      get { return State != ProcessState.NotInitialized; }
    }

    /// <summary>
    ///   Is the process in a live state.
    /// </summary>
    public bool IsAlive
    {
      get
      {
        return State == ProcessState.Running || State == ProcessState.Paused;
      }
    }

    /// <summary>
    ///   Is the process alive and running.
    /// </summary>
    public bool IsRunning
    {
      get { return State == ProcessState.Running; }
    }

    /// <summary>
    ///   Is the process alive and paused.
    /// </summary>
    public bool IsPaused
    {
      get { return State == ProcessState.Paused; }
      set
      {
        if (value)
        {
          Pause();
        }
        else
        {
          Resume();
        }
      }
    }

    /// <summary>
    ///   Is the process in any completed state.
    /// </summary>
    public bool IsCompleted
    {
      get
      {
        return State == ProcessState.Succeeded ||
               State == ProcessState.Failed ||
               State == ProcessState.Aborted;
      }
    }

    /// <summary>
    ///   Did the process succeed.
    /// </summary>
    public bool HasSucceeded
    {
      get { return State == ProcessState.Succeeded; }
    }

    /// <summary>
    ///   Did the process fail.
    /// </summary>
    public bool HasFailed
    {
      get { return State == ProcessState.Failed; }
    }

    /// <summary>
    ///   Was the process aborted.
    /// </summary>
    public bool WasAborted
    {
      get { return State == ProcessState.Aborted; }
    }

    #endregion
    #region Child Methods

    /// <summary>
    ///   Attach a child to this process chain.  The process will be attached 
    ///   to the first child which does not have a child process.  The process 
    ///   must not be initialized.
    /// </summary>
    /// <param name="child"></param>
    /// <exception cref="ArgumentNullException">
    ///   child is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   child is initialized.
    /// </exception>
    public void AttachChild(ProcessBase child)
    {
      if (child == null) throw new ArgumentNullException("child");
      if (child.IsInitialized) 
        throw new InvalidOperationException("child can not be initialized");

      if (Child == null)
      {
        Child = child;
      }
      else
      {
        Child.AttachChild(child);
      }
    }

    /// <summary>
    ///   Removes and returns this process's child process.  Returns null if 
    ///   there is no child.
    /// </summary>
    /// <returns></returns>
    public ProcessBase RemoveChild()
    {
      var result = Child;
      Child = null;
      return result;
    }

    #endregion
    #region State Control Methods

    /// <summary>
    ///   Initializes a new process to the paused state.
    /// </summary>
    /// <returns>
    ///   Success or failure of initialization.
    /// </returns>
    public bool Initialize()
    {
      Debug.Assert(!IsInitialized);

      if (!DoInitialize())
      {
        Log.Error("{0} failed initialization", Name);
        return false;
      }

      Log.Trace("{0} initialized", Name);
      State = ProcessState.Paused;

      return true;
    }

    /// <summary>
    ///   Updates a running process.  Should only be called by the process 
    ///   manager.
    /// </summary>
    /// <param name="deltaTime">
    ///   Elapsed time since the last update.
    /// </param>
    public void Update(float deltaTime)
    {
      Debug.Assert(IsRunning);

      RunningTime += deltaTime;
      DoUpdate(deltaTime);
    }

    /// <summary>
    ///   Pauses a running process.
    /// </summary>
    public void Pause()
    {
      Debug.Assert(IsAlive);

      if (IsPaused)
      {
        return;
      }

      Debug.Assert(IsRunning);
      State = ProcessState.Paused;
      DoPause();
      OnPaused();
    }

    /// <summary>
    ///   Resumes a paused process.
    /// </summary>
    public void Resume()
    {
      Debug.Assert(IsAlive);

      if (IsRunning)
      {
        return;
      }

      Debug.Assert(IsPaused);
      State = ProcessState.Running;
      DoResume();
      OnResume();
    }

    /// <summary>
    ///   Marks this process as completed successfully.  May be called by 
    ///   anyone, generally called by the process itself when it meets success 
    ///   conditions.
    /// </summary>
    public void Succeed()
    {
      Debug.Assert(IsAlive);

      Log.Trace("{0} succeeded", Name);
      State = ProcessState.Succeeded;
      DoSucceed();
      OnSucceeded();
    }
    /// <summary>
    ///   Marks this process as completed with failure.  May be called by 
    ///   anyone.
    /// </summary>
    public void Fail()
    {
      Debug.Assert(IsAlive);

      Log.Trace("{0} failed", Name);
      State = ProcessState.Failed;
      DoFail();
      OnFailed();
    }

    /// <summary>
    ///   Marks this process as aborted.  May be called by anyone.
    /// </summary>
    public void Abort()
    {
      Log.Trace("{0} aborted", Name);
      State = ProcessState.Aborted;
      DoAbort();
      OnAborted();
    }

    /// <summary>
    ///   Aborts this process and all of its children.  Maybe be called by 
    ///   anyone.
    /// </summary>
    public void AbortAll()
    {
      Abort();
      if (Child == null)
      {
        return;
      }

      Child.AbortAll();
      Child = null;
    }

    #endregion

    /// <summary>
    ///   Performs initialization of the process.
    /// </summary>
    /// <returns>
    ///   Success or failure of initialization.
    /// </returns>
    protected abstract bool DoInitialize();

    /// <summary>
    ///   Performs an update of the process.
    /// </summary>
    /// <param name="deltaTime">
    ///   Time elapsed since DoUpdate was last called.
    /// </param>
    protected abstract void DoUpdate(float deltaTime);

    /// <summary>
    ///   Performs an action when the process is paused.
    /// </summary>
    protected abstract void DoPause();

    /// <summary>
    ///   Performs an action when the process is unpaused.
    /// </summary>
    protected abstract void DoResume();

    /// <summary>
    ///   Performs an action when the process succeeds.
    /// </summary>
    protected abstract void DoSucceed();

    /// <summary>
    ///   Performs an action when the process fails.
    /// </summary>
    protected abstract void DoFail();

    /// <summary>
    ///   Performs an action when the process is aborted.
    /// </summary>
    protected abstract void DoAbort();

    #region Event Invokers
    
    private void OnPaused()
    {
      if (Paused != null)
      {
        Paused(this, EventArgs.Empty);
      }
    }

    private void OnResume()
    {
      if (Resumed != null)
      {
        Resumed(this, EventArgs.Empty);
      }
    }

    private void OnSucceeded()
    {
      if (Succeeded != null)
      {
        Succeeded(this, EventArgs.Empty);
      }
    }

    private void OnFailed()
    {
      if (Failed != null)
      {
        Failed(this, EventArgs.Empty);
      }
    }

    private void OnAborted()
    {
      if (Aborted != null)
      {
        Aborted(this, EventArgs.Empty);
      }
    }

    #endregion
  }
}
