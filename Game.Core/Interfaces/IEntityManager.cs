using System;
using Game.Core.Entities;

namespace Game.Core.Interfaces
{
  /// <summary>
  ///   The base for all entity manager classes.
  /// </summary>
  /// <remarks>
  ///   The entity life cycle is as follows:
  ///   1.  Create the entity, assign it an id from the entity manager.
  ///   2.  Initialize the entity.
  ///   3.  Add the entity to the entity manager.
  ///   4.  Activate and deactivate the entity as necessary.  This can be done 
  ///       by calling the appropriate methods on the entity manager or the 
  ///       entity itself.
  ///   5.  When done with the entity, destroy it by calling the appropriate
  ///       method on the either the entity manager or the entity itself.
  ///   6.  When the entity manager detects that an entity has been destroyed,
  ///       it will be removed from the manager.
  /// </remarks>
  public interface IEntityManager
    : IManager
  {
    /// <summary>
    ///   Gets the next valid entity id.
    /// </summary>
    int NextId { get; }

    /// <summary>
    ///   Retrieves an entity from the manager.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    ///   The entity, or null if it does not exist.
    /// </returns>
    Entity GetEntity(int id);

    /// <summary>
    ///   Attempt to retrieve an entity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="entity">
    ///   Set to the entity, or null if it does not exist.
    /// </param>
    /// <returns>
    ///   True if the entity was found.
    /// </returns>
    bool TryGetEntity(int id, out Entity entity);

    /// <summary>
    ///   Adds an entity to be handled by the manager.
    /// </summary>
    /// <param name="entity">
    ///   An initialized entity.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   entity is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The entity's id is not unique.
    /// </exception>
    void AddEntity(Entity entity);

    /// <summary>
    ///   Activates an entity.
    /// </summary>
    /// <param name="id"></param>
    void ActivateEntity(int id);

    /// <summary>
    ///   Deactivates an entity.
    /// </summary>
    /// <param name="id"></param>
    void DeActivateEntity(int id);

    /// <summary>
    ///   Destroys an entity.
    /// </summary>
    /// <param name="id"></param>
    void DestroyEntity(int id);
  }
}
