using System;
using System.Collections.Generic;
using Game.Core.Interfaces;
using SFML.Graphics;

namespace Game.Core.Managers.Interfaces
{
  /// <summary>
  ///   The base for all render managers.
  /// </summary>
  public interface IRenderManager
    : IManager
  {
    /// <summary>
    ///   The frame rate that the manager will attempt to maintain.  Must be 
    ///   a positive value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   The value is zero or negative.
    /// </exception>
    int TargetFrameRate { get; set; }

    /// <summary>
    ///   The time interval after which the manager renders a frame.
    /// </summary>
    float UpdateInterval { get; }

    /// <summary>
    ///   The color the render target is cleared to before rendering a frame.
    /// </summary>
    Color BackgroundColor { get; set; }

    /// <summary>
    ///   The current view rendered by the manager.
    /// </summary>
    View View { get; }

    /// <summary>
    ///   Current renderable objects in the manager.
    /// </summary>
    IReadOnlyCollection<IRenderable> Renderables { get; }

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
