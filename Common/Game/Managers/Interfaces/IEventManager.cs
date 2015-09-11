using System;
using Common.Game.Events;

namespace Common.Game.Managers.Interfaces
{
  /// <summary>
  ///   Base interface for event managers.
  /// </summary>
  public interface IEventManager
    : IManager
  {
    /// <summary>
    ///   Adds a listener for event type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listener"></param>
    /// <exception cref="ArgumentNullException">
    ///   listener is null.
    /// </exception>
    void AddListener<T>(Action<Event> listener)
      where T : Event;

    /// <summary>
    ///   Removes a listener for event type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listener"></param>
    /// <exception cref="ArgumentNullException">
    ///   listener is null.
    /// </exception>
    void RemoveListener<T>(Action<Event> listener)
      where T : Event;

    /// <summary>
    ///   Immediately triggers an event, ignoring the queue.
    /// </summary>
    /// <param name="evt"></param>
    /// <exception cref="ArgumentNullException">
    ///   evt is null.
    /// </exception>
    void TriggerEvent(Event evt);

    /// <summary>
    ///   Adds an event to the queue.
    /// </summary>
    /// <param name="evt"></param>
    /// <exception cref="ArgumentNullException">
    ///   evt is null.
    /// </exception>
    void QueueEvent(Event evt);

    /// <summary>
    ///   Removes the oldest event of type T.  Events cannot be aborted after 
    ///   Update has been called and event processing begun.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    ///   True if an event was aborted.
    /// </returns>
    bool AbortFirstEvent<T>()
      where T : Event;

    /// <summary>
    ///   Removes all pending events of type T.  Events cannot be aborted after 
    ///   Update has been called and event processing begun.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>
    ///   The number of events removed.
    /// </returns>
    int AbortEvents<T>()
      where T : Event;

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
