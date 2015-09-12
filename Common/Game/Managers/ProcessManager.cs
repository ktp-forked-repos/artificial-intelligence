using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Common.Game.Interfaces;
using Common.Game.Processes;
using log4net;

namespace Common.Game.Managers
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
    private readonly List<ProcessBase> m_toRemove =
      new List<ProcessBase>(InitialListSize);

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

    public void Update(double deltaTime, double maxTime)
    {
      if (Paused)
      {
        return;
      }

      UpdateProcesses(deltaTime);
      ProcessPendingAddRemove();
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
    #region IEventManager

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

      Debug.Assert(!process.Initialized);
      m_processes.Add(process);
      Log.VerboseFmt("Added {0}", process.Name);
    }

    #endregion

    private void UpdateProcesses(double deltaTime)
    {
      foreach (var process in m_processes)
      {
        switch (process.State)
        {
          case ProcessState.NotInitialized:
            if (!process.Initialize())
            {
              Log.ErrorFmt("{0} failed initialization and will be removed",
                process.Name);
              m_toRemove.Add(process);
            }
            break;

          case ProcessState.Running:
            process.Update(deltaTime);
            break;

          case ProcessState.Paused:
            // intentionally empty
            break;

          case ProcessState.Succeeded:
          case ProcessState.Failed:
          case ProcessState.Aborted:
            var activateChild = process.Succeeded ||
                                process.ActivateChildOnFailure ||
                                process.ActivateChildOnAbort;
            if (activateChild && process.Child != null)
            {
              var child = process.RemoveChild();
              Log.VerboseFmt("Activating {0}, child of {1}", child.Name,
                process.Name);
              m_toAdd.Add(child);
            }
            m_toRemove.Add(process);
            break;

          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    private void ProcessPendingAddRemove()
    {
      if (m_toRemove.Count > 0)
      {
        // track the number left to remove
        var toRemove = m_toRemove.Count;

        // iterate backwards over the process list so we can remove as we go
        for (var procIndex = m_processes.Count - 1; procIndex >= 0; procIndex--)
        {
          var process = m_processes[procIndex];

          // iterate over the valid processes remaining to be removed
          for (var removeIndex = 0; removeIndex < toRemove; removeIndex++)
          {
            if (m_toRemove[removeIndex].Id != process.Id)
            {
              continue;
            }

            m_processes.RemoveAt(procIndex);
            Log.VerboseFmt("{0} removed", process.Name);

            // based on the C++ swap & pop idiom... except we don't care about
            // keeping the process that was just removed so we overwrite it
            // with the last process on the list waiting for removal
            m_toRemove[removeIndex] = m_toRemove[toRemove - 1];
            toRemove--;
            break;
          }

          if (toRemove == 0)
          {
            break;
          }
        }

        m_toRemove.Clear();
      }

      m_processes.AddRange(m_toAdd);
      m_toAdd.Clear();
    }
  }
}
