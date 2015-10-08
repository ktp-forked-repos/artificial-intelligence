namespace Game.Core.Events.EntityEvents
{
  /// <summary>
  ///   Signals that an entity was added to the entity manager.
  /// 
  ///   The entity is initialized when this event is triggered.
  /// </summary>
  public class EntityAddedEvent
    : EventBase
  {
    public int EntityId { get; set; }
  }
}
