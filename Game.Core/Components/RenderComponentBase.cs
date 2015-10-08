using System;
using System.Reflection;
using Common.Extensions;
using Game.Core.Interfaces;
using log4net;
using SFML.Graphics;

namespace Game.Core.Components
{
  /// <summary>
  ///   The base for all SFML renderable components.  Adding a renderable 
  ///   component requires the entity to contain a transform component.
  /// </summary>
  public abstract class RenderComponentBase
    : ComponentBase, IRenderable
  {
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    private RenderStates m_renderStates = new RenderStates
    {
      BlendMode = BlendMode.Alpha
    };

    /// <summary>
    ///   The render states to be used for rendering, using the parent entity's
    ///   transform.
    /// </summary>
    protected RenderStates RenderStates 
    {
      get
      {
        m_renderStates.Transform = Parent.TransformComponent.GraphicsTransform;
        return m_renderStates;
      } 
    }

    /// <summary>
    ///   Create the component.
    /// </summary>
    /// <param name="parent"></param>
    protected RenderComponentBase(Entity parent) 
      : base(parent)
    {
      RenderId = 0;
    }

    #region ComponentBase

    protected override bool DoInitialize()
    {
      if (!base.DoInitialize())
      {
        return false;
      }

      Log.ErrorFmtIf(Parent.TransformComponent == null, 
        "{0} tried to initialize a TransformComponentBase but " +
        "has no TransformComponent", Parent.Name);
      return Parent.TransformComponent != null;
    }

    #endregion
    #region IRenderable

    public int RenderId { get; set; }
    
    public int RenderDepth { get; protected set; }

    public abstract void Draw(RenderTarget target);

    public int CompareTo(IRenderable other)
    {
      if (other == null) throw new ArgumentNullException("other");

      return RenderableCompare.CompareTo(this, other);
    }
    
    #endregion
  }
}
