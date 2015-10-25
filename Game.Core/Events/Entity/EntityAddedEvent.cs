namespace Game.Core.Events.Entity
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
