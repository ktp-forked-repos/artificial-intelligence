using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Game.Core.Components;
using Game.Core.Events;
using Game.Core.Events.EntityEvents;
using Game.Core.Interfaces;
using log4net;
using SFML.Graphics;

namespace Game.Core.Managers
{
  /// <summary>
  ///   The general render manager implementation.  Renders to a SFML 
  ///   RenderWindow
  /// </summary>
  public sealed class RenderManager
    : IRenderManager
  {
    private const int TargetFrameRate = 60;
    private const float UpdateInterval = 1f / TargetFrameRate;
    private const int InitialListSize = 10;

    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);
    private static readonly Color ClearColor = Color.White;

    // dependencies
    private readonly IEntityManager m_entityManager;
    private readonly IEventManager m_eventManager;
    private readonly RenderWindow m_window;
    
    private readonly List<IRenderable> m_renderables = 
      new List<IRenderable>(InitialListSize);
    private readonly List<int> m_toRemove = 
      new List<int>(InitialListSize); 
    private bool m_stateChanged = false;
    private float m_timeSinceLastRender = 0f;
    private int m_nextId = 1;

    // renderables with the default id of 0 get an id from this
    private int NextId
    {
      get { return m_nextId++; }
    }

    /// <summary>
    ///   Create the render manager.
    /// </summary>
    /// <param name="entityManager"></param>
    /// <param name="eventManager"></param>
    /// <param name="window"></param>
    /// <exception cref="ArgumentNullException">
    ///   entityManager is null.
    ///   -or-
    ///   eventManager is null.
    ///   -or-
    ///   window is null.
    /// </exception>
    public RenderManager(IEntityManager entityManager, 
      IEventManager eventManager, RenderWindow window)
    {
      if (entityManager == null) 
        throw new ArgumentNullException("entityManager");
      if (eventManager == null) throw new ArgumentNullException("eventManager");
      if (window == null) throw new ArgumentNullException("window");

      m_entityManager = entityManager;
      m_eventManager = eventManager;
      m_window = window;
      CanPause = true;
    }

    #region IManager

    public bool CanPause { get; private set; }

    public bool Paused { get; set; }

    public bool Initialize()
    {
      return true;
    }

    public bool PostInitialize()
    {
      m_eventManager.AddListener<EntityAddedEvent>(HandleEntityAdded);
      return true;
    }

    public void Update(float deltaTime, float maxTime)
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
      DrawOneFrame(m_window);
      m_window.Display();
    }

    public void Shutdown()
    {
      m_eventManager.RemoveListener<EntityAddedEvent>(HandleEntityAdded);
    }

    #endregion
    #region IRenderManager

    public void DrawOneFrame(RenderTarget target)
    {
      if (target == null) throw new ArgumentNullException("target");

      if (m_stateChanged)
      {
        ProcessPendingRemovals();
        m_renderables.Sort();
        m_stateChanged = false;
      }

      target.Clear(ClearColor);
      foreach (var renderable in m_renderables)
      {
        renderable.Draw(target);
      }
    }

    public void AddRenderable(IRenderable renderable)
    {
      if (renderable == null) throw new ArgumentNullException("renderable");
      if (renderable.RenderId != 0 && m_renderables.Any(r => r.RenderId == renderable.RenderId))
        throw new InvalidOperationException(string.Format(
          "IRenderable {0} is already tracked", renderable.RenderId));

      if (renderable.RenderId == 0)
      {
        renderable.RenderId = NextId;
      }

      m_renderables.Add(renderable);
      m_stateChanged = true;
    }

    public void RemoveRenderable(IRenderable renderable)
    {
      if (renderable == null) throw new ArgumentNullException("renderable");

      m_toRemove.Add(renderable.RenderId);
      m_stateChanged = true;
    }

    #endregion

    private void ProcessPendingRemovals()
    {
      if (m_toRemove.Count == 0)
      {
        return;
      }

      var remaining = m_renderables.RemoveAllItems(m_toRemove,
        (renderable, id) => renderable.RenderId == id);
      Debug.Assert(!remaining.Any());
      m_toRemove.Clear();
    }

    #region Event Handlers

    private void HandleEntityAdded(EventBase e)
    {
      var evt = (EntityAddedEvent) e;
      Entity entity;
      if (m_entityManager.TryGetEntity(evt.EntityId, out entity))
      {
        Log.ErrorFmt("Entity {0} not found", evt.EntityId);
        return;
      }

      var components = entity.GetComponentsByBase<RenderComponentBase>();
      foreach (var component in components)
      {
        component.Activated += HandleComponentActivated;
        component.DeActivated += HandleComponentDeActivated;
        component.Destroyed += HandleComponentDestroyed;

        if (component.IsActive)
        {
          AddRenderable(component);
        }
      }

      Log.VerboseFmtIf(components.Count > 0,
        "Tracking {0} IRenderables from {1}", components.Count, entity.Name);
    }

    private void HandleComponentActivated(object sender, EventArgs e)
    {
      var renderable = (IRenderable) sender;
      AddRenderable(renderable);
    }

    private void HandleComponentDeActivated(object sender, EventArgs e)
    {
      var renderable = (IRenderable) sender;
      RemoveRenderable(renderable);
    }

    private void HandleComponentDestroyed(object sender, EventArgs e)
    {
      var renderable = (IRenderable) sender;
      RemoveRenderable(renderable);
    }

    #endregion
  }
}
