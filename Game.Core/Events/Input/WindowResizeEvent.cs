using SFML.System;

namespace Game.Core.Events.Input
{
  /// <summary>
  ///   Fires when the user resizes the game window.
  /// </summary>
  public class WindowResizeEvent
    : EventBase
  {
    /// <summary>
    ///   The new window size.
    /// </summary>
    public Vector2u Size { get; set; }
  }
}
