namespace Common.Game.Events.EntityEvents
{
  /// <summary>
  ///   Signals that an entity is about to be removed from the entity manager.
  /// 
  ///   This event is triggered immediately before the entity is removed.
  /// </summary>
  public class EntityRemovedEvent
    : EventBase
  {
    public int EntityId { get; set; }
  }
}
