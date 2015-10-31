using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Common.Extensions;
using Game.Core.Interfaces;
using log4net;
using SFML.Graphics;

namespace Game.Core.Entities.Components
{
  /// <summary>
  ///   Provides a common base for rendering components.  Any component class 
  ///   may implement IRenderable in order to draw, this is just a convenience 
  ///   setup for components dedicated to drawing.
  /// 
  ///   Note: This class requires the entity to have a transform component, or
  ///   it will fail initialization.
  /// </summary>
  /// <remarks>
  ///   No testing because there's so little logic in the class it isn't worth
  ///   the trouble.
  /// </remarks>
  [ExcludeFromCodeCoverage]
  public abstract class RenderComponentBase
    : ComponentBase, IRenderable
  {
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    ///   Render states to be used when drawing.
    /// </summary>
    protected RenderStates RenderStates;

    /// <summary>
    ///   Create the component.
    /// </summary>
    /// <param name="parent"></param>
    protected RenderComponentBase(Entity parent) 
      : base(parent)
    {
      DrawingEnabled = true;
      RenderStates = new RenderStates
      {
        BlendMode = BlendMode.Alpha,
        Transform = Transform.Identity
      };
    }

    /// <summary>
    ///   If false, this shape is not drawn.
    /// </summary>
    /// <remarks>
    ///   Default: true
    /// </remarks>
    public bool DrawingEnabled { get; set; }
    
    #region IRenderable

    public int CompareTo(IRenderable other)
    {
      return RenderableCompare.CompareTo(this, other);
    }

    public int RenderId { get; set; }
    
    public int RenderDepth { get; set; }

    public void Draw(RenderTarget target)
    {
      if (!DrawingEnabled)
      {
        return;
      }

      DoDraw(target);
    }

    #endregion
    #region ComponentBase

    protected override bool DoInitialize()
    {
      if (!base.DoInitialize())
      {
        return false;
      }

      if (Parent.TransformComponent == null)
      {
        Log.ErrorFmt("{0} tried to initialize RenderComponentBase without " +
                     "a TransformComponentBase", Parent.Name);
        return false;
      }

      return true;
    }

    #endregion

    /// <summary>
    ///   Does the actual drawing action.
    /// </summary>
    /// <param name="target"></param>
    protected abstract void DoDraw(RenderTarget target);
  }
}
