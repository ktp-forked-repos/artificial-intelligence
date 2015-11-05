using System;
using System.Diagnostics.CodeAnalysis;
using NLog;
using SFML.Graphics;

namespace Game.Core.Entities.Components
{
  /// <summary>
  ///   Renders a basic primitive shape.  The shape may be supplied at 
  ///   initialization or created later via a function. 
  /// </summary>
  /// <remarks>
  ///   No unit testing because it's a pain to cover all the possibilities with
  ///   mocking render target and shape.
  /// </remarks>
  [ExcludeFromCodeCoverage]
  public sealed class PrimitiveRenderComponent
    : RenderComponentBase
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

    private readonly Func<Shape> m_creator; 
    private Shape m_shape;
    private Color m_fillColor;
    private Color m_outlineColor;
    private float m_outlineThickness;
    private bool m_updateShapeProperties = false;

    // sets defaults
    private PrimitiveRenderComponent(Entity parent)
      : base(parent)
    {
      NeedsUpdate = false;
    }

    /// <summary>
    ///   Create the component with a supplied shape.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="shape"></param>
    /// <exception cref="ArgumentNullException">
    ///   shape is null.
    /// </exception>
    public PrimitiveRenderComponent(Entity parent, Shape shape) 
      : this(parent)
    {
      if (shape == null) throw new ArgumentNullException("shape");

      m_shape = shape;
    }

    /// <summary>
    ///   Create the component with a shape creator function, deferring shape
    ///   creation until initialization.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="creator"></param>
    /// <exception cref="ArgumentNullException">
    ///   creator is null
    /// </exception>
    public PrimitiveRenderComponent(Entity parent, Func<Shape> creator)
      : this(parent)
    {
      if (creator == null) throw new ArgumentNullException("creator");

      m_creator = creator;
    }

    /// <summary>
    ///   Get or set the fill color of the shape.  May be set at any time.
    /// </summary>
    public Color FillColor
    {
      get { return m_fillColor; }
      set
      {
        m_fillColor = value;
        m_updateShapeProperties = true;
      }
    }

    /// <summary>
    ///   Get or set the outline color of the shape.  May be set at any time.
    /// </summary>
    public Color OutlineColor
    {
      get { return m_outlineColor; }
      set
      {
        m_outlineColor = value;
        m_updateShapeProperties = true;
      }
    }

    /// <summary>
    ///   Get or set the outline thickness of the shape.  Maybe be set at any 
    ///   time.
    /// </summary>
    public float OutlineThickness
    {
      get { return m_outlineThickness; }
      set
      {
        m_outlineThickness = value;
        m_updateShapeProperties = true;
      }
    }

    #region RenderComponentBase
    
    public override void Update(float deltaTime)
    {
    }

    protected override void DoDraw(RenderTarget target)
    {
      if (m_updateShapeProperties)
      {
        m_shape.FillColor = FillColor;
        m_shape.OutlineColor = OutlineColor;
        m_shape.OutlineThickness = OutlineThickness;
        m_updateShapeProperties = false;
      }

      RenderStates.Transform = Parent.TransformComponent.GraphicsTransform;
      target.Draw(m_shape, RenderStates);
    }

    protected override bool DoInitialize()
    {
      if (!base.DoInitialize())
      {
        return false;
      }

      m_shape = m_shape ?? m_creator();
      if (m_shape == null)
      {
        Log.Error("{0} {1} failed to create shape", 
          Parent.Name,GetType().FullName);
        return false;
      }

      return true;
    }

    #endregion
    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      if (disposing && m_shape != null)
      {
        m_shape.Dispose();
      }
    }

    #endregion
  }
}
