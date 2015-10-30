using System;
using System.Diagnostics.CodeAnalysis;
using SFML.Window;

namespace Game.Core.SFML
{
  /// <summary>
  ///   Joins the SFML RenderWindow class with our IRenderWindow interface.
  ///   All constructors are direct implementations of RenderWindow 
  ///   constructors.
  /// </summary>
  [ExcludeFromCodeCoverage]
  public class RenderWindow
    : global::SFML.Graphics.RenderWindow, IRenderWindow
  {
    public RenderWindow(VideoMode mode, string title) 
      : base(mode, title)
    {
    }

    public RenderWindow(VideoMode mode, string title, Styles style) 
      : base(mode, title, style)
    {
    }

    public RenderWindow(VideoMode mode, string title, Styles style, 
      ContextSettings settings) 
      : base(mode, title, style, settings)
    {
    }

    public RenderWindow(IntPtr handle) 
      : base(handle)
    {
    }

    public RenderWindow(IntPtr handle, ContextSettings settings) 
      : base(handle, settings)
    {
    }
  }
}
