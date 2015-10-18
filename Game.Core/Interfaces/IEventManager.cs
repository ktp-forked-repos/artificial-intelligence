using System;
using System.Collections.Generic;
using Game.Core.Events;

namespace Game.Core.Interfaces
{
  /// <summary>
  ///   Base interface for event managers.
  /// </summary>
  public interface IEventManager
    : IManager
  {
    /// <summary>
    ///   Events that are pending and have not been fired yet.
    /// </summary>
    IReadOnlyCollection<EventBase> PendingEvents { get; }

    /// <summary>
    ///   Gets the number of listeners registered for type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    int GetListenerCount<T>()
      where T : EventBase;

    /// <summary>
    ///   Adds a listener for event type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listener"></param>
    /// <exception cref="ArgumentNullException">
    ///   listener is null.
    /// </exception>
    void AddListener<T>(Action<EventBase> listener)
      where T : EventBase;

    /// <summary>
    ///   Removes a listener for event type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listener"></param>
    /// <exception cref="ArgumentNullException">
    ///   listener is null.
    /// </exception>
    void RemoveListener<T>(Action<EventBase> listener)
      where T : EventBase;

    /// <summary>
    ///   Immediately triggers an event, ignoring the queue.
    /// </summary>
    /// <param name="evt"></param>
    /// <exception cref="ArgumentNullException">
    ///   evt is null.
    /// </exception>
    void TriggerEvent(EventBase evt);

    /// <summary>
    ///   Adds an event to the queue.
    /// </summary>
    /// <param name="evt"></param>
    /// <exception cref="ArgumentNullException">
    ///   evt is null.
    /// </exception>
    void QueueEvent(EventBase evt);

    /// <summary>
    ///   Removes the oldest event of type T.  Events cannot be aborted after 
    ///   Update has been called and event processing begun.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    ///   True if an event was aborted.
    /// </returns>
    bool AbortFirstEvent<T>()
      where T : EventBase;

    /// <summary>
    ///   Removes all pending events of type T.  Events cannot be aborted after 
    ///   Update has been called and event processing begun.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    ///   The number of events removed.
    /// </returns>
    int AbortEvents<T>()
      where T : EventBase;

    /// <summary>
    ///   Clears all events from the queue.  Events cannot be aborted after 
    ///   Update has been called and event processing begun.
    /// </summary>
    /// <returns>
    ///   The number of events cleared.
    /// </returns>
    int AbortAllEvents();
  }
}
