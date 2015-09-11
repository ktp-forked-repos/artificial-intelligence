using System;

namespace Common.Game.Events
{
  /// <summary>
  ///   The base class for all events in the game.
  /// </summary>
  public abstract class Event
    : EventArgs
  {
    /// <summary>
    ///   Creates the event.
    /// </summary>
    protected Event()
    {
      TimeStamp = DateTime.Now.Ticks;
    }

    /// <summary>
    ///   The creation timestamp of the event in system ticks.
    /// </summary>
    public long TimeStamp { get; private set; }
  }
}
