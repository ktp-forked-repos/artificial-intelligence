using System;
using FarseerPhysics.Dynamics;

namespace Game.Core.Managers.Interfaces
{
  /// <summary>
  ///   The base for all physics manager classes.  Updates a Farseer physics
  ///   world based on elapsed game time and fires events before and after 
  ///   updates.
  /// </summary>
  public interface IPhysicsManager
    : IManager
  {
    /// <summary>
    ///   Event fires immediately before the physics world performs an update.  
    ///   The parameter is the elapsed time in seconds that the world will be 
    ///   updated by.
    /// </summary>
    event Action<float> PreStep;

    /// <summary>
    ///   Event fires immediately after the physics world performs an update.  
    ///   The parameter is the elapsed time in seconds that the world was 
    ///   updated by.
    /// </summary>
    event Action<float> PostStep;

    /// <summary>
    ///   The current Farseer physics world.
    /// </summary>
    World World { get; }
  }
}
