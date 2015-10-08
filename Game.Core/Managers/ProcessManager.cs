using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Game.Core.Interfaces;
using Game.Core.Processes;
using log4net;

namespace Game.Core.Managers
{
  /// <summary>
  ///   The general process manager implementation.
  /// </summary>
  public class ProcessManager
    : IProcessManager
  {
    private const int InitialListSize = 10;

    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    private readonly List<ProcessBase> m_processes = 
      new List<ProcessBase>(InitialListSize);
    private readonly List<ProcessBase> m_toAdd = 
      new List<ProcessBase>(InitialListSize);
    private readonly List<int> m_toRemove = new List<int>(InitialListSize);

    public ProcessManager()
    {
      CanPause = true;
    }

    #region IManager

    public bool CanPause { get; private set; }
    
    public bool Paused { get; set; }

    public bool Initialize()
    {
      return true;
    }

    public bool PostInitialize()
    {
      return true;
    }

    public void Update(float deltaTime, float maxTime)
    {
      if (Paused)
      {
        return;
      }

      ProcessPendingRemoves();
      ProcessPendingAdds();
      UpdateProcesses(deltaTime);
    }

    public void Shutdown()
    {
      foreach (var process in m_processes)
      {
        process.AbortAll();
      }

      m_processes.Clear();
    }

    #endregion
    #region IProcessManager

    public ProcessBase GetProcess(int id)
    {
      return m_processes.SingleOrDefault(p => p.Id == id);
    }

    public IEnumerable<T> GetProcesses<T>() 
      where T : ProcessBase
    {
      return m_processes.OfType<T>();
    }

    public void AddProcess(ProcessBase process)
    {
      if (process == null) throw new ArgumentNullException("process");
      if (m_processes.Any(p => p.Id == process.Id))
        throw new InvalidOperationException(string.Format(
          "Process id {0} is already in use", process.Id));

      Debug.Assert(process.IsInitialized);
      m_processes.Add(process);
      Log.VerboseFmt("Added {0}", process.Name);
    }

    #endregion

    private void UpdateProcesses(float deltaTime)
    {
      foreach (var process in m_processes)
      {
        switch (process.State)
        {
          case ProcessState.NotInitialized:
          case ProcessState.Paused:
            // intentionally empty
            break;

          case ProcessState.Running:
            process.Update(deltaTime);
            break;
            
          case ProcessState.Succeeded:
          case ProcessState.Failed:
          case ProcessState.Aborted:
            var activateChild = process.HasSucceeded ||
                                process.ActivateChildOnFailure ||
                                process.ActivateChildOnAbort;
            if (activateChild && process.Child != null)
            {
              var child = process.RemoveChild();
              Log.VerboseFmt("Activating {0}, child of {1}", child.Name,
                process.Name);
              m_toAdd.Add(child);
            }
            m_toRemove.Add(process.Id);
            break;

          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    private void ProcessPendingAdds()
    {
      if (m_toAdd.Count == 0)
      {
        return;
      }

      m_processes.AddRange(m_toAdd);
      m_toAdd.Clear();
    }

    private void ProcessPendingRemoves()
    {
      if (m_toRemove.Count == 0)
      {
        return;
      }

      var remaining = m_processes.RemoveAllItems(m_toRemove, 
        (process, id) => process.Id == id);
      Debug.Assert(!remaining.Any());
      m_toRemove.Clear();
    }
  }
}
