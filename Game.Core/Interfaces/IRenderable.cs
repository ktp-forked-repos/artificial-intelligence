using System;
using SFML.Graphics;

namespace Game.Core.Interfaces
{
  /// <summary>
  ///   Holds some constants for common render depth values.
  /// </summary>
  public static class RenderDepth
  {
    public const int Default = 0;

    public const int GUI = 1;
    public const int DebugOverlay = 100;

    public const int Vehicle = 10000;
    public const int Projectile = 15000;
    public const int Terrain = 20000;
  }

  /// <summary>
  ///   The interface for all renderable items.
  /// 
  ///   IComparable should be implemented by calling 
  ///   <see cref="RenderableCompare.CompareTo"/> for consistent comparisons.
  /// </summary>
  public interface IRenderable
    : IComparable<IRenderable>
  {
    /// <summary>
    ///   The unique id for this renderable.  This will be generated and 
    ///   assigned by the render manager.
    /// </summary>
    int RenderId { get; set; }

    /// <summary>
    ///   The depth this object is rendered at, with smaller values being 
    ///   closer to the camera.
    /// </summary>
    int RenderDepth { get; }

    /// <summary>
    ///   Draws this object onto the provided render target.
    /// </summary>
    /// <param name="target"></param>
    /// <exception cref="ArgumentNullException">
    ///   target is null.
    /// </exception>
    void Draw(RenderTarget target);
  }

  /// <summary>
  ///   Provides a comparison function for two renderables.
  /// </summary>
  public static class RenderableCompare
  {
    public static int CompareTo(IRenderable a, IRenderable b)
    {
      if (a.RenderDepth > b.RenderDepth)
      {
        return -1;
      }
      if (a.RenderDepth < b.RenderDepth)
      {
        return 1;
      }

      return 0;
    }
  }
}
