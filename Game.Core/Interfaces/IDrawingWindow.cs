using System;
using System.Security.Cryptography.X509Certificates;

namespace Game.Core.Interfaces
{
  /// <summary>
  ///   A window with a panel where the game engine will draw.
  /// </summary>
  public interface IDrawingWindow
  {
    /// <summary>
    ///   The visibility of the UI window.
    /// </summary>
    bool Visible { get; }

    /// <summary>
    ///   The pointer to the panel that SFML will draw to.
    /// </summary>
    IntPtr DrawingPanelHandle { get; }
  }
}
