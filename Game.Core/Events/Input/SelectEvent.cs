using SFML.System;

namespace Game.Core.Events.Input
{
  /// <summary>
  ///   Fires when the user click to select an item.
  /// </summary>
  public class SelectEvent
    : EventBase
  {
    /// <summary>
    ///   The window coordinates of the click.
    /// </summary>
    public Vector2i Position { get; set; }
  }
}
