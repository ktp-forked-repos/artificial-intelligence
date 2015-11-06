using Game.Core.Entities;
using Game.Core.Managers.Interfaces;

namespace Game.Core.Factories
{
  /// <summary>
  ///   The interface for entity factories.
  /// </summary>
  public interface IEntityFactory
    : IFactory<Entity>
  {
    /// <summary>
    ///   The game's entity manager, used to inject ids into newly created
    ///   entities.
    /// </summary>
    IEntityManager EntityManager { get; set; }
  }
}
