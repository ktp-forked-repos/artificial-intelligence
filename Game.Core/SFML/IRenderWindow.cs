using System;
using SFML.Graphics;
using SFML.Window;

namespace Game.Core.SFML
{
  /// <summary>
  ///   SFML doesn't provide an adequate interface for RenderWindow, so we'll
  ///   make our own for easier dependency injection.
  /// </summary>
  public interface IRenderWindow
    : RenderTarget
  {
    /// <summary>
    ///   See <see cref="RenderWindow.Display()"/>.
    /// </summary>
    void Display();

    /// <summary>
    ///   See <see cref="Window.Resized"/>
    /// </summary>
    event EventHandler<SizeEventArgs> Resized;
  }
}
