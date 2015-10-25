using SFML.Graphics;

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
    ///   See <see cref="SFML.Graphics.RenderWindow.Display()"/>.
    /// </summary>
    void Display();
  }
}
