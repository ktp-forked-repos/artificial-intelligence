namespace Game.Core.Events.Input
{
  /// <summary>
  ///   Fires when the user zooms the view.
  /// </summary>
  public class ViewZoomEvent
    : EventBase
  {
    /// <summary>
    ///   The number of steps to zoom.  Positive amount zooms in, negative
    ///   zooms out.
    /// </summary>
    public int Delta { get; set; }
  }
}
