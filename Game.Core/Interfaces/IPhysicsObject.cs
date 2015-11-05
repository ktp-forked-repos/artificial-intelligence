using FarseerPhysics.Dynamics;

namespace Game.Core.Interfaces
{
  /// <summary>
  ///   The interface for all objects that have a physics body.
  /// </summary>
  public interface IPhysicsObject
  {
    /// <summary>
    ///   The physics body for this object.
    /// </summary>
    Body Body { get; }
  }
}
