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
  public abstract class RenderComponentBase
    : ComponentBase, IRenderable
  {
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    ///   Render states to be used when drawing.
    /// </summary>
    protected RenderStates RenderStates { get; set; }

    /// <summary>
    ///   Create the component.
    /// </summary>
    /// <param name="parent"></param>
    protected RenderComponentBase(Entity parent) 
      : base(parent)
    {
      RenderStates = new RenderStates
      {
        BlendMode = BlendMode.Alpha,
        Transform = Transform.Identity
      };
    }
    
    #region IRenderable

    public int CompareTo(IRenderable other)
    {
      return RenderableCompare.CompareTo(this, other);
    }

    public int RenderId { get; set; }
    
    public int RenderDepth { get; protected set; }

    public abstract void Draw(RenderTarget target);

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
  }
}
