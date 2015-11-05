using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Game.Core.Entities;
using Game.Core.Events;
using Game.Core.Events.Entity;
using Game.Core.Events.Input;
using Game.Core.Interfaces;
using Game.Core.Managers.Interfaces;
using Game.Core.SFML;
using log4net;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Game.Core.Managers
{
  /// <summary>
  ///   The general render manager implementation.  Renders to a SFML 
  ///   RenderTarget;
  /// </summary>
  public sealed class RenderManager
    : IRenderManager
  {
    private static readonly ILog Log = LogManager.GetLogger(
     MethodBase.GetCurrentMethod().DeclaringType);

    public const int DefaultFrameRate = 60;
    public static readonly Color DefaultBackgroundColor = Color.White;
    public const float InitialViewWidth = 100f;
    public const float MinViewWidth = 10f;
    public const float ZoomPercentIncrement = 10f;

    private const int InitialListSize = 10;

    // dependencies
    private readonly IEntityManager m_entityManager;
    private readonly IEventManager m_eventManager;
    private readonly IRenderWindow m_renderWindow;
    
    private readonly List<IRenderable> m_renderables = 
      new List<IRenderable>(InitialListSize);
    private bool m_stateChanged = false;
    private float m_timeSinceLastRender = 0f;
    private int m_nextRenderId = 1;
    private int m_targetFrameRate;
    private float m_viewWidth = InitialViewWidth;

    // renderables with the default id of 0 get an id from this
    private int NextRenderId
    {
      get { return m_nextRenderId++; }
    }

    /// <summary>
    ///   Create the render manager.
    /// </summary>
    /// <param name="entityManager"></param>
    /// <param name="eventManager"></param>
    /// <param name="renderWindow"></param>
    /// <exception cref="ArgumentNullException">
    ///   entityManager is null.
    ///   -or-
    ///   eventManager is null.
    ///   -or-
    ///   renderWindow is null.
    /// </exception>
    public RenderManager(IEntityManager entityManager, 
      IEventManager eventManager, IRenderWindow renderWindow)
    {
      if (entityManager == null) 
        throw new ArgumentNullException("entityManager");
      if (eventManager == null) throw new ArgumentNullException("eventManager");
      if (renderWindow == null) throw new ArgumentNullException("renderWindow");

      m_entityManager = entityManager;
      m_eventManager = eventManager;
      m_renderWindow = renderWindow;

      CanPause = true;
      TargetFrameRate = DefaultFrameRate;
      BackgroundColor = DefaultBackgroundColor;
      View = new View
      {
        Center = new Vector2f(0, 0),
        Viewport = new FloatRect(0, 0, 1, 1)
      };
    }

    #region IManager

    public bool CanPause { get; private set; }

    public bool Paused { get; set; }

    public bool Initialize()
    {
      Log.Verbose("RenderManager Initializing");

      return true;
    }

    public bool PostInitialize()
    {
      Log.Verbose("RenderManager Post-Initializing");

      m_eventManager.AddListener<EntityAddedEvent>(HandleEntityAdded);
      m_eventManager.AddListener<ViewDragEvent>(HandleViewDrag);
      m_eventManager.AddListener<ViewZoomEvent>(HandleViewZoom);

      m_renderWindow.Resized += HandleWindowResized;

      UpdateViewSize();
      return true;
    }
    
    public void Update(float deltaTime)
    {
      if (Paused)
      {
        return;
      }

      m_timeSinceLastRender += deltaTime;
      if (m_timeSinceLastRender < UpdateInterval)
      {
        return;
      }

      m_timeSinceLastRender -= UpdateInterval;
      m_renderWindow.SetView(View);
      DrawOneFrame(m_renderWindow);
      m_renderWindow.Display();
    }

    public void Shutdown()
    {
      Log.Verbose("RenderManager Shutting Down");

      m_eventManager.RemoveListener<EntityAddedEvent>(HandleEntityAdded);
      m_eventManager.RemoveListener<ViewDragEvent>(HandleViewDrag);
      m_eventManager.RemoveListener<ViewZoomEvent>(HandleViewZoom);

      m_renderWindow.Resized -= HandleWindowResized;
    }

    #endregion
    #region IRenderManager

    public int TargetFrameRate
    {
      get { return m_targetFrameRate; }
      set
      {
        if (value < 1) throw new ArgumentOutOfRangeException("value");

        m_targetFrameRate = value;
        UpdateInterval = 1f / value;
      }
    }

    public float UpdateInterval { get; private set; }

    public Color BackgroundColor { get; set; }

    public View View { get; private set; }

    public IReadOnlyCollection<IRenderable> Renderables
    {
      get { return m_renderables; }
    }

    public void DrawOneFrame(RenderTarget target)
    {
      if (target == null) throw new ArgumentNullException("target");

      if (m_stateChanged)
      {
        m_renderables.Sort();
        m_stateChanged = false;
      }

      target.Clear(BackgroundColor);
      foreach (var renderable in m_renderables)
      {
        renderable.Draw(target);
      }
    }

    public void AddRenderable(IRenderable renderable)
    {
      if (renderable == null) throw new ArgumentNullException("renderable");
      if (renderable.RenderId != 0 && 
          m_renderables.Any(r => r.RenderId == renderable.RenderId))
        throw new InvalidOperationException(string.Format(
          "IRenderable {0} is already tracked", renderable.RenderId));

      if (renderable.RenderId == 0)
      {
        renderable.RenderId = NextRenderId;
      }

      m_renderables.Add(renderable);
      m_stateChanged = true;
    }

    public void RemoveRenderable(IRenderable renderable)
    {
      if (renderable == null) throw new ArgumentNullException("renderable");

      // removal doesn't reorder, so doesn't count as a state change
      m_renderables.Remove(renderable);
    }

    #endregion

    private void AddRenderablesFromEntity(Entity entity)
    {
      var components = entity.GetComponentsByBase<IRenderable>();
      foreach (var component in components)
      {
        AddRenderable(component);
      }

      Log.DebugFmtIf(components.Count > 0,
        "Added {0} IRenderables from {1}", components.Count, entity.Name);
    }

    private void RemoveRenderablesFromEntity(Entity entity)
    {
      var components = entity.GetComponentsByBase<IRenderable>();
      var notFound = m_renderables.RemoveAllItems(components,
            (r1, r2) => r1.RenderId == r2.RenderId);
      // a destroyed entity may have already had its renderables removed
      Log.ErrorFmtIf(notFound.Any() && !entity.IsDestroyed,
        "Components not found during removal: {0}",
        string.Join(",", notFound));

      Log.DebugFmtIf(components.Count > 0,
        "Removed {0} IRenderables from {1}", components.Count, entity.Name);
    }

    private void UpdateViewSize()
    {
      var ratio = (float)m_renderWindow.Size.Y / m_renderWindow.Size.X;
      View.Size = new Vector2f(m_viewWidth, m_viewWidth * ratio);
    }

    #region Event Handlers

    private void HandleEntityAdded(EventBase e)
    {
      var evt = (EntityAddedEvent) e;
      Entity entity;
      if (!m_entityManager.TryGetEntity(evt.EntityId, out entity))
      {
        Log.ErrorFmt("Entity {0} not found", evt.EntityId);
        return;
      }

      entity.Activated += HandleEntityActivated;
      entity.DeActivated += HandleEntityDeActivated;
      entity.Destroyed += HandleEntityDestroyed;

      if (entity.IsActive)
      {
        AddRenderablesFromEntity(entity);
      }
    }

    private void HandleEntityActivated(object sender, EventArgs e)
    {
      var entity = (Entity)sender;
      AddRenderablesFromEntity(entity);
    }

    private void HandleEntityDeActivated(object sender, EventArgs e)
    {
      var entity = (Entity)sender;
      RemoveRenderablesFromEntity(entity);
    }

    private void HandleEntityDestroyed(object sender, EventArgs e)
    {
      var entity = (Entity)sender;
      RemoveRenderablesFromEntity(entity);

      entity.Activated -= HandleEntityActivated;
      entity.DeActivated -= HandleEntityDeActivated;
      entity.Destroyed -= HandleEntityDestroyed;
    }

    private void HandleViewDrag(EventBase e)
    {
      var evt = (ViewDragEvent) e;
      var delta = evt.Delta;
      var size = View.Size;
      View.Center += new Vector2f(delta.X * size.X, delta.Y * size.Y);
    }

    private void HandleViewZoom(EventBase e)
    {
      var evt = (ViewZoomEvent) e;
      m_viewWidth += -evt.Delta * (m_viewWidth / ZoomPercentIncrement);
      m_viewWidth = Math.Max(m_viewWidth, MinViewWidth);
      UpdateViewSize();
    }

    private void HandleWindowResized(object sender, SizeEventArgs e)
    {
      UpdateViewSize();
    }

    #endregion
  }
}
