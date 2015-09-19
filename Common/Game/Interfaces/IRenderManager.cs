using System;
using SFML.Graphics;

namespace Common.Game.Interfaces
{
  /// <summary>
  ///   The base for all render managers.
  /// </summary>
  public interface IRenderManager
    : IManager
  {
    /// <summary>
    ///   Draws all renderable objects to the render target.
    /// </summary>
    /// <param name="target"></param>
    /// <exception cref="ArgumentNullException">
    ///   target is null.
    /// </exception>
    void DrawOneFrame(RenderTarget target);

    /// <summary>
    ///   Add an object to be rendered.
    /// </summary>
    /// <param name="renderable"></param>
    /// <exception cref="ArgumentNullException">
    ///   renderable is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   This renderable is already being tracked by the render manager.  At 
    ///   least, the render manager already has a renderable with this id.
    /// </exception>
    void AddRenderable(IRenderable renderable);

    /// <summary>
    ///   Remove an object from the manager.
    /// </summary>
    /// <param name="renderable"></param>
    /// <exception cref="ArgumentNullException">
    ///   renderable is null.
    /// </exception>
    void RemoveRenderable(IRenderable renderable);
  }
}
