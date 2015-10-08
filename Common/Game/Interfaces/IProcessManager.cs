﻿using System;
using System.Collections.Generic;
using Common.Game.Processes;

namespace Common.Game.Interfaces
{
  /// <summary>
  ///   Base interface for process managers.
  /// 
  ///   The interface is intentionally limited, because most process management 
  ///   is done by retrieving a process and changing its state via control 
  ///   methods.  The manager will then remove or otherwise respond to the state
  ///   changes.
  /// </summary>
  public interface IProcessManager
    : IManager
  {
    /// <summary>
    ///   Get a process by its id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    ///   The process or null if it was not found.
    /// </returns>
    ProcessBase GetProcess(int id);

    /// <summary>
    ///   Gets all processes of a given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    IEnumerable<T> GetProcesses<T>()
      where T : ProcessBase;

    /// <summary>
    ///   Adds a process to the manager.
    /// </summary>
    /// <param name="process"></param>
    /// <exception cref="ArgumentNullException">
    ///   process is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   process does not have a unique id.
    /// </exception>
    void AddProcess(ProcessBase process);
  }
}
