using Microsoft.Xna.Framework;

namespace Game.Core.Events.Input
{
  /// <summary>
  ///   Fires when the user drags the view.
  /// </summary>
  public class ViewDragEvent
    : EventBase
  {
    /// <summary>
    ///   The amount that the view moved by as a percentage of the window size.
    /// </summary>
    public Vector2 Delta { get; set; }
  }
}
