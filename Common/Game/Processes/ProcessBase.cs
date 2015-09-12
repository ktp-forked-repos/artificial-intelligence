using System;
using System.Diagnostics;
using System.Reflection;
using Common.Extensions;
using log4net;

namespace Common.Game.Processes
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
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

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
      BeginPaused = false;
      ActivateChildOnFailure = false;
      ActivateChildOnAbort = false;
    }

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
    public double RunningTime { get; private set; }

    /// <summary>
    ///   The current state of the process.
    /// </summary>
    public ProcessState State { get; private set; }

    /// <summary>
    ///   If true the process is paused immediately after initialization.
    /// 
    ///   Default: false.
    /// </summary>
    public bool BeginPaused { get; set; }

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

    #region State Check Properties

    /// <summary>
    ///   Was the process initialized successfully.
    /// </summary>
    public bool Initialized
    {
      get { return State != ProcessState.NotInitialized; }
    }

    /// <summary>
    ///   Is the process in a live state.
    /// </summary>
    public bool Alive
    {
      get
      {
        return State == ProcessState.Running || State == ProcessState.Paused;
      }
    }

    /// <summary>
    ///   Is the process alive and running.
    /// </summary>
    public bool Running
    {
      get { return State == ProcessState.Running; }
    }

    /// <summary>
    ///   Is the process alive and paused.
    /// </summary>
    public bool Paused
    {
      get { return State == ProcessState.Paused; }
      set
      {
        Debug.Assert(Alive);
        if (value)
        {
          State = ProcessState.Paused;
          OnPause();
        }
        else
        {
          State = ProcessState.Running;
        }
      }
    }

    /// <summary>
    ///   Is the process in any completed state.
    /// </summary>
    public bool Completed
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
    public bool Succeeded
    {
      get { return State == ProcessState.Succeeded; }
    }

    /// <summary>
    ///   Did the process fail.
    /// </summary>
    public bool Failed
    {
      get { return State == ProcessState.Failed; }
    }

    /// <summary>
    ///   Was the process aborted.
    /// </summary>
    public bool Aborted
    {
      get { return State == ProcessState.Aborted; }
    }

    #endregion
    #region Child Methods

    /// <summary>
    ///   Attach a child to this process chain.  The process will be attached 
    ///   to the first child which does not have a child process.
    /// </summary>
    /// <param name="child"></param>
    /// <exception cref="ArgumentNullException">
    ///   child is null.
    /// </exception>
    public void AttachChild(ProcessBase child)
    {
      if (child == null) throw new ArgumentNullException("child");

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
    ///   Initializes a new process.  Should only be called by the process 
    ///   manager.
    /// </summary>
    /// <returns>
    ///   Success or failure of initialization.
    /// </returns>
    public bool Initialize()
    {
      Debug.Assert(!Initialized);

      if (!OnInitialize())
      {
        Log.ErrorFmt("{0} failed initialization", Name);
        return false;
      }

      Log.VerboseFmt("{0} initialized", Name);
      if (BeginPaused)
      {
        State = ProcessState.Paused;
        OnPause();
      }
      else
      {
        State = ProcessState.Running;
      }
      return true;
    }

    /// <summary>
    ///   Updates a running process.  Should only be called by the process 
    ///   manager.
    /// </summary>
    /// <param name="deltaTime">
    ///   Elapsed time since the last update.
    /// </param>
    public void Update(double deltaTime)
    {
      Debug.Assert(Running);

      RunningTime += deltaTime;
      OnUpdate(deltaTime);
    }

    /// <summary>
    ///   Marks this process as completed successfully.  May be called by 
    ///   anyone, general called by the process itself when it meets success 
    ///   conditions.
    /// </summary>
    public void Succeed()
    {
      Debug.Assert(Alive);

      Log.VerboseFmt("{0} succeeded", Name);
      State = ProcessState.Succeeded;
      OnSucceed();
    }
    /// <summary>
    ///   Marks this process as completed with failure.  May be called by 
    ///   anyone.
    /// </summary>
    public void Fail()
    {
      Debug.Assert(Alive);

      Log.VerboseFmt("{0} failed");
      State = ProcessState.Failed;
      OnFail();
    }

    /// <summary>
    ///   Marks this process as aborted.  May be called by anyone.
    /// </summary>
    public void Abort()
    {
      Log.VerboseFmt("{0} aborted", Name);
      State = ProcessState.Failed;
      OnAbort();
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
    protected abstract bool OnInitialize();

    /// <summary>
    ///   Performs an update of the process.
    /// </summary>
    /// <param name="deltaTime">
    ///   Time elapsed since OnUpdate was last called.
    /// </param>
    protected abstract void OnUpdate(double deltaTime);

    /// <summary>
    ///   Performs an action when the process is paused.
    /// </summary>
    protected abstract void OnPause();

    /// <summary>
    ///   Performs an action when the process succeeds.
    /// </summary>
    protected abstract void OnSucceed();

    /// <summary>
    ///   Performs an action when the process fails.
    /// </summary>
    protected abstract void OnFail();

    /// <summary>
    ///   Performs an action when the process is aborted.
    /// </summary>
    protected abstract void OnAbort();
  }
}
