using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Common.Game.Components;
using Common.Game.Events;
using Common.Game.Events.EntityEvents;
using Common.Game.Interfaces;
using log4net;
using SFML.Graphics;

namespace Common.Game.Managers
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
    private readonly IEventManager m_eventManager;
    private readonly RenderWindow m_window;
    
    private readonly List<IRenderable> m_renderables = 
      new List<IRenderable>(InitialListSize);
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
    /// <param name="eventManager"></param>
    /// <param name="window"></param>
    /// <exception cref="ArgumentNullException">
    ///   eventManager is null.
    ///   -or-
    ///   window is null.
    /// </exception>
    public RenderManager(IEventManager eventManager, RenderWindow window)
    {
      if (eventManager == null) throw new ArgumentNullException("eventManager");
      if (window == null) throw new ArgumentNullException("window");

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
      m_eventManager.AddListener<EntityRemovedEvent>(HandleEntityRemoved);
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
      m_eventManager.RemoveListener<EntityRemovedEvent>(HandleEntityRemoved);
    }

    #endregion
    #region IRenderManager

    public void DrawOneFrame(RenderTarget target)
    {
      if (target == null) throw new ArgumentNullException("target");

      if (m_stateChanged)
      {
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
      if (renderable.Id != 0 && m_renderables.Any(r => r.Id == renderable.Id))
        throw new InvalidOperationException(string.Format(
          "IRenderable {0} is already tracked", renderable.Id));

      if (renderable.Id == 0)
      {
        renderable.Id = NextId;
      }

      m_renderables.Add(renderable);
      m_stateChanged = true;
    }

    public void RemoveRenderable(IRenderable renderable)
    {
      if (renderable == null) throw new ArgumentNullException("renderable");

      m_renderables.RemoveAll(r => r.Id == renderable.Id);
    }

    #endregion

    private void HandleEntityAdded(EventBase e)
    {
      var evt = (EntityAddedEvent) e;

      // TODO: Get the entity
      var entity = new Entity(-1);
      var components = entity.GetComponentsByBase<RenderComponentBase>();

      foreach (var component in components)
      {
        component.Activated += HandleComponentActivated;
        component.DeActivated += HandleComponentDeActivated;

        if (component.IsActive)
        {
          AddRenderable(component);
        }
      }

      Log.VerboseFmtIf(components.Count > 0,
        "Tracking {0} IRenderables from {1}", components.Count, entity.Name);
    }
    
    private void HandleEntityRemoved(EventBase e)
    {
      var evt = (EntityRemovedEvent) e;

      // TODO: Get the entity
      var entity = new Entity(-1);
      var components = entity.GetComponentsByBase<RenderComponentBase>();

      foreach (var component in components)
      {
        component.Activated -= HandleComponentActivated;
        component.DeActivated -= HandleComponentDeActivated;
        RemoveRenderable(component);
      }

      Log.VerboseFmtIf(components.Count > 0,
        "Cleared {0} IRenderables from {1}", components.Count, entity.Name);
    }

    private void HandleComponentActivated(EntityLifeCycleBase sender)
    {
      var renderable = (IRenderable) sender;
      AddRenderable(renderable);
    }

    private void HandleComponentDeActivated(EntityLifeCycleBase sender)
    {
      var renderable = (IRenderable) sender;
      RemoveRenderable(renderable);
    }
  }
}
