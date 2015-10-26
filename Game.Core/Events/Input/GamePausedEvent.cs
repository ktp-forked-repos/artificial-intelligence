namespace Game.Core.Events.Input
{
  /// <summary>
  ///   Fires when the game is paused or unpaused by the user.
  /// </summary>
  public class GamePausedEvent
    : EventBase
  {
    /// <summary>
    ///   True if the game is paused, false to unpause.
    /// </summary>
    public bool IsPaused { get; set; }
  }
}
