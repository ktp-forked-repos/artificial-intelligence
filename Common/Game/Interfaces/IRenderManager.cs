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
  }
}
