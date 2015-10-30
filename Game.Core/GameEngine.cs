using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common.Extensions;
using Game.Core.Events;
using Game.Core.Events.Input;
using Game.Core.Interfaces;
using Game.Core.Managers;
using Game.Core.SFML;
using log4net;
using SFML.Window;

namespace Game.Core
{
  /// <summary>
  ///   The core engine class.  Allows for operation in headless mode, or with
  ///   output to a UI window.  Automatically creates the managers from 
  ///   <see cref="Game.Core.Managers"/>, and allows additional managers to be
  ///   added.
  /// 
  ///   Note: RenderManager is not created in headless mode.
  /// </summary>
  /// <remarks>
  ///   No unit testing here, not worth the trouble of making everything work
  ///   with mocking.
  /// </remarks>
  [ExcludeFromCodeCoverage]
  public class GameEngine
  {
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    public const uint AntialiasingLevel = 8;
    // target 60 fps, allowing 1 ms to sleep
    public const double FrameTimeLimit = (1.0 / 60.0) - 0.001;
    // the frame time used when real time usage is disabled
    public const float FixedFrameTime = 1f / 30f;

    private readonly IDrawingWindow m_drawingWindow;
    private readonly List<IManager> m_managers = new List<IManager>();
    private bool m_paused = false;

    /// <summary>
    ///   Create the engine in headless mode.
    /// </summary>
    public GameEngine()
    {
      // TODO: inject configuration or something?

      LimitCpuUsage = true;
      UseWallTime = true;
      StartPaused = false;

      // insert managers, in the order they will be initialized
      EventManager = new EventManager();
      m_managers.Add(EventManager);
      PhysicsManager = new PhysicsManager();
      m_managers.Add(PhysicsManager);
      ProcessManager = new ProcessManager();
      m_managers.Add(ProcessManager);
      // TODO: entity manager!
      //EntityManager = new EntityManager(...);
      m_managers.Add(EntityManager);
    }

    /// <summary>
    ///   Create the engine with rendering output to a UI window.
    /// </summary>
    /// <param name="drawingWindow">
    ///   The window the game will draw to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   drawingWindow is null.
    /// </exception>
    public GameEngine(IDrawingWindow drawingWindow)
      : this()
    {
      if (drawingWindow == null) 
        throw new ArgumentNullException("drawingWindow");

      m_drawingWindow = drawingWindow;

      var renderWindow = new RenderWindow(drawingWindow.DrawingPanelHandle,
        new ContextSettings { AntialiasingLevel = AntialiasingLevel });
      RenderManager = new RenderManager(EntityManager, EventManager,
        renderWindow);
      m_managers.Add(RenderManager);
    }

    /// <summary>
    ///   If you want the engine to process application events as part of the
    ///   game loop, set this action to the appropriate function.
    /// </summary>
    public Action ApplicationEventHook { get; set; }

    /// <summary>
    ///   If true, the engine will sleep to limit CPU usage if a frame is 
    ///   completed faster than the target time.
    /// </summary>
    /// <remarks>
    ///   Default: true
    /// </remarks>
    public bool LimitCpuUsage { get; set; }

    /// <summary>
    ///   When true, the engine uses elapsed wall time to drive the game.  When
    ///   false, a fixed time step is used to drive the engine as fast as it
    ///   can process.
    /// 
    ///   Note: It's highly recommended to set <see cref="LimitCpuUsage"/> to 
    ///   false when setting this property to false.
    /// </summary>
    /// <remarks>
    ///   Default: true
    /// </remarks>
    public bool UseWallTime { get; set; }

    /// <summary>
    ///   If true, the engine pauses all managers after initialization.
    /// 
    ///   Has no effect after Run() is called.
    /// </summary>
    /// <remarks>
    ///   Default: false
    /// </remarks>
    public bool StartPaused { get; set; }

    /// <summary>
    ///   Get or set the paused state of the game.
    /// </summary>
    public bool IsPaused
    {
      get { return m_paused; }
      set
      {
        // very important:
        // without this check we'd get in an infinite loop of pause events
        // and pause state updates
        if (value == m_paused)
        {
          return;
        }

        m_paused = value;
        UpdatePausedState();
      }
    }

    /// <summary>
    ///   When set to true, the engine will shut down.
    /// </summary>
    public bool Stop { get; set; }

    /// <summary>
    ///   If true, the engine is running in headless mode.
    /// </summary>
    public bool IsHeadless
    {
      get { return RenderManager == null; }
    }

    /// <summary>
    ///   Get the engine entity manager.
    /// </summary>
    public IEntityManager EntityManager { get; private set; }

    /// <summary>
    ///   Get the game event Manager.
    /// </summary>
    public IEventManager EventManager { get; private set; }

    /// <summary>
    ///   Get the game physics manager.
    /// </summary>
    public IPhysicsManager PhysicsManager { get; private set; }

    /// <summary>
    ///   Get the game process manager.
    /// </summary>
    public IProcessManager ProcessManager { get; private set; }

    /// <summary>
    ///   Get the game render manager.
    /// </summary>
    public IRenderManager RenderManager { get; private set; }
    
    /// <summary>
    ///   Get a manager based on its type.  The engine contains only one manager
    ///   of each type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetManager<T>()
      where T : IManager
    {
      return m_managers.OfType<T>().SingleOrDefault();
    }

    /// <summary>
    ///   Adds an manager to the engine in addition to the default core 
    ///   managers.
    /// 
    ///   Note that added managers are initialized and updated in the order they
    ///   are added, always after the core managers.  They will be shut down in
    ///   the reverse of that order.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="manager"></param>
    /// <exception cref="ArgumentNullException">
    ///   manager is null 
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The engine already has a manager with the same type as manager.
    /// </exception>
    public void AddExtraManager<T>(T manager)
      where T : IManager
    {
      if (manager == null) throw new ArgumentNullException("manager");
      if (GetManager<T>() != null) 
        throw new InvalidOperationException(string.Format(
          "GameEngine already has manager type {0}", manager.GetType().FullName));

      m_managers.Add(manager);
    }

    /// <summary>
    ///   Initialize the game and run the main loop until the engine is stopped.
    ///   The engine stops when <see cref="Stop"/> is set, or if it is drawing 
    ///   to a UI, when the drawing window closes.
    /// </summary>
    public void Run()
    {
      if (!Initialize() || !PostInitialize())
      {
        return;
      }

      var frameTimer = new Stopwatch();
      while (!StopEngine())
      {
        var lastFrameTime = UseWallTime
          ? (float) frameTimer.Elapsed.TotalSeconds
          : FixedFrameTime;
        frameTimer.Restart();

        if (ApplicationEventHook != null)
        {
          ApplicationEventHook();
        }

        foreach (var manager in m_managers)
        {
          manager.Update(lastFrameTime);
        }

        if (LimitCpuUsage && (frameTimer.Elapsed.TotalSeconds < FrameTimeLimit))
        {
          Thread.Sleep(1);
        }
      }

      Shutdown();
    }

    #region Private Methods

    private bool Initialize()
    {
      Log.Verbose("GameEngine Initializing");

      foreach (var manager in m_managers)
      {
        if (!manager.Initialize())
        {
          Log.ErrorFmt("{0} failed initialization, aborting", 
            manager.GetType().FullName);
          return false;
        }
      }

      return true;
    }

    private bool PostInitialize()
    {
      Log.Verbose("GameEngine Post-Initializing");

      foreach (var manager in m_managers)
      {
        if (!manager.PostInitialize())
        {
          Log.ErrorFmt("{0} failed post-initialization, aborting",
            manager.GetType().FullName);
          return false;
        }
      }

      EventManager.AddListener<GamePausedEvent>(HandleGamePaused);

      IsPaused = StartPaused;
      return true;
    }

    private void Shutdown()
    {
      Log.Verbose("GameEngine Shutting Down");

      var reverseManagers = ((IEnumerable<IManager>) m_managers).Reverse();
      foreach (var manager in reverseManagers)
      {
        manager.Shutdown();
      }
    }

    private void UpdatePausedState()
    {
      foreach (var manager in m_managers.Where(m => m.CanPause))
      {
        manager.Paused = m_paused;
      }

      EventManager.TriggerEvent(new GamePausedEvent { IsPaused = IsPaused });
    }

    private bool StopEngine()
    {
      return Stop || (!IsHeadless && !m_drawingWindow.Visible);
    }

    #endregion
    #region Event Handlers

    private void HandleGamePaused(EventBase e)
    {
      var evt = (GamePausedEvent) e;
      IsPaused = evt.IsPaused;
    }

    #endregion
  }
}
