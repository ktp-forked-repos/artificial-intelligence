namespace Game.Core.Events.Input
{
  /// <summary>
  ///   Fires when the user begins or ends a movement action.
  /// </summary>
  public class MoveEvent
    : EventBase
  {
    public enum MoveAction
    {
      BeginUp,
      EndUp,
      BeginDown,
      EndDown,
      BeginLeft,
      EndLeft,
      BeginRight,
      EndRight
    }

    /// <summary>
    ///   The action this event signifies.
    /// </summary>
    public MoveAction Action { get; set; }
  }
}
