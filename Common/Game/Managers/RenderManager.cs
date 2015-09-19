using System;
using System.Collections.Generic;
using System.Reflection;
using Common.Game.Events;
using Common.Game.Events.EntityEvents;
using Common.Game.Interfaces;
using log4net;
using SFML.Graphics;

namespace Common.Game.Managers
{
  public class RenderManager
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

    #endregion

    private void HandleEntityAdded(EventBase e)
    {
      var evt = (EntityAddedEvent) e;
      // TODO: Check for renderables to add
    }

    private void HandleEntityRemoved(EventBase e)
    {
      var evt = (EntityRemovedEvent) e;
      // TODO: Check for renderables to remove
    }
  }
}
