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

      m_toRemove.Add(renderable.Id);
      m_stateChanged = true;
    }

    #endregion

    private void ProcessPendingRemovals()
    {
      if (m_toRemove.Count == 0)
      {
        return;
      }

      // track the number left to remove
      var numToRemove = m_toRemove.Count;

      // iterate backwards over the renderable list so we can remove as we go
      for (var renderIdx = m_renderables.Count - 1; renderIdx >= 0; renderIdx--)
      {
        var renderable = m_renderables[renderIdx];

        // iterate over renderables waiting to be removed
        for (var removeIdx = 0; removeIdx < numToRemove; removeIdx++)
        {
          if (m_toRemove[removeIdx] != renderable.Id)
          {
            continue;
          }

          // swap & pop, but with just an overwrite instead of swap
          m_renderables.RemoveAt(renderIdx);
          m_toRemove[removeIdx] = m_toRemove[numToRemove - 1];
          numToRemove--;
          break;
        }
      }

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
